using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using static CAN_Test.CANOpenDll;

namespace CAN_Test;

public static class SDOcommunication
{
    #region Чтение
    
    public static T ReadSDO<T>(byte node, UInt16 Index, byte SubIndex, T data) // чтение
    {
        UInt32 DataSize = (UInt16)TypeSize<T>.Size;
        Int16 functionResultCode = 0;

        unsafe
        {
            functionResultCode = 
                CANOpenDll.read_device_object_sdo(node, Index, SubIndex, (byte*)&data, ref DataSize);
        }

        if (functionResultCode != 0 && data == null) // типа пустой индекс
            throw new Exception("Попытка прочесть пустой индекс");

        return data;
    }


    public static T[] ReadArraySDO<T>(byte node, UInt16 Index, byte SubIndex, T[] data) // чтение
    {
        UInt32 DataSize = (UInt16)TypeSize<T>.Size;
        Int16 functionResultCode = 0;
        ushort ArraySize = GetLengthOfArray(node, Index);
        
        for (byte index = 0; index < ArraySize; index++)
        {
            data[index] = ReadSDO(node, Index, (byte)(0x01 + index), data[index]);
        }

        if (functionResultCode != 0 && data == null) // типа пустой индекс
            throw new Exception("Попытка прочесть пустой индекс");

        return data;
    }

    #endregion

    #region Запись
    public static T WriteSDO<T>(byte node, UInt16 Index, byte SubIndex, T data) // чтение
    {
        UInt32 DataSize = (UInt16)TypeSize<T>.Size;
        Int16 functionResultCode = 0;

        unsafe
        {
            functionResultCode =
                CANOpenDll.read_device_object_sdo(node, Index, SubIndex, (byte*)&data, ref DataSize);
        }

        if (functionResultCode != 0 && data == null) // типа пустой индекс
            throw new Exception("Попытка прочесть пустой индекс");

        return data;
    }



    static public void WriteSDO(byte node, UInt16 Index, byte SubIndex, in byte Data)// 1) byte
    {
        byte newData = Data;
        UInt32 DataSize = sizeof(byte);

        Int16 fnr = 0;

        unsafe
        {
            fnr = CANOpenDll.write_device_object_sdo(node, Index, SubIndex, (byte*)(&newData), DataSize);
        }
    }


    #endregion


    #region Чтение массива


    //static public void ReadArraySDO(byte node, UInt16 Index, out byte[] Arr)
    //{
    //    UInt16 ArrayLength = GetLengthOfArray(node, Index);
    //    Arr = new byte[ArrayLength];
    //    byte data = 0;
    //    if (ArrayLength == 0)
    //    {
    //        throw new Exception("Попытка прочесть несуществующую или пустую переменную");
    //    }

    //    for (byte index = 0; index < ArrayLength; index++)
    //    {
    //        SDO.ReadSDO(node, Index, (byte)(0x01 + index), out data);
    //        Arr[index] = data;
    //    }
    //}

     private static UInt16 GetLengthOfArray(byte node, UInt16 Index)
    {
        UInt16 Size = sizeof(byte);
        Size = ReadSDO(node, Index, 0x00, Size);

        return Size;
    }

    #region Запись массива


    //static public void WriteArraySDO(byte node, UInt16 Index, in byte[] Arr)
    //{
    //    UInt16 ArrayLength = GetLengthOfArray(node, Index);

    //    if (ArrayLength == 0)
    //    {
    //        throw new Exception("Попытка записать в несуществующую переменную");
    //    }

    //    byte data = 0;

    //    for (byte index = 0; index < ArrayLength; index++)
    //    {
    //        data = Arr[index];
    //        WriteSDO(node, Index, (byte)(0x01 + index), in data);

    //    }

    //}

    #endregion




    #endregion

}
