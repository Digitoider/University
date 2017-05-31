using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TOI3
{
    class ChartForm: Form
    {
        public ChartForm()
        {
            this.Width = 600;
            this.Height = 400;
        }
        public void AddChart(List<KeyValuePair<int, string>> data, string name)
        {
            var series = new Series() {
                Name = name,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Pie
            };
            List<int> x = new List<int>();
            List<int> y = new List<int>();
            for(int i = 0; i < data.Count; i++)
            {
                x.Add(i);
                y.Add(data[i].Key);
                series.Points.InsertXY(i, data[i].Value, data[i].Key);
            }

            //series.Points.AddXY(x.ToArray(), y.ToArray());

            ChartArea chartArea1 = new ChartArea();
            chartArea1.Name = name;
            Chart chart1 = new Chart();
            ((System.ComponentModel.ISupportInitialize)(chart1)).BeginInit();
            chart1.ChartAreas.Add(chartArea1);
            chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            Legend legend1 = new Legend("Legend1");
            chart1.Legends.Add(legend1);
            chart1.Name = name;
            // this.chart1.Size = new System.Drawing.Size(284, 212);
            chart1.TabIndex = 0;
            chart1.Text = name;

            chart1.Series.Add(series);
            this.Controls.Add(chart1);
        }
    }   
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader rd = new StreamReader(@"C:\Users\Alexander\Documents\visual studio 2015\Projects\TOI3\TOI3\Articles\eng.txt");
            string englishArticle = rd.ReadToEnd().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").ToLower();
            rd.Close();

            rd = new StreamReader(@"C:\Users\Alexander\Documents\visual studio 2015\Projects\TOI3\TOI3\Articles\rus.txt");
            string russianArticle = rd.ReadToEnd().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").ToLower();
            rd.Close();

            string[] englishWords = GetWords(englishArticle);
            string[] russianWords = GetWords(russianArticle);

            List<KeyValuePair<int, string>> englishFrequency = GetFrequency(englishWords);
            List<KeyValuePair<int, string>> russianFrequency = GetFrequency(russianWords);

            WriteAnswer(englishFrequency, @"C:\Users\Alexander\Documents\visual studio 2015\Projects\TOI3\TOI3\Articles\engFrequency.txt");
            WriteAnswer(russianFrequency, @"C:\Users\Alexander\Documents\visual studio 2015\Projects\TOI3\TOI3\Articles\rusFrequency.txt");
            
            Console.WriteLine("Нажмите ENTER для завершения...");
            Console.ReadLine();
        }

        private static void DisplayGraph(string lang, List<KeyValuePair<int, string>> englishFrequency)
        {
            
        }

        private static void WriteAnswer(List<KeyValuePair<int, string>> frequency, string path)
        {
            StreamWriter wr = new StreamWriter(path, false);
            foreach (var elem in frequency)
            {
                string str = elem.Value + (new string(' ', 30 - elem.Value.Length));
                wr.WriteLine("{0:30}:{1}", str, elem.Key);
            }
            wr.Close();
        }

        private static List<KeyValuePair<int, string>> GetFrequency(string[] englishWords)
        {
            List<KeyValuePair<int, string>> usedWords = new List<KeyValuePair<int, string>>();
            for (int i = 0; i < englishWords.Length; i++)
            {
                int amount = 0;
                bool wordExists = false;
                for (int j = i; j < englishWords.Length; j++)
                {
                    if (usedWords.Exists(elem => elem.Value == englishWords[i]))
                    {
                        wordExists = true;
                        break;
                    }
                    if (englishWords[i] == englishWords[j])
                    {
                        amount++;
                    }
                }
                if (!wordExists)
                {
                    usedWords.Add(new KeyValuePair<int, string>(amount, englishWords[i]));
                }
            }
            return usedWords;
        }

        private static string[] GetWords(string str)
        {
            char[] separators = { '.', '!', '?', ' ', ',', ':', '(', ')'};
            string[] words = str.Split(separators);

            for (int i = 0; i < words.Length; i++)
            {
                foreach (char separator in separators)
                {
                    words[i] = words[i].Replace(separator.ToString(), "");
                }
            }
            List<string> filteredWords = new List<string>();
            foreach (var elem in words)
            {
                if (elem.Length > 1)
                {
                    filteredWords.Add(elem);
                }
            }
            return filteredWords.ToArray();
        }
    }
}
