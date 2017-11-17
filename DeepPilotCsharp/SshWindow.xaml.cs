using Renci.SshNet;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DeepPilotCsharp
{
    /// <summary>
    /// Interaction logic for SshWindow.xaml
    /// </summary>
    public partial class SshWindow : Window
    {
        private SshClient _sshClient;
        private string _IPAddress;
        private string _user;
        private string _pass;

        public SshWindow(string IPAddress, string user, string pass)
        {
            InitializeComponent();
            _IPAddress = IPAddress;
            _user = user;
            _pass = pass;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_sshClient != null)
            {
                _sshClient.Disconnect();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hideAllControll();
            lbLoad.Visibility = Visibility.Visible;
            Task.Run(() =>
            {
                _sshClient = new SshClient(_IPAddress, _user, _pass);
                _sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(20);
                try
                {
                    _sshClient.Connect();
                    if (_sshClient.IsConnected)
                    {
                        Dispatcher.Invoke(() => { tbOutput.Text += ("Connecting successful!\n"); }, DispatcherPriority.Normal);
                    }
                    else
                    {
                        Dispatcher.Invoke(() => { tbOutput.Text += ("Connecting fail!\n"); }, DispatcherPriority.Normal);
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => { tbOutput.Text += (ex.Message); }, DispatcherPriority.Normal);
                }
                Dispatcher.Invoke(() => {
                    lbLoad.Visibility = Visibility.Hidden;
                    showAllControll();
                }, DispatcherPriority.Normal);
            });
        }

        private void btRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_sshClient.IsConnected)
                {
                    var s = _sshClient.RunCommand(tbCommand.Text);
                    tbOutput.Text += "---" + DateTime.Now.ToString() + "\n";
                    if (s.Error != "")
                    {
                        tbOutput.Text += "---Error---\n" + s.Error + "---Error---\n";
                    }
                    else
                    {
                        tbOutput.Text += "---Get---\n" + s.Result + "---Get---\n";
                    }
                }
                else
                {
                    tbOutput.Text += ("No connected!\n");
                }
            }
            catch (Renci.SshNet.Common.SshConnectionException ex)
            {
                tbOutput.Text += "Server closed the connection!\n" + ex.Message + "\n";
            }
            catch (Exception ex)
            {
                tbOutput.Text += ex.Message + "\n";
            }
            tbOutput.ScrollToEnd();
        }

        private void btReConn_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(null, null);
        }

        private void hideAllControll()
        {
            tbCommand.Visibility = Visibility.Hidden;
            tbOutput.Visibility = Visibility.Hidden;
            btReConn.Visibility = Visibility.Hidden;
            btRun.Visibility = Visibility.Hidden;
            lbCommand.Visibility = Visibility.Hidden;
            lbOutput.Visibility = Visibility.Hidden;
        }

        private void showAllControll()
        {
            tbCommand.Visibility = Visibility.Visible;
            tbOutput.Visibility = Visibility.Visible;
            btReConn.Visibility = Visibility.Visible;
            btRun.Visibility = Visibility.Visible;
            lbCommand.Visibility = Visibility.Visible;
            lbOutput.Visibility = Visibility.Visible;
        }

        private void tbCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btRun_Click(null, null);
            }
        }
    }
}
