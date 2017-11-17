using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForm = System.Windows.Forms;

namespace DeepPilotCsharp.Cam
{
    /// <summary>
    /// Interaction logic for CamViewControl.xaml
    /// </summary>
    public partial class CamViewControl : UserControl
    {
        public delegate void GetImages(Bitmap image);
        public event GetImages Images;

        private CamTCP tcp;
        private double zoom = 1;
        private WinForm.PictureBox _pictureBox;

        public CamViewControl(string IPAddress, int port)
        {
            InitializeComponent();
            _pictureBox = CamCanvasHost.Child as WinForm.PictureBox;
            tcp = new CamTCP(IPAddress, port);
            tcp.Images += Tcp_Images;
            tcp.DoWork();
        }

        private void Tcp_Images(Bitmap bm)
        {
            if (zoom != 0)
            {
                _pictureBox.Image = new Bitmap(bm, new System.Drawing.Size((int)(bm.Width * zoom), (int)(bm.Height * zoom)));
                Images?.Invoke(bm);
            }
        }

        public void Stop()
        {
            if (tcp != null)
            {
                tcp.Stop();
            }
        }

        public void SetZoom(double zoom)
        {
            this.zoom = zoom;
        }

        public double GetZoom() => this.zoom;
    }
}
