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

            int errorCode = 0;





            //void ActivateCanOpenChannel(byte chanNumber, int cond = 0)
            //{
            //    if (cond == 1)
            //    {
            //        errorCode = CANOpenDll.StartCANMaster(chanNumber, 4);
            //        Thread.Sleep(10);
            //        Console.WriteLine($"Состояние запуска канала {chanNumber} (CanOpen): {errorCode}");
            //    }
            //}

            

            //byte Node = 103;
            //UInt16 Index = 0x6666;
            //byte SubIndex = 0;

            int[] TestArr = new int[] { 99, 98, 97, 96, 95, 94, 93, 92 };
            int[] ReadedTestArr = new int[8];
            int data = 0;





            #region Тест функций CAN Open

            #region Тест чтения массива из OD
            //int FRC = ApiCanController.ReadArray(Node, Index, out TestArr);
            //Console.WriteLine(ApiCanController.GetErrorInfo(FRC));
            //PrintArr(TestArr, Index);
            #endregion

            #region Тест чтения элемента из OD
            //int FRC = ApiCanController.Read(Node, Index, SubIndex, ref data);
            //Console.WriteLine(data);
            #endregion

            #region Тест записи массива в OD

            //Console.WriteLine($"Запись ErrorCode: {ApiCanController.WriteArray(Node, Index, TestArr)}");

            //errorCode = ApiCanController.ReadArray(Node, Index, out ReadedTestArr);
            //Console.WriteLine($"Чтение ErrorCode: {errorCode}");
            //PrintArr(ReadedTestArr, 0x6666);
            #endregion

            #region Тест записи элемента в OD

            // ApiCanController.Write(Node, 0x1017, SubIndex, (ushort)228);


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

            #region MyRegion
            // Первый параметр - номер канала, второй - "кнопка" активации
            // По умолчанию второй параметр имеет значение 0 (канал выкл.), 1 - активировать канал
            //ActivateCanChannel(0);
            //ActivateCanChannel(1, 1);


            //byte[] TestArrayW = new byte[8] { 3, 4, 5, 2, 3, 4, 5, 2 };
            //byte[] TestArrayR = new byte[8];
            //errorCode = ApiCanController.FastWrite(TestArrayW, 0x0123, errorCode);
            //Console.WriteLine($"Состояние выполнения передачи: {errorCode}");

            //errorCode = ApiCanController.FastRead(TestArrayR, errorCode);
            //foreach (byte elem in TestArrayR) { Console.Write($"{elem} "); }


            // Тест HBT
            //ushort HBT = 1111;
            //ushort test = 0;

            //errorCode = ApiCanController.SetHBT(Node, 5000);
            //Console.WriteLine($"FRC = {errorCode}");

            //errorCode = ApiCanController.GetHBT(Node, ref test);
            //Console.WriteLine($"HBT = {test} | FRC = {errorCode}");


            // Тест States'ов

            //Console.WriteLine($"Установка состояния: " +
            //    $"{ApiCanController.GetErrorInfo(ApiCanController.SetDeviceState(Node, 127))}");
            ////Thread.Sleep(10);
            //Console.WriteLine($"State {ApiCanController.GetDeviceState(Node)}");

            // Тест ReadDeviceInfo
            //Console.WriteLine("Активация CANopen:" + ApiCanController.GetErrorInfo(ApiCanController.ActivateCanOpen()));

            //uint Data = 0;
            //errorCode = ApiCanController.ReadDeviceInfo(Node, ref Data);
            //errorCode = ApiCanController.Read(Node, 0x1018, 0x01, ref Data);
            //Console.WriteLine(Data);

            // ТЕст FastRead

            //Console.WriteLine(ApiCanController.GetErrorInfo(ApiCanController.ActivateCan())); 

            //int data = 3452;
            //Console.WriteLine(ApiCanController.GetErrorInfo(ApiCanController.ActivateCanOpen()));
            //ApiCanController.Write(Node, Index, 0x01, data);
            //Console.WriteLine(ApiCanController.GetErrorInfo(ApiCanController.Write(Node, Index, 0x01, data)));

            //Console.WriteLine(errorCode);



            #endregion

            Console.WriteLine(
                "Для запуска CAN введи 111 / Для остановки CAN введи 222\n" +
                "0 - Тест CAN(write);\n" +
                "1 - Установить HBT в 3000 ()\n" +
                "2 - Cчитать HBT и вывести через CAN (write)\n" +
                "3 - Тест записи/чтения в ОС\n" +
                "4 - Тест записи/чтения массива в ОС\n" +
                "5 - Тест чтения/изменения состояний узла\n" +
                "6 - Вывести значение текущего HBT\n" +
                "333 - Тест на устойчивость к спаму включения и выключения"
                );

            byte Node = 103;
            ushort Index = 0x9228;
            byte Subindex = 0x00;
            int PDO = 14;
            byte UPD = 0;


            ACC.ActivateCanOpen();

            ACC.CreatePDO(Node, Index, Subindex);
            ACC.BoundPDO(Node, Index, Subindex, 4);
            Thread.Sleep(500);
            ACC.ReadPDO(Node, Index, Subindex, ref UPD, ref PDO);

            Console.WriteLine($" Значение PDO = {PDO}");

            //while (true)
            //{
            //int? key =  int.Parse(Console.ReadLine());

            //if (key == 111)
            //    {
            //        Console.WriteLine(ACC.ActivateCanOpen() == 0 ? "Успешный запуск": "Какая-то ошибка"); 
            //    }

            //if (key == 222)
            //    {
            //        Console.WriteLine(ACC.DisactivateCanOpen() == 0 ? "CAN дизактивирован" : "Какая-то ошибка");
            //    }

            //if (key == 0)
            //{
            //    #region Test CAN


            //    canmsg wr = new canmsg();
            //    wr.id = 0x0126;
            //    //wr.id = 0x0123;
            //    wr.data = new byte[8] { 0x1, 0x2, 0x3, 0x4,
            //                            0x5, 0x6, 0x7, 0x8 };
            //    wr.len = 8;

            //    ACC.FastWrite(wr.data, wr.id);

            //    #endregion

            //}
            //if(key == 1)
            //{
            //    //ACC.ActivateCanOpen();
            //    ACC.SetHBT(103, 3000);
            //}


            //if (key == 6)
            //    {
            //        byte node = 103;
            //        ushort HBT = 111;
            //        ACC.GetHBT(node, ref HBT);
            //        Console.WriteLine($" HBT = {HBT}");
            //    }


            //if(key == 2)
            //{
            //    ushort MyHBT = 0;
            //    ACC.GetHBT(103, ref MyHBT);
            //    canmsg wr = new canmsg();
            //    wr.id = 0x0126;
            //    //wr.id = 0x0123;
            //    wr.data = new byte[8] { 0x1, 0x2, 0x3, 0x4,
            //                            0x5, 0x6, 0x7, 0x8 };

            //    var sdata = SplitBytes(MyHBT);

            //    wr.data[0] = sdata.LSB;
            //    wr.data[1] = sdata.MSB;
            //    wr.len = 2;

            //    ACC.FastWrite(wr.data, wr.id);

            //}

            //if (key == 3)  
            //{

            //    //ACC.ActivateCanOpen();
            //    byte node = 103;
            //    // Для теста вычитывания данных из несуществующего индекса
            //    //ushort ind = 0x9228;

            //    ushort ind = 0x6666;
            //    byte subind = 0x1;
            //    int dan = 0;

                   
                    
            //    int FRC = ACC.Read(node, ind, subind, ref dan);
            //    Console.WriteLine("Данные до изменения "+ dan);
            //    ACC.Write(node, ind, subind, 13);


            //    FRC = ACC.Read(node, ind, subind, ref dan);
            //    Console.WriteLine("Данные после изменения " + dan);
            //    Console.WriteLine(ACC.GetErrorInfo(FRC));

            //    //ACC.DisactivateCanOpen();
            //}

            //if (key == 4)
            //{
            //    //ACC.ActivateCanOpen();

            //    byte node = 103;
            //    // Для теста вычитывания данных из несуществующего индекса
            //    //ushort ind = 0x9228;
            //    ushort ind = 0x6666;
            //    byte subind = 0x1;
            //    int[] dan = new int[20];
            //    int FRC = 11;
            //    FRC = ACC.Read(node, ind, subind, ref dan);
            //    Console.WriteLine(ACC.GetErrorInfo(FRC));
            //    FRC = ACC.ReadArray(node, ind, out dan);
            //    Console.WriteLine($"FRC после первого чтения = {FRC}");
            //    Console.WriteLine("До записи:");
            //    PrintArr(dan);
            //    ACC.WriteArray(node, ind, new int[] {228, 222, 333, 444, 666, 1, 8, 9 });
            //    Console.WriteLine($"FRC после первой записи = {FRC}");

            //    FRC = ACC.ReadArray(node, ind, out dan);
            //    Console.WriteLine($"FRC после 2 чтения= {FRC}");
            //    Console.WriteLine("После записи:");
            //    PrintArr(dan);
            //    Console.WriteLine($"FRC = {FRC}");

            //    }


            //if (key == 5)
            //{

            //    var state = ACC.GetDeviceState(103);
            //    Thread.Sleep(10);
            //    Console.WriteLine(ACC.GetDeviceStateInfo((byte)state));

            //    ACC.SetDeviceState(103, 129);
            //    Thread.Sleep(10);
            //    state = ACC.GetDeviceState(103);
            //    Thread.Sleep(10);
            //    Console.WriteLine(ACC.GetDeviceStateInfo((byte)state));
            //}

            //if (key == 333)
            //    {
            //        while (true)
            //        {
            //            ACC.ActivateCanOpen();
            //            //Thread.Sleep(10);
            //            ACC.DisactivateCanOpen();
            //        }

            //        Console.WriteLine("Произошел сбой!!!");

            //    }
                

            //}

            
        

 
            #region MyRegion

            //canmsg rd = new canmsg();

            //rd.id = 0x0123;
            //wr.data = new byte[8] { 0x1, 0x2, 0x3, 0x4,
            //                        0x5, 0x6, 0x7, 0x8 };
            //wr.len = 8;

            //int pdo = 13;
            //byte Upd = 0;

            //errorCode = ApiCanController.ReadPDO(Node, Index, SubIndex, ref Upd, ref pdo);
            //Console.WriteLine("Статус выполнения:" + ApiCanController.GetErrorInfo(errorCode));

            //Console.WriteLine("Данные: " + pdo);
            //Console.WriteLine("UPD = " + Upd); 
            #endregion



        }
    }
}