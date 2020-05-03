/* Okno ACMotorSettings.
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

namespace PLCEmulator.ACMotor
{
    /// <summary>
    /// Interaction logic for ACMotorSettings.xaml
    /// </summary>
    public partial class ACMotorSettings : Window
    {
        public              ACMotorSettings()
        {
            InitializeComponent();
            txtBoxMoveingSpeed.Text = Convert.ToString(Properties.ACMotor.Default.MoveingSpeed);
            /* Ustawienie etykiet tekstowych */
            setText();
        }
        /// <summary>
        /// Ustawia etykiety tekstowe dla całego okna.
        /// </summary>
        void                setText()
        {
            /* Ustawienie tytułu okna */
            this.Title = Messages.msgACmotorSettings;
            labelMoveingSpeed.Content = Messages.msgMoveingSpeed + ":";
            buttonClose.Content = Messages.msgClose;
            buttonSave.Content = Messages.msgSave;
        }
        /// <summary>
        /// Obsługa przycisku Zamknij.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void        buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Obsługa przycisku Zapisz.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void        buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.ACMotor.Default.MoveingSpeed = Convert.ToDouble(txtBoxMoveingSpeed.Text);
            Properties.ACMotor.Default.Save();
        }
    }
}
