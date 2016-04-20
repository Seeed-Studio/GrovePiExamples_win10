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

namespace GroveTempAndHumi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Timer periodicTimer;
        private String SGroveTempSensor;
        private String SGroveHumiditySensor;

        IDHTTemperatureAndHumiditySensor GroveTempHumi = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin3,DHTModel.Dht11);
        

        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }

        private void TimerCallBack(object state)
        {
            rpiRun();
        }

        private void rpiRun()
        {
            try
            {
                GroveTempHumi.Measure();
                var tmp = GroveTempHumi.TemperatureInCelsius;
                SGroveTempSensor = "Temperature: " + tmp.ToString() + "℃";
                var tmp1 = GroveTempHumi.Humidity;
                SGroveHumiditySensor = "Humidity: " + tmp1.ToString() + "%";
            }
            catch (Exception ex)
            {

            }

            var UItask = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                GroveHumidityUI.Text = SGroveHumiditySensor;
                GroveTempUI.Text = SGroveTempSensor;

            });
        }

    }
}
