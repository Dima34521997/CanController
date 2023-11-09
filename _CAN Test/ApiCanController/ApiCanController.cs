using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CAN_Test.ApiCanController
{
    public class ApiCanController : IApiCanController
    {
        static byte CanOpenPort = 0;
        static byte CanPort = 1;

        public uint DeviceCanId;


        public int Write<T>(byte Node, ushort Index, byte SubIndex, T Data)
        {
            int FRC;
            T NewData = Data;
            uint DataSize = (ushort)TypeSize<T>.Size;

            unsafe
            {
                FRC = CANOpenDll.
                    write_device_object_sdo(Node, Index, SubIndex, (byte*)&NewData, DataSize);

            }

            if (FRC != 0) throw new Exception("Ошибка записи");
            return FRC;
        }


        public int WriteArray<T>(byte Node, ushort Index, T[] Data)
        {
            int FRC = 0;
            T[] NewData = Data;
            ushort ArraySize = GetLengthOfArray(Node, Index);
            for (byte subIndex = 1; subIndex <= ArraySize; subIndex++)
            {
                FRC = Write(Node, Index, subIndex, Data[subIndex - 1]);
                Thread.Sleep(10);
            }
            return FRC;
        }



        public int Read<T>(byte Node, ushort Index, byte SubIndex, ref T Data)
        {
            int FRC = 0;
            uint DataSize = (ushort)TypeSize<T>.Size;
            T NewData = Data;
            unsafe
            {
                FRC = CANOpenDll.read_device_object_sdo(Node, Index, SubIndex, (byte*)&NewData, ref DataSize);
            }
            if (FRC != 0 && Data == null)
                throw new Exception("Попытка прочесть пустой индекс");
            Data = NewData;
            return FRC;
        }


        public int ReadArray<T>(byte Node, ushort Index, out T[] DataArr)
        {
            int FRC = 0;
            uint DataSize = (ushort)TypeSize<T>.Size;
            ushort ArraySize = GetLengthOfArray(Node, Index);

            T[] NewDataArr = new T[ArraySize];

            for (byte index = 0; index < ArraySize; index++)
            {
                FRC = Read(Node, Index, (byte)(0x01 + index), ref NewDataArr[index]);
            }
            if (FRC != 0 && NewDataArr == null) // типа пустой индекс
                throw new Exception($"Попытка прочесть пустой индекс. Код ошибки {FRC}");

            DataArr = NewDataArr;
            return FRC;
        }


        ushort GetLengthOfArray(byte Node, ushort Index)
        {
            ushort Size = 0;
            Read(Node, Index, 0, ref Size);
            return Size;
        }


        public int FastWrite(byte[] Data, uint ID)
        {
            int FRC = 0;
            canmsg wr = new canmsg();
            wr.id = ID;
            wr.data = Data;
            wr.len = (byte)Data.Length;
            FRC = CHAICanDLL.CanWrite(CanPort, wr);
            Thread.Sleep(10);
            return FRC;
        }

        public int FastRead(byte[] Data)
        {
            int FRC = 0;
            canmsg rd = new canmsg();
            while (true)
            {
                FRC = CHAICanDLL.CanRead(CanPort, ref rd);
                if (FRC == (int)CHAICodes.ECIOK) break;
            }

            for (int i = 0; i < rd.data.Length; i++) { Data[i] = rd.data[i]; }
            return FRC;
        }

        public int GetHBT<T>(byte Node, ref ushort HBT) => Read(Node, 0x1017, 0x0000, ref HBT);


        public int SetHBT(byte Node, ushort HBT) => Write(Node, 0x1017, 0x000, HBT);

        public int ActivateCan()
        {
            int FRC = 0;
            unsafe
            {
                CHAICanDLL.CiOpen(CanPort, 0x2);
                CHAICanDLL.CiSetBaud(CanPort, bt0: 0x03, bt1: 0x1c);
                FRC = CHAICanDLL.CiStart(CanPort);
            }
            return FRC;
        }


        public int DisactivateCan()
        {
            int FRC = 0;
            unsafe
            {
                FRC = CHAICanDLL.CiClose(CanPort);
            }
            return FRC;
        }


        public int ActivateCanOpen()
        {
            int FRC = 0;
            unsafe
            {
                FRC = CANOpenDll.start_can_master(CanPort, 4);
            }
            return FRC;
        }


        public int DisactivateCanOpen() 
        {
            int FRC = 0;
            unsafe
            {
                FRC = CANOpenDll.stop_can_master();
            }
            return FRC;
        }


        public int GetDeviceState(byte Node) => CANOpenDll.read_nmt_state(Node);


        public int SetDeviceState(byte Node, byte State) => CANOpenDll.nmt_master_command(State, Node);



        public int ReadDeviceInfo<T>(byte Node, ref T Data)
        {
            int FRC = 0;

            Read(Node, 0x1000, 0x0000, ref Data);
            Console.WriteLine($"Тип устройства: ");

            FRC = Read(Node, 0x1018, 0x01, ref  Data);
            Console.WriteLine($"Vendor ID: ");

            return FRC;
        }


        public int ChangeDeviceInfo<T>(byte Node)
        {
            int FRC = 0;

            Console.WriteLine("Введите новый тип устройства: ");
            uint Data = Convert.ToUInt32(Console.ReadLine());
            Write(Node, 0x1000, 0x00, Data);

            Console.WriteLine("Введите новый Vendor ID: ");
            Data = Convert.ToUInt32(Console.ReadLine());
            FRC = Write(Node, 0x1018, 0x01, Data);


            return FRC;
        }


        public string GetErrorInfo(int FRC)
        {
            var CodesDict = new Dictionary<int, string>()
            {
                [0] = " успешное выполнение",
                [1] = "???",
                [2] = " устройство или ресурс заняты",
                [3] = " ошибка памяти",
                [4] = " метод не может быть использован из текущего состояния контроллера",
                [5] = " ошибка вызова, метод не может быть вызван для этого объекта",
                [6] = " переданы некорректные параметры",
                [7] = " не удаётся получить доступ к ресурсу",
                [8] = " метод не реализован",
                [9] = " ошибка ввода/вывода",
                [10] = " устройство отсутствует",
                [11] = " вызов был остановлен событием",
                [12] = " нет ресурсов",
                [13] = " произошло прерывание",


            };
            return CodesDict[FRC];
        }

    }

}
