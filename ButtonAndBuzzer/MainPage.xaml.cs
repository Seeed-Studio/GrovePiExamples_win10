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
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ButtonAndBuzzer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Timer periodicTimer;
        private static UInt16 flag = 0;
        int RelayOnOff = 0;
        /* Define Grove Modules name */
        IButtonSensor GroveButton = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin7);
        IBuzzer GroveBuzzer = DeviceFactory.Build.Buzzer(Pin.DigitalPin6);


        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 100);
        }

        private void InitGrovePi()
        {
            DeviceFactory.Build.GrovePi().PinMode(Pin.DigitalPin6, PinMode.Output);
        }

        private void TimerCallBack(object state)
        {
            if (GroveButton.CurrentState == SensorStatus.On)
            {
                if (flag == 0)
                {
                    flag = 1;
                    GroveBuzzer.ChangeState(SensorStatus.Off);
                }
                else {
                    flag = 0;
                    GroveBuzzer.ChangeState(SensorStatus.On);
                }
                Task.Delay(100).Wait();
            }
        }
    }
}