using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPK_4;

namespace TOPC4.COMPILER
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = Process.Start(@"C:\Users\Alexander\Documents\Visual Studio 2015\Projects\TOPK_2\TOPK_2\bin\Debug\TOPK_2.exe");
            while (!p.HasExited) ;
            SyntaxAnalizer sa = new SyntaxAnalizer();
            sa.ReadCodes(@"C:\users\alexander\documents\visual studio 2015\Projects\TOPK_3\TOPK_3\Codes.txt");
            if (sa.Scan())
            {
                //Console.WriteLine("Okay");
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
            return;
        }
    }
}