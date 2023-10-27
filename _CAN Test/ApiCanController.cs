using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test
{
    public class ApiCanController
    {
        static byte CanOpenPort = 0;
        static byte CanPort = 1;
        
        public UInt32 DeviceCanId;


        public static int Write<T>(byte Node, UInt16 Index, byte SubIndex, T Data, int FRC)
        {
            T NewData = Data;
            UInt32 DataSize = (UInt16)TypeSize<T>.Size;

            unsafe
            {
                FRC = CANOpenDll.
                    write_device_object_sdo(Node, Index, SubIndex, (byte*)&NewData, DataSize);

            }

            if (FRC != 0) throw new Exception("Ошибка записи");
            return FRC;
        }


        public static void WriteArray<T>(byte Node, UInt16 Index, T[] Data, int FRC)
        {
            T[] NewData = Data;
            UInt16 ArraySize = GetLengthOfArray(Node, Index);
            for (byte subIndex = 1; subIndex <= ArraySize; subIndex++)
            {
                Write(Node, Index, (byte)(subIndex), Data[subIndex - 1], FRC);
                Thread.Sleep(10);
            }
        }


        public static T Read<T>(byte Node, UInt16 Index, byte SubIndex, T Data, int FRC)
        {
            UInt32 DataSize = (UInt16)TypeSize<T>.Size;
            unsafe
            {
                FRC = CANOpenDll.read_device_object_sdo(Node, Index, SubIndex, (byte*)&Data, ref DataSize);
            }
            if (FRC != 0 && Data == null) 
                throw new Exception("Попытка прочесть пустой индекс");
            return Data;
        }


        public static int ReadArray<T>(byte Node, UInt16 Index, out T[] DataArr, int FRC)
        {
            UInt32 DataSize = (UInt16)TypeSize<T>.Size;
            UInt16 ArraySize = GetLengthOfArray(Node, Index);

            T[] NewDataArr = new T[ArraySize];
            
            for (byte index = 0; index < ArraySize; index++)
            {
                NewDataArr[index] = Read(Node, Index, (byte)(0x01 + index), NewDataArr[index], FRC);
            }
            if (FRC != 0 && NewDataArr == null) // типа пустой индекс
                throw new Exception("Попытка прочесть пустой индекс");

            DataArr = NewDataArr;
            return FRC;
        }


        static UInt16 GetLengthOfArray(byte Node, UInt16 Index)
        {
            byte pass = 0;
            UInt16 Size = sizeof(byte);
            Size = Read(Node, Index, pass, Size, pass);
            return Size;
        }


        public static int FastWrite(byte[] Data, UInt32 ID, int FRC)
        {
            canmsg wr = new canmsg();
            wr.id = ID;
            wr.data = Data;
            wr.len = (byte)Data.Length;
            FRC = CHAICanDLL.CanWrite(CanPort, wr);
            Thread.Sleep(10);
            return FRC;
        }

        public static int FastRead(byte[] Data, int FRC)
        {
            canmsg rd = new canmsg();
            while (true)
            {
                FRC = CHAICanDLL.CanRead(CanPort, ref rd);
                if (FRC == (int)CHAICodes.ECIOK) break;
            }

            for (int i = 0; i < rd.data.Length; i++) { Data[i] = rd.data[i]; }
            return FRC;
        }

        public static T GetHBT<T>(byte Node, T Data, int FRC)
        {
            UInt32 DataSize = (UInt16)TypeSize<T>.Size;
            unsafe
            {
                FRC = CANOpenDll.read_device_object_sdo(Node, 0x1017, 0x0000, (byte*)&Data, ref DataSize);
            }
            if (FRC != 0 && Data == null)
                throw new Exception("Попытка прочесть пустой индекс");
            return Data;
        }


        public static int ActivateCan(int FRC, int cond=0)
        {
            if (cond == 1)
            {
                FRC = CHAICanDLL.CanOpen(CanPort, 0x2);
                FRC = CHAICanDLL.CanSetBaud(CanPort, bt0: 0x03, bt1: 0x1c);
                FRC = CHAICanDLL.CanStart(CanPort);
            }
            return FRC;
        }


        public static void CanPortClose(byte CanPort)
        {
            CHAICanDLL.CanClose(CanPort);
        }


    }
}
