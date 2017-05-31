using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI2
{
    class KnuthMorrisPratt
    {
        /** Failure array **/

        private int[] failure;

        /** Constructor **/

        public KnuthMorrisPratt(String text, String pat)

        {

            /** pre construct failure array for a pattern **/

            failure = new int[pat.Length];

            fail(pat);

            /** find match **/

            int pos = posMatch(text, pat);

            if (pos == -1)

                Console.WriteLine("\nNo match found");

        else

            Console.WriteLine("\nMatch found at index " + pos);

        }

        /** Failure function for a pattern **/

        private void fail(String pat)

        {

            int n = pat.Length;

            failure[0] = -1;

            for (int j = 1; j < n; j++)

            {

                int i = failure[j - 1];
                
                while ((pat.ElementAt(j) != pat.ElementAt(i + 1)) && i >= 0)

                    i = failure[i];

                if (pat.ElementAt(j) == pat.ElementAt(i + 1))

                    failure[j] = i + 1;

                else

                    failure[j] = -1;

            }

        }

        /** Function to find match for a pattern **/

        public int posMatch(String text, String pat)

        {

            int i = 0, j = 0;

            int lens = text.Length;

            int lenp = pat.Length;

            while (i < lens && j < lenp)

            {

                if (text.ElementAt(i) == pat.ElementAt(j))

                {

                    i++;

                    j++;

                }

                else if (j == 0)

                    i++;

                else

                    j = failure[j - 1] + 1;

            }

            return ((j == lenp) ? (i - lenp) : -1);

        }
    }
}
