using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace TalkClient
{
    class ImageManager
    {
        public Bitmap GetScreenshot()
        {
            Bitmap memimg = new Bitmap(Properties.Settings.Default.ScreenshotDownsampleSizeX, Properties.Settings.Default.ScreenshotDownsampleSizeY);
            System.Drawing.Size s = new System.Drawing.Size(memimg.Width, memimg.Height);
            using (Graphics g = Graphics.FromImage(memimg))
            {
                g.InterpolationMode = (System.Drawing.Drawing2D.InterpolationMode)Properties.Settings.Default.ScreenshotInterpolationMode;
                g.CompositingQuality = (System.Drawing.Drawing2D.CompositingQuality)Properties.Settings.Default.ScreenshotCompositingQuality;
                g.CopyFromScreen(0, 0, 0, 0, s);
            }
            return memimg;
        }
    }
}
