/* Klasa CaptionsSettings.
 * Klasa ta jest oknem pozwalającym modyfikować ustawienia dla wyświetlanych etykiet tekstowych.
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
using System.Windows.Shapes;
using System.IO;

namespace PLCEmulator
{
    /// <summary>
    /// Interaction logic for CaptionsSettings.xaml
    /// </summary>
    public partial class CaptionsSettings : Window
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/
        
        /// <summary>
        /// Konstruktor.
        /// </summary>
        public              CaptionsSettings()
        {
            InitializeComponent();

            _lblViewBits.Add((Label)FindName("lblViewAIBit"));
            _lblViewBits.Add((Label)FindName("lblViewAOBit"));
            _lblViewBits.Add((Label)FindName("lblViewDIBit"));
            _lblViewBits.Add((Label)FindName("lblViewDOBit"));

            _lblViewStartValues.Add((Label)FindName("lblViewAINo"));
            _lblViewStartValues.Add((Label)FindName("lblViewAONo"));
            _lblViewStartValues.Add((Label)FindName("lblViewDINo"));
            _lblViewStartValues.Add((Label)FindName("lblViewDONo"));

            _lblViewSymbols.Add((Label)FindName("lblViewAISymbol"));
            _lblViewSymbols.Add((Label)FindName("lblViewAOSymbol"));
            _lblViewSymbols.Add((Label)FindName("lblViewDISymbol"));
            _lblViewSymbols.Add((Label)FindName("lblViewDOSymbol"));

            _lblViewDots.Add((Label)FindName("lblViewAIDot"));
            _lblViewDots.Add((Label)FindName("lblViewAODot"));
            _lblViewDots.Add((Label)FindName("lblViewDIDot"));
            _lblViewDots.Add((Label)FindName("lblViewDODot"));

            _tbUserDefSymbols.Add((TextBox)FindName("lblUserDefineSymbol0"));
            _tbUserDefSymbols.Add((TextBox)FindName("lblUserDefineSymbol1"));
            _tbUserDefSymbols.Add((TextBox)FindName("lblUserDefineSymbol2"));
            _tbUserDefSymbols.Add((TextBox)FindName("lblUserDefineSymbol3"));

            _tbUserDefStartValues.Add((TextBox)FindName("lblUserDefineStartValue0"));
            _tbUserDefStartValues.Add((TextBox)FindName("lblUserDefineStartValue1"));
            _tbUserDefStartValues.Add((TextBox)FindName("lblUserDefineStartValue2"));
            _tbUserDefStartValues.Add((TextBox)FindName("lblUserDefineStartValue3"));

            _tbUserDefStartValuesBytes.Add((TextBox)FindName("lblUserDefineStartValueByte0"));
            _tbUserDefStartValuesBytes.Add((TextBox)FindName("lblUserDefineStartValueByte1"));
            _tbUserDefStartValuesBytes.Add((TextBox)FindName("lblUserDefineStartValueByte2"));
            _tbUserDefStartValuesBytes.Add((TextBox)FindName("lblUserDefineStartValueByte3"));

            _tbUserDefStartValuesBits.Add((TextBox)FindName("lblUserDefineStartValueBit0"));
            _tbUserDefStartValuesBits.Add((TextBox)FindName("lblUserDefineStartValueBit1"));
            _tbUserDefStartValuesBits.Add((TextBox)FindName("lblUserDefineStartValueBit2"));
            _tbUserDefStartValuesBits.Add((TextBox)FindName("lblUserDefineStartValueBit3"));

            _tbUserDefNumerations.Add((TextBox)FindName("lblUserDefineNumeration0"));
            _tbUserDefNumerations.Add((TextBox)FindName("lblUserDefineNumeration1"));
            _tbUserDefNumerations.Add((TextBox)FindName("lblUserDefineNumeration2"));
            _tbUserDefNumerations.Add((TextBox)FindName("lblUserDefineNumeration3"));
            
            /* Popranie poprzednich ustawień */
            _nPLCChoice = Properties.Settings.Default.PLCChoice;
            _listStartSymbols.Add(Properties.Settings.Default.PLCAIStartSymbol);
            _listStartValues.Add(Properties.Settings.Default.PLCAIStartValue);
            _listStartValueBits.Add(Properties.Settings.Default.PLCAIStartValueBit);
            _listStartSymbols.Add(Properties.Settings.Default.PLCAOStartSymbol);
            _listStartValues.Add(Properties.Settings.Default.PLCAOStartValue);
            _listStartValueBits.Add(Properties.Settings.Default.PLCAOStartValueBit);
            _listStartSymbols.Add(Properties.Settings.Default.PLCDIStartSymbol);
            _listStartValues.Add(Properties.Settings.Default.PLCDIStartValue);
            _listStartValueBits.Add(Properties.Settings.Default.PLCDIStartValueBit);
            _listStartSymbols.Add(Properties.Settings.Default.PLCDOStartSymbol);
            _listStartValues.Add(Properties.Settings.Default.PLCDOStartValue);
            _listStartValueBits.Add(Properties.Settings.Default.PLCDOStartValueBit);
            _listBits.Add(Properties.Settings.Default.PLCAIBit);
            _listBits.Add(Properties.Settings.Default.PLCAOBit);
            _listBits.Add(Properties.Settings.Default.PLCDIBit);
            _listBits.Add(Properties.Settings.Default.PLCDOBit);

            /* Odczytanie danych z pliku tekstowego */
            try
            {
                StreamReader plik = File.OpenText("PLCCaptions.txt");
                string tmp = null;
                while ((tmp = plik.ReadLine()) != null)
                {
                    _CapSettings.Add(new CapSettings(tmp));
                }
                plik.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem z plikiem z konfiguracją etykiet.\nOkno zostanie zamknięte.\nBłąd: " + e.Message, "Błąd pliku");
                this.Close();
            }
            /* Wypełnienie ComboBoxa */
            for (int i = 0; i < _CapSettings.Count; i++)
                cbSettings.Items.Add(_CapSettings[i].Name);
            /* Ustawienie tekstu */
            setText();
            /* Aktualizacja widoku */
            setView();
        }
        /// <summary>
        /// Ustawia etykiety tekstowe dla całej aplikacji.
        /// </summary>
        void                setText()
        {
            /* Ustawianie tekstu aplikacji */
            this.Title = Properties.Messages.settingsWndTitle;
            gbView.Header = Properties.Messages.settingsWndView;
            lblViewAnalog.Content = Properties.Messages.settingsWndAnalog;
            lblViewDigital.Content = Properties.Messages.settingsWndDiscrete;
            lblViewAOHeader.Content = Properties.Messages.settingsWndOutputs;
            lblViewDOHeader.Content = Properties.Messages.settingsWndOutputs;
            lblViewDIHeader.Content = Properties.Messages.settingsWndInputs;
            lblViewAIHeader.Content = Properties.Messages.settingsWndInputs;
            gbSetType.Header = Properties.Messages.settingsWndSetType;


            lblSettings.Content = Properties.Messages.settingsWndSettings + ":";
            lblUserDefSymbol.Content = Properties.Messages.settingsWndSymbol;
            string[] s = Properties.Messages.settingsWndStartValue.Split(' ');
            lblUserDefValue0.Content = lblUserDefValue1.Content = lblUserDefValue2.Content = s[0];
            lblUserDefValueStart0.Content = lblUserDefValueStart1.Content = lblUserDefValueStart2.Content = s[1];
            lblUserDefValueBit.Content = Properties.Messages.settingsWndBit;
            lblUserDefValueByte.Content = Properties.Messages.settingsWndByte;
            lblUserDefNumeration.Content = Properties.Messages.settingsWngNumeration;

            gbNumerations.Header = Properties.Messages.settingsWngNumeration;
            lblUserDefine8BitNumeration.Content = "8 - " + Properties.Messages.settingsWndBits + " (0.0 - 0.7)";
            lblUserDefine16BitNumeration.Content = "16 - " + Properties.Messages.settingsWndBits + " (0.0 - 0.15)";
            lblUserDefineNormalNumeration.Content = "0 - " + Properties.Messages.settingsWndNormal + " (1; 2; 3; ...)";
            
            buttonClose.Content = Properties.Messages.msgCancel;
            buttonSave.Content = Properties.Messages.msgOK;
            buttonSaveConf.Content = Properties.Messages.settingsWndSaveSettings;
            buttonSaveConfAs.Content = Properties.Messages.settingsWndSaveSettingsAs + "...";

            /* Ustawianie tekstu dla View */
            for (int i = 0; i < 4; i++)
            {
                if (_nPLCChoice == 1)
                {
                    _lblViewBits[i].Content = _listStartValueBits[i];
                    _lblViewStartValues[i].Content = _listStartValues[i];
                    _lblViewSymbols[i].Content = (_listStartValues[i] < 10 ? _listStartSymbols[i] + "000" : _listStartSymbols[i] + "00" );
                }
                else
                {
                    _lblViewBits[i].Content = _listStartValueBits[i];
                    _lblViewStartValues[i].Content = _listStartValues[i];
                    _lblViewSymbols[i].Content = _listStartSymbols[i];
                }
            }

            /* Ustawianie elementów UserDefine */
            for (int i = 0; i < 4; i++)
            {
                _tbUserDefSymbols[i].Text = _listStartSymbols[i];
                _tbUserDefStartValues[i].Text = Convert.ToString(_listStartValues[i]);
                _tbUserDefStartValuesBytes[i].Text = Convert.ToString(_listStartValues[i]);
                _tbUserDefStartValuesBits[i].Text = Convert.ToString(_listStartValueBits[i]);
                _tbUserDefNumerations[i].Text = Convert.ToString(_listBits[i]);
            }

        }
        /// <summary>
        /// Ustawia wygląd okna.
        /// </summary>
        void                setView()
        {
            for (int i = 0; i < 4; i++)
            {
                _lblViewDots[i].Visibility = (_listBits[i] > 0 ? Visibility.Visible : Visibility.Collapsed);
                _lblViewBits[i].Visibility = (_listBits[i] > 0 ? Visibility.Visible : Visibility.Collapsed);
                _tbUserDefStartValues[i].Visibility = (_listBits[i] > 0 ? Visibility.Hidden : Visibility.Visible);
                _tbUserDefStartValuesBytes[i].Visibility = (_listBits[i] > 0 ? Visibility.Visible : Visibility.Hidden);
                _tbUserDefStartValuesBits[i].Visibility = (_listBits[i] > 0 ? Visibility.Visible : Visibility.Hidden);
            }

        }
        /// <summary>
        /// Obsługa zmiany wartości w TextBox'ie związanym z symbolem poczatkowym.
        /// </summary>
        private void        UserDefineSymbol_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int n = Int32.Parse(tb.Name.Remove(0, 19));
            if (tb.Text.Length > 0)
                _listStartSymbols[n] = tb.Text;
            setText();
            setView();
        }
        /// <summary>
        /// Obsługa zmiany wartości w TextBox'ie związanym z wartością początkową.
        /// </summary>
        private void        UserDefineStartValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int n = Int32.Parse(tb.Name.Remove(0, 23));
            if (tb.Text.Length > 0)
                _listStartValues[n] = Int32.Parse(tb.Text);
            setText();
            setView();
        }
        /// <summary>
        /// Obsługa zmiany wartości w TextBox'ie związanym z wartością początkową bitu.
        /// </summary>
        private void        UserDefineStartValueBit_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int n = Int32.Parse(tb.Name.Remove(0, 26));
            if (tb.Text.Length > 0)
                _listStartValueBits[n] = Int32.Parse(tb.Text);
            setText();
            setView();
        }
        /// <summary>
        /// Obsługa zmiany wartości w TextBox'ie związanym z wartością poczatkową bajtu.
        /// </summary>
        private void        UserDefineStartValueByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int n = Int32.Parse(tb.Name.Remove(0, 27));
            if (tb.Text.Length > 0)
                _listStartValues[n] = Int32.Parse(tb.Text);
            setText();
            setView();
        }
        /// <summary>
        /// Obsługa zmiany wartości w TextBox'ie związanym z numeracją.
        /// </summary>
        private void        UserDefineNumeration_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int n = Int32.Parse(tb.Name.Remove(0, 23));
            if(tb.Text.Length > 0)
                _listBits[n] = Int32.Parse(tb.Text);
            setText();
            setView();
        }
        /// <summary>
        /// Obsługa przycisku Zamknij.
        /// </summary>
        private void        buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Obsługa przycisku Zapisz.
        /// </summary>
        private void        buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PLCChoice = _nPLCChoice;
            Properties.Settings.Default.PLCAIStartSymbol = _listStartSymbols[0];
            Properties.Settings.Default.PLCAIStartValue = _listStartValues[0];
            Properties.Settings.Default.PLCAIStartValueBit = _listStartValueBits[0];
            Properties.Settings.Default.PLCAOStartSymbol = _listStartSymbols[1];
            Properties.Settings.Default.PLCAOStartValue = _listStartValues[1];
            Properties.Settings.Default.PLCAOStartValueBit = _listStartValueBits[1];
            Properties.Settings.Default.PLCDIStartSymbol = _listStartSymbols[2];
            Properties.Settings.Default.PLCDIStartValue = _listStartValues[2];
            Properties.Settings.Default.PLCDIStartValueBit = _listStartValueBits[2];
            Properties.Settings.Default.PLCDOStartSymbol = _listStartSymbols[3];
            Properties.Settings.Default.PLCDOStartValue = _listStartValues[3];
            Properties.Settings.Default.PLCDOStartValueBit = _listStartValueBits[3];
            Properties.Settings.Default.PLCAIBit = _listBits[0];
            Properties.Settings.Default.PLCAOBit = _listBits[1];
            Properties.Settings.Default.PLCDIBit = _listBits[2];
            Properties.Settings.Default.PLCDOBit = _listBits[3];
            Properties.Settings.Default.Save();
            this.Close();
        }
        /// <summary>
        /// Obsługa zdarzenia - zmiana elementu w ComboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void        cbSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            /* Wprowadzenie nowych danych do zmiennych */
            int n = cb.SelectedIndex;
            _listStartSymbols[0] = _CapSettings[n].AISymbol;
            _listStartSymbols[1] = _CapSettings[n].AOSymbol;
            _listStartSymbols[2] = _CapSettings[n].DISymbol;
            _listStartSymbols[3] = _CapSettings[n].DOSymbol;

            _listStartValues[0] = _CapSettings[n].AIStartValue;
            _listStartValues[1] = _CapSettings[n].AOStartValue;
            _listStartValues[2] = _CapSettings[n].DIStartValue;
            _listStartValues[3] = _CapSettings[n].DOStartValue;

            _listStartValueBits[0] = _CapSettings[n].AIStartValueBit;
            _listStartValueBits[1] = _CapSettings[n].AOStartValueBit;
            _listStartValueBits[2] = _CapSettings[n].DIStartValueBit;
            _listStartValueBits[3] = _CapSettings[n].DOStartValueBit;

            _listBits[0] = _CapSettings[n].AIBit;
            _listBits[1] = _CapSettings[n].AOBit;
            _listBits[2] = _CapSettings[n].DIBit;
            _listBits[3] = _CapSettings[n].DOBit;

            setText();
            setView();

        }
        /// <summary>
        /// Obsługa zdarzenia - Zamknięcia okna.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void        Window_Closed(object sender, EventArgs e)
        {
            /* Zapis danych do pliku tekstowego */
            try
            {
                StreamWriter plik = File.CreateText("PLCCaptions.txt");
                for (int i = 0; i < _CapSettings.Count; i++)
                    plik.WriteLine(_CapSettings[i].getOutput());
                plik.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem z plikiem z konfiguracją etykiet.\nOkno zostanie zamknięte.\nBłąd: " + ex.Message, "Błąd pliku");
                this.Close();
            }
        }
        /// <summary>
        /// Obsługa zdarzenia - Naciśnięcia przycisku Zapisz ustawienia jako.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void        buttonSaveConfAs_Click(object sender, RoutedEventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(Properties.Messages.settingsWndSettingsName + ".\n" + Properties.Messages.settingsWndSettingsInfo, Properties.Messages.settingsWndSettingsName + ".", "NoweUstawienie",0,0);
            if (name != "")
            {
                for (int i = 0; i < 4; i++)
                    name += " " + _listStartSymbols[i] + " " + _listStartValues[i] + " " + _listStartValueBits[i] + " " + _listBits[i];
                _CapSettings.Add(new CapSettings(name));
                cbSettings.Items.Add(_CapSettings[_CapSettings.Count - 1].Name);
            }
        }
        /// <summary>
        /// Obsługa zdarzenia - Naciśnięcia przycisku Zapisz ustawienia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void        buttonSaveConf_Click(object sender, RoutedEventArgs e)
        {
            int n = cbSettings.SelectedIndex;
            string name = cbSettings.SelectedItem.ToString();
            for (int i = 0; i < 4; i++)
                name += " " + _listStartSymbols[i] + " " + _listStartValues[i] + " " + _listStartValueBits[i] + " " + _listBits[i];
            _CapSettings[n] = new CapSettings(name);
        }

        /************************************************************
         * PROPERTIES                                               *
         ************************************************************/

        /************************************************************
         * FIELDS                                                   *
         ************************************************************/
        /// <summary>
        /// Poprzednoio ustawione opcje.
        /// 0 - dla GE Fanuc
        /// 1 - dla Modicon
        /// 2 - dla Allen-Bradley
        /// 3 - dla Simatic
        /// 4 - dla Użytkownika
        /// </summary>
        int                 _nPLCChoice;
        /// <summary>
        /// Symbole dla I/O.
        /// </summary>
        List<string>        _listStartSymbols = new List<string>();
        /// <summary>
        /// Wartości początkowe dla I/O lub wartości Bajtów I/O.
        /// </summary>
        List<int>           _listStartValues = new List<int>();
        /// <summary>
        /// Wartości początkowe Bitu dla I/O.
        /// </summary>
        List<int>           _listStartValueBits = new List<int>();
        /// <summary>
        /// Informacja o wyświetlaniu bitowym I/O.
        /// </summary>
        List<int>           _listBits = new List<int>();
        /// <summary>
        /// Lista Label'i z wartościami bitów w oknie podglądu.
        /// </summary>
        List<Label>         _lblViewBits = new List<Label>();
        /// <summary>
        /// Lista Label'i z wartościami początkowymi w oknie podglądu.
        /// </summary>
        List<Label>         _lblViewStartValues = new List<Label>();
        /// <summary>
        /// Lista Label'i z symbolami w oknie podglądu.
        /// </summary>
        List<Label>         _lblViewSymbols = new List<Label>();
        /// <summary>
        /// Lista Label'i z kropkami oddzielającymi bity w oknie podglądu.
        /// </summary>
        List<Label>         _lblViewDots = new List<Label>();
        /// <summary>
        /// Lista TextBox'ów z symbolami w oknie UserDefine.
        /// </summary>
        List<TextBox>       _tbUserDefSymbols = new List<TextBox>();
        /// <summary>
        /// Lista TextBox'ów z wartościami początkowymi w oknie UserDefine.
        /// </summary>
        List<TextBox>       _tbUserDefStartValues = new List<TextBox>();
        /// <summary>
        /// Lista TextBox'ów z wartościami początkowymi bajtów w oknie UserDefine.
        /// </summary>
        List<TextBox>       _tbUserDefStartValuesBytes = new List<TextBox>();
        /// <summary>
        /// Lista TextBox'ów z wartościami początkowymi bitów w oknie UserDefine.
        /// </summary>
        List<TextBox>       _tbUserDefStartValuesBits = new List<TextBox>();
        /// <summary>
        /// Lista TextBox'ów z numeracją w oknie UserDefine.
        /// </summary>
        List<TextBox>       _tbUserDefNumerations = new List<TextBox>();
        /// <summary>
        /// Lista z ustawieniami dla etykiet.
        /// </summary>
        List<CapSettings>   _CapSettings = new List<CapSettings>();
    }
}
