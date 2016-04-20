using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using GrovePi;
using GrovePi.Sensors;
using GrovePi.Common;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroveLedBar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Timer periodicTimer;

        private static UInt16 index = 0;

        // Define the Grove Modules
        ILedBar Grove_LedBar = DeviceFactory.Build.BuildLedBar(Pin.DigitalPin5);
        public MainPage()
        {
            this.InitializeComponent();
            //this.Loaded += MainPage_Loaded;
            InitGrovePi();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitGrovePi();
        }

        private void TimerCallBack(object state)
        {
            rpiRun();
        }

        private void InitGrovePi()
        {
            Grove_LedBar.Initialize(GrovePi.Sensors.Orientation.GreenToRed);
        }

        private void rpiRun()
        {
            Grove_LedBar.SetLevel((byte)index);
            if (index++ == 10) {
                index = 0;
            }
        }



    }
}
