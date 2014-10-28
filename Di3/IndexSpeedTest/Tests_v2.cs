using System;
using System.IO;
using System.Diagnostics;
using DI3;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Di3BMain;
using System.Collections.Generic;

namespace IndexSpeedTest
{
    class Tests_v2
    {
        IndexSpeedTest_v2 SpeedTest = new IndexSpeedTest_v2();

        string path = @"D:\VahidTest";//Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
        Int32Comparer int32Comparer = new Int32Comparer();

        public void Test_01()
        {
            string testTitle = "Test_01";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_02()
        {
            string testTitle = "Test_02";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.MinimumChildNodes = 12;
            options.MaximumChildNodes = 32;
            options.MinimumValueNodes = 12;
            options.MaximumValueNodes = 32;

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_03()
        {
            string testTitle = "Test_03";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_04()
        {
            string testTitle = "Test_04";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.MinimumChildNodes = 2;
            options.MaximumChildNodes = 256;
            options.MinimumValueNodes = 2;
            options.MaximumValueNodes = 256;

            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_05()
        {
            string testTitle = "Test_05";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_06()
        {
            string testTitle = "Test_06";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_07()
        {
            string testTitle = "Test_07";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                600,//150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_08()
        {
            string testTitle = "Test_08";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_09()
        {
            string testTitle = "Test_09";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                150,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_10()
        {
            string testTitle = "Test_10";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_11()
        {
            string testTitle = "Test_11";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_12()
        {
            string testTitle = "Test_12";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_13()
        {
            string testTitle = "Test_13";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_14()
        {
            string testTitle = "Test_14";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_15()
        {
            string testTitle = "Test_15";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_16()
        {
            string testTitle = "Test_16";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 8192;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_17()
        {
            string testTitle = "Test_17";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_18()
        {
            string testTitle = "Test_18";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 1024;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_19()
        {
            string testTitle = "Test_19";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 196;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_20()
        {
            string testTitle = "Test_20";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 96;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_21()
        {
            string testTitle = "Test_21";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            //options.AverageKeySize = 4;
            //options.AverageValueSize = 96;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 2;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_22()
        {
            string testTitle = "Test_22";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.LogFileInCache;

            options.Locking = LockMode.SimpleReadWriteLocking;


            SpeedTest.Run(
                100,
                200000,
                false,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_23()
        {
            string testTitle = "Test_23";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 2;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_22()
        {
            string testTitle = "Test_22";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.AverageKeySize = 4;
            options.AverageValueSize = 32;
            options.FileBlockSize = 4096;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.LogFileInCache;

            options.Locking = LockMode.SimpleReadWriteLocking;


            SpeedTest.Run(
                100,
                200000,
                false,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_23()
        {
            string testTitle = "Test_23";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 2;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_24()
        {
            string testTitle = "Test_24";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 4;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 4;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_25()
        {
            string testTitle = "Test_25";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 16;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 16;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_26()
        {
            string testTitle = "Test_26";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 32;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 32;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_27()
        {
            string testTitle = "Test_27";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 32;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 2;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_28()
        {
            string testTitle = "Test_28";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.FileBlockSize = 512;

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.WriterOnlyLocking;

            options.MaximumChildNodes = 256;
            options.MinimumChildNodes = 2;

            options.MaximumValueNodes = 256;
            options.MinimumValueNodes = 32;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }
        public void Test_29()
        {
            string testTitle = "Test_29";

            Di3Options<int> options = new Di3Options<int>(
                path + Path.DirectorySeparatorChar + "Di3_" + testTitle + ".idx",
                CSharpTest.Net.Collections.CreatePolicy.IfNeeded,
                PrimitiveSerializer.Int32, int32Comparer);

            options.CachePolicy = CachePolicy.Recent;

            options.StoragePerformance = StoragePerformance.Fastest;

            options.Locking = LockMode.SimpleReadWriteLocking;

            options.AverageKeySize = 4;
            options.AverageValueSize = 64;

            SpeedTest.Run(
                100,
                200000,
                true,
                path,
                testTitle,
                50,
                500,
                500,
                1000,
                options,
                Mode.SinglePass);
        }

    }
}

