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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Threading;
using System.Globalization;
using System.Net;
using System.Web;
using System.IO;
using System.Resources;
using PLCEmulator.Reaktor;
using PLCEmulator.ACMotor;
using PLCEmulator.Sygnalizacja;
using PLCEmulator.SygnalizacjaII;
using PLCEmulator.SygnalizacjaIII;

namespace PLCEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainWindow()
        {
            /* Domyślny język aplikacji - PL */
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("");
            InitializeComponent();

            /* Dodanie tekstu do etykiety języka */
            lblLanguage.Content = "EN";

            /* Dodanie tabów z poziomu konstruktora pozwala im nadać oznaczenie na odpowiedni stan */
            _tcMain = (TabControl)this.FindName("MainTab");
            _elConnected = (Ellipse)this.FindName("ellipseConnection");
            _miConnect = (MenuItem)this.FindName("ConnectMenuItem");
            _miTestPanel = (MenuItem)this.FindName("TestPanelMenuItem");
            _miOptions = (MenuItem)this.FindName("OptionsMenuItem");
            _miCaptions = (MenuItem)this.FindName("CaptionsMenuItem");
            _stConnectionBlinking = (Storyboard)FindResource("StoryboardConnectionBlinking");
            _stConnectionBlinking.Begin(this, true);
            // _stConnectionBlinking.Pause(this);

            /* Dodanie metod dla kliknięcia tabów Simulation i Test */
            tiSimulation.MouseLeftButtonDown += new MouseButtonEventHandler(_tiSimulation_MouseLeftButtonDown);
            tiTest.MouseLeftButtonDown += new MouseButtonEventHandler(_tiTest_MouseLeftButtonDown);

            /* Ustawienie timera, który co 500ms sprawdza połączenie i miga diodą */
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();

            /* Wstawienie pustej strony */
            webBrowserHelp.Navigate(new Uri("about:blank"));
            /* Stworzenie obiektu MSHTML */
            _htmlHelp = webBrowserHelp.Document as mshtml.IHTMLDocument2;


            /* Ustawienie etykiet tekstowych */
            SetText();
        }

        /// <summary>
        /// Obsługa zdarzenia kliknięcia myszką na Tab Simulation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _tiSimulation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActivateSimulationOutputs();
        }
        /// <summary>
        /// Obsługa zdarzenia kliknięcia myszką na Tab Test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _tiTest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActivateTestOutputs();
        }
        /// <summary>
        /// Obsługa zdarzenia kliknięcia etykiety zmiany języka aplikacji.
        /// </summary>
        private void lblLanguage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /* Jeżeli ustawiony język PL (etykieta jest EN) */
            if (lblLanguage.Content.Equals("EN"))
            {
                /* Zmień CultureInfo na EN */
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("EN");
                /* Ustaw tekst */
                SetText();
                /* Zmień etykietę na PL */
                lblLanguage.Content = "PL";
            }
            /* Jeżeli ustawiony język EN (etykieta jest PL) */
            else
            {
                /* Ustaw język domyślny - PL */
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("");
                /* Ustaw tekst */
                SetText();
                /* Zmień etykietę na EN */
                lblLanguage.Content = "EN";
            }
            /* Ustaw tekst w oknie pomocy */
            SetHelp();
        }
        /// <summary>
        /// Obsługa zdarzenia najechania myszką na etykietę.
        /// </summary>
        private void lblLanguage_MouseEnter(object sender, MouseEventArgs e)
        {
            lblLanguage.Background = new SolidColorBrush(Colors.AliceBlue);
        }
        /// <summary>
        /// Obsługa zdarzenia opuszczenia kursorem obszaru etykiety.
        /// </summary>
        private void lblLanguage_MouseLeave(object sender, MouseEventArgs e)
        {
            lblLanguage.Background = new SolidColorBrush(Colors.Transparent);
        }
        /// <summary>
        /// Obsługa zdarzenia - Kliknięcie na przycisk Menu - Opcje.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CaptionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CaptionsSettings settings = new CaptionsSettings();
            try
            {
                settings.ShowDialog();
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku połączenia z driver'em USB.
        /// </summary>
        void ConnectItem_Click(object sender, RoutedEventArgs e)
        {
            /* Jeżeli połączony - to rozłącz */
            if (_USBConnected)
            {
                USBComm.usbDisconnect();
                _USBConnected = false;
                _miConnect.Header = Properties.Messages.menuDisconnect;
            }
            /* Jeżeli nie podłączony - to nawiąż połączenie */
            else
            {
                USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
                if (devState.state == (int)stateCodes.STATE_DISCONNECTED)
                {
                    int ret = USBComm.usbConnect();
                    if (ret == (int)errorCodes.ERR_NONE)
                    {
                        Console.WriteLine("Connected");
                        _USBConnected = true;
                        _miConnect.Header = Properties.Messages.menuConnect;
                    }
                    else
                        Console.WriteLine("Connection failed with return code: " + ret);
                }
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zamknięcia okna aplikacji.
        /// </summary>
        void MainWindow_closed(object sender, EventArgs e)
        {
            _timer.Stop();
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            if (devState.state == (int)stateCodes.STATE_CONNECTED)
                USBComm.usbDisconnect();
        }
        /// <summary>
        /// Obsługa zdarzenia kliknięcia przycisku Test panel
        /// </summary>
        void TestPanel_click(object sender, RoutedEventArgs e)
        {
            TestPanel t = new TestPanel();
            t.Show();
        }
        /// <summary>
        /// Obsługa zdarzenia dla timera.
        /// </summary>
        void Timer_Tick(object sender, EventArgs e)
        {
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            if (devState.state == (int)stateCodes.STATE_CONNECTED)
            {
                _stConnectionBlinking.Begin(this, true);
                _miConnect.Header = Properties.Messages.menuDisconnect;
            }
            else
            {
                _miConnect.Header = Properties.Messages.menuConnect;
                _elConnected.Fill = _grayBrush;
                //if (cst.ToString().Equals("Active"))
                _stConnectionBlinking.Stop(this);
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zmiany elementu w widoku TreeView.
        /// </summary>
        void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = (TreeView)sender;
            TreeViewItem tvi = (TreeViewItem)e.NewValue;
            if (tvi.Header.Equals(Properties.Messages.msgReactor))
                onReactorExpand();
            if (tvi.Header.Equals(Properties.Messages.msgSygnalizacja))
                onSygnalizacjaExpand();
            if (tvi.Header.Equals(Properties.Messages.msgSygnalizacjaII))
                onSygnalizacjaIIExpand();
            if (tvi.Header.Equals(Properties.Messages.msgSygnalizacjaIII))
                onSygnalizacjaIIIExpand();
            if (tvi.Header.Equals(Properties.Messages.msgACMotor))
                onACMotorExpand();
            if (tvi.Header.Equals(Properties.Messages.msgDemo))
            {
                _tcMain.SelectedIndex = 0;
                DeactivateOutputs();
            }
            if (tvi.Header.Equals(Properties.Messages.msgSimulation))
            {
                _tcMain.SelectedIndex = 1;
                ActivateSimulationOutputs();
                /* łączenie z urządzeniem */
                if (!_USBConnected)
                {
                    USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
                    if (devState.state == (int)stateCodes.STATE_DISCONNECTED)
                    {
                        int ret = USBComm.usbConnect();
                        if (ret == (int)errorCodes.ERR_NONE)
                        {
                            Console.WriteLine("Connected");
                            _USBConnected = true;
                            _miConnect.Header = Properties.Messages.menuConnect;
                        }
                        else
                            Console.WriteLine("Connection failed with return code: " + ret);
                    }
                }
            }
            if (tvi.Header.Equals(Properties.Messages.msgTest))
            {
                _tcMain.SelectedIndex = 2;
                ActivateTestOutputs();
                /* łączenie z urządzeniem */
                if (!_USBConnected)
                {
                    USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
                    if (devState.state == (int)stateCodes.STATE_DISCONNECTED)
                    {
                        int ret = USBComm.usbConnect();
                        if (ret == (int)errorCodes.ERR_NONE)
                        {
                            Console.WriteLine("Connected");
                            _USBConnected = true;
                            _miConnect.Header = Properties.Messages.menuConnect;
                        }
                        else
                            Console.WriteLine("Connection failed with return code: " + ret);
                    }
                }
            }
            if (tvi.Header.Equals(Properties.Messages.msgSettings))
            {
                if (_bACMotorActive)
                {
                    PLCEmulator.ACMotor.ACMotorSettings settingsWnd = new PLCEmulator.ACMotor.ACMotorSettings();
                    settingsWnd.ShowDialog();
                }
                if (_bReactorActive)
                {
                    PLCEmulator.Reactor.ReactorSettings settingsWnd = new PLCEmulator.Reactor.ReactorSettings();
                    if (settingsWnd.ShowDialog() == true)
                        _bReactorActive = false;
                    onReactorExpand();
                }
            }
            if (tvi.Header.Equals(Properties.Messages.msgObjasnienia))
            {
                if (_bSygnalizacjaActive)
                {
                    PLCEmulator.Sygnalizacja.SygnalizacjaSettings settingsWnd = new PLCEmulator.Sygnalizacja.SygnalizacjaSettings();
                    if (settingsWnd.ShowDialog() == true)
                        _bSygnalizacjaActive = false;
                    onSygnalizacjaExpand();
                }
                if (_bSygnalizacjaIIActive)
                {
                    PLCEmulator.SygnalizacjaII.SygnalizacjaIISettings settingsWnd = new PLCEmulator.SygnalizacjaII.SygnalizacjaIISettings();
                    if (settingsWnd.ShowDialog() == true)
                        _bSygnalizacjaIIActive = false;
                    onSygnalizacjaIIExpand();
                }
                if (_bSygnalizacjaIIIActive)
                {
                    PLCEmulator.SygnalizacjaIII.SygnalizacjaIIISettings settingsWnd = new PLCEmulator.SygnalizacjaIII.SygnalizacjaIIISettings();
                    if (settingsWnd.ShowDialog() == true)
                        _bSygnalizacjaIIIActive = false;
                    onSygnalizacjaIIIExpand();
                }
            }
            SetHelp();
        }
        /// <summary>
        /// Obsługa zdarzenia - Rozwinięcia drzewa ACMotor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tviACMotor_Expanded(object sender, RoutedEventArgs e)
        {
            onACMotorExpand();
        }
        /// <summary>
        /// Obsługa zdarzenia - Rozwinięcia drzewa Reaktor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tviReactor_Expanded(object sender, RoutedEventArgs e)
        {
            onReactorExpand();
        }
        /// <summary>
        /// Obsługa zdarzenia - Rozwinięcia drzewa Sygnalizacja.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tviSygnalizacja_Expanded(object sender, RoutedEventArgs e)
        {
            onSygnalizacjaExpand();
        }
        /// <summary>
        /// Obsługa zdarzenia - Rozwinięcia drzewa SygnalizacjaII.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tviSygnalizacjaII_Expanded(object sender, RoutedEventArgs e)
        {
            onSygnalizacjaIIExpand();
        }
        /// <summary>
        /// Obsługa zdarzenia - Rozwinięcia drzewa SygnalizacjaIII.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tviSygnalizacjaIII_Expanded(object sender, RoutedEventArgs e)
        {
            onSygnalizacjaIIIExpand();
        }

        /// <summary>
        /// Uaktywnia wyjścia dla drivera USB z kontrolki Symulacji.
        /// </summary>
        void ActivateSimulationOutputs()
        {
            if (_bReactorActive)
            {
                ucReaktor tmp = (ucReaktor)tiSimulation.Content;
                tmp.AllowOutputs = true;
                tmp = (ucReaktor)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bACMotorActive)
            {
                ucACMotor tmp = (ucACMotor)tiSimulation.Content;
                tmp.AllowOutputs = true;
                tmp = (ucACMotor)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bSygnalizacjaActive)
            {
                ucSygnalizacja tmp = (ucSygnalizacja)tiSimulation.Content;
                tmp.AllowOutputs = true;
                tmp = (ucSygnalizacja)tiTest.Content;
                tmp.AllowOutputs = false;
                
            }
            if (_bSygnalizacjaIIActive)
            {
                ucSygnalizacjaII tmp = (ucSygnalizacjaII)tiSimulation.Content;
                tmp.AllowOutputs = true;
                tmp = (ucSygnalizacjaII)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bSygnalizacjaIIIActive)
            {
                ucSygnalizacjaIII tmp = (ucSygnalizacjaIII)tiSimulation.Content;
                tmp.AllowOutputs = true;
                tmp = (ucSygnalizacjaIII)tiTest.Content;
                tmp.AllowOutputs = false;
            }
        }
        /// <summary>
        /// Uaktywnia wyjścia dla drivera USB z kontrolki Testu.
        /// </summary>
        void ActivateTestOutputs()
        {
            if (_bReactorActive)
            {
                ucReaktor tmp = (ucReaktor)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucReaktor)tiTest.Content;
                tmp.AllowOutputs = true;
            }
            if (_bACMotorActive)
            {
                ucACMotor tmp = (ucACMotor)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucACMotor)tiTest.Content;
                tmp.AllowOutputs = true;
            }
            if (_bSygnalizacjaActive)
            {
                ucSygnalizacja tmp = (ucSygnalizacja)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucSygnalizacja)tiTest.Content;
                tmp.AllowOutputs = true;
            }
            if (_bSygnalizacjaIIActive)
            {
                ucSygnalizacjaII tmp = (ucSygnalizacjaII)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucSygnalizacjaII)tiTest.Content;
                tmp.AllowOutputs = true;
            }
            if (_bSygnalizacjaIIIActive)
            {
                ucSygnalizacjaIII tmp = (ucSygnalizacjaIII)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucSygnalizacjaIII)tiTest.Content;
                tmp.AllowOutputs = true;
            }
        }
        /// <summary>
        /// Deaktywuje wyjścia dla drivera USB.
        /// </summary>
        void DeactivateOutputs()
        {
            if (_bReactorActive)
            {
                ucReaktor tmp = (ucReaktor)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucReaktor)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bACMotorActive)
            {
                ucACMotor tmp = (ucACMotor)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucACMotor)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bSygnalizacjaActive)
            {
                ucSygnalizacja tmp = (ucSygnalizacja)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucSygnalizacja)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bSygnalizacjaIIActive)
            {
                ucSygnalizacjaII tmp = (ucSygnalizacjaII)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucSygnalizacjaII)tiTest.Content;
                tmp.AllowOutputs = false;
            }
            if (_bSygnalizacjaIIIActive)
            {
                ucSygnalizacjaIII tmp = (ucSygnalizacjaIII)tiSimulation.Content;
                tmp.AllowOutputs = false;
                tmp = (ucSygnalizacjaIII)tiTest.Content;
                tmp.AllowOutputs = false;
            }
        }
        /// <summary>
        /// Rozwinięcie drzewa dla ACMotor.
        /// </summary>
        void onACMotorExpand()
        {
            if (!_bACMotorActive)
            {
                ucACMotor ACMotorDemo = new ucACMotor(ucACMotor.TabStates.DEMO, ref _htmlHelp);
                tiDemo.Content = ACMotorDemo;

                ucACMotor ACMotorSimulation = new ucACMotor(ucACMotor.TabStates.SIMULATION, ref _htmlHelp);
                tiSimulation.Content = ACMotorSimulation;

                ucACMotor ACMotorTest = new ucACMotor(ucACMotor.TabStates.TEST, ref _htmlHelp);
                tiTest.Content = ACMotorTest;

                /* Program działą jako Reaktor a nie jako pozostałe */
                _bReactorActive = false;
                _bACMotorActive = true;
                _bSygnalizacjaActive = false;
                _bSygnalizacjaIIActive = false;
                _bSygnalizacjaIIIActive = false;

                tviReactor.IsExpanded = false;
                tviSygnalizacja.IsExpanded = false;
                tviSygnalizacjaII.IsExpanded = false;
                tviSygnalizacjaIII.IsExpanded = false;
                tviACMotor.IsExpanded = true;
                /* Przejście na zakładkę - Demo */
                _tcMain.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Rozwinięcie drzewa dla Reaktora.
        /// </summary>
        void onReactorExpand()
        {
            if (!_bReactorActive)
            {
                ucReaktor reaktorDemo = new ucReaktor(ucReaktor.TabStates.DEMO, ref _htmlHelp);
                tiDemo.Content = reaktorDemo;

                ucReaktor reaktorSimulation = new ucReaktor(ucReaktor.TabStates.SIMULATION, ref _htmlHelp);
                tiSimulation.Content = reaktorSimulation;

                ucReaktor reaktorTest = new ucReaktor(ucReaktor.TabStates.TEST, ref _htmlHelp);
                tiTest.Content = reaktorTest;

                /* Program działą jako Reaktor a nie jako pozostałe */
                _bReactorActive = true;
                _bACMotorActive = false;
                _bSygnalizacjaActive = false;
                _bSygnalizacjaIIActive = false;
                _bSygnalizacjaIIIActive = false;

                tviReactor.IsExpanded = true;
                tviACMotor.IsExpanded = false;
                tviSygnalizacja.IsExpanded = false;
                tviSygnalizacjaII.IsExpanded = false;
                tviSygnalizacjaIII.IsExpanded = false;
                /* Przejście na zakładkę - Demo */
                _tcMain.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Rozwinięcie drzewa dla Sygnalizacji.
        /// </summary>
        void onSygnalizacjaExpand()
        {
            if (!_bSygnalizacjaActive)
            {
                ucSygnalizacja SygnalizacjaDemo = new ucSygnalizacja(ucSygnalizacja.TabStates.DEMO, ref _htmlHelp);
                tiDemo.Content = SygnalizacjaDemo;

                ucSygnalizacja SygnalizacjaSimulation = new ucSygnalizacja(ucSygnalizacja.TabStates.SIMULATION, ref _htmlHelp);
                tiSimulation.Content = SygnalizacjaSimulation;

                ucSygnalizacja SygnalizacjaTest = new ucSygnalizacja(ucSygnalizacja.TabStates.TEST, ref _htmlHelp);
                tiTest.Content = SygnalizacjaTest;
                
                /* Program działą jako Sygnalizacja a nie jako pozostałe */
                _bReactorActive = false;
                _bACMotorActive = false;
                _bSygnalizacjaActive = true;
                _bSygnalizacjaIIActive = false;
                _bSygnalizacjaIIIActive = false;

                tviReactor.IsExpanded = false;
                tviACMotor.IsExpanded = false;
                tviSygnalizacjaII.IsExpanded = false;
                tviSygnalizacjaIII.IsExpanded = false;
                tviSygnalizacja.IsExpanded = true;
                /* Przejście na zakładkę - Demo */
                _tcMain.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Rozwinięcie drzewa dla SygnalizacjiII.
        /// </summary>
        void onSygnalizacjaIIExpand()
        {
            if (!_bSygnalizacjaIIActive)
            {
                ucSygnalizacjaII SygnalizacjaDemo = new ucSygnalizacjaII(ucSygnalizacjaII.TabStates.DEMO, ref _htmlHelp);
                tiDemo.Content = SygnalizacjaDemo;

                ucSygnalizacjaII SygnalizacjaSimulation = new ucSygnalizacjaII(ucSygnalizacjaII.TabStates.SIMULATION, ref _htmlHelp);
                tiSimulation.Content = SygnalizacjaSimulation;

                ucSygnalizacjaII SygnalizacjaTest = new ucSygnalizacjaII(ucSygnalizacjaII.TabStates.TEST, ref _htmlHelp);
                tiTest.Content = SygnalizacjaTest;

                /* Program działą jako Sygnalizacja a nie jako pozostałe */
                _bReactorActive = false;
                _bACMotorActive = false;
                _bSygnalizacjaActive = false;
                _bSygnalizacjaIIActive = true;
                _bSygnalizacjaIIIActive = false;

                tviReactor.IsExpanded = false;
                tviACMotor.IsExpanded = false;
                tviSygnalizacja.IsExpanded = false;
                tviSygnalizacjaII.IsExpanded = true;
                tviSygnalizacjaIII.IsExpanded = false;
                /* Przejście na zakładkę - Demo */
                _tcMain.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Rozwinięcie drzewa dla SygnalizacjiIII.
        /// </summary>
        void onSygnalizacjaIIIExpand()
        {
            if (!_bSygnalizacjaIIIActive)
            {
                ucSygnalizacjaIII SygnalizacjaDemo = new ucSygnalizacjaIII(ucSygnalizacjaIII.TabStates.DEMO, ref _htmlHelp);
                tiDemo.Content = SygnalizacjaDemo;

                ucSygnalizacjaIII SygnalizacjaSimulation = new ucSygnalizacjaIII(ucSygnalizacjaIII.TabStates.SIMULATION, ref _htmlHelp);
                tiSimulation.Content = SygnalizacjaSimulation;

                ucSygnalizacjaIII SygnalizacjaTest = new ucSygnalizacjaIII(ucSygnalizacjaIII.TabStates.TEST, ref _htmlHelp);
                tiTest.Content = SygnalizacjaTest;

                /* Program działą jako Sygnalizacja a nie jako pozostałe */
                _bReactorActive = false;
                _bACMotorActive = false;
                _bSygnalizacjaActive = false;
                _bSygnalizacjaIIActive = false;
                _bSygnalizacjaIIIActive = true;

                tviReactor.IsExpanded = false;
                tviACMotor.IsExpanded = false;
                tviSygnalizacja.IsExpanded = false;
                tviSygnalizacjaII.IsExpanded = false;
                tviSygnalizacjaIII.IsExpanded = true;
                /* Przejście na zakładkę - Demo */
                _tcMain.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Ustawia odpowiedni tekst w oknie pomocy.
        /// </summary>
        void SetHelp()
        {
            string tmp = "";
            /* Jeżeli aktywna kontrolka reaktora */
            if (_bReactorActive)
            {
                /* Dla zakładek Demo i Simulation */
                if (_tcMain.SelectedIndex == 0 || _tcMain.SelectedIndex == 1)
                {
                    _htmlHelp.close();
                    /* Wyświetlenie odpowiednij informacji w HTML'u */
                    tmp = Properties.Messages.ReactorHLP;
                    /* Wyświetlenie odpowiedniego tekstu */
                    _htmlHelp.clear();
                    _htmlHelp.writeln(tmp);
                    _htmlHelp.close();
                }
                else
                {
                    _htmlHelp.close();
                    ucReaktor ucReactorTmp = (ucReaktor)tiTest.Content;
                    ucReactorTmp.setTestModeText();
                }
            }
            /* Jeżeli aktywna kontrolka reaktora */
            if (_bACMotorActive)
            {
                /* Dla zakładek Demo i Simulation */
                if (_tcMain.SelectedIndex == 0 || _tcMain.SelectedIndex == 1)
                {
                    _htmlHelp.close();
                    /* Wyświetlenie odpowiednij informacji w HTML'u */
                    tmp = Properties.Messages.ACMotorHLP;
                    /* Wyświetlenie odpowiedniego tekstu */
                    _htmlHelp.clear();
                    _htmlHelp.writeln(tmp);
                    _htmlHelp.close();
                }
                else
                {
                    _htmlHelp.close();
                    ucACMotor ucMotorTmp = (ucACMotor)tiTest.Content;
                    ucMotorTmp.setTestModeText();
                }
            }
            /* Jeżeli aktywna kontrolka sygnalizacji */
            if (_bSygnalizacjaActive)
            {
                /* Dla zakładek Demo i Simulation */
                if (_tcMain.SelectedIndex == 0 || _tcMain.SelectedIndex == 1)
                {
                    _htmlHelp.close();
                    /* Wyświetlenie odpowiednij informacji w HTML'u */
                    tmp = Properties.Messages.SygnalizacjaHLP;
                    /* Wyświetlenie odpowiedniego tekstu */
                    _htmlHelp.clear();
                    _htmlHelp.writeln(tmp);
                    _htmlHelp.close();
                }
                else
                {
                    _htmlHelp.close();
                    ucSygnalizacja ucSygnalizacjaTmp = (ucSygnalizacja)tiTest.Content;
                    ucSygnalizacjaTmp.setTestModeText();
                }
            }
            /* Jeżeli aktywna kontrolka sygnalizacji II */
            if (_bSygnalizacjaIIActive)
            {
                /* Dla zakładek Demo i Simulation */
                if (_tcMain.SelectedIndex == 0 || _tcMain.SelectedIndex == 1)
                {
                    _htmlHelp.close();
                    /* Wyświetlenie odpowiednij informacji w HTML'u */
                    tmp = Properties.Messages.SygnalizacjaIIHLP;
                    /* Wyświetlenie odpowiedniego tekstu */
                    _htmlHelp.clear();
                    _htmlHelp.writeln(tmp);
                    _htmlHelp.close();
                }
                else
                {
                    _htmlHelp.close();
                    ucSygnalizacjaII ucSygnalizacjaIITmp = (ucSygnalizacjaII)tiTest.Content;
                    ucSygnalizacjaIITmp.setTestModeText();
                }
            }
            /* Jeżeli aktywna kontrolka sygnalizacji III */
            if (_bSygnalizacjaIIIActive)
            {
                /* Dla zakładek Demo i Simulation */
                if (_tcMain.SelectedIndex == 0 || _tcMain.SelectedIndex == 1)
                {
                    _htmlHelp.close();
                    /* Wyświetlenie odpowiednij informacji w HTML'u */
                    tmp = Properties.Messages.SygnalizacjaIIIHLP;
                    /* Wyświetlenie odpowiedniego tekstu */
                    _htmlHelp.clear();
                    _htmlHelp.writeln(tmp);
                    _htmlHelp.close();
                }
                else
                {
                    _htmlHelp.close();
                    ucSygnalizacjaIII ucSygnalizacjaIIITmp = (ucSygnalizacjaIII)tiTest.Content;
                    ucSygnalizacjaIIITmp.setTestModeText();
                }
            }
        }
        /// <summary>
        /// Ustawia etykiety tekstowe dla całej aplikacji.
        /// </summary>
        void SetText()
        {
            tiTest.Header = Properties.Messages.msgTest;
            tiDemo.Header = Properties.Messages.msgDemo;

            /* Etykiety Menu Głównego */
            tiSimulation.Header = Properties.Messages.msgSimulation;
            if (!_USBConnected)
                _miConnect.Header = Properties.Messages.menuConnect;
            else
                _miConnect.Header = Properties.Messages.menuDisconnect;
            _miTestPanel.Header = Properties.Messages.msgTestPannel;
            _miOptions.Header = Properties.Messages.menuOptions;
            _miCaptions.Header = Properties.Messages.menuCaptions;

            /* Etykiety na TreeView dla Reaktora */
            tviReactor.Header = Properties.Messages.msgReactor;
            tvi1Demo.Header = Properties.Messages.msgDemo;
            tvi1Simulation.Header = Properties.Messages.msgSimulation;
            tvi1Test.Header = Properties.Messages.msgTest;
            tvi1Settings.Header = Properties.Messages.msgSettings;

            /* Etykiety na TreeView dla ACMotor */
            tviACMotor.Header = Properties.Messages.msgACMotor;
            tvi2Demo.Header = Properties.Messages.msgDemo;
            tvi2Simulation.Header = Properties.Messages.msgSimulation;
            tvi2Test.Header = Properties.Messages.msgTest;
            tvi2Settings.Header = Properties.Messages.msgSettings;

            /* Etykiety na TreeView dla Sygnalizacji */
            tviSygnalizacja.Header = Properties.Messages.msgSygnalizacja;
            tvi3Demo.Header = Properties.Messages.msgDemo;
            tvi3Simulation.Header = Properties.Messages.msgSimulation;
            tvi3Test.Header = Properties.Messages.msgTest;
            tvi3Settings.Header = Properties.Messages.msgObjasnienia;

            /* Etykiety na TreeView dla Sygnalizacji II */
            tviSygnalizacjaII.Header = Properties.Messages.msgSygnalizacjaII;
            tvi4Demo.Header = Properties.Messages.msgDemo;
            tvi4Simulation.Header = Properties.Messages.msgSimulation;
            tvi4Test.Header = Properties.Messages.msgTest;
            tvi4Settings.Header = Properties.Messages.msgObjasnienia;

            /* Etykiety na TreeView dla Sygnalizacji III */
            tviSygnalizacjaIII.Header = Properties.Messages.msgSygnalizacjaIII;
            tvi5Demo.Header = Properties.Messages.msgDemo;
            tvi5Simulation.Header = Properties.Messages.msgSimulation;
            tvi5Test.Header = Properties.Messages.msgTest;
            tvi5Settings.Header = Properties.Messages.msgObjasnienia;
        }

        /************************************************************
         * PROPERTIES                                               *
         ************************************************************/

        /************************************************************
         * FIELDS                                                   *
         ************************************************************/

        /// <summary>
        /// TabControl dla różnych tabów.
        /// </summary>
        TabControl _tcMain = new TabControl();
        /// <summary>
        /// Timer dla migającej lampki połączenia driver'a USB
        /// </summary>
        DispatcherTimer _timer = new DispatcherTimer();
        /// <summary>
        /// Przycisk menu dla połączenia z driver'em USB.
        /// </summary>
        MenuItem _miConnect = new MenuItem();
        /// <summary>
        /// Przycisk menu uruchamiający panel testowy.
        /// </summary>
        MenuItem _miTestPanel = new MenuItem();
        /// <summary>
        /// Przycisk menu opcji.
        /// </summary>
        MenuItem _miOptions = new MenuItem();
        /// <summary>
        /// Przycisk menu opcji.
        /// </summary>
        MenuItem _miCaptions = new MenuItem();
        /// <summary>
        /// Lampka pokazująca połączenie z driver'em USB.
        /// </summary>
        Ellipse _elConnected = new Ellipse();
        /// <summary>
        /// Dla migającej lampki ozanczającej połączenie driver'a USB
        /// </summary>
        Storyboard _stConnectionBlinking = new Storyboard();
        /// <summary>
        /// Czerwony kolor dla lampki.
        /// </summary>
        SolidColorBrush _redBrush = new SolidColorBrush(Colors.Red);
        /// <summary>
        /// Szary kolor dla lampki.
        /// </summary>
        SolidColorBrush _grayBrush = new SolidColorBrush(Colors.Gray);
        /// <summary>
        /// Informuje czy połaczono z driver'em USB
        /// </summary>
        bool _USBConnected;
        /// <summary>
        /// Informacja o tym że działa program dla Reaktora.
        /// </summary>
        bool _bReactorActive;
        /// <summary>
        /// Informacja o tym że działa program dla silnika trójfazowego.
        /// </summary>
        bool _bACMotorActive;
        /// <summary>
        /// Informacja o tym że działa program dla sygnalizacji świetlnej.
        /// </summary>
        bool _bSygnalizacjaActive;
        /// <summary>
        /// Informacja o tym że działa program dla sygnalizacji świetlnej.
        /// </summary>
        bool _bSygnalizacjaIIActive;
        /// <summary>
        /// Informacja o tym że działa program dla sygnalizacji świetlnej.
        /// </summary>
        bool _bSygnalizacjaIIIActive;
        /// <summary>
        /// Obiekt dla przechowywania elementów HTML'a dla okna pomocy.
        /// </summary>
        mshtml.IHTMLDocument2 _htmlHelp = null;
    }
}