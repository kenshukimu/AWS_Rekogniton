using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;


namespace Aws_Rekognition
{

    public partial class photoForm : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        Image returnImage;
               
        public photoForm()
        {
            InitializeComponent();
            videoCaptureDevice = new VideoCaptureDevice();
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_newFrame;
            videoCaptureDevice.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Form1 form1 = (Form1)Owner;
            form1.ReceivedData = (Bitmap)pictureBox01.Image.Clone();

            videoCaptureDevice.Stop();
            this.Close();

        }

        private void VideoCaptureDevice_newFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox01.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox01.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void photoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice.IsRunning == true)
            {
                videoCaptureDevice.Stop();
            }
        }
    }
}
