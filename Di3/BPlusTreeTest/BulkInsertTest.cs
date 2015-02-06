using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BPlusTreeTest
{
    public static class BulkInsertTest
    {
        public static void Run()
        {
            Console.WriteLine("Now generating sorted items ...");

            int nNewItems = 100000;
            int nOldItems = 100000;
            var rnd = new Random();
            var sortedItems = new SortedDictionary<double, string>();
            int newItem = 0;
            for (int i = 1; i <= nNewItems; i++)
            {
                do { newItem = rnd.Next(0, Int32.MaxValue); }
                while (sortedItems.ContainsKey(newItem));

                sortedItems.Add(newItem, Convert.ToString(rnd.Next(0, Int32.MaxValue)));
                Console.Write("\rAdding {0,5}% : {1,10:N0}/{2:N0}", Math.Round((((double)i / (double)nNewItems) * 100)), i, nNewItems);
            }

            Stopwatch stp = new Stopwatch();

            var options = new BPlusTree<double, string>.OptionsV2(PrimitiveSerializer.Double, PrimitiveSerializer.String);
            options.CalcBTreeOrder(16, 24);
            options.FileBlockSize = 8192;
            options.CreateFile = CreatePolicy.Always;
            options.FileName = "I:\\test.tmp";

            BulkInsertOptions opts = new BulkInsertOptions();
            opts.CommitOnCompletion = true; // check how to properly set this value using Roger examples.
            opts.DuplicateHandling = DuplicateHandling.LastValueWins;
            opts.InputIsSorted = true;
            opts.ReplaceContents = false;

            AddUpdateValue update = new AddUpdateValue();

            Console.WriteLine();
            Console.WriteLine("Now creating tree ...");
            using (var tree = new BPlusTree<double, string>(options))
            {
                stp.Start();
                for (int i = 0; i < nOldItems; i++)
                    tree.AddOrUpdate(rnd.Next(0, Int32.MaxValue), ref update);
                stp.Stop();
                Console.WriteLine("Initial <{0:N0}> items =>  ET : {1}     Speed : {2:N0} item/sec", nOldItems, stp.Elapsed, Math.Round((double)(nOldItems / stp.Elapsed.TotalSeconds), 5));

                stp.Restart();
                tree.BulkInsert(sortedItems, opts);
                stp.Stop();
                Console.WriteLine("Bulk    <{0:N0}> items =>  ET : {1}     Speed : {2:N0} item/sec", nNewItems, stp.Elapsed, Math.Round((double)(nNewItems / stp.Elapsed.TotalSeconds), 5));
            }

            Console.ReadLine();
        }

        public static void Test_DuplicateHandelingOptions()
        {
            var options = new BPlusTree<double, string>.OptionsV2(PrimitiveSerializer.Double, PrimitiveSerializer.String);
            options.CalcBTreeOrder(16, 24);
            options.FileBlockSize = 8192;
            options.CreateFile = CreatePolicy.Always;
            options.FileName = "I:\\test.tmp";

            BulkInsertOptions opts = new BulkInsertOptions();
            opts.CommitOnCompletion = true; // check how to properly set this value using Roger examples.
            opts.DuplicateHandling = DuplicateHandling.FirstValueWins;
            opts.InputIsSorted = true;
            opts.ReplaceContents = false;

            var sortedContent = new SortedDictionary<double, string>();
            sortedContent.Add(10.0, "Demah");

            using (var tree = new BPlusTree<double, string>(options))
            {
                tree.Add(10.0, "Hamed");
                tree.BulkInsert(sortedContent, opts);
            }
        }

        struct AddUpdateValue : ICreateOrUpdateValue<double, string>, IRemoveValue<double, string>
        {
            public string oldValue;
            public bool CreateValue(double key, out string value)
            {
                oldValue = null;
                value = "Hamed";
                return true;
            }
            public bool UpdateValue(double key, ref string value)
            {
                oldValue = value;
                return true;
            }
            public bool RemoveValue(double key, string value)
            {
                oldValue = value;
                return true;
            }
        }
    }
}
