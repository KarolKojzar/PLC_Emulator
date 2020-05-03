/* Kontrolka ucACMotor.
 * Służy do prezentacji graficznej stanu silnika trójfazowego.
 * 
 * Autor: Przemysław Olczak
 *        Automatyka i Robotyka
 *        Komputerowe Systemy Sterowania
 */
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
using System.Threading;
using System.Globalization;
using PLCEmulator;
using PLCEmulator.ACMotor;

namespace PLCEmulator.ACMotor
{
    /// <summary>
    /// Interaction logic for ucACMotor.xaml
    /// </summary>
    public partial class ucACMotor : UserControl
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/
        
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="a_tsState">Tryb pracy kontrolki.</param>
        public                      ucACMotor(TabStates a_tsState, ref mshtml.IHTMLDocument2 htmlDoc)
        {
            State = a_tsState;
            _htmlResult = htmlDoc;

            InitializeComponent();

            _lblDOCaptions.Add((Label)FindName("labelDI1"));
            _lblDOCaptions.Add((Label)FindName("labelDI2"));
            _lblDOCaptions.Add((Label)FindName("labelDI3"));
            _lblDOCaptions.Add((Label)FindName("labelDI4"));

            _lblDICaptions.Add((Label)FindName("labelDO1"));
            _lblDICaptions.Add((Label)FindName("labelDO2"));
            _lblDICaptions.Add((Label)FindName("labelDO3"));
            _lblDICaptions.Add((Label)FindName("labelDO4"));

            _lblDICaptions.Add((Label)FindName("labelDO5"));
            _lblDICaptions.Add((Label)FindName("labelDO6"));
            _lblDICaptions.Add((Label)FindName("labelDO7"));
            _lblDICaptions.Add((Label)FindName("labelDO8"));
            _lblDICaptions.Add((Label)FindName("labelDO9"));
            _lblDICaptions.Add((Label)FindName("labelDO10"));
            _lblDICaptions.Add((Label)FindName("labelDO11"));
            _lblDICaptions.Add((Label)FindName("labelDO12"));

            for (int i = 0; i < _lblDICaptions.Count; i++)
                _isSetDO.Add(false);

            /* Przygotowanie kontrolki w zależności od trybu działania */
            if (State == TabStates.DEMO)
            {
                /* Ustawienie poczatkowego stanu */
                StateCode = 0;
                /* Ustawienie początkowego położenia ciężarka - lewy brzeg */
                _motor.Position = 0;
            }
            if (State == TabStates.SIMULATION)
            {
                /* Ustawienie początkowego położenia ciężarka - środek */
                _motor.Position = 50;
                /* Wyłączenie elementów sterujących */
                stackPanelSterring.IsEnabled = false;

                /* Wystawienie jedynej na wyjściach 11 12 */
                USBComm.usbSetDO(10, 1);
                USBComm.usbSetDO(11, 1);
            }
            if (State == TabStates.TEST)
            {
                /* Ustawienie początkowego położenia ciężarka - niezerowa */
                _motor.Position = 2;
                /* Wyłączenie suwaka prędkości symulacji */
                sliderFastness.IsEnabled = false;
                /* Ustawienie timer'a. Odpytywanie co 50 [ms] */
                _timerTest.Interval = new TimeSpan(0, 0, 0, 0, 50);
                /* Ustawienie poczatkowego stanu */
                StateCode = 0;
            }
            /* Wartość na sliderze */
            sliderFastness.Value = 100;
            /* Ustawienie timer'a. Odpytywanie co 100 [ms] */
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            /* Przypisanie odpowiedniej metody */
            _timer.Tick += new EventHandler(_timer_Tick);
            /* Start timera */
            _timer.Start();

        }
        /// <summary>
        /// Obsługa timer'a dla kontrolki.
        /// </summary> 
        void                        _timer_Tick(object sender, EventArgs e)
        {
            /* Działania w zalęzności od trybu */
            if (State == TabStates.DEMO)
            {
                /* Obliczenia */
                calculateDemo();
            }
            /* Sprawdzenie stanu połączenia z driver'em */
            USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
            /* Jeżeli jest połączenie */
            if (devState.state == (int)stateCodes.STATE_CONNECTED)
            {
                /* Jeżeli kontrolka ma zezwolenie na wystawianie wyjść */
                if (AllowOutputs)
                {
                    if (State == TabStates.SIMULATION)
                    {
                        /* Ustawienie wejść klasy ACMotor */
                        setInputs();
                        /* Wykonuje krok symulacji */
                        _motor.DiscretStep();
                    }
                    if (State == TabStates.TEST)
                    {
                        /* Obliczenia */
                        calculateTest();
                    }
                    /* Aktualizacja wyjść */
                    if (State == TabStates.SIMULATION || State == TabStates.TEST)
                        setOutputs();
                }
            }
            /* Aktualizacja grafiki reaktora */
            updateACMotorImg();
            /* Aktualizacja etykiet tekstowych */
            setText();
        }
        /// <summary>
        /// Logika trybu test TEST.
        /// </summary>
        void                        calculateTest()
        {
            string tmpStr = "";
            /* Gdy uruchomiony tryb TEST */
            if (_bTestEnabled)
            {
                /* Ustawienie wejść klasy ACMotor */
                setInputs();
                /** Początek testu **/
                /* Test na przycisk START w PRAWO */
                if (StateCode == 0)
                {
                    if (_nWait == 0)
                    {
                        /* Dodaje informację o teście */
                        tmpStr = Messages.testSR;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku SR */
                        _motor.StartRight = true;
                        USBComm.usbSetDO(2, 1);
                        USBComm.usbSetDO(3, 1);
                        _motor.Position = 5;
                        /* Dodaje zdarzenie sprawdzania startu w prawo do timera test */
                        _timerTest.Tick += new EventHandler(testStartRight);
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nWait = 0;
                    }
                }
                /* Test na przycisk STOP */
                else if (StateCode == 1)
                {
                    if (_nWait == 0)
                    {
                        /* Dodaje informacje o teście */
                        tmpStr = Messages.testSTOP;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku STOP */
                        _motor.Stop = false;
                        /* Dodaje zdarzenie sprawdzania stop do timera test */
                        _timerTest.Tick += new EventHandler(testStop);
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nWait = 0;
                    }
                }
                /* Test na przycisk START w LEWO */
                if (StateCode == 2)
                {
                    if (_nWait == 0)
                    {
                        _startLeftError = false;
                        /* Dodaje informację o teście */
                        tmpStr = Messages.testSL;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku SL */
                        _motor.StartLeft = true;
                        /* Przesuwam ciężarek na prawy koniec */
                        _motor.Position = 95;
                        /* Dodaje zdarzenie sprawdzania startu w prawo do timera test */
                        _timerTest.Tick += new EventHandler(testStartLeft);
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nWait = 0;
                    }
                }
                /* Test na przycisk OVERLOAD */
                if (StateCode == 3)
                {
                    if (_nWait == 0)
                    {
                        int[] DIdata = new int[16];
                        USBComm.usbGetAllDI(DIdata);

                        /* Dodaje informację o teście */
                        tmpStr = Messages.testOverload;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku OVERLOAD */
                        _motor.Overload = false;
                        /** Sprawdzenie aktualnego stanu styków **/
                        /* Gdy obracanie w prawo */
                        if (Convert.ToBoolean(DIdata[3]) && Convert.ToBoolean(DIdata[0]) && !Convert.ToBoolean(DIdata[1]) && !Convert.ToBoolean(DIdata[2]))
                        {
                            /* Przesunięcie ciężarka na lewy koniec */
                            _motor.Position = 5;
                            /* Dodaje zdarzenie sprawdzania startu w prawo do timera test */
                            _timerTest.Tick += new EventHandler(testOverloadR);
                        }
                        /* Gdy obracanie w lewo */
                        else if (Convert.ToBoolean(DIdata[3]) && Convert.ToBoolean(DIdata[1]) && !Convert.ToBoolean(DIdata[0]) && !Convert.ToBoolean(DIdata[2]))
                        {
                            /* Przesunięcie ciężarka na prawy koniec */
                            _motor.Position = 95;
                            /* Dodaje zdarzenie sprawdzania startu w prawo do timera test */
                            _timerTest.Tick += new EventHandler(testOverloadL);
                        }
                        else
                        {
                            /* Dodaje informację o niemożliwości wykonania testu */
                            tmpStr = Messages.testOverload;
                            putRedHtmlText(tmpStr, false);
                            tmpStr = Messages.testNoOverload;
                            putRedHtmlText(tmpStr, false);
                            _nWait = 0;
                            StateCode = 4;
                        }
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nWait = 0;
                    }
                }
                /* Test na zatrzymanie przy krańcówce lewej */
                if (StateCode == 4)
                {
                    if (_startLeftError)
                    {
                        /* Dodaje informację o niemożliwości wykonania testu */
                        tmpStr = Messages.testLimitSwitch;
                        putRedHtmlText(tmpStr, false);
                        tmpStr = Messages.testNoLeftLimitTest;
                        putRedHtmlText(tmpStr, false);
                        _nWait = 0;
                        StateCode = 5;
                    }
                    else
                    {
                        if (_nWait == 0)
                        {
                            /* Dodaje informację o teście */
                            tmpStr = Messages.testLimitSwitch;
                            putGrayHtmlText(tmpStr);
                            /* Przesuwam ciężarek na 50% */
                            _motor.Position = 50;
                            /* Uruchamianie ruchu w lewo */
                            _motor.StartLeft = true;
                            /* Dodaje zdarzenie sprawdzania startu w prawo do timera test */
                            _timerTest.Tick += new EventHandler(testLimitSwitch);
                        }
                        _nWait++;
                        /* Odczekaj 100ms (2) */
                        if (_nWait > 2)
                        {
                            /* Start timera test */
                            _timerTest.Start();
                            StateCode = 100;
                            _nWait = 0;
                        }
                    }
                }
                if (StateCode == 5)
                {
                    tmpStr = Messages.testEnd;
                    putGrayHtmlText(tmpStr);
                    _bTestEnabled = false;
                    buttonPauseStart.Content = Messages.msgStartTest;
                    StateCode = 100;
                    for (int i = 0; i < 4; i++)
                        USBComm.usbSetDO(i, 0);
                }
                /* Wykonywanie kroku symulacji */
                _motor.DiscretStep();
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu startu ruchu w prawo.
        /// </summary>
        void                        testStartRight(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";

            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);

            /* Inkrementuje czas */
            _nWait++;
            if (_nInnerState == 0)
            {
                /* Oczekiwanie na K3 */
                testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.NOTEST, CoilsStates.TESTOFF);
                /* Jeżeli prawidłowo pojawił się sygnał na K3 */
                if (Convert.ToBoolean(DIdata[2]))
                {
                    tmpStr += Messages.testPLC + ": " + Messages.testCoilOn + " K3";
                    putGreenHtmlText(tmpStr, false);
                    _nWait = 0;
                    _nInnerState = 1;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 1)
            {
                /* Oczekiwanie na K1 */
                testInputs(ref tmpError, CoilsStates.NOTEST, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTOFF);
                /* Jeżeli prawidłowo pojawił się sygnał na K1 */
                if (Convert.ToBoolean(DIdata[0]))
                {
                    tmpStr += Messages.testPLC + ": " + Messages.testCoilOn + " K1";
                    putGreenHtmlText(tmpStr, false);
                    tmpStr = Messages.testYConn;
                    putGreenHtmlText(tmpStr, true);
                    _nWait = 0;
                    _nInnerState = 2;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 2)
            {
                /* Przed upływem 6s */
                if (_nWait < 120) //Dobrze działało dla 120 
                    testInputs(ref tmpError, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTOFF);
                /* Po upływie 7s */
                else
                {
                    if(!Convert.ToBoolean(DIdata[2]))
                    {
                        /* Oczekiwanie na K3 - OFF */
                        testInputs(ref tmpError, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.NOTEST, CoilsStates.TESTOFF);
                        tmpStr += Messages.testPLC + ": " + Messages.testCoilOff + " K3";
                        putGreenHtmlText(tmpStr, false);
                        _nWait = 0;
                        _nInnerState = 3;
                    }
                    /* Gdy przekroczono 1s (140 + 20) oczekiwania na prawidłowy stan */
                    if (_nWait >= 140 + 20)
                        tmpError = 2;
                }
            }
            else if (_nInnerState == 3)
            {
                if (Convert.ToBoolean(DIdata[3]))
                {
                    /* Oczekiwanie na K4 */
                    testInputs(ref tmpError, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.NOTEST);
                    /* Jeżeli prawidłowo pojawił się sygnał na K4 */
                    tmpStr += Messages.testPLC + ": " + Messages.testCoilOn + " K4";
                    putGreenHtmlText(tmpStr, false);
                    tmpStr = Messages.testDConn;
                    putGreenHtmlText(tmpStr, true);
                    _nWait = 0;
                    _nInnerState = 4;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 4)
            {
                _nWait = 0;
                _nInnerState = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testStartRight;
                StateCode = 1;
                tmpStr += Messages.testSR + " " + Messages.testPass;
                putGreenHtmlText(tmpStr, false);
                _motor.StartRight = false;
            }
            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testSR + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testStartRight;
                StateCode = 1;
                _motor.StartRight = false;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu stop.
        /// </summary>
        void                        testStop(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";

            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);

            /* Oczekiwanie na wyłączenie styków K1, K2, K3, K4 */
            testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF);
            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testSTOP + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                putRedHtmlText(tmpStr, false);
            }
            else
            {
                tmpStr = Messages.testPLC + ": " + Messages.testAllOff;
                putGreenHtmlText(tmpStr, true);
                tmpStr = Messages.testSTOP + " " + Messages.testPass;
                putGreenHtmlText(tmpStr, false);
            }
            _nWait = 0;
            _nInnerState = 0;
            _timerTest.Stop();
            _timerTest.Tick -= testStop;
            /* Wyzerowanie stanu wyjść */
            _motor.Stop = true;
            /* Przejście do następnego etapu */
            StateCode = 2;
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu startu ruchu w lewo.
        /// </summary>
        void                        testStartLeft(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";

            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);

            /* Inkrementuje czas */
            _nWait++;
            if (_nInnerState == 0)
            {
                /* Oczekiwanie na K3 */
                testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.NOTEST, CoilsStates.TESTOFF);
                /* Jeżeli prawidłowo pojawił się sygnał na K3 */
                if (Convert.ToBoolean(DIdata[2]))
                {
                    tmpStr = Messages.testPLC + ": " + Messages.testCoilOn + " K3";
                    putGreenHtmlText(tmpStr, false);
                    _nWait = 0;
                    _nInnerState = 1;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 1)
            {
                /* Jeżeli prawidłowo pojawił się sygnał na K2 */
                if (Convert.ToBoolean(DIdata[1]))
                {
                    /* Oczekiwanie na K2 */
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.NOTEST, CoilsStates.TESTON, CoilsStates.TESTOFF);
                    tmpStr = Messages.testPLC + ": " + Messages.testCoilOn + " K2";
                    putGreenHtmlText(tmpStr, false);
                    tmpStr = Messages.testYConn;
                    putGreenHtmlText(tmpStr, true);
                    _nWait = 0;
                    _nInnerState = 2;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 2)
            {
                /* Przed upływem 6s */
                if (_nWait < 120)
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTON, CoilsStates.TESTOFF);
                /* Po upływie 7s */
                else
                {
                    if (!Convert.ToBoolean(DIdata[2]))
                    {
                        /* Oczekiwanie na K3 - OFF */
                        testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.NOTEST, CoilsStates.TESTOFF);
                        tmpStr += Messages.testPLC + ": " + Messages.testCoilOff + " K3";
                        putGreenHtmlText(tmpStr, false);
                        _nWait = 0;
                        _nInnerState = 3;
                    }
                    /* Gdy przekroczono 1s (140 + 20) oczekiwania na prawidłowy stan */
                    if (_nWait >= 140 + 20)
                        tmpError = 2;
                }
            }
            else if (_nInnerState == 3)
            {
                if (Convert.ToBoolean(DIdata[3]))
                {
                    /* Oczekiwanie na K4 */
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.NOTEST);
                    /* Jeżeli prawidłowo pojawił się sygnał na K4 */
                    tmpStr = Messages.testPLC + ": " + Messages.testCoilOn + " K4";
                    putGreenHtmlText(tmpStr, false);
                    tmpStr = Messages.testDConn;
                    putGreenHtmlText(tmpStr, true);
                    _nWait = 0;
                    _nInnerState = 4;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 4)
            {
                _nWait = 0;
                _nInnerState = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testStartLeft;
                StateCode = 3;
                tmpStr += Messages.testSL + " " + Messages.testPass;
                putGreenHtmlText(tmpStr, false);
                _motor.StartLeft = false;
            }
            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testSL + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr += Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr += Messages.testErrorTime;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testStartLeft;
                StateCode = 3;
                _motor.StartLeft = false;
                _startLeftError = true;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu przeciążenia przy ruchu w prawo.
        /// </summary>
        void                        testOverloadR(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";

            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);

            /* Inkrementuje czas */
            _nWait++;
            if (_nInnerState == 0)
            {
                _nWait++;
                /* Przed upływem 2s (40) */
                if (_nWait < 40)
                    testInputs(ref tmpError, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTON);
                /* Po upływie 2s */
                else if (_nWait > 60)
                {
                    /* Oczekiwanie na K1, K2, K3, K4 - OFF */
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF);
                }
            }
            
            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testOverload + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testOverloadR;
                StateCode = 4;
                _motor.Overload = true;
            }
            else if (!Convert.ToBoolean(tmpError) && (_nWait > 60))
            {
                tmpStr = Messages.testOverload + " " + Messages.testPass;
                putGreenHtmlText(tmpStr, false);

                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testOverloadR;
                StateCode = 4;
                _motor.Overload = true;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu przeciążenia przy ruchu w lewo.
        /// </summary>
        void                        testOverloadL(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";

            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);

            /* Inkrementuje czas */
            _nWait++;
            if (_nInnerState == 0)
            {
                _nWait++;
                /* Przed upływem 2s (40) */
                if (_nWait < 40)
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.TESTON);
                /* Po upływie 3s */
                else if (_nWait > 70)
                {
                    /* Oczekiwanie na K1, K2, K3, K4 - OFF */
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF);
                }
            }

            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testOverload + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testOverloadL;
                StateCode = 4;
                _motor.Overload = true;
            }
            else if (!Convert.ToBoolean(tmpError) && (_nWait > 60))
            {
                tmpStr = Messages.testOverload + " " + Messages.testPass;
                putGreenHtmlText(tmpStr, false);

                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testOverloadL;
                StateCode = 4;
                _motor.Overload = true;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu lewego wyłącznika krańcowego.
        /// </summary>
        void                        testLimitSwitch(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";

            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);

            /* Inkrementuje czas */
            _nWait++;
            if (_nInnerState == 0)
            {
                /* Oczekiwanie na K3 */
                testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.NOTEST, CoilsStates.TESTOFF);
                /* Jeżeli prawidłowo pojawił się sygnał na K3 */
                if (Convert.ToBoolean(DIdata[2]))
                {
                    tmpStr = Messages.testPLC + ": " + Messages.testCoilOn + " K3";
                    putGreenHtmlText(tmpStr, false);
                    _nWait = 0;
                    _nInnerState = 1;
                }
                /* Gdy przekroczono 200ms (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 1)
            {
                /* Jeżeli prawidłowo pojawił się sygnał na K2 */
                if (Convert.ToBoolean(DIdata[1]))
                {
                    /* Oczekiwanie na K2 */
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.NOTEST, CoilsStates.TESTON, CoilsStates.TESTOFF);
                    tmpStr = Messages.testPLC + ": " + Messages.testCoilOn + " K2";
                    putGreenHtmlText(tmpStr, false);
                    tmpStr = Messages.testYConn;
                    putGreenHtmlText(tmpStr, true);
                    _nWait = 0;
                    _nInnerState = 2;
                    /* Przesunięcie ciężarka na położenie 0 - działa krańcówka lewa */
                    _motor.Position = 0;
                    _nWait = 0;
                    _motor.StartLeft = false;
                }
                /* Gdy przekroczono 0.2s (4) oczekiwania na prawidłowy stan */
                if (_nWait >= 4)
                    tmpError = 2;
            }
            else if (_nInnerState == 2)
            {
                /* Po 300ms (6) */
                if (_nWait > 6)
                {
                    /* Oczekiwanie na K1, K2, K3, K4 - Off */
                    testInputs(ref tmpError, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF);
                    if (!Convert.ToBoolean(tmpError))
                    {
                        tmpStr = Messages.testPLC + ": " + Messages.testAllOff;
                        putGreenHtmlText(tmpStr, true);
                        tmpStr = Messages.testLimitSwitch + " " + Messages.testPass;
                        putGreenHtmlText(tmpStr, false);
                        /* Zatrzymanie testu */
                        _nInnerState = 0;
                        _nWait = 0;
                        _timerTest.Stop();
                        _timerTest.Tick -= testLimitSwitch;
                        StateCode = 5;
                        _motor.StartLeft = false;
                        _motor.Position = 5;
                    }
                }
            }


            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testLimitSwitch + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr += Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr += Messages.testErrorTime;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _nInnerState = 0;
                _nWait = 0;
                _timerTest.Stop();
                _timerTest.Tick -= testLimitSwitch;
                StateCode = 5;
                _motor.StartLeft = false;
                _motor.Position = 5;
            }
        }
        /// <summary>
        /// Aktualizacja grafiki wyświetlanej na kontrolce.
        /// </summary>
        void                        updateACMotorImg()
        {
            int[] DOdata = new int[16];
            int[] DIdata = new int[16];
            if (SimulationEnabled)
            {
                DOdata[0] = Convert.ToInt16(_motor.StartRight);
                DOdata[1] = Convert.ToInt16(_motor.StartLeft);
                DOdata[2] = Convert.ToInt16(_motor.Stop);
                DOdata[3] = Convert.ToInt16(_motor.Overload);
                DOdata[4] = Convert.ToInt16(_motor.SK1Coil);
                DOdata[5] = Convert.ToInt16(_motor.SK2Coil);
                DOdata[6] = Convert.ToInt16(_motor.SK3Coil);
                DOdata[7] = Convert.ToInt16(_motor.SK4Coil);
                DOdata[8] = Convert.ToInt16(_motor.LeftLimitSwitch);
                DOdata[9] = Convert.ToInt16(_motor.RightLimitSwitch);
                DOdata[10] = 1;
                DOdata[11] = 1;

                DIdata[0] = Convert.ToInt16(_motor.K1Coil);
                DIdata[1] = Convert.ToInt16(_motor.K2Coil);
                DIdata[2] = Convert.ToInt16(_motor.K3Coil);
                DIdata[3] = Convert.ToInt16(_motor.K4Coil);
            }
            else
            {
                USBComm.usbGetAllDO(DOdata);
                USBComm.usbGetAllDI(DIdata);
            }
            /** Zobrazowanie stanu silnika: **/
            /* Sygnalizacja ustawienia w gwiazdę */
            imgY.Visibility = (_motor.YConnection ? Visibility.Visible : Visibility.Hidden);
            /* Sygnalizacja ustawienia w trójkąt */
            imgD.Visibility = (_motor.DConnection ? Visibility.Visible : Visibility.Hidden);
            
            /* Stany styków SK1-SK4 */
            imgSK1Open.Visibility = (!Convert.ToBoolean(DIdata[0]) ? Visibility.Visible : Visibility.Hidden);
            imgSK1Close.Visibility = (Convert.ToBoolean(DIdata[0]) ? Visibility.Visible : Visibility.Hidden);
            imgSK2Open.Visibility = (!Convert.ToBoolean(DIdata[1]) ? Visibility.Visible : Visibility.Hidden);
            imgSK2Close.Visibility = (Convert.ToBoolean(DIdata[1]) ? Visibility.Visible : Visibility.Hidden);
            imgSK3Open.Visibility = (!Convert.ToBoolean(DIdata[2]) ? Visibility.Visible : Visibility.Hidden);
            imgSK3Close.Visibility = (Convert.ToBoolean(DIdata[2]) ? Visibility.Visible : Visibility.Hidden);
            imgSK4Open.Visibility = (!Convert.ToBoolean(DIdata[3]) ? Visibility.Visible : Visibility.Hidden);
            imgSK4Close.Visibility = (Convert.ToBoolean(DIdata[3]) ? Visibility.Visible : Visibility.Hidden);

            /* Wyświetlenie grafiki silnika */
            imgMotorOn.Visibility = (((Convert.ToBoolean(DOdata[6]) || Convert.ToBoolean(DOdata[7])) && (Convert.ToBoolean(DOdata[5]) || Convert.ToBoolean(DOdata[4])) && !_motor.CircuitError) ? Visibility.Visible : Visibility.Hidden);
            imgMotorOff.Visibility = ((!((Convert.ToBoolean(DOdata[6]) || Convert.ToBoolean(DOdata[7])) && (Convert.ToBoolean(DOdata[5]) || Convert.ToBoolean(DOdata[4]))) || _motor.CircuitError) ? Visibility.Visible : Visibility.Hidden);

            /* Zobrazowanie przesunięcia ciężarka.
             * Funkcja liniowa: y = 18 * x - 900   */
            Canvas.SetLeft(imgWeight, 18.0 * _motor.Position - 900);

            /* Animacja przycisków */
            buttonRightOff.Visibility = (Convert.ToBoolean(DOdata[0]) ? Visibility.Collapsed : Visibility.Visible);
            buttonRightOn.Visibility = (Convert.ToBoolean(DOdata[0]) ? Visibility.Visible : Visibility.Collapsed);
            buttonLeftOff.Visibility = (Convert.ToBoolean(DOdata[1]) ? Visibility.Collapsed : Visibility.Visible);
            buttonLeftOn.Visibility = (Convert.ToBoolean(DOdata[1]) ? Visibility.Visible : Visibility.Collapsed);
            buttonStopOff.Visibility = (Convert.ToBoolean(DOdata[2]) ? Visibility.Collapsed : Visibility.Visible);
            buttonStopOn.Visibility = (Convert.ToBoolean(DOdata[2]) ? Visibility.Visible : Visibility.Collapsed);
            buttonOverloadOff.Visibility = (Convert.ToBoolean(DOdata[3]) ? Visibility.Collapsed : Visibility.Visible);
            buttonOverloadOn.Visibility = (Convert.ToBoolean(DOdata[3]) ? Visibility.Visible : Visibility.Collapsed);

            /* Ostrzeżenie gdy błędne połączenie lub gdy ciężarek osiągnął graniczne położenie*/
            labelWarning.Content = Messages.msgWarning;
            if (_motor.CircuitError || _motor.CrossingRightEnd || _motor.CrossingLeftEnd)
                _nWarningDelay++;
            else
                _nWarningDelay = 0;
            labelWarning.Visibility = ( _nWarningDelay >= 10 ? Visibility.Visible : Visibility.Hidden);
            /* Jeżeli tylko błędne połączenie */
            if (_motor.CircuitError && !_motor.CrossingLeftEnd && !_motor.CrossingRightEnd && _nWarningDelay >= 10)
            {
                labelMsgWarning.Content = Messages.msgBadConnection;
                labelMsgWarning.Visibility = Visibility.Visible;
            }
            /* Jeżeli ciężarek osiągnął graniczne położenie a obwód połączony prawidłowo */
            else if ((_motor.CrossingLeftEnd || _motor.CrossingRightEnd) && !_motor.CircuitError && _nWarningDelay >= 10)
            {
                labelMsgWarning.Content = Messages.msgOverheat;
                labelMsgWarning.Visibility = Visibility.Visible;
            }
            /* Jeżeli wystąpiły oba błędy */
            else if (_motor.CircuitError && (_motor.CrossingLeftEnd || _motor.CrossingRightEnd) && _nWarningDelay >= 10)
            {
                labelMsgWarning.Content = Messages.msgBadConnection + " , " + Messages.msgOverheat;
                labelMsgWarning.Visibility = Visibility.Visible;
            }
            /* Jak nic z powyższych */
            else
                labelMsgWarning.Visibility = Visibility.Hidden;

            /* Kolory tekstu */
            labelDO1.Foreground = (Convert.ToBoolean(DOdata[0]) ? _greenBrush : _redBrush);
            labelDO2.Foreground = (Convert.ToBoolean(DOdata[1]) ? _greenBrush : _redBrush);
            labelDO3.Foreground = (Convert.ToBoolean(DOdata[2]) ? _greenBrush : _redBrush);
            labelDO4.Foreground = (Convert.ToBoolean(DOdata[3]) ? _greenBrush : _redBrush);
            labelDO5.Foreground = (Convert.ToBoolean(DOdata[4]) ? _greenBrush : _redBrush);
            labelDO6.Foreground = (Convert.ToBoolean(DOdata[5]) ? _greenBrush : _redBrush);
            labelDO7.Foreground = (Convert.ToBoolean(DOdata[6]) ? _greenBrush : _redBrush);
            labelDO8.Foreground = (Convert.ToBoolean(DOdata[7]) ? _greenBrush : _redBrush);
            labelDO9.Foreground = (Convert.ToBoolean(DOdata[8]) ? _greenBrush : _redBrush);
            labelDO10.Foreground = (Convert.ToBoolean(DOdata[9]) ? _greenBrush : _redBrush);
            labelDO11.Foreground = _greenBrush;
            labelDO12.Foreground = _greenBrush;
            labelDI1.Foreground = (Convert.ToBoolean(DIdata[0]) ? _greenBrush : _redBrush);
            labelDI2.Foreground = (Convert.ToBoolean(DIdata[1]) ? _greenBrush : _redBrush);
            labelDI3.Foreground = (Convert.ToBoolean(DIdata[2]) ? _greenBrush : _redBrush);
            labelDI4.Foreground = (Convert.ToBoolean(DIdata[3]) ? _greenBrush : _redBrush);
        }
        /// <summary>
        /// Ustawia etykiety tekstowe dla całej kontrolki.
        /// </summary>
        void                        setText()
        {
            /* Ustawienie etykiet dla przycisków */
            labelRotateRight.Content = Messages.msgRotateRight;
            labelRotateLeft.Content = Messages.msgRotateLeft;
            labelStop.Content = Messages.msgStop;
            labelOverload.Content = Messages.msgOverload;
            if (!SimulationEnabled && (State == TabStates.SIMULATION || State == TabStates.DEMO))
                buttonPauseStart.Content = Messages.msgStartSimulation;
            if (SimulationEnabled && (State == TabStates.SIMULATION || State == TabStates.DEMO))
                buttonPauseStart.Content = Messages.msgPauseSimulation;
            if (!_bTestEnabled && State == TabStates.TEST)
                buttonPauseStart.Content = Messages.msgStartTest;
            if (_bTestEnabled && State == TabStates.TEST)
                buttonPauseStart.Content = Messages.msgStopTest;
            lblSimulationSpeed.Content = Messages.msgSimulationSpeed + ":";

            setDICaptions(ref _lblDICaptions);
            setDOCaptions(ref _lblDOCaptions);

            this.UpdateLayout();

        }
        /// <summary>
        /// Ustawia etykiety DI.
        /// </summary>
        /// <param name="a_DICaptions">Uporządkowana lista Labeli tekstowych</param>
        private void                setDICaptions(ref List<Label> a_DICaptions)
        {
            string tmpStart = PLCEmulator.Properties.Settings.Default.PLCDIStartSymbol;
            int tmpStartValue = PLCEmulator.Properties.Settings.Default.PLCDIStartValue;
            int tmpStartValueBit = PLCEmulator.Properties.Settings.Default.PLCDIStartValueBit;
            int tmpBit = PLCEmulator.Properties.Settings.Default.PLCDIBit;
            int tmpPLCType = PLCEmulator.Properties.Settings.Default.PLCChoice;
            int k = 0;
            for (int i = 0; i < a_DICaptions.Count; i++)
            {
                if (tmpBit > 0)
                {
                    if (tmpBit <= i)
                    {
                        tmpBit += tmpBit;
                        tmpStartValue++;
                        k = 0;
                    }
                        a_DICaptions[i].Content = tmpStart + tmpStartValue + "." + (tmpStartValueBit + k);
                    
                }
                else
                    a_DICaptions[i].Content = tmpStart + (tmpStartValue + k);
                k++;
            }
        }
        /// <summary>
        /// Ustawia etykiety DO.
        /// </summary>
        /// <param name="a_DOCaptions">Uporządkowana lista Labeli tekstowych</param>
        private void                setDOCaptions(ref List<Label> a_DOCaptions)
        {
            string tmpStart = PLCEmulator.Properties.Settings.Default.PLCDOStartSymbol;
            int tmpStartValue = PLCEmulator.Properties.Settings.Default.PLCDOStartValue;
            int tmpStartValueBit = PLCEmulator.Properties.Settings.Default.PLCDOStartValueBit;
            int tmpBit = PLCEmulator.Properties.Settings.Default.PLCDOBit;
            int tmpPLCType = PLCEmulator.Properties.Settings.Default.PLCChoice;
            int k = 0;
            for (int i = 0; i < a_DOCaptions.Count; i++)
            {
                if (tmpBit > 0)
                {
                    if (tmpBit <= i)
                    {
                        tmpBit += tmpBit;
                        tmpStartValue++;
                        k = 0;
                    }
                    a_DOCaptions[i].Content = tmpStart + tmpStartValue + "." + (tmpStartValueBit + i);
                }
                else
                    a_DOCaptions[i].Content = tmpStart + (tmpStartValue + i);
                k++;
            }
        }
        /// <summary>
        /// Ustawia odpowiednie wyjścia dla driver'a USB.
        /// </summary>
        void                        setOutputs()
        {
            /* I1 - SR - Obroty w prawo */
            if (_isSetDO[0] != _motor.StartRight)
            {
                USBComm.usbSetDO(0, Convert.ToInt16(_motor.StartRight));
                _isSetDO[0] = _motor.StartRight;
            }
            /* I2 - SL - Obroty w lewo */
            if (_isSetDO[1] != _motor.StartLeft)
            {
                USBComm.usbSetDO(1, Convert.ToInt16(_motor.StartLeft));
                _isSetDO[1] = _motor.StartLeft;
            }
            /* I3 - ST - Stop */
            if (_isSetDO[2] != _motor.Stop)
            {
                USBComm.usbSetDO(2, Convert.ToInt16(_motor.Stop));
                _isSetDO[2] = _motor.Stop;
            }
            /* I4 - UB - Przeciążenie */
            if (_isSetDO[3] != _motor.Overload)
            {
                USBComm.usbSetDO(3, Convert.ToInt16(_motor.Overload));
                _isSetDO[3] = _motor.Overload;
            }
            /* I5 - SK1 - Potwierdzenie dla styku SK1 */
            if (_isSetDO[4] != _motor.SK1Coil)
            {
                USBComm.usbSetDO(4, Convert.ToInt16(_motor.SK1Coil));
                _isSetDO[4] = _motor.SK1Coil;
            }
            /* I6 - SK2 - Potwierdzenie dla styku SK2 */
            if (_isSetDO[5] != _motor.SK2Coil)
            {
                USBComm.usbSetDO(5, Convert.ToInt16(_motor.SK2Coil));
                _isSetDO[5] = _motor.SK2Coil;
            }
            /* I7 - SK3 - Potwierdzenie dla styku SK3 */
            if (_isSetDO[6] != _motor.SK3Coil)
            {
                USBComm.usbSetDO(6, Convert.ToInt16(_motor.SK3Coil));
                _isSetDO[6] = _motor.SK3Coil;
            }
            /* I8 - SK4 - Potwierdzenie dla styku SK4 */
            if (_isSetDO[7] != _motor.SK4Coil)
            {
                USBComm.usbSetDO(7, Convert.ToInt16(_motor.SK4Coil));
                _isSetDO[7] = _motor.SK4Coil;
            }
            /* I9 - L - Krańcówka lewa */
            if (_isSetDO[8] != _motor.LeftLimitSwitch)
            {
                USBComm.usbSetDO(8, Convert.ToInt16(_motor.LeftLimitSwitch));
                _isSetDO[8] = _motor.LeftLimitSwitch;
            }
            /* I10 - R - Krańcówka prawa */
            if (_isSetDO[9] != _motor.RightLimitSwitch)
            {
                USBComm.usbSetDO(9, Convert.ToInt16(_motor.RightLimitSwitch));
                _isSetDO[9] = _motor.RightLimitSwitch;
            }
        }
        /// <summary>
        /// Ustawia wejścia klasy Reaktor.
        /// </summary>
        void                        setInputs()
        {
            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);
            /** Przekazanie stanu wejść do kalsy ACMotor **/
            /* Q1 */
            _motor.K1Coil = Convert.ToBoolean(DIdata[0]);
            /* Q2 */
            _motor.K2Coil = Convert.ToBoolean(DIdata[1]);
            /* Q3 */
            _motor.K3Coil = Convert.ToBoolean(DIdata[2]);
            /* Q4 */
            _motor.K4Coil = Convert.ToBoolean(DIdata[3]);
        }
        /// <summary>
        /// Obliczanie stanu silnika trójfazowego w trybie DEMO.
        /// </summary>
        void                        calculateDemo()
        {
            /* Gdy uruchomiona symulacja */
            if (SimulationEnabled)
            {
                /* Maszyna stanów dla trybu DEMO silnika */
                switch (StateCode)
                {
                    case 0:
                        {
                            /* Naciśnięcie przycisku SR przez 5 jednostek czasu */
                            _motor.StartRight = true;
                            if (_nWait < 5)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 1;
                            }
                        }
                        break;
                    case 1:
                        {
                            _motor.StartRight = false;
                            /* Połączenie w gwiazdę */
                            _motor.K3Coil = true;
                            /* Startuje z ruchem w prawo */
                            _motor.K1Coil = true;
                            /* Start ruchu w prawo przez 10 jednostki czasu */
                            if (_nWait < 10)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 2;
                            }
                        }
                        break;
                    case 2:
                        {
                            /* Przełączenie na trójkąt */
                            _motor.K3Coil = false;
                            _motor.K4Coil = true;
                            /* Ruch w prawo do puki krancówka */
                            if (_motor.RightLimitSwitch)
                            {
                                _motor.K1Coil = false;
                                _motor.K4Coil = false;
                                StateCode = 3;
                            }
                        }
                        break;
                    case 3:
                        {
                            /* Naciśnięcie przycisku SL przez 5 jednostki czasu */
                            _motor.StartLeft = true;
                            if (_nWait < 5)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 4;
                            }
                        }
                        break;
                    case 4:
                        {
                            _motor.StartLeft = false;
                            /* Połączenie w gwiazdę */
                            _motor.K3Coil = true;
                            /* Startuje z ruchem w lewo */
                            _motor.K2Coil = true;
                            /* Start ruchu w lewo przez 10 jednostki czasu */
                            if (_nWait < 10)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 5;
                            }
                        }
                        break;
                    case 5:
                        {
                            /* Przełączenie na trójkąt */
                            _motor.K3Coil = false;
                            _motor.K4Coil = true;
                            /* Ruch w lewo */
                            if (_nWait < 10)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 6;
                            }
                        }
                        break;
                    case 6:
                        {
                            /* Naciśnięcie przycisku STOP */
                            _motor.Stop = false;
                            if (_nWait < 5)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 7;
                            }
                        }
                        break;
                    case 7:
                        {
                            _motor.Stop = true;
                            /* Zatrzymanie */
                            _motor.K4Coil = false;
                            _motor.K2Coil = false;
                            if (_nWait < 10)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 8;
                            }
                        }
                        break;
                    case 8:
                        {
                            /* Naciśnięcie przycisku SL przez 5 jednostki czasu */
                            _motor.StartLeft = true;
                            if (_nWait < 5)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 9;
                            }
                        }
                        break;
                    case 9:
                        {
                            _motor.StartLeft = false;
                            /* Połączenie w gwiazdę */
                            _motor.K3Coil = true;
                            /* Startuje z ruchem w lewo */
                            _motor.K2Coil = true;
                            /* Start ruchu w lewo przez 10 jednostki czasu */
                            if (_nWait < 10)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 10;
                            }
                        }
                        break;
                    case 10:
                        {
                            /* Przełączenie na trójkąt */
                            _motor.K3Coil = false;
                            _motor.K4Coil = true;
                            /* Ruch w lewo */
                            if (_nWait < 10)
                                _nWait++;
                            else
                            {
                                _nWait = 0;
                                StateCode = 11;
                            }
                        }
                        break;
                    case 11:
                        {
                            /* Symulacja obciążenia */
                            _motor.Overload = false;
                            /* Ruch w lewo */
                            if (_motor.LeftLimitSwitch)
                            {
                                _motor.Overload = true;
                                _motor.K2Coil = false;
                                _motor.K4Coil = false;
                                StateCode = 0;
                            }
                        }
                        break;
                }
                /* Wykonuje krok symulacji */
                _motor.DiscretStep();
            }
        }
        /// <summary>
        /// Testuje wejścia cyfrowe. Sprawdza czy są na nich pożadane sygnały.
        /// </summary>
        /// <param name="tmpError">Zmienna określająca wystąpienie błedu w teście.</param>
        /// <param name="k1">Pożadany stan na K1.</param>
        /// <param name="k2">Pożadany stan na K1.</param>
        /// <param name="k3">Pożadany stan na K1.</param>
        /// <param name="k4">Pożadany stan na K1.</param>
        private void                testInputs(ref int tmpError, CoilsStates k1, CoilsStates k2, CoilsStates k3, CoilsStates k4)
        {
            int[] DIdata = new int[16];
            USBComm.usbGetAllDI(DIdata);
            string tmpStr = Messages.testPLC + ": ";
            /** Do momentu zmiany stanu wejść **/
            if(k1 >= 0)
                if (Convert.ToBoolean(DIdata[0]) != Convert.ToBoolean(k1))
                {
                    /* Jeżeli miało być ON */
                    tmpStr += (k1 == CoilsStates.TESTON ? Messages.testCoilOff : Messages.testCoilOn) + " K1";
                    putRedHtmlText(tmpStr, false);
                    tmpStr = Messages.testPLC + ": ";
                    /* Wystąpił błąd */
                    tmpError = 1;
                }
            if (k2 >= 0)
                if (Convert.ToBoolean(DIdata[1]) != Convert.ToBoolean(k2))
                {
                    /* Jeżeli miało być ON */
                    tmpStr += (k2 == CoilsStates.TESTON ? Messages.testCoilOff : Messages.testCoilOn) + " K2";
                    putRedHtmlText(tmpStr, false);
                    tmpStr = Messages.testPLC + ": ";
                    /* Wystąpił błąd */
                    tmpError = 1;
                }
            if (k3 >= 0)
                if (Convert.ToBoolean(DIdata[2]) != Convert.ToBoolean(k3))
                {
                    /* Jeżeli miało być ON */
                    tmpStr += (k3 == CoilsStates.TESTON ? Messages.testCoilOff : Messages.testCoilOn) + " K3";
                    putRedHtmlText(tmpStr, false);
                    tmpStr = Messages.testPLC + ": ";
                    /* Wystąpił błąd */
                    tmpError = 1;
                }
            if (k4 >= 0)
                if (Convert.ToBoolean(DIdata[3]) != Convert.ToBoolean(k4))
                {
                    /* Jeżeli miało być ON */
                    tmpStr += (k4 == CoilsStates.TESTON ? Messages.testCoilOff : Messages.testCoilOn) + " K4";
                    putRedHtmlText(tmpStr, false);
                    /* Wystąpił błąd */
                    tmpError = 1;
                }
        }
        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku obroty w prawo.
        /// </summary>
        void                        buttonRightOff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.StartRight = true;
                USBComm.usbSetDO(0, Convert.ToInt16(_motor.StartRight));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku obroty w prawo.
        /// </summary>
        void                        buttonRightOn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.StartRight = false;
                USBComm.usbSetDO(0, Convert.ToInt16(_motor.StartRight));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zjechania myszką z przycisku obroty w prawo.
        /// </summary>
        void                        buttonRightOn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.StartRight = false;
                USBComm.usbSetDO(0, Convert.ToInt16(_motor.StartRight));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku obroty w lewo.
        /// </summary>
        void                        buttonLeftOn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.StartLeft = false;
                USBComm.usbSetDO(1, Convert.ToInt16(_motor.StartLeft));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku obroty w lewo.
        /// </summary>
        void                        buttonLeftOff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.StartLeft = true;
                USBComm.usbSetDO(1, Convert.ToInt16(_motor.StartLeft));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zjechania myszką z przycisku obroty w lewo.
        /// </summary>
        private void                buttonLeftOn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.StartLeft = false;
                USBComm.usbSetDO(1, Convert.ToInt16(_motor.StartLeft));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku stop.
        /// </summary>
        private void                buttonStopOn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.Stop = false;
                USBComm.usbSetDO(2, Convert.ToInt16(_motor.Stop));
            }
        }
        /// <summary>
        /// Obsługa zwolnienia przycisku stop.
        /// </summary>
        private void                buttonStopOff_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.Stop = true;
                USBComm.usbSetDO(2, Convert.ToInt16(_motor.Stop));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zjechania myszką z przycisku stop.
        /// </summary>
        private void                buttonStopOff_MouseLeave(object sender, MouseEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.Stop = true;
                USBComm.usbSetDO(2, Convert.ToInt16(_motor.Stop));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku przeciążenie.
        /// </summary>
        void                        buttonOverloadOff_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.Overload = true;
                USBComm.usbSetDO(3, Convert.ToInt16(_motor.Overload));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku przeciążenie.
        /// </summary>
        void                        buttonOverloadOn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.Overload = false;
                USBComm.usbSetDO(3, Convert.ToInt16(_motor.Overload));
            }
        }
        /// <summary>
        /// Obsługa zdarzenia zjechania myszką z przycisku przeciązenie.
        /// </summary>
        private void                buttonOverloadOff_MouseLeave(object sender, MouseEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _motor.Overload = true;
                USBComm.usbSetDO(3, Convert.ToInt16(_motor.Overload));
            }
        }
        /// <summary>
        /// Obsługa przycisku Zatrzymania/Wznowienia symulacji.
        /// </summary>
        private void                buttonPauseStart_Click(object sender, RoutedEventArgs e)
        {
            /* Dla trybu DEMO */
            SimulationEnabled = ((SimulationEnabled && State == TabStates.DEMO) ? false : true);
            buttonPauseStart.Content = ((SimulationEnabled && State == TabStates.DEMO) ? Messages.msgStartSimulation : Messages.msgPauseSimulation);
            /* Dla trybu TEST */
            if (_bTestEnabled && State == TabStates.TEST)
            {
                _bTestEnabled = false;
                buttonPauseStart.Content = Messages.msgStartTest;
                _htmlResult.close();
                for (int i = 0; i < 4; i++)
                    USBComm.usbSetDO(i, 0);
            }
            /* Gdy w trybie test naciśnięto przycisk start test */
            else if (!_bTestEnabled && State == TabStates.TEST)
            {
                /* Sprawdzam stan połączenia */
                USBComm.TDeviceState devState = USBComm.usbGetDeviceState();
                /* Jeżeli rozłączone */
                if (devState.state == (int)stateCodes.STATE_DISCONNECTED)
                {
                    /* Próba połączenia */
                    int ret = USBComm.usbConnect();
                    /* Jeżeli nie udana */
                    if (ret != (int)errorCodes.ERR_NONE)
                        putRedHtmlText(Messages.msgNoConnection + ".", false);
                }
                    /* Jeżeli podłączone */
                else if (devState.state == (int)stateCodes.STATE_CONNECTED)
                {
                    _bTestEnabled = true;
                    buttonPauseStart.Content = Messages.msgStopTest;
                    _htmlResult.close();
                    /* Ustawienie początkowego stanu wyjść */
                    _motor.StartLeft = false;
                    _motor.StartRight = false;
                    _motor.Overload = true;
                    _motor.Stop = true;
                    setTestModeText();
                    StateCode = 0;
                    putGrayHtmlText("Rozpoczęcie testu...");
                }
            }
        }
        /// <summary>
        /// Wyświetla informację w pomocy dla trybu TEST.
        /// </summary>
        public void                 setTestModeText()
        {
            string tmp = "";
            /* Wyświetl informację o trybie TEST */
            tmp += Messages.testInfoMain;
            tmp += Messages.testInfoHeader;
            tmp += Messages.testInfoInfo;
            _htmlResult.clear();
            _htmlResult.writeln(tmp);
        }
        /// <summary>
        /// Obsługa zmiany wartości na sliderze związanym z prędkością symulacji.
        /// </summary>
        private void                sliderFastness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, (int)sliderFastness.Value);
        }
        /// <summary>
        /// Wstawia czerwony tekst zawarty w argumencie do okna HTML pomocy.
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        /// <param name="a_bOK">Czy dodać drugą kolumnę w tabeli z informacją "NOK"</param>
        private void                putRedHtmlText(string a_sText, bool a_bOK)
        {
            string tmp = "";
            if(a_bOK)
                tmp += "<table border=\"0\" width=\"100%\"><tr><td width=\"80%\"><font color=\"Red\">" + a_sText + "</font></td><td width=\"20%\"><font color=\"Red\">NOK</font></td></tr></table>";
            else
                tmp += "<table border=\"0\" width=\"100%\"><tr><td><font color=\"Red\">" + a_sText + "</font></td></tr></table>";
            _htmlResult.writeln(tmp);
            _htmlResult.parentWindow.scrollTo(0, _htmlResult.body.innerHTML.Length);
        }
        /// <summary>
        /// Wstawia zielony tekst zawarty w argumencie do okna HTML pomocy.
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        /// <param name="a_bOK">Czy dodać drugą kolumnę w tabeli z informacją "OK"</param>
        private void                putGreenHtmlText(string a_sText, bool a_bOK)
        {
            string tmp = "";
            if(a_bOK)
                tmp += "<table border=\"0\" width=\"100%\"><tr><td width=\"80%\"><font color=\"Green\">" + a_sText + "</font></td><td width=\"20%\"><font color=\"Green\">OK</font></td></tr></table>";
            else
                tmp += "<table border=\"0\" width=\"100%\"><tr><td><font color=\"Green\">" + a_sText + "</font></td></tr></table>";
            _htmlResult.writeln(tmp);
            _htmlResult.parentWindow.scrollTo(0, _htmlResult.body.innerHTML.Length);
        }
        /// <summary>
        /// Wstawia szary tekst zawarty w argumencie do okna HTML pomocy.
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        private void                putGrayHtmlText(string a_sText)
        {
            string tmp = "";
            tmp += "<table border=\"0\" width=\"100%\"><tr><td><font color=\"Gray\"><center>" + a_sText + "</center></font></td></tr></table>";
            _htmlResult.writeln(tmp);
            _htmlResult.parentWindow.scrollTo(0, _htmlResult.body.innerHTML.Length);
        }
        
        /************************************************************
         * PROPERTIES                                               *
         ************************************************************/

        /// <summary>
        /// Tryby pracy programu.
        /// </summary>
        public enum                 TabStates
        {
            /// <summary>
            /// Tryb DEMO
            /// </summary>
            DEMO = 1,
            /// <summary>
            /// Tryb SIMULATION
            /// </summary>
            SIMULATION = 2,
            /// <summary>
            /// Tryb TEST
            /// </summary>
            TEST = 3,
        }
        /// <summary>
        /// Tryby testowania styków.
        /// </summary>
        public enum                 CoilsStates
        {
            /// <summary>
            /// Testuj na włączenie.
            /// </summary>
            TESTOFF = 0,
            /// <summary>
            /// Testuj na wyłączenie.
            /// </summary>
            TESTON = 1,
            /// <summary>
            /// Nie testuj.
            /// </summary>
            NOTEST = -1,
        }
        /// <summary>
        /// Czyta/Zmienia tryb pracy kontrolki. 1 - Demo, 2 - Simulation, 3 - Test.
        /// </summary>
        public TabStates            State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// Czyta/Zmienia stan silnika w trybie DEMO. 0 - ????.
        /// </summary>
        public int                  StateCode
        {
            get { return _nStateCode; }
            set { _nStateCode = value; }
        }
        /// <summary>
        /// Czyta/Zmienia wartość oznaczającą działanie symylacji.
        /// </summary>
        public bool                 SimulationEnabled
        {
            get { return _bSimulationEnabled; }
            set { _bSimulationEnabled = value; }
        }
        /// <summary>
        /// Czyta / ustawia zezwolenie na wystawienie wartości na wyjście drivera USB.
        /// </summary>
        public bool                 AllowOutputs
        {
            get { return _bAllowOutputs; }
            set { _bAllowOutputs = value; }
        }

        /************************************************************
         * FIELDS                                                   *
         ************************************************************/

        /// <summary>
        /// Aktualny tryb pracy kontrolki.
        /// </summary>
        TabStates                   _state;
        /// <summary>
        /// Timer dla kontrolki.
        /// </summary>
        DispatcherTimer             _timer = new DispatcherTimer();
        /// <summary>
        /// Timer dla trybu test.
        /// </summary>
        DispatcherTimer             _timerTest = new DispatcherTimer();
        /// <summary>
        /// Obiekt ACMotor.
        /// </summary>
        PLCEmulator.ACMotor.ACMotor _motor = new PLCEmulator.ACMotor.ACMotor();
        /// <summary>
        /// Numer stanu dla trybu demo.
        /// </summary>
        int                         _nStateCode;
        /// <summary>
        /// Stany wewnętrzne przy tesowaniu.
        /// </summary>
        int                         _nInnerState;
        /// <summary>
        /// Włączona/Wyłączona symulacja.
        /// </summary>
        bool                        _bSimulationEnabled;
        /// <summary>
        /// Informuje o stanie trybu TEST.
        /// </summary>
        bool                        _bTestEnabled;
        /// <summary>
        /// Zmienna dla trybu DEMO określa czas oczekiwania między zdarzeniami.
        /// </summary>
        int                         _nWait;
        /// <summary>
        /// Zezwala / zabrania na wystawianie wartości na wyjścia drivera USB.
        /// </summary>
        bool                        _bAllowOutputs;
        /// <summary>
        /// Obiekt dla przechowywania elementów HTML'a dla okna pomocy.
        /// </summary>
        mshtml.IHTMLDocument2       _htmlResult = null;
        /// <summary>
        /// Czarny kolor dla etykiety.
        /// </summary>
        SolidColorBrush             _redBrush = new SolidColorBrush(Colors.Red);
        /// <summary>
        /// Zielony kolor dla etykiety.
        /// </summary>
        SolidColorBrush             _greenBrush = new SolidColorBrush(Colors.LightGreen);
        /// <summary>
        /// Lista etykiet DO.
        /// </summary>
        List<Label>                 _lblDOCaptions = new List<Label>();
        /// <summary>
        /// Lista etykiet DI.
        /// </summary>
        List<Label>                 _lblDICaptions = new List<Label>();
        /// <summary>
        /// Przechowuje poprzednie stany DO z symulatora.
        /// </summary>
        List<bool>                  _isSetDO = new List<bool>();
        /// <summary>
        /// Opóźnienie dla wyświetlenia ostrzeżeń.
        /// </summary>
        int                         _nWarningDelay;
        /// <summary>
        /// Zmienna informuje o niepowodzeniu testu "start w lewo".
        /// </summary>
        bool                        _startLeftError;  
    }
}
