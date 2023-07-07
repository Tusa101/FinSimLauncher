using CefSharp;
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

namespace FinSimLauncher.View
{
    /// <summary>
    /// Interaction logic for EducationPage.xaml
    /// </summary>
    public partial class EducationPage : Page
    {
        public EducationPage()
        {
            InitializeComponent();
            browser.ZoomLevelIncrement = 0.5;
        }


        private void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            browser.SetZoomLevel(-4.08);
        }
    }
}
