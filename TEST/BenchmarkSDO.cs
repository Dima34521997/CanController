using BenchmarkDotNet.Attributes;
using CAN_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEST
{
     public class BenchmarkSDO
    {

        public static void PrintArr<T>(T[] arr)
        {
            foreach (T b in arr) Console.Write($"{b} ");
            Console.WriteLine("\n");
        }

        [Benchmark]
        public static  void Test()
        {
            CANOpenDll.StartCANMaster(0, 4);

            ulong data = 0;
            byte Node = 99;
            UInt16 Index = 0x6100;
            byte SubIndex = 7;

            //UInt16 ArrayLength = SDO.GetLengthOfArray(Node, Index);
            //Console.WriteLine($"Длина массива = {ArrayLength}");
            Console.WriteLine("");

            Int32[] arr = new Int32[] { 0, 0, 0, 0, 0, 0, 0, 0 };




            Console.WriteLine("Read T data");
            arr = SDOcommunication.ReadArraySDO(Node, Index, SubIndex, arr);
            Console.WriteLine(new String('-', 35));

            PrintArr(arr);

            /*
            SDO.WriteArraySDO(Node, Index, arr);
            Console.Write($"Записанный массив: ");
            PrintArr(arr);

            SDO.ReadArraySDO(Node, Index, out arr);
            Console.Write("Считанный массив: ");
            PrintArr(arr);
            */

            //PrintArr(dataArr);

            //Console.WriteLine($"Размерность массива: {ArrayLength}");

            //Console.Write($"Считанное значение data = ");
            //foreach (byte i in data) Console.Write($"{i} ");
            //Console.WriteLine("\n");
        }
    }
}
