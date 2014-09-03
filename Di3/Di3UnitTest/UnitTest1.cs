using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DI3;

namespace Di3UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Di3<INTCoordinate, int, int> di3 = new Di3<INTCoordinate, int, int>();
        }

        
    }

    public class C : ICoordinate<int>
    {

        public char CompareTo(int that)
        {
            throw new NotImplementedException();
        }

        public int defaultValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public class INTCoordinate : ICoordinate<int>
    {
        public char CompareTo(int that)
        {
            throw new NotImplementedException();
        }

        public int defaultValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

}
