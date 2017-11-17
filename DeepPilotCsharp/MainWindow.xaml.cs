using DeepPilotCsharp.Cam;
using DeepPilotCsharp.CarController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

namespace DeepPilotCsharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MenuSmall = true;

        //--Move controll--
        InputController _inputController;
        //------------------

        //--Cam--
        CamViewControl _cvc1;
        CamViewControl _cvc2;
        //------------------

        public MainWindow()
        {
            InitializeComponent();
            TabHome.Visibility = Visibility.Collapsed;
            TabSettings.Visibility = Visibility.Collapsed;
            TabRaw.Visibility = Visibility.Collapsed;

            //Hotkey def
            RoutedCommand rc = new RoutedCommand();
            rc.InputGestures.Add(new KeyGesture(Key.F2));
            CommandBindings.Add(new CommandBinding(rc, SshWindowOpen));

            //ReacingWheel and keyboard controller
            _inputController = new InputController();
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MenuSmall)
            {
                MenuColumn.Width = new GridLength(100, GridUnitType.Auto);
                MenuSmall = false;
            }
            else
            {
                MenuColumn.Width = new GridLength(44, GridUnitType.Pixel);
                MenuSmall = true;
            }
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            TabMain.SelectedIndex = 0;
            BtnHome.Foreground = Brushes.DarkMagenta;
            BtnSett.Foreground = Brushes.Gray;
            BtnRaw.Foreground = Brushes.Gray;
        }

        private void BtnSett_Click(object sender, RoutedEventArgs e)
        {
            TabMain.SelectedIndex = 1;
            BtnHome.Foreground = Brushes.Gray;
            BtnSett.Foreground = Brushes.DarkMagenta;
            BtnRaw.Foreground = Brushes.Gray;
        }

        private void BtnRaw_Click(object sender, RoutedEventArgs e)
        {
            TabMain.SelectedIndex = 2;
            BtnHome.Foreground = Brushes.Gray;
            BtnSett.Foreground = Brushes.Gray;
            BtnRaw.Foreground = Brushes.DarkMagenta;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _inputController.ConnectToServer(Properties.Settings.Default.IPAddress, Properties.Settings.Default.MovePort);
            SelectedControllerCheck();
            Console.WriteLine("Connecting...");
            LoadSetting();
        }

        #region Setting Menu
        private void LoadSetting()
        {
            tbIP.Text = Properties.Settings.Default.IPAddress;
            tbCamPort1.Text = Properties.Settings.Default.Cam1Port.ToString();
            tbCamPort2.Text = Properties.Settings.Default.Cam2Port.ToString();
            tbMovePort.Text = Properties.Settings.Default.MovePort.ToString();
            tbRecDataPort.Text = Properties.Settings.Default.ReciveDataPort.ToString();
            tbLogLocation.Text = Properties.Settings.Default.LogLocation;
            tbFileName.Text = Properties.Settings.Default.LogFileName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ipaddress = IPAddress.Parse(tbIP.Text);
                int Camport1 = int.Parse(tbCamPort1.Text);
                int Camport2 = int.Parse(tbCamPort2.Text);
                int Moveport = int.Parse(tbMovePort.Text);
                int RecDataPort = int.Parse(tbRecDataPort.Text);
                if (!Directory.Exists(tbLogLocation.Text))
                {
                    new Exception("Folder is not avaiable!");
                }

                Properties.Settings.Default.IPAddress = ipaddress.ToString();
                Properties.Settings.Default.Cam1Port = Camport1;
                Properties.Settings.Default.Cam2Port = Camport2;
                Properties.Settings.Default.MovePort = Moveport;
                Properties.Settings.Default.ReciveDataPort = RecDataPort;
                Properties.Settings.Default.LogLocation = tbLogLocation.Text;
                Properties.Settings.Default.LogFileName = tbFileName.Text;
                Properties.Settings.Default.Save();
                LoadSetting();
                if (MessageBox.Show("Save succesful!\nWould you like reconnect?", "Mentés", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _inputController.ConnectToServer(ipaddress.ToString(), Moveport);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btFileOpen_Click(object sender, RoutedEventArgs e)
        {
            WinForm.FolderBrowserDialog fbd = new WinForm.FolderBrowserDialog()
            {
                ShowNewFolderButton = true
            };
            if (fbd.ShowDialog() == WinForm.DialogResult.OK)
            {
                tbLogLocation.Text = fbd.SelectedPath;
            }
        }
        #endregion

        #region Keyboard to drive car
        private void TabHome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                _inputController.SetForward(true);
            }
            if (e.Key == Key.S)
            {
                _inputController.SetBackward(true);
            }
            if (e.Key == Key.D)
            {
                _inputController.SetRight(true);
            }
            if (e.Key == Key.A)
            {
                _inputController.SetLeft(true);
            }
        }

        private void TabHome_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                _inputController.SetForward(false);
            }
            if (e.Key == Key.S)
            {
                _inputController.SetBackward(false);
            }

            if (e.Key == Key.D)
            {
                _inputController.SetRight(false);
            }
            if (e.Key == Key.A)
            {
                _inputController.SetLeft(false);
            }
        }
        #endregion

        #region Button for drive car
        private void BtnFwd_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetForward(true);
        }

        private void BtnFwd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetForward(false);
        }

        private void BtnBck_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetBackward(true);
        }

        private void BtnBck_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetBackward(false);
        }

        private void BtnLft_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetLeft(true);
        }

        private void BtnLft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetLeft(false);
        }

        private void BtnRgh_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetRight(true);
        }

        private void BtnRgh_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _inputController.SetRight(false);
        }
        #endregion

        #region Buttons
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (_cvc1 != null)
            {
                _cvc1.Stop();
            }
            if (_cvc2 != null)
            {
                _cvc2.Stop();
            }
            _inputController.Stop();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            _inputController.GetSend();
        }

        private void BtnCamera_Click(object sender, RoutedEventArgs e)
        {
            _inputController.GetCam();

            //Setup cam
            if (Properties.Settings.Default.IPAddress.Length > 0)
            {
                if (Properties.Settings.Default.Cam1Port != 0)
                {
                    _cvc1 = new CamViewControl(Properties.Settings.Default.IPAddress, Properties.Settings.Default.Cam1Port);
                    Cam1Tab.Content = _cvc1;
                }
                if (Properties.Settings.Default.Cam2Port != 0)
                {
                    _cvc2 = new CamViewControl(Properties.Settings.Default.IPAddress, Properties.Settings.Default.Cam2Port);
                    Cam2Tab.Content = _cvc2;
                }
            }
        }

        private void bnIPTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress address = IPAddress.Parse(tbIP.Text);
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(address);
                if (reply.Status == IPStatus.Success)
                {
                    MessageBox.Show("Connection success!");
                }
                else
                {
                    MessageBox.Show("Connection fail!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
           //For logger
        }

        #endregion

        #region Cam section
        private void SliderRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Cams != null && Cams.SelectedContent != null)
            {
                if (((TabItem)Cams.Items[Cams.SelectedIndex]).Content is CamViewControl cvc)
                {
                    cvc.SetZoom(SliderRate.Value / 5);
                }
            }
        }

        private void Cams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cams != null && Cams.SelectedContent != null)
            {
                if (((TabItem)Cams.Items[Cams.SelectedIndex]).Content is CamViewControl cvc)
                {
                    SliderRate.Value = cvc.GetZoom() * 5;
                }
            }
        }

        private void cbReacingWheel_Click(object sender, RoutedEventArgs e)
        {
            SelectedControllerCheck();
        }

        private void SelectedControllerCheck()
        {
            if (cbReacingWheel.IsChecked == true)
            {
                _inputController.DisableKeyboard();
                _inputController.AbleRaicingWheel();
            }
            else
            {
                _inputController.DisableRaicingWheel();
                _inputController.AbleKeyboard();
            }
        }
        #endregion

        private void SshWindowOpen(object sender, ExecutedRoutedEventArgs e)
        {
            SshWindow sshwindow = new SshWindow(Properties.Settings.Default.IPAddress, "pi", "Szenergy01");
            sshwindow.Show();
        }
    }
}
