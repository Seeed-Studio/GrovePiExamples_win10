using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Button_Rotary_LED
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        IButtonSensor GroveButton = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin6);
        IRotaryAngleSensor GroveRotary = DeviceFactory.Build.RotaryAngleSensor(Pin.AnalogPin0);
        ILed GroveLed = DeviceFactory.Build.Led(Pin.DigitalPin5);

        bool ledStatus = false;
        int RotaryDegree = 0;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPageLoaded;
            
        }

        private void TimerCallBack(object state)
        {
            string Degree = GroveRotary.Degrees().ToString();
            System.Diagnostics.Debug.WriteLine("Degree is " + Degree);

            /* UI updates must be invoked on the UI thread */
            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Text_AngleSensor.Text = "GroveRotaryAngle: " + Degree;
                Text_LEDStatus.Text = "LED Status: " + (ledStatus ? "On" : "Off");
            });
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(async (workItem) =>
            {
                while (true)
                {
                    try
                    {
                        //led.AnalogWrite(getAnalogOut());
                        if (GroveButton.CurrentState == SensorStatus.On)
                        {
                            while (GroveButton.CurrentState == SensorStatus.On) ;
                            ledStatus = !ledStatus;
                            System.Diagnostics.Debug.WriteLine("LED light status: " + (ledStatus ? "On" : "Off"));
                        }

                        if (ledStatus)
                        {
                            RotaryDegree = Convert.ToInt16(GroveRotary.Degrees());
                            byte analogOut = (byte)(RotaryDegree * 255 / 300);
                            GroveLed.AnalogWrite(analogOut);

                        }
                        else
                        {
                            GroveLed.AnalogWrite(0);
                        }

                        // System.Diagnostics.Debug.WriteLine("AnalogOut is " + getAnalogOut());                    
                    }
                    catch (Exception ex)
                    {
                        // If you want to see the exceptions uncomment the following:
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }

                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        Text_AngleSensor.Text = "GroveRotaryAngle: " + RotaryDegree.ToString();
                        Text_LEDStatus.Text = "LED Status: " + (ledStatus ? "On" : "Off");
                    });
                }
            });
        }
    }
}
            


        
            
