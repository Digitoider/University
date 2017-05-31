using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TOPK_2
{
    class Program
    {
        private static List<int> CodesStack = new List<int>();
        private static bool done = false;

        static void Main(string[] args)
        {
            StreamReader rd = new StreamReader(@"C:\users\alexander\documents\visual studio 2015\Projects\TOPK_2\TOPK_2\SourceData.txt");
            string data = rd.ReadToEnd();
            rd.Close();

            string[] phrases = data.Replace("\r", "").Split('\n');
            phrases[phrases.Length - 1] += " ";

            int[,] mp = ReadJumpTable(@"C:\Users\Alexander\documents\visual studio 2015\Projects\TOPK_2\TOPK_2\JumpTable.txt");
            int N_sost = 0, line = 0;
            foreach (var phrase in phrases)
            {
                line++;
                int cnt = 0;
                int N_str;
                while (cnt < phrase.Length)
                {
                    string letter = phrase[cnt].ToString();

                    if (IsLetter(letter))
                    {
                        switch (letter)
                        {
                            case "F": N_str = 0; break;
                            case "U": N_str = 1; break;
                            case "N": N_str = 2; break;
                            case "C": N_str = 3; break;
                            case "I": N_str = 4; break;
                            case "A": N_str = 5; break;
                            case "L": N_str = 6; break;
                            case "E": N_str = 7; break;
                            default: N_str = 8; break;
                        }
                    }
                    else if (IsDigit(letter))
                    {
                        N_str = 9;
                    }
                    else
                    {
                        switch (letter)
                        {
                            case ".": N_str = 10; break;
                            case "+": N_str = 11; break;
                            case "-": N_str = 12; break;
                            case ":": N_str = 13; break;
                            case @"\": N_str = 14; break;
                            case "!": N_str = 15; break;
                            case "{": N_str = 16; break;
                            case "}": N_str = 17; break;
                            case "(": N_str = 18; break;
                            case ")": N_str = 19; break;
                            case "=": N_str = 20; break;
                            case "&": N_str = 21; break;
                            case " ": N_str = 22; break;
                            default: N_str = 23; break;
                        }
                    }

                    N_sost = mp[N_str, N_sost];

                    /* Получение состояния перехода */
                    if (N_sost > 99 && N_sost < 500)
                    {
                        /* Анализ кода состояния */
                        Console.WriteLine(DefineStateCode(N_sost) + "; строка " + line);
                        N_sost = 0;
                    }
                    else if (N_sost > 500 && N_sost <= 800)
                    {
                        Console.WriteLine(DefineStateCode(N_sost) + "; строка " + line);
                        N_sost = 0;
                        if(letter == "(" && phrase[cnt-1] == '!' && !done)
                        {
                            done = true;
                        }
                        else
                        {
                            done = false;
                            cnt++;
                        }
                    }
                    else if (N_sost > 800)
                    {
                        Console.WriteLine(DefineStateCode(N_sost) + "; строка " + line);
                        N_sost = 0;
                        cnt++;
                    }
                    else
                    {
                        cnt++;
                    }
                }
            }
            Console.WriteLine("Нажмите ENTER...");
            SaveCodes();
            //Console.ReadLine();
        }

        private static void SaveCodes()
        {
            StreamWriter wr = new StreamWriter(@"C:\users\alexander\documents\visual studio 2015\Projects\TOPK_3\TOPK_3\Codes.txt");
            wr.Write(String.Join(" ", CodesStack.ToArray()));// ","
            wr.Write(" 999");// 999
            wr.Close();
        }

        private static string DefineStateCode(int n_sost)
        {
            if(n_sost != 511)
            {
                CodesStack.Add(n_sost);
            }
            switch (n_sost)
            {
                case 100: return "Допущено: ключевое слово FUNC: код " + n_sost;
                case 200: return "Допущено: ключевое слово FINAL: код " + n_sost;
                case 300: return "Допущено: идентификатор: код " + n_sost;
                case 400: return "Допущено: константа: код " + n_sost;
                case 501: return "Допущено: знак ':' : код " + n_sost;
                case 502: return @"Допущено: знак '\' : код " + n_sost;
                case 503: return "Допущено: знак '!' : код " + n_sost;
                case 504: return "Допущено: знак '{' : код " + n_sost;
                case 505: return "Допущено: знак '}' : код " + n_sost;
                case 506: return "Допущено: знак '(' : код " + n_sost;
                case 507: return "Допущено: знак ')' : код " + n_sost;
                case 601: return "Допущено: знак ':=' : код " + n_sost;//508
                case 602: return "Допущено: знак '&&' : код " + n_sost;//509
                case 603: return "Допущено: знак '!!' : код " + n_sost;//510
                case 511: return "Допущено: знак 'пробел' : код " + n_sost;
                case 512: return "Допущено: знак '+' : код " + n_sost;
                case 513: return "Допущено: знак '-' : код " + n_sost;
                case 801: return "Не допущено: ошибка в слове FUNC: код " + n_sost;
                case 802: return "Не допущено: ошибка в слове FINAL: код " + n_sost;
                case 803: return "Не допущено: ошибка в идентификаторе: код " + n_sost;
                case 804: return "Не допущено: ошибка в определении константы: код"  + n_sost;
                default: return "Ошибка " + n_sost;
            }
        }

        private static int[,] ReadJumpTable(string path)
        {
            StreamReader rd = new StreamReader(path);
            string data = rd.ReadToEnd();
            rd.Close();

            string[] lines = data.Replace("\r", "").Split('\n');
            int[,] jumpTable = new int[lines.Length, lines[0].Split(',').Length];
            for(var i = 0; i < lines.Length; i++)
            {
                var numbers = lines[i].Split(',');
                for(var q = 0; q < numbers.Length; q++)
                {
                    jumpTable[i, q] = Int32.Parse(numbers[q]);
                }
            }
            return jumpTable;
        }
        
        static bool IsLetter(string symbol)
        {
            var regex = new Regex("[a-zA-Z]");
            return regex.IsMatch(symbol);
        }
        static bool IsDigit(string symbol)
        {
            return (new Regex("[0-9]")).IsMatch(symbol);
        }
    }
}
