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
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ultrasonic_LCD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string ultrasonic_last = "";
        private Timer periodicTimer;
        // Connect the Ultrasonic Sensor to digital port 7
        IUltrasonicRangerSensor ultrasonic = DeviceFactory.Build.UltraSonicSensor(Pin.DigitalPin7);
        // Connect the RGB display to one of the I2C ports.
        IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();

        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 200);
        }

        private void TimerCallBack(object state)
        {
            string value = ultrosonic();
            if (ultrasonic_last != value)
            {
                lcd_display(value);
            }
            ultrasonic_last = value;

            /* UI updates must be invoked on the UI thread */
            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Text_distance.Text = "Distance: " + ultrasonic_last + "cm";
            });
        }

        private void Init_Grovepi()
        {

        }

        private void lcd_display(string str)
        {               
            try
            {
                display.SetText("Distance: " + str + "cm").SetBacklightRgb(0, 250, 20);               

            }
            catch (Exception ex)
            {
                // If you want to see the exceptions uncomment the following:
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }            
        }

        private string ultrosonic()
        {
            string sensorvalue = "";

            try
            {
                // Check the value of the Ultrasonic Sensor
                sensorvalue = ultrasonic.MeasureInCentimeters().ToString();
                System.Diagnostics.Debug.WriteLine("Ultrasonic value " + sensorvalue);
            }
            catch (Exception ex)
            {
                // If you want to see the exceptions uncomment the following:
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return sensorvalue;
        }
    }
}
