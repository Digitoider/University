using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace toi4
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText(@"c:\users\alexander\documents\visual studio 2015\Projects\toi4\toi4\text.txt");

            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            string[] words = text.Split(' ', ',', '!', '.', '/', '(', '?', ')', '_', '\t', '\0', '\r', '\n');
            string[] sentences = text.Split('!', '.', '?');
            string[] prepositions = {"и" , "а", "то", "по", "на", "-", "от", "но", "о", "и", "е", "а",
            "т", "с", "л", "в", "к", "д", "у", "из", " ", "до", "их", "им", "", "", ""};

            foreach (string word in words)
            {
                if (!dictionary.ContainsKey(word) && !prepositions.Contains(word))
                {
                    dictionary.Add(word, new Regex(word).Matches(text).Count);
                }
            }

            Dictionary<string, double> frequencyDictionary = new Dictionary<string, double>();
            foreach (KeyValuePair<string, int> dictionaryWord in dictionary)
            {
                frequencyDictionary.Add(dictionaryWord.Key, (double)dictionaryWord.Value / (double)words.Length);
            }

            string mainWord = "";
            double maxValue = 0.0;
            foreach (KeyValuePair<string, double> pair in frequencyDictionary)
            {
                if (pair.Value > maxValue)
                {
                    mainWord = pair.Key;
                    maxValue = pair.Value;
                }
            }
            Console.WriteLine("Mainword: " + mainWord);

            foreach (string sentence in sentences)
            {
                if (sentence.Contains(mainWord))
                {
                    Console.WriteLine(sentence);
                }
            }


            Console.ReadKey();

        }
    }
}
