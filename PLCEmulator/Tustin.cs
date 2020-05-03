/* Klasa Tustin.
 * Służy do zamiany obiektów ciągłych danych transmitancją K(s) na obiekty dyskretne dane transmitancją K(z).
 * 
 * Autor: Przemysław Olczak
 *        Automatyka i Robotyka
 *        Komputerowe Systemy Sterowania
 */
using System.Collections.Generic;

namespace PLCEmulator
{
    /// <summary>
    /// Klasa będąca implementacją transformacji Tustina.
    /// Na postawie parametrów obiektu ciągłego o transmitancji K(s) otrzymujemy parametry obiektu o transmitancji K(z^-1).
    /// </summary>
    public class Tustin
    {
        /************************************************************
         * METHODS                                                  *
         ************************************************************/
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="a_B">Parametry wielomianu B(s). B = [b0 b1 ...]. </param>
        /// <param name="a_A">Parametry wielomianu A(s). A = [a0 a1 ...].</param>
        /// <param name="a_Tp">Okres próbkowania.</param>
        /// <param name="a_Delay">Opóźnienie obiektu.</param>
        public                  Tustin(List<double> a_B, List<double> a_A, double a_Delay, double a_Tp)
        {
            _dTp = a_Tp;
            _Delay = (int)(a_Delay / a_Tp);
            if (a_A.Count == 2)
            {
                _arrB = CopyParameters(a_B, 2);
                _arrA = CopyParameters(a_A, 2);
            }
            else if (a_A.Count == 3)
            {
                _arrB = CopyParameters(a_B, 3);
                _arrA = CopyParameters(a_A, 3);
            }
            ChangeTimeConstant(1.0);
            SetPastValues();
        }

        /// <summary>
        /// Oblicza aktualną wartość wyjścia obiektu dyskretnego na podstawie próbki wejścia obiektu.
        /// </summary>
        /// <param name="a_dInputSample">Próbka wejścia obiektu.</param>
        /// <param name="a_dLastOutput">Próbka wyjścia obiektu.</param>
        /// <returns>Wartość wyjścia obiektu dyskretnego.</returns>
        public double           DiscretStep(double a_dInputSample, double a_dLastOutput)
        {
            double sum = 0.0;
            /* Aktualizuje wektor _arrY, na potrzeby KLasy Reaktor odkomentować poniższe.*/
            UpdateArray(a_dLastOutput, ref _arrY);
            /* Aktualizuje wektor _arrU */
            UpdateArray(a_dInputSample, ref _arrU);
            /* Obliczam wyrażenie B(z^-1)*U */
            for (int i = 0; i < _arrDiscreteB.Count; i++)
                sum += _arrU[i] * _arrDiscreteB[i];
            /* Obliczam wyrażenie A(z^-1)*Y */
            for (int i = 0; i < _arrDiscreteA.Count - 1; i++)
                sum += _arrY[i] * _arrDiscreteA[i] * (-1.0);
            /* Aktualizuje wektor _arrY, na potrzeby ogólne odkomentować poniższe*/
            // UpdateArray(sum, ref _arrY);
            /* Zwracam przyrost */
            return (sum - a_dLastOutput);
        }
        /// <summary>
        /// Zwraca okres próbkowania.
        /// </summary>
        /// <returns>Okres próbkowania.</returns>
        public double           GetTp()
        {
            return _dTp;
        }
        /// <summary>
        /// Zwraca parametry wielomianu A(z).
        /// </summary>
        /// <returns>Parametry wielomianu A(z).</returns>
        public List<double>     GetDiscreteA()
        {
            return _arrDiscreteA;
        }
        /// <summary>
        /// Zwraca parametry wielomianu A(z).
        /// </summary>
        /// <returns>Parametry wielomianu A(z).</returns>
        public List<double>     GetDiscreteB()
        {
            return _arrDiscreteB;
        }
        /// <summary>
        /// Słuzy do zmiany/skalowania stałych czasowych obiektów inercyjnych 1 lub 2 rzędu.
        /// Metoda tylko dla obiektów będących inercjami 1 lub 2 rzedu.
        /// Postać obiektów: K(s) = 1/[(T1*T2)s^2 + (T1+T2)s + 1].
        /// Dla pozostałych obiektów ustawiac parametr a_dScaler = 1.0.
        /// </summary>
        /// <param name="a_dScaler">
        /// Liczba dla skalowania stałych czasowych obiektów.
        /// Dla inercji 1 rzędu K(s) = 1/(1+sT1): T1_new = T1 * a_dScaler.
        /// Dla inercji 2 rzędu K(s) = 1/[(T1*T2)s^2 + (T1+T2)s + 1]: (T1*T2)_new = (T1*T2) * a_dScaler^2, (T1+T2)_new = (T1+T2) * a_dScaler.
        /// </param>
        public void             ChangeTimeConstant(double a_dScaler)
        {
            if (_arrA.Count == 2)
            {
                _arrDiscreteB = ContinuousToDiscrete2(_arrB, 1.0);
                _arrDiscreteA = ContinuousToDiscrete2(_arrA, a_dScaler);
                Divide();
            }
            else if (_arrA.Count == 3)
            {
                _arrDiscreteB = ContinuousToDiscrete3(_arrB, 1.0);
                _arrDiscreteA = ContinuousToDiscrete3(_arrA, a_dScaler);
                Divide();
            }
        }
        /// <summary>
        /// Ustawienie niezerowych warunków poczatkowych.
        /// </summary>
        /// <param name="a_dPastValue">Przeszła wartość wyjścia.</param>
        /// <param name="a_dPastValueY">Poprzednie wartości Y.</param>
        /// <param name="a_dPastValueU">Poprzednie wartości U.</param>
        public void             SetInitialCondition(double a_dPastValueY, double a_dPastValueU)
        {
            for (int i = 0; i < _arrY.Count; i++)
                _arrY[i] = a_dPastValueY;
            for (int i = 0; i < _arrU.Count; i++)
                _arrU[i] = a_dPastValueU;
        }
        /// <summary>
        /// Ustawienie niezerowych warunków poczatkowych.
        /// </summary>
        /// <param name="a_dPastValue">Przeszła wartość wyjścia.</param>
        /// <param name="a_dPastValueY">Poprzednie wartości Y.</param>
        public void             SetPastOutputValues(double a_dPastValueY)
        {
            for (int i = 0; i < _arrY.Count; i++)
                _arrY[i] = a_dPastValueY;
        }
        /// <summary>
        /// Tworzy dyskretny wielomian zmiennej 'z' z wielomianu zmiennej 's'.
        /// Dla wielomianów stopnia 2.
        /// </summary>
        /// <param name="a_arrIn">
        /// Wejściowy wielomian zmiennej 's'.
        /// Postaci: [[a0][a1]]. Zerowy element tablicy to argument zerowy itd.
        /// </param>
        /// <param name="a_dScaler">
        /// Słuzy do zmiany/skalowania stałych czasowych obiektów inercyjnych 1 lub 2 rzędu.
        /// Postać obiektów: K(s) = 1/[(T1*T2)s^2 + (T1+T2)s + 1].
        /// Dla pozostałych obiektów ustawiac parametr a_dScaler = 1.0.
        /// </param>
        /// <returns>
        /// Wieloimian zmiennej 'z'.
        /// Postaci: [[a0][a1]]. Zerowy element tablicy odpowiada zerowemu argumentowi wielomianu zmiennej 'z'.
        /// </returns>
        private List<double>    ContinuousToDiscrete2(List<double> a_arrIn, double a_dScaler)
        {
            List<double> tmp = new List<double>(2);
            tmp.Add(-2.0 * a_arrIn[1] * a_dScaler + a_arrIn[0] * _dTp);
            tmp.Add(a_arrIn[1] * 2.0 * a_dScaler + a_arrIn[0] * _dTp);
            return tmp;
        }
        /// <summary>
        /// Tworzy dyskretny wielomian zmiennej 'z' z wielomianu zmiennej 's'.
        /// Dla wielomianów stopnia 3.
        /// </summary>
        /// <param name="a_arrIn">
        /// Wejściowy wielomian zmiennej 's'.
        /// Postaci: [[a0][a1][a2]]. Zerowy element tablicy to argument zerowy itd.
        /// </param>
        /// <param name="a_dScaler">
        /// Słuzy do zmiany/skalowania stałych czasowych obiektów inercyjnych 1 lub 2 rzędu.
        /// Postać obiektów: K(s) = 1/[(T1*T2)s^2 + (T1+T2)s + 1].
        /// Dla pozostałych obiektów ustawiac parametr a_dScaler = 1.
        /// </param>
        /// <returns>
        /// Wieloimian zmiennej 'z'.
        /// Postaci: [[a0][a1][a2]]. Zerowy element tablicy odpowiada zerowemu argumentowi wielomianu zmiennej 'z'.
        /// </returns>
        private List<double>    ContinuousToDiscrete3(List<double> a_arrIn, double a_dScaler)
        {
            List<double> tmp = new List<double>(3);
            tmp.Add(a_arrIn[2] * 4.0 * a_dScaler * a_dScaler - a_arrIn[1] * _dTp * 2.0 * a_dScaler + a_arrIn[0] * _dTp * _dTp);
            tmp.Add(a_arrIn[2] * (-8.0) * a_dScaler * a_dScaler + a_arrIn[0] * 2.0 * _dTp * _dTp);
            tmp.Add(a_arrIn[2] * 4.0 * a_dScaler * a_dScaler + a_arrIn[1] * _dTp * 2.0 * a_dScaler + a_arrIn[0] * _dTp * _dTp);
            return tmp;
        }
        /// <summary>
        /// Zamienia paramentry wielomianów na postać wymaganą do dalszych obliczeń w programie.
        /// </summary>
        /// <param name="a_arrParamIn">Parmetry wielomianu.</param>
        /// <param name="a_nParamNo">Liczba parametrów wielomianu.</param>
        /// <returns>Parametry wymagane przy obliczaniu wartości wyjścia obiektu.</returns>
        private List<double>    CopyParameters(List<double> a_arrParamIn, int a_nParamNo)
        {
            List<double> tmp = new List<double>();
            if (a_nParamNo == 3)
            {
                tmp.AddRange(new double[] { 0, 0, 0 });
                for (int i = 0; i < a_arrParamIn.Count; i++)
                    tmp[i] = a_arrParamIn[i];
            }
            else if (a_nParamNo == 2)
            {
                tmp.AddRange(new double[] { 0, 0 });
                for (int i = 0; i < a_arrParamIn.Count; i++)
                    tmp[i] = a_arrParamIn[i];
            }
            return tmp;
        }
        /// <summary>
        /// Dzieli parametry wielomianów A(z) i B(z). Tworzy moniczny wielomian A(z).
        /// </summary>
        /// <example>dfgdfg</example>
        private void            Divide()
        {
            double tmp = _arrDiscreteA[_arrDiscreteA.Count - 1];
            for (int i = 0; i < _arrDiscreteA.Count; i++)
                _arrDiscreteA[i] /= tmp;
            for (int i = 0; i < _arrDiscreteB.Count; i++)
                _arrDiscreteB[i] /= tmp;
        }
        /// <summary>
        /// Tworzy wektory U i Y zawierające zerowe warunki początkowe obiektu dyskretnego.
        /// </summary>
        private void            SetPastValues()
        {
            /* Zerowe warunki początkowe dla wektorów U i Y */
            _arrU.AddRange(new double[_arrDiscreteB.Count + _Delay]);
            _arrY.AddRange(new double[_arrDiscreteA.Count - 1]);
        }
        /// <summary>
        /// Aktualizuje wartości wektorów U i Y zawierających poprzenie wartości wejść i wyjść układu dyskretnego.
        /// </summary>
        private void            UpdateArray(double a_dNewData, ref List<double> a_arrOldArray)
        {
            List<double> tmp = new List<double>();
            tmp.AddRange(a_arrOldArray);
            a_arrOldArray.Clear();
            for (int i = 0; i < tmp.Count - 1; i++)
                a_arrOldArray.Add(tmp[i + 1]);
            a_arrOldArray.Add(a_dNewData);
        }

        /************************************************************
         * FIELDS                                                   *
         ************************************************************/

        /// <summary>
        /// Okres próbkowania.
        /// </summary>
        private double          _dTp;
        /// <summary>
        /// Opóźnienie obiektu dyskrertnego.
        /// </summary>
        private int             _Delay;
        /// <summary>
        /// Parametry wielomianu A(s).
        /// </summary>
        private List<double>    _arrA = new List<double>();
        /// <summary>
        /// Parametry wielomianu B(s).
        /// </summary>
        private List<double>    _arrB = new List<double>();
        /// <summary>
        /// Parametry wielomianu A(z).
        /// </summary>
        private List<double>    _arrDiscreteA = new List<double>();
        /// <summary>
        /// Parametry wielomianu B(z).
        /// </summary>
        private List<double>    _arrDiscreteB = new List<double>();
        /// <summary>
        /// Wczesniejsze wartości wejścia.
        /// </summary>
        private List<double>    _arrU = new List<double>();
        /// <summary>
        /// Wcześniejsze wartości wyjścia.
        /// </summary>
        private List<double>    _arrY = new List<double>();
        
        
    }
}
