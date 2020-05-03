/* Kontrolka ucReaktor.
 * Służy do prezentacji graficznej stanu Reaktora.
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
using PLCEmulator.Reactor;

namespace PLCEmulator.Reaktor
{
    /// <summary>
    /// Kontrolka do prezentacji graficznej stanu obiektu Reaktor.
    /// </summary>
    public partial class ucReaktor : UserControl
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="a_tsState">Tryb pracy kontrolki.</param>
        public                      ucReaktor(TabStates a_tsState, ref mshtml.IHTMLDocument2 htmlDoc)
        {
            /* Zapisuje tryb działania */
            State = a_tsState;
            _htmlResult = htmlDoc;

            InitializeComponent();

            _lblDICaptions.Add((Label)FindName("labelDI1"));
            _lblDICaptions.Add((Label)FindName("labelDI2"));
            _lblDICaptions.Add((Label)FindName("labelDI3"));
            _lblDICaptions.Add((Label)FindName("labelDI4"));
            _lblDICaptions.Add((Label)FindName("labelDI5"));
            _lblDICaptions.Add((Label)FindName("labelDI6"));
            _lblDICaptions.Add((Label)FindName("labelDI7"));

            _lblDOCaptions.Add((Label)FindName("labelDO1"));
            _lblDOCaptions.Add((Label)FindName("labelDO2"));
            _lblDOCaptions.Add((Label)FindName("labelDO3"));
            _lblDOCaptions.Add((Label)FindName("labelDO4"));
            _lblDOCaptions.Add((Label)FindName("labelDO5"));
            _lblDOCaptions.Add((Label)FindName("labelDO6"));

            _lblAICaptions.Add((Label)FindName("labelAO1"));
            _lblAICaptions.Add((Label)FindName("labelAO2"));

            _lblAOCaptions.Add((Label)FindName("labelAI1"));
            _lblAOCaptions.Add((Label)FindName("labelAI2"));
            _lblAOCaptions.Add((Label)FindName("labelAI3"));
            _lblAOCaptions.Add((Label)FindName("labelAI4"));

            _buttonsOn.Add((Image)FindName("buttonOnDI1"));
            _buttonsOn.Add((Image)FindName("buttonOnDI2"));
            _buttonsOn.Add((Image)FindName("buttonOnDI3"));
            _buttonsOn.Add((Image)FindName("buttonOnDI4"));
            _buttonsOn.Add((Image)FindName("buttonOnDI5"));
            _buttonsOn.Add((Image)FindName("buttonOnDI6"));
            _buttonsOn.Add((Image)FindName("buttonOnDI7"));

            _buttonsOff.Add((Image)FindName("buttonOffDI1"));
            _buttonsOff.Add((Image)FindName("buttonOffDI2"));
            _buttonsOff.Add((Image)FindName("buttonOffDI3"));
            _buttonsOff.Add((Image)FindName("buttonOffDI4"));
            _buttonsOff.Add((Image)FindName("buttonOffDI5"));
            _buttonsOff.Add((Image)FindName("buttonOffDI6"));
            _buttonsOff.Add((Image)FindName("buttonOffDI7"));

            /* Dodanie zdarzeń do przycisków On i Off */
            for (int i = 0; i < _buttonsOn.Count; i++)
            {
                _buttonsOn[i].MouseLeftButtonDown += new MouseButtonEventHandler(buttonDIOn_MouseLeftButtonDown);
                _buttonsOff[i].MouseLeftButtonDown += new MouseButtonEventHandler(buttonDIOff_MouseLeftButtonDown);
            }

            /* Przygotowanie kontrolki w zależności od trybu działania */
            if (State == TabStates.DEMO)
            {
                /* Ustawienie poczatkowego stanu */
                StateCode = 0;
                /* Ustawienie wartości zadanej poziomu na 90% (wartość dowolna) */
                FillingSetPoint = 90.0;
                sliderFill.Value = FillingSetPoint;
                /* Ustawienie wartości zadane temperatury na 80oC (wartość dowolna)*/
                TemperatureSetPoint = 200.0;
                sliderTemperature.Value = TemperatureSetPoint;
                /* Ustawienie etykiety stanu reaktora */
                labelState.Content = Messages.msgFilling;
                /* Schowanie panelu wartości zadanych */
                //gridSetPoints.Visibility = Visibility.Collapsed;
                /* Zamiast schowania - wyłączenie elementów */
                gridSetPoints.IsEnabled = false;
            }
            if (State == TabStates.SIMULATION)
            {
                /* Ukrycie niepotrzebnych elementów */
                //stackPanelSterring.Visibility = Visibility.Collapsed;
                /* Zamiast ukrycia - wyłączenie elementów */
                stackPanelSterring.IsEnabled = false;
                /* Symulacja zawsze włączona */
                SimulationEnabled = true;
                /* Ukrycie etykiety opisującej stan procesu */
                labelState.Visibility = Visibility.Collapsed;
            }
            if (State == TabStates.TEST)
            {
                /* Ukrycie niepotrzebnych elementów */
                //stackPanelSterring.Visibility = Visibility.Collapsed;
                /* Wyłączenie suwaka prędkości symulacji */
                sliderFastness.IsEnabled = false;
                /* Ukrycie etykiety opisującej stan procesu */
                labelState.Visibility = Visibility.Collapsed;
                /* Ustawienie timer'a. Odpytywanie co 50 [ms] */
                _timerTest.Interval = new TimeSpan(0, 0, 0, 0, 50);
                /* Ustawienie poczatkowego stanu */
                StateCode = 0;
                /* Symulacja zawsze włączona */
                SimulationEnabled = true;
                /* Ustawienie wartości zadanych */
                _nTemperatureSetPoint = 140.0;
                _nFillingSetPoint = 70.0;
                sliderTemperature.Value = _nTemperatureSetPoint;
                sliderFill.Value = _nFillingSetPoint;
            }
            /* Ustawienie timer'a. Odpytywanie co 100 ms (takie jak dla próbkowania obiektu) */
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            /* Ustawienie wartości na sliderFastness */
            sliderFastness.Value = 100;
            /* Przypisanie odpowiedniej metody */
            _timer.Tick += new EventHandler(Timer_Tick);
            /* Przycinanie grafiki wypełnienia cieczy w reaktorze */
            RectangleGeometry rgWypelnienie = new RectangleGeometry();
            rgWypelnienie.Rect = new Rect(1070, 1380, 650, 670);
            imgFilling.Clip = rgWypelnienie;
            /* Start timera */
            _timer.Start();
            /* Ustawienie etykiet tekstowych */
            setText();
        }
        /// <summary>
        /// Ustawia etykiety tekstowe dla całej kontrolki.
        /// </summary>
        void                        setText()
        {
            /* Treści etykiet */
            if (!SimulationEnabled && (State == TabStates.SIMULATION || State == TabStates.DEMO))
                buttonPauseStart.Content = Messages.msgStartSimulation;
            if (SimulationEnabled && (State == TabStates.SIMULATION || State == TabStates.DEMO))
                buttonPauseStart.Content = Messages.msgPauseSimulation;
            if (!_bTestEnabled && State == TabStates.TEST)
                buttonPauseStart.Content = Messages.msgStartTest;
            if (_bTestEnabled && State == TabStates.TEST)
                buttonPauseStart.Content = Messages.msgStopTest;
            lblSimulationSpeed.Content = Messages.msgSimulationSpeed + ":";
            gbSetPoints.Header = Messages.msgSetPointValues;
            if (StateCode == 0)
                labelState.Content = Messages.msgFilling;
            if (StateCode == 1)
                labelState.Content = Messages.msgHeating;
            if (StateCode == 2)
                labelState.Content = Messages.msgStabilization;
            if (StateCode == 3)
                labelState.Content = Messages.msgCooling;
            if (StateCode == 4)
                labelState.Content = Messages.msgEmptying;

            /* Treści etykiet dla przycisków */
            labelDI1Name.Content = Messages.msgFilling;
            labelDI2Name.Content = Messages.msgHeating;
            labelDI3Name.Content = Messages.msgCooling;
            labelDI4Name.Content = Messages.msgEmptying;
            labelDI5Name.Content = "1 " + Messages.msgCycle;

            /* Ustawienie wartości dla analogów */
            labelAO1Value.Content = Math.Round(_reaktor.OutFlowingValveOpening / 10.0, 1) + "V";
            labelAO2Value.Content = Math.Round(_reaktor.HeatingValveValue / 10.0, 1) + "V";

            labelAI1Value.Content = Math.Round(_reaktor.ContainerPercentageFilling / 10.0, 1) + "V";
            labelAI2Value.Content = Math.Round(_reaktor.ReactorTemperature * (10.0 / 220.0), 1) + "V";
            labelAI3Value.Content = Math.Round(FillingSetPoint / 10.0, 1) + "V";
            labelAI4Value.Content = Math.Round(TemperatureSetPoint * (10.0 / 220.0), 1) + "V";

            /* Ustawienie etykiet */
            setDOCaptions(ref _lblDOCaptions);
            setAICaptions(ref _lblAOCaptions);
            setAOCaptions(ref _lblAICaptions);
            setDICaptions(ref _lblDICaptions);
        }
        /// <summary>
        /// Ustawia etykiety DI.
        /// </summary>
        /// <param name="a_DICaptions">Uporządkowana lista Labeli tekstowych.</param>
        private void                setDICaptions(ref List<Label> a_DICaptions)
        {
            string tmpStart = Properties.Settings.Default.PLCDIStartSymbol;
            int tmpStartValue = Properties.Settings.Default.PLCDIStartValue;
            int tmpStartValueBit = Properties.Settings.Default.PLCDIStartValueBit;
            int tmpBit = Properties.Settings.Default.PLCDIBit;
            int tmpPLCType = Properties.Settings.Default.PLCChoice;
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
                    a_DICaptions[i].Content = tmpStart + tmpStartValue + "." + (tmpStartValueBit + i);
                }
                else
                    a_DICaptions[i].Content = tmpStart + (tmpStartValue + i);
                k++;
            }
        }
        /// <summary>
        /// Ustawia etykiety DO.
        /// </summary>
        /// <param name="a_DICaptions">Uporządkowana lista Labeli tekstowych.</param>
        private void                setDOCaptions(ref List<Label> a_DOCaptions)
        {
            string tmpStart = Properties.Settings.Default.PLCDOStartSymbol;
            int tmpStartValue = Properties.Settings.Default.PLCDOStartValue;
            int tmpStartValueBit = Properties.Settings.Default.PLCDOStartValueBit;
            int tmpBit = Properties.Settings.Default.PLCDOBit;
            int tmpPLCType = Properties.Settings.Default.PLCChoice;
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
        /// Ustawia etykiety AO.
        /// </summary>
        /// <param name="a_AOCaptions">Uporządkowana lista Labeli tekstowych.</param>
        private void                setAOCaptions(ref List<Label> a_AOCaptions)
        {
            string tmpStart = Properties.Settings.Default.PLCAOStartSymbol;
            int tmpStartValue = Properties.Settings.Default.PLCAOStartValue;
            int tmpStartValueBit = Properties.Settings.Default.PLCAOStartValueBit;
            int tmpBit = Properties.Settings.Default.PLCAOBit;
            int tmpPLCType = Properties.Settings.Default.PLCChoice;
            int k = 0;
            for (int i = 0; i < a_AOCaptions.Count; i++)
            {
                if (tmpBit > 0)
                {
                    if (tmpBit <= i)
                    {
                        tmpBit += tmpBit;
                        tmpStartValue++;
                        k = 0;
                    }
                    a_AOCaptions[i].Content = tmpStart + tmpStartValue + "." + (tmpStartValueBit + i);
                }
                else
                    a_AOCaptions[i].Content = tmpStart + (tmpStartValue + i);
                k++;
            }
        }
        /// <summary>
        /// Ustawia etykiety AI.
        /// </summary>
        /// <param name="a_AICaptions">Uporządkowana lista Labeli tekstowych.</param>
        private void                setAICaptions(ref List<Label> a_AICaptions)
        {
            string tmpStart = Properties.Settings.Default.PLCAIStartSymbol;
            int tmpStartValue = Properties.Settings.Default.PLCAIStartValue;
            int tmpStartValueBit = Properties.Settings.Default.PLCAIStartValueBit;
            int tmpBit = Properties.Settings.Default.PLCAIBit;
            int tmpPLCType = Properties.Settings.Default.PLCChoice;
            int k = 0;
            for (int i = 0; i < a_AICaptions.Count; i++)
            {
                if (tmpBit > 0)
                {
                    if (tmpBit <= i)
                    {
                        tmpBit += tmpBit;
                        tmpStartValue++;
                        k = 0;
                    }
                    a_AICaptions[i].Content = tmpStart + tmpStartValue + "." + (tmpStartValueBit + i);
                }
                else
                    a_AICaptions[i].Content = tmpStart + (tmpStartValue + i);
                k++;
            }
        }
        /// <summary>
        /// Obsługa zmiany wartości na sliderze związanym z prędkością symulacji.
        /// </summary>
        private void                sliderFastness_MouseLeave(object sender, MouseEventArgs e)
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, (int)sliderFastness.Value);
        }
        /// <summary>
        /// Obsługa zmiany wartości na sliderze związanym z temperaturą zadaną.
        /// </summary>
        private void                sliderTemperature_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TemperatureSetPoint = (int)sliderTemperature.Value;
        }
        /// <summary>
        /// Obsługa zmiany wartości na sliderze związanym z wartością zadaną wypełnienia.
        /// </summary>
        private void                sliderFill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FillingSetPoint = (int)sliderFill.Value;
        }
        /// <summary>
        /// Obsługa timer'a dla kontrolki.
        /// </summary> 
        private void                Timer_Tick(object sender, EventArgs e)
        {
            /* Pobranie aktualnych stanów wejść */
            USBComm.usbGetAllDI(_DIData);
            USBComm.usbGetAllAI(_AIData);
            /* Działania w zalęzności od trybu */
            if (State == TabStates.DEMO && SimulationEnabled)
            {
                /* Obliczenia */
                calculateDemo();
                /* Wykonuje krok symulacji */
                _reaktor.DiscretStep();
            }
            /* Jeżeli kontrolka ma zezwolenie na wystawianie wyjść */
            if (AllowOutputs)
            {
                if (State == TabStates.SIMULATION)
                {
                    /* Ustawienie wejść klasy Reaktor */
                    setInputs();
                    /* Wykonuje krok symulacji */
                    _reaktor.DiscretStep();
                }
                if (State == TabStates.TEST)
                {
                    /* Ustawienie wejść klasy Reaktor */
                    setInputs();
                    /* Obliczenia */
                    calculateTest();
                }
                /* Aktualizacja wyjść */
                if (State == TabStates.SIMULATION || State == TabStates.TEST)
                    setOutputs();
            }
            /* Aktualizacja grafiki reaktora */
            updateReactorImg();
            /* Aktualizacja etykiet tekstowych */
            setText();
        }
        /// <summary>
        /// Ustawia wejścia klasy Reaktor.
        /// </summary>
        private void                setInputs()
        {
            /** Przekazanie stanu wejść do kalsy Reaktor **/
            /* Q1 --> V1 */
            _reaktor.AValveEnabled = Convert.ToBoolean(_DIData[0]);
            /* Q2 --> V2 */
            _reaktor.BValveEnabled = Convert.ToBoolean(_DIData[1]);
            /* Q3 --> V3 */
            _reaktor.CoolingValveEnabled = Convert.ToBoolean(_DIData[2]);
            /* Q4 --> V4 */
            _reaktor.CoolingOutputValveEnabled = Convert.ToBoolean(_DIData[3]);
            /* Q5 --> V5 */
            _reaktor.HeatingOutputValveEnabled = Convert.ToBoolean(_DIData[4]);
            /* Q6 --> Mieszadło */
            _reaktor.MixerEnable = Convert.ToBoolean(_DIData[5]);
            /* AQ1 --> AV1 - Odpływ */
            /* Przeliczenie wartości - liniowa zależnośc: [0 - 100]% --> [0 - 4096] stąd [0 - 100]% = 0.0244 * [0 - 4096] */
            _reaktor.OutFlowingValveOpening = Math.Round(0.0244 * _AIData[0], 1);
            /* AQ2 --> AV2 - dopływ pary grzejącej */
            /* Przeliczenie wartości - liniowa zależnośc: [0 - 100]% --> [0 - 4096] stąd [0 - 100]% = 0.0244 * [0 - 4096] */
            _reaktor.HeatingValveValue = Math.Round(0.0244 * _AIData[1], 1);
        }
        /// <summary>
        /// Ustawia odpowiednie wyjścia dla driver'a USB.
        /// </summary>
        private void                setOutputs()
        {
            /* Przeliczenie wypełnienia zbiornika: 
             * [0 - 4096] --> [0 - 100]% */
            _dicAOData[0] = Convert.ToInt16(_reaktor.ContainerPercentageFilling * 40.96);

            /* Temperatura w reaktorze przeliczana jest:
             * [0 - 4096] --> [0 - 220]oC */
            _dicAOData[1] = Convert.ToInt16(_reaktor.ReactorTemperature * 18.61818182);

            /* Przeliczenie wartości zadanej wypełnienia zbiornika: 
             * [0 - 4096] --> [0 - 100]% */
            _dicAOData[2] = Convert.ToInt16(FillingSetPoint * 40.96);

            /* Temperatura zadana przeliczana jest:
             * [0 - 4096] --> [0 - 220]oC */
            _dicAOData[3] = Convert.ToInt16(TemperatureSetPoint * 18.61818182);

            /* Wyprowadzenie wartości analogowych na wyjścia driver'a */
            foreach (int i in _dicAOData.Keys)
                USBComm.usbSetAO(i, _dicAOData[i]);
        }
        /// <summary>
        /// Obliczanie stanu reaktora w trybie DEMO.
        /// </summary>
        private void                calculateDemo()
        {
            /* Gdy uruchomiona symulacja */
            if (SimulationEnabled)
            {
                /* Maszyna stanów dla trybu DEMO reaktora */
                switch (StateCode)
                {
                    case 0:
                        {
                            /* Gdy poziom mniejszy od zadanego */
                            if (_reaktor.ContainerPercentageFilling < FillingSetPoint)
                            {
                                /* Otwieram zawory */
                                _reaktor.AValveEnabled = true;
                                _reaktor.BValveEnabled = true;
                            }
                            /* Gdy poziom zadany osiągnięty */
                            else
                            {
                                /* Zamykam zawory */
                                _reaktor.BValveEnabled = false;
                                _reaktor.AValveEnabled = false;
                                StateCode = 1;
                            }
                        }
                        break;
                    case 1:
                        {
                            /* Gdy temperatura niższa od zadanej */
                            if (_reaktor.ReactorTemperature < TemperatureSetPoint)
                            {
                                /* Coraz bardziej owieram zawór pary */
                                _reaktor.HeatingValveValue += 10;
                                /* Otwieram zawór wylotowy pary */
                                _reaktor.HeatingOutputValveEnabled = true;
                            }
                            /* Gdy ogrzano do odpowiedniej temperatury */
                            else
                            {
                                /* Zamykam zawór pary */
                                _reaktor.HeatingValveValue = 0;
                                /* Zamykam zawór wylotowy */
                                _reaktor.HeatingOutputValveEnabled = false;
                                StateCode = 2;
                                _nWait = 0;
                            }
                        }
                        break;
                    case 2:
                        {
                            /* Stabilizacja temperatury przez zadany czas */
                            _nWait++;
                            if (_nWait > 30)
                                StateCode = 3;
                        }
                        break;
                    case 3:
                        {
                            /* Gdy temperatura większa od otoczenia */
                            if (_reaktor.ReactorTemperature > 20)
                            {
                                /* Otiwieram zawór wody chłodzącej */
                                _reaktor.CoolingValveEnabled = true;
                                /* Otwieram zawór wylotowy wody */
                                _reaktor.CoolingOutputValveEnabled = true;
                            }
                            /* Po schłodzeniu */
                            else
                            {
                                /* Zamykam zawór wody chłodzącej */
                                _reaktor.CoolingValveEnabled = false;
                                /* Zamykam zawór wylotowy wody */
                                _reaktor.CoolingOutputValveEnabled = false;
                                StateCode = 4;
                            }
                        }
                        break;
                    case 4:
                        {
                            /* Gdy poziom większy od zera */
                            if (Math.Round(_reaktor.ContainerPercentageFilling, 2) > 1.5)
                                /* Coraz bardziej otwieram zawór wylotowy */
                                _reaktor.OutFlowingValveOpening += 5;
                            /* Gdy opróżniono zbiornik */
                            else
                            {
                                /* Zamykam zawór wylotowy */
                                _reaktor.OutFlowingValveOpening = 0;
                                StateCode = 0;
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Aktualizacja grafiki wyświetlanej na kontrolce.
        /// </summary>
        private void                updateReactorImg()
        {
            /** Zobrazowanie stanu reaktora: **/
            /* Wypełnienie raktora*/
            RectangleGeometry rgWypelnienie = new RectangleGeometry();
            rgWypelnienie.Rect = new Rect(1070, (Math.Round(_reaktor.ContainerPercentageFilling, 1) * (-6.7) + 1380), 650, 670);
            imgFilling.Clip = rgWypelnienie;
            labelFilling.Content = "H=" + Math.Round(_reaktor.ContainerPercentageFilling, 0) + " %";

            /* Temperatura reaktora */
            labelTemperature.Content = "T=" + Math.Round(_reaktor.ReactorTemperature, 0) + " oC";

            /* Przesunięcie zaworu analogowego A1 */
            Canvas.SetLeft(imgValveA1, _reaktor.HeatingValveValue * (-1.2));

            /* Przesunięcie zaworu analogowego A2 */
            Canvas.SetLeft(imgValveA2, _reaktor.OutFlowingValveOpening * 1.2);

            /* Animacja toru grzania */
            /* Przyjąłem wartośc 1 gdyż zadajnik nie jest w stanie "stabilnie" utrzymywać 0 */
            if (_reaktor.HeatingOutputValveEnabled && _reaktor.HeatingValveValue > 1)
                imgHeating.Visibility = Visibility.Visible;
            else
                imgHeating.Visibility = Visibility.Hidden;

            /* Animacja odpływu substancji */
            /* Przyjąłem wartośc 1 gdyż zadajnik nie jest w stanie "stabilnie" utrzymywać 0 */
            if (_reaktor.OutFlowingValveOpening > 1)
                imgOutflow.Visibility = Visibility.Visible;
            else
                imgOutflow.Visibility = Visibility.Hidden;

            /* Animacja dopływu czynnika A */
            if (_reaktor.AValveEnabled)
                imgSubstanceA.Visibility = Visibility.Visible;
            else
                imgSubstanceA.Visibility = Visibility.Hidden;

            /* Animacja dopływu czynnika B */
            if (_reaktor.BValveEnabled)
                imgSubstanceB.Visibility = Visibility.Visible;
            else
                imgSubstanceB.Visibility = Visibility.Hidden;

            /* Animacja czynnika chłodzącego */
            if (_reaktor.CoolingValveEnabled && _reaktor.CoolingOutputValveEnabled)
                imgCooling.Visibility = Visibility.Visible;
            else
                imgCooling.Visibility = Visibility.Hidden;

            /* Animacja mieszadła */
            if (_reaktor.MixerEnable)
            {
                imgMotorOn.Visibility = Visibility.Visible;
                imgMotorOff.Visibility = Visibility.Hidden;
            }
            else
            {
                imgMotorOff.Visibility = Visibility.Visible;
                imgMotorOn.Visibility = Visibility.Hidden;
            }

            /* Ostrzeżenie gdy nastąpiło przepełnienie lub temperatura reaktora przekroczyła 180oC (przyjęte nominalnie)*/
            if (_reaktor.Overflow || _reaktor.ReactorTemperature > 180)
            {
                labelWarning.Content = Messages.msgWarning;
                labelWarning.Visibility = Visibility.Visible;
            }
            else
                labelWarning.Visibility = Visibility.Hidden;
            /* Jeżeli wystąpiło przepełnienie bez przegrzania */
            if (_reaktor.Overflow && _reaktor.ReactorTemperature <= 180)
            {
                labelMsgWarning.Content = Messages.msgOverfill;
                labelMsgWarning.Visibility = Visibility.Visible;
            }
            /* Jeżeli wystapiło przegranie bez przepełnienia */
            else if (_reaktor.ReactorTemperature > 180 && !_reaktor.Overflow)
            {
                labelMsgWarning.Content = Messages.msgOverheat;
                labelMsgWarning.Visibility = Visibility.Visible;
            }
            /* Jeżeli wystąpiło przepełnienie i przegrzanie */
            else if (_reaktor.Overflow && _reaktor.ReactorTemperature > 180)
            {
                labelMsgWarning.Content = Messages.msgOverfill + " , " + Messages.msgOverheat;
                labelMsgWarning.Visibility = Visibility.Visible;
            }
            /* Jak nic z powyższych */
            else
                labelMsgWarning.Visibility = Visibility.Hidden;

            /* Zobrazowanie stanu przycisków */
            if (State == TabStates.SIMULATION || State == TabStates.TEST)
            {
                int[] DOdata = new int[16];
                /* Pobranie stanu z driver'a */
                USBComm.usbGetAllDO(DOdata);
                for (int i = 0; i < _buttonsOn.Count; i++)
                {
                    _buttonsOff[i].Visibility = (Convert.ToBoolean(DOdata[i]) ? Visibility.Collapsed : Visibility.Visible);
                    _buttonsOn[i].Visibility = (Convert.ToBoolean(DOdata[i]) ? Visibility.Visible : Visibility.Collapsed);
                    _lblDICaptions[i].Foreground = (Convert.ToBoolean(DOdata[i]) ? _brushOn : _brushOff);
                }
            }

            /* Ustawienie kolorów dla Caption'ów */
            _lblDOCaptions[0].Foreground = (_reaktor.AValveEnabled ? _brushOn : _brushOff);
            _lblDOCaptions[1].Foreground = (_reaktor.BValveEnabled ? _brushOn : _brushOff);
            _lblDOCaptions[2].Foreground = (_reaktor.CoolingValveEnabled ? _brushOn : _brushOff);
            _lblDOCaptions[3].Foreground = (_reaktor.CoolingOutputValveEnabled ? _brushOn : _brushOff);
            _lblDOCaptions[4].Foreground = (_reaktor.HeatingOutputValveEnabled ? _brushOn : _brushOff);
            _lblDOCaptions[5].Foreground = (_reaktor.MixerEnable ? _brushOn : _brushOff);
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
                for (int i = 0; i < 7; i++)
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
                    /* Ustawienie początkowego stanu wyjść */
                    for (int i = 0; i < 16; i++)
                        USBComm.usbSetDO(i, 0);
                    _htmlResult.close();
                    setTestModeText();
                    StateCode = 0;
                    putGrayHtmlText("Rozpoczęcie testu...</br>");
                }
            }
        }
        /// <summary>
        /// Obsługa przycisków uaktywnienia wyjścia cyfrowego.
        /// </summary>
        private void                buttonDIOn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                Image button = (Image)sender;
                int n = Int32.Parse(button.Name.Remove(0, 10));
                USBComm.usbSetDO(n - 1, 0);
            }
        }
        /// <summary>
        /// Obsługa przycisków deaktywacji wyjścia cyfrowego.
        /// </summary>
        private void                buttonDIOff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                Image button = (Image)sender;
                int n = Int32.Parse(button.Name.Remove(0, 11));
                USBComm.usbSetDO(n - 1, 1);
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
        /// Logika trybu test TEST.
        /// </summary>
        void                        calculateTest()
        {
            string tmpStr = "";
            /* Gdy uruchomiony tryb TEST */
            if (_bTestEnabled)
            {
                /* Ustawienie wejść klasy Reactor */
                setInputs();
                /** Początek testu **/
                /* Test na przycisk Napełnianie */
                if (StateCode == 0)
                {
                    if (_nWait == 0)
                    {
                        /* Dodaje informację o teście */
                        tmpStr = Messages.testFilling;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku Napełnianie */
                        USBComm.usbSetDO(0, 1);
                        /* Dodaje zdarzenie sprawdzania Napełniania do timera test */
                        _timerTest.Tick += new EventHandler(testFilling);
                        /* Wyzerowanie Overshot test */
                        _bStartOvershotTest = false;
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nInnerState = 0;
                    }
                }
                /* Test na przycisk Podgrzewanie */
                if (StateCode == 1)
                {
                    if (_nWait == 0)
                    {
                        /* Dodaje informację o teście */
                        tmpStr = Messages.testHeating;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku Podgrzewanie */
                        USBComm.usbSetDO(1, 1);
                        /* Dodaje zdarzenie sprawdzania Podgrzewania do timera test */
                        _timerTest.Tick += new EventHandler(testHeating);
                        /* Wyzerowanie Overshot test */
                        _bStartOvershotTest = false;
                    }
                    _nWait++;
                    /* Odczekaj 300ms (6) */
                    if (_nWait > 6)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nInnerState = 0;
                    }
                }
                /* Test na przycisk Chłodzenie */
                if (StateCode == 2)
                {
                    if (_nWait == 0)
                    {
                        /* Dodaje informację o teście */
                        tmpStr = Messages.testCooling;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku Chłodzenie */
                        USBComm.usbSetDO(2, 1);
                        /* Dodaje zdarzenie sprawdzania Chłodzenia do timera test */
                        _timerTest.Tick += new EventHandler(testCooling);
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nInnerState = 0;
                    }
                }
                /* Test na przycisk Opróżnianie */
                if (StateCode == 3)
                {
                    if (_nWait == 0)
                    {
                        /* Dodaje informację o teście */
                        tmpStr = Messages.testEmptying;
                        putGrayHtmlText(tmpStr);
                        /* Symuluje naciśnięcie przycisku Opróżnianie */
                        USBComm.usbSetDO(3, 1);
                        /* Dodaje zdarzenie sprawdzania Opróżniania do timera test */
                        _timerTest.Tick += new EventHandler(testEmptying);
                    }
                    _nWait++;
                    /* Odczekaj 100ms (2) */
                    if (_nWait > 2)
                    {
                        /* Start timera test */
                        _timerTest.Start();
                        StateCode = 100;
                        _nInnerState = 0;
                    }
                }
                if (StateCode == 4)
                {
                    tmpStr = Messages.testEnd;
                    putGrayHtmlText(tmpStr);
                    _bTestEnabled = false;
                    buttonPauseStart.Content = Messages.msgStartTest;
                    StateCode = 100;
                }
                /* Wykonywanie kroku symulacji */
                _reaktor.DiscretStep();
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu Napełnianie.
        /// </summary>
        void                        testFilling(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";
            List<CoilsStates> correctPLCDO = new List<CoilsStates>();
            List<CoilsStates> correctPLCAO = new List<CoilsStates>();

            /* Sprawdzam czy pojawiły się odpowiednie stany na wyjściu */
            if (_nInnerState == 0)
            {
                /* Określenie prawidłowych wyjść sterownika PLC dla tego etapu testu */
                correctPLCDO.AddRange(new CoilsStates[] { CoilsStates.TESTON, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF });
                correctPLCAO.AddRange(new CoilsStates[] { CoilsStates.TESTOFF, CoilsStates.TESTOFF });
                /* Sprawdzanie wyjść sterownika PLC */
                testInputs(ref tmpError, correctPLCDO, correctPLCAO);
                if (!Convert.ToBoolean(tmpError))
                {
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "1"; 
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "2";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testOvershot + " (10 [s])";
                    putGrayHtmlText(tmpStr);
                    _nWait = 0;
                    _nWait2 = 0;
                    _maxOvershot = 0.0;
                }
                _nInnerState++;
            }
            /* Sprawdzanie przeregulowania. */
            else if (_nInnerState == 1)
            {
                _nWait++;
                /* Jeżeli osiągnięto pożadane wypełnienie - zacznij testować przeregulowanie*/
                if (_reaktor.ContainerPercentageFilling >= _nFillingSetPoint && !_bStartOvershotTest)
                    _bStartOvershotTest = true;
                if (_bStartOvershotTest)
                {
                    double tmp = 0.0;
                    _nWait2++;
                    if (_reaktor.ContainerPercentageFilling > _nFillingSetPoint)
                        tmp = _reaktor.ContainerPercentageFilling - _nFillingSetPoint;
                    if (_reaktor.ContainerPercentageFilling < _nFillingSetPoint)
                        tmp = _nFillingSetPoint - _reaktor.ContainerPercentageFilling;
                    if (_maxOvershot < tmp)
                        _maxOvershot = tmp;
                    /* Inkrementuje czas */
                    _nWait2++;
                    /* Po 10[s] */
                    if (_nWait2 >= 200)
                    {
                        /* Zatrzymanie testu */
                        _timerTest.Stop();
                        _timerTest.Tick -= testFilling;
                        StateCode = 1;
                        USBComm.usbSetDO(0, 0);
                        tmpStr = Messages.testOvershotBorders + ": " + Math.Round(_maxOvershot, 1) + "%";
                        putGrayHtmlText(tmpStr);
                        tmpStr = Messages.testFilling + " " + Messages.testPass;
                        putGreenHtmlText(tmpStr, true);
                        _nWait = 0;
                    }
                }
                /* Po 10[s] bez osiągnięcia wartości zadanej */
                if (_nWait >= 200 && !_bStartOvershotTest)
                    tmpError = 3;
                /* Gdy nastąpi przepełnienie */
                if (_reaktor.Overflow)
                    tmpError = 4;
            }
            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testFilling + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                if (tmpError == 3)
                    tmpStr = Messages.testOvershotError;
                if (tmpError == 4)
                    tmpStr = Messages.testOverfillError;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _timerTest.Stop();
                _timerTest.Tick -= testFilling;
                StateCode = 1;
                USBComm.usbSetDO(0, 0);
                _nWait = 0;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu Ogrzewanie.
        /// </summary>
        void                        testHeating(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";
            List<CoilsStates> correctPLCDO = new List<CoilsStates>();
            List<CoilsStates> correctPLCAO = new List<CoilsStates>();

            /* Sprawdzam czy pojawiły się odpowiednie stany na wyjściu */
            if (_nInnerState == 0)
            {
                /* Określenie prawidłowych wyjść sterownika PLC dla tego etapu testu */
                correctPLCDO.AddRange(new CoilsStates[] { CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTON });
                correctPLCAO.AddRange(new CoilsStates[] { CoilsStates.TESTOFF, CoilsStates.TESTON });
                /* Sprawdzanie wyjść sterownika PLC */
                testInputs(ref tmpError, correctPLCDO, correctPLCAO);
                if (!Convert.ToBoolean(tmpError))
                {
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "5";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "6";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testAnalogOn + " " + Properties.Settings.Default.PLCAOStartSymbol + "2";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testOvershot + " (20 [s])";
                    putGrayHtmlText(tmpStr);
                    _nWait2 = 0;
                    _nWait = 0;
                    _maxOvershot = 0.0;
                }
                _nInnerState++;
            }
            /* Sprawdzanie przeregulowania. */
            else if (_nInnerState == 1)
            {
                _nWait++;
                /* Jeżeli osiągnięto pożadane wypełnienie - zacznij testować przeregulowanie*/
                if (_reaktor.ReactorTemperature >= _nTemperatureSetPoint && !_bStartOvershotTest)
                    _bStartOvershotTest = true;
                if (_bStartOvershotTest)
                {
                    double tmp = 0.0;
                    if (_reaktor.ReactorTemperature > _nTemperatureSetPoint)
                        tmp = _reaktor.ReactorTemperature - _nTemperatureSetPoint;
                    if (_reaktor.ReactorTemperature < _nTemperatureSetPoint)
                        tmp = _nTemperatureSetPoint - _reaktor.ReactorTemperature;
                    if (_maxOvershot < tmp)
                        _maxOvershot = tmp;
                    /* Inkrementuje czas */
                    _nWait2++;
                    /* Po 20[s] */
                    if (_nWait2 >= 200)
                    {
                        /* Zatrzymanie testu */
                        _timerTest.Stop();
                        _timerTest.Tick -= testHeating;
                        StateCode = 2;
                        USBComm.usbSetDO(1, 0);
                        tmpStr = Messages.testOvershotBorders + ": " + Math.Round(_maxOvershot, 1) + "oC";
                        putGrayHtmlText(tmpStr);
                        tmpStr = Messages.testHeating + " " + Messages.testPass;
                        putGreenHtmlText(tmpStr, true);
                        _nWait = 0;
                    }
                }
                /* Po 20[s] bez osiągnięcia wartości zadanej */
                if (_nWait >= 400 && !_bStartOvershotTest)
                    tmpError = 3;
                /* Gdy nastąpiło przegrzanie */
                if (_reaktor.ReactorTemperature > 180)
                    tmpError = 4;
            }
            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testHeating + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                if (tmpError == 3)
                    tmpStr = Messages.testOvershotError;
                if (tmpError == 4)
                    tmpStr = Messages.testOverheatError;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _timerTest.Stop();
                _timerTest.Tick -= testHeating;
                StateCode = 2;
                USBComm.usbSetDO(1, 0);
                _nWait = 0;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu Chłodzenia.
        /// </summary>
        void                        testCooling(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";
            List<CoilsStates> correctPLCDO = new List<CoilsStates>();
            List<CoilsStates> correctPLCAO = new List<CoilsStates>();

            /* Sprawdzam czy pojawiły się odpowiednie stany na wyjściu */
            if (_nInnerState == 0)
            {
                /* Określenie prawidłowych wyjść sterownika PLC dla tego etapu testu */
                correctPLCDO.AddRange(new CoilsStates[] { CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTON, CoilsStates.TESTON, CoilsStates.TESTOFF, CoilsStates.TESTON });
                correctPLCAO.AddRange(new CoilsStates[] { CoilsStates.TESTOFF, CoilsStates.TESTOFF });
                /* Sprawdzanie wyjść sterownika PLC */
                testInputs(ref tmpError, correctPLCDO, correctPLCAO);
                if (!Convert.ToBoolean(tmpError))
                {
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "3";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "4";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOn + " " + Properties.Settings.Default.PLCDOStartSymbol + "6";
                    putGreenHtmlText(tmpStr, true);
                }
                _nInnerState++;
            }
            /* Czekanie na ochłodzenie */
            else if (_nInnerState == 1)
            {
                _nWait++;
                if (!(Convert.ToBoolean(USBComm.usbGetDI(2))) && !(Convert.ToBoolean(USBComm.usbGetDI(3))))
                {
                    /* Zatrzymanie testu */
                    _timerTest.Stop();
                    _timerTest.Tick -= testCooling;
                    StateCode = 3;
                    USBComm.usbSetDO(2, 0);
                    _nWait = 0;
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOff + " " + Properties.Settings.Default.PLCDOStartSymbol + "3";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOff + " " + Properties.Settings.Default.PLCDOStartSymbol + "4";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testPLC + ": " + Messages.testDigitalOff + " " + Properties.Settings.Default.PLCDOStartSymbol + "6";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testCooling + " " + Messages.testPass;
                    putGreenHtmlText(tmpStr, true);
                }
                /* Po 10[s] bez osiągnięcia temperatury otoczenia */
                if (_nWait >= 200)
                    tmpError = 3;
            }

            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testCooling + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                if (tmpError == 3)
                    tmpStr = Messages.testOvershotError;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _timerTest.Stop();
                _timerTest.Tick -= testCooling;
                StateCode = 3;
                USBComm.usbSetDO(2, 0);
                _nWait = 0;
            }
        }
        /// <summary>
        /// Zdarzenie dodawane do _timerTest dla testu Opróżniania.
        /// </summary>
        void                        testEmptying(object sender, EventArgs e)
        {
            int tmpError = 0;
            string tmpStr = "";
            List<CoilsStates> correctPLCDO = new List<CoilsStates>();
            List<CoilsStates> correctPLCAO = new List<CoilsStates>();

            /* Sprawdzam czy pojawiły się odpowiednie stany na wyjściu */
            if (_nInnerState == 0)
            {
                /* Określenie prawidłowych wyjść sterownika PLC dla tego etapu testu */
                correctPLCDO.AddRange(new CoilsStates[] { CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF, CoilsStates.TESTOFF });
                correctPLCAO.AddRange(new CoilsStates[] { CoilsStates.TESTON, CoilsStates.TESTOFF });
                /* Sprawdzanie wyjść sterownika PLC */
                testInputs(ref tmpError, correctPLCDO, correctPLCAO);
                if (!Convert.ToBoolean(tmpError))
                {
                    tmpStr = Messages.testPLC + ": " + Messages.testAnalogOn + " " + Properties.Settings.Default.PLCAOStartSymbol + "1";
                    putGreenHtmlText(tmpStr, true);
                    _nWait = 0;
                }
                _nInnerState++;
            }
            /* Czekanie na opróżnienie */
            else if (_nInnerState == 1)
            {
                _nWait++;
                if (USBComm.usbGetAI(0) <= 100)
                {
                    /* Zatrzymanie testu */
                    _timerTest.Stop();
                    _timerTest.Tick -= testEmptying;
                    StateCode = 4;
                    USBComm.usbSetDO(3, 0);
                    _nWait = 0;
                    tmpStr = Messages.testPLC + ": " + Messages.testAnalogOff + " " + Properties.Settings.Default.PLCAOStartSymbol + "1";
                    putGreenHtmlText(tmpStr, true);
                    tmpStr = Messages.testEmptying + " " + Messages.testPass;
                    putGreenHtmlText(tmpStr, true);
                }
                /* Po 20[s] bez opróżnienia */
                if (_nWait >= 400)
                    tmpError = 3;
            }

            /* Gdy wystąpił błąd wyświetl informację i zatrzymaj TEST */
            if (Convert.ToBoolean(tmpError))
            {
                tmpStr = Messages.testEmptying + " " + Messages.testError;
                putRedHtmlText(tmpStr, false);
                /* Wyświetlenie informacji o błędzie*/
                if (tmpError == 1)
                    tmpStr = Messages.testErrorPLC;
                if (tmpError == 2)
                    tmpStr = Messages.testErrorTime;
                if (tmpError == 3)
                    tmpStr = Messages.testOvershotError;
                putRedHtmlText(tmpStr, false);
                /* Zatrzymanie testu */
                _timerTest.Stop();
                _timerTest.Tick -= testEmptying;
                StateCode = 4;
                USBComm.usbSetDO(3, 0);
                _nWait = 0;
            }
        }
        /// <summary>
        /// Testuje wyjścia cyfrowe i analogowe. Sprawdza czy są na nich pożadane sygnały.
        /// </summary>
        private void                testInputs(ref int tmpError, List<CoilsStates> coilsQ, List<CoilsStates> analogQ)
        {
            int[]   DIdata = new int[16];
            short[] AIdata = new short[4];
            string  tmpStr = "";

            USBComm.usbGetAllDI(DIdata);
            USBComm.usbGetAllAI(AIdata);
            /* Test wyjść cyfrowych sterownika */
            for (int i = 0; i < coilsQ.Count; i++)
            {
                if(coilsQ[i] >= 0)
                    if(Convert.ToBoolean(DIdata[i]) != Convert.ToBoolean(coilsQ[i]))
                    {
                        /* Jeżeli miało być ON/OFF */
                        tmpStr = Messages.testPLC + ": " + (coilsQ[i] == CoilsStates.TESTON ? Messages.testDigitalOff : Messages.testDigitalOn) +" " + Properties.Settings.Default.PLCDOStartSymbol + (i + 1);
                        putRedHtmlText(tmpStr, true);
                        /* Wystąpił błąd */
                        tmpError = 1;
                    }
            }
            /* Test wyjść analogowych steownika */
            for (int i = 0; i < analogQ.Count; i++)
            {
                /* Jeżeli ma być testowane */
                if (analogQ[i] >= 0)
                {
                    /* Jeżeli testowany stan załączenia na max */
                    if (analogQ[i] == CoilsStates.TESTON)
                        if (AIdata[i] < 4000)
                        {
                            /* Jeżeli miało być ON */
                            tmpStr = Messages.testPLC + ": " + Messages.testAnalogOff + " " + Properties.Settings.Default.PLCAOStartSymbol + (i + 1);
                            putRedHtmlText(tmpStr, true);
                            /* Wystąpił błąd */
                            tmpError = 1;
                        }
                    /* Jeżeli testowany stan wyłączenia na min */
                    if (analogQ[i] == CoilsStates.TESTOFF)
                        if (AIdata[i] > 100)
                        {
                            /* Jeżeli miało być OFF */
                            tmpStr = Messages.testPLC + ": " + Messages.testAnalogOn + " " + Properties.Settings.Default.PLCAOStartSymbol + (i + 1);
                            putRedHtmlText(tmpStr, true);
                            /* Wystąpił błąd */
                            tmpError = 1;
                        }
                }
            }
        }
        /// <summary>
        /// Wstawia czerwony tekst zawarty w argumencie do okna HTML pomocy.
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        /// <param name="a_bOK">Czy dodać drugą kolumnę w tabeli z informacją "NOK"</param>
        private void                putRedHtmlText(string a_sText, bool a_bOK)
        {
            string tmp = "";
            if (a_bOK)
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
            if (a_bOK)
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
        /// Czyta/Zmienia stan reaktora w trybie DEMO. 0 - napełnianie, 1 - ogrzewanie, 2 - chłodzenie, 3 - opróznianie.
        /// </summary>
        public int                  StateCode
        {
            get { return _nStateCode; }
            set { _nStateCode = value; }
        }
        /// <summary>
        /// Czyta/Zmienia wartośc zadaną wypełnienia zbiornika.
        /// </summary>
        public double               FillingSetPoint
        {
            get { return _nFillingSetPoint; }
            set { _nFillingSetPoint = value; }
        }
        /// <summary>
        /// Czyta/Zmienia wartość zadaną temperatury raktora.
        /// </summary>
        public double               TemperatureSetPoint
        {
            get { return _nTemperatureSetPoint; }
            set { _nTemperatureSetPoint = value; }
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
        /// Model reaktora dla trybu DEMO.
        /// </summary>
        PLCEmulator.Reactor.Reaktor _reaktor = new PLCEmulator.Reactor.Reaktor();
        /// <summary>
        /// Numer stanu dla trybu demo.
        /// </summary>
        int                         _nStateCode;
        /// <summary>
        /// Stany wewnętrzne przy tesowaniu.
        /// </summary>
        int                         _nInnerState;
        /// <summary>
        /// Wartość zadana wypełnienia reaktora.
        /// </summary>
        double                      _nFillingSetPoint;
        /// <summary>
        /// Wartość zadana temperatury reaktora.
        /// </summary>
        double                      _nTemperatureSetPoint;
        /// <summary>
        /// Kolor dla kontrolki działającego elementu.
        /// </summary>
        SolidColorBrush             _brushOn = new SolidColorBrush(Colors.LightGreen);
        /// <summary>
        /// Czarny kolor dla etykiety.
        /// </summary>
        SolidColorBrush             _brushOff = new SolidColorBrush(Colors.Black);
        /// <summary>
        /// Włączona/Wyłączona symulacja.
        /// </summary>
        bool                        _bSimulationEnabled;
        /// <summary>
        /// Przechowuje wartości wyjść analogowych dla driver'a USB.
        /// </summary>
        Dictionary<int, short>      _dicAOData = new Dictionary<int, short>();
        /// <summary>
        /// Przechowuje wartości wejść cyfrowych z driver'a USB.
        /// </summary>
        int[]                       _DIData = new int[16];
        /// <summary>
        /// Przechowuje wartości wejść analogowych z driver'a USB.
        /// </summary>
        short[]                     _AIData = new short[8];
        /// <summary>
        /// Zmienna dla trybu DEMO i TEST określa czas oczekiwania między zdarzeniami.
        /// </summary>
        int                         _nWait;
        /// <summary>
        /// Zmienna dla trybu DEMO i TEST określa czas oczekiwania między zdarzeniami.
        /// </summary>
        int                         _nWait2;
        /// <summary>
        /// Zezwala / zabrania na wystawianie wartości na wyjścia drivera USB.
        /// </summary>
        bool                        _bAllowOutputs;
        /// <summary>
        /// Lista etykiet DO.
        /// </summary>
        List<Label>                 _lblDOCaptions = new List<Label>();
        /// <summary>
        /// Lista etykiet AO.
        /// </summary>
        List<Label>                 _lblAOCaptions = new List<Label>();
        /// <summary>
        /// Lista etykiet AI.
        /// </summary>
        List<Label>                 _lblAICaptions = new List<Label>();
        /// <summary>
        /// Lista etykiet AI.
        /// </summary>
        List<Label>                 _lblDICaptions = new List<Label>();
        /// <summary>
        /// Przechowuje poprzednie stany AO z symulatora.
        /// </summary>
        List<int>                   _isSetAO = new List<int>();
        /// <summary>
        /// Lista przycisków On.
        /// </summary>
        List<Image>                 _buttonsOn = new List<Image>();
        /// <summary>
        /// Lista przycisków Off.
        /// </summary>
        List<Image>                 _buttonsOff = new List<Image>();
        /// <summary>
        /// Obiekt dla przechowywania elementów HTML'a dla okna pomocy.
        /// </summary>
        mshtml.IHTMLDocument2       _htmlResult = null;
        /// <summary>
        /// Informuje o stanie trybu TEST.
        /// </summary>
        bool                        _bTestEnabled;
        /// <summary>
        /// Maksymalna wartość przeregulowania.
        /// </summary>
        double                      _maxOvershot;
        /// <summary>
        /// Minimalna wartość przeregulowania.
        /// </summary>
        double                      _minOvershot;
        /// <summary>
        /// Informuje o rozpoczęciu sprawdzania przeregulowania.
        /// </summary>
        bool                        _bStartOvershotTest;
    }
}
