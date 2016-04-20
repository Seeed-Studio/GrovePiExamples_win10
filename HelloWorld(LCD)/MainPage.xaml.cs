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
using GrovePi.I2CDevices;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWorld_LCD_
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Timer periodicTimer;

        IRgbLcdDisplay GroveLCD = DeviceFactory.Build.RgbLcdDisplay();

        public MainPage()
        {

            this.InitializeComponent();
            //DeviceFactory.Build.RgbLcdDisplay().Initialize();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }
        private void TimerCallBack(object state)
        {
            GroveLCD.SetText("Hello World\nby SeeedStudio").SetBacklightRgb(255, 50, 255);
        }
    }
}
