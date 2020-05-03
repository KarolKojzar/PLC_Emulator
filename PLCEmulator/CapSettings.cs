/* Klasa CapSettings.
 * Służy do przechowywania ustawień dla wyświetlanych etykiet tekstowych.
 * 
 * Autor: Przemysław Olczak
 *        Automatyka i Robotyka
 *        Komputerowe Systemy Sterowania
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLCEmulator
{
    class CapSettings
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="a_sinput">Ciąg tekstu do konwersji na wartości.</param>
        public          CapSettings(string a_sinput)
        {
            string[] tmpIn = a_sinput.Split(' ');
            Name = tmpIn[0];
            AISymbol = tmpIn[1];
            AIStartValue = Convert.ToInt16(tmpIn[2]);
            AIStartValueBit = Convert.ToInt16(tmpIn[3]);
            AIBit = Convert.ToInt16(tmpIn[4]);
            AOSymbol = tmpIn[5];
            AOStartValue = Convert.ToInt16(tmpIn[6]);
            AOStartValueBit = Convert.ToInt16(tmpIn[7]);
            AOBit = Convert.ToInt16(tmpIn[8]);
            DISymbol = tmpIn[9];
            DIStartValue = Convert.ToInt16(tmpIn[10]);
            DIStartValueBit = Convert.ToInt16(tmpIn[11]);
            DIBit = Convert.ToInt16(tmpIn[12]);
            DOSymbol = tmpIn[13];
            DOStartValue = Convert.ToInt16(tmpIn[14]);
            DOStartValueBit = Convert.ToInt16(tmpIn[15]);
            DOBit = Convert.ToInt16(tmpIn[16]);
        }
        /// <summary>
        /// Tworzy string do zpisania do pliku.
        /// </summary>
        /// <returns>String do zapisu do pliku.</returns>
        public string   getOutput()
        {
            return (Name + " " + AISymbol + " " + AIStartValue + " " + AIStartValueBit + " " + AIBit + " " + AOSymbol + " " + AOStartValue + " " + AOStartValueBit + " " + AOBit + " " + DISymbol + " " + DIStartValue + " " + DIStartValueBit + " " + DIBit + " " + DOSymbol + " " + DOStartValue + " " + DOStartValueBit + " " + DOBit); 
        }

        /************************************************************
         * PROPERTIES                                               *
         ************************************************************/

        public string   Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string   AISymbol
        {
            get { return _AISymbol; }
            set { _AISymbol = value; }
        }
        public int      AIStartValue
        {
            get { return _AIStartValue; }
            set { _AIStartValue = value; }
        }
        public int      AIStartValueBit
        {
            get { return _AIStartValueBit; }
            set { _AIStartValueBit = value; }
        }
        public int      AIBit
        {
            get { return _AIBit; }
            set { _AIBit = value; }
        }
        public string   AOSymbol
        {
            get { return _AOSymbol; }
            set { _AOSymbol = value; }
        }
        public int      AOStartValue
        {
            get { return _AOStartValue; }
            set { _AOStartValue = value; }
        }
        public int      AOStartValueBit
        {
            get { return _AOStartValueBit; }
            set { _AOStartValueBit = value; }
        }
        public int      AOBit
        {
            get { return _AOBit; }
            set { _AOBit = value; }
        }
        public string   DISymbol
        {
            get { return _DISymbol; }
            set { _DISymbol = value; }
        }
        public int      DIStartValue
        {
            get { return _DIStartValue; }
            set { _DIStartValue = value; }
        }
        public int      DIStartValueBit
        {
            get { return _DIStartValueBit; }
            set { _DIStartValueBit = value; }
        }
        public int      DIBit
        {
            get { return _DIBit; }
            set { _DIBit = value; }
        }
        public string   DOSymbol
        {
            get { return _DOSymbol; }
            set { _DOSymbol = value; }
        }
        public int      DOStartValue
        {
            get { return _DOStartValue; }
            set { _DOStartValue = value; }
        }
        public int      DOStartValueBit
        {
            get { return _DOStartValueBit; }
            set { _DOStartValueBit = value; }
        }
        public int      DOBit
        {
            get { return _DOBit; }
            set { _DOBit = value; }
        }

        /************************************************************
         * FIELDS                                                   *
         ************************************************************/

        string          _Name;
        string          _AISymbol;
        int             _AIStartValue;
        int             _AIStartValueBit;
        int             _AIBit;
        string          _AOSymbol;
        int             _AOStartValue;
        int             _AOStartValueBit;
        int             _AOBit;
        string          _DISymbol;
        int             _DIStartValue;
        int             _DIStartValueBit;
        int             _DIBit;
        string          _DOSymbol;
        int             _DOStartValue;
        int             _DOStartValueBit;
        int             _DOBit;
    }
}
