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
using GrovePi.I2CDevices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DHT_LCD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Connect DHT Temperature and humidity Sensor to port D5
        IDHTTemperatureAndHumiditySensor sensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin5, DHTModel.Dht11);
        // Connect the RGB display to one of the I2C ports.
        IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();
        private Timer periodicTimer;

        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }

        private void TimerCallBack(object state)
        {
            try
            {
                // Check the value of the Sensor.
                // Temperature in Celsius is returned as a double type.  Convert it to string so we can print it.
                sensor.Measure();
                string sensortemp = sensor.TemperatureInCelsius.ToString();
                // Same for Humidity.  
                string sensorhum = sensor.Humidity.ToString();

                // Print all of the values to the debug window.  
                System.Diagnostics.Debug.WriteLine("Temp is " + sensortemp + " C.  And the Humidity is " + sensorhum + "%. ");
                display.SetText("Temp: " + sensortemp + "C" + "       " + "Humidity: " + sensorhum + "%").SetBacklightRgb(0, 50, 255);

                /* UI updates must be invoked on the UI thread */
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Text_Temperature.Text = "Temperature: " + sensortemp + "C";
                    Text_Humidity.Text = "Humidity: " + sensorhum + "%";
                });
            }
            catch (Exception ex) 
            {
                // NOTE: There are frequent exceptions of the following:
                // WinRT information: Unexpected number of bytes was transferred. Expected: '. Actual: '.
                // This appears to be caused by the rapid frequency of writes to the GPIO
                // These are being swallowed here/

                // If you want to see the exceptions uncomment the following:
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }       
    }
}
