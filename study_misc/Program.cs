using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace study_misc
{
    class Program
    {
        static private System.IO.StreamWriter ListOutput;
        static private int SequenceLength = 0;
        static private int CursorLeft;
        static private int CursorTop;
        static private long CalculatedAmount;

        static private List<KeyValuePair<string, int>> getPartialSequence(int n, int r)
        {
            if (n == 1)
            {
                return new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("1", 1), new KeyValuePair<string, int>("0", 0) });
            }
            var lowerPartial = getPartialSequence(n - 1, r);
            var result = new List<KeyValuePair<string, int>>();
            foreach (var s in lowerPartial)
            {
                result.Add(new KeyValuePair<string, int>("0" + s.Key, s.Value));
                if (s.Value == r - 1)
                {
                    var str = "1" + s.Key;
                    while (str.Length != SequenceLength)
                    {
                        str = "0" + str;
                    }
                    ListOutput.WriteLine(str);
                    Console.SetCursorPosition(CursorLeft, CursorTop);
                    continue;
                }
                else
                    result.Add(new KeyValuePair<string, int>("1" + s.Key, s.Value + 1));
            }
            // Result should contains only lines with less than required amount
            return result;
        }

        static double factor(int n)
        {
            if (n < 0) return 0;
            if (n == 1 || n == 0)
            {
                return 1;
            }
            return n * factor(n - 1);
        }

        static double c(int n, int m)
        {
            return factor(n) / factor(m) / factor(n - m);
        }

        static void Main(string[] args)
        {
            // Load test data
            var input = new System.IO.StreamReader("data.txt");
            var output = new System.IO.StreamWriter("data_output.txt");
            var data_list = new List<int[]>();
            while (!input.EndOfStream)
            {
                var line = input.ReadLine();
                var match = System.Text.RegularExpressions.Regex.Match(line, @"^(\d+)[\s\t](\d+)[\s\t](\d+)$");
                if (!match.Success) return;
                var n = int.Parse(match.Groups[1].Value);
                var r = int.Parse(match.Groups[2].Value);
                var m = int.Parse(match.Groups[3].Value);
                data_list.Add(new int[] { n, r, m });
            }
            input.Close();
            var results = new List<string>();
            Console.WriteLine(" n  | r  | m  |\tC\t|\tExp\t|\tFormula\t|\tR(exp)\t|\tR(formula)");
            foreach (var data_sequence in data_list)
            {
                // Generate all possible sequences with numbers 0 and 1
                // Sequence length
                SequenceLength = data_sequence[0];
                // Amount of 1
                var amount1 = data_sequence[1];
                // Required length
                var lengthR = data_sequence[2];

                var count = 0;
                var count2 = 0;

                CursorLeft = Console.CursorLeft;
                CursorTop = Console.CursorTop;
                var c1 = factor(SequenceLength);
                var c2 = factor(amount1);
                var c3 = factor(SequenceLength - amount1);
                CalculatedAmount = (long)Math.Round(c1 / c2 / c3);

                if (!System.IO.File.Exists(SequenceLength + "_" + amount1 + ".txt"))
                {
                    ListOutput = new System.IO.StreamWriter(SequenceLength + "_" + amount1 + ".txt");
                    getPartialSequence(SequenceLength, amount1);
                    ListOutput.Close();
                }

                var list = new System.IO.StreamReader(SequenceLength + "_" + amount1 + ".txt");
                while (!list.EndOfStream)
                {
                    //Console.Write(s);
                    var s = list.ReadLine();
                    int firstIndex = 0;
                    int lastIndex = 0;
                    int l = 0;
                    for (int i = 0; i < s.Length; ++i)
                    {
                        if (s[i] != '1') continue;
                        // Assumed first 1
                        firstIndex = i;
                        lastIndex = 0;
                        // Looping with wrapping
                        for (int k = i + 1; k != firstIndex; k++)
                        {
                            // overflow?
                            if (k == s.Length)
                            {
                                k = 0;
                                if (firstIndex == 0) break;
                            }
                            if (s[k] != '1') continue;
                            // Assumed last 1
                            lastIndex = k;
                        }
                        if (firstIndex < lastIndex)
                        {
                            // Normal flow
                            l = lastIndex - firstIndex + 1;
                        }
                        else
                        {
                            // Wrapped flow
                            l = SequenceLength - (firstIndex - lastIndex) + 1;
                        }
                        if (l <= lengthR)
                        {
                            count2++;
                            break;
                        }
                    }
                    //Console.WriteLine(" - " + l + "(" + (l > lengthR ? "false" : "true") + ")");
                    count++;
                    Console.SetCursorPosition(CursorLeft, CursorTop);
                    Console.Write(((float)list.BaseStream.Position / list.BaseStream.Length).ToString("00.00%"));
                }
                list.Close();
                Console.SetCursorPosition(CursorLeft, CursorTop);
                var n = SequenceLength;
                var m = lengthR;
                var r = amount1;
                var kk = 2 * m - n;
                var t = 3 * m - 2 * n - 2;

                var B = n * c(m - 1, r - 1);
                var L = 0.0;
                var P = 0.0;
                if (kk >= r)
                {
                    L = c(kk - 2, r - 2) * (kk - 1) * ((n - m) + kk / 2.0);
                }
                if (t >= r - 2)
                {
                    P = c(t - 1, r - 3) * t * (1 + t) / 2.0 * (n - m + 1);
                }
                var formula = B - L + P;
                Console.WriteLine(" " + SequenceLength.ToString("00") + " | " + amount1.ToString("00") + " | " + lengthR.ToString("00") + " |\t" + c(n, r) + "\t|\t" + count2 + "\t|\t" + formula.ToString("0") + "\t|\t" + ((double)count2 / count).ToString("0.000") + "\t|\t" + ((double)formula / count).ToString("0.000"));
                output.WriteLine(n + "\t" + r + "\t" + m + "\t" + count2);
            }
            output.Close();
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
