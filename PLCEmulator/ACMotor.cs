/* Klasa ACMotor.
 * Służy do symulacji pracy silnika trófazowego.
 * 
 * Autor: Przemysław Olczak
 *        Automatyka i Robotyka
 *        Komputerowe Systemy Sterowania
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLCEmulator.ACMotor
{
    /// <summary>
    /// Klasa do symulacji pracy silnika trójfazowego.
    /// </summary>
    public class ACMotor
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public                  ACMotor()
        {
            double tmpMoveingSpeed = Properties.ACMotor.Default.MoveingSpeed;
            if (tmpMoveingSpeed > 100 || tmpMoveingSpeed <= 0)
                throw new ArgumentOutOfRangeException("Zła wartość prędkości poruszania się ciężarka");
            /* Ustawienie pozycji na środkową czyli 50[%]. */
            _dPosition = 50.0;
            /* Poczatkowa prędkość poruszanie się silnika po taśmie wyrażona w [%/s]. */
            _dMoveingSpeed = tmpMoveingSpeed;
            /* Standardowy czas próbkowania 100[ms] */
            _dTP = 0.1;
        }
        /// <summary>
        /// Uaktualnia stan krańcówek i stanu pracy "na krańcu".
        /// </summary>
        private void            CalculateLimitSwitchs()
        {
            /* Zadziałanie kranćówki prawej */
            RightLimitSwitch = (Position >= 100 ? true : false);
            /* Gdy silnik w pozycji prawej krańcowej dalej się przemieszcza */
            CrossingRightEnd = (Position > 100 ? true : false);
            if (Position > 100)
                /* Ustawienie pozycji dokładnie na 100[%] */
                Position = 100;
            /* Zadziałanie krańcówki lewej */
            LeftLimitSwitch = (Position <= 0 ? true : false);
            /* Gdy silnik w pozycji lewej krańcowej dalej się przemieszcza */
            CrossingLeftEnd = (Position < 0 ? true : false);
            if (Position < 0)
                /* Ustawienie pozycji dokładnie na 0[%] */
                Position = 0;
        }
        /// <summary>
        /// Aktualizuje pozycję silnika na taśmie.
        /// </summary>
        private void            CalculateMotorPosition()
        {
            /* Jeżeli połączenie w gwiazdę - rozruch. Poruszanie jest o pierwiastek z 3 wolniejsze.
             * i ruch w prawo */
            if (YConnection && K1Coil)
                /* Jeżeli przeciążenie to dodatkowo zmniejsz prędkość obrotów o połowę */
                Position += (Overload ? ((MoveingSpeed * TP) / Math.Sqrt(3.0)) : (MoveingSpeed * TP) / Math.Sqrt(3.0)) / 2.0;
            if (YConnection && K2Coil)
                /* Jeżeli przeciążenie to dodatkowo zmniejsz prędkość obrotów o połowę*/
                Position -= (Overload ? ((MoveingSpeed * TP) / Math.Sqrt(3.0)) : (MoveingSpeed * TP) / Math.Sqrt(3.0)) / 2.0;
            /* Jeżeli połączenie w trójkąt - rozruch. Poruszanie jest normalne.
             * i ruch w prawo */
            if (DConnection && K1Coil)
                /* Jeżeli przeciążenie to dodatkowo zmniejsz prędkość obrotów o połowę*/
                Position += (Overload ? MoveingSpeed * TP : (MoveingSpeed * TP) / 2.0);
            if (DConnection && K2Coil)
                /* Jeżeli przeciążenie to dodatkowo zmniejsz prędkość obrotów o połowę*/
                Position -= (Overload ? MoveingSpeed * TP : (MoveingSpeed * TP) / 2.0);
        }
        /// <summary>
        /// Wykonuje dyskretny krok obliczeń.
        /// Oblicza nowe położenie silnika na taśmie.
        /// </summary>
        public void             DiscretStep()
        {
            /* Sprawdzanie rodzaju połączenia */
            YConnection = (K3Coil && !K4Coil && (K1Coil && !K2Coil)) || (K3Coil && !K4Coil && (!K1Coil && K2Coil));
            DConnection = (K4Coil && !K3Coil && (K1Coil && !K2Coil)) || (K4Coil && !K3Coil && (!K1Coil && K2Coil));

            /** Sprawdzenie błędów w połączeniu **/
            /* Ruch w prawo i w lewo lub podłaczenia i gwiazdy i trójkąta */
            CircuitError = (K1Coil && K2Coil) || (K3Coil && K4Coil);

            /** Aktualizacja pozycji silnika na taśmie **/
            CalculateMotorPosition();

            /* Sprawdzenie stanu krańcówek i pracy "na krańcu" */
            CalculateLimitSwitchs();

        }

        /************************************************************
         * PROPERTIES                                               *
         ************************************************************/
        /// <summary>
        /// Odczytuje / Zmienia stan połaczenia w gwiazdę.
        /// </summary>
        public bool             YConnection
        {
            get { return _bYConnection; }
            set { _bYConnection = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan połączenia w trójkąt.
        /// </summary>
        public bool             DConnection
        {
            get { return _bDConnection; }
            set { _bDConnection = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan przycisku SR.
        /// </summary>
        public bool             StartRight
        {
            get { return _bStartRight; }
            set { _bStartRight = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan przycisku SL.
        /// </summary>
        public bool             StartLeft
        {
            get { return _bStartLeft; }
            set { _bStartLeft = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan przycisku ST.
        /// </summary>
        public bool             Stop
        {
            get { return _bStop; }
            set { _bStop = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan przycisku UB.
        /// </summary>
        public bool             Overload
        {
            get { return _bOverload; }
            set { _bOverload = value; }
        }
        /// <summary>
        /// Odczytuje stan wyłącznika krańcowego lewego.
        /// </summary>
        public bool             LeftLimitSwitch
        {
            get { return _bLeftLimitSwitch; }
            set { _bLeftLimitSwitch = value; }
        }
        /// <summary>
        /// Odczytuje stan wyłącznika krańcowego prawego.
        /// </summary>
        public bool             RightLimitSwitch
        {
            get { return _bRightLimitSwitch; }
            set { _bRightLimitSwitch = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku SK1.
        /// </summary>
        public bool             SK1Coil
        {
            get { return _bSK1Coil; }
            set { _bSK1Coil = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku SK2.
        /// </summary>
        public bool             SK2Coil
        {
            get { return _bSK2Coil; }
            set { _bSK2Coil = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku SK3.
        /// </summary>
        public bool             SK3Coil
        {
            get { return _bSK3Coil; }
            set { _bSK3Coil = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku SK4.
        /// </summary>
        public bool             SK4Coil
        {
            get { return _bSK4Coil; }
            set { _bSK4Coil = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku K1.
        /// </summary>
        public bool             K1Coil
        {
            get { return _bK1Coil; }
            set
            {
                _bK1Coil = value;
                /* Ustaw stan potwierdzenia zadziałania styku K1 */
                _bSK1Coil = value;
            }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku K2.
        /// </summary>
        public bool             K2Coil
        {
            get { return _bK2Coil; }
            set
            {
                _bK2Coil = value;
                /* Ustaw stan potwierdzenia zadziałania styku K2 */
                _bSK2Coil = value;
            }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku K3.
        /// </summary>
        public bool             K3Coil
        {
            get { return _bK3Coil; }
            set
            {
                _bK3Coil = value;
                /* Ustaw stan potwierdzenia zadziałania styku K3 */
                _bSK3Coil = value;
            }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan styku K4.
        /// </summary>
        public bool             K4Coil
        {
            get { return _bK4Coil; }
            set
            {
                _bK4Coil = value;
                /* Ustaw stan potwierdzenia zadziałania styku K4 */
                _bSK4Coil = value;
            }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan lampki "obroty w lewo".
        /// </summary>
        public bool             LampLeft
        {
            get { return _bLampLeft; }
            set { _bLampLeft = value; }
        }
        /// <summary>
        /// Odczytuje / Zmienia stan lampki "obroty w prawo".
        /// </summary>
        public bool             LampRight
        {
            get { return _bLampRight; }
            set { _bLampRight = value; }
        }
        /// <summary>
        /// Odczytuje / zmienia pozycję.
        /// </summary>
        public double           Position
        {
            get { return _dPosition; }
            set { _dPosition = value; }
        }
        /// <summary>
        /// Odczytuje / zmienia prędkość poruszania się silnika po taśmie.
        /// </summary>
        public double           MoveingSpeed
        {
            get { return _dMoveingSpeed; }
            set { _dMoveingSpeed = value; }
        }
        /// <summary>
        /// Odczytuje / zmienia czas próbkowania dla tego obiektu.
        /// </summary>
        public double           TP
        {
            get { return _dTP; }
            set { _dTP = value; }
        }
        /// <summary>
        /// Odczytuje / zmienia pracę silnika na prawym krańcu.
        /// </summary>
        public bool             CrossingRightEnd
        {
            get { return _bCrossingRightEnd; }
            set { _bCrossingRightEnd = value; }
        }
        /// <summary>
        /// Odczytuje / zmienia pracę silnika na prawym krańcu.
        /// </summary>
        public bool             CrossingLeftEnd
        {
            get { return _bCrossingLeftEnd; }
            set { _bCrossingLeftEnd = value; }
        }
        /// <summary>
        /// Odczytuje / zmienia informację o błędach połączenia obwodu.
        /// </summary>
        public bool             CircuitError
        {
            get { return _bCircuitError; }
            set { _bCircuitError = value; }
        }
        /************************************************************
         * FIELDS                                                   *
         ************************************************************/
        
        /// <summary>
        /// Oznacza że aktywne jest połączenie w gwiazdę.
        /// </summary>
        bool                    _bYConnection;
        /// <summary>
        /// Oznacza że aktywne jest połączenie w trójkąt.
        /// </summary>
        bool                    _bDConnection;
        /// <summary>
        /// Stan przycisku: rozruch silnika - kierunek obrotów w prawo.
        /// </summary>
        bool                    _bStartRight;
        /// <summary>
        /// Stan przycisku: rozruch silnika - kierunek obrotów w lewo.
        /// </summary>
        bool                    _bStartLeft;
        /// <summary>
        /// Stan przycisku: zatrzymanie silnika.
        /// </summary>
        bool                    _bStop = true;
        /// <summary>
        /// Stan przycisku: przeciązenie.
        /// </summary>
        bool                    _bOverload = true;
        /// <summary>
        /// Stan wyłącznika krańcowego lewego.
        /// </summary>
        bool                    _bLeftLimitSwitch;
        /// <summary>
        /// Stan wyłącznika krańcowego prawego.
        /// </summary>
        bool                    _bRightLimitSwitch;
        /// <summary>
        /// Stan styku K1 - obroty w prawo.
        /// </summary>
        bool                    _bSK1Coil;
        /// <summary>
        /// Stan styku K2 - obroty w lewo.
        /// </summary>
        bool                    _bSK2Coil;
        /// <summary>
        /// Stan styku K3 - praca przy połączeniu uzwojeń w gwiazdę.
        /// </summary>
        bool                    _bSK3Coil;
        /// <summary>
        /// Stan styku K4 - praca przy połączeniu uzwojeń w trójkąt.
        /// </summary>
        bool                    _bSK4Coil;
        /// <summary>
        /// Stan cewki K1.
        /// </summary>
        bool                    _bK1Coil;
        /// <summary>
        /// Stan cewki K2.
        /// </summary>
        bool                    _bK2Coil;
        /// <summary>
        /// Stan cewki K3.
        /// </summary>
        bool                    _bK3Coil;
        /// <summary>
        /// Stan cewki K4.
        /// </summary>
        bool                    _bK4Coil;
        /// <summary>
        /// Stan lampki lewo.
        /// </summary>
        bool                    _bLampLeft;
        /// <summary>
        /// Stan lampki prawo.
        /// </summary>
        bool                    _bLampRight;
        /// <summary>
        /// Aktualna pozycja sinika na taśmociągu wyrażona w %.
        /// </summary>
        double                  _dPosition;
        /// <summary>
        /// Prędkośc poruszania się silnika po taśmie.
        /// Ilościowy przyrost procentów po upływie 1s.
        /// </summary>
        double                  _dMoveingSpeed;
        /// <summary>
        /// Czas próbkowania.
        /// </summary>
        double                  _dTP;
        /// <summary>
        /// Oznacza że silnik dojechał do prawego końca i dalej jest wysterowany do ruchu w prawo.
        /// </summary>
        bool                    _bCrossingRightEnd;
        /// <summary>
        /// Oznacza że silnik dojechał do lewego końca i dalej jest wysterowany do ruchu w lewo.
        /// </summary>
        bool                    _bCrossingLeftEnd;
        /// <summary>
        /// Oznajmia o błędnym połączeniu obwodu.
        /// </summary>
        bool                    _bCircuitError;
    }
}
