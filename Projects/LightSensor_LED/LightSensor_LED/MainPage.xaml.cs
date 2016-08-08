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
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LightSensor_LED
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int darkLevel = 100;  // Turn off LED when light snesor lower than darkLevel
        int lightLevel = 120; // Turn on LED when light sensor higher than lightLevel

        private Timer periodicTimer;

        // Connect Grove-LED to port D5
        ILed led = DeviceFactory.Build.Led(Pin.DigitalPin5);

        // Connect Grove-Light Sensor to port A0
        ILightSensor lightSensor = DeviceFactory.Build.LightSensor(Pin.AnalogPin0);

        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 100);
        }

        private void TimerCallBack(object state)
        {
            try
            {
                int value = getLightSensor();
                System.Diagnostics.Debug.WriteLine("Light sensor: " + value.ToString());
                if (value < darkLevel)
                {
                    led.ChangeState(SensorStatus.On);
                }
                else if(value > lightLevel)
                {
                    led.ChangeState(SensorStatus.Off);
                }

                /* UI updates must be invoked on the UI thread */
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Text_lightSensor.Text = "Light Sensor: " + value.ToString();
                });
            }
            catch (Exception ex)
            {
                // If you want to see the exceptions uncomment the following:
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private int getLightSensor()
        {
            int value = Convert.ToInt16(lightSensor.SensorValue());

            return value;
        }
    }
}
