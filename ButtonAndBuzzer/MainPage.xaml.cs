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

namespace ButtonAndBuzzer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly IBuildGroveDevices ButtonAndBuzzer = DeviceFactory.Build;
        private Timer periodicTimer;
        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 50);
        }
        private void TimerCallBack(object state)
        {
            if (DeviceFactory.Build.ButtonSensor(Pin.DigitalPin4).CurrentState == SensorStatus.On)
            {
                DeviceFactory.Build.Buzzer(Pin.DigitalPin2).ChangeState(SensorStatus.On);
            }
            else {
                DeviceFactory.Build.Buzzer(Pin.DigitalPin2).ChangeState(SensorStatus.Off);
            }
        }
    }
}
