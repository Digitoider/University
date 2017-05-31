using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI2
{
    class Program
    {

        static void Main(string[] args)
        {
            int n = 50;
            int[] mas = new int[n];
            Random rand = new Random();
            for (int i = 0; i < n; i++)
            {
                mas[i] = rand.Next(0, 2000);
            }
            //for (int i = 0; i < n; i++)
            //{
            //    Console.WriteLine(mas[i]);
            //}
            //Console.WriteLine("После сортировки:");
            Array.Sort(mas);
            //for (int i = 0; i < n; i++)
            //{
            //    Console.WriteLine(mas[i]);
            //}
            int index = interpolationSearch(mas, mas[rand.Next(0, n - 1)]);
            Console.WriteLine("Индекс найденного элемента {0}", index);
            Console.WriteLine("Элемент с индексом {0}: {1}", index, mas[index]);

            Console.WriteLine("Нажмите ENTER для продолжения...");
            Console.ReadLine();

            Console.WriteLine("Строка \"{0}\". Посдстрока \"{1}\"", "abcdefghijklmnopqrstuvwxyz", "defghi");
            KnuthMorrisPratt kmp = new KnuthMorrisPratt("abcdefghijklmnopqrstuvwxyz", "defghi");

            Console.WriteLine("Нажмите ENTER для продолжения...");
            Console.ReadLine();

            List<string> file = new List<string>();
            file.Add("Строка слово супер");
            file.Add("Строка может быть тут");
            file.Add("слово тут тоже есть");
            List<string> words = new List<string>();
            foreach (var line in file)
            {
                List<string> temp = line.Split(' ').ToList();
                foreach (string word in line.Split(' '))
                {
                    if (!words.Exists(elem => elem == word))
                    {
                        words.Add(word);
                    }
                }
            }
            List<KeyValuePair<string, List<int>>> glossary = new List<KeyValuePair<string, List<int>>>();
            foreach (string word in words)
            {
                glossary.Add(new KeyValuePair<string, List<int>>(word, new List<int>()));
            }
            //for (int i = 0; i< file.Count; i++)
            //{
            //    if()
            //}
            foreach (var elem in glossary)
            {
                //foreach(string sentence in file)
                //{

                //}
                for (int i = 0; i < file.Count; i++)
                {
                    if (kmp.posMatch(file[i], elem.Key) != -1)
                    {
                        elem.Value.Add(i);
                    }
                }
            }
            Console.WriteLine("Считанные строки:");
            foreach (string line in file)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("\nГлоссарий:");
            foreach (var elem in glossary)
            {
                Console.Write("{0:10}: ", elem.Key);
                foreach (int lineNumber in elem.Value)
                {
                    Console.Write(lineNumber+1 + ", ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Нажмите ENTER для продолжения...");
            Console.ReadLine();
        }

        static int interpolationSearch(int[] sortedArray, int toFind)
        {
            // Возвращает индекс элемента со значением toFind или -1, если такого элемента не существует
            int mid;
            int low = 0;
            int high = sortedArray.Length - 1;

            while (sortedArray[low] < toFind && sortedArray[high] > toFind)
            {
                mid = low + ((toFind - sortedArray[low]) * (high - low)) / (sortedArray[high] - sortedArray[low]);

                if (sortedArray[mid] < toFind)
                    low = mid + 1;
                else if (sortedArray[mid] > toFind)
                    high = mid - 1;
                else
                    return mid;
            }

            if (sortedArray[low] == toFind)
                return low;
            else if (sortedArray[high] == toFind)
                return high;
            else
                return -1; // Not found
        }
    }
}
