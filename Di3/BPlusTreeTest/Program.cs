using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPlusTreeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //BasicTest();

            // A Runtime test for multiple insertions. 
            //TimeSpan elapsed = BasicInsertionSpeedTest(500000000);
            //double millisec = elapsed.TotalSeconds;

            //LargeInsertion.Sequence("D:\\VahidTest\\", "BPlusTreeTest_LargeInsertion__Sequence");
            //LargeInsertion.RandomOverlaps("D:\\VahidTest\\", "BPlusTreeTest_LargeInsertion__RandomOverlaps");

            //BulkInsertTest.Run();
            //BulkInsertTest.Test_DuplicateHandelingOptions();

            //RangeInsertTest.Run();
            RangeInsertTest.Test_AllowUpdates();
        }

        private static void BasicTest()
        {
            BPlusTree<double, string>.OptionsV2 options =
                new BPlusTree<double, string>.OptionsV2(PrimitiveSerializer.Double, PrimitiveSerializer.String);

            options.CalcBTreeOrder(16, 24);
            options.CreateFile = CreatePolicy.Always;
            options.FileName = System.IO.Path.GetTempFileName();
            using (var tree = new BPlusTree<double, string>(options))
            {
                // Insertion to tree.
                // Note: numbers are NOT inserted sorted.
                tree.Add(30.1, "30.2");
                tree.Add(10.1, "10.2");
                tree.Add(20.1, "20.2");
                tree.Add(80.1, "80.2");
                tree.Add(40.1, "40.2");
                tree.Add(60.1, "60.2");
                tree.Add(70.1, "70.2");
                tree.Add(50.1, "50.2");




                // To get first element.
                // Since sorted, first element is: 10.1
                KeyValuePair<double, string> first_with_Try;
                tree.TryGetFirst(out first_with_Try);

                // Similar to previous function.
                var first = tree.First();




                // To get last element.
                // Since sorted, last element is: 80.1
                KeyValuePair<double, string> last_with_Try;
                tree.TryGetLast(out last_with_Try);

                // Similar to previous function.
                var last = tree.Last();




                // Given key get the value. 
                // Key is valid, region.e., it is available in tree.
                // Hence it returns: "50.2"
                string value_of_valid_key;
                tree.TryGetValue(50.1, out value_of_valid_key);


                // Given key get the value.
                // Key is invalid, region.e., it is NOT available in tree.
                // Hence it returns: null (default value of int)
                string value_of_invalid_key;
                tree.TryGetValue(55, out value_of_invalid_key);




                // Runtime error
                //var list = tree.ToList();


                // Gets an enumerator.
                IEnumerator<KeyValuePair<double, string>> enumerator = tree.GetEnumerator();


                // Iterating through items with enumerator.
                // starting from first item to last. 
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                }


                // Another syntac of iterations, which is automatically
                // calling "GetEnumerator" function.
                // starting from first item to last.
                foreach (var item in tree)
                {

                }


                // Iterates through items starting from given key: 40.1 (inclusively) 
                // and goes till the last item.
                foreach (var item in tree.EnumerateFrom(39.1))
                {

                }




                // Iterates through items starting from given index: 2 (inclusively)
                // and goes till the last item.
                foreach (var item in tree.EnumerateFrom(tree.ElementAtOrDefault(2).Key))
                {

                }



                // Iterate from an item that is NOT available in collection
                // to the item which is neither available.
                foreach (var item in tree.EnumerateRange(20.5, 40.9))
                {

                }


                // Gets the item at specific index. 
                // All return valid values, but the last one which is
                // refereing to an index out-of-bound; the return of this 
                // call is the default value for key and value. 
                var element_at_0 = tree.ElementAtOrDefault(0);
                var element_at_1 = tree.ElementAtOrDefault(1);
                var element_at_2 = tree.ElementAtOrDefault(2);
                var element_at_3 = tree.ElementAtOrDefault(3);
                var element_at_100 = tree.ElementAtOrDefault(100);


                using (BPlusTree<double, string> data = new BPlusTree<double, string>(options))
                {
                    bool sT1 = data.TryAdd(1, "a");
                    bool sF1 = data.TryAdd(1, "a");

                    data[1] = "did it";

                    bool sT2 = data.TryUpdate(1, "a");
                    bool sT3 = data.TryUpdate(1, "c");
                    bool sT4 = data.TryUpdate(1, "d", "c");
                    bool sF2 = data.TryUpdate(1, "f", "c");
                    bool equality1 = "d".Equals(data[1]);
                    bool sT5 = data.TryUpdate(1, "a", data[1]);
                    bool equality2 = "a".Equals(data[1]);
                    bool sF3 = data.TryUpdate(2, "b");

                    string val;
                    bool st6 = data.TryRemove(1, out val) && val == "a";
                    bool sF4 = data.TryRemove(2, out val);
                    bool notEqual = val.Equals("a");
                }
            }
        }

        private static TimeSpan BasicInsertionSpeedTest(int inputSize)
        {
            Stopwatch watch = new Stopwatch();

            Random rnd = new Random();

            string longString = "";
            for (int i = 0; i < 300; i++)
                longString += (char)rnd.Next(48, 90);

            int longStringSize = longString.Length * sizeof(Char);

            BPlusTree<double, string>.OptionsV2 options =
                new BPlusTree<double, string>.OptionsV2(PrimitiveSerializer.Double, PrimitiveSerializer.String);

            //options.CalcBTreeOrder(16, 20);
            options.CalcBTreeOrder(16, longStringSize);


            // to write to RAM:
            //options.CreateFile = CreatePolicy.Never;


            // to write to disk:
            options.CreateFile = CreatePolicy.Always;
            options.FileName = Path.GetTempFileName();


            using (var tree = new BPlusTree<double, string>(options))
            {
                watch.Start();

                while (inputSize-- > 0)
                {
                    Console.Write("\rRemaining: {0}", inputSize.ToString("N0", CultureInfo.InvariantCulture));
                    tree.Add(inputSize, longString);
                }

                watch.Stop();
            }



            return watch.Elapsed;
        }

        private static void TEST_AddOrUpdate()
        {
            BPlusTree<int, string>.OptionsV2 options = new BPlusTree<int, string>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.String);
            options.CreateFile = CreatePolicy.Never;

            AddUpdateValue update = new AddUpdateValue();

            var tree = new BPlusTree<int, string>(options);

            update.Value = "Hamed";

            //tree[1] = "a";
            tree.AddOrUpdate(1, ref update);
            tree.AddOrUpdate(2, ref update);

            update.Value = "Vahid";
            tree.AddOrUpdate(2, ref update);

            update.Value = "New";
            tree.AddOrUpdate(3, ref update);
        }

        struct AddUpdateValue : ICreateOrUpdateValue<int, string>, IRemoveValue<int, string>
        {
            public string OldValue;
            public string Value;
            public bool CreateValue(int key, out string value)
            {
                OldValue = null;
                value = Value;
                return Value != null;
            }
            public bool UpdateValue(int key, ref string value)
            {
                OldValue = value;
                value = Value;
                return Value != null;
            }
            public bool RemoveValue(int key, string value)
            {
                OldValue = value;
                return value == Value;
            }
        }
    }
}
