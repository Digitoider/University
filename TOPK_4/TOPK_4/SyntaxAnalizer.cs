using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPK_4
{

    class SyntaxAnalizer
    {
        List<string> codes;
        string[] stack = new string[1000];
        Dictionary<string, int> GR = new Dictionary<string, int>();
        int[,] SR = new int[,]
            {
                //{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 99 },
                //{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                //{4, 2, 1, 6, 6, 6, 6, 6, 6, 6, 1, 6, 1, 6, 6, 6, 6, 6, 6, 3},
                //{4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2, 3},
                //{4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 3},
                //{4, 6, 6, 6, 6, 6, 2, 6, 6, 6, 6, 3, 6, 6, 6, 6, 1, 1, 6, 3},
                //{4, 6, 6, 6, 6, 2, 6, 6, 6, 6, 1, 6, 6, 6, 6, 1, 6, 6, 6, 3},
                //{10, 10, 10, 10, 10, 10, 10, 10, 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 3},
                //{4, 5, 5, 5, 5, 5, 5, 5, 5, 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 3},
                //{4, 6, 6, 6, 2, 1, 6, 6, 6, 6, 1, 6, 6, 6, 6, 1, 6, 6, 6, 3},
                //{4, 6, 6, 6, 6, 6, 3, 2, 6, 6, 6, 3, 6, 2, 6, 6, 3, 3, 6, 3},
                //{4, 2, 1, 6, 6, 6, 6, 6, 6, 6, 1, 5, 1, 5, 5, 5, 5, 5, 3, 3},
                //{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
                //{4, 7, 7, 2, 7, 7, 7, 7, 7, 1, 7, 7, 7, 7, 1, 7, 7, 7, 7, 3},
                //{4, 7, 7, 7, 7, 7, 7, 7, 7, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 3},
                //{4, 7, 7, 7, 7, 7, 3, 7, 7, 7, 7, 3, 7, 7, 7, 7, 3, 3, 7, 3},
                //{4, 7, 7, 7, 7, 3, 7, 7, 7, 7, 3, 7, 7, 7, 7, 3, 7, 7, 7, 3},
                //{4, 7, 7, 7, 7, 3, 7, 7, 7, 7, 3, 7, 7, 7, 7, 3, 7, 7, 7, 3},
                //{4, 3, 3, 7, 7, 7, 7, 7, 7, 7, 3, 7, 3, 7, 7, 7, 7, 7, 7, 3},
                //{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 99}

               //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5  6  7  8  9 
               /* 0*/ {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 99 },
               /* 1*/ {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
               /* 2*/ {4, 2, 1, 6, 6, 6, 6, 6, 6, 6, 1, 6, 1, 6, 6, 6, 6, 6, 6, 3},
               /* 3*/ {4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2, 3},
               /* 4*/ {4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 3},
               /* 5*/ {4, 6, 6, 6, 6, 13, 2, 6, 6, 6, 6, 3, 6, 6, 6, 6, 1, 1, 6, 3},
               /* 6*/ {4, 6, 6, 6, 6, 2, 6, 6, 6, 6, 1, 6, 6, 6, 6, 1, 6, 6, 6, 3},
               /* 7*/ {4, 4, 4, 4, 4, 4, 4, 4, 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 3},
               /* 8*/ {4, 5, 5, 5, 5, 5, 5, 5, 5, 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 3},
               /* 9*/ {4, 6, 6, 6, 2, 1, 6, 6, 6, 6, 1, 6, 6, 6, 6, 1, 6, 6, 6, 3},
               /*10*/ {4, 6, 6, 6, 6, 6, 3, 2, 4, 6, 6, 3, 6, 2, 6, 6, 3, 3, 6, 3},
               /*11*/ {4, 2, 1, 6, 6, 6, 6, 6, 6, 6, 1, 5, 1, 5, 5, 5, 5, 5, 3, 3},
               /*12*/ {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3},
               /*13*/ {4, 7, 7, 2, 7, 7, 7, 7, 7, 1, 6, 7, 7, 7, 1, 7, 7, 7, 7, 3},
               /*14*/ {4, 7, 7, 7, 7, 7, 7, 7, 7, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 3},
               /*15*/ {4, 7, 7, 7, 7, 7, 3, 4, 4, 7, 8, 3, 8, 8, 8, 8, 3, 3, 7, 3},
               /*16*/ {4, 6, 6, 6, 6, 3, 7, 7, 7, 7, 3, 6, 6, 6, 6, 3, 7, 7, 7, 3},
               /*17*/ {4, 6, 6, 6, 6, 3, 7, 7, 7, 7, 3, 6, 6, 6, 6, 3, 7, 7, 7, 3},
               /*18*/ {4, 3, 3, 8, 8, 8, 8, 8, 7, 7, 3, 8, 3, 8, 8, 8, 8, 8, 8, 3},
               /*19*/ {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 99}
            };

        public SyntaxAnalizer()
        {
            InitGrammar();
        }

        private void InitGrammar()
        {
            GR.Add("300 501 100 506 705 507 702", 701);
            GR.Add("703 702", 702);
            GR.Add("200", 702);
            GR.Add("300 601 704 502", 703);
            GR.Add("506 705 507", 704);
            GR.Add("503 506 705 507", 704);
            GR.Add("706", 705);
            GR.Add("706 707 706", 705);
            GR.Add("300", 706);
            GR.Add("400", 706);
            GR.Add("602", 707);
            GR.Add("603", 707);
        }


        public void ReadCodes(string path)
        {
            StreamReader rd = new StreamReader(path);
            codes = rd.ReadToEnd().Split(' ').ToList();
            rd.Close();
        }

        public bool Scan()
        {
            try
            {
                stack[0] = "999";
                string currentCode = "";
                int i = 0, k = 0;
                while (stack[i] != "701" && currentCode != "999" )// <фрагмент>
                {
                    currentCode = codes[k];
                    k++;


                    while (true)
                    {
                        int row = FindColRow(stack[i]);
                        int col = FindColRow(currentCode);
                        if (SR[row, col] == 3 && i > 0)// ">"
                        {
                            int q = i;

                            while (true)
                            {
                                if (q <= 0)
                                {
                                    Console.WriteLine("God damn it");
                                    return false;
                                }
                                row = FindColRow(stack[q - 1]);
                                col = FindColRow(stack[q]);
                                if (SR[row, col] == 1)// "<"
                                {
                                    break;
                                }
                                q--;
                            }

                            int rule = FindRule(q, i);
                            if (rule != 0)
                            {
                                i = q;
                                stack[i] = rule.ToString();
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("Ошибка!");
                                //PrintError(col, row);
                                Console.ReadKey();
                                return false;
                            }
                        }
                        else
                        {
                            if (PrintError(row, col))
                            {
                                Console.WriteLine("[{0},{1}]", row, col);
                                return false;
                            }
                            i++;
                            stack[i] = currentCode;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("В ходе разбора произошла ошибка");
                return false;
            }
            return true;
        }

        private bool PrintError(int row, int col)
        {
            switch (SR[row, col])
            {
                case 4: Console.WriteLine("Неверная структура программы"); return true;
                case 5: Console.WriteLine("Ошибка компоновки тела модуля"); return true;
                case 6: Console.WriteLine("Ошибка составления выражения"); return true;
                case 7: Console.WriteLine("Недопустимая комбинация знаков"); return true;
                case 10: Console.WriteLine("После идентификатора функции д следовать имя функции"); return true;
                case 13: Console.WriteLine("Недопустимое кол-во операндов"); return true;
                case 99: Console.WriteLine("Ok"); return false;
            }
            return false;
        }

        private int FindRule(int startIndex, int endIndex)
        {

            // abcdefg (3,5)
            //int count = endIndex - startIndex+1;
            //string str = String.Join(" ", codes, startIndex, count);
            string str = "";
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i != endIndex)
                    str += stack[i].ToString() + " ";
                else
                    str += stack[i].ToString();
            }
            if (GR.ContainsKey(str))
            {
                return GR[str];
            }
            return 0;
        }

        private int FindColRow(string currentCode)
        {
            int code = Int32.Parse(currentCode);
            switch (code)
            {
                case 100://FUNC
                    return 8;
                case 200://FINAL
                    return 12;
                case 300:// <iden>
                    return 10;
                case 400:// <data>
                    return 15;
                case 501:// :
                    return 7;
                case 502:// \
                    return 18;
                case 503:// !
                    return 14;
                case 506:// (
                    return 9;
                case 507:// )
                    return 11;
                case 601:// :=
                    return 13;
                case 602:// &&
                    return 16;
                case 603:// !!
                    return 17;
                case 701:// фрагмент 
                    return 0;
                case 702:// тело
                    return 1;
                case 703:// оператор
                    return 2;
                case 704:// выражение
                    return 3;
                case 705:// подвыражение
                    return 4;
                case 706:// операнд
                    return 5;
                case 707:// знак
                    return 6;
                case 999:
                    return 19;
                default:
                    return 322;

            }
        }
    }
}
