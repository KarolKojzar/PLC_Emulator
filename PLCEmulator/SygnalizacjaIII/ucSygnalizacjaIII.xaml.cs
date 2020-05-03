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
using PLCEmulator.SygnalizacjaIII;

namespace PLCEmulator.SygnalizacjaIII
{
    /// <summary>
    /// Interaction logic for SygnalizacjaIII.xaml
    /// </summary>
    public partial class ucSygnalizacjaIII : UserControl
    {
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="a_tsState">Tryb pracy kontrolki.</param>
        public ucSygnalizacjaIII(TabStates a_tsState, ref mshtml.IHTMLDocument2 htmlDoc)
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
            _lblDICaptions.Add((Label)FindName("labelDI8"));
            _lblDICaptions.Add((Label)FindName("labelDI9"));
            _lblDICaptions.Add((Label)FindName("labelDI10"));

            _lblDOCaptions.Add((Label)FindName("labelDO1"));
            _lblDOCaptions.Add((Label)FindName("labelDO2"));
            _lblDOCaptions.Add((Label)FindName("labelDO3"));
            _lblDOCaptions.Add((Label)FindName("labelDO4"));
            _lblDOCaptions.Add((Label)FindName("labelDO5"));
            _lblDOCaptions.Add((Label)FindName("labelDO6"));
            _lblDOCaptions.Add((Label)FindName("labelDO7"));
            _lblDOCaptions.Add((Label)FindName("labelDO8"));
            _lblDOCaptions.Add((Label)FindName("labelDO9"));
            _lblDOCaptions.Add((Label)FindName("labelDO10"));
            _lblDOCaptions.Add((Label)FindName("labelDO11"));
            _lblDOCaptions.Add((Label)FindName("labelDO12"));
            _lblDOCaptions.Add((Label)FindName("labelDO13"));
            _lblDOCaptions.Add((Label)FindName("labelDO14"));
            _lblDOCaptions.Add((Label)FindName("labelDO15"));

            

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

            zielone1.Visibility = Visibility.Hidden;
            czerwone1.Visibility = Visibility.Visible;
            zolte1.Visibility = Visibility.Hidden;
            zielone2.Visibility = Visibility.Hidden;
            czerwone2.Visibility = Visibility.Visible;
            zolte2.Visibility = Visibility.Hidden;
            zielone3.Visibility = Visibility.Hidden;
            czerwone3.Visibility = Visibility.Visible;
            zolte3.Visibility = Visibility.Hidden;
            zielone4.Visibility = Visibility.Hidden;
            czerwone4.Visibility = Visibility.Visible;
            zolte4.Visibility = Visibility.Hidden;
            zielone5.Visibility = Visibility.Hidden;
            czerwone5.Visibility = Visibility.Visible;
            zolte5.Visibility = Visibility.Hidden;
            zielone6.Visibility = Visibility.Hidden;
            czerwone6.Visibility = Visibility.Visible;
            zolte6.Visibility = Visibility.Hidden;
            zielone7.Visibility = Visibility.Hidden;
            czerwone7.Visibility = Visibility.Visible;
            zolte7.Visibility = Visibility.Hidden;
            zielone8.Visibility = Visibility.Hidden;
            czerwone8.Visibility = Visibility.Visible;
            zolte8.Visibility = Visibility.Hidden;
            zielone9.Visibility = Visibility.Hidden;
            czerwone9.Visibility = Visibility.Visible;
            zielone10.Visibility = Visibility.Hidden;
            czerwone10.Visibility = Visibility.Visible;
            zolte10.Visibility = Visibility.Hidden;
            zielone11.Visibility = Visibility.Hidden;
            czerwone11.Visibility = Visibility.Visible;
            zolte11.Visibility = Visibility.Hidden;
            zielone12.Visibility = Visibility.Hidden;
            czerwone12.Visibility = Visibility.Visible;
            zolte12.Visibility = Visibility.Hidden;
            zielone13.Visibility = Visibility.Hidden;
            czerwone13.Visibility = Visibility.Visible;
            zolte13.Visibility = Visibility.Hidden;
            zielone14.Visibility = Visibility.Hidden;
            czerwone14.Visibility = Visibility.Visible;
            zolte14.Visibility = Visibility.Hidden;
            zielone15.Visibility = Visibility.Hidden;
            czerwone15.Visibility = Visibility.Visible;
            zolte15.Visibility = Visibility.Hidden;
            samochod1_skret.Visibility = Visibility.Hidden;
            samochod1.Visibility = Visibility.Hidden;
            samochod2_skret.Visibility = Visibility.Hidden;
            samochod2.Visibility = Visibility.Hidden;
            samochod3_skret.Visibility = Visibility.Hidden;
            samochod3.Visibility = Visibility.Hidden;
            samochod4_skret.Visibility = Visibility.Hidden;
            samochod4.Visibility = Visibility.Hidden;
            samochod5.Visibility = Visibility.Hidden;
            samochod6_skret.Visibility = Visibility.Hidden;
            samochod6_skret_2.Visibility = Visibility.Hidden;
            samochod6.Visibility = Visibility.Hidden;
            samochod7_skret.Visibility = Visibility.Hidden;
            samochod7.Visibility = Visibility.Hidden;
            samochod8_skret.Visibility = Visibility.Hidden;
            samochod8_skret_2.Visibility = Visibility.Hidden;
            samochod8.Visibility = Visibility.Hidden;
            samochod9_skret.Visibility = Visibility.Hidden;
            samochod9.Visibility = Visibility.Hidden;
            samochod10.Visibility = Visibility.Hidden;

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
                    labelDO15.Visibility = Visibility.Visible;
                    labelDO14.Visibility = Visibility.Visible;
                    labelDO13.Visibility = Visibility.Visible;
                    labelDO12.Visibility = Visibility.Visible;
                    labelDO11.Visibility = Visibility.Visible;
                    labelDO12.Visibility = Visibility.Visible;
                    labelDO10.Visibility = Visibility.Visible;
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
                    labelDO15.Visibility = Visibility.Hidden;
                    labelDO14.Visibility = Visibility.Hidden;
                    labelDO13.Visibility = Visibility.Hidden;
                    labelDO12.Visibility = Visibility.Hidden;
                    labelDO11.Visibility = Visibility.Hidden;
                    labelDO12.Visibility = Visibility.Hidden;
                    labelDO10.Visibility = Visibility.Hidden;
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
                            _nWait_car7 = 0;
                            _nWait_car8 = 0;
                            _nWait_car2_2 = 0;
                            _nWait_car10 = 0;
                            _nWait_car6_2 = 0;
                            _nWait_car8_2 = 0;
                            _nWait_car9 = 0;
                            samochod1.Visibility = Visibility.Hidden;
                            samochod2.Visibility = Visibility.Hidden;
                            samochod3.Visibility = Visibility.Hidden;
                            samochod4.Visibility = Visibility.Hidden;
                            samochod5.Visibility = Visibility.Hidden;
                            samochod6.Visibility = Visibility.Hidden;
                            samochod7.Visibility = Visibility.Hidden;
                            samochod8.Visibility = Visibility.Hidden;
                            samochod9.Visibility = Visibility.Hidden;
                            samochod10.Visibility = Visibility.Hidden;
                            samochod1_skret.Visibility = Visibility.Hidden;
                            samochod2_skret.Visibility = Visibility.Hidden;
                            samochod4_skret.Visibility = Visibility.Hidden;
                            samochod6_skret_2.Visibility = Visibility.Hidden;
                            samochod6_skret.Visibility = Visibility.Hidden;
                            samochod7_skret.Visibility = Visibility.Hidden;
                            samochod8_skret.Visibility = Visibility.Hidden;
                            samochod8_skret_2.Visibility = Visibility.Hidden;
                            samochod9_skret.Visibility = Visibility.Hidden;
                            pojazd_5_stop = false;
                            pojazd_6_stop = false;
                            pojazd_6_stop_2 = false;
                            pojazd_8_stop = false;
                            pojazd_8_stop_2 = false;
                            ruch_samochod2_2 = false;
                            ruch_samochod6_2 = false;
                            ruch_samochod8_2 = false;
                            _fala_2 = false;
                            _fala_8 = false;
                            _sygnalizacja.pojazd1 = false;
                            _sygnalizacja.pojazd2 = false;
                            _sygnalizacja.pojazd3 = false;
                            _sygnalizacja.pojazd4 = false;
                            _sygnalizacja.pojazd5 = false;
                            _sygnalizacja.pojazd6 = false;
                            _sygnalizacja.pojazd7 = false;
                            _sygnalizacja.pojazd8 = false;
                            _sygnalizacja.pojazd9 = false;
                            _sygnalizacja.pojazd10 = false;
                            _licznik_testu = 0;
                            procenty = 0.0;
                            suma_pojazdów = 0.0;

                            StateCode = 1;
                        } break;

                    case 1:
                        {
                            if (_nWait == 20)
                            {
                                zmienna_losowa = x.Next(1, 11);

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
                                if (samochod7.Visibility == Visibility.Visible && zmienna_losowa == 7)
                                    zmienna_losowa = 100;
                                if (samochod8.Visibility == Visibility.Visible && zmienna_losowa == 8)
                                    zmienna_losowa = 100;
                                if (samochod9.Visibility == Visibility.Visible && zmienna_losowa == 9)
                                    zmienna_losowa = 100;
                                if (samochod10.Visibility == Visibility.Visible && zmienna_losowa == 10)
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
                                if (zmienna_losowa == 7)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest7;
                                    _sygnalizacja.pojazd7 = true;
                                }
                                if (zmienna_losowa == 8)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest8;
                                    _sygnalizacja.pojazd8 = true;
                                }
                                if (zmienna_losowa == 9)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest9;
                                    _sygnalizacja.pojazd9 = true;
                                }
                                if (zmienna_losowa == 10)
                                {
                                    suma_pojazdów++;
                                    tmpStr = Messages.msgTest10;
                                    _sygnalizacja.pojazd10 = true;
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

                            if (_Wait_Test < 680)
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
                            if (_nWait < 120)
                            {
                                _nWait++;
                            }
                            else
                            {
                                procenty = (Convert.ToDouble(_licznik_testu)) / suma_pojazdów * 100;
                                tmpStr = Messages.testEnd;
                                putGrayHtmlText(tmpStr);

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
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza1 = false;
                }
                if (ruch_samochod1 == false)
                {
                    _pomocnicza1 = true;
                }

                if (opuszczenie_skrzyzowania_2 == true && _pomocnicza2 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie2;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza2 = false;
                }
                if (opuszczenie_skrzyzowania_2 == false)
                {
                    _pomocnicza2 = true;
                }

                if (ruch_samochod3 == true && _pomocnicza3 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie3;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza3 = false;
                }
                if (ruch_samochod3 == false)
                {
                    _pomocnicza3 = true;
                }

                if (ruch_samochod4 == true && _pomocnicza4 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie4;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza4 = false;
                }
                if (ruch_samochod4 == false)
                {
                    _pomocnicza4 = true;
                }

                if (opuszczenie_skrzyzowania_5 == true && _pomocnicza5 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie5;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza5 = false;
                }
                if (opuszczenie_skrzyzowania_5 == false)
                {
                    _pomocnicza5 = true;
                }

                if (opuszczenie_skrzyzowania_6 == true && _pomocnicza6 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie6;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza6 = false;
                }
                if (opuszczenie_skrzyzowania_6 == false)
                {
                    _pomocnicza6 = true;
                }

                if (ruch_samochod7 == true && _pomocnicza7 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie7;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza7 = false;
                }
                if (ruch_samochod7 == false)
                {
                    _pomocnicza7 = true;
                }

                if (opuszczenie_skrzyzowania_8 == true && _pomocnicza8 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie8;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza8 = false;
                }
                if (opuszczenie_skrzyzowania_8 == false)
                {
                    _pomocnicza8 = true;
                }

                if (ruch_samochod9 == true && _pomocnicza9 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie9;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza9 = false;
                }
                if (ruch_samochod9 == false)
                {
                    _pomocnicza9 = true;
                }

                if (ruch_samochod10 == true && _pomocnicza10 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie10;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza10 = false;
                }
                if (ruch_samochod10 == false)
                {
                    _pomocnicza10 = true;
                }

                if (ruch_samochod4_skret == true && _pomocnicza11 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie4;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza11 = false;
                }
                if (ruch_samochod4_skret == false)
                {
                    _pomocnicza11 = true;
                }

                if (ruch_samochod7_skret == true && _pomocnicza12 == true && _Wait_Test != 0)
                {
                    tmpStr = Messages.msgTest_opuszczenie7;
                    putGreenHtmlText(tmpStr, false);
                    if (labelWarning.Visibility == Visibility.Hidden && labelKolizja.Visibility == Visibility.Hidden)
                        _licznik_testu++;
                    _pomocnicza12 = false;
                }
                if (ruch_samochod7_skret == false)
                {
                    _pomocnicza12 = true;
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

                if (labelKolizja.Visibility == Visibility.Visible && _kolizja_car == true)
                {
                    tmpStr = Messages.msgWarning_car;
                    putRedHtmlText(tmpStr, false);

                    _kolizja_car = false;
                }
                if (labelKolizja.Visibility == Visibility.Hidden)
                {
                    _kolizja_car = true;
                }

                if (labelFala.Visibility == Visibility.Visible && _Fala == true)
                {
                    tmpStr = Messages.msgFala;
                    putRedHtmlText(tmpStr, false);

                    _Fala = false;
                }
                if (labelFala.Visibility == Visibility.Hidden)
                {
                    _Fala = true;
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
                _sygnal7 = _sygnalizacja.Sygna7;
                _sygnal8 = _sygnalizacja.Sygna8;
                _sygnal10 = _sygnalizacja.Sygna10;
                _sygnal11 = _sygnalizacja.Sygna11;
                _sygnal12 = _sygnalizacja.Sygna12;
                _sygnal13 = _sygnalizacja.Sygna13;
                _sygnal14 = _sygnalizacja.Sygna14;
                _sygnal15 = _sygnalizacja.Sygna15;

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
                            _sygnalizacja.Sygna9 = false;
                            _sygnalizacja.Sygna10 = false;
                            _sygnalizacja.Sygna11 = false;
                            _sygnalizacja.Sygna12 = false;
                            _sygnalizacja.Sygna13 = false;
                            _sygnalizacja.Sygna14 = false;
                            _sygnalizacja.Sygna15 = false;

                            StateCode = 1;
                        } break;
                    case 1:
                        {
                            if (_nWait == 15)
                            {
                                _sygnalizacja.pojazd3 = true;
                                _sygnalizacja.pojazd4 = true;
                            }
                            if (_nWait == 25)
                            {
                                _sygnalizacja.pojazd5 = true;
                                _sygnalizacja.pojazd10 = true;
                                _sygnalizacja.pojazd7 = true;
                                _sygnalizacja.Sygna3 = true;
                                _sygnalizacja.Sygna5 = true;
                                _sygnalizacja.Sygna9 = true;
                            }
                            if (_nWait == 45)
                            {
                                _sygnalizacja.Sygna12 = true;
                                _sygnalizacja.Sygna15 = true;
                                _sygnalizacja.Sygna7 = true;
                                _sygnalizacja.pojazd1 = true;
                            }
                            if (_nWait == 85)
                            {
                                _sygnalizacja.pojazd2 = true;
                                _sygnalizacja.pojazd6 = true;
                            }
                            if (_nWait < 100)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _sygnalizacja.Sygna3 = false;
                                _sygnalizacja.Sygna5 = false;
                                _nWait = 0;
                                StateCode = 2;
                            }
                        }
                        break;
                    case 2:
                        {
                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna2 = true;
                                _sygnalizacja.Sygna1 = true;
                                _sygnalizacja.Sygna12 = false;
                                _sygnalizacja.Sygna15 = false;
                            }
                            if (_nWait == 40)
                            {
                                _sygnalizacja.Sygna7 = false;
                            }
                            if (_nWait == 60)
                            {
                                _sygnalizacja.Sygna11 = true;
                                _sygnalizacja.Sygna12 = true;
                                _sygnalizacja.Sygna9 = false;
                                _sygnalizacja.Sygna7 = false;
                                _sygnalizacja.pojazd8 = true;
                                _sygnalizacja.Sygna6 = true;
                            }
                            if (_nWait == 80)
                            {
                                _sygnalizacja.Sygna1 = false;
                                _sygnalizacja.Sygna8 = true;
                            }

                            if (_nWait < 90)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _sygnalizacja.Sygna6 = true;
                                _sygnalizacja.Sygna11 = false;
                                _sygnalizacja.Sygna12 = false;
                                _sygnalizacja.pojazd4 = true;
                                _nWait = 0;
                                StateCode = 3;
                            }
                        }
                        break;
                    case 3:
                        {
                            if(_nWait == 10)
                            {
                                _sygnalizacja.Sygna10 = true;
                                _sygnalizacja.pojazd10 = true;
                            }

                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna4 = true;
                                _sygnalizacja.Sygna13 = true;
                                _sygnalizacja.pojazd2 = true;
                                _sygnalizacja.pojazd9 = true;
                            }

                            if (_nWait == 50)
                            {
                                _sygnalizacja.Sygna13 = false;
                                _sygnalizacja.Sygna10 = false;
                                _sygnalizacja.pojazd2 = true;
                            }
                            if (_nWait == 70)
                            {
                                _sygnalizacja.Sygna14 = true;
                                _sygnalizacja.Sygna15 = true;
                            }

                            if (_nWait < 90)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _nWait = 0;
                                StateCode = 4;
                                _sygnalizacja.Sygna14 = false;
                                _sygnalizacja.Sygna15 = false;
                                _sygnalizacja.Sygna2 = false;
                            }
                        }
                        break;
                    case 4:
                        {
                            if (_nWait == 10)
                            {
                                _sygnalizacja.Sygna10 = true;
                                _sygnalizacja.Sygna13 = true;
                            }


                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna4 = false;
                                _sygnalizacja.Sygna6 = false;
                            }

                            if (_nWait == 50)
                            {
                                _sygnalizacja.Sygna3 = true;
                                _sygnalizacja.Sygna5 = true;
                                _sygnalizacja.pojazd8 = true;
                                _sygnalizacja.pojazd3 = true;
                                _sygnalizacja.pojazd5 = true;
                            }
                            if (_nWait == 70)
                            {
                                _sygnalizacja.pojazd9 = true;
                                _sygnalizacja.Sygna3 = false;
                                _sygnalizacja.Sygna5 = false;
                                _sygnalizacja.Sygna13 = false;
                                _sygnalizacja.Sygna10 = false;
                            }

                            if (_nWait == 100)
                            {
                                _sygnalizacja.Sygna14 = true;
                                _sygnalizacja.Sygna11 = true;
                            }

                            if (_nWait < 110)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _sygnalizacja.Sygna8 = false;
                                _sygnalizacja.Sygna4 = true;
                                _nWait = 0;
                                StateCode = 5;
                            }
                        }
                        break;
                    case 5:
                        {


                            if (_nWait == 30)
                            {
                                _sygnalizacja.Sygna7 = true;
                                _sygnalizacja.pojazd6 = true;
                            }

                            if (_nWait < 60)
                            {
                                _nWait++;
                            }
                            else
                            {
                                _sygnalizacja.Sygna7 = false;
                                _sygnalizacja.Sygna4 = false;
                                _nWait = 0;
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
            _sygnal7 = _sygnalizacja.Sygna7;
            _sygnal8 = _sygnalizacja.Sygna8;
            _sygnal10 = _sygnalizacja.Sygna10;
            _sygnal11 = _sygnalizacja.Sygna11;
            _sygnal12 = _sygnalizacja.Sygna12;
            _sygnal13 = _sygnalizacja.Sygna13;
            _sygnal14 = _sygnalizacja.Sygna14;
            _sygnal15 = _sygnalizacja.Sygna15;
            /** Przekazanie aktualnych stanów wejść **/
            _sygnalizacja.Sygna1 = Convert.ToBoolean(_DIData[0]);
            _sygnalizacja.Sygna2 = Convert.ToBoolean(_DIData[1]);
            _sygnalizacja.Sygna3 = Convert.ToBoolean(_DIData[2]);
            _sygnalizacja.Sygna4 = Convert.ToBoolean(_DIData[3]);
            _sygnalizacja.Sygna5 = Convert.ToBoolean(_DIData[4]);
            _sygnalizacja.Sygna6 = Convert.ToBoolean(_DIData[5]);
            _sygnalizacja.Sygna7 = Convert.ToBoolean(_DIData[6]);
            _sygnalizacja.Sygna8 = Convert.ToBoolean(_DIData[7]);
            _sygnalizacja.Sygna9 = Convert.ToBoolean(_DIData[8]);
            _sygnalizacja.Sygna10 = Convert.ToBoolean(_DIData[9]);
            _sygnalizacja.Sygna11 = Convert.ToBoolean(_DIData[10]);
            _sygnalizacja.Sygna12 = Convert.ToBoolean(_DIData[11]);
            _sygnalizacja.Sygna13 = Convert.ToBoolean(_DIData[12]);
            _sygnalizacja.Sygna14 = Convert.ToBoolean(_DIData[13]);
            _sygnalizacja.Sygna15 = Convert.ToBoolean(_DIData[14]);
        }

        /// <summary>
        /// Ustawia odpowiednie wyjścia dla driver'a USB. ***********************************************
        /// </summary>
        void setOutputs()
        {
            /** Zapamiętanie poprzednich stanów wejść **/
            _sygnal1 = _sygnalizacja.Sygna1;
            _sygnal2 = _sygnalizacja.Sygna2;
            _sygnal3 = _sygnalizacja.Sygna3;
            _sygnal4 = _sygnalizacja.Sygna4;
            _sygnal5 = _sygnalizacja.Sygna5;
            _sygnal6 = _sygnalizacja.Sygna6;
            _sygnal7 = _sygnalizacja.Sygna7;
            _sygnal8 = _sygnalizacja.Sygna8;
            _sygnal10 = _sygnalizacja.Sygna10;
            _sygnal11 = _sygnalizacja.Sygna11;
            _sygnal12 = _sygnalizacja.Sygna12;
            _sygnal13 = _sygnalizacja.Sygna13;
            _sygnal14 = _sygnalizacja.Sygna14;
            _sygnal15 = _sygnalizacja.Sygna15;
            /** Przekazanie aktualnych stanów wejść **/
            _sygnalizacja.Sygna1 = Convert.ToBoolean(_DIData[0]);
            _sygnalizacja.Sygna2 = Convert.ToBoolean(_DIData[1]);
            _sygnalizacja.Sygna3 = Convert.ToBoolean(_DIData[2]);
            _sygnalizacja.Sygna4 = Convert.ToBoolean(_DIData[3]);
            _sygnalizacja.Sygna5 = Convert.ToBoolean(_DIData[4]);
            _sygnalizacja.Sygna6 = Convert.ToBoolean(_DIData[5]);
            _sygnalizacja.Sygna7 = Convert.ToBoolean(_DIData[6]);
            _sygnalizacja.Sygna8 = Convert.ToBoolean(_DIData[7]);
            _sygnalizacja.Sygna9 = Convert.ToBoolean(_DIData[8]);
            _sygnalizacja.Sygna10 = Convert.ToBoolean(_DIData[9]);
            _sygnalizacja.Sygna11 = Convert.ToBoolean(_DIData[10]);
            _sygnalizacja.Sygna12 = Convert.ToBoolean(_DIData[11]);
            _sygnalizacja.Sygna13 = Convert.ToBoolean(_DIData[12]);
            _sygnalizacja.Sygna14 = Convert.ToBoolean(_DIData[13]);
            _sygnalizacja.Sygna15 = Convert.ToBoolean(_DIData[14]);

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
                _sygnalizacja.pojazd7 = true;
            else
                _sygnalizacja.pojazd7 = false;
            if (DOdata[7] == 1)
                _sygnalizacja.pojazd8 = true;
            else
                _sygnalizacja.pojazd8 = false;
            if (DOdata[8] == 1)
                _sygnalizacja.pojazd9 = true;
            else
                _sygnalizacja.pojazd9 = false;
            if (DOdata[9] == 1)
                _sygnalizacja.pojazd10 = true;
            else
                _sygnalizacja.pojazd10 = false;
        }

        /// <summary>
        /// Aktualizacja grafiki wyświetlanej na kontrolce. *********************************************
        /// </summary>
        private void updateSygnalizacjaImg()
        {
            // jezeli jest klikniety przycisk 1 pokaż pojazd 1, w przeciwnym wypadku wyłącz go.
            // analogicznie pozostałe 8 pojazdów
            if (_sygnalizacja.pojazd1 == true && _nWait_car1 == 0)
            {
                Canvas.SetTop(samochod1, 0.0);
                samochod1.Visibility = Visibility.Visible;
                Canvas.SetRight(samochod1_skret, 0.0);
            }

            if (_sygnalizacja.pojazd2 == true && _nWait_car2 == 0 && ruch_samochod2_2 == false)
            {
                Canvas.SetTop(samochod2, 0.0);
                samochod2.Visibility = Visibility.Visible;
            }

            if (_sygnalizacja.pojazd3 == true && _nWait_car3 == 0)
            {
                Canvas.SetRight(samochod3, 0.0);
                Canvas.SetTop(samochod3_skret, 0.0);
                samochod3.Visibility = Visibility.Visible;
            }

            if (_sygnalizacja.pojazd4 == true && _nWait_car4 == 0)
            {
                Canvas.SetLeft(samochod4, 0.0);
                samochod4.Visibility = Visibility.Visible;
                Canvas.SetTop(samochod4_skret, 0.0);
            }

            if (_sygnalizacja.pojazd5 == true && _nWait_car5 == 0)
            {
                Canvas.SetTop(samochod5, 0.0);
                samochod5.Visibility = Visibility.Visible;
            }

            if (_sygnalizacja.pojazd6 == true && _nWait_car6 == 0 && ruch_samochod6_2 == false)
            {
                Canvas.SetTop(samochod6, 0.0);
                Canvas.SetLeft(samochod6_skret, 0.0);
                samochod6.Visibility = Visibility.Visible;
            }

            if (_sygnalizacja.pojazd7 == true && _nWait_car7 == 0)
            {
                Canvas.SetTop(samochod7, 0.0);
                samochod7.Visibility = Visibility.Visible;
                Canvas.SetRight(samochod7_skret, 0.0);
            }

            if (_sygnalizacja.pojazd8 == true && _nWait_car8 == 0 && ruch_samochod8_2 == false)
            {
                Canvas.SetLeft(samochod8, 0.0);
                Canvas.SetTop(samochod8_skret, 0.0);
                samochod8.Visibility = Visibility.Visible;
            }

            if (_sygnalizacja.pojazd9 == true && _nWait_car9 == 0)
            {
                samochod9.Visibility = Visibility.Visible;
                Canvas.SetTop(samochod9, 0.0);
                Canvas.SetRight(samochod9_skret, 0.0);
            }

            if (_sygnalizacja.pojazd10 == true && _nWait_car10 == 0)
            {
                samochod10.Visibility = Visibility.Visible;
                Canvas.SetTop(samochod10, 0.0);
            }

            /*ruch pojazdów */
            //animacja pojazdów
            if (_sygnalizacja.Sygna1 == true && samochod1.Visibility == Visibility.Visible)
            {
                ruch_samochod1 = true;
            }

            if (_sygnalizacja.Sygna2 == true && samochod2.Visibility == Visibility.Visible)
            {
                ruch_samochod2 = true;
            }

            if (_sygnalizacja.Sygna3 == true && samochod3.Visibility == Visibility.Visible)
            {
                ruch_samochod3 = true;
            }

            if (_sygnalizacja.Sygna5 == true && samochod4.Visibility == Visibility.Visible && _zmiana_samochodu4 == false)
            {
                ruch_samochod4 = true;
            }

            if (_sygnalizacja.Sygna5 == true && samochod4.Visibility == Visibility.Visible && _zmiana_samochodu4 == true)
            {
                ruch_samochod4_skret = true;
            }

            if (_sygnalizacja.Sygna7 == true && samochod5.Visibility == Visibility.Visible)
            {
                ruch_samochod5 = true;
            }

            if (_sygnalizacja.Sygna11 == true && samochod6.Visibility == Visibility.Visible)
            {
                ruch_samochod6 = true;
            }

            if (_sygnalizacja.Sygna12 == true && samochod7.Visibility == Visibility.Visible && _zmiana_samochodu7 == false)
            {
                ruch_samochod7 = true;
            }

            if (_sygnalizacja.Sygna12 == true && samochod7.Visibility == Visibility.Visible && _zmiana_samochodu7 == true)
            {
                ruch_samochod7_skret = true;
            }

            if (_sygnalizacja.Sygna13 == true && samochod8.Visibility == Visibility.Visible)
            {
                ruch_samochod8 = true;
            }

            if (_sygnalizacja.Sygna14 == true && samochod9.Visibility == Visibility.Visible)
            {
                ruch_samochod9 = true;
            }

            if (_sygnalizacja.Sygna15 == true && samochod10.Visibility == Visibility.Visible)
            {
                ruch_samochod10 = true;
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
            if (_sygnalizacja.Sygna7 == true && _sygnal7 == false)
            {
                _zmiana13 = true;
            }
            if (_sygnalizacja.Sygna7 == false && _sygnal7 == true)
            {
                _zmiana14 = true;
            }

            // sygnalizacja 8
            if (_sygnalizacja.Sygna8 == true && _sygnal8 == false)
            {
                _zmiana15 = true;
            }
            if (_sygnalizacja.Sygna8 == false && _sygnal8 == true)
            {
                _zmiana16 = true;
            }

            // sygnalizacja 9
            if (_sygnalizacja.Sygna9 == true)
            {
                zielone9.Visibility = Visibility.Visible;
                czerwone9.Visibility = Visibility.Hidden;
            }
            else
            {
                zielone9.Visibility = Visibility.Hidden;
                czerwone9.Visibility = Visibility.Visible;
            }

            // sygnalizacja 10
            if (_sygnalizacja.Sygna10 == true && _sygnal10 == false)
            {
                _zmiana17 = true;
            }
            if (_sygnalizacja.Sygna10 == false && _sygnal10 == true)
            {
                _zmiana18 = true;
            }

            // sygnalizacja 11
            if (_sygnalizacja.Sygna11 == true && _sygnal11 == false)
            {
                _zmiana19 = true;
            }
            if (_sygnalizacja.Sygna11 == false && _sygnal11 == true)
            {
                _zmiana20 = true;
            }

            // sygnalizacja 12
            if (_sygnalizacja.Sygna12 == true && _sygnal12 == false)
            {
                _zmiana21 = true;
            }
            if (_sygnalizacja.Sygna12 == false && _sygnal12 == true)
            {
                _zmiana22 = true;
            }

            // sygnalizacja 13
            if (_sygnalizacja.Sygna13 == true && _sygnal13 == false)
            {
                _zmiana23 = true;
            }
            if (_sygnalizacja.Sygna13 == false && _sygnal13 == true)
            {
                _zmiana24 = true;
            }

            // sygnalizacja 14
            if (_sygnalizacja.Sygna14 == true && _sygnal14 == false)
            {
                _zmiana25 = true;
            }
            if (_sygnalizacja.Sygna14 == false && _sygnal14 == true)
            {
                _zmiana26 = true;
            }

            // sygnalizacja 15
            if (_sygnalizacja.Sygna15 == true && _sygnal15 == false)
            {
                _zmiana27 = true;
            }
            if (_sygnalizacja.Sygna15 == false && _sygnal15 == true)
            {
                _zmiana28 = true;
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
            _lblDOCaptions[8].Foreground = (_sygnalizacja.Sygna9 ? _brushOn : _brushOff);
            _lblDOCaptions[9].Foreground = (_sygnalizacja.Sygna10 ? _brushOn : _brushOff);
            _lblDOCaptions[10].Foreground = (_sygnalizacja.Sygna11 ? _brushOn : _brushOff);
            _lblDOCaptions[11].Foreground = (_sygnalizacja.Sygna12 ? _brushOn : _brushOff);
            _lblDOCaptions[12].Foreground = (_sygnalizacja.Sygna13 ? _brushOn : _brushOff);
            _lblDOCaptions[13].Foreground = (_sygnalizacja.Sygna14 ? _brushOn : _brushOff);
            _lblDOCaptions[14].Foreground = (_sygnalizacja.Sygna15 ? _brushOn : _brushOff);

            /* Ostrzeżenie gdy nastąpi kolizja świateł*/
            if ((_sygnalizacja.Sygna1 && _sygnalizacja.Sygna3) || (_sygnalizacja.Sygna1 && _sygnalizacja.Sygna4) || (_sygnalizacja.Sygna1 && _sygnalizacja.Sygna5)
                || (_sygnalizacja.Sygna2 && _sygnalizacja.Sygna3) || (_sygnalizacja.Sygna2 && _sygnalizacja.Sygna5) || (_sygnalizacja.Sygna3 && _sygnalizacja.Sygna4)
                || (_sygnalizacja.Sygna4 && _sygnalizacja.Sygna5) || (_sygnalizacja.Sygna6 && _sygnalizacja.Sygna7) || (_sygnalizacja.Sygna7 && _sygnalizacja.Sygna8)
                || (_sygnalizacja.Sygna9 && _sygnalizacja.Sygna6) || (_sygnalizacja.Sygna9 && _sygnalizacja.Sygna11) || (_sygnalizacja.Sygna10 && _sygnalizacja.Sygna11)
                || (_sygnalizacja.Sygna10 && _sygnalizacja.Sygna12) || (_sygnalizacja.Sygna10 && _sygnalizacja.Sygna14) || (_sygnalizacja.Sygna10 && _sygnalizacja.Sygna15)
                || (_sygnalizacja.Sygna11 && _sygnalizacja.Sygna13) || (_sygnalizacja.Sygna11 && _sygnalizacja.Sygna15) || (_sygnalizacja.Sygna12 && _sygnalizacja.Sygna13)
                || (_sygnalizacja.Sygna12 && _sygnalizacja.Sygna14) || (_sygnalizacja.Sygna13 && _sygnalizacja.Sygna14) || (_sygnalizacja.Sygna13 && _sygnalizacja.Sygna15)
                || (_sygnalizacja.Sygna9 && _sygnalizacja.Sygna13))
            {
                labelWarning.Content = Messages.msgWarning;
                labelWarning.Visibility = Visibility.Visible;
            }
            else
                labelWarning.Visibility = Visibility.Hidden;

            /* Ostrzeżenie gdy nastąpi kolizja pojazdów*/
            if ((ruch_samochod1 && ruch_samochod3)
                || (ruch_samochod1 && ruch_samochod4) || (ruch_samochod7 && ruch_samochod9) || (ruch_samochod7_skret && ruch_samochod9)
                || (0 < _nWait_car2 && _nWait_car2 < 24 && ruch_samochod3) || (0 < _nWait_car2 && _nWait_car2 < 24 && ruch_samochod4)
                || (0 < _nWait_car6 && _nWait_car6 < 28 && ruch_samochod10) || (0 < _nWait_car6 && _nWait_car6 < 28 && 0 < _nWait_car8 && _nWait_car8 < 28)
                || (0 < _nWait_car8 && _nWait_car8 < 28 && ruch_samochod10) || (0 < _nWait_car8 && _nWait_car8 < 28 && ruch_samochod9) || (0 < _nWait_car8 && _nWait_car8 < 28 && ruch_samochod7)
                || (_nWait_car6_2 > 21 && ruch_samochod3) || (_nWait_car6_2 > 21 && ruch_samochod4) || (_nWait_car6_2 > 21 && ruch_samochod1)
                || (_nWait_car8_2 > 21 && ruch_samochod3) || (_nWait_car8_2 > 21 && ruch_samochod4) || (_nWait_car8_2 > 21 && ruch_samochod1)
                || (_nWait_car5 > 38 && ruch_samochod3) || (_nWait_car5 > 38 && ruch_samochod4) || (_nWait_car5 > 38 && ruch_samochod1))
            {
                labelKolizja.Content = Messages.msgWarning_car;
                labelKolizja.Visibility = Visibility.Visible;
            }
            else
                labelKolizja.Visibility = Visibility.Hidden;

            /* Ostrzeżenie gdy brak zielonej fali*/
            if (_fala_2 == true || _fala_8 == true)
            {
                labelFala.Content = Messages.msgFala;
                labelFala.Visibility = Visibility.Visible;
            }
            else
                labelFala.Visibility = Visibility.Hidden;

            przygotowanie();

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
            // przyciski dla samochodu 7
            auto7_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd7) ? Visibility.Collapsed : Visibility.Visible);
            auto7.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd7) ? Visibility.Visible : Visibility.Collapsed);
            // przyciski dla samochodu 8
            auto8_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd8) ? Visibility.Collapsed : Visibility.Visible);
            auto8.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd8) ? Visibility.Visible : Visibility.Collapsed);
            // przyciski dla samochodu 9
            auto9_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd9) ? Visibility.Collapsed : Visibility.Visible);
            auto9.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd9) ? Visibility.Visible : Visibility.Collapsed);
            // przyciski dla samochodu 10
            auto10_brak.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd10) ? Visibility.Collapsed : Visibility.Visible);
            auto10.Visibility = (Convert.ToBoolean(_sygnalizacja.pojazd10) ? Visibility.Visible : Visibility.Collapsed);
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
                    if (_nWait_car1 >= 0 && _nWait_car1 < 17)
                        Canvas.SetTop(samochod1, -(_nWait_car1)*10);
                    if (_nWait_car1 == 17)
                    {
                        samochod1.Visibility = Visibility.Hidden;
                        samochod1_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car1 > 17)
                    {
                        Canvas.SetRight(samochod1_skret, (_nWait_car1 - 17)*10);
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
            if (ruch_samochod2 == true && ruch_samochod2_2 == false)
            {
                if (zielone2.Visibility == Visibility.Visible || _nWait_car2 != 0)
                {
                    _nWait_car2++;
                    if (_nWait_car2 >= 0 && _nWait_car2 < 37 )
                    {
                        Canvas.SetTop(samochod2, -(_nWait_car2) * 10);
                        stop_2 = true;
                    }

                    if (_nWait_car2 >= 37 && _nWait_car2 <= 55)
                    {
                        if (zielone6.Visibility == Visibility.Hidden && stop_2 == true)
                        {
                            _nWait_car2--;
                            _fala_2 = true;
                        }
                        else
                        {
                            stop_2 = false;
                            _fala_2 = false;
                            Canvas.SetTop(samochod2, -(_nWait_car2) * 10);
                            if (_nWait_car2 == 55)
                            {
                                samochod2.Visibility = Visibility.Hidden;
                                Canvas.SetRight(samochod2_skret, 0.0);
                                samochod2_skret.Visibility = Visibility.Visible;
                                ruch_samochod2_2 = true;
                                _nWait_car2 = 0;
                                _sygnalizacja.pojazd2 = false;
                                ruch_samochod2 = false;
                            }
                        }
                    }
                }
                else
                    ruch_samochod2 = false;
            }

            ///Samochod 2 - jazda II
            if (ruch_samochod2_2 == true)
            {
                
                    _nWait_car2_2++;

                    if (_nWait_car2_2 >= 0 && _nWait_car2_2 < 30)
                    {
                            Canvas.SetRight(samochod2_skret, (_nWait_car2_2) * 10);
                            stop_2_2 = true;
                    }
                        
                    if(_nWait_car2_2 >=30)
                    {
                        if (zielone10.Visibility == Visibility.Hidden && stop_2_2 == true)
                        {
                            _nWait_car2_2--;
                            _fala_2 = true;
                        }
                        else
                        {
                            stop_2_2 = false;
                            _fala_2 = false;
                            Canvas.SetRight(samochod2_skret, (_nWait_car2_2) * 10);
                            if (_nWait_car2_2 == 55)
                            {
                                opuszczenie_skrzyzowania_2 = true;
                                samochod2_skret.Visibility = Visibility.Hidden;
                                _nWait_car2_2 = 0;
                                ruch_samochod2_2 = false;
                            }
                        }
                    }
            }


            ///Samochod 3 - jazda
            if (ruch_samochod3 == true)
            {
                if (zielone3.Visibility == Visibility.Visible || _nWait_car3 != 0)
                {
                    _nWait_car3++;
                    if (_nWait_car3 >= 0)
                        Canvas.SetRight(samochod3, (_nWait_car3) * 10);
                    if (_nWait_car3 == 30)
                    {
                        samochod3.Visibility = Visibility.Hidden;
                        _nWait_car3 = 0;
                        _sygnalizacja.pojazd3 = false;
                        ruch_samochod3 = false;
                    }
                }
                else
                    ruch_samochod3 = false;
            }

            ///Samochod 4 - jazda
            if (ruch_samochod4 == true)
            {
                if (zielone5.Visibility == Visibility.Visible || _nWait_car4 != 0)
                {
                    _nWait_car4++;
                    if (_nWait_car4 >= 0)
                        Canvas.SetLeft(samochod4, (_nWait_car4) * 10);
                    if (_nWait_car4 == 30)
                    {
                        samochod4.Visibility = Visibility.Hidden;
                        _nWait_car4 = 0;
                        _sygnalizacja.pojazd4 = false;
                        ruch_samochod4 = false;
                        _zmiana_samochodu4 = true;
                    }
                }
                else
                    ruch_samochod4 = false;
            }

            ///Samochod 4 - skret
            if (ruch_samochod4_skret == true)
            {
                if (zielone5.Visibility == Visibility.Visible || _nWait_car4 != 0)
                {
                    _nWait_car4++;
                    if (_nWait_car4 >= 0)
                        Canvas.SetLeft(samochod4, (_nWait_car4) * 10);
                    if (_nWait_car4 == 10)
                    {
                        samochod4.Visibility = Visibility.Hidden;
                        samochod4_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car4 > 10)
                    {
                        Canvas.SetTop(samochod4_skret, (_nWait_car4 - 10) * 15);
                    }
                    if (_nWait_car4 == 15)
                    {
                        samochod4_skret.Visibility = Visibility.Hidden;
                        _nWait_car4 = 0;
                        _sygnalizacja.pojazd4 = false;
                        ruch_samochod4_skret = false;
                        _zmiana_samochodu4 = false;
                    }
                }
                else
                    ruch_samochod4_skret = false;
            }

            ///Samochod 5 - jazda
            if (ruch_samochod5 == true)
            {
                if (zielone7.Visibility == Visibility.Visible || _nWait_car5 != 0)
                {
                    _nWait_car5++;
                    if (_nWait_car5 >= 0 && _nWait_car5 < 25)
                    {
                        Canvas.SetTop(samochod5, (_nWait_car5) * 10);
                        stop_5 = true;
                        pojazd_5_stop = false;
                    }

                    if ((pojazd_6_stop_2 == true && pojazd_8_stop_2 == false) || (pojazd_8_stop_2 == true && pojazd_6_stop_2 == false))
                    {
                        if (_nWait_car5 >= 25 && _nWait_car5 < 31)
                            Canvas.SetTop(samochod5, (_nWait_car5) * 10);

                        if (_nWait_car5 >= 31)
                        {
                            if (zielone4.Visibility == Visibility.Hidden && stop_5 == true)
                            {
                                _nWait_car5--;
                                pojazd_5_stop = true;
                            }
                            else
                            {
                                pojazd_5_stop = false;
                                stop_5 = false;
                                Canvas.SetTop(samochod5, (_nWait_car5) * 10);
                                if (_nWait_car5 == 62)
                                {
                                    samochod5.Visibility = Visibility.Hidden;
                                    _nWait_car5 = 0;
                                    _sygnalizacja.pojazd5 = false;
                                    ruch_samochod5 = false;
                                    opuszczenie_skrzyzowania_5 = true;
                                }
                            }
                        }
                    }
                    else if (pojazd_6_stop_2 == true && pojazd_8_stop_2 == true)
                    {
                        if (_nWait_car5 >= 25)
                        {
                            if (zielone4.Visibility == Visibility.Hidden && stop_5 == true)
                            {
                                _nWait_car5--;
                                pojazd_5_stop = true;
                            }
                            else
                            {
                                pojazd_5_stop = false;
                                stop_5 = false;
                                Canvas.SetTop(samochod5, (_nWait_car5) * 10);
                                if (_nWait_car5 == 62)
                                {
                                    samochod5.Visibility = Visibility.Hidden;
                                    _nWait_car5 = 0;
                                    _sygnalizacja.pojazd5 = false;
                                    ruch_samochod5 = false;
                                    opuszczenie_skrzyzowania_5 = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_nWait_car5 >= 25 && _nWait_car5 < 37)
                            Canvas.SetTop(samochod5, (_nWait_car5) * 10);

                        if (_nWait_car5 >= 37)
                        {
                            if (zielone4.Visibility == Visibility.Hidden && stop_5 == true)
                            {
                                _nWait_car5--;
                                pojazd_5_stop = true;
                            }
                            else
                            {
                                pojazd_5_stop = false;
                                stop_5 = false;
                                Canvas.SetTop(samochod5, (_nWait_car5) * 10);
                                if (_nWait_car5 == 62)
                                {
                                    samochod5.Visibility = Visibility.Hidden;
                                    _nWait_car5 = 0;
                                    _sygnalizacja.pojazd5 = false;
                                    ruch_samochod5 = false;
                                    opuszczenie_skrzyzowania_5 = true;
                                }
                            }
                        }
                    }
                }
                else
                    ruch_samochod5 = false;
            }

            ///Samochod 6 - jazda
            if (ruch_samochod6 == true)
            {
                if (zielone11.Visibility == Visibility.Visible || _nWait_car6 != 0)
                {
                    _nWait_car6++;
                    if (_nWait_car6 >= 0 && _nWait_car6 < 17)
                        Canvas.SetTop(samochod6, (_nWait_car6) * 10);
                    if (_nWait_car6 == 17)
                    {
                        samochod6.Visibility = Visibility.Hidden;
                        samochod6_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car6 > 17 && _nWait_car6 <36 )
                    {
                        Canvas.SetLeft(samochod6_skret, (_nWait_car6 - 17) * 10);
                        stop_6 = true;
                        pojazd_6_stop = false;
                    }

                    if (pojazd_8_stop == true)
                    {
                        if (_nWait_car6 >= 36 && _nWait_car6 <= 60)
                        {
                            if (zielone8.Visibility == Visibility.Hidden && stop_6 == true)
                            {
                                _nWait_car6--;
                                pojazd_6_stop = true;
                            }
                            else
                            {
                                pojazd_6_stop = false;
                                stop_6 = false;
                                Canvas.SetLeft(samochod6_skret, (_nWait_car6 - 17) * 10);
                                if (_nWait_car6 == 55)
                                {
                                    samochod6_skret.Visibility = Visibility.Hidden;
                                    Canvas.SetTop(samochod6_skret_2, 0.0);
                                    samochod6_skret_2.Visibility = Visibility.Visible;
                                    ruch_samochod6_2 = true;
                                    _nWait_car6 = 0;
                                    _sygnalizacja.pojazd6 = false;
                                    ruch_samochod6 = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_nWait_car6 >= 36 && _nWait_car6 <43 )
                        Canvas.SetLeft(samochod6_skret, (_nWait_car6 - 17) * 10);

                        if (_nWait_car6 >= 43 && _nWait_car6 <= 60)
                        {
                            if (zielone8.Visibility == Visibility.Hidden && stop_6 == true)
                            {
                                _nWait_car6--;
                                pojazd_6_stop = true;
                            }
                            else
                            {
                                pojazd_6_stop = false;
                                stop_6 = false;
                                Canvas.SetLeft(samochod6_skret, (_nWait_car6 - 17) * 10);
                                if (_nWait_car6 == 55)
                                {
                                    samochod6_skret.Visibility = Visibility.Hidden;
                                    Canvas.SetTop(samochod6_skret_2, 0.0);
                                    samochod6_skret_2.Visibility = Visibility.Visible;
                                    ruch_samochod6_2 = true;
                                    _nWait_car6 = 0;
                                    _sygnalizacja.pojazd6 = false;
                                    ruch_samochod6 = false;
                                }
                            }
                        }
                    }
                }
                else
                    ruch_samochod6 = false;
            }

            ///Samochod 6 - jazda II
            if (ruch_samochod6_2 == true)
            {

                _nWait_car6_2++;

                if (_nWait_car6_2 >= 0 && _nWait_car6_2 < 7)
                {
                    Canvas.SetTop(samochod6_skret_2, (_nWait_car6_2) * 10);
                    stop_6_2 = true;
                    pojazd_6_stop_2 = false;
                }

                if ((pojazd_5_stop == true && pojazd_8_stop_2 == false) || (pojazd_8_stop_2 == true && pojazd_5_stop == false))
                {
                    if (_nWait_car6_2 >= 7 && _nWait_car6_2 < 13)
                        Canvas.SetTop(samochod6_skret_2, (_nWait_car6_2) * 10);

                    if (_nWait_car6_2 >= 13)
                    {
                        if (zielone4.Visibility == Visibility.Hidden && stop_6_2 == true)
                        {
                            _nWait_car6_2--;
                            pojazd_6_stop_2 = true;
                        }
                        else
                        {
                            pojazd_6_stop_2 = false;
                            stop_6_2 = false;
                            Canvas.SetTop(samochod6_skret_2, (_nWait_car6_2) * 10);
                            if (_nWait_car6_2 == 45)
                            {
                                samochod6_skret_2.Visibility = Visibility.Hidden;
                                _nWait_car6_2 = 0;
                                ruch_samochod6_2 = false;
                                opuszczenie_skrzyzowania_6 = true;
                            }
                        }
                    }
                }
                else if (pojazd_5_stop == true && pojazd_8_stop_2 == true)
                {
                    if (_nWait_car6_2 >= 7)
                    {
                        if (zielone4.Visibility == Visibility.Hidden && stop_6_2 == true)
                        {
                            _nWait_car6_2--;
                            pojazd_6_stop_2 = true;
                        }
                        else
                        {
                            pojazd_6_stop_2 = false;
                            stop_6_2 = false;
                            Canvas.SetTop(samochod6_skret_2, (_nWait_car6_2) * 10);
                            if (_nWait_car6_2 == 45)
                            {
                                samochod6_skret_2.Visibility = Visibility.Hidden;
                                _nWait_car6_2 = 0;
                                ruch_samochod6_2 = false;
                                opuszczenie_skrzyzowania_6 = true;
                            }
                        }
                    }
                }
                else
                {
                    if (_nWait_car6_2 >= 7 && _nWait_car6_2 < 19)
                        Canvas.SetTop(samochod6_skret_2, (_nWait_car6_2) * 10);

                    if (_nWait_car6_2 >= 19)
                    {
                        if (zielone4.Visibility == Visibility.Hidden && stop_6_2 == true)
                        {
                            _nWait_car6_2--;
                            pojazd_6_stop_2 = true;
                        }
                        else
                        {
                            pojazd_6_stop_2 = false;
                            stop_6_2 = false;
                            Canvas.SetTop(samochod6_skret_2, (_nWait_car6_2) * 10);
                            if (_nWait_car6_2 == 45)
                            {
                                samochod6_skret_2.Visibility = Visibility.Hidden;
                                _nWait_car6_2 = 0;
                                ruch_samochod6_2 = false;
                                opuszczenie_skrzyzowania_6 = true;
                            }
                        }
                    }
                }
            }

            ///Samochod 7 - jazda
            if (ruch_samochod7 == true)
            {
                if (zielone12.Visibility == Visibility.Visible || _nWait_car7 != 0)
                {
                    _nWait_car7++;
                    if (_nWait_car7 >= 0)
                        Canvas.SetTop(samochod7, (_nWait_car7) * 10);
                    if (_nWait_car7 == 30)
                    {
                        samochod7.Visibility = Visibility.Hidden;
                        _nWait_car7 = 0;
                        _sygnalizacja.pojazd7 = false;
                        ruch_samochod7 = false;
                        _zmiana_samochodu7 = true;
                    }
                }
                else
                    ruch_samochod7 = false;
            }

            ///Samochod 7 - skret
            if (ruch_samochod7_skret == true)
            {
                if (zielone12.Visibility == Visibility.Visible || _nWait_car7 != 0)
                {
                    _nWait_car7++;
                    if (_nWait_car7 >= 0)
                        Canvas.SetTop(samochod7, (_nWait_car7) * 10);
                    if (_nWait_car7 == 10)
                    {
                        samochod7.Visibility = Visibility.Hidden;
                        samochod7_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car7 > 10)
                    {
                        Canvas.SetRight(samochod7_skret, (_nWait_car7 - 10) * 15);
                    }
                    if (_nWait_car7 == 15)
                    {
                        samochod7_skret.Visibility = Visibility.Hidden;
                        _nWait_car7 = 0;
                        _sygnalizacja.pojazd7 = false;
                        ruch_samochod7_skret = false;
                        _zmiana_samochodu7 = false;
                    }
                }
                else
                    ruch_samochod7_skret = false;
            }

            ///Samochod 8 - jazda
            if (ruch_samochod8 == true)
            {
                if (zielone13.Visibility == Visibility.Visible || _nWait_car8 != 0)
                {
                    _nWait_car8++;
                    if (_nWait_car8 >= 0 && _nWait_car8 < 37)
                    {
                        Canvas.SetLeft(samochod8, (_nWait_car8) * 10);
                        stop_8 = true;
                        pojazd_8_stop = false;
                    }

                    if (pojazd_6_stop == true)
                    {
                        if (_nWait_car8 >= 37 && _nWait_car8 <= 60)
                        {
                            if (zielone8.Visibility == Visibility.Hidden && stop_8 == true)
                            {
                                pojazd_8_stop = true;
                                _nWait_car8--;
                                _fala_8 = true;
                            }
                            else
                            {
                                pojazd_8_stop = false;
                                _fala_8 = false;
                                stop_8 = false;
                                Canvas.SetLeft(samochod8, (_nWait_car8) * 10);
                                if (_nWait_car8 == 55)
                                {
                                    samochod8.Visibility = Visibility.Hidden;
                                    Canvas.SetTop(samochod8_skret_2, 0.0);
                                    samochod8_skret_2.Visibility = Visibility.Visible;
                                    ruch_samochod8_2 = true;
                                    _nWait_car8 = 0;
                                    _sygnalizacja.pojazd8 = false;
                                    ruch_samochod8 = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_nWait_car8 >= 37 && _nWait_car8 < 43)
                        Canvas.SetLeft(samochod8, (_nWait_car8) * 10);

                        if (_nWait_car8 >= 43 && _nWait_car8 <= 60)
                        {
                            if (zielone8.Visibility == Visibility.Hidden && stop_8 == true)
                            {
                                pojazd_8_stop = true;
                                _nWait_car8--;
                                _fala_8 = true;
                            }
                            else
                            {
                                pojazd_8_stop = false;
                                stop_8 = false;
                                _fala_8 = false;
                                Canvas.SetLeft(samochod8, (_nWait_car8) * 10);
                                if (_nWait_car8 == 55)
                                {
                                    samochod8.Visibility = Visibility.Hidden;
                                    Canvas.SetTop(samochod8_skret_2, 0.0);
                                    samochod8_skret_2.Visibility = Visibility.Visible;
                                    ruch_samochod8_2 = true;
                                    _nWait_car8 = 0;
                                    _sygnalizacja.pojazd8 = false;
                                    ruch_samochod8 = false;
                                }
                            }
                        }
                    }
                }
                else
                    ruch_samochod8 = false;
            }

            ///Samochod 8 - jazda II
            if (ruch_samochod8_2 == true)
            {

                _nWait_car8_2++;

                if (_nWait_car8_2 >= 0 && _nWait_car8_2 < 7)
                {
                    Canvas.SetTop(samochod8_skret_2, (_nWait_car8_2) * 10);
                    stop_8_2 = true;
                    pojazd_8_stop_2 = false;
                }

                if ((pojazd_5_stop == true && pojazd_6_stop_2 == false) || (pojazd_6_stop_2 == true && pojazd_5_stop == false))
                {
                    if (_nWait_car8_2 >= 7 && _nWait_car8_2 < 13)
                        Canvas.SetTop(samochod8_skret_2, (_nWait_car8_2) * 10);

                    if (_nWait_car8_2 >= 13)
                    {
                        if (zielone4.Visibility == Visibility.Hidden && stop_8_2 == true)
                        {
                            _nWait_car8_2--;
                            pojazd_8_stop_2 = true;
                            _fala_8 = true;
                        }
                        else
                        {
                            pojazd_8_stop_2 = false;
                            stop_8_2 = false;
                            _fala_8 = false;
                            Canvas.SetTop(samochod8_skret_2, (_nWait_car8_2) * 10);
                            if (_nWait_car8_2 == 45)
                            {
                                samochod8_skret_2.Visibility = Visibility.Hidden;
                                _nWait_car8_2 = 0;
                                ruch_samochod8_2 = false;
                                opuszczenie_skrzyzowania_8 = true;
                            }
                        }
                    }
                }
                else if (pojazd_5_stop == true && pojazd_6_stop_2 == true)
                {
                    if (_nWait_car8_2 >= 7)
                    {
                        if (zielone4.Visibility == Visibility.Hidden && stop_8_2 == true)
                        {
                            _nWait_car8_2--;
                            pojazd_8_stop_2 = true;
                            _fala_8 = true;
                        }
                        else
                        {
                            pojazd_8_stop_2 = false;
                            stop_8_2 = false;
                            _fala_8 = false;
                            Canvas.SetTop(samochod8_skret_2, (_nWait_car8_2) * 10);
                            if (_nWait_car8_2 == 45)
                            {
                                samochod8_skret_2.Visibility = Visibility.Hidden;
                                _nWait_car8_2 = 0;
                                ruch_samochod8_2 = false;
                                opuszczenie_skrzyzowania_8 = true;
                            }
                        }
                    }
                }
                else
                {
                    if (_nWait_car8_2 >= 7 && _nWait_car8_2 < 19)
                        Canvas.SetTop(samochod8_skret_2, (_nWait_car8_2) * 10);

                    if (_nWait_car8_2 >= 19)
                    {
                        if (zielone4.Visibility == Visibility.Hidden && stop_8_2 == true)
                        {
                            _nWait_car8_2--;
                            pojazd_8_stop_2 = true;
                            _fala_8 = true;
                        }
                        else
                        {
                            pojazd_8_stop_2 = false;
                            stop_8_2 = false;
                            _fala_8 = false;
                            Canvas.SetTop(samochod8_skret_2, (_nWait_car8_2) * 10);
                            if (_nWait_car8_2 == 45)
                            {
                                samochod8_skret_2.Visibility = Visibility.Hidden;
                                _nWait_car8_2 = 0;
                                ruch_samochod8_2 = false;
                                opuszczenie_skrzyzowania_8 = true;
                            }
                        }
                    }
                }
            }

            ///Samochod 9 - jazda
            if (ruch_samochod9 == true)
            {
                if (zielone14.Visibility == Visibility.Visible || _nWait_car9 != 0)
                {
                    _nWait_car9++;
                    if (_nWait_car9 >= 0 && _nWait_car9 < 17)
                        Canvas.SetTop(samochod9, -(_nWait_car9) * 10);
                    if (_nWait_car9 == 17)
                    {
                        samochod9.Visibility = Visibility.Hidden;
                        samochod9_skret.Visibility = Visibility.Visible;
                    }
                    if (_nWait_car9 > 17)
                    {
                        Canvas.SetRight(samochod9_skret, (_nWait_car9 - 17) * 10);
                    }
                    if (_nWait_car9 == 30)
                    {
                        samochod9_skret.Visibility = Visibility.Hidden;
                        _nWait_car9 = 0;
                        _sygnalizacja.pojazd9 = false;
                        ruch_samochod9 = false;
                    }
                }
                else
                    ruch_samochod9 = false;
            }

            ///Samochod 10 - jazda
            if (ruch_samochod10 == true)
            {
                if (zielone15.Visibility == Visibility.Visible || _nWait_car10 != 0)
                {
                    _nWait_car10++;
                    if (_nWait_car10 >= 0)
                        Canvas.SetTop(samochod10, -(_nWait_car10) * 10);
                    if (_nWait_car10 == 30)
                    {
                        samochod10.Visibility = Visibility.Hidden;
                        _nWait_car10 = 0;
                        _sygnalizacja.pojazd10 = false;
                        ruch_samochod10 = false;
                    }
                }
                else
                    ruch_samochod10 = false;
            }

                    /// Ruszanie - sygnalizacja 1
            if (_zmiana1)
            {
                if (_nWait2 < 10)
                {
                    zielone1.Visibility = Visibility.Hidden;
                    czerwone1.Visibility = Visibility.Visible;
                    zolte1.Visibility = Visibility.Visible;
                    _nWait2++;
                }
                else
                {
                    _nWait2 = 0;
                    _zmiana1 = false;
                    zielone1.Visibility = Visibility.Visible;
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
                    _nWait3++;
                }
                else
                {
                    _nWait3 = 0;
                    _zmiana2 = false;
                    zielone1.Visibility = Visibility.Hidden;
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
                    czerwone2.Visibility = Visibility.Visible;
                    zolte2.Visibility = Visibility.Visible;
                    _nWait4++;
                }
                else
                {
                    _nWait4 = 0;
                    _zmiana3 = false;
                    zielone2.Visibility = Visibility.Visible;
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
                    _nWait5++;
                }
                else
                {
                    _nWait5 = 0;
                    _zmiana4 = false;
                    zielone2.Visibility = Visibility.Hidden;
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
                    czerwone3.Visibility = Visibility.Visible;
                    zolte3.Visibility = Visibility.Visible;
                    _nWait6++;
                }
                else
                {
                    _nWait6 = 0;
                    _zmiana5 = false;
                    zielone3.Visibility = Visibility.Visible;
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
                    _nWait7++;
                }
                else
                {
                    _nWait7 = 0;
                    _zmiana6 = false;
                    zielone3.Visibility = Visibility.Hidden;
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
                    czerwone4.Visibility = Visibility.Visible;
                    zolte4.Visibility = Visibility.Visible;
                    _nWait8++;
                }
                else
                {
                    _nWait8 = 0;
                    _zmiana7 = false;
                    zielone4.Visibility = Visibility.Visible;
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
                    _nWait9++;
                }
                else
                {
                    _nWait9 = 0;
                    _zmiana8 = false;
                    zielone4.Visibility = Visibility.Hidden;
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
                    czerwone5.Visibility = Visibility.Visible;
                    zolte5.Visibility = Visibility.Visible;
                    _nWait10++;
                }
                else
                {
                    _nWait10 = 0;
                    _zmiana9 = false;
                    zielone5.Visibility = Visibility.Visible;
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
                    _nWait11++;
                }
                else
                {
                    _nWait11 = 0;
                    _zmiana10 = false;
                    zielone5.Visibility = Visibility.Hidden;
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
                    czerwone6.Visibility = Visibility.Visible;
                    zolte6.Visibility = Visibility.Visible;
                    _nWait12++;
                }
                else
                {
                    _nWait12 = 0;
                    _zmiana11 = false;
                    zielone6.Visibility = Visibility.Visible;
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
                    _nWait13++;
                }
                else
                {
                    _nWait13 = 0;
                    _zmiana12 = false;
                    zielone6.Visibility = Visibility.Hidden;
                    czerwone6.Visibility = Visibility.Visible;
                    zolte6.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 7
            if (_zmiana13)
            {
                if (_nWait14 < 10)
                {
                    zielone7.Visibility = Visibility.Hidden;
                    czerwone7.Visibility = Visibility.Visible;
                    zolte7.Visibility = Visibility.Visible;
                    _nWait14++;
                }
                else
                {
                    _nWait14 = 0;
                    _zmiana13 = false;
                    zielone7.Visibility = Visibility.Visible;
                    czerwone7.Visibility = Visibility.Hidden;
                    zolte7.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 7
            if (_zmiana14)
            {
                if (_nWait15 < 30)
                {
                    zielone7.Visibility = Visibility.Hidden;
                    czerwone7.Visibility = Visibility.Hidden;
                    zolte7.Visibility = Visibility.Visible;
                    _nWait15++;
                }
                else
                {
                    _nWait15 = 0;
                    _zmiana14 = false;
                    zielone7.Visibility = Visibility.Hidden;
                    czerwone7.Visibility = Visibility.Visible;
                    zolte7.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 8
            if (_zmiana15)
            {
                if (_nWait16 < 10)
                {
                    zielone8.Visibility = Visibility.Hidden;
                    czerwone8.Visibility = Visibility.Visible;
                    zolte8.Visibility = Visibility.Visible;
                    _nWait16++;
                }
                else
                {
                    _nWait16 = 0;
                    _zmiana15 = false;
                    zielone8.Visibility = Visibility.Visible;
                    czerwone8.Visibility = Visibility.Hidden;
                    zolte8.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 8
            if (_zmiana16)
            {
                if (_nWait17 < 30)
                {
                    zielone8.Visibility = Visibility.Hidden;
                    czerwone8.Visibility = Visibility.Hidden;
                    zolte8.Visibility = Visibility.Visible;
                    _nWait17++;
                }
                else
                {
                    _nWait17 = 0;
                    _zmiana16 = false;
                    zielone8.Visibility = Visibility.Hidden;
                    czerwone8.Visibility = Visibility.Visible;
                    zolte8.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 10
            if (_zmiana17)
            {
                if (_nWait17 < 10)
                {
                    zielone10.Visibility = Visibility.Hidden;
                    czerwone10.Visibility = Visibility.Visible;
                    zolte10.Visibility = Visibility.Visible;
                    _nWait17++;
                }
                else
                {
                    _nWait17 = 0;
                    _zmiana17 = false;
                    zielone10.Visibility = Visibility.Visible;
                    czerwone10.Visibility = Visibility.Hidden;
                    zolte10.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 10
            if (_zmiana18)
            {
                if (_nWait18 < 30)
                {
                    zielone10.Visibility = Visibility.Hidden;
                    czerwone10.Visibility = Visibility.Hidden;
                    zolte10.Visibility = Visibility.Visible;
                    _nWait18++;
                }
                else
                {
                    _nWait18 = 0;
                    _zmiana18 = false;
                    zielone10.Visibility = Visibility.Hidden;
                    czerwone10.Visibility = Visibility.Visible;
                    zolte10.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 11
            if (_zmiana19)
            {
                if (_nWait19 < 10)
                {
                    zielone11.Visibility = Visibility.Hidden;
                    czerwone11.Visibility = Visibility.Visible;
                    zolte11.Visibility = Visibility.Visible;
                    _nWait19++;
                }
                else
                {
                    _nWait19 = 0;
                    _zmiana19 = false;
                    zielone11.Visibility = Visibility.Visible;
                    czerwone11.Visibility = Visibility.Hidden;
                    zolte11.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 11
            if (_zmiana20)
            {
                if (_nWait20 < 30)
                {
                    zielone11.Visibility = Visibility.Hidden;
                    czerwone11.Visibility = Visibility.Hidden;
                    zolte11.Visibility = Visibility.Visible;
                    _nWait20++;
                }
                else
                {
                    _nWait20 = 0;
                    _zmiana20 = false;
                    zielone11.Visibility = Visibility.Hidden;
                    czerwone11.Visibility = Visibility.Visible;
                    zolte11.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 12
            if (_zmiana21)
            {
                if (_nWait21 < 10)
                {
                    zielone12.Visibility = Visibility.Hidden;
                    czerwone12.Visibility = Visibility.Visible;
                    zolte12.Visibility = Visibility.Visible;
                    _nWait21++;
                }
                else
                {
                    _nWait21 = 0;
                    _zmiana21 = false;
                    zielone12.Visibility = Visibility.Visible;
                    czerwone12.Visibility = Visibility.Hidden;
                    zolte12.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 12
            if (_zmiana22)
            {
                if (_nWait22 < 30)
                {
                    zielone12.Visibility = Visibility.Hidden;
                    czerwone12.Visibility = Visibility.Hidden;
                    zolte12.Visibility = Visibility.Visible;
                    _nWait22++;
                }
                else
                {
                    _nWait22 = 0;
                    _zmiana22 = false;
                    zielone12.Visibility = Visibility.Hidden;
                    czerwone12.Visibility = Visibility.Visible;
                    zolte12.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 13
            if (_zmiana23)
            {
                if (_nWait23 < 10)
                {
                    zielone13.Visibility = Visibility.Hidden;
                    czerwone13.Visibility = Visibility.Visible;
                    zolte13.Visibility = Visibility.Visible;
                    _nWait23++;
                }
                else
                {
                    _nWait23 = 0;
                    _zmiana23 = false;
                    zielone13.Visibility = Visibility.Visible;
                    czerwone13.Visibility = Visibility.Hidden;
                    zolte13.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 13
            if (_zmiana24)
            {
                if (_nWait24 < 30)
                {
                    zielone13.Visibility = Visibility.Hidden;
                    czerwone13.Visibility = Visibility.Hidden;
                    zolte13.Visibility = Visibility.Visible;
                    _nWait24++;
                }
                else
                {
                    _nWait24 = 0;
                    _zmiana24 = false;
                    zielone13.Visibility = Visibility.Hidden;
                    czerwone13.Visibility = Visibility.Visible;
                    zolte13.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 14
            if (_zmiana25)
            {
                if (_nWait25 < 10)
                {
                    zielone14.Visibility = Visibility.Hidden;
                    czerwone14.Visibility = Visibility.Visible;
                    zolte14.Visibility = Visibility.Visible;
                    _nWait25++;
                }
                else
                {
                    _nWait25 = 0;
                    _zmiana25 = false;
                    zielone14.Visibility = Visibility.Visible;
                    czerwone14.Visibility = Visibility.Hidden;
                    zolte14.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 14
            if (_zmiana26)
            {
                if (_nWait26 < 30)
                {
                    zielone14.Visibility = Visibility.Hidden;
                    czerwone14.Visibility = Visibility.Hidden;
                    zolte14.Visibility = Visibility.Visible;
                    _nWait26++;
                }
                else
                {
                    _nWait26 = 0;
                    _zmiana26 = false;
                    zielone14.Visibility = Visibility.Hidden;
                    czerwone14.Visibility = Visibility.Visible;
                    zolte14.Visibility = Visibility.Hidden;
                }
            }
            /// Ruszanie - sygnalizacja 15
            if (_zmiana27)
            {
                if (_nWait27 < 10)
                {
                    zielone15.Visibility = Visibility.Hidden;
                    czerwone15.Visibility = Visibility.Visible;
                    zolte15.Visibility = Visibility.Visible;
                    _nWait27++;
                }
                else
                {
                    _nWait27 = 0;
                    _zmiana27 = false;
                    zielone15.Visibility = Visibility.Visible;
                    czerwone15.Visibility = Visibility.Hidden;
                    zolte15.Visibility = Visibility.Hidden;
                }
            }
            /// Zatrzymanie - sygnalizacja 15
            if (_zmiana28)
            {
                if (_nWait28 < 30)
                {
                    zielone15.Visibility = Visibility.Hidden;
                    czerwone15.Visibility = Visibility.Hidden;
                    zolte15.Visibility = Visibility.Visible;
                    _nWait28++;
                }
                else
                {
                    _nWait28 = 0;
                    _zmiana28 = false;
                    zielone15.Visibility = Visibility.Hidden;
                    czerwone15.Visibility = Visibility.Visible;
                    zolte15.Visibility = Visibility.Hidden;
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
            car7.Content = Messages.msgPojazd7;
            car8.Content = Messages.msgPojazd8;
            car9.Content = Messages.msgPojazd9;
            car10.Content = Messages.msgPojazd10;

            /* Ustawienie etykiet */
            setDOCaptions(ref _lblDOCaptions);
            setDICaptions(ref _lblDICaptions);

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
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 7. **********************************************
        /// </summary>
        void auto7_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd7 = false;
                USBComm.usbSetDO(6, Convert.ToInt16(_sygnalizacja.pojazd7));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 7. *********************************************
        /// </summary>
        void auto7_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd7 = true;
                USBComm.usbSetDO(6, Convert.ToInt16(_sygnalizacja.pojazd7));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 8. **********************************************
        /// </summary>
        void auto8_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd8 = false;
                USBComm.usbSetDO(7, Convert.ToInt16(_sygnalizacja.pojazd8));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 8. *********************************************
        /// </summary>
        void auto8_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd8 = true;
                USBComm.usbSetDO(7, Convert.ToInt16(_sygnalizacja.pojazd8));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 9. **********************************************
        /// </summary>
        void auto9_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd9 = false;
                USBComm.usbSetDO(8, Convert.ToInt16(_sygnalizacja.pojazd9));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 9. *********************************************
        /// </summary>
        void auto9_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd9 = true;
                USBComm.usbSetDO(8, Convert.ToInt16(_sygnalizacja.pojazd9));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia zwolnienia przycisku pojazd 10. **********************************************
        /// </summary>
        void auto10_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd10 = false;
                USBComm.usbSetDO(9, Convert.ToInt16(_sygnalizacja.pojazd10));
            }
        }

        /// <summary>
        /// Obsługa zdarzenia naciśnięcia przycisku pojazd 10. *********************************************
        /// </summary>
        void auto10_brak_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State == TabStates.SIMULATION)
            {
                _sygnalizacja.pojazd10 = true;
                USBComm.usbSetDO(9, Convert.ToInt16(_sygnalizacja.pojazd10));
            }
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
                for (int i = 0; i < 15; i++)
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
        PLCEmulator.SygnalizacjaIII.Sygnalizacja _sygnalizacja = new PLCEmulator.SygnalizacjaIII.Sygnalizacja();
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
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 7.
        /// </summary>
        int _nWait14 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 7.
        /// </summary>
        int _nWait15 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 8.
        /// </summary>
        int _nWait16 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 8.
        /// </summary>
        int _nWait17 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 10.
        /// </summary>
        int _nWait18 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 10.
        /// </summary>
        int _nWait19 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 11.
        /// </summary>
        int _nWait20 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 11.
        /// </summary>
        int _nWait21 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 12.
        /// </summary>
        int _nWait22 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 12.
        /// </summary>
        int _nWait23 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 13.
        /// </summary>
        int _nWait24 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 13.
        /// </summary>
        int _nWait25 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 14.
        /// </summary>
        int _nWait26 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 14.
        /// </summary>
        int _nWait27 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do ruszania - sygnalizacja 15.
        /// </summary>
        int _nWait28 = 0;
        /// <summary>
        /// Zmienna czasu dla światła przygotowania do zatrzymania - sygnalizacja 15.
        /// </summary>
        int _nWait29 = 0;
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
        /// Poprzedni stan na sygnalizacji 7.
        /// </summary>
        bool _sygnal7 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 8.
        /// </summary>
        bool _sygnal8 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 10.
        /// </summary>
        bool _sygnal10 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 11.
        /// </summary>
        bool _sygnal11 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 12.
        /// </summary>
        bool _sygnal12 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 13.
        /// </summary>
        bool _sygnal13 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 14.
        /// </summary>
        bool _sygnal14 = false;
        /// <summary>
        /// Poprzedni stan na sygnalizacji 15.
        /// </summary>
        bool _sygnal15 = false;
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
        /// zmiana na sygnalizacji 7 do ruszania.
        /// </summary>
        bool _zmiana13 = false;
        /// <summary>
        /// zmiana na sygnalizacji 7 do zatrzymania.
        /// </summary>
        bool _zmiana14 = false;
        /// <summary>
        /// zmiana na sygnalizacji 8 do ruszania.
        /// </summary>
        bool _zmiana15 = false;
        /// <summary>
        /// zmiana na sygnalizacji 8 do zatrzymania.
        /// </summary>
        bool _zmiana16 = false;
        /// <summary>
        /// zmiana na sygnalizacji 10 do ruszania.
        /// </summary>
        bool _zmiana17 = false;
        /// <summary>
        /// zmiana na sygnalizacji 10 do zatrzymania.
        /// </summary>
        bool _zmiana18 = false;
        /// <summary>
        /// zmiana na sygnalizacji 11 do ruszania.
        /// </summary>
        bool _zmiana19 = false;
        /// <summary>
        /// zmiana na sygnalizacji 11 do zatrzymania.
        /// </summary>
        bool _zmiana20 = false;
        /// <summary>
        /// zmiana na sygnalizacji 12 do ruszania.
        /// </summary>
        bool _zmiana21 = false;
        /// <summary>
        /// zmiana na sygnalizacji 12 do zatrzymania.
        /// </summary>
        bool _zmiana22 = false;
        /// <summary>
        /// zmiana na sygnalizacji 13 do ruszania.
        /// </summary>
        bool _zmiana23 = false;
        /// <summary>
        /// zmiana na sygnalizacji 13 do zatrzymania.
        /// </summary>
        bool _zmiana24 = false;
        /// <summary>
        /// zmiana na sygnalizacji 14 do ruszania.
        /// </summary>
        bool _zmiana25 = false;
        /// <summary>
        /// zmiana na sygnalizacji 14 do zatrzymania.
        /// </summary>
        bool _zmiana26 = false;
        /// <summary>
        /// zmiana na sygnalizacji 15 do ruszania.
        /// </summary>
        bool _zmiana27 = false;
        /// <summary>
        /// zmiana na sygnalizacji 15 do zatrzymania.
        /// </summary>
        bool _zmiana28 = false;
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
        /// Zmienna czasu dla samochodu 1.
        /// </summary>
        int _nWait_car1 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 2.
        /// </summary>
        int _nWait_car2 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 2_2.
        /// </summary>
        int _nWait_car2_2 = 0;
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
        /// Zmienna czasu dla samochodu 6_2.
        /// </summary>
        int _nWait_car6_2 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 7.
        /// </summary>
        int _nWait_car7 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 8.
        /// </summary>
        int _nWait_car8 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 8.
        /// </summary>
        int _nWait_car8_2 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 9.
        /// </summary>
        int _nWait_car9 = 0;
        /// <summary>
        /// Zmienna czasu dla samochodu 10.
        /// </summary>
        int _nWait_car10 = 0;
        /// <summary>
        /// ruch samochodu 1.
        /// </summary>
        bool ruch_samochod1 = false;
        /// <summary>
        /// ruch samochodu 2.
        /// </summary>
        bool ruch_samochod2 = false;
        /// <summary>
        /// ruch samochodu 2_2.
        /// </summary>
        bool ruch_samochod2_2 = false;
        /// <summary>
        /// zatrzymanie samochodu 2.
        /// </summary>
        bool stop_2 = false;
        /// <summary>
        /// zatrzymanie samochodu 2_2.
        /// </summary>
        bool stop_2_2 = false;
        /// <summary>
        /// zatrzymanie samochodu 2 - fala.
        /// </summary>
        bool _fala_2 = false;
        /// <summary>
        /// ruch samochodu 3.
        /// </summary>
        bool ruch_samochod3 = false;
        /// <summary>
        /// ruch samochodu 4.
        /// </summary>
        bool ruch_samochod4 = false;
        /// <summary>
        /// ruch samochodu 4 skret.
        /// </summary>
        bool _zmiana_samochodu4 = false;
        /// <summary>
        /// ruch samochodu 4.
        /// </summary>
        bool ruch_samochod4_skret = false;
        /// <summary>
        /// ruch samochodu 5.
        /// </summary>
        bool ruch_samochod5 = false;
        /// <summary>
        /// zatrzymanie samochodu 5.
        /// </summary>
        bool stop_5 = false;
        /// <summary>
        /// zatrzymanie pojazdu 5.
        /// </summary>
        bool pojazd_5_stop = false;
        /// <summary>
        /// ruch samochodu 6.
        /// </summary>
        bool ruch_samochod6 = false;
        /// <summary>
        /// ruch samochodu 6_2.
        /// </summary>
        bool ruch_samochod6_2 = false;
        /// <summary>
        /// zatrzymanie samochodu 6.
        /// </summary>
        bool stop_6 = false;
        /// <summary>
        /// zatrzymanie samochodu 6_2.
        /// </summary>
        bool stop_6_2 = false;
        /// <summary>
        /// zatrzymanie pojazdu 6.
        /// </summary>
        bool pojazd_6_stop = false;
        /// <summary>
        /// zatrzymanie pojazdu 6_2.
        /// </summary>
        bool pojazd_6_stop_2 = false;
        /// <summary>
        /// ruch samochodu 7.
        /// </summary>
        bool ruch_samochod7 = false;
        /// <summary>
        /// ruch samochodu 7 skret.
        /// </summary>
        bool _zmiana_samochodu7 = false;
        /// <summary>
        /// ruch samochodu 7.
        /// </summary>
        bool ruch_samochod7_skret = false;
        /// <summary>
        /// ruch samochodu 8.
        /// </summary>
        bool ruch_samochod8 = false;
        /// <summary>
        /// ruch samochodu 8_2.
        /// </summary>
        bool ruch_samochod8_2 = false;
        /// <summary>
        /// zatrzymanie pojazdu 8.
        /// </summary>
        bool pojazd_8_stop = false;
        /// <summary>
        /// zatrzymanie pojazdu 8_2.
        /// </summary>
        bool pojazd_8_stop_2 = false;
        /// <summary>
        /// zatrzymanie samochodu 8.
        /// </summary>
        bool stop_8 = false;
        /// <summary>
        /// zatrzymanie samochodu 8_2.
        /// </summary>
        bool stop_8_2 = false;
        /// <summary>
        /// zatrzymanie samochodu 8 - fala.
        /// </summary>
        bool _fala_8 = false;
        /// <summary>
        /// ruch samochodu 9.
        /// </summary>
        bool ruch_samochod9 = false;
        /// <summary>
        /// ruch samochodu 10.
        /// </summary>
        bool ruch_samochod10 = false;
        /// <summary>
        /// funkcja losowania - tryb testowy
        /// </summary>
        int zmienna_losowa;
        System.Random x = new Random(System.DateTime.Now.Millisecond);
        int _Wait_Test = 0;
        int _licznik_testu = 0;
        double procenty = 0.0;
        double suma_pojazdów = 0.0;
        /// <summary>
        /// test pojazd 1.
        /// </summary>
        bool _pomocnicza1 = true;
        /// <summary>
        /// test pojazd 2.
        /// </summary>
        bool _pomocnicza2 = true;
        /// <summary>
        /// test pojazd 3.
        /// </summary>
        bool _pomocnicza3 = true;
        /// <summary>
        /// test pojazd 4.
        /// </summary>
        bool _pomocnicza4 = true;
        /// <summary>
        /// test pojazd 4.
        /// </summary>
        bool _pomocnicza11 = true;
        /// <summary>
        /// test pojazd 5.
        /// </summary>
        bool _pomocnicza5 = true;
        /// <summary>
        /// test pojazd 6.
        /// </summary>
        bool _pomocnicza6 = true;
        /// <summary>
        /// test pojazd 7.
        /// </summary>
        bool _pomocnicza7 = true;
        /// <summary>
        /// test pojazd 7.
        /// </summary>
        bool _pomocnicza12 = true;
        /// <summary>
        /// test pojazd 8.
        /// </summary>
        bool _pomocnicza8 = true;
        /// <summary>
        /// test pojazd 9.
        /// </summary>
        bool _pomocnicza9 = true;
        /// <summary>
        /// test pojazd 10.
        /// </summary>
        bool _pomocnicza10 = true;
        /// <summary>
        /// test kolizja.
        /// </summary>
        bool _kolizja = true;
        /// <summary>
        /// test kolizja.
        /// </summary>
        bool _kolizja_car = true;
        /// <summary>
        /// test Fala.
        /// </summary>
        bool _Fala = true;
        bool opuszczenie_skrzyzowania_2 = false;
        bool opuszczenie_skrzyzowania_5 = false;
        bool opuszczenie_skrzyzowania_6 = false;
        bool opuszczenie_skrzyzowania_8 = false;
    }
}