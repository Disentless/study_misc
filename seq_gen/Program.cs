using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seq_gen
{
    class Program
    {
        static void Main(string[] args)
        {
            var output = new System.IO.StreamWriter("data.txt", false);
            for (int n = 10; n < 20; ++n)
            {
                for (int r = 2; r < n - 2; ++r)
                {
                    for (int k = r; k < n - 2; ++k)
                    {
                        output.WriteLine(n + "\t" + r + "\t" + k);
                    }
                }
            }
            output.Close();
        }
    }
}
