/* Klasa Sygnalizacja.
 * Służy do symulacji.
 * Krok symulacji odbywa się po przez wywołanie metody DiscretStep().
 * 
 * Autor: Karol Kojzar
 *        Automatyka i Robotyka
 *        Praca inżynierska
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLCEmulator.Sygnalizacja
{
    class Sygnalizacja
    {
        /// <summary>
        /// Włącza / wyłącza, 0 - wyłączony, 1 - włączony.
        /// </summary>
        public bool Sygna1
        {
            get { return _syg1; }
            set { _syg1 = value; }
        }
        public bool Sygna2
        {
            get { return _syg2; }
            set { _syg2 = value; }
        }
        public bool Sygna3
        {
            get { return _syg3; }
            set { _syg3 = value; }
        }
        public bool Sygna4
        {
            get { return _syg4; }
            set { _syg4 = value; }
        }
        public bool Sygna5
        {
            get { return _syg5; }
            set { _syg5 = value; }
        }
        public bool Sygna6
        {
            get { return _syg6; }
            set { _syg6 = value; }
        }
        public bool Sygna7
        {
            get { return _syg7; }
            set { _syg7 = value; }
        }
        public bool Sygna8
        {
            get { return _syg8; }
            set { _syg8 = value; }
        }
        public bool pojazd1
        {
            get { return _bPojazd1; }
            set { _bPojazd1 = value; }
        }
        public bool pojazd2
        {
            get { return _bPojazd2; }
            set { _bPojazd2 = value; }
        }
        public bool pojazd3
        {
            get { return _bPojazd3; }
            set { _bPojazd3 = value; }
        }
        public bool pojazd4
        {
            get { return _bPojazd4; }
            set { _bPojazd4 = value; }
        }
        public bool pojazd5
        {
            get { return _bPojazd5; }
            set { _bPojazd5 = value; }
        }
        public bool pojazd6
        {
            get { return _bPojazd6; }
            set { _bPojazd6 = value; }
        }
        public bool awaria
        {
            get { return _awaria; }
            set { _awaria = value; }
        }

        /************************************************************
         * FIELDS                                                   *
        ************************************************************/

        /// <summary>
        /// Określa stan Sygnalizacji 1.
        /// </summary>
        private bool _syg1;
        /// <summary>
        /// Określa stan Sygnalizacji 2.
        /// </summary>
        private bool _syg2;
        /// <summary>
        /// Określa stan Sygnalizacji 3.
        /// </summary>
        private bool _syg3;
        /// <summary>
        /// Określa stan Sygnalizacji 4.
        /// </summary>
        private bool _syg4;
        /// <summary>
        /// Określa stan Sygnalizacji 5.
        /// </summary>
        private bool _syg5;
        /// <summary>
        /// Określa stan Sygnalizacji 6.
        /// </summary>
        private bool _syg6;
        /// <summary>
        /// Określa stan Sygnalizacji 7.
        /// </summary>
        private bool _syg7;
        /// <summary>
        /// Określa stan Sygnalizacji 8.
        /// </summary>
        private bool _syg8;
        /// <summary>
        /// Określa stan przycisku awaria.
        /// </summary>
        private bool _awaria;
        /// <summary>
        /// Stan przycisku: pojazd1.
        /// </summary>
        bool _bPojazd1;
        /// <summary>
        /// Stan przycisku: pojazd2.
        /// </summary>
        bool _bPojazd2;
        /// <summary>
        /// Stan przycisku: pojazd3.
        /// </summary>
        bool _bPojazd3;
        /// <summary>
        /// Stan przycisku: pojazd4.
        /// </summary>
        bool _bPojazd4;
        /// <summary>
        /// Stan przycisku: pojazd5.
        /// </summary>
        bool _bPojazd5;
        /// <summary>
        /// Stan przycisku: pojazd6.
        /// </summary>
        bool _bPojazd6;
    }
}
