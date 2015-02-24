using CSharpTest.Net.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polimi.DEIB.VahidJalili.DI3.BasicOperations.IndexFunctions
{
    internal class IndexesCardinality
    {
        public IndexesCardinality(BPlusTree<string, int> di3_info)
        {
            _di3_info = di3_info;
        }

        private BPlusTree<string, int> _di3_info { set; get; }
        private AddUpdateValue update = new AddUpdateValue();

        public void AddOrUpdate(string key, int value)
        {
            update.newValue = value;
            _di3_info.AddOrUpdate(key, ref update);
        }
        public void GetValue(string key, out int value)
        {
            _di3_info.TryGetValue(key, out value);
        }


        private struct AddUpdateValue : ICreateOrUpdateValue<string, int>, IRemoveValue<string, int>
        {
            public int oldValue { set; get; }
            public int newValue { set; get; }

            public bool CreateValue(string key, out int value)
            {
                oldValue = 0;
                value = newValue;
                return true;
            }

            public bool UpdateValue(string key, ref int value)
            {
                oldValue = value;
                value = newValue + oldValue;
                return value == oldValue;
            }

            public bool RemoveValue(string key, int value)
            {
                oldValue = value;
                return true;
            }
        }
    }
}
