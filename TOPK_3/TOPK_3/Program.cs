using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPK_3
{
    class Program
    {
        static int codeindex = 0;
        static int[] codes;
        static void Main(string[] args)
        {
            codes = ReadCodes();
            if (S_fragment() == 0)
            {
                Console.WriteLine("Синтаксическая ошибка");
            }
            else
            {
                Console.WriteLine("FINISHED. Статус: OK");
            }
            Console.ReadLine();
        }

        private static int S_fragment()
        {
            if (codes[codeindex] == 300)
            {
                return S_iden() *
                    S_colon() *
                    S_func() *
                    S_leftRounded() *
                    S_params() *
                    S_rightRounded() *
                    S_leftCurly() *
                    S_operators() *
                    S_rightCurly() * S_final()
                    ;
            }
            else
            {
                Console.WriteLine("Неверное начало программы");
                return 0;
            }
        }

        private static int S_operators()
        {
            if (codes[codeindex] == 300) //iden
            {
                while(codes[codeindex] == 300)
                {
                    if(S_operator() == 0)
                    {
                        return 0;
                    }
                }
                return 1;
            }
            else
            {
                return 1;////????????????????????????????????
            }
        }

        private static int S_operator()
        {
            return S_iden() *
                S_assign() *
                S_denySign() *
                S_leftRounded() *
                S_expression() *
                S_rightRounded() *
                S_endOperator();
        }

        private static int S_endOperator()
        {
            if (codes[codeindex] == 502)//\
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine(@"Ожидался знак '\'");
                return 0;
            }
        }

        private static int S_expression()
        {
            return S_operand() * S_sign_iden();
        }

        private static int S_sign_iden()
        {
            int response = 1;
            while(codes[codeindex] == 509 || codes[codeindex] == 510)
            {
                response *= S_sign() * S_operand();
            }

            return response;
        }

        private static int S_operand()
        {
            switch (codes[codeindex])
            {
                case 300: return S_iden();
                case 400: return S_data();
                default:
                    Console.WriteLine("Ожидался идентификатор или константа");//Ожидался знак '&&' или '!!'
                    return 0;
            }
        }

        private static int S_data()
        {
            if (codes[codeindex] == 400)//!
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидалась константа");
                return 0;
            }
        }

        private static int S_sign()
        {
            switch (codes[codeindex])
            {
                case 509: if(codeindex < codes.Length-1)  codeindex++; return 1;
                case 510: if(codeindex < codes.Length-1)  codeindex++; return 1;
                default: Console.WriteLine("Ожидался знак '&&' или '!!'");
                    return 0;
            }
        }

        private static int S_denySign()
        {
            if (codes[codeindex] == 503)//!
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else if(codes[codeindex] == 506)//(
            {
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидался знак '!'");
                return 0;
            }
        }

        private static int S_assign()
        {
            if (codes[codeindex] == 508)//:=
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидался знак ':='");
                return 0;
            }
        }

        private static int S_final()
        {
            if (codes[codeindex] == 200)
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидалось ключевое слово 'FINAL'");
                return 0;
            }
        }

        private static int S_rightCurly()
        {
            if (codes[codeindex] == 505)
            {
                
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидался знак '}'");
                return 0;
            }
        }

        private static int S_leftCurly()
        {
            if (codes[codeindex] == 504)
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидался знак '{'");
                return 0;
            }
        }

        private static int S_rightRounded()
        {
            if (codes[codeindex] == 507)//)
            {
                if(codeindex < codes.Length-1) if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидался знак ')'");
                return 0;
            }
        }

        private static int S_params()
        {
            if (codes[codeindex] == 300)//iden
            {
                return S_iden();
            }
            else if (codes[codeindex] == 507)//)
            {
                return S_rightRounded();
            }
            else
            {
                Console.WriteLine("Ожидался знак ')'");
                return 0;
            }
        }

        private static int S_leftRounded()
        {
            if (codes[codeindex] == 506)
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидался знак '('");
                return 0;
            }
        }

        private static int S_func()
        {
            if (codes[codeindex] == 100)
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидалось ключевое слово 'FUNC'");
                return 0;
            }
        }

        private static int S_colon()
        {
            if (codes[codeindex] == 501)
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Ожидалось ':'");
                return 0;
            }
        }

        private static int S_iden()
        {
            if(codes[codeindex] == 300)//iden
            {
                if(codeindex < codes.Length-1)  codeindex++;
                return 1;
            }
            else
            {
                Console.WriteLine("Нет идентификатора");
                return 0;
            }
        }

        private static int[] ReadCodes()
        {
            StreamReader rd = new StreamReader(@"c:\users\alexander\documents\visual studio 2015\Projects\TOPK_3\TOPK_3\Codes.txt");
            string codes = rd.ReadToEnd().Replace("\n", "").Replace("\r", "");
            rd.Close();

            string[] splitedCodes = codes.Split(',');
            List<int> result = new List<int>();
            foreach(var code in splitedCodes)
            {
                result.Add(Int32.Parse(code));
            }
            
            return result.ToArray();
        }
    }
}
