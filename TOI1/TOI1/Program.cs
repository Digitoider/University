using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите строку:");
            string str = Console.ReadLine();

            string[] words = GetWords(str);

            string[] shortestWords = GetShortestWords(words);

            List<KeyValuePair<int, string>> usedWords1 = new List<KeyValuePair<int, string>>();
            for(int i = 0; i < shortestWords.Length; i++)
            {
                int amount = 0;
                bool wordExists = false;
                for(int j = i; j < shortestWords.Length; j++)
                {
                    if (usedWords1.Exists(elem => elem.Value == shortestWords[i]))
                    {
                        wordExists = true;
                        break;
                    }
                    if(shortestWords[i] == shortestWords[j])
                    {
                        amount++;
                    }
                }
                if (!wordExists)
                {
                    usedWords1.Add(new KeyValuePair<int, string>(amount, shortestWords[i]));
                }
            }
            Console.WriteLine("Ответ: ");
            foreach(var elem in usedWords1)
            {
                Console.WriteLine("{0} встретилось {1} раз", elem.Value, elem.Key);
            }
            Console.WriteLine("Нажмите ENTER для завершения...");
            Console.ReadLine();
        }

        private static string[] GetShortestWords(string[] words)
        {
            int min = words[0].Length;
            for (int i = 1; i < words.Length; i++)
            {
                if(words[i].Length < min && words.Length > 1)
                {
                    min = words[i].Length;
                }
            }
            List<string> shortestWords = new List<string>(); 
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length == min)
                {
                    shortestWords.Add(words[i]);
                }
            }

            return shortestWords.ToArray();
        }

        private static string[] GetWords(string str)
        {
            char[] separators = { '.', '!', '?', ' ', ',' };
            string[] words = str.Split(separators);

            for(int i = 0; i < words.Length; i++)
            {
                foreach (char separator in separators) {
                    words[i] = words[i].Replace(separator.ToString(), "");
                }
            }
            List<string> filteredWords = new List<string>();
            foreach(var elem in words)
            {
                if(elem.Length > 2)
                {
                    filteredWords.Add(elem);
                }
            }
            return filteredWords.ToArray();
        }
    }
}
