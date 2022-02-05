using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageResize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listBox1.Items.Add(ImageFormat.MemoryBmp);
            listBox1.Items.Add(ImageFormat.Bmp);
            listBox1.Items.Add(ImageFormat.Jpeg);
            listBox1.Items.Add(ImageFormat.Png);

            listBox1.SelectedIndex = 2;

        }


        List<FileInfo> images;
        string savepath = "";
        ImageFormat SaveFormat;
        string ext;
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
                textBox1.Text = fd.SelectedPath;
            else
                return;

            savepath = textBox1.Text + $"_{nmWidth.Value}x{nmHeight.Value}\\";

            DirectoryInfo dir = new DirectoryInfo(textBox1.Text);
            var bmp = dir.GetFiles("*.bmp");
            var jpg = dir.GetFiles("*.jpg");
            var jpeg = dir.GetFiles("*.jpeg");
            var png = dir.GetFiles("*.png");

            images = new List<FileInfo>();
            images.AddRange(bmp);
            images.AddRange(jpg);
            images.AddRange(jpeg);
            images.AddRange(png);

            lblProgress.Text = $"0 / {images.Count}";
            progressBar.Maximum = images.Count;
            progressBar.Value = 0;

            lblState.Text = "Hazır.";

        }


        private void btnResize_Click(object sender, EventArgs e)
        {
            lblState.Text = "İşlem Başlatılıyor...";
            SaveFormat = listBox1.SelectedItem as ImageFormat;

            if (SaveFormat == ImageFormat.Bmp || SaveFormat == ImageFormat.MemoryBmp)
                ext = ".bmp";
            else if (SaveFormat == ImageFormat.Jpeg)
                ext = ".jpeg";
            else if (SaveFormat == ImageFormat.Png)
                ext = ".png";

            btnResize.Enabled = false;
            backgroundWorker.RunWorkerAsync();

        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(savepath);
            if (!dir.Exists)
                dir.Create();

            Size size = new Size((int)nmWidth.Value, (int)nmHeight.Value);

            for (int i = 0; i < images.Count; i++)
            {
                FileInfo file = images[i];
                Image img = Image.FromFile(file.FullName);

                img = ResizeImage(img, size);

                string name = file.Name.Substring(0, file.Name.LastIndexOf("."));

                img.Save($"{savepath}{name}{ext}", SaveFormat);

                backgroundWorker.ReportProgress(i, file.FullName);
            }

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblProgress.Text = $"{e.ProgressPercentage + 1} / {images.Count}";
            lblState.Text = e.UserState.ToString();
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblState.Text = "Bitti.";
            btnResize.Enabled = true;
            progressBar.Value = 0;

        }

        public static Image ResizeImage(Image img, Size newSize)
        {
            if (img.Width > newSize.Width || img.Height > newSize.Height)
            {
                return img.GetThumbnailImage(newSize.Width, newSize.Height, () => false, IntPtr.Zero);
            }
            return img;
        }

    }
}
