using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTable
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkTest();
            Console.ReadLine();
        }

        private static void BenchmarkTest()
        {
            var limit = int.MaxValue / 10752;

            var hs = new HashTable<string, int>();

            var addHs = new Stopwatch();
            addHs.Start();
            for (int i = 0; i < limit; i++)
                hs.Add(i.ToString(), i);
            addHs.Stop();

            var readHs = new Stopwatch();
            readHs.Start();
            foreach (var item in hs)
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            readHs.Stop();

            var delHs = new Stopwatch();
            delHs.Start();
            hs.Clear();
            delHs.Stop();

            hs = null;

            var dic = new Dictionary<string, int>();

            var addDic = new Stopwatch();
            addDic.Start();
            for (int i = 0; i < limit; i++)
                dic.Add(i.ToString(), i);
            addDic.Stop();

            var readDic = new Stopwatch();
            readDic.Start();
            foreach (var item in dic)
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            readDic.Stop();

            var delDic = new Stopwatch();
            delDic.Start();
            dic.Clear();
            delDic.Stop();

            dic = null;

            var csHs = new System.Collections.Hashtable();

            var addCsHs = new Stopwatch();
            addCsHs.Start();
            for (int i = 0; i < limit; i++)
                csHs.Add(i, i);
            addCsHs.Stop();

            var readCsHs = new Stopwatch();
            readCsHs.Start();
            foreach (System.Collections.DictionaryEntry item in csHs)
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            readCsHs.Stop();

            var delCsHs = new Stopwatch();
            delCsHs.Start();
            csHs.Clear();
            delCsHs.Stop();

            csHs = null;

            Console.WriteLine("HashTable vs. Dictionary vs C# HashTable");
            Console.WriteLine("Add - {0} - {1} - {2}", addHs.Elapsed.TotalSeconds, addDic.Elapsed.TotalSeconds, addCsHs.Elapsed.TotalSeconds);
            Console.WriteLine("Read - {0} - {1} - {2}", readHs.Elapsed.TotalSeconds, readDic.Elapsed.TotalSeconds, readCsHs.Elapsed.TotalSeconds);
            Console.WriteLine("Delete - {0} - {1} - {2}", delHs.Elapsed.TotalSeconds, delDic.Elapsed.TotalSeconds, delCsHs.Elapsed.TotalSeconds);
        }
    }
}
