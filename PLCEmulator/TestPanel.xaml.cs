using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PLCEmulator
{
    /// <summary>
    /// Interaction logic for TestPanel.xaml
    /// </summary>
    public partial class TestPanel : Window
    {
        public TestPanel()
        {
            _bLoadData = true;
            InitializeComponent();

            for (int i = 1; i <= 16; i++)
            {
                Ellipse eDI = (Ellipse)FindName("DI" + i);
                eDI.Fill = _grayBrush;
                _arrDIs.Add(eDI);
                Ellipse eDO = (Ellipse)FindName("DO" + i);
                eDO.Fill = _grayBrush;
                _arrDOs.Add(eDO);
                Ellipse ebDO = (Ellipse)FindName("b_DO" + i);
                _arrB_DOs.Add(ebDO);
            }
            for (int i = 1; i <= 4; i++)
            {
                TextBox tbAI = (TextBox)FindName("AI" + i);
                _arrAIs.Add(tbAI);
                TextBox tbAO = (TextBox)FindName("AO" + i);
                _arrAOs.Add(tbAO);
                Slider sAO = (Slider)FindName("s_AO" + i);
                _arrS_AOs.Add(sAO);
            }

            // Ustawienie timera, który co 200ms odpytuje urządzenie
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();

            _bLoadData = false;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            if (devState.state == (int)stateCodes.STATE_DISCONNECTED)
            {

                int ret = USBComm.usbConnect();
                if (ret == (int)errorCodes.ERR_NONE)
                {
                    Console.WriteLine("Connected");
                }
                else
                {
                    Console.WriteLine("Connection failed with return code: " + ret);
                    //USBComm.usbDisconnect();
                }
            }
            else
            {
                if (devState.state == (int)stateCodes.STATE_CONNECTED)
                {
                    //USBComm.usbDisconnect();
                    //int r=USBComm.usbConnect();

                    SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
                    SolidColorBrush grayBrush = new SolidColorBrush(Colors.Gray);

                    int[] DIdata = new int[16];
                    int[] DOdata = new int[16];

                    short[] AOdata = new short[8];
                    short[] AIdata = new short[8];

                    USBComm.usbGetAllDI(DIdata);
                    USBComm.usbGetAllDO(DOdata);
                    USBComm.usbGetAllAO(AOdata);
                    USBComm.usbGetAllAI(AIdata);

                    _bLoadData = true;

                    for (int i = 0; i < 16; i++)
                    {
                        _arrDIs[i].Fill = (DIdata[i] == 1 ? redBrush : grayBrush);
                        _arrDOs[i].Fill = (DOdata[i] == 1 ? redBrush : grayBrush);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        _arrAIs[i].Text = String.Format("{0:0.00}", ((Double)(10 * AIdata[i]) / 4095));
                        _arrAOs[i].Text = String.Format("{0:0.00}", (((Double)(10 * AOdata[i])) / 4095));
                        _arrS_AOs[i].Value = AOdata[i];
                        // _arrS_AOs[i].Value = AOdata[i];
                    }
                    this.UpdateLayout();
                    _bLoadData = false;
                }
            }

        }

        private void b_DO_click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Ellipse el = (Ellipse)sender;
            int n = Int32.Parse(el.Name.Remove(0, 4));
            int value = (((SolidColorBrush)(_arrDOs[n - 1].Fill)).Color.Equals(_redBrush.Color) ? 0 : 1);
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            if (devState.state == (int)stateCodes.STATE_CONNECTED)
                USBComm.usbSetDO(n - 1, value);
        }

        private void AO_valueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_bLoadData)
            {
                Slider sl = (Slider)sender;
                int n = Int32.Parse(sl.Name.Remove(0, 4));
                short value = (Int16)e.NewValue; // pobranie nowo ustawionej wartość, Slider-y mają działają od 0 do 1024
                USBComm.TDeviceState devState = USBComm.usbGetDeviceState(); // Pobieramy stan
                if (devState.state == (int)stateCodes.STATE_CONNECTED)
                    USBComm.usbSetAO(n - 1, value); //Jeżeli jestśmy połączeni to ustaw to wyjście
            }
        }

        private void AO_textChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!_bLoadData)
            {
                TextBox tb = (TextBox)sender;
                int n = Int32.Parse(tb.Name.Remove(0, 2));
                try
                {
                    double volt = double.Parse(tb.Text);
                    short value = (Int16)(volt * 4095 / 10); // pobranie nowo ustawionej wartość, musi być to jakieś Double 
                    USBComm.TDeviceState devState = USBComm.usbGetDeviceState(); // Pobieramy stan
                    if (devState.state == (int)stateCodes.STATE_CONNECTED) USBComm.usbSetAO(n - 1, value);
                    //Jeżeli jesteśmy połączeni to ustaw to wyjście
                }
                catch
                {
                }
            }
        }

        private void TestPanel_closed(object sender, EventArgs e)
        {
            _timer.Stop();
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            if (devState.state == (int)stateCodes.STATE_CONNECTED)
                USBComm.usbDisconnect();

        }
        private void EnkoderEnable_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            _bEncoderEnable = chb.IsChecked == true ? true : false;
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            if (_bEncoderEnable)
            {
                short value = Int16.Parse(_tbEncoder.Text);
                char dir = _bRightDirection ? (char)0 : (char)1;
                if (devState.state == (int)stateCodes.STATE_CONNECTED)
                    USBComm.usbSetEncoder(value, dir);
            }
            else
            {
                if (devState.state == (int)stateCodes.STATE_CONNECTED)
                    USBComm.usbDisEncoder();
            }
        }
        private void EncoderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sl = (Slider)sender;
            short value = (Int16)e.NewValue; // pobranie nowo ustawionej wartość, 
            char dir = _bRightDirection ? (char)0 : (char)1;
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState(); // Pobieramy stan
            _tbEncoder.Text = ((Int32)value).ToString();
            if (devState.state == (int)stateCodes.STATE_CONNECTED)
            {
                USBComm.usbSetEncoder(value, dir);
            }
        }
        private void Right_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            _bRightDirection = true;
        }
        private void LeftDirection_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            _bRightDirection = false;
        }


        bool _bLoadData = false;
        DispatcherTimer _timer = new DispatcherTimer();

        public bool _bEncoderEnable = false;
        public bool _bRightDirection = true;
        public List<Ellipse> _arrDIs = new List<Ellipse>();
        public List<Ellipse> _arrDOs = new List<Ellipse>();
        public List<Ellipse> _arrB_DOs = new List<Ellipse>();
        public List<TextBox> _arrAOs = new List<TextBox>();
        public List<Slider> _arrS_AOs = new List<Slider>();
        public List<TextBox> _arrAIs = new List<TextBox>();
        public SolidColorBrush _redBrush = new SolidColorBrush(Colors.Red);
        public SolidColorBrush _grayBrush = new SolidColorBrush(Colors.Gray);
        public TextBox _tbEncoder = new TextBox();
        public Slider _sEncoder = new Slider();
    }
}
