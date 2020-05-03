/* Klasa Reaktor.
 * Służy do symulacji obiektu reaktora.
 * Krok symulacji odbywa się po przez wywołanie metody DiscretStep().
 * 
 * Autor: Przemysław Olczak
 *        Automatyka i Robotyka
 *        Komputerowe Systemy Sterowania
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLCEmulator.Reactor
{
    /// <summary>
    /// Klasa do symulacji obiektu Reaktora.
    /// </summary>
    public class Reaktor
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/
        /// <summary>
        /// Konstruktor.
        /// </summary>
        public          Reaktor()
        {
            SetContainerParameters();
            SetReactorParameters();
            SetHeatingDynamicsParameters();
            SetCoolingDynamicsParameters();
            SetCoolDownDynamicsParameters();
        }

        /// <summary>
        /// Wykonanie jednego kroku obliczeń.
        /// </summary>
        public void     DiscretStep()
        {
            /* Obliczenie aktualnego poziomu oraz nowej wartości temperatury */
            CalculateContainerFilling();
            /* Obliczenie współczynnika skalowania dla stałych czasowych grzania i chłodzenia */
            _dTimeConstantScaling = CalculateTimeConstantsScaling();
            /* Obliczenie przyrostu temperatury od chłodzenia */
            CalculateCoolingTemperature();
            /* Obliczenie przyrostu temperatury od grzania */
            CalculateHeatingTemperature();
            /* Obliczanie temperatury od stygnięcia */
            CalculateCoolDownTemperature();
        }

        /// <summary>
        /// Oblicza współczynnik skalowania stałych czasowych.
        /// </summary>
        /// <returns>Wartość współczynnika skalowania stałych czasowych.</returns>
        private double  CalculateTimeConstantsScaling()
        {
            /* Od poziomu: */
            double tmpFromLevel = ((90.0 / 100.0) * _dContainerPercentageFilling + 10.0) / 100.0;
            /* Od mieszadła: */
            double tmpFromMixer = 1.0;
            /* Jeżeli mieszadło włączone */
            if (_bMixerEnable)
                tmpFromMixer = _dMixerRatio;
            return (tmpFromLevel * tmpFromMixer);
        }

        /// <summary>
        /// Oblicza wypełnienie zbiornika dla danego kroku symulacji.
        /// </summary>
        private void    CalculateContainerFilling()
        {
            double deltaA = (_dAValveFlow * Convert.ToByte(_bAValveEnabled)) * _dSampleTime,
                   deltaB = (_dBValveFlow * Convert.ToByte(_bBValveEnabled)) * _dSampleTime;
            /* Gdy przewiduje przepełnienie */
            if ((_dContainerPercentageFilling + deltaA + deltaB) > 100.0)
            {
                /* Wystawiam informację o przepełnieniu */
                _bOverflow = true;
                /* Jeżeli wczesniejsze wypełnienie było 100%, to przyrost wypełnienia wynosi 0 */
                if (_dContainerPercentageFilling == 100.0)
                {
                    deltaA = 0.0;
                    deltaB = 0.0;
                }
                /* Jeżeli mniejszy od 100% */
                else
                {
                    /* To dolewam tyle aby wypełnienie ostateczne wyniosło 100% */
                    while ((_dContainerPercentageFilling + deltaA + deltaB) > 100.0)
                    {
                        deltaA -= 0.01;
                        deltaB -= 0.01;
                    }
                }
            }
            /* Gdy nie będzie przepełnienia */
            else
            {
                _bOverflow = false;
                /* Obliczenie przyrostu temperatury od dolanych czynników */
                CalculateExoEndoThermicReactionTemperature(deltaA, deltaB);
            }
            /* Obliczanie wypełnienia zbiornika */
            _dMediumAPercentage += deltaA;
            _dMediumBPercentage += deltaB;
            _dContainerPercentageFilling = _dMediumAPercentage + _dMediumBPercentage;
            if (_dContainerPercentageFilling > 0.0)
            {
                if (_bOutflowingValveType)
                {
                    /* Dla zaworu odpływowego turbulentnego */
                    _dMediumAPercentage -= ((((_dOutFlowingValveOpening / 100.0) * Math.Sqrt(_dContainerPercentageFilling / 100.0)) * _dOutFlowValveValue) * _dSampleTime) * (_dMediumAPercentage / _dContainerPercentageFilling);
                    _dMediumBPercentage -= ((((_dOutFlowingValveOpening / 100.0) * Math.Sqrt(_dContainerPercentageFilling / 100.0)) * _dOutFlowValveValue) * _dSampleTime) * (_dMediumBPercentage / _dContainerPercentageFilling);
                    _dMediumNotReacted -= ((((_dOutFlowingValveOpening / 100.0) * Math.Sqrt(_dContainerPercentageFilling / 100.0)) * _dOutFlowValveValue) * _dSampleTime) * (_dMediumNotReacted / _dContainerPercentageFilling);
                }
                else
                {
                    /* Dla zaworu odpływowego laminarnego */
                    _dMediumAPercentage -= ((((_dOutFlowingValveOpening / 100.0) * (_dContainerPercentageFilling / 100.0)) * _dOutFlowValveValue) * _dSampleTime) * (_dMediumAPercentage / _dContainerPercentageFilling);
                    _dMediumBPercentage -= ((((_dOutFlowingValveOpening / 100.0) * (_dContainerPercentageFilling / 100.0)) * _dOutFlowValveValue) * _dSampleTime) * (_dMediumBPercentage / _dContainerPercentageFilling);
                    _dMediumNotReacted -= ((((_dOutFlowingValveOpening / 100.0) * (_dContainerPercentageFilling / 100.0)) * _dOutFlowValveValue) * _dSampleTime) * (_dMediumNotReacted / _dContainerPercentageFilling);
                }
            }
            if (_dMediumAPercentage < 0.0)
                _dMediumAPercentage = 0.0;
            if (_dMediumBPercentage < 0.0)
                _dMediumBPercentage = 0.0;
            _dContainerPercentageFilling = _dMediumAPercentage + _dMediumBPercentage;
            /* Gdy poziom poniżej zera */
            if (_dContainerPercentageFilling < 0.0)
                _dContainerPercentageFilling = 0.0;
        }
        /// <summary>
        /// Obliczenie wypadkowej temperatury pochodzacej od dynamiki stygnięcia.
        /// </summary>
        private void    CalculateCoolDownTemperature()
        {
            /* Skalowanie stałych czasowych */
            _TCoolDownDynamics.ChangeTimeConstant(_dTimeConstantScaling);
            /* Wykonanie kroku obliczeń */
            double tmp = (_TCoolDownDynamics.DiscretStep(_dEnvironmentTemperature, _dReactorTemperature));
            /* Obliczenie wartości przyrostu temperatury oddawanej do otoczenia gdy brak chłodzenia i brak grzania */
            if ((_dHeatingValveValue == 0.0 || _bHeatingOutputValveEnabled == false) && (_bCoolingValveEnabled == false || _bCoolingOutputValveEnabled == false))
                _dReactorTemperature += tmp;
        }
        /// <summary>
        /// Obliczenie przyrostu temperatury pochodzacej od dynamiki chłodzenia.
        /// </summary>
        private void    CalculateCoolingTemperature()
        {
            /* Skalowanie stałych czasowych */
            _TCoolingDynamics.ChangeTimeConstant(_dTimeConstantScaling);
            /* Wykonanie kroku obliczeń */
            double tmp = _TCoolingDynamics.DiscretStep(_dCoolingWaterTemperature, _dReactorTemperature);
            /* Obliczanie nowej temperatury reaktora, jeżeli otwary zawór wejściowy i otwary zawór wyjściowy chłodzenia 
             * to dadaj przyrost temperatury od chłodzenia. */
            if ((_bCoolingValveEnabled && _bCoolingOutputValveEnabled) && !(_dHeatingValveValue > 0.0 && _bHeatingOutputValveEnabled))
                _dReactorTemperature += tmp;
        }
        /// <summary>
        /// Oblicza aktualną temperaturę czynników w zbiorniku na podstawie aktualnych ilości czynników A i B
        /// oraz reakcji między nimi zachodzącej.
        /// </summary>
        /// <param name="a_dV1i">Ilość składnika A aktualnie dolana do reaktora.</param>
        /// <param name="a_dV2i">Ilość składnika B aktualnie dolana do reaktora.</param>
        private void    CalculateExoEndoThermicReactionTemperature(double a_dV1i, double a_dV2i)
        {
            /// Wywołać ją przed okresleniem nowych wartości ilości A i B.
            double deltaV = a_dV2i - a_dV1i,
                   Yd,
                   deltaVRi;
            Yd = 2.0 * Math.Min(a_dV1i, a_dV2i) * (20.0 + _dReactionType);
            if ((_dMediumNotReacted * deltaV) >= 0.0)
                Yd += Math.Abs(deltaV) * 20.0 + (_dMediumAPercentage + _dMediumBPercentage) * _dReactorTemperature;
            else
            {
                if (Math.Abs(deltaV) >= Math.Abs(_dMediumNotReacted))
                    Yd += (Math.Abs(deltaV) - Math.Abs(_dMediumNotReacted)) * 20.0;
                deltaVRi = Math.Min(Math.Abs(deltaV), Math.Abs(_dMediumNotReacted));
                Yd += deltaVRi * (_dReactorTemperature + _dReactionType) + deltaVRi * (20.0 + _dReactionType) + ((_dMediumAPercentage + _dMediumBPercentage) - deltaVRi) * _dReactorTemperature;
            }
            /* Obliczenie nowej temperetury */
            if (((_dMediumAPercentage + _dMediumBPercentage) + a_dV1i + a_dV2i) == 0.0)
                _dReactorTemperature = _dEnvironmentTemperature;
            else
                _dReactorTemperature += Yd / ((_dMediumAPercentage + _dMediumBPercentage) + a_dV1i + a_dV2i) - _dReactorTemperature;
            _dMediumNotReacted += deltaV;
        }
        /// <summary>
        /// Obliczenie wypadkowej temperatury pochodzacej od dynamiki grzania.
        /// </summary>
        private void    CalculateHeatingTemperature()
        {
            double tmpTemp = 0.0;
            /* Zmiana stałych czasowych grzania */
            _THeatingDynamics.ChangeTimeConstant(_dTimeConstantScaling);
            if (_bSteamValveType)
                /* Dla zaworu stałoprocentowego */
                tmpTemp = Math.Exp(2.3026 * ((_dHeatingValveValue / 100.0) - 1.0)) * _dSteamTemperature;
            else
                /* Dla zaworu liniowego */
                tmpTemp = (_dHeatingValveValue / 100.0) * (_dSteamTemperature - 20.0) + 20.0;
            /* Sprawdzam czy jednocześnie jest włączony tor chłodzenia */
            if (_bCoolingValveEnabled && _bCoolingOutputValveEnabled)
                /* Jeśli tak to odejmuje od temperatury pary, temperaturę wody */
                tmpTemp -= _dCoolingWaterTemperature;
            /* Wykonanie kroku obliczeń */
            tmpTemp = _THeatingDynamics.DiscretStep(tmpTemp, _dReactorTemperature);
            /* Jeżeli otwarty zawór wejściowy i otwarty zawór wyjściowy grzania to dodaj przyrost temperatury */
            if (_dHeatingValveValue > 0.0 && _bHeatingOutputValveEnabled)
                /* Obliczanie temperatury od grzania */
                _dReactorTemperature += tmpTemp;           
        }
        /// <summary>
        /// Zmienia / ustawia parametry reaktora. Parametry są czytane z pliku konfiguracyjnego: Reaktor.settings.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość wypełnienia zbiornika.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość przepływu dla zaworu A.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość przepływu dla zaworu B.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość przepływu dla zaworu odpływowego.</exception>
        private void    SetContainerParameters()
        {
            /* Tymczasowe zmienne. Ich wartości są pobierane z pliku */
            double InitialPercentageFilling = Properties.Reactor.Default.InitialPercentageFilling;
            double InitialAFlow = Properties.Reactor.Default.InitialAFlow;
            double InitialBFlow = Properties.Reactor.Default.InitialBFlow;
            double OutflowValveValue = Properties.Reactor.Default.OutflowValveValue;
            double tmpEnTemp = Properties.Reactor.Default.EnvironmentTemperature;
            bool OutflowingValveType = Properties.Reactor.Default.OutflowingValveType;
            /* Sprawdzanie poprawności danych */
            if (InitialPercentageFilling > 100.0 || InitialPercentageFilling < 0.0)
                throw new ArgumentOutOfRangeException("InitialPercentageFilling");
            if (InitialAFlow > 50.0 || InitialAFlow < 0.1)
                throw new ArgumentOutOfRangeException("InitialAFlow");
            if (InitialBFlow > 50.0 || InitialBFlow < 0.1)
                throw new ArgumentOutOfRangeException("InitialBFlow");
            if (OutflowValveValue > 100.0 || OutflowValveValue < 0.2)
                throw new ArgumentOutOfRangeException("OutflowValveValue");
            if (tmpEnTemp > 50.0 || tmpEnTemp < 5.0)
                throw new ArgumentOutOfRangeException("EnvironmentTemperature");
            /* Zapis poprawnych wartości */
            _dContainerPercentageFilling = InitialPercentageFilling;
            /* Połowa poczatkowego wypełnienia to składnik A */
            _dMediumAPercentage = InitialPercentageFilling / 2.0;
            /* Druga połowa poczatkowego wypełnienia to składnik B */
            _dMediumBPercentage = InitialPercentageFilling / 2.0;
            _bOutflowingValveType = OutflowingValveType;
            _dAValveFlow = InitialAFlow;
            _dBValveFlow = InitialBFlow;
            _dOutFlowValveValue = OutflowValveValue;
            _dEnvironmentTemperature = tmpEnTemp;
            _dReactorTemperature = _dEnvironmentTemperature;
        }
        /// <summary>
        /// Ustawienie parametrów dla dynamiki stygnięcia.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość stałej czasowej Tn dla wymiany temperatury z otoczeniem.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość opóźnienia dla wymiany temperatury z otoczeniem.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość temperatury otoczenia.</exception>
        private void    SetCoolDownDynamicsParameters()
        {
            /* Zmienne tymczasowe. Czytane z pliku konfiguracyjnego  */
            double tmpTn = Properties.Reactor.Default.CoolDownTn;
            double tmpTdelay = Properties.Reactor.Default.CoolDownTdelay;
            /* Sprawdzanie poprawności danych */
            if (tmpTn > 1200.0 || tmpTn < 60.0)
                throw new ArgumentOutOfRangeException("CoolDownTn");
            if (tmpTdelay < 1.0)
                throw new ArgumentOutOfRangeException("CoolDownTdelay");
            /* Tworzenie obiektu dla dynamiki grzania */
            List<double> tmpB = new List<double>();
            tmpB.Add(1.0);
            List<double> tmpA = new List<double>();
            tmpA.Add(1.0);
            tmpA.Add(tmpTn);
            _TCoolDownDynamics = new PLCEmulator.Tustin(tmpB, tmpA, tmpTdelay, _dSampleTime);
            _TCoolDownDynamics.SetInitialCondition(_dEnvironmentTemperature, _dEnvironmentTemperature);
        }
        /// <summary>
        /// Ustawienie parametrów dla dynamiki chłodzenia.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość stałej czasowej T3 dla chłodzenia.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość opóźnienia dla chłodzenia.</exception>
        private void    SetCoolingDynamicsParameters()
        {
            /* Zmienne tymczasowe. Czytane z pliku konfiguracyjnego  */
            double tmpT3 = Properties.Reactor.Default.CoolingT3;
            double tmpTdelay = Properties.Reactor.Default.CoolingTdelay;
            /* Sprawdzanie poprawności danych */
            if (tmpT3 > 600.0 || tmpT3 < 5.0)
                throw new ArgumentOutOfRangeException("CoolingT3");
            if (tmpTdelay < 1.0)
                throw new ArgumentOutOfRangeException("CoolingTdelay");
            /* Tworzenie obiektu dla dynamiki grzania */
            List<double> tmpB = new List<double>();
            tmpB.Add(1.0);
            List<double> tmpA = new List<double>();
            tmpA.Add(1.0);
            tmpA.Add(tmpT3);
            _TCoolingDynamics = new PLCEmulator.Tustin(tmpB, tmpA, tmpTdelay, _dSampleTime);
            _TCoolingDynamics.SetInitialCondition(_dEnvironmentTemperature, _dEnvironmentTemperature);
        }
        /// <summary>
        /// Ustawienie parametrów dla dynamiki grzania.
        /// </summary>
        /// <exception cref="System.Exception">W pliku konfracyjnym zła warość stałej czasowej T1 dla grzania.</exception>
        /// <exception cref="System.Exception">W pliku konfracyjnym zła warość stałej czasowej T2 dla grzania.</exception>
        /// <exception cref="System.Exception">W pliku konfracyjnym stałe czasowe T1 i T2 mają tą samą wartość.</exception>
        /// <exception cref="System.Exception">W pliku konfracyjnym zła warość opóźnienia dla grzania.</exception>
        private void    SetHeatingDynamicsParameters()
        {
            /* Zmienne tymczasowe. Czytane z pliku konfiguracyjnego */
            double tmpT1 = Properties.Reactor.Default.HeatingT1;
            double tmpT2 = Properties.Reactor.Default.HeatingT2;
            double tmpTdelay = Properties.Reactor.Default.HeatingTdelay;
            /* Sprawdzanie poprawności danych */
            if (tmpT1 > 600.0 || tmpT1 < 1.0)
                throw new ArgumentOutOfRangeException("HeatingT1");
            if (tmpT2 > 600.0 || (tmpT2 < 1.0 && tmpT2 != 0.0))
                throw new ArgumentOutOfRangeException("HeatingT2");
            if (tmpT1 == tmpT2)
                throw new ArgumentOutOfRangeException("HeatingT1 same as HeatingT2");
            if (tmpTdelay < 1.0)
                throw new ArgumentOutOfRangeException("HeatingTdelay");
            /* Tworzenie obiektu dla dynamiki grzania */
            List<double> tmpB = new List<double>();
            tmpB.Add(1.0);
            List<double> tmpA = new List<double>();
            if (tmpT2 != 0.0)
            {
                tmpA.Add(1.0);
                tmpA.Add(tmpT1 + tmpT2);
                tmpA.Add(tmpT1 * tmpT2);
            }
            if (tmpT2 == 0.0)
            {
                tmpA.Add(1.0);
                tmpA.Add(tmpT1);
            }
            _THeatingDynamics = new PLCEmulator.Tustin(tmpB, tmpA, tmpTdelay, _dSampleTime);
            _THeatingDynamics.SetInitialCondition(_dEnvironmentTemperature, _dEnvironmentTemperature);
        }
        /// <summary>
        /// Ustawia parametry reaktora. Parametry są czytane z pliku konfiguracyjnego: Reaktor.settings.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość temperatury pary.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość temperatury wody chłodzącej.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">W pliku konfracyjnym zła warość współczynnika mieszania.</exception>
        private void    SetReactorParameters()
        {
            /* Zmienne tymczasowe. Pobierane z pliku. */
            double SteamTemperature = Properties.Reactor.Default.SteamTemperature;
            double CoolingWaterTemperature = Properties.Reactor.Default.CoolingWaterTemperature;
            double MixerRatio = Properties.Reactor.Default.MixerRatio;
            double ReactionType = Properties.Reactor.Default.ReactionType;
            bool SteamValveType = Properties.Reactor.Default.SteamValveType;
            /* Sprawdzanie poprawności danych */
            if (SteamTemperature > 220.0 || SteamTemperature < 190.0)
                throw new ArgumentOutOfRangeException("SteamTemperature");
            if (CoolingWaterTemperature > 5.0 || CoolingWaterTemperature < 1.0)
                throw new ArgumentOutOfRangeException("CoolingWaterTemperature");
            if (MixerRatio > 1.0 || MixerRatio < 0.2)
                throw new ArgumentOutOfRangeException("MixerRatio");
            /* Zapis poprawnych wartości */
            _dSteamTemperature = SteamTemperature;
            _dCoolingWaterTemperature = CoolingWaterTemperature;
            _dMixerRatio = MixerRatio;
            _bSteamValveType = SteamValveType;
            _dReactionType = ReactionType;
        }
        
        /************************************************************
         * PROPERTIES                                               *
         ************************************************************/
      
        /// <summary>
        /// Włącza / wyłącza zawór A. 0 - wyłączony, 1 - włączony.
        /// </summary>
        public bool     AValveEnabled
        {
            get { return _bAValveEnabled; }
            set { _bAValveEnabled = value; }
        }
        /// <summary>
        /// Włącza / wyłącza zawór B. 0 - wyłączony, 1 - włączony.
        /// </summary>
        public bool     BValveEnabled
        {
            get { return _bBValveEnabled; }
            set { _bBValveEnabled = value; }
        }
        /// <summary>
        /// Włącza / wyłącza zawór chłodzący wyjściowy. true - otwarty, false - zamknięty.
        /// </summary>
        public bool     CoolingOutputValveEnabled
        {
            get { return _bCoolingOutputValveEnabled; }
            set { _bCoolingOutputValveEnabled = value; }
        }
        /// <summary>
        /// Włącza / wyłącza zawór chłodzący wejściowy. true - otwarty, false - zamknięty.
        /// </summary>
        public bool     CoolingValveEnabled
        {
            get { return _bCoolingValveEnabled; }
            set
            {
                if (_bCoolingValveEnabled != value)
                {
                    _bCoolingValveEnabled = value;
                    _TCoolingDynamics.SetInitialCondition(_dReactorTemperature, _dReactorTemperature);
                }
            }
        }
        /// <summary>
        /// Włącza / wyłącza zawór grzejący wyjściowy. true - otwarty, false - zamknięty.
        /// </summary>
        public bool     HeatingOutputValveEnabled
        {
            get { return _bHeatingOutputValveEnabled; }
            set { _bHeatingOutputValveEnabled = value; }
        }
        /// <summary>
        /// Włącza / wyłącza mieszadło. 0 - wyłączone, 1 - włączone.
        /// </summary>
        public bool     MixerEnable
        {
            get { return _bMixerEnable; }
            set { _bMixerEnable = value; }
        }
        /// <summary>
        /// Zwraca informację o przepełnieniu.
        /// False - brak przepełnienia.
        /// True - Nastąpiło przepełnienie reaktora.
        /// </summary>
        public bool     Overflow
        {
            get { return _bOverflow; }
        }

        /// <summary>
        /// Zmienia wartość wypełnienia zbiornika. Zakres wartości [0.0; 100.0] %.
        /// </summary>
        public double   ContainerPercentageFilling
        {
            get { return _dContainerPercentageFilling; }
            set
            {
                if (value <= 100.0 && value >= 0.0)
                    _dContainerPercentageFilling = value;
            }
        }
        /// <summary>
        /// Ustawia wartość otwarcia zaworu wejściowego dla grzania. Zakres wartości [0.0; 100.0] %.
        /// </summary>
        public double   HeatingValveValue
        {
            get { return _dHeatingValveValue; }
            set
            {
                if (value <= 100.0 && value >= 0.0)
                {
                    _dHeatingValveValue = value;
                }
            }
        }
        /// <summary>
        /// Zmienia wartość otwarcia zaworu odpływowego. Zakres wartości [0.0; 100.0] %.
        /// </summary>
        public double   OutFlowingValveOpening
        {
            get { return _dOutFlowingValveOpening; }
            set
            {
                if (value <= 100.0 && value >= 0.0)
                    _dOutFlowingValveOpening = value;
            }
        }
        /// <summary>
        /// Zmienia wartość przepływu maksymalnego zaworu odpływowego. Zakres wartości [0.2; 100.0] m^3/s.
        /// </summary>
        public double   OutFlowValveValue
        {
            get { return _dOutFlowValveValue; }
            set
            {
                if (value <= 100.0 && value >= 0.2)
                    _dOutFlowValveValue = value;
            }
        }
        /// <summary>
        /// Zwraca temperaturę cieczy w reaktorze.
        /// </summary>
        public double   ReactorTemperature
        {
            get { return _dReactorTemperature; }
        }

        /************************************************************
         * FIELDS                                                   *
         ************************************************************/

        /// <summary>
        /// Określa czy zawór A jest włączony czy wyłączony.
        /// </summary>
        private bool    _bAValveEnabled;
        /// <summary>
        /// Określa czy zawór B jest włączony czy wyłączony.
        /// </summary>
        private bool    _bBValveEnabled;
        /// <summary>
        /// Określa stan zaworu na wujściu toru chłodzenia. True - otwarty, False - zamknięty.
        /// </summary>
        private bool    _bCoolingOutputValveEnabled;
        /// <summary>
        /// Określa czy zawór dla medium chłodząceg jest zamknięty czy otwarty. False - zamknięty, true - otwarty.
        /// </summary>
        private bool    _bCoolingValveEnabled;
        /// <summary>
        /// Określa stan zaworu na wujściu toru grzania. True - otwarty, False - zamknięty.
        /// </summary>
        private bool    _bHeatingOutputValveEnabled;
        /// <summary>
        /// Informacja o włączonym/wyłączonym mieszadle.
        /// </summary>
        private bool    _bMixerEnable;
        /// <summary>
        /// Rodzaj zaworu odpływowego. false - liniowy, true - nieliniowy.
        /// </summary>
        private bool    _bOutflowingValveType;
        /// <summary>
        /// Przepełnienie zbiornika. false - poziom w zbiorniku zawiera się pomiędzy 0 a 100 %. True - poziom w zbiorniku przekroczył 100%. Odpowiednia metoda (CalculateContainerFilling) zapewnia nasycenie dla poziomu.
        /// </summary>
        private bool    _bOverflow;
        /// <summary>
        /// Rodzaj zaworu na dopływie pary grzejnej. false - liniowy, true - stałoprocentowy.
        /// </summary>
        private bool    _bSteamValveType;

        /// <summary>
        /// Przepływ dla czynnika A. Zakres wartości: [0.1; 50] m^3/s.
        /// </summary>
        private double  _dAValveFlow;
        /// <summary>
        /// Przepływ dla czynnika B. Zakres wartości: [0.1; 50] m^3/s.
        /// </summary>
        private double  _dBValveFlow;
        /// <summary>
        /// Aktualne wypełnienie zbiornika w % (poziom cieczy w zbiorniku). Zakres wartości: [0; 100] %.
        /// </summary>
        private double  _dContainerPercentageFilling;
        /// <summary>
        /// Temperatura płynu chłodzącego. Nominalnie 5[oC].
        /// </summary>
        private double  _dCoolingWaterTemperature;
        /// <summary>
        /// Temperatura otoczenia [oC].
        /// </summary>
        private double  _dEnvironmentTemperature;
        /// <summary>
        /// Otwarcie zaworu dla pary grzejącej. Zakres wartości [0.0; 100.0] %.
        /// </summary>
        private double  _dHeatingValveValue;
        /// <summary>
        /// Procent zawartości medium A w reaktorze [%].
        /// </summary>
        private double  _dMediumAPercentage;
        /// <summary>
        /// Procent zawartości medium B w reaktorze [%].
        /// </summary>
        private double  _dMediumBPercentage;
        /// <summary>
        /// Procentowa ilość czynnika który nie wszedł w reakcję.
        /// Wartość większa od 0 oznacza nadmiar czynnika B,
        /// mniejsza od 0 nadmiar czynnika A,
        /// 0 oznacza stan równowagi czynników.
        /// </summary>
        private double  _dMediumNotReacted;
        /// <summary>
        /// Współczynnik skalowania stałych czasowych dla włączonego mieszadła. Zakres wartości: [20; 100] %.
        /// </summary>
        private double  _dMixerRatio;
        /// <summary>
        /// Określa jak bardzo jest otwarty zawór odpływowy. Zakres wartości [0.0; 100.0] %.
        /// </summary>
        private double  _dOutFlowingValveOpening;
        /// <summary>
        /// Określa maksymalny przepływ zaworu odpływowego. Zakres wartości [0.2; 100.0] m^3/s.
        /// </summary>
        private double  _dOutFlowValveValue;
        /// <summary>
        /// Określa rodzaj reakcji.
        /// Wartość ujemna oznacza reakcję endotermiczną,
        /// dodatnia reakcję egzotermiczną,
        /// zerowa oznacza brak reakcji między czynnikami A i B.
        /// </summary>
        private double  _dReactionType;
        /// <summary>
        /// Aktualna temperatura medium w reaktorze [oC].
        /// </summary>
        private double  _dReactorTemperature;
        /// <summary>
        /// Czas próbkowania obiektu. Nominalnie 0.1 [s].
        /// </summary>
        private double  _dSampleTime = 0.1;
        /// <summary>
        /// Temperatura pary grzejnej. Nominalnie 220[oC].
        /// </summary>
        private double  _dSteamTemperature;
        /// <summary>
        /// Współczynnik skalowania stałych czasowych reakcji.
        /// </summary>
        private double  _dTimeConstantScaling;

        /// <summary>
        /// Obiekt dla dynamiki stygnięcia.
        /// </summary>
        private         Tustin _TCoolDownDynamics;
        /// <summary>
        /// Obiekt dla dynamiki chłodzenia.
        /// </summary>
        private         Tustin _TCoolingDynamics;
        /// <summary>
        /// Obiekt dla dynamiki grzania.
        /// </summary>
        private         Tustin _THeatingDynamics;
    }
}
