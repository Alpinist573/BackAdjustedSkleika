using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // string str = "44570.0000000";
            string str = "44570.0000";
            string str2 = str.Replace('.', ',');

            double open = double.Parse(str);

        }
    }
}
