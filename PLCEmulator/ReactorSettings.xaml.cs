/* Okno ReactorSettings.
 * Służy do zmiany parametrów zapisanych w pliku konfiguracyjnym.
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

namespace PLCEmulator.Reactor
{
    /// <summary>
    /// Interaction logic for ReactorSettings.xaml
    /// </summary>
    public partial class ReactorSettings : Window
    {
        public          ReactorSettings()
        {
            InitializeComponent();

            txtInitialFill.Text = Convert.ToString(Properties.Reactor.Default.InitialPercentageFilling);
            txtInitialAFlow.Text = Convert.ToString(Properties.Reactor.Default.InitialAFlow);
            txtInitialBFlow.Text = Convert.ToString(Properties.Reactor.Default.InitialBFlow);
            txtOutflowValveValue.Text = Convert.ToString(Properties.Reactor.Default.OutflowValveValue);
            txtSteamTemperature.Text = Convert.ToString(Properties.Reactor.Default.SteamTemperature);
            txtCoolingWaterTemperature.Text = Convert.ToString(Properties.Reactor.Default.CoolingWaterTemperature);
            txtMixerRatio.Text = Convert.ToString(Properties.Reactor.Default.MixerRatio);
            txtEnvironmentTemperature.Text = Convert.ToString(Properties.Reactor.Default.EnvironmentTemperature);
            txtReactionType.Text = Convert.ToString(Properties.Reactor.Default.ReactionType);
            txtHeatingT1.Text = Convert.ToString(Properties.Reactor.Default.HeatingT1);
            txtHeatingT2.Text = Convert.ToString(Properties.Reactor.Default.HeatingT2);
            txtHeatingDelay.Text = Convert.ToString(Properties.Reactor.Default.HeatingTdelay);
            txtCoolingT3.Text = Convert.ToString(Properties.Reactor.Default.CoolingT3);
            txtCoolingDelay.Text = Convert.ToString(Properties.Reactor.Default.CoolingTdelay);
            txtCoolDown.Text = Convert.ToString(Properties.Reactor.Default.CoolDownTn);
            txtCoolDownDelay.Text = Convert.ToString(Properties.Reactor.Default.CoolDownTdelay);
            /* Gdy turbulentny */
            if (Properties.Reactor.Default.OutflowingValveType)
                comboBoxOutflowingValveType.SelectedIndex = 1;
            /* Gdy laminarny */
            else
                comboBoxOutflowingValveType.SelectedIndex = 0;
            /* Gdy stałoprocentowy */
            if (Properties.Reactor.Default.SteamValveType)
                comboBoxSteamValveType.SelectedIndex = 1;
            else
                comboBoxSteamValveType.SelectedIndex = 0;

            setText();
        }
        /// <summary>
        /// Ustawia etykiety tekstowe dla całego okna.
        /// </summary>
        void            setText()
        {
            /* Ustawienie tytułu okna */
            this.Title = Messages.msgReactorSettings;

            labelInitiaFill.Content = Messages.msgInitialFill + ":";
            labelInitialAFlow.Content = Messages.msgInitialFlow + " A:";
            labelInitialBFlow.Content = Messages.msgInitialFlow + " B:";
            labelOutflowingValveType.Content = Messages.msgOutflowingValveType + ":";
            cbiLaminar.Content = Messages.msgLaminar;
            cbiTurbulent.Content = Messages.msgTurbulent;
            labelOutflowValveValue.Content = Messages.msgOutflowValveValue + ":";
            labelSteamTemperature.Content = Messages.msgSteamTemperature + ":";
            labelCoolingWaterTemperature.Content = Messages.msgCoolingWaterTemperature + ":";
            labelMixerRatio.Content = Messages.msgMixerRatio + ":";
            labelSteamValveType.Content = Messages.msgSteamValveType + ":";
            cbiLinear.Content = Messages.msgLinear;
            cbiConstant.Content = Messages.msgConstant;
            labelEnvironmentTemperature.Content = Messages.msgEnvironmentTemperature + ":";
            labelReactionType.Content = Messages.msgReactionType + ":";
            gbHeating.Header = Messages.msgHeatingParameters + ":";
            gbCooling.Header = Messages.msgCoolingParameters + ":";
            gbCoolDown.Header = Messages.msgCoolDownParameters + ":";
            labelHeatingT1.Content = Messages.msgTimeConstant + " T1:";
            labelHeatingT2.Content = Messages.msgTimeConstant + " T2:";
            labelHeatingDelay.Content = Messages.msgDelay + ":";
            labelCoolingT3.Content = Messages.msgTimeConstant + " T3:";
            labelCoolingDelay.Content = Messages.msgDelay + ":";
            labelCoolDown.Content = Messages.msgTimeConstant + ":";
            labelCoolDownDelay.Content = Messages.msgDelay + ":";


            buttonClose.Content = Messages.msgClose;
            buttonSave.Content = Messages.msgSave;
            
        }
        /// <summary>
        /// Obsługa przycisku Zamknij.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void    buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = _bDialogResult;
            this.Close();
        }
        /// <summary>
        /// Obsługa przycisku Zapisz.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void    buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Reactor.Default.InitialPercentageFilling = Convert.ToDouble(txtInitialFill.Text);
            Properties.Reactor.Default.InitialAFlow = Convert.ToDouble(txtInitialAFlow.Text);
            Properties.Reactor.Default.InitialBFlow = Convert.ToDouble(txtInitialBFlow.Text);
            Properties.Reactor.Default.OutflowValveValue = Convert.ToDouble(txtOutflowValveValue.Text);
            Properties.Reactor.Default.SteamTemperature = Convert.ToDouble(txtSteamTemperature.Text);
            Properties.Reactor.Default.CoolingWaterTemperature = Convert.ToDouble(txtCoolingWaterTemperature.Text);
            Properties.Reactor.Default.MixerRatio = Convert.ToDouble(txtMixerRatio.Text);
            Properties.Reactor.Default.EnvironmentTemperature = Convert.ToDouble(txtEnvironmentTemperature.Text);
            Properties.Reactor.Default.ReactionType = Convert.ToDouble(txtReactionType.Text);
            Properties.Reactor.Default.HeatingT1 = Convert.ToDouble(txtHeatingT1.Text);
            Properties.Reactor.Default.HeatingT2 = Convert.ToDouble(txtHeatingT2.Text);
            Properties.Reactor.Default.HeatingTdelay = Convert.ToDouble(txtHeatingDelay.Text);
            Properties.Reactor.Default.CoolingT3 = Convert.ToDouble(txtCoolingT3.Text);
            Properties.Reactor.Default.CoolingTdelay = Convert.ToDouble(txtCoolingDelay.Text);
            Properties.Reactor.Default.CoolDownTn = Convert.ToDouble(txtCoolDown.Text);
            Properties.Reactor.Default.CoolDownTdelay = Convert.ToDouble(txtCoolDownDelay.Text);
            /* Gdy turbulentny */
            if (comboBoxOutflowingValveType.SelectedIndex == 1)
                Properties.Reactor.Default.OutflowingValveType = true;
            /* Gdy laminarny */
            else
                Properties.Reactor.Default.OutflowingValveType = false;
            /* Gdy stałoprocentowy */
            if (comboBoxSteamValveType.SelectedIndex == 1)
                Properties.Reactor.Default.SteamValveType = true;
            else
                Properties.Reactor.Default.SteamValveType = false;
            Properties.Reactor.Default.Save();
            _bDialogResult = true;
        }
       
        /// <summary>
        /// Zwracana wartość przez okno.
        /// </summary>
        bool            _bDialogResult;
    }
}
