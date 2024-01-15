using CAN_Test;
using CAN_Test.ApiCanController;
using System;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
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



            Console.WriteLine(
                "Для запуска CAN введи 1 / Для остановки CAN введи 0\n" +
                "2 - Тест установки HBT и вычитки текущего значения ()\n" +
                "3 - Cчитать HBT и вывести через CAN (write)\n" +
                "4 - Тест записи/чтения в ОС\n" +
                "5 - Тест записи/чтения массива в ОС\n" +
                "6 - Тест чтения/изменения состояний узла\n" +
                "7 - Вывести значение текущего HBT\n" +
                "8 - Тест на устойчивость к спаму включения и выключения\n" +
                "9 - Тест \"склеивания\" виртуального индекса для маппинга PDO\n" +
                "10 - Тест вычитывания сразу 4 RPDO с многократным вызовом"
                );

            ACC.ActivateCanOpen();

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


                    //Тест установки HBT на заданное значение
                    case 2:
                        Console.WriteLine("Введите значение HBT: ");
                        ushort HBT2 = ushort.Parse(Console.ReadLine());

                        ACC.SetHBT(103, HBT2);

                        ACC.GetHBT(103, ref HBT2);
                        Console.WriteLine($"Установленное значение HBT = {HBT2}");
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
                        Console.WriteLine("-------------------------------------------------------");
                        byte subind = 0x1;
                        int[] data4 = new int[20];
                        int FRC = 11;
                        Random r4 = new Random();


                        FRC = ACC.Read(103, 0x6666, 0x1, ref data4);
                        Console.WriteLine($"Статус:{ACC.GetErrorInfo(FRC)}");
                        FRC = ACC.ReadArray(103, 0x6666, out data4);
                        Console.WriteLine($"Статус после первого чтения:{ACC.GetErrorInfo(FRC)}");
                        Console.WriteLine("До записи:");
                        PrintArr(data4);
                        ACC.WriteArray(103, 0x6666, new int[] { r4.Next(100), r4.Next(100), r4.Next(100), r4.Next(100),
                            r4.Next(100), r4.Next(100), r4.Next(100), r4.Next(100) });
                        Console.WriteLine($"Статус после первой записи:{ACC.GetErrorInfo(FRC)}");

                        FRC = ACC.ReadArray(103, 0x6666, out data4);
                        Console.WriteLine($"Статус после 2 чтения:{ACC.GetErrorInfo(FRC)}");
                        Console.WriteLine("После записи:");
                        PrintArr(data4);
                        Console.WriteLine($"Статус:{ACC.GetErrorInfo(FRC)}");

                        Console.WriteLine("-------------------------------------------------------");

                        break;


                    //Тест записи/чтения массива в ОС
                    case 5:
                        Console.WriteLine("-------------------------------------------------------");

                        int[] data5 = new int[20];
                        Random r5 = new Random();


                        FRC = ACC.Read(103, 0x6666, 0x1, ref data5);
                        Console.WriteLine($"Статус:{ACC.GetErrorInfo(FRC)}");
                        FRC = ACC.ReadArray(103, 0x6666, out data5);
                        Console.WriteLine($"Статус после первого чтения:{ACC.GetErrorInfo(FRC)}");
                        Console.WriteLine("До записи:");
                        PrintArr(data5);
                        ACC.WriteArray(103, 0x6666, new int[] { r5.Next(100), r5.Next(100), r5.Next(100), r5.Next(100),
                            r5.Next(100), r5.Next(100), r5.Next(100), r5.Next(100) });
                        Console.WriteLine($"Статус после первой записи:{ACC.GetErrorInfo(FRC)}");

                        FRC = ACC.ReadArray(103, 0x6666, out data5);
                        Console.WriteLine($"Статус после 2 чтения:{ACC.GetErrorInfo(FRC)}");
                        Console.WriteLine("После записи:");
                        PrintArr(data5);
                        Console.WriteLine($"Статус:{ACC.GetErrorInfo(FRC)}");

                        Console.WriteLine("-------------------------------------------------------");

                        break;


                    //Тест чтения/изменения состояний узла
                    case 6:
                        Console.WriteLine("-------------------------------------------------------");

                        var state = ACC.GetDeviceState(103);

                        Thread.Sleep(10);

                        Console.WriteLine($"Состояние до установки:" +
                            $" {ACC.GetDeviceStateInfo((byte)state)}");

                        ACC.SetDeviceState(103, 129);
                        Thread.Sleep(10);

                        state = ACC.GetDeviceState(103);
                        Thread.Sleep(10);
                        Console.WriteLine($"Состояние после его изменения:" +
                            $"{ACC.GetDeviceStateInfo((byte)state)}");

                        Console.WriteLine("-------------------------------------------------------");

                        break;


                    //Тест вывода значения текущего HBT без его изменения
                    case 7:
                        Console.WriteLine("-------------------------------------------------------");
                        ushort HBT = 111;
                        ACC.GetHBT(103, ref HBT);
                        Console.WriteLine($" HBT = {HBT}");
                        Console.WriteLine("-------------------------------------------------------");
                        break;


                    //Тест на устойчивость к спаму включения и выключения
                    case 8:
                        Console.WriteLine("-------------------------------------------------------");
                        int errcode = 0;

                        while (errcode == 0)
                        {
                            ACC.ActivateCanOpen();
                            Thread.Sleep(10);
                            ACC.DisactivateCanOpen();
                        }

                        Console.WriteLine("Произошел сбой!!!");

                        Console.WriteLine("-------------------------------------------------------");

                        break;


                    //Тест "склеивания" виртуального индекса для маппинга PDO
                    case 9:
                        Console.WriteLine("-------------------------------------------------------");

                        Console.Write("Введите индекс в dec: ");
                        ushort Index9 = ushort.Parse(Console.ReadLine());
                        Console.Write("Введите субиндекс: ");
                        byte Subindex9 = byte.Parse(Console.ReadLine());

                        uint Vind = ACC.VirtualIndexBuilder(Index9, Subindex9);
                        Console.WriteLine($"Виртуальный индекс равен: { Convert.ToString(Vind, 16)}");

                        Console.WriteLine("-------------------------------------------------------");

                        break;


                    //Тест вычитывания сразу 4 RPDO c многократным вызовом
                    case 10:
                        Console.WriteLine("-------------------------------------------------------");

                        int PDO10 = 10;
                        byte UPD10 = 0;


                        ACC.CreatePDO(103, 0x6501, 0x01);   
                        ACC.BoundPDO(103, 0x6501, 0x01, 1);

                        ACC.CreatePDO(103, 0x6502, 0x01);
                        ACC.BoundPDO(103, 0x6502, 0x01, 2);

                        ACC.CreatePDO(103, 0x6503, 0x01);
                        ACC.BoundPDO(103, 0x6503, 0x01, 3);

                        ACC.CreatePDO(103, 0x6504, 0x01);
                        ACC.BoundPDO(103, 0x6504, 0x01, 4);

                        for (int i = 0; i < 10; i++)
                        {
                            ACC.ReadPDO(103, 0x6501, 0x01, ref UPD10, ref PDO10);
                            Console.WriteLine($" Значение PDO1 = {PDO10}");
                            Console.WriteLine($" Значение флага обновления = {UPD10}");

                            ACC.ReadPDO(103, 0x6502, 0x01, ref UPD10, ref PDO10);
                            Console.WriteLine($" Значение PDO2 = {PDO10}");
                            Console.WriteLine($" Значение флага обновления = {UPD10}");

                            ACC.ReadPDO(103, 0x6503, 0x01, ref UPD10, ref PDO10);
                            Console.WriteLine($" Значение PDO3 = {PDO10}");
                            Console.WriteLine($" Значение флага обновления = {UPD10}");

                            ACC.ReadPDO(103, 0x6504, 0x01, ref UPD10, ref PDO10);
                            Console.WriteLine($" Значение PDO4 = {PDO10}");
                            Console.WriteLine($" Значение флага обновления = {UPD10}");

                            Console.WriteLine("-------------------------------------------------------");

                        }

                        break;
                }
            }
        }
    }
}