using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DeepPilotCsharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                UDPSender uDPSender = new UDPSender(DeepPilotCsharp.Properties.Settings.Default.IPAddress, DeepPilotCsharp.Properties.Settings.Default.MovePort);
                uDPSender.SendText(new MessageFromComputerToRaspberry() { Exit = true }.Serialize());
                uDPSender.Close();
                MessageBox.Show("Shutdown message send to rpi!\nApplication will be close");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Environment.Exit(1);
        }
    }
}
