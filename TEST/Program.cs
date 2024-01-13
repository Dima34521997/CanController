using CAN_Test;
using CAN_Test.ApiCanController;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using static CAN_Test.CANOpenDll;

namespace TEST

{
    internal class Program
    {
        static public (byte LSB, byte MSB) SplitBytes(UInt16 value)
        {
            byte LSB = (byte)value;
            byte MSB = (byte)(value >> 8);
            var result = (LSB: LSB, MSB: MSB);
            return result;
        }


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




        static void  Main(string[] args)
        {
            ApiCanController ACC = new ApiCanController();


            Console.WriteLine("Расшифровка состояний:\n" +
                "  0 - успешное выполнение\n< 0 - код ошибки");

            Console.WriteLine("--------------------------------");

            Console.WriteLine(
                "Для запуска CAN введи 1 / Для остановки CAN введи 0\n" +
                "2 - Тест установки HBT и вычитки текущего значения ()\n" +
                "3 - Cчитать HBT и вывести через CAN (write)\n" +
                "4 - Тест записи/чтения в ОС\n" +
                "5 - Тест записи/чтения массива в ОС\n" +
                "6 - Тест чтения/изменения состояний узла\n" +
                "7 - Вывести значение текущего HBT\n" +
                "8 - Тест на устойчивость к спаму включения и выключения\n" +
                "9 - Тест методов PDO"
                );



            while (true)
            {
                int? Test = int.Parse(Console.ReadLine());

                switch (Test)
                { 
                    //Тест запуска CanOpen
                    case 1: 
                        Console.WriteLine(ACC.ActivateCanOpen() == 0 ?
                            "Успешный запуск" : "Какая-то ошибка, проверь подключение контроллера");
                        break;


                    //Тест остановки CanOpen
                    case 0:
                        Console.WriteLine(ACC.DisactivateCanOpen() == 0 ?
                            "CAN дизактивирован" : "Какая-то ошибка");
                        break;


                    //Тест установки HBT и вычитки текущего значения
                    case 2:
                        ushort HBT2 = 228;

                        ACC.SetHBT(103, 3000);

                        Console.WriteLine(ACC.GetHBT(103, ref HBT2));
                        break;


                    //Тест считывания HBT и вывода через CAN (write)
                    case 3:
                        ushort MyHBT = 0;
                        ACC.GetHBT(103, ref MyHBT);

                        canmsg wr = new canmsg();
                        wr.id = 0x0126;
                        //wr.id = 0x0123;
                        wr.data = new byte[8] { 0x1, 0x2, 0x3, 0x4,
                                                0x5, 0x6, 0x7, 0x8 };

                        var sdata = SplitBytes(MyHBT);

                        wr.data[0] = sdata.LSB;
                        wr.data[1] = sdata.MSB;
                        wr.len = 2;

                        ACC.FastWrite(wr.data, wr.id);

                        break;


                    //Тест записи/чтения в ОС
                    case 4:
                        byte subind = 0x1;
                        int[] data4 = new int[20];
                        int FRC = 11;

                        FRC = ACC.Read(103, 0x6666, 0x1, ref data4);
                        Console.WriteLine(ACC.GetErrorInfo(FRC));
                        FRC = ACC.ReadArray(103, 0x6666, out data4);
                        Console.WriteLine($"FRC после первого чтения = {FRC}");
                        Console.WriteLine("До записи:");
                        PrintArr(data4);
                        ACC.WriteArray(103, 0x6666, new int[] { 228, 222, 333, 444, 666, 1, 8, 9 });
                        Console.WriteLine($"FRC после первой записи = {FRC}");

                        FRC = ACC.ReadArray(103, 0x6666, out data4);
                        Console.WriteLine($"FRC после 2 чтения= {FRC}");
                        Console.WriteLine("После записи:");
                        PrintArr(data4);
                        Console.WriteLine($"FRC = {FRC}");

                        break;


                    //Тест записи/чтения массива в ОС
                    case 5:
                        int[] data5 = new int[20];

                        FRC = ACC.Read(103, 0x6666, 0x1, ref data5);
                        Console.WriteLine(ACC.GetErrorInfo(FRC));
                        FRC = ACC.ReadArray(103, 0x6666, out data5);
                        Console.WriteLine($"FRC после первого чтения = {FRC}");
                        Console.WriteLine("До записи:");
                        PrintArr(data5);
                        ACC.WriteArray(103, 0x6666, new int[] { 228, 222, 333, 444, 666, 1, 8, 9 });
                        Console.WriteLine($"FRC после первой записи = {FRC}");

                        FRC = ACC.ReadArray(103, 0x6666, out data5);
                        Console.WriteLine($"FRC после 2 чтения= {FRC}");
                        Console.WriteLine("После записи:");
                        PrintArr(data5);
                        Console.WriteLine($"FRC = {FRC}");

                        break;


                    //Тест чтения/изменения состояний узла
                    case 6:
                        var state = ACC.GetDeviceState(103);

                        Thread.Sleep(10);

                        Console.WriteLine(ACC.GetDeviceStateInfo((byte)state));

                        ACC.SetDeviceState(103, 129);
                        Thread.Sleep(10);

                        state = ACC.GetDeviceState(103);
                        Thread.Sleep(10);
                        Console.WriteLine(ACC.GetDeviceStateInfo((byte)state));

                        break;


                    //Тест вывода значения текущего HBT без его изменения
                    case 7:
                        ushort HBT = 111;
                        ACC.GetHBT(103, ref HBT);
                        Console.WriteLine($" HBT = {HBT}");
                        break;


                    //Тест на устойчивость к спаму включения и выключения
                    case 8:
                        int errcode = 0;

                        while (errcode == 0)
                        {
                            ACC.ActivateCanOpen();
                            Thread.Sleep(10);
                            ACC.DisactivateCanOpen();
                        }

                        Console.WriteLine("Произошел сбой!!!");
                        break;


                    //Тест методов PDO
                    case 9:
                        int PDO = 14;
                        byte UPD = 0;

                        ACC.CreatePDO(103, 0x9228, 0x01);
                        ACC.BoundPDO(103, 0x9228, 0x01, 4);
                        Thread.Sleep(500);
                        ACC.ReadPDO(103, 0x9228, 0x01, ref UPD, ref PDO);

                        Console.WriteLine($" Значение PDO = {PDO}");
                        Console.WriteLine($" Значение флага обновления = {UPD}");

                        break;
                }   
            }
        }
    }
}