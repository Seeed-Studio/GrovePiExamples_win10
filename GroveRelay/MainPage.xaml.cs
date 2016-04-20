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

using System.Threading;
using GrovePi;
using GrovePi.Sensors;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace GroveRelay
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Timer periodicTimer;

        private static int RelayOnOff = 0;

        /* Define Grove Modules object */

        IRelay GroveRelay = DeviceFactory.Build.Relay(Pin.DigitalPin2);
       

        public MainPage()
        {
            this.InitializeComponent();

            InitGrovePi();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);

        }

        private void InitGrovePi()
        {
          
        }

        private void TimerCallBack(object state)
        {
            if (RelayOnOff == 0)
            {
                RelayOnOff = 1;
                GroveRelay.ChangeState(SensorStatus.On);

            }
            else
            {
                RelayOnOff = 0;
                GroveRelay.ChangeState(SensorStatus.Off);
            }

        }
    }
}
