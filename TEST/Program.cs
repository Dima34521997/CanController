using CAN_Test;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using static CAN_Test.CANOpenDll;

namespace TEST

{
    internal class Program
    {
        public static void PrintArr<T>(UInt16 Index, T[] arr)
        {
            Console.WriteLine($"Считанный массив данных по индексу 0x{Index:X}:");
            foreach(T b in arr) Console.Write($"{b} ");
            Console.WriteLine("\n");
        }










        static void Main(string[] args)
        {
            //int errorCode = CANOpenDll.StartCANMaster(1, 4);
            int errorCode = CANOpenDll.StartCANMaster(0, 4);
            //Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            //stopwatch.Start();

            const uint mask = 0x0; const byte chan0 = 0x0;
            const uint code = 0x0; const byte chan1 = 0x1;

            //int errorCode = CHAICanDLL.CanInit();

            //#region setup channels
            //void ActivateChan0(int activateChan = 0)
            //{
            //    if (activateChan == 1)
            //    {
            //        errorCode = CHAICanDLL.CanOpen(chan0, 0x2);
            //        Console.WriteLine("Канал 0:" + errorCode);

            //        errorCode = CHAICanDLL.CanSetBaud(chan0, bt0: 0x03, bt1: 0x1c);
            //        Console.WriteLine("Канал 0 Бод-Рейт:" + errorCode);

            //        errorCode = CHAICanDLL.CanStart(0);
            //        Console.WriteLine("Открытие канала 0:" + errorCode);
            //    }
            //}


            //void ActivateChan1(int activateChan = 0)
            //{
            //    if (activateChan == 1)
            //    {
            //        errorCode = CHAICanDLL.CanOpen(chan1, 0x2);
            //        Console.WriteLine("Канал 1:" + errorCode);

            //        errorCode = CHAICanDLL.CanSetBaud(chan1, bt0: 0x03, bt1: 0x1c);
            //        Console.WriteLine("Канал 1 Бод-Рейт:" + errorCode);

            //        errorCode = CHAICanDLL.CanStart(1);
            //        Console.WriteLine("Открытие канала 1:" + errorCode);
            //    }
            //}
            //#endregion

            //ActivateChan0(1);
            //ActivateChan1(1);

            byte Node = 103;
            UInt16 Index = 0x6666;
            byte SubIndex = 0x04;

            UInt16 arrLen = SDOcommunication.GetLengthOfArray(Node, Index);
            int[] data_Arr = new int[arrLen];
            Int32 data = 8;

            #region Тест функций CAN Open

            #region Тест чтения массива из OD
            //SDOcommunication.ReadArraySDO(Node, Index, data_Arr);
            //PrintArr(0x6666, data_Arr);
            #endregion

            #region Тест чтения элемента из OD
            //data = SDOcommunication.ReadSDO(Node, Index, SubIndex, data);
            //Console.WriteLine($"Считанный элемент: {data}");
            #endregion

            #region Тест записи массива в OD
            //SDOcommunication.WriteArraySDO(Node, Index, data_Arr);
            //SDOcommunication.ReadArraySDO(103, Index, data_Arr);
            //PrintArr(0x6666, data_Arr);
            #endregion

            #region Тест записи элемента в OD

            //SDOcommunication.WriteSDO(Node, Index, SubIndex, data);
            //SDOcommunication.ReadArraySDO(Node, Index, data_Arr);
            //PrintArr(0x6666, data_Arr);
            #endregion

            #endregion

            #region Тест CANOpen + CAN
            //Write message
            //canmsg_t canmsgW = new canmsg_t();
            //canmsgW.id = 0x0126;
            //canmsgW.data = new byte[8] { 0x1, 0x2, 0x3, 0x4,
            //                0x5, 0x6, 0x7, 0x8 };
            //canmsgW.len = 8;
            //errorCode = CHAICanDLL.CanWrite(0, canmsgW);
            //Console.WriteLine("Отправка кадра: " + errorCode);

            ////Read message
            //canmsg_t canmsgRarr = new canmsg_t();
            //errorCode = CHAICanDLL.CanRead(1, ref canmsgRarr);
            //Console.WriteLine("Чтение: " + errorCode);
            //foreach (byte t in canmsgRarr.data) Console.Write($"{t} ");

            canmsg_t canmsgW = new canmsg_t();
            canmsgW.id = 0x0126;
            canmsgW.data = new byte[8]; //{ 0x1,02,03,04,05,06,07,08 };
            canmsgW.len = 1;
            errorCode = CHAICanDLL.CanWrite(0, canmsgW);
            Console.WriteLine("Отправка кадра: " + errorCode);

            #endregion
        }
    }
}