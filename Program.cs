//algorytm genetyczny 2 | Jacek Domeracki | numer albumu: 173518

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zadanie_2
{
    public class Osobnik
    {
        public byte[] param_1;
        public byte[] param_2;
        public byte[] param_3;
        public double wart_fun_przyst = 0;

        public Osobnik(int ile_chrom)
        {
            param_1 = new byte[ile_chrom];
            param_2 = new byte[ile_chrom];
            param_3 = new byte[ile_chrom];
        }

        private void Losuj_parametr(byte[] param)
        {
            Random random = new Random();
            for (int i = 0; i < param.Length; i++)
            {
                param[i] = (byte)random.Next(2);
            }
        }

        public void Losuj_wszystkie_parametry()
        {
            Losuj_parametr(param_1);
            Losuj_parametr(param_2);
            Losuj_parametr(param_3);
        }

        private int Konwersja_bin2dec_param(byte[] param)
        {
            int param_dec = 0;
            for (int i = 0; i < param.Length; i++)
            {
                param_dec += param[i] * (int)Math.Pow(2, i);            //LSB ... MSB
            }
            return param_dec;
        }

        private double Funkcja_fx(double x, double pa, double pb, double pc)
        {
            return pa * Math.Sin(pb * x + pc);
        }

        private double Funkcja_przystosowania(Dictionary<int, double> przedzial_dyskretny, Dictionary<double, double> probki_funkcji_fx)
        {
            double wart_fun_przyst_rob = 0;
            foreach (var probka in probki_funkcji_fx)
            {
                wart_fun_przyst_rob += Math.Pow(probka.Value - Funkcja_fx(probka.Key, przedzial_dyskretny[Konwersja_bin2dec_param(param_1)],
                    przedzial_dyskretny[Konwersja_bin2dec_param(param_2)], przedzial_dyskretny[Konwersja_bin2dec_param(param_3)]), 2);
            }
            return Math.Round(wart_fun_przyst_rob, 6);
        }

        public void Oblicz_funkcje_przystosowania(Dictionary<int, double> przedzial_dyskretny, Dictionary<double, double> probki_funkcji_fx)
        {
            wart_fun_przyst = Funkcja_przystosowania(przedzial_dyskretny, probki_funkcji_fx);
        }

        public void SkopiujZ(Osobnik osob_do_skopiowania, int ile_chrom)
        {
            for (int i = 0; i < ile_chrom; i++)
            {
                this.param_1[i] = osob_do_skopiowania.param_1[i];
                this.param_2[i] = osob_do_skopiowania.param_2[i];
                this.param_3[i] = osob_do_skopiowania.param_3[i];
            }
            this.wart_fun_przyst = osob_do_skopiowania.wart_fun_przyst;
        }

        private void Pokaz_parametr(byte[] param)
        {
            for (int i = param.Length - 1; i >= 0; i--)
            {
                Console.Write("{0,3}", param[i]);               //MSB ... LSB
            }
        }

        public void Pokaz_wszystkie_parametry(Dictionary<int, double> przedzial_dyskretny)
        {
            Pokaz_parametr(param_1);
            Console.Write("  |");
            Pokaz_parametr(param_2);
            Console.Write("  |");
            Pokaz_parametr(param_3);
            Console.Write("  |");
            Console.Write("{0,5}", Konwersja_bin2dec_param(param_1));
            Console.Write("  |");
            Console.Write("{0,5}", Konwersja_bin2dec_param(param_2));
            Console.Write("  |");
            Console.Write("{0,5}", Konwersja_bin2dec_param(param_3));
            Console.Write("  |");
            Console.Write("{0,8:F2}", przedzial_dyskretny[Konwersja_bin2dec_param(param_1)]);
            Console.Write("  |");
            Console.Write("{0,8:F2}", przedzial_dyskretny[Konwersja_bin2dec_param(param_2)]);
            Console.Write("  |");
            Console.Write("{0,8:F2}", przedzial_dyskretny[Konwersja_bin2dec_param(param_3)]);
            Console.Write("  |");
            Console.Write("{0,12:F6}", wart_fun_przyst);
            Console.WriteLine("  |");
        }
    }

    internal class Program
    {
        const double PRZEDZ_MIN = 0;
        const double PRZEDZ_MAX = 3;
        //const int ILE_PARAM = 3;
        const int ILE_CHROM_NP = 6;
        const int ILE_OSOB = 13;
        const int ILE_OS_TUR = 3;
        const int ILE_ITERACJE = 100;

        static void Dyskretyzacja_przedzialu(Dictionary<int, double> przedzial_dyskretny, double przedz_min, double przedz_max, int ile_chrom)
        {
            int dyskretny_max = (int)Math.Pow(2, ile_chrom) - 1;
            double delta = Math.Round((przedz_max - przedz_min) / dyskretny_max, 4);        //dokładność zaokrąglenia ma znaczenie
            przedzial_dyskretny.Add(0, przedz_min);
            przedzial_dyskretny.Add(dyskretny_max, przedz_max);
            for (int i = 1; i < dyskretny_max; i++)
            {
                przedzial_dyskretny.Add(i, przedz_min + i * delta);
            }
        }

        static void Operator_selekcji_hot_deck(Osobnik[] pula_osobnikow, ref Osobnik osob_hot_deck, int ile_chrom)
        {
            Osobnik osob_hot_deck_rob;
            osob_hot_deck_rob = pula_osobnikow[0];
            for (int i = 1; i < pula_osobnikow.Length; i++)
            {
                if (osob_hot_deck_rob.wart_fun_przyst > pula_osobnikow[i].wart_fun_przyst) osob_hot_deck_rob = pula_osobnikow[i];
            }
            osob_hot_deck.SkopiujZ(osob_hot_deck_rob, ile_chrom);
        }

        static void Operator_selekcji_turniejowej(Osobnik[] pula_osobnikow, ref Osobnik osob_zwyc_turnieju, int ile_osob_tur, int ile_chrom)
        {
            List<int> indeksy_puli = new List<int>();
            for (int i = 0; i < pula_osobnikow.Length; i++)
            {
                indeksy_puli.Add(i);
            }
            Random random = new Random();
            int i_ip = random.Next(indeksy_puli.Count);
            int n_osob = indeksy_puli[i_ip];
            indeksy_puli.Remove(n_osob);
            int n_osob_rywal;
            for (int i = 0; i < ile_osob_tur - 1; i++)
            {
                i_ip = random.Next(indeksy_puli.Count);
                n_osob_rywal = indeksy_puli[i_ip];
                indeksy_puli.Remove(n_osob_rywal);
                if (pula_osobnikow[n_osob].wart_fun_przyst > pula_osobnikow[n_osob_rywal].wart_fun_przyst) n_osob = n_osob_rywal;
            }
            osob_zwyc_turnieju.SkopiujZ(pula_osobnikow[n_osob], ile_chrom);
        }

        static void Operator_mutowanie(ref Osobnik osob_zmutowany, int ile_chrom, Dictionary<int, double> przedzial_dyskretny, Dictionary<double, double> probki_funkcji_fx)
        {
            Random random = new Random();
            int n_par = random.Next(1, 4);
            int n_bit = random.Next(ile_chrom);
            switch (n_par)
            {
                case 1:
                    osob_zmutowany.param_1[n_bit] = (byte)(1 - osob_zmutowany.param_1[n_bit]);
                    break;
                case 2:
                    osob_zmutowany.param_2[n_bit] = (byte)(1 - osob_zmutowany.param_2[n_bit]);
                    break;
                case 3:
                    osob_zmutowany.param_3[n_bit] = (byte)(1 - osob_zmutowany.param_3[n_bit]);
                    break;
            }
            osob_zmutowany.Oblicz_funkcje_przystosowania(przedzial_dyskretny, probki_funkcji_fx);
        }

        static void Operator_krzyzowanie(ref Osobnik osob_krzyzow_1, ref Osobnik osob_krzyzow_2, int ile_chrom, Dictionary<int, double> przedzial_dyskretny, Dictionary<double, double> probki_funkcji_fx)
        {
            byte rob;
            for (int i = 0; i < ile_chrom; i++)           //miejsce cięcia: param_2|param_3
            {
                //rob = osob_krzyzow_1.param_2[i];
                //osob_krzyzow_1.param_2[i] = osob_krzyzow_2.param_2[i];
                //osob_krzyzow_2.param_2[i] = rob;

                rob = osob_krzyzow_1.param_3[i];
                osob_krzyzow_1.param_3[i] = osob_krzyzow_2.param_3[i];
                osob_krzyzow_2.param_3[i] = rob;
            }
            osob_krzyzow_1.Oblicz_funkcje_przystosowania(przedzial_dyskretny, probki_funkcji_fx);
            osob_krzyzow_2.Oblicz_funkcje_przystosowania(przedzial_dyskretny, probki_funkcji_fx);
        }

        static double Najlepsza_wart_fun_przyst(Osobnik[] pula_osobnikow)
        {
            double wart_fun_przyst = pula_osobnikow[0].wart_fun_przyst;
            for (int i = 1; i < pula_osobnikow.Length; i++)
            {
                if (wart_fun_przyst > pula_osobnikow[i].wart_fun_przyst) wart_fun_przyst = pula_osobnikow[i].wart_fun_przyst;
            }
            return wart_fun_przyst;
        }

        static double Srednia_wart_fun_przyst(Osobnik[] pula_osobnikow)
        {
            double sum_wart_fun_przyst = 0;
            for (int i = 0; i < pula_osobnikow.Length; i++)
            {
                sum_wart_fun_przyst += pula_osobnikow[i].wart_fun_przyst;
            }
            return Math.Round(sum_wart_fun_przyst / pula_osobnikow.Length, 6);
        }

        static void TEST_1<T>(string nazwa, Dictionary<T, double> Przedzial_dyskretny)       //< dziedzina funkcji, double>
        {
            Console.WriteLine("(TEST) " + nazwa);
            foreach (var p_dyskr in Przedzial_dyskretny.OrderBy(x => x.Key))
            {
                Console.WriteLine("{0,5}  -  {1,8:F6}", p_dyskr.Key, p_dyskr.Value);
            }
            Console.WriteLine();
        }

        static void TEST_2(string nazwa, Osobnik[] pula_osobnikow, Dictionary<int, double> Przedzial_dyskretny)
        {
            Console.WriteLine("(TEST) " + nazwa);
            for (int i = 0; i < pula_osobnikow.Length; i++)
            {
                Console.Write("{0,3}|", i + 1);
                pula_osobnikow[i].Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            }
            Console.WriteLine();
        }

        static void TEST_3(string nazwa, Osobnik[] pula_osobnikow, ref Osobnik osobnik_rob_1, ref Osobnik osobnik_rob_2, Dictionary<int, double> Przedzial_dyskretny, Dictionary<double, double> Probki_funkcji_fx)
        {
            Console.WriteLine("(TEST) " + nazwa);

            Operator_selekcji_hot_deck(pula_osobnikow, ref osobnik_rob_1, ILE_CHROM_NP);
            Console.WriteLine("Hot Deck :");
            osobnik_rob_1.Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            Console.WriteLine();

            Operator_selekcji_turniejowej(pula_osobnikow, ref osobnik_rob_1, ILE_OS_TUR, ILE_CHROM_NP);
            Console.WriteLine("Turniej :");
            osobnik_rob_1.Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            Console.WriteLine();

            osobnik_rob_1.SkopiujZ(pula_osobnikow[ILE_OSOB - 3], ILE_CHROM_NP);
            Operator_mutowanie(ref osobnik_rob_1, ILE_CHROM_NP, Przedzial_dyskretny, Probki_funkcji_fx);
            Console.WriteLine("Zmutowanie :");
            osobnik_rob_1.Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            Console.WriteLine("Przed mutacją :");
            pula_osobnikow[ILE_OSOB - 3].Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            Console.WriteLine();

            osobnik_rob_1.SkopiujZ(pula_osobnikow[ILE_OSOB - 2], ILE_CHROM_NP);
            osobnik_rob_2.SkopiujZ(pula_osobnikow[ILE_OSOB - 1], ILE_CHROM_NP);
            Operator_krzyzowanie(ref osobnik_rob_1, ref osobnik_rob_2, ILE_CHROM_NP, Przedzial_dyskretny, Probki_funkcji_fx);
            Console.WriteLine("Skrzyżowane :");
            osobnik_rob_1.Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            osobnik_rob_2.Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            Console.WriteLine("Przed skrzyżowaniem :");
            pula_osobnikow[ILE_OSOB - 2].Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            pula_osobnikow[ILE_OSOB - 1].Pokaz_wszystkie_parametry(Przedzial_dyskretny);
            Console.WriteLine();
        }

        static void Main()
        {
            Console.WriteLine("ALGORYTM GENETYCZNY ( ZADANIE 2 )");
            Console.WriteLine();

            Dictionary<int, double> Przedzial_dyskretny = new Dictionary<int, double>();
            Dyskretyzacja_przedzialu(Przedzial_dyskretny, PRZEDZ_MIN, PRZEDZ_MAX, ILE_CHROM_NP);
            //TEST_1("Dyskretyzacja:", Przedzial_dyskretny);

            Dictionary<double, double> Probki_funkcji_fx = new Dictionary<double, double>
            {
                { -1.00000 ,  0.59554 }, { -0.80000 ,  0.58813 }, { -0.60000 ,  0.64181 }, { -0.40000 ,  0.68587 },
                { -0.20000 ,  0.44783 }, {  0.00000 ,  0.40836 }, {  0.20000 ,  0.38241 }, {  0.40000 , -0.05933 },
                {  0.60000 , -0.12478 }, {  0.80000 , -0.36847 }, {  1.00000 , -0.39935 }, {  1.20000 , -0.50881 },
                {  1.40000 , -0.63435 }, {  1.60000 , -0.59979 }, {  1.80000 , -0.64107 }, {  2.00000 , -0.51808 },
                {  2.20000 , -0.38127 }, {  2.40000 , -0.12349 }, {  2.60000 , -0.09624 }, {  2.80000 ,  0.27893 },
                {  3.00000 ,  0.48965 }, {  3.20000 ,  0.33089 }, {  3.40000 ,  0.70615 }, {  3.60000 ,  0.53342 },
                {  3.80000 ,  0.43321 }, {  4.00000 ,  0.64790 }, {  4.20000 ,  0.48834 }, {  4.40000 ,  0.18440 },
                {  4.60000 , -0.02389 }, {  4.80000 , -0.10261 }, {  5.00000 , -0.33594 }, {  5.20000 , -0.35101 },
                {  5.40000 , -0.62027 }, {  5.60000 , -0.55719 }, {  5.80000 , -0.66377 }, {  6.00000 , -0.62740 }
            };
            //TEST_1("Próbki funkcji fx:", Probki_funkcji_fx);

            Osobnik[] pula_osobnikow = new Osobnik[ILE_OSOB];
            Osobnik[] nowa_pula_osobnikow = new Osobnik[ILE_OSOB];
            Osobnik[] pula_osobnikow_rob;
            Osobnik osobnik_rob_1 = new Osobnik(ILE_CHROM_NP);
            Osobnik osobnik_rob_2 = new Osobnik(ILE_CHROM_NP);

            for (int i = 0; i < ILE_OSOB; i++)
            {
                pula_osobnikow[i] = new Osobnik(ILE_CHROM_NP);
                pula_osobnikow[i].Losuj_wszystkie_parametry();
                pula_osobnikow[i].Oblicz_funkcje_przystosowania(Przedzial_dyskretny, Probki_funkcji_fx);

                nowa_pula_osobnikow[i] = new Osobnik(ILE_CHROM_NP);
            }
            //TEST_2("Pula osobników:", pula_osobnikow, Przedzial_dyskretny);

            //TEST_3("Operatory genetyczne:", pula_osobnikow, ref osobnik_rob_1, ref osobnik_rob_2, Przedzial_dyskretny, Probki_funkcji_fx);

            Console.WriteLine("-->  START");
            Console.WriteLine("Najlepsza wartość funkcji przystosowania :{0,10:F6}", Najlepsza_wart_fun_przyst(pula_osobnikow));
            Console.WriteLine("  Średnia wartość funkcji przystosowania :{0,10:F6}", Srednia_wart_fun_przyst(pula_osobnikow));
            Console.WriteLine();

            for (int i = 0; i < ILE_ITERACJE; i++)
            {
                for (int j = 0; j < ILE_OSOB / 2; j++)          //z puli osobników bierzemy po 2 kolejne osobniki
                {
                    Operator_selekcji_turniejowej(pula_osobnikow, ref osobnik_rob_1, ILE_OS_TUR, ILE_CHROM_NP);         //zwycięzca pierwszego turnieju
                    Operator_selekcji_turniejowej(pula_osobnikow, ref osobnik_rob_2, ILE_OS_TUR, ILE_CHROM_NP);         //zwycięzca drugiego turnieju

                    if (j / 2 == 0 || j / 2 == 2)           //zerowa lub druga czwórka osobników
                    {
                        //Console.WriteLine("--------|TEST|  KRZYŻOWANIE :{0,3}  --{1,3}", j, j / 2);
                        Operator_krzyzowanie(ref osobnik_rob_1, ref osobnik_rob_2, ILE_CHROM_NP, Przedzial_dyskretny, Probki_funkcji_fx);          //skrzyżowanie pierwszego i drugiego osobnika
                    }
                    if (j / 2 == 1 || j / 2 == 2)           //pierwsza lub druga czwórka osobników
                    {
                        //Console.WriteLine("--------|TEST|      MUTACJA :{0,3}  --{1,3}", j, j / 2);
                        Operator_mutowanie(ref osobnik_rob_1, ILE_CHROM_NP, Przedzial_dyskretny, Probki_funkcji_fx);           //zmutowanie pierwszego osobnika
                        Operator_mutowanie(ref osobnik_rob_2, ILE_CHROM_NP, Przedzial_dyskretny, Probki_funkcji_fx);           //zmutowanie drugiego osobnika
                    }

                    nowa_pula_osobnikow[j * 2].SkopiujZ(osobnik_rob_1, ILE_CHROM_NP);
                    nowa_pula_osobnikow[j * 2 + 1].SkopiujZ(osobnik_rob_2, ILE_CHROM_NP);
                }
                if (ILE_OSOB % 2 == 1)          //tylko gdy nieparzysta liczba osobników w puli
                {
                    Operator_selekcji_hot_deck(pula_osobnikow, ref osobnik_rob_1, ILE_CHROM_NP);            //najlepszy osobnik w puli
                    nowa_pula_osobnikow[ILE_OSOB - 1].SkopiujZ(osobnik_rob_1, ILE_CHROM_NP);
                }
                Console.WriteLine("-->  ITERACJA NR :{0,3}", i + 1);
                Console.WriteLine("Najlepsza wartość funkcji przystosowania :{0,10:F6}", Najlepsza_wart_fun_przyst(nowa_pula_osobnikow));
                Console.WriteLine("  Średnia wartość funkcji przystosowania :{0,10:F6}", Srednia_wart_fun_przyst(nowa_pula_osobnikow));
                Console.WriteLine();

                pula_osobnikow_rob = pula_osobnikow;
                pula_osobnikow = nowa_pula_osobnikow;
                nowa_pula_osobnikow = pula_osobnikow_rob;
            }
            Console.WriteLine("-->  KONIEC");
            Console.WriteLine();
        }
    }
}
