using System;
using System.IO;

namespace TOPK_1
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader rd = new StreamReader(@"C:\users\alexander\documents\visual studio 2015\Projects\TOPK_1\TOPK_1\SourceData.txt");
            string data = rd.ReadToEnd();
            rd.Close();

            string[] phrases = data.Replace("\r", "").Split('\n');

            int[,] mp = new int[3, 7]{{ 1,-2, 2, 3, 4, 6, -10},
                                      { 5, 2, 3, 3,-4, 5, -10},
                                      {-1,-2,-3, 4, 4,-5, -10}};
            int i=0, n=0; /* i – номер состояния КА, n – число
            правильных слов */
            foreach (var phrase in phrases)
            {
                int cnt = 0;
                string error = "Ошибка";
                bool errorFlag = false;
                while (cnt < phrase.Length)
                {
                    string letter = phrase[cnt].ToString();
                    switch (letter)
                    {
                        case "a": i = mp[0,i]; break;
                        case "b": i = mp[1,i]; break;
                        case "c": i = mp[2,i]; break;
                    }
                    if (i < 0)//проеверяем, есть ли ошибка
                    {
                        error = GetErrorMessage(i);
                        i = 0;
                        break;
                    }
                    if ((i==3 || i==4 || i==6) && cnt == phrase.Length - 1)//конечное состояние
                    {
                        i = 0;
                        n++;
                        errorFlag = true;
                    }
                    cnt++;
                }
                if(errorFlag)
                {
                    Console.WriteLine("Слово '{0}' является верной", phrase);
                }
                else
                {
                    Console.WriteLine("{0} в слове '{1}'",error, phrase);
                }
                errorFlag = false;
            }
            Console.ReadLine();
        }

        private static string GetErrorMessage(int i)
        {
            switch (i)
            {
                default: return "Ошибка";
            }
        }
    }
}
