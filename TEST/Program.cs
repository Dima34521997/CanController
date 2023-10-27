﻿using CAN_Test;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using static CAN_Test.CANOpenDll;

namespace TEST

{
    internal class Program
    {
        public static void PrintArr<T>(T[] arr, UInt16 Index=0)
        {
            if (Index != 0)
            {
                Console.WriteLine($"Считанный массив данных по индексу 0x{Index:X}:");
                foreach (T b in arr) Console.Write($"{b} ");
                Console.WriteLine("\n");
            }
            else
            {
                Console.WriteLine($"Считанный массив данных:");
                foreach (T b in arr) Console.Write($"{b} ");
                Console.WriteLine("\n");
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Расшифровка состояний:\n" +
                "  0 - успешное выполнение\n< 0 - код ошибки");
            Console.WriteLine("--------------------------------");

            int errorCode = 0;

            //errorCode = CHAICanDLL.CanInit();
            //Console.WriteLine($"CHAI Init: {errorCode}");

            void ActivateCanChannel(byte chanNumber, int cond=0)
            {
                if (cond == 1)
                {
                    Console.WriteLine("--------------------------------");

                    errorCode = CHAICanDLL.CanOpen(chanNumber, 0x2);
                    Console.WriteLine($"Канал {chanNumber}: " + errorCode);

                    errorCode = CHAICanDLL.CanSetBaud(chanNumber, bt0: 0x03, bt1: 0x1c);
                    Console.WriteLine($"Канал {chanNumber} Бод-Рейт: " + errorCode);

                    errorCode = CHAICanDLL.CanStart(chanNumber);
                    Console.WriteLine($"Открытие канала {chanNumber}: " + errorCode);
                }
            }

            
            void ActivateCanOpenChannel(byte chanNumber, int cond = 0)
            {
                if (cond == 1)
                {
                    errorCode = CANOpenDll.StartCANMaster(chanNumber, 4);
                    Thread.Sleep(10);
                    Console.WriteLine($"Состояние запуска канала {chanNumber} (CanOpen): {errorCode}");
                }
            }

            ActivateCanOpenChannel(0,1);

            byte Node = 103;
            UInt16 Index = 0x6666;
            byte SubIndex = 0x04;


            //UInt16 arrLen = ApiCanController.GetLengthOfArray(Node, Index);
            int[] dataArr = new int[8] {9,8,7,6,5,4,3,2};
            int[] dataArrReaded = new int[4];
            int data = 112;

            #region Тест функций CAN Open

            #region Тест чтения массива из OD
            //ApiCanController.ReadArray(Node, Index, out dataArr, errorCode);
            //PrintArr(dataArr, Index);
            #endregion

            #region Тест чтения элемента из OD
            //data = ApiCanController.Read(Node, Index, SubIndex, data);
            //Console.WriteLine($"Считанный элемент: {data}");
            #endregion

            #region Тест записи массива в OD
            //ApiCanController.WriteArray(Node, Index, dataArr, errorCode);
            //ApiCanController.ReadArray(Node, Index, out dataArrReaded, errorCode);
            //PrintArr(dataArrReaded, 0x6666);
            #endregion

            #region Тест записи элемента в OD

            //ApiCanController.Write(Node, Index, SubIndex, data);
            //ApiCanController.ReadArray(Node, Index, dataArrReaded);
            //PrintArr(0x6666, dataArrReaded);
            #endregion

            #endregion

            #region Тест CANOpen + CAN

            //Console.WriteLine("--------------------------------");
            //canmsg wr = new canmsg();
            //wr.id = 0x0126;
            ////wr.id = 0x0123;
            //wr.data = new byte[8] { 0x1, 0x2, 0x3, 0x4,
            //                        0x5, 0x6, 0x7, 0x8 };
            //wr.len = 8;
            //errorCode = CHAICanDLL.CanWrite(0, wr);
            //Console.WriteLine("Отправка кадра: " + errorCode);

            //Console.WriteLine("--------------------------------");

            //Thread.Sleep(100);

            //canmsg rd = new canmsg();
            //while (true)
            //{
            //    errorCode = CHAICanDLL.CanRead(1, ref rd);
            //    if (errorCode == (int)CHAICodes.ECIOK) break;
            //}
            //Console.WriteLine("Считанные данные:");
            //foreach (byte elem in rd.data) Console.Write($"{elem} ");

            #endregion

            // Первый параметр - номер канала, второй - "кнопка" активации
            // По умолчанию второй параметр имеет значение 0 (канал выкл.), 1 - активировать канал
            ActivateCanChannel(0);
            ActivateCanChannel(1, 1);


            //byte[] TestArrayW = new byte[8] { 3, 4, 5, 2, 3, 4, 5, 2 };
            //byte[] TestArrayR = new byte[8];
            //errorCode = ApiCanController.FastWrite(TestArrayW, 0x0123, errorCode);
            //Console.WriteLine($"Состояние выполнения передачи: {errorCode}");

            //errorCode = ApiCanController.FastRead(TestArrayR, errorCode);
            //foreach (byte elem in TestArrayR) { Console.Write($"{elem} "); }


            // Тест HBT

            data = ApiCanController.GetHBT(Node, data, errorCode);
            Console.WriteLine($"HBT = {data}");

        }
    }
}