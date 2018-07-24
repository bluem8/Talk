using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkClient
{
    public partial class ImageForm : Form
    {
        public ImageForm(ref Bitmap img)
        {
            InitializeComponent();

            try
            {
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.BackgroundImage = (Bitmap)img.Clone();
                this.FormClosing += ImageForm_FormClosing;
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.MouseClick += ImageForm_MouseClick;
            }
            catch (Exception e)
            {
            }
        }

        private void ImageForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void ImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.BackgroundImage.Dispose();
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {

        }
    }
}
