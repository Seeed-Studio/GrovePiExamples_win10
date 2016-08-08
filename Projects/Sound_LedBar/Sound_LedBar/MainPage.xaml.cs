using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GrovePi;
using GrovePi.Sensors;
using System.Threading.Tasks;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sound_LedBar
{
    public class DataPoint
    {
        public int Time { get; set; }
        public double Value { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<DataPoint> SoundList;
        int DataPointCnt = 5;

        // Connect the LedBar  to digital port 5
        ILedBar GroveLedBar = DeviceFactory.Build.BuildLedBar(Pin.DigitalPin5);

        // Connect the Sound Sensor to analog port 0
        ISoundSensor GroveSound = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPageLoaded;            
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {           
            GroveLedBar.Initialize(GrovePi.Sensors.Orientation.GreenToRed);
            SoundList = new List<DataPoint>();
            for (int i = 0; i < DataPointCnt; i++)
            {
                SoundList.Add(new DataPoint() { Time = i + 1, Value = 0 });
            }

            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(async (workItem) =>
            {
                int sound = 0;

                while (true)
                {
                    try
                    {
                        sound = GroveSound.SensorValue();                                                
                        setLedBar(ConvertSoundToBarLevel(sound));                         
                        System.Diagnostics.Debug.WriteLine("Sound: " + sound.ToString());
                    }
                    catch (Exception ex)
                    {
                        // If you want to see the exceptions uncomment the following:
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                                       
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ShiftLeft(SoundList, sound);
                        List<DataPoint> lst = new List<DataPoint>(SoundList);
                        (LineChart.Series[0] as LineSeries).ItemsSource = lst;
                    });
                }                
            });
        }

        private void ShiftLeft(List<DataPoint> list, double newValue)
        {
            for (int i = 0; i < DataPointCnt - 1; i++)
            {
                list[i].Value = list[i + 1].Value;
            }
            list[DataPointCnt - 1].Value = newValue;
        }

        // Set LedBar light up level
        private void setLedBar(byte level)
        {            
            for (byte i = 1; i < level + 1; i++)
            {
                GroveLedBar.SetLevel(i);
                Task.Delay(1).Wait();
            }
        }

        private byte ConvertSoundToBarLevel(int value)
        {
            int max = 1023;
            int min = 350;
            byte MaxLevel = 10;
            byte MinLevel = 1;
            int unit = (max - min)/ 10;  // Setting 10 levels

            if (value >= max)
                return MaxLevel;
            if (value < min)
                return MinLevel;

            byte level = (byte)(( value - min ) / unit + 1);

            return level;
        }    
    }
}
