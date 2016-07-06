using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// Add using statements to the GrovePi libraries
using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;
using GrovePi.Common;
using System.Threading.Tasks;


//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace All_in_One
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public class DataPoint
    {
        public int Time { get; set; }
        public double Value { get; set; }
    }


    public sealed partial class MainPage : Page
    {
        List<DataPoint> DistanceList;
        List<DataPoint> SoundList;
        List<DataPoint> LightList;

        int RotaryValue = 0;
        int RelayOnOff = 0;

        int LastRotaryValue = 1000;
        int LastRelayOnOff = 1000;

        double LastTemp;
        double LastHumi;

        static int DataPointCnt = 10;

        IRotaryAngleSensor GroveRotary;
        IRelay GroveRelay;
        IDHTTemperatureAndHumiditySensor GroveTempHumi;
        IUltrasonicRangerSensor GroveRanger;
        ILedBar GroveLedBar;
        IBuzzer GroveBuzzer;
        ISoundSensor GroveSound;
        ILightSensor GroveLight;
        IButtonSensor GroveButton;
        IRgbLcdDisplay GroveLCD;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitGrovePi();
            InitDataList();
            InitWorker();
            UpdataUISlow();
            UpdateUIFast();
        }

        private void InitGrovePi()
        {
            System.Diagnostics.Debug.WriteLine(DeviceFactory.Build.GrovePi().GetFirmwareVersion());

            GroveRotary = DeviceFactory.Build.RotaryAngleSensor(Pin.AnalogPin0);
            GroveSound = DeviceFactory.Build.SoundSensor(Pin.AnalogPin1);
            GroveLight = DeviceFactory.Build.LightSensor(Pin.AnalogPin2);

            GroveRelay = DeviceFactory.Build.Relay(Pin.DigitalPin2);
            GroveTempHumi = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin3, DHTModel.Dht11);
            GroveRanger = DeviceFactory.Build.UltraSonicSensor(Pin.DigitalPin4);
            GroveLedBar = DeviceFactory.Build.BuildLedBar(Pin.DigitalPin5);
            GroveBuzzer = DeviceFactory.Build.Buzzer(Pin.DigitalPin6);
            GroveButton = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin7);
            GroveLCD = DeviceFactory.Build.RgbLcdDisplay();

            GroveLedBar.Initialize(GrovePi.Sensors.Orientation.GreenToRed);
            GroveLCD.SetBacklightRgb(255, 50, 255);

            DeviceFactory.Build.GrovePi().PinMode(Pin.DigitalPin2, PinMode.Output);
            Delay.Milliseconds(10);
            DeviceFactory.Build.GrovePi().Flush();

            DeviceFactory.Build.GrovePi().PinMode(Pin.DigitalPin6, PinMode.Output);
            Delay.Milliseconds(10);
            DeviceFactory.Build.GrovePi().Flush();

        }

        private void InitDataList()
        {
            DistanceList = new List<DataPoint>();
            for (int i = 0; i < DataPointCnt; i++)
            {
                DistanceList.Add(new DataPoint() { Time = i + 1, Value = 0 });
            }
            SoundList = new List<DataPoint>();
            for (int i = 0; i < DataPointCnt; i++)
            {
                SoundList.Add(new DataPoint() { Time = i + 1, Value = 0 });
            }
            LightList = new List<DataPoint>();
            for (int i = 0; i < DataPointCnt; i++)
            {
                LightList.Add(new DataPoint() { Time = i + 1, Value = 0 });
            }
        }

        private void InitWorker()
        {
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
            async (workItem) =>
            {
                int work_index = 0;

                int distance = 0, sound = 0, light = 0, rotary = 0;
                SensorStatus button = SensorStatus.Off;
                SensorStatus buzzer = SensorStatus.Off;

                while (true)
                {
                    if (work_index < 5)
                    {
                        rotary = GroveRotary.SensorValue();
                        button = GroveButton.CurrentState;

                        //
                        RotaryValue = rotary;
                        int level = RotaryValue / 100;
                        if (level > 10) level = 10;
                        GroveLedBar.SetLevel((byte)level);

                        buzzer = GroveBuzzer.CurrentState;

                        if (RotaryValue > 1000)
                        {
                            if (buzzer != SensorStatus.On)
                            {
                                GroveBuzzer.ChangeState(SensorStatus.On);
                            }
                        }
                        else
                        {
                            if (buzzer != SensorStatus.Off)
                            {
                                GroveBuzzer.ChangeState(SensorStatus.Off);
                            }
                        }

                        if (button == SensorStatus.On)
                        {
                            RelayOnOff = 1;
                            GroveRelay.ChangeState(SensorStatus.On);
                        }
                        else
                        {
                            RelayOnOff = 0;
                            GroveRelay.ChangeState(SensorStatus.Off);
                        }

                        work_index++;
                    }
                    else
                    {

                        // Read temp & humidity
                        GroveTempHumi.Measure();
                        LastTemp = GroveTempHumi.TemperatureInCelsius;
                        LastHumi = GroveTempHumi.Humidity;

                        distance = GroveRanger.MeasureInCentimeters();
                        //System.Diagnostics.Debug.WriteLine(distance);

                        sound = GroveSound.SensorValue();
                        //System.Diagnostics.Debug.WriteLine(sound);

                        light = GroveLight.SensorValue();
                        //System.Diagnostics.Debug.WriteLine(light);

                        if (distance > 0 && distance < 100)
                        {
                            ShiftLeft(DistanceList, distance);
                        }

                        ShiftLeft(SoundList, sound);
                        ShiftLeft(LightList, light);

                        work_index = 0;
                    }

                    await Dispatcher.RunAsync(
                        CoreDispatcherPriority.High,
                        () =>
                        {
                            if (work_index == 0)
                            {
                                UpdataUISlow();
                            }
                            else
                            {
                                UpdateUIFast();
                            }

                        });

                    //Delay.Milliseconds(100);
                }
            }
            );
        }

        private static void ShiftLeft(List<DataPoint> list, double newValue)
        {
            for (int i = 0; i < DataPointCnt - 1; i++)
            {
                list[i].Value = list[i + 1].Value;
            }
            list[DataPointCnt - 1].Value = newValue;
        }

        private void UpdataUISlow()
        {
            List<DataPoint> lst = new List<DataPoint>(DistanceList);
            (LineChart0.Series[0] as LineSeries).ItemsSource = lst;

            lst = new List<DataPoint>(SoundList);
            (LineChart1.Series[0] as LineSeries).ItemsSource = lst;
            lst = new List<DataPoint>(LightList);
            (LineChart1.Series[1] as LineSeries).ItemsSource = lst;

            GroveLCD.SetText("Temp: " + LastTemp.ToString("F2") + "C\nHumi: " + LastHumi.ToString("F2") + "%");
        }

        private void UpdateUIFast()
        {
            if (LastRotaryValue != RotaryValue)
            {
                PieSlice.Radius = 50;
                PieSlice.StartAngle = 0;
                double angle = RotaryValue / 2.844;
                if (angle > 360) angle = 360;
                PieSlice.EndAngle = angle;

                LastRotaryValue = RotaryValue;
            }

            if (LastRelayOnOff != RelayOnOff)
            {
                if (RelayOnOff > 0)
                {
                    Rect0.Fill = new SolidColorBrush(Colors.Green);
                    Textblock0.Text = "On";
                }
                else
                {
                    Rect0.Fill = new SolidColorBrush(Colors.Red);
                    Textblock0.Text = "Off";
                }
                LastRelayOnOff = RelayOnOff;
            }

        }
    }
}
