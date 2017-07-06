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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeepPilotCsharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MenuSmall = true;
        public MainWindow()
        {
            InitializeComponent();
            TabHome.Visibility = Visibility.Collapsed;
            TabSettings.Visibility = Visibility.Collapsed;
            TabRaw.Visibility = Visibility.Collapsed;
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
    }
}
