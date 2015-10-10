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


//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Blink
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        private static UInt16 flag = 0;
        private static readonly IBuildGroveDevices Blink = DeviceFactory.Build;
        //private static readonly ILed led;
        private Timer periodicTimer;
        public MainPage()
        {
            this.InitializeComponent();
            periodicTimer = new Timer(this.TimerCallBack, null, 0, 1000);
        }
        private void TimerCallBack(object state)
        {
            rpiRun();
           
            //IotupLoadData();
        }
        private void rpiRun() {
            //DeviceFactory.Build.RgbLcdDisplay().SetText("GrovePi Demo\nby SeeedStudio").SetBacklightRgb(0, 255, 255);
            if (flag == 0)
            {
                flag = 1;
                DeviceFactory.Build.Led(Pin.DigitalPin4).ChangeState(SensorStatus.On);
            }
            else {
                DeviceFactory.Build.Led(Pin.DigitalPin4).ChangeState(SensorStatus.Off);
                flag = 0;
            }
            
        }
    }
}
