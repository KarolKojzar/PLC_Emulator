/* Sygnalizacja świetlna.
 * 
 * Autor: Karol Kojzar
 *        Automatyka i Robotyka
 *        Praca inzynierska
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
using PLCEmulator.Sygnalizacja;

namespace PLCEmulator.Sygnalizacja
{
    /// <summary>
    /// Interaction logic for ucSygnalizacja.xaml
    /// </summary>
    public partial class ucSygnalizacja : UserControl
    {
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="a_tsState">Tryb pracy kontrolki.</param>
        public ucSygnalizacja(TabStates a_tsState, ref mshtml.IHTMLDocument2 htmlDoc)
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
            _lblDOCaptions.Add((Label)FindName("labelDO7"));
            _lblDOCaptions.Add((Label)FindName("labelDO8"));
            _lblDOCaptions.Add((Label)FindName("labelDO9"));

            /* Przygotowanie kontrolki w zależności od trybu działania */
            if (State == TabStates.DEMO)
            {
                /* Ustawienie poczatkowego stanu */
                StateCode = 0;

            }
            if (State == TabStates.SIMULATION)
            {
                /* wyłączenie suwaka */
                stackPanelSterring.IsEnabled = false;
                /* Symulacja zawsze włączona */
                SimulationEnabled = true;
            }
            if (State == TabStates.TEST)
            {

                /* Symulacja zawsze włączona */
                SimulationEnabled = true;
                /* Wyłączenie suwaka prędkości symulacji */
                sliderFastness.IsEnabled = false;
                /* Ustawienie poczatkowego stanu */
                StateCode = 0;
            }

            samochod1_skret.Visibility = Visibility.Hidden;
            samochod1.Visibility = Visibility.Hidden;
            samochod2_skret.Visibility = Visibility.Hidden;
            samochod2.Visibility = Visibility.Hidden;
            samochod3_skret.Visibility = Visibility.Hidden;
            samochod3_skret_2.Visibility = Visibility.Hidden;
            samochod3.Visibility = Visibility.Hidden;
            samochod4_skret.Visibility = Visibility.Hidden;
            samochod4.Visibility = Visibility.Hidden;
            samochod5_skret.Visibility = Visibility.Hidden;
            samochod5.Visibility = Visibility.Hidden;
            samochod6_skret.Visibility = Visibility.Hidden;
            samochod6_skret_2.Visibility = Visibility.Hidden;
            samochod6.Visibility = Visibility.Hidden;
            zielone1.Visibility = Visibility.Hidden;
            zielone2.Visibility = Visibility.Hidden;
            zielone3.Visibility = Visibility.Hidden;
            zielone4.Visibility = Visibility.Hidden;
            zielone5.Visibility = Visibility.Hidden;
            zielone6.Visibility = Visibility.Hidden;
            sygnalizacje.Visibility = Visibility.Visible;


            /* Ustawienie timer'a. Odpytywanie co 100 ms (takie jak dla próbkowania obiektu) */
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            /* Ustawienie wartości na sliderFastness */
            sliderFastness.Value = 100;
            /* Przypisanie odpowiedniej metody */
            _timer.Tick += new EventHandler(Timer_Tick);
            /* Start timera */
            _timer.Start();
            /* Ustawienie etykiet tekstowych */
            setText();
        }

        /// <summary>
        /// Obsługa zmiany wartości na sliderze związanym z prędkością symulacji. ***********************
        /// </summary>
        private void sliderFastness_MouseLeave(object sender, MouseEventArgs e)
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, (int)sliderFastness.Value);
        }

        /// <summary>
        /// Obsługa timer'a dla kontrolki. **************************************************************
        /// </summary> 
        private void Timer_Tick(object sender, EventArgs e)
        {
            /* Pobranie aktualnych stanów wejść */
            USBComm.usbGetAllDI(_DIData);
            /* Pobranie aktualnych stanów wyjść */
            USBComm.usbGetAllDO(DOdata);

            
            /* Działania w zalęzności od trybu */
            if (State == TabStates.DEMO && SimulationEnabled)
            {
                /* Obliczenia */
                calculateDemo();
            }

            if (AllowOutputs)
            {
                if (State == TabStates.SIMULATION)
                {
                    /* Ustawienie wejść */
                    setOutputs();
                    grid1.Visibility = Visibility.Visible;
                    grid2.Visibility = Visibility.Visible;
                    labelDO9.Visibility = Visibility.Visible;
                    labelDO8.Visibility = Visibility.Visible;
                    labelDO7.Visibility = Visibility.Visible;
                    labelDO6.Visibility = Visibility.Visible;
                    labelDO5.Visibility = Visibility.Visible;
                    labelDO4.Visibility = Visibility.Visible;
                    labelDO3.Visibility = Visibility.Visible;
                    labelDO2.Visibility = Visibility.Visible;
                    labelDO1.Visibility = Visibility.Visible;
                }
                if (State == TabStates.TEST)
                {
                    calculateTest();
                    setTestOutputs();
                    grid1.Visibility = Visibility.Hidden;
                    grid2.Visibility = Visibility.Hidden;
                    labelDO9.Visibility = Visibility.Hidden;
                    labelDO8.Visibility = Visibility.Hidden;
                    labelDO7.Visibility = Visibility.Hidden;
                    labelDO6.Visibility = Visibility.Hidden;
                    labelDO5.Visibility = Visibility.Hidden;
                    labelDO4.Visibility = Visibility.Hidden;
                    labelDO3.Visibility = Visibility.Hidden;
                    labelDO2.Visibility = Visibility.Hidden;
                    labelDO1.Visibility = Visibility.Hidden;
                }   
            }
            /* Aktualizacja grafiki */
            updateSygnalizacjaImg();
            /* Aktualizacja etykiet tekstowych */
            setText();
        }

        /// <summary>
        /// Tryb TEST Sygnalizacji. *********************************************************************
        /// </summary>
        private void calculateTest()
        {
            string tmpStr = "";
            /* Gdy uruchomiony tryb TEST */
            if (_bTestEnabled)
            {
                switch (StateCode)
                {
                    case 0:
                        {
                            _nWait_car1 = 0;
                            _nWait_car2 = 0;
                            _nWait_car3 = 0;
                            _nWait_car4 = 0;
                            _nWait_car5 = 0;
                            _nWait_car6 = 0;
                            samochod1.Visibility = Visibility.Hidden;
                            samochod2.Visibility = Visibility.Hidden;
                            samochod3.Visibility = Visibility.Hidden;
                            samochod4.Visibility = Visibility.Hidden;
                            samochod5.Visibility = Visibility.Hidden;
                            samochod6.Visibility = Visibility.Hidden;
                            _sygnalizacja.pojazd1 = false;
                            _sygnalizacja.pojazd2 = false;
                            _sygnalizacja.pojazd3 = false;
                            _sygnalizacja.pojazd4 = false;
                            _sygnalizacja.pojazd5 = false;
                            _sygnalizacja.pojazd6 = false;
                            _sygnalizacja.awaria = false;
                            _licznik_testu = 0;
                            procenty = 0.0;
                            suma_pojazdów = 0.0;

                            StateCode = 1;
                        } break;

                    case 1:
                        {
                            if (_nWait == 30)
                            {
                                zmienna_losowa = x.Next(1, 7);

                                if (samochod1.Visibility == Visibility.Visible && zmienna_losowa == 1)
                                    zmienna_losowa = 100;
                                if (samochod2.Visibility == Visibility.Visible && zmienna_losowa == 2)
                                    zmienna_losowa = 100;
                                if (samochod3.Visibility == Visibility.Visible && zmienna_losowa == 3)
                                    zmienna_losowa = 100;
                                if (samochod4.Visibility == Visibility.Visible && zmienna_losowa == 4)
                                    zmienna_losowa = 100;
                                if (samochod5.Visibility == Visibility.Visible && zmienna_losowa == 5)
                                    zmienna_losowa = 100;
                                if (samochod6.Visibility == Visibility.Visible && zmienna_losowa == 6)
                                    zmienna_losowa = 100;

                                if (zmienna_losowa == 1)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest1;
                                    _sygnalizacja.pojazd1 = true;
                                }
                                if (zmienna_losowa == 2)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest2;
                                    _sygnalizacja.pojazd2 = true;
                                }
                                if (zmienna_losowa == 3)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest3;
                                    _sygnalizacja.pojazd3 = true;
                                }
                                if (zmienna_losowa == 4)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest4;
                                    _sygnalizacja.pojazd4 = true;
                                }
                                if (zmienna_losowa == 5)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest5;
                                    _sygnalizacja.pojazd5 = true;
                                }
                                if (zmienna_losowa == 6)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest6;
                                    _sygnalizacja.pojazd6 = true;
                                }

                                putGrayHtmlText(tmpStr);
                            }

                            if (_nWait < 30)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;
                            }

                            if (_Wait_Test < 330)
                            {
                                _Wait_Test++;
                            }
                            else
                            {
                                _nWait = 0;
                                StateCode = 2;
                                _Wait_Test = 0;
                            }

                        }
                        break;
                    case 2:
                        {
                            if (_nWait == 80)
                            {
                                tmpStr = Messages.msgTest7;
                                putGrayHtmlText(tmpStr);
                                _sygnalizacja.awaria = true;
                            }
                            if (_nWait < 120)
                            {
                                _nWait++;
                            }
                            else
                            {
                                tmpStr = Messages.testEnd;
                                putGrayHtmlText(tmpStr);

                                procenty = (Convert.ToDouble(_licznik_testu)) / suma_pojazdów * 100;

                                tmpStr = Math.Round(procenty, 0) + Messages.msgPodsumowanie;

                                if (procenty < 50)
                                    putRedHtmlText(tmpStr, false);
                                else
                                    if (procenty >= 50 && procenty <= 80)
                                        putGrayHtmlText(tmpStr);
                                    else
                                        putGreenHtmlText(tmpStr, false);
                                _bTestEnabled = false;
                                buttonPauseStart.Content = Messages.msgStartTest;
                                StateCode = 100;
                            }
                        }
                        break;
                }

                if (ruch_samochod1 == true && _pomocnicza1 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie1;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    _pomocnicza1 = false;
                }
                if (ruch_samochod1 == false)
                {
                    _pomocnicza1 = true;
                }

                if (ruch_samochod2 == true && _pomocnicza2 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie2;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza2 = false;
                }
                if (ruch_samochod2 == false)
                {
                    _pomocnicza2 = true;
                }

                if (ruch_samochod2_skret == true && _pomocnicza2_2 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie3;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza2_2 = false;
                }
                if (ruch_samochod2_skret == false)
                {
                    _pomocnicza2_2 = true;
                }

                if (ruch_samochod3 == true && _pomocnicza3 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie4;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza3 = false;
                }
                if (ruch_samochod3 == false)
                {
                    _pomocnicza3 = true;
                }

                if (ruch_samochod3_skret == true && _pomocnicza3_2 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie5;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza3_2 = false;
                }
                if (ruch_samochod3_skret == false)
                {
                    _pomocnicza3_2 = true;
                }

                if (ruch_samochod3_skret_2 == true && _pomocnicza3_3 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie6;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza3_3 = false;
                }
                if (ruch_samochod3_skret_2 == false)
                {
                    _pomocnicza3_3 = true;
                }

                if (ruch_samochod4 == true && _pomocnicza4 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie7;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza4 = false;
                }
                if (ruch_samochod4 == false)
                {
                    _pomocnicza4 = true;
                }

                if (ruch_samochod5 == true && _pomocnicza5 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie8;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza5 = false;
                }
                if (ruch_samochod5 == false)
                {
                    _pomocnicza5 = true;
                }

                if (ruch_samochod5_skret == true && _pomocnicza5_2 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie9;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza5_2 = false;
                }
                if (ruch_samochod5_skret == false)
                {
                    _pomocnicza5_2 = true;
                }

                if (ruch_samochod6 == true && _pomocnicza6 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie10;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza6 = false;
                }
                if (ruch_samochod6 == false)
                {
                    _pomocnicza6 = true;
                }

                if (ruch_samochod6_skret == true && _pomocnicza6_2 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie11;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza6_2 = false;
                }
                if (ruch_samochod6_skret == false)
                {
                    _pomocnicza6_2 = true;
                }

                if (ruch_samochod6_skret_2 == true && _pomocnicza6_3 == true && _Wait_Test != 0)
                {
                    if (labelWarning.Visibility == Visibility.Hidden && labelWarning_Car.Visibility == Visibility.Hidden
                        && labelAwaria.Visibility == Visibility.Hidden)
                    _licznik_testu++;
                    tmpStr = Messages.msgTest_opuszczenie12;
                    putGreenHtmlText(tmpStr, false);

                    _pomocnicza6_3 = false;
                }
                if (ruch_samochod6_skret_2 == false)
                {
                    _pomocnicza6_3 = true;
                }

                if (labelWarning.Visibility == Visibility.Visible && _kolizja == true)
                {
                    tmpStr = Messages.msgWarning;
                    putRedHtmlText(tmpStr, false);

                    _kolizja = false;
                }
                if (labelWarning.Visibility == Visibility.Hidden)
                {
                    _kolizja = true;
                }

                if (labelWarning_Car.Visibility == Visibility.Visible && _kolizja_car == true)
                {
                    tmpStr = Messages.msgWarning_car;
                    putRedHtmlText(tmpStr, false);

                    _kolizja_car = false;
                }
                if (labelWarning_Car.Visibility == Visibility.Hidden)
                {
                    _kolizja_car = true;
                }

                if (labelAwaria.Visibility == Visibility.Visible && _awaryjnosc == true)
                {
                    tmpStr = Messages.msgAwaria;
                    putRedHtmlText(tmpStr, false);

                    _awaryjnosc = false;
                }
                if (labelAwaria.Visibility == Visibility.Hidden)
                {
                    _awaryjnosc = true;
                }

            }
        }

        /// <summary>
        /// Tryb DEMO Sygnalizacji. *********************************************************************
        /// </summary>
        private void calculateDemo()
        {
            /* Gdy uruchomiona symulacja */
            if (SimulationEnabled)
            {
                /** Zapamiętanie poprzednich stanów wejść **/
                _sygnal1 = _sygnalizacja.Sygna1;
                _sygnal2 = _sygnalizacja.Sygna2;
                _sygnal3 = _sygnalizacja.Sygna3;
                _sygnal4 = _sygnalizacja.Sygna4;
                _sygnal5 = _sygnalizacja.Sygna5;
                _sygnal6 = _sygnalizacja.Sygna6;
                /* Maszyna stanów dla trybu DEMO sygnalizacji */
                switch (StateCode)
                {
                    case 0:
                        {
                            _sygnalizacja.Sygna1 = false;
                            _sygnalizacja.Sygna2 = false;
                            _sygnalizacja.Sygna3 = false;
                            _sygnalizacja.Sygna4 = false;
                            _sygnalizacja.Sygna5 = false;
                            _sygnalizacja.Sygna6 = false;
                            _sygnalizacja.Sygna7 = false;
                            _sygnalizacja.Sygna8 = false;
                            _sygnalizacja.awaria = false;

                            if (_nWait == 15)
                            {
                                _sygnalizacja.pojazd5 = true;
                                _sygnalizacja.pojazd2 = true;
                            }
                            if (_nWait < 30)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;
                                StateCode = 1;
                            }
                        }
                        break;
                    case 1:
                        {
                            _sygnalizacja.Sygna2 = true;
                            _sygnalizacja.Sygna5 = true;
                            if (_nWait == 50)
                            {
                                _sygnalizacja.pojazd1 = true;
                            }
                            if (_nWait < 80)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;                          
                                StateCode = 2;
                            }
                        }
                        break;
                    case 2:
                        {
                            _sygnalizacja.Sygna5 = false;
                            if (_nWait == 10)
                            {
                                _sygnalizacja.pojazd2 = true;
                            }
                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna1 = true;
                                _sygnalizacja.Sygna8 = true;
                            }
                            if (_nWait == 55)
                            {
                                _sygnalizacja.pojazd6 = true;
                                _sygnalizacja.pojazd3 = true;
                                _zmiana_samochodu3 = 2;
                            }
                            if (_nWait < 80)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;                          
                                StateCode = 3;
                            }
                        }
                        break;
                    case 3:
                        {
                            _sygnalizacja.Sygna1 = false;
                            _sygnalizacja.Sygna2 = false;
                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna8 = false;
                                _sygnalizacja.Sygna6 = true;
                                _sygnalizacja.Sygna3 = true;
                            }
                            if (_nWait == 65)
                            {
                                _sygnalizacja.pojazd1 = true;
                            }
                            if (_nWait == 75)
                            {
                                _sygnalizacja.pojazd4 = true;
                            }
                            if (_nWait < 80)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;
                                StateCode = 4;
                            }
                        }
                        break;
                    case 4:
                        {
                            _sygnalizacja.Sygna3 = false;
                            _sygnalizacja.Sygna6 = false;
                            if (_nWait == 1)
                            {
                                _sygnalizacja.pojazd6= true;
                            }
                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna1 = true;
                                _sygnalizacja.Sygna4 = true;
                                _sygnalizacja.Sygna7 = true;
                                _sygnalizacja.Sygna8 = true;
                            }
                            if (_nWait < 80)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;
                                StateCode = 5;
                            }
                        }
                        break;
                    case 5:
                        {
                            _sygnalizacja.awaria = true;
                            _awaria = true;
                            _sygnalizacja.Sygna1 = false;
                            _sygnalizacja.Sygna4 = false;
                            _sygnalizacja.Sygna7 = false;
                            _sygnalizacja.Sygna8 = false;
                            if (_nWait < 80)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;
                                _sygnalizacja.awaria = false;
                                _awaria = false;
                                StateCode = 0;
                            }
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// Ustawia wyjścia. ****************************************************************************
        /// </summary>
        private void setTestOutputs()
        {

            /** Zapamiętanie poprzednich stanów wejść **/
            _sygnal1 = _sygnalizacja.Sygna1;
            _sygnal2 = _sygnalizacja.Sygna2;
            _sygnal3 = _sygnalizacja.Sygna3;
            _sygnal4 = _sygnalizacja.Sygna4;
            _sygnal5 = _sygnalizacja.Sygna5;
            _sygnal6 = _sygnalizacja.Sygna6;
            /** Przekazanie aktualnych stanów wejść **/
            _sygnalizacja.Sygna1 = Convert.ToBoolean(_DIData[0]);
            _sygnalizacja.Sygna2 = Convert.ToBoolean(_DIData[1]);
            _sygnalizacja.Sygna3 = Convert.ToBoolean(_DIData[2]);
            _sygnalizacja.Sygna4 = Convert.ToBoolean(_DIData[3]);
            _sygnalizacja.Sygna5 = Convert.ToBoolean(_DIData[4]);
            _sygnalizacja.Sygna6 = Convert.ToBoolean(_DIData[5]);
            _sygnalizacja.Sygna7 = Convert.ToBoolean(_DIData[6]);
            _sygnalizacja.Sygna8 = Convert.ToBoolean(_DIData[7]);
            _awaria = Convert.ToBoolean(_DIData[8]);
        }

        /// <summary>
        /// Ustawia wyjścia. ****************************************************************************
        /// </summary>
        private void setOutputs()
        {

            /** Zapamiętanie poprzednich stanów wejść **/
            _sygnal1 = _sygnalizacja.Sygna1;
            _sygnal2 = _sygnalizacja.Sygna2;
            _sygnal3 = _sygnalizacja.Sygna3;
            _sygnal4 = _sygnalizacja.Sygna4;
            _sygnal5 = _sygnalizacja.Sygna5;
            _sygnal6 = _sygnalizacja.Sygna6;
            /** Przekazanie aktualnych stanów wejść **/
            _sygnalizacja.Sygna1 = Convert.ToBoolean(_DIData[0]);
            _sygnalizacja.Sygna2 = Convert.ToBoolean(_DIData[1]);
            _sygnalizacja.Sygna3 = Convert.ToBoolean(_DIData[2]);
            _sygnalizacja.Sygna4 = Convert.ToBoolean(_DIData[3]);
            _sygnalizacja.Sygna5 = Convert.ToBoolean(_DIData[4]);
            _sygnalizacja.Sygna6 = Convert.ToBoolean(_DIData[5]);
            _sygnalizacja.Sygna7 = Convert.ToBoolean(_DIData[6]);
            _sygnalizacja.Sygna8 = Convert.ToBoolean(_DIData[7]);
            _awaria = Convert.ToBoolean(_DIData[8]);

            if (DOdata[0] == 1)
                _sygnalizacja.pojazd1 = true;
            else
                _sygnalizacja.pojazd1 = false;
            if (DOdata[1] == 1)
                _sygnalizacja.pojazd2 = true;
            else
                _sygnalizacja.pojazd2 = false;
            if (DOdata[2] == 1)
                _sygnalizacja.pojazd3 = true;
            else
                _sygnalizacja.pojazd3 = false;
            if (DOdata[3] == 1)
                _sygnalizacja.pojazd4 = true;
            else
                _sygnalizacja.pojazd4 = false;
            if (DOdata[4] == 1)
                _sygnalizacja.pojazd5 = true;
            else
                _sygnalizacja.pojazd5 = false;
            if (DOdata[5] == 1)
                _sygnalizacja.pojazd6 = true;
            else
                _sygnalizacja.pojazd6 = false;
            if (DOdata[6] == 1)
                _sygnalizacja.awaria = true;
            else
                _sygnalizacja.awaria = false;
        }
        /// <summary>
        /// Aktualizacja grafiki wyświetlanej na kontrolce. *********************************************
        /// </summary>
        private void updateSygnalizacjaImg()
        {
             // jezeli jest klikniety przycisk 1 pokaż pojazd 1, w przeciwnym wypadku wyłącz go.
             // analogicznie pozostałe 5 pojazdów
            if (_sygnalizacja.pojazd1 == true && _nWait_car1 == 0)
            {
                Canvas.SetTop(samochod1, 0.0);
                samochod1.Visibility = Visibility.Visible;
                Canvas.SetRight(samochod1_skret, 0.0);
                
            }

            if (_sygnalizacja.pojazd2 == true && _nWait_car2 == 0)
            {
                Canvas.SetTop(samochod2, 0.0);
                Canvas.SetLeft(samochod2_skret, 0.0);
                samochod2.Visibility = Visibility.Visible;
            }

            if (_sygnalizacja.pojazd3 == true && _nWait_car3 == 0)
            {
                Canvas.SetRight(samochod3, 0.0);
                Canvas.SetTop(samochod3_skret, 0.0);
                Canvas.SetTop(samochod3_skret_2, 0.0);
                samochod3.Visibility = Visibility.Visible;
            }

             if (_sygnalizacja.pojazd4 == true && _nWait_car4 == 0)
             {
                 Canvas.SetTop(samochod4, 0.0);
                 samochod4.Visibility = Visibility.Visible;
                 Canvas.SetLeft(samochod4_skret, 0.0);
             }

             if (_sygnalizacja.pojazd5 == true && _nWait_car5 == 0)
             {
                 Canvas.SetTop(samochod5, 0.0);
                 Canvas.SetRight(samochod5_skret, 0.0);
                 samochod5.Visibility = Visibility.Visible;
             }

             if (_sygnalizacja.pojazd6 == true && _nWait_car6 == 0)
             {
                 Canvas.SetLeft(samochod6, 0.0);
                 Canvas.SetTop(samochod6_skret, 0.0);
                 Canvas.SetTop(samochod6_skret_2, 0.0);
                 samochod6.Visibility = Visibility.Visible;
             }

             if ((_awaria == false && _sygnalizacja.awaria == true) || (_awaria == true && _sygnalizacja.awaria == false))
             {
                 labelAwaria.Content = Messages.msgAwaria;
                 labelAwaria.Visibility = Visibility.Visible;
             }
             else
                 labelAwaria.Visibility = Visibility.Hidden;

            //jeśli przycisk awarii nie jest kliknięty wykonuje się normalna praca sygnalizacji
             if (_awaria == false && !_resetSwiatel)
             {
                 if (_sygnalizacja.Sygna1 == true && samochod1.Visibility == Visibility.Visible)
                 {
                     ruch_samochod1 = true;
                 }

                 if (_sygnalizacja.Sygna2 == true && samochod2.Visibility == Visibility.Visible && _zmiana_samochodu2 == false)
                 {
                     ruch_samochod2 = true;
                 }
                 if (_sygnalizacja.Sygna2 == true && samochod2.Visibility == Visibility.Visible && _zmiana_samochodu2 == true)
                 {
                     ruch_samochod2_skret = true;
                 }

                 if (_sygnalizacja.Sygna3 == true && samochod3.Visibility == Visibility.Visible && _zmiana_samochodu3 == 0)
                 {
                     ruch_samochod3 = true;
                 }

                 if ((_sygnalizacja.Sygna7 == true || _sygnalizacja.Sygna3 == true) && samochod3.Visibility == Visibility.Visible && _zmiana_samochodu3 == 1)
                 {
                     ruch_samochod3_skret = true;
                 }

                 if (_sygnalizacja.Sygna3 == true && samochod3.Visibility == Visibility.Visible && _zmiana_samochodu3 == 2)
                 {
                     ruch_samochod3_skret_2 = true;
                 }

                 if (_sygnalizacja.Sygna4 == true && samochod4.Visibility == Visibility.Visible)
                 {
                     ruch_samochod4 = true;
                 }

                 if (_sygnalizacja.Sygna5 == true && samochod5.Visibility == Visibility.Visible && _zmiana_samochodu5 == false)
                 {
                     ruch_samochod5 = true;
                 }

                 if (_sygnalizacja.Sygna5 == true && samochod5.Visibility == Visibility.Visible && _zmiana_samochodu5 == true)
                 {
                     ruch_samochod5_skret = true;
                 }

                 if (_sygnalizacja.Sygna6 == true && samochod6.Visibility == Visibility.Visible && _zmiana_samochodu6 == 2)
                 {
                     ruch_samochod6_skret_2 = true;
                 }

                 if (_sygnalizacja.Sygna6 == true && samochod6.Visibility == Visibility.Visible && _zmiana_samochodu6 == 0)
                 {
                     ruch_samochod6 = true;
                 }

                 if ((_sygnalizacja.Sygna8 == true || _sygnalizacja.Sygna6 == true) && samochod6.Visibility == Visibility.Visible && _zmiana_samochodu6 == 1)
                 {
                     ruch_samochod6_skret = true;
                 }

                 /* Animacja sygnalizacji */
                 // sygnalizacja 1
                 if (_sygnalizacja.Sygna1 == true && _sygnal1 == false)
                 {
                     _zmiana1 = true;
                 }
                 if (_sygnalizacja.Sygna1 == false && _sygnal1 == true)
                 {
                     _zmiana2 = true;
                 }

                 // sygnalizacja 2
                 if (_sygnalizacja.Sygna2 == true && _sygnal2 == false)
                 {
                     _zmiana3 = true;
                 }
                 if (_sygnalizacja.Sygna2 == false && _sygnal2 == true)
                 {
                     _zmiana4 = true;
                 }
                 

                 // sygnalizacja 3
                 if (_sygnalizacja.Sygna3 == true && _sygnal3 == false)
                 {
                     _zmiana5 = true;
                 }
                 if (_sygnalizacja.Sygna3 == false && _sygnal3 == true)
                 {
                     _zmiana6 = true;
                 }
                 

                 // sygnalizacja 4
                 if (_sygnalizacja.Sygna4 == true && _sygnal4 == false)
                 {
                     _zmiana7 = true;
                 }
                 if (_sygnalizacja.Sygna4 == false && _sygnal4 == true)
                 {
                     _zmiana8 = true;
                 }


                 // sygnalizacja 5
                 if (_sygnalizacja.Sygna5 == true && _sygnal5 == false)
                 {
                     _zmiana9 = true;
                 }
                 if (_sygnalizacja.Sygna5 == false && _sygnal5 == true)
                 {
                     _zmiana10 = true;
                 }

                 // sygnalizacja 6
                 if (_sygnalizacja.Sygna6 == true && _sygnal6 == false)
                 {
                     _zmiana11 = true;
                 }
                 if (_sygnalizacja.Sygna6 == false && _sygnal6 == true)
                 {
                     _zmiana12 = true;
                 }


                 // sygnalizacja 7
                 if (_sygnalizacja.Sygna7)
                 {
                     strzalka_1_brak.Visibility = Visibility.Hidden;
                     strzalka_1.Visibility = Visibility.Visible;
                 }
                 else
                 {
                     strzalka_1.Visibility = Visibility.Hidden;
                     strzalka_1_brak.Visibility = Visibility.Visible;
                 }

                 // sygnalizacja 8
                 if (_sygnalizacja.Sygna8)
                 {
                     strzalka_2_brak.Visibility = Visibility.Hidden;
                     strzalka_2.Visibility = Visibility.Visible;
                 }
                 else
                 {
                     strzalka_2.Visibility = Visibility.Hidden;
                     strzalka_2_brak.Visibility = Visibility.Visible;
                 }




                 /* Ustawienie kolorów dla Caption'ów */
                 _lblDOCaptions[0].Foreground = (_sygnalizacja.Sygna1 ? _brushOn : _brushOff);
                 _lblDOCaptions[1].Foreground = (_sygnalizacja.Sygna2 ? _brushOn : _brushOff);
                 _lblDOCaptions[2].Foreground = (_sygnalizacja.Sygna3 ? _brushOn : _brushOff);
                 _lblDOCaptions[3].Foreground = (_sygnalizacja.Sygna4 ? _brushOn : _brushOff);
                 _lblDOCaptions[4].Foreground = (_sygnalizacja.Sygna5 ? _brushOn : _brushOff);
                 _lblDOCaptions[5].Foreground = (_sygnalizacja.Sygna6 ? _brushOn : _brushOff);
                 _lblDOCaptions[6].Foreground = (_sygnalizacja.Sygna7 ? _brushOn : _brushOff);
                 _lblDOCaptions[7].Foreground = (_sygnalizacja.Sygna8 ? _brushOn : _brushOff);

                 /* Ostrzeżenie gdy nastąpi kolizja świateł*/
                 if ((_sygnalizacja.Sygna1 && _sygnalizacja.Sygna5) || (_sygnalizacja.Sygna1 && _sygnalizacja.Sygna6)
                     || (_sygnalizacja.Sygna1 && _sygnalizacja.Sygna3) || (_sygnalizacja.Sygna2 && _sygnalizacja.Sygna3)
                     || (_sygnalizacja.Sygna2 && _sygnalizacja.Sygna4) || (_sygnalizacja.Sygna2 && _sygnalizacja.Sygna6)
                     || (_sygnalizacja.Sygna3 && _sygnalizacja.Sygna4) || (_sygnalizacja.Sygna3 && _sygnalizacja.Sygna5)
                     || (_sygnalizacja.Sygna4 && _sygnalizacja.Sygna6) || (_sygnalizacja.Sygna5 && _sygnalizacja.Sygna6)
                     || (_sygnalizacja.Sygna7 && _sygnalizacja.Sygna2) || (_sygnalizacja.Sygna8 && _sygnalizacja.Sygna5))
                 {
                     labelWarning.Content = Messages.msgWarning;
                     labelWarning.Visibility = Visibility.Visible;
                 }
                 else
                     labelWarning.Visibility = Visibility.Hidden;

                 /* Ostrzeżenie gdy nastąpi kolizja pojazdów*/
                 if ((ruch_samochod1 && ruch_samochod3) || (ruch_samochod1 && ruch_samochod3_skret_2) || (ruch_samochod1 && ruch_samochod5)
                     || (ruch_samochod1 && ruch_samochod5_skret) || (ruch_samochod1 && ruch_samochod6) || (ruch_samochod1 && ruch_samochod6_skret_2)
                     || (ruch_samochod2_skret && ruch_samochod4) || (ruch_samochod2_skret && ruch_samochod6) || (ruch_samochod2_skret && ruch_samochod6_skret_2)
                     || (ruch_samochod2 && ruch_samochod3) || (ruch_samochod2 && ruch_samochod3_skret) || (ruch_samochod2 && ruch_samochod3_skret_2)
                     || (ruch_samochod2 && ruch_samochod4) || (ruch_samochod2 && ruch_samochod6) || (ruch_samochod2 && ruch_samochod6_skret_2)
                     || (ruch_samochod3 && ruch_samochod4) || (ruch_samochod3 && ruch_samochod5) || (ruch_samochod3 && ruch_samochod5_skret)
                     || (ruch_samochod3_skret_2 && ruch_samochod4) || (ruch_samochod3_skret_2 && ruch_samochod5) || (ruch_samochod3_skret_2 && ruch_samochod5_skret)
                     || (ruch_samochod4 && ruch_samochod6) || (ruch_samochod4 && ruch_samochod3_skret_2) || (ruch_samochod5 && ruch_samochod6)
                     || (ruch_samochod5 && ruch_samochod6_skret) || (ruch_samochod5 && ruch_samochod6_skret_2) || (ruch_samochod5_skret && ruch_samochod6))
                 {
                     labelWarning_Car.Content = Messages.msgWarning_car;
                     labelWarning_Car.Visibility = Visibility.Visible;
                 }
                 else
                     labelWarning_Car.Visibility = Visibility.Hidden;

                 przygotowanie();

             }
             else
                 if (_awaria == true)
             {
                 if (_nWait_car1 == 0 && _sygnalizacja.pojazd1 == true)
                 {
                     _nWait_car1 = 1;
                     ruch_samochod1 = true;
                 }

                 if (_nWait_car2 == 0 && _sygnalizacja.pojazd2 == true)
                 {
                     _nWait_car2 = 1;
                     ruch_samochod2_skret = true;
                     _zmiana_samochodu2 = true;
                 }

                 if (_nWait_car3 == 0 && _sygnalizacja.pojazd3 == true)
                 {
                     _nWait_car3 = 1;
                     ruch_samochod3_skret = true;
                     _zmiana_samochodu3 = 1;
                 }

                 if (_nWait_car4 == 0 && _sygnalizacja.pojazd4 == true)
                 {
                     _nWait_car4 = 1;
                     ruch_samochod4 = true;
                 }

                 if (_nWait_car5 == 0 && _sygnalizacja.pojazd5 == true)
                 {
                     _nWait_car5 = 1;
                     ruch_samochod5_skret = true;
                     _zmiana_samochodu5 = true;
                 }

                 if (_nWait_car6 == 0 && _sygnalizacja.pojazd6 == true)
                 {
                     _nWait_car6 = 1;
                     ruch_samochod6_skret = true;
                     _zmiana_samochodu6 = 1;
                 }

                 if (_nWaitAwaria < 10 && zmiana == 0)
                 {
                     if (_nWaitAwaria == 9)
                     {
                         zmiana = 1;
                         _nWaitAwaria = 0;
                     }

                     zielone1.Visibility = Visibility.Hidden;
                     czerwone1.Visibility = Visibility.Hidden;
                     przygotowanie1.Visibility = Visibility.Hidden;
                     zolte1.Visibility = Visibility.Visible;
                     zielone2.Visibility = Visibility.Hidden;
                     czerwone2.Visibility = Visibility.Hidden;
                     przygotowanie2.Visibility = Visibility.Hidden;
                     zolte2.Visibility = Visibility.Visible;
                     zielone3.Visibility = Visibility.Hidden;
                     czerwone3.Visibility = Visibility.Hidden;
                     przygotowanie3.Visibility = Visibility.Hidden;
                     zolte3.Visibility = Visibility.Visible;
                     zielone4.Visibility = Visibility.Hidden;
                     czerwone4.Visibility = Visibility.Hidden;
                     przygotowanie4.Visibility = Visibility.Hidden;
                     zolte4.Visibility = Visibility.Visible;
                     zielone5.Visibility = Visibility.Hidden;
                     czerwone5.Visibility = Visibility.Hidden;
                     przygotowanie5.Visibility = Visibility.Hidden;
                     zolte5.Visibility = Visibility.Visible;
                     zielone6.Visibility = Visibility.Hidden;
                     czerwone6.Visibility = Visibility.Hidden;
                     przygotowanie6.Visibility = Visibility.Hidden;
                     zolte6.Visibility = Visibility.Visible;
                     strzalka_1_brak.Visibility = Visibility.Visible;
                     strzalka_1.Visibility = Visibility.Hidden;
                     strzalka_2_brak.Visibility = Visibility.Visible;
                     strzalka_2.Visibility = Visibility.Hidden;
                     _nWaitAwaria++;
                 }
                 else
                     if (_nWaitAwaria < 10 && zmiana == 1)
                     {
                         if (_nWaitAwaria == 9)
                         {
                             zmiana = 0;
                             _nWaitAwaria = 0;
                         }
                         zielone1.Visibility = Visibility.Hidden;
                         czerwone1.Visibility = Visibility.Hidden;
                         przygotowanie1.Visibility = Visibility.Hidden;
                         zolte1.Visibility = Visibility.Hidden;
                         zielone2.Visibility = Visibility.Hidden;
                         czerwone2.Visibility = Visibility.Hidden;
                         przygotowanie2.Visibility = Visibility.Hidden;
                         zolte2.Visibility = Visibility.Hidden;
                         zielone3.Visibility = Visibility.Hidden;
                         czerwone3.Visibility = Visibility.Hidden;
                         przygotowanie3.Visibility = Visibility.Hidden;
                         zolte3.Visibility = Visibility.Hidden;
                         zielone4.Visibility = Visibility.Hidden;
                         czerwone4.Visibility = Visibility.Hidden;
                         przygotowanie4.Visibility = Visibility.Hidden;
                         zolte4.Visibility = Visibility.Hidden;
                         zielone5.Visibility = Visibility.Hidden;
                         czerwone5.Visibility = Visibility.Hidden;
                         przygotowanie5.Visibility = Visibility.Hidden;
                         zolte5.Visibility = Visibility.Hidden;
                         zielone6.Visibility = Visibility.Hidden;
                         czerwone6.Visibility = Visibility.Hidden;
                         przygotowanie6.Visibility = Visibility.Hidden;
                         zolte6.Visibility = Visibility.Hidden;
                         _nWaitAwaria++;
                     }
                 przygotowanie();
                 _resetSwiatel = true;
             }
             else
             if (_resetSwiatel == true && _awaria == false)
             {
               czerwone1.Visibility = Visibility.Visible;
               zolte1.Visibility = Visibility.Hidden;
               czerwone2.Visibility = Visibility.Visible;
               zolte2.Visibility = Visibility.Hidden;
               czerwone3.Visibility = Visibility.Visible;
               zolte3.Visibility = Visibility.Hidden;
               czerwone4.Visibility = Visibility.Visible;
               zolte4.Visibility = Visibility.Hidden;
               czerwone5.Visibility = Visibility.Visible;
               zolte5.Visibility = Visibility.Hidden;
               czerwone6.Visibility = Visibility.Visible;
               zolte6.Visibility = Visibility.Hidden;
               if (_sygnalizacja.Sygna1 == true)
                   _zmiana1 = true;
               if (_sygnalizacja.Sygna2 == true)
                   _zmiana3 = true;
               if (_sygnalizacja.Sygna3 == true)
                   _zmiana5 = true;
               if (_sygnalizacja.Sygna4 == true)
                   _zmiana7 = true;
               if (_sygnalizacja.Sygna5 == true)
                   _zmiana9 = true;
               if (_sygnalizacja.Sygna6 == true)
                   _zmiana11 = true;

               _resetSwiatel = false;
             }

             // przyciski dla samochodu 1
             auto1_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd1) ? Visibility.Collapsed : Visibility.Visible);
             auto1.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd1) ? Visibility.Visible : Visibility.Collapsed);
             // przyciski dla samochodu 2
             auto2_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd2) ? Visibility.Collapsed : Visibility.Visible);
             auto2.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd2) ? Visibility.Visible : Visibility.Collapsed);
             // przyciski dla samochodu 3
             auto3_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd3) ? Visibility.Collapsed : Visibility.Visible);
             auto3.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd3) ? Visibility.Visible : Visibility.Collapsed);
             // przyciski dla samochodu 4
             auto4_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd4) ? Visibility.Collapsed : Visibility.Visible);
             auto4.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd4) ? Visibility.Visible : Visibility.Collapsed);
             // przyciski dla samochodu 5
             auto5_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd5) ? Visibility.Collapsed : Visibility.Visible);
             auto5.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd5) ? Visibility.Visible : Visibility.Collapsed);
             // przyciski dla samochodu 6
             auto6_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd6) ? Visibility.Collapsed : Visibility.Visible);
             auto6.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd6) ? Visibility.Visible : Visibility.Collapsed);
             // przyciski dla awarii
             awaria1_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.awaria) ? Visibility.Collapsed : Visibility.Visible);
             awaria1.Visibility = (Convert.ToBoolean(_sygnalizacja.awaria) ? Visibility.Visible : Visibility.Collapsed);

        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 1. **********************************************
        /// </summary>
        void auto1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd1 = false;
                USBComm.usbSetDO(0, Convert.ToInt16(_sygnalizacja.pojazd1));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 1. *********************************************
        /// </summary>
        void auto1_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd1 = true;
                USBComm.usbSetDO(0, Convert.ToInt16(_sygnalizacja.pojazd1));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 2. **********************************************
        /// </summary>
        void auto2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd2 = false;
                USBComm.usbSetDO(1, Convert.ToInt16(_sygnalizacja.pojazd2));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 2. *********************************************
        /// </summary>
        void auto2_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd2 = true;
                USBComm.usbSetDO(1, Convert.ToInt16(_sygnalizacja.pojazd2));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 3. **********************************************
        /// </summary>
        void auto3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd3 = false;
                USBComm.usbSetDO(2, Convert.ToInt16(_sygnalizacja.pojazd3));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 3. *********************************************
        /// </summary>
        void auto3_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd3 = true;
                USBComm.usbSetDO(2, Convert.ToInt16(_sygnalizacja.pojazd3));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 4. **********************************************
        /// </summary>
        void auto4_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd4 = false;
                USBComm.usbSetDO(3, Convert.ToInt16(_sygnalizacja.pojazd4));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 4. *********************************************
        /// </summary>
        void auto4_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd4 = true;
                USBComm.usbSetDO(3, Convert.ToInt16(_sygnalizacja.pojazd4));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 5. **********************************************
        /// </summary>
        void auto5_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd5 = false;
                USBComm.usbSetDO(4, Convert.ToInt16(_sygnalizacja.pojazd5));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 5. *********************************************
        /// </summary>
        void auto5_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd5 = true;
                USBComm.usbSetDO(4, Convert.ToInt16(_sygnalizacja.pojazd5));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 6. **********************************************
        /// </summary>
        void auto6_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd6 = false;
                USBComm.usbSetDO(5, Convert.ToInt16(_sygnalizacja.pojazd6));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 6. *********************************************
        /// </summary>
        void auto6_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd6 = true;
                USBComm.usbSetDO(5, Convert.ToInt16(_sygnalizacja.pojazd6));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia awari. **********************************************
        /// </summary>
        void awaria_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.awaria = false;
                USBComm.usbSetDO(6, Convert.ToInt16(_sygnalizacja.awaria));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku awari. *********************************************
        /// </summary>
        void awaria_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
               _sygnalizacja.awaria = true;
               USBComm.usbSetDO(6, Convert.ToInt16(_sygnalizacja.awaria));
            }
        }


        /// <summary>
        /// Zmiana wizualizacji (zmiana świateł). **********************************************************
        /// </summary>
        private void przygotowanie()
        {
            ///Samochod 1 - jazda
            if (ruch_samochod1 == true)
            {
                if (zielone1.Visibility == Visibility.Visible || _nWait_car1 != 0)
                {
                    _nWait_car1++;
                    if (_nWait_car1 < 18)
                        Canvas.SetTop(samochod1, -(_nWait_car1) * 20);
                    if (_nWait_car1 == 18)
                    {
                        samochod1.Visibility = Visibility.Hidden;
                        samochod1_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car1 > 18)
                    {
                        Canvas.SetRight(samochod1_skret, (_nWait_car1 - 18) * 20);
                    }
                    if (_nWait_car1 == 30)
                    {
                        samochod1_skret.Visibility = Visibility.Hidden;
                        _nWait_car1 = 0;
                        _sygnalizacja.pojazd1 = false;
                        ruch_samochod1 = false;
                    }
                }
                else
                    ruch_samochod1 = false;
            }

            ///Samochod 2 - jazda
            if (ruch_samochod2 == true)
            {
                if (zielone2.Visibility == Visibility.Visible || _nWait_car2 != 0)
                {
                    _nWait_car2++;
                    if (_nWait_car2 >= 00)
                        Canvas.SetTop(samochod2, -(_nWait_car2) * 20);
                    if (_nWait_car2 == 30)
                    {
                        samochod2.Visibility = Visibility.Hidden;
                        _nWait_car2 = 0;
                        _sygnalizacja.pojazd2 = false;
                        ruch_samochod2 = false;
                        _zmiana_samochodu2 = true;
                    }
                }
                else
                    ruch_samochod2 = false;
            }

            ///Samochod 2 - skret
            if (ruch_samochod2_skret == true)
            {
                if (zielone2.Visibility == Visibility.Visible || _nWait_car2 != 0)
                {
                    _nWait_car2++;
                    if (_nWait_car2 >= 0 && _nWait_car2 < 10)
                        Canvas.SetTop(samochod2, -(_nWait_car2) * 20);
                    if (_nWait_car2 == 10)
                    {
                        samochod2.Visibility = Visibility.Hidden;
                        samochod2_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car2 > 10)
                    {
                        Canvas.SetLeft(samochod2_skret, (_nWait_car2 - 10) * 20);
                    }
                    if (_nWait_car2 == 15)
                    {
                        samochod2_skret.Visibility = Visibility.Hidden;
                        _nWait_car2 = 0;
                        _sygnalizacja.pojazd2 = false;
                        ruch_samochod2_skret = false;
                        _zmiana_samochodu2 = false;
                    }
                }
                else
                    ruch_samochod2_skret = false;
            }

            ///Samochod 3 - jazda
            if (ruch_samochod3 == true)
            {
                if (zielone3.Visibility == Visibility.Visible || _nWait_car3 != 0)
                {
                    _nWait_car3++;
                    if (_nWait_car3 >= 0)
                        Canvas.SetRight(samochod3, (_nWait_car3) * 20);
                    if (_nWait_car3 == 30)
                    {
                        samochod3.Visibility = Visibility.Hidden;
                        _nWait_car3 = 0;
                        _sygnalizacja.pojazd3 = false;
                        ruch_samochod3 = false;
                        _zmiana_samochodu3 = 1;
                    }
                }
                else
                    ruch_samochod3 = false;
            }

            ///Samochod 3 - skret
            if (ruch_samochod3_skret == true)
            {
                if (zielone3.Visibility == Visibility.Visible || _nWait_car3 != 0 || strzalka_1.Visibility == Visibility.Visible)
                {
                    _nWait_car3++;
                    if (_nWait_car3 >= 0 && _nWait_car3 < 10)
                        Canvas.SetRight(samochod3, (_nWait_car3) * 20);
                    if (_nWait_car3 == 10)
                    {
                        samochod3.Visibility = Visibility.Hidden;
                        samochod3_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car3 > 10)
                    {
                        Canvas.SetTop(samochod3_skret, -(_nWait_car3 - 10) * 20);
                    }
                    if (_nWait_car3 == 15)
                    {
                        samochod3_skret.Visibility = Visibility.Hidden;
                        _nWait_car3 = 0;
                        _sygnalizacja.pojazd3 = false;
                        ruch_samochod3_skret = false;
                        _zmiana_samochodu3 = 2;
                    }
                }
                else
                    ruch_samochod3_skret = false;
            }

            ///Samochod 3 - skret_2
            if (ruch_samochod3_skret_2 == true)
            {
                if (zielone3.Visibility == Visibility.Visible || _nWait_car3 != 0)
                {
                    _nWait_car3++;
                    if (_nWait_car3 >= 0 && _nWait_car3 < 20)
                        Canvas.SetRight(samochod3, (_nWait_car3) * 20);
                    if (_nWait_car3 == 20)
                    {
                        samochod3.Visibility = Visibility.Hidden;
                        samochod3_skret_2.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car3 > 20)
                    {
                        Canvas.SetTop(samochod3_skret_2, (_nWait_car3 - 20) * 25);
                    }
                    if (_nWait_car3 == 30)
                    {
                        samochod3_skret_2.Visibility = Visibility.Hidden;
                        _nWait_car3 = 0;
                        _sygnalizacja.pojazd3 = false;
                        ruch_samochod3_skret_2 = false;
                        _zmiana_samochodu3 = 0;
                    }
                }
                else
                    ruch_samochod3_skret_2 = false;
            }

            ///Samochod 4 - jazda
            if (ruch_samochod4 == true)
            {
                if (zielone4.Visibility == Visibility.Visible || _nWait_car4 != 0)
                {
                    _nWait_car4++;
                    if (_nWait_car4 >= 0 && _nWait_car4 < 18)
                        Canvas.SetTop(samochod4, (_nWait_car4) * 20);
                    if (_nWait_car4 == 18)
                    {
                        samochod4.Visibility = Visibility.Hidden;
                        samochod4_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car4 > 18)
                    {
                        Canvas.SetLeft(samochod4_skret, (_nWait_car4 - 18) * 20);
                    }
                    if (_nWait_car4 == 30)
                    {
                        samochod4_skret.Visibility = Visibility.Hidden;
                        _nWait_car4 = 0;
                        _sygnalizacja.pojazd4 = false;
                        ruch_samochod4 = false;
                    }
                }
                else
                    ruch_samochod4 = false;
            }

            ///Samochod 5 - jazda
            if (ruch_samochod5 == true)
            {
                if (zielone5.Visibility == Visibility.Visible || _nWait_car5 != 0)
                {
                    _nWait_car5++;
                    if (_nWait_car5 >= 0)
                        Canvas.SetTop(samochod5, (_nWait_car5) * 20);
                    if (_nWait_car5 == 30)
                    {
                        samochod5.Visibility = Visibility.Hidden;
                        _nWait_car5 = 0;
                        _sygnalizacja.pojazd5 = false;
                        ruch_samochod5 = false;
                        _zmiana_samochodu5 = true;
                    }
                }
                else
                    ruch_samochod5 = false;
            }

            ///Samochod 5 - skret
            if (ruch_samochod5_skret == true)
            {
                if (zielone5.Visibility == Visibility.Visible || _nWait_car5 != 0)
                {
                    _nWait_car5++;
                    if (_nWait_car5 >= 0 && _nWait_car5 < 10)
                        Canvas.SetTop(samochod5, (_nWait_car5) * 20);
                    if (_nWait_car5 == 10)
                    {
                        samochod5.Visibility = Visibility.Hidden;
                        samochod5_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car5 > 10)
                    {
                        Canvas.SetRight(samochod5_skret, (_nWait_car5 - 10) * 20);
                    }
                    if (_nWait_car5 == 15)
                    {
                        samochod5_skret.Visibility = Visibility.Hidden;
                        _nWait_car5 = 0;
                        _sygnalizacja.pojazd5 = false;
                        ruch_samochod5_skret = false;
                        _zmiana_samochodu5 = false;
                    }
                }
                else
                    ruch_samochod5_skret = false;
            }

            ///Samochod 6 - jazda
            if (ruch_samochod6 == true)
            {
                if (zielone6.Visibility == Visibility.Visible || _nWait_car6 != 0)
                {
                    _nWait_car6++;
                    if (_nWait_car6 >= 0)
                        Canvas.SetLeft(samochod6, (_nWait_car6) * 20);
                    if (_nWait_car6 == 30)
                    {
                        samochod6.Visibility = Visibility.Hidden;
                        _nWait_car6 = 0;
                        _sygnalizacja.pojazd6 = false;
                        ruch_samochod6 = false;
                        _zmiana_samochodu6 = 1;
                    }
                }
                else
                    ruch_samochod6 = false;
            }

            ///Samochod 6 - skret
            if (ruch_samochod6_skret == true)
            {
                if (zielone6.Visibility == Visibility.Visible || _nWait_car6 != 0 || strzalka_2.Visibility == Visibility.Visible)
                {
                    _nWait_car6++;
                    if (_nWait_car6 >= 0 && _nWait_car6 < 10)
                        Canvas.SetLeft(samochod6, (_nWait_car6) * 20);
                    if (_nWait_car6 == 10)
                    {
                        samochod6.Visibility = Visibility.Hidden;
                        samochod6_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car6 > 10)
                    {
                        Canvas.SetTop(samochod6_skret, (_nWait_car6 - 10) * 20);
                    }
                    if (_nWait_car6 == 15)
                    {
                        samochod6_skret.Visibility = Visibility.Hidden;
                        _nWait_car6 = 0;
                        _sygnalizacja.pojazd6 = false;
                        ruch_samochod6_skret = false;
                        _zmiana_samochodu6 = 2;
                    }
                }
                else
                    ruch_samochod6_skret = false;
            }

            ///Samochod 6 - skret_2
            if (ruch_samochod6_skret_2 == true)
            {
                if (zielone6.Visibility == Visibility.Visible || _nWait_car6 != 0)
                {
                    _nWait_car6++;
                    if (_nWait_car6 < 20)
                        Canvas.SetLeft(samochod6, _nWait_car6 * 20);
                    if (_nWait_car6 == 20)
                    {
                        samochod6.Visibility = Visibility.Hidden;
                        samochod6_skret_2.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car6 > 20)
                    {
                        Canvas.SetTop(samochod6_skret_2, -(_nWait_car6 - 20) * 25);
                    }
                    if (_nWait_car6 == 30)
                    {
                        samochod6_skret_2.Visibility = Visibility.Hidden;
                        _nWait_car6 = 0;
                        _sygnalizacja.pojazd6 = false;
                        ruch_samochod6_skret_2 = false;
                        _zmiana_samochodu6 = 0;
                    }
                }
                else
                    ruch_samochod6_skret_2 = false;
            }

            /// Ruszanie - sygnalizacja 1
            if (_zmiana1)
            {
                if (_nWait2 < 10)
                {
                    zielone1.Visibility = Visibility.Hidden;
                    czerwone1.Visibility = Visibility.Hidden;
                    zolte1.Visibility = Visibility.Hidden;
                    przygotowanie1.Visibility = Visibility.Visible;
                    _nWait2++;
                }
                else
                {
                    _nWait2 = 0;
                    _zmiana1 = false;
                    zielone1.Visibility = Visibility.Visible;
                    przygotowanie1.Visibility = Visibility.Hidden;
                    czerwone1.Visibility = Visibility.Hidden;
                    zolte1.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 1
            if (_zmiana2)
            {
                if (_nWait3 < 30)
                {
                    zielone1.Visibility = Visibility.Hidden;
                    czerwone1.Visibility = Visibility.Hidden;
                    zolte1.Visibility = Visibility.Visible;
                    przygotowanie1.Visibility = Visibility.Hidden;
                    _nWait3++;
                }
                else
                {
                    _nWait3 = 0;
                    _zmiana2 = false;
                    zielone1.Visibility = Visibility.Hidden;
                    przygotowanie1.Visibility = Visibility.Hidden;
                    czerwone1.Visibility = Visibility.Visible;
                    zolte1.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 2
            if (_zmiana3)
            {
                if (_nWait4 < 10)
                {
                    zielone2.Visibility = Visibility.Hidden;
                    czerwone2.Visibility = Visibility.Hidden;
                    zolte2.Visibility = Visibility.Hidden;
                    przygotowanie2.Visibility = Visibility.Visible;
                    _nWait4++;
                }
                else
                {
                    _nWait4 = 0;
                    _zmiana3 = false;
                    zielone2.Visibility = Visibility.Visible;
                    przygotowanie2.Visibility = Visibility.Hidden;
                    czerwone2.Visibility = Visibility.Hidden;
                    zolte2.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 2
            if (_zmiana4)
            {
                if (_nWait5 < 30)
                {
                    zielone2.Visibility = Visibility.Hidden;
                    czerwone2.Visibility = Visibility.Hidden;
                    zolte2.Visibility = Visibility.Visible;
                    przygotowanie2.Visibility = Visibility.Hidden;
                    _nWait5++;
                }
                else
                {
                    _nWait5 = 0;
                    _zmiana4 = false;
                    zielone2.Visibility = Visibility.Hidden;
                    przygotowanie2.Visibility = Visibility.Hidden;
                    czerwone2.Visibility = Visibility.Visible;
                    zolte2.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 3
            if (_zmiana5)
            {
                if (_nWait6 < 10)
                {
                    zielone3.Visibility = Visibility.Hidden;
                    czerwone3.Visibility = Visibility.Hidden;
                    zolte3.Visibility = Visibility.Hidden;
                    przygotowanie3.Visibility = Visibility.Visible;
                    _nWait6++;
                }
                else
                {
                    _nWait6 = 0;
                    _zmiana5 = false;
                    zielone3.Visibility = Visibility.Visible;
                    przygotowanie3.Visibility = Visibility.Hidden;
                    czerwone3.Visibility = Visibility.Hidden;
                    zolte3.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 3
            if (_zmiana6)
            {
                if (_nWait7 < 30)
                {
                    zielone3.Visibility = Visibility.Hidden;
                    czerwone3.Visibility = Visibility.Hidden;
                    zolte3.Visibility = Visibility.Visible;
                    przygotowanie3.Visibility = Visibility.Hidden;
                    _nWait7++;
                }
                else
                {
                    _nWait7 = 0;
                    _zmiana6 = false;
                    zielone3.Visibility = Visibility.Hidden;
                    przygotowanie3.Visibility = Visibility.Hidden;
                    czerwone3.Visibility = Visibility.Visible;
                    zolte3.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 4
            if (_zmiana7)
            {
                if (_nWait8 < 10)
                {
                    zielone4.Visibility = Visibility.Hidden;
                    czerwone4.Visibility = Visibility.Hidden;
                    zolte4.Visibility = Visibility.Hidden;
                    przygotowanie4.Visibility = Visibility.Visible;
                    _nWait8++;
                }
                else
                {
                    _nWait8 = 0;
                    _zmiana7 = false;
                    zielone4.Visibility = Visibility.Visible;
                    przygotowanie4.Visibility = Visibility.Hidden;
                    czerwone4.Visibility = Visibility.Hidden;
                    zolte4.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 4
            if (_zmiana8)
            {
                if (_nWait9 < 30)
                {
                    zielone4.Visibility = Visibility.Hidden;
                    czerwone4.Visibility = Visibility.Hidden;
                    zolte4.Visibility = Visibility.Visible;
                    przygotowanie4.Visibility = Visibility.Hidden;
                    _nWait9++;
                }
                else
                {
                    _nWait9 = 0;
                    _zmiana8 = false;
                    zielone4.Visibility = Visibility.Hidden;
                    przygotowanie4.Visibility = Visibility.Hidden;
                    czerwone4.Visibility = Visibility.Visible;
                    zolte4.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 5
            if (_zmiana9)
            {
                if (_nWait10 < 10)
                {
                    zielone5.Visibility = Visibility.Hidden;
                    czerwone5.Visibility = Visibility.Hidden;
                    zolte5.Visibility = Visibility.Hidden;
                    przygotowanie5.Visibility = Visibility.Visible;
                    _nWait10++;
                }
                else
                {
                    _nWait10 = 0;
                    _zmiana9 = false;
                    zielone5.Visibility = Visibility.Visible;
                    przygotowanie5.Visibility = Visibility.Hidden;
                    czerwone5.Visibility = Visibility.Hidden;
                    zolte5.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 5
            if (_zmiana10)
            {
                if (_nWait11 < 30)
                {
                    zielone5.Visibility = Visibility.Hidden;
                    czerwone5.Visibility = Visibility.Hidden;
                    zolte5.Visibility = Visibility.Visible;
                    przygotowanie5.Visibility = Visibility.Hidden;
                    _nWait11++;
                }
                else
                {
                    _nWait11 = 0;
                    _zmiana10 = false;
                    zielone5.Visibility = Visibility.Hidden;
                    przygotowanie5.Visibility = Visibility.Hidden;
                    czerwone5.Visibility = Visibility.Visible;
                    zolte5.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 6
            if (_zmiana11)
            {
                if (_nWait12 < 10)
                {
                    zielone6.Visibility = Visibility.Hidden;
                    czerwone6.Visibility = Visibility.Hidden;
                    zolte6.Visibility = Visibility.Hidden;
                    przygotowanie6.Visibility = Visibility.Visible;
                    _nWait12++;
                }
                else
                {
                    _nWait12 = 0;
                    _zmiana11 = false;
                    zielone6.Visibility = Visibility.Visible;
                    przygotowanie6.Visibility = Visibility.Hidden;
                    czerwone6.Visibility = Visibility.Hidden;
                    zolte6.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 6
            if (_zmiana12)
            {
                if (_nWait13 < 30)
                {
                    zielone6.Visibility = Visibility.Hidden;
                    czerwone6.Visibility = Visibility.Hidden;
                    zolte6.Visibility = Visibility.Visible;
                    przygotowanie6.Visibility = Visibility.Hidden;
                    _nWait13++;
                }
                else
                {
                    _nWait13 = 0;
                    _zmiana12 = false;
                    zielone6.Visibility = Visibility.Hidden;
                    przygotowanie6.Visibility = Visibility.Hidden;
                    czerwone6.Visibility = Visibility.Visible;
                    zolte6.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Ustawia etykiety tekstowe dla całej kontrolki. **********************************************
        /// </summary>
        void setText()
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
            car1.Content = Messages.msgPojazd1;
            car2.Content = Messages.msgPojazd2;
            car3.Content = Messages.msgPojazd3;
            car4.Content = Messages.msgPojazd4;
            car5.Content = Messages.msgPojazd5;
            car6.Content = Messages.msgPojazd6;
            awaria.Content = Messages.msgAwaria_Przycisk;

            /* Ustawienie etykiet */
            setDOCaptions(ref _lblDOCaptions);
            setDICaptions(ref _lblDICaptions);
        }
        
        /// <summary>
        /// Ustawia etykiety DO.
        /// </summary>
        /// <param name="a_DICaptions">Uporządkowana lista Labeli tekstowych.</param>
        private void setDOCaptions(ref List<Label> a_DOCaptions)
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
        /// Ustawia etykiety DI.
        /// </summary>
        /// <param name="a_DICaptions">Uporządkowana lista Labeli tekstowych.</param>
        private void setDICaptions(ref List<Label> a_DICaptions)
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
        /// Wyświetla informację w pomocy dla trybu TEST. ***********************************************
        /// </summary>
        public void setTestModeText()
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
        /// Obsługa przycisku Zatrzymania/Wznowienia symulacji. *****************************************
        /// </summary>
        private void buttonPauseStart_Click(object sender, RoutedEventArgs e)
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
                }
            }
        }

        /// <summary>
        /// Wstawia czerwony tekst zawarty w argumencie do okna HTML pomocy. *******************************************
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        /// <param name="a_bOK">Czy dodać drugą kolumnę w tabeli z informacją "NOK"</param>
        private void putRedHtmlText(string a_sText, bool a_bOK)
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
        /// Wstawia zielony tekst zawarty w argumencie do okna HTML pomocy. *******************************************
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        /// <param name="a_bOK">Czy dodać drugą kolumnę w tabeli z informacją "OK"</param>
        private void putGreenHtmlText(string a_sText, bool a_bOK)
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
        /// Wstawia szary tekst zawarty w argumencie do okna HTML pomocy. ************************************************
        /// </summary>
        /// <param name="a_sText">Tekst do wstawienia do okna HTML pomocy.</param>
        private void putGrayHtmlText(string a_sText)
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
        public enum TabStates
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
        public enum CoilsStates
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
        public TabStates State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Czyta/Zmienia stan załączenia świateł w sygnalizacji w trybie DEMO.
        /// </summary>
        public int StateCode
        {
            get { return _nStateCode; }
            set { _nStateCode = value; }
        }

        /// <summary>
        /// Czyta/Zmienia wartość oznaczającą działanie symulacji.
        /// </summary>
        public bool SimulationEnabled
        {
            get { return _bSimulationEnabled; }
            set { _bSimulationEnabled = value; }
        }

        /// <summary>
        /// Czyta / ustawia zezwolenie na wystawienie wartości na wyjście drivera USB.
        /// </summary>
        public bool AllowOutputs
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
        TabStates _state;
        /// <summary>
        /// Timer dla kontrolki.
        /// </summary>
        DispatcherTimer _timer = new DispatcherTimer();
        /// <summary>
        /// Numer stanu dla trybu demo.
        /// </summary>
        int _nStateCode;
        /// <summary>
        /// Włączona/Wyłączona symulacja.
        /// </summary>
        bool _bSimulationEnabled;
        /// <summary>
        /// Model Sygnalizacji.
        /// </summary>
        PLCEmulator.Sygnalizacja.Sygnalizacja _sygnalizacja = new PLCEmulator.Sygnalizacja.Sygnalizacja();
        /// <summary>
        /// Stany wewnętrzne przy tesowaniu.
        /// </summary>
        int _nInnerState;
        /// <summary>
        /// Informuje o stanie trybu TEST.
        /// </summary>
        bool _bTestEnabled;
        /// <summary>
        /// Obiekt dla przechowywania elementów HTML'a dla okna pomocy.
        /// </summary>
        mshtml.IHTMLDocument2 _htmlResult = null;
        /// <summary>
        /// Zezwala / zabrania na wystawianie wartości na wyjścia drivera USB.
        /// </summary>
        /// Przechowuje wartości wejść cyfrowych z driver'a USB.
        /// </summary>
        int[] _DIData = new int[16];
        /// </summary>
        /// Przechowuje wartości wyjść cyfrowych z driver'a USB.
        /// </summary>
        int[] DOdata = new int[16];
        /// <summary>
        bool _bAllowOutputs;
        /// <summary>
        /// Zmienna dla trybu DEMO określa czas oczekiwania między zdarzeniami.
        /// </summary>
        int _nWait = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 1.
        /// </summary>
        int _nWait2 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 1.
        /// </summary>
        int _nWait3 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 2.
        /// </summary>
        int _nWait4 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 2.
        /// </summary>
        int _nWait5 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 3.
        /// </summary>
        int _nWait6 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 3.
        /// </summary>
        int _nWait7 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 4.
        /// </summary>
        int _nWait8 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 4.
        /// </summary>
        int _nWait9 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 5.
        /// </summary>
        int _nWait10 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 5.
        /// </summary>
        int _nWait11 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 6.
        /// </summary>
        int _nWait12 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 6.
        /// </summary>
        int _nWait13 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 1.
        /// </summary>
        int _nWait_car1 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 2.
        /// </summary>
        int _nWait_car2 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 3.
        /// </summary>
        int _nWait_car3 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 4.
        /// </summary>
        int _nWait_car4 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 5.
        /// </summary>
        int _nWait_car5 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 6.
        /// </summary>
        int _nWait_car6 = 0;
        /// <summary>
        /// ruch samochodu 1.
        /// </summary>
        bool ruch_samochod1 = false;
        /// <summary>
        /// ruch samochodu 2.
        /// </summary>
        bool ruch_samochod2 = false;
        /// <summary>
        /// skret samochodu 2.
        /// </summary>
        bool ruch_samochod2_skret = false;
        /// <summary>
        /// ruch samochodu 2 skret.
        /// </summary>
        bool _zmiana_samochodu2 = false;
        /// <summary>
        /// ruch samochodu 3.
        /// </summary>
        bool ruch_samochod3 = false;
        /// <summary>
        /// skret samochodu 3.
        /// </summary>
        bool ruch_samochod3_skret = false;
        /// <summary>
        /// skret_2 samochodu 3.
        /// </summary>
        bool ruch_samochod3_skret_2 = false;
        /// <summary>
        /// ruch samochodu 3 skret.
        /// </summary>
        int _zmiana_samochodu3 = 0;
        /// <summary>
        /// ruch samochodu 4.
        /// </summary>
        bool ruch_samochod4 = false;
        /// <summary>
        /// ruch samochodu 5.
        /// </summary>
        bool ruch_samochod5 = false;
        /// <summary>
        /// skret samochodu 5.
        /// </summary>
        bool ruch_samochod5_skret = false;
        /// <summary>
        /// ruch samochodu 5 skret.
        /// </summary>
        bool _zmiana_samochodu5 = false;
        /// <summary>
        /// ruch samochodu 6.
        /// </summary>
        bool ruch_samochod6 = false;
        /// <summary>
        /// skret samochodu 6.
        /// </summary>
        bool ruch_samochod6_skret = false;
        /// <summary>
        /// skret_2 samochodu 6.
        /// </summary>
        bool ruch_samochod6_skret_2 = false;
        /// <summary>
        /// ruch samochodu 6 skret.
        /// </summary>
        int _zmiana_samochodu6 = 0;
        /// <summary>
        /// Zmienna czasu dla awarii sygnalizacji.
        /// </summary>
        int _nWaitAwaria = 0;
        /// <summary>
        /// Zmienna pomocnicza dla awarii sygnalizacji.
        /// </summary>
        int zmiana = 0;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 1.
        /// </summary>
        bool _sygnal1 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 2.
        /// </summary>
        bool _sygnal2 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 3.
        /// </summary>
        bool _sygnal3 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 4.
        /// </summary>
        bool _sygnal4 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 5.
        /// </summary>
        bool _sygnal5 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 6.
        /// </summary>
        bool _sygnal6 = false;
        /// <summary>
        /// zmiana na sygnalizacji 1 do ruszania.
        /// </summary>
        bool _zmiana1 = false;
        /// <summary>
        /// zmiana na sygnalizacji 1 do zatrzymania.
        /// </summary>
        bool _zmiana2 = false;
        /// <summary>
        /// zmiana na sygnalizacji 2 do ruszania.
        /// </summary>
        bool _zmiana3 = false;
        /// <summary>
        /// zmiana na sygnalizacji 2 do zatrzymania.
        /// </summary>
        bool _zmiana4 = false;
        /// <summary>
        /// zmiana na sygnalizacji 3 do ruszania.
        /// </summary>
        bool _zmiana5 = false;
        /// <summary>
        /// zmiana na sygnalizacji 3 do zatrzymania.
        /// </summary>
        bool _zmiana6 = false;
        /// <summary>
        /// zmiana na sygnalizacji 4 do ruszania.
        /// </summary>
        bool _zmiana7 = false;
        /// <summary>
        /// zmiana na sygnalizacji 4 do zatrzymania.
        /// </summary>
        bool _zmiana8 = false;
        /// <summary>
        /// zmiana na sygnalizacji 5 do ruszania.
        /// </summary>
        bool _zmiana9 = false;
        /// <summary>
        /// zmiana na sygnalizacji 5 do zatrzymania.
        /// </summary>
        bool _zmiana10 = false;
        /// <summary>
        /// zmiana na sygnalizacji 6 do ruszania.
        /// </summary>
        bool _zmiana11 = false;
        /// <summary>
        /// zmiana na sygnalizacji 6 do zatrzymania.
        /// </summary>
        bool _zmiana12 = false;
        /// <summary>
        /// test kolizja.
        /// </summary>
        bool _kolizja = true;
        /// <summary>
        /// test kolizja.
        /// </summary>
        bool _kolizja_car = true;
        /// <summary>
        /// test kolizja.
        /// </summary>
        bool _awaryjnosc = true;
        /// <summary>
        /// resetowanie swiatel po awarii.
        /// </summary>
        bool _resetSwiatel = false;
        /// <summary>
        /// Lista etykiet DO.
        /// </summary>
        List<Label> _lblDOCaptions = new List<Label>();
        /// <summary>
        /// Lista etykiet DI.
        /// </summary>
        List<Label> _lblDICaptions = new List<Label>();
        /// <summary>
        /// Kolor dla kontrolki działającego elementu.
        /// </summary>
        SolidColorBrush _brushOn = new SolidColorBrush(Colors.DarkBlue);
        /// <summary>
        /// Czarny kolor dla etykiety.
        /// </summary>
        SolidColorBrush _brushOff = new SolidColorBrush(Colors.Black);
        /// <summary>
        /// Awaria !!!!!!!!.
        /// </summary>
        bool _awaria = false;
        /// <summary>
        /// test pojazd 1.
        /// </summary>
        bool _pomocnicza1 = true;
        /// <summary>
        /// test pojazd 2.
        /// </summary>
        bool _pomocnicza2 = true;
        /// <summary>
        /// test pojazd 2.
        /// </summary>
        bool _pomocnicza2_2 = true;
        /// <summary>
        /// test pojazd 3.
        /// </summary>
        bool _pomocnicza3 = true;
        /// <summary>
        /// test pojazd 3.
        /// </summary>
        bool _pomocnicza3_2 = true;
        /// <summary>
        /// test pojazd 3.
        /// </summary>
        bool _pomocnicza3_3 = true;
        /// <summary>
        /// test pojazd 4.
        /// </summary>
        bool _pomocnicza4 = true;
        /// <summary>
        /// test pojazd 5.
        /// </summary>
        bool _pomocnicza5 = true;
        /// <summary>
        /// test pojazd 5.
        /// </summary>
        bool _pomocnicza5_2 = true;
        /// <summary>
        /// test pojazd 6.
        /// </summary>
        bool _pomocnicza6 = true;
        /// <summary>
        /// test pojazd 6.
        /// </summary>
        bool _pomocnicza6_2 = true;
        /// <summary>
        /// test pojazd 6.
        /// </summary>
        bool _pomocnicza6_3 = true;
        /// <summary>
        /// funkcja losowania - tryb testowy
        /// </summary>
        int zmienna_losowa;
        System.Random x = new Random(System.DateTime.Now.Millisecond);
        int _Wait_Test = 0;
        int _licznik_testu = 0;
        double procenty = 0.0;
        double suma_pojazdów = 0.0;
    }
}
