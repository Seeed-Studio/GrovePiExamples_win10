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
using System.Threading;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeWeatherDisplay
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly IBuildGroveDevices HomeWeatherDisplay = DeviceFactory.Build;
        private Timer periodicTimer;
        private String temperature = "";
        private String humidity = "";
        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }
        private void TimerCallBack(object state)
        {
            var tmp = DeviceFactory.Build.TemperatureAndHumiditySensor(Pin.AnalogPin1, Model.OnePointOne).TemperatureInCelcius();
            temperature = "temp: " + tmp.ToString("F2") + "C";
            DeviceFactory.Build.RgbLcdDisplay().SetText(temperature).SetBacklightRgb(0, 255, 255);
            //IotupLoadData();
        }
    }
}
