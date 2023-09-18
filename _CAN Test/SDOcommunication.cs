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


    public static T[] ReadArraySDO<T>(byte node, UInt16 Index, T[] data) 
    {
        UInt32 DataSize = (UInt16)TypeSize<T>.Size;
        Int16 functionResultCode = 0;
        UInt16 ArraySize = GetLengthOfArray(node, Index);

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
    public static void WriteSDO<T>(byte node, UInt16 Index, byte SubIndex, T data) 
    {
        T newData = data;
        UInt32 DataSize = (UInt16)TypeSize<T>.Size;
        Int16 functionResultCode = 0;

        unsafe
        {
            functionResultCode = CANOpenDll.
                write_device_object_sdo(node, Index, SubIndex, (byte*)&newData, DataSize);
            
        }

        if (functionResultCode != 0) throw new Exception("Ошибка записи");
    }



    static public void WriteArraySDO<T>(byte node, UInt16 Index, T[] data) 
    {
        T[] newData = data;
        UInt16 ArraySize = GetLengthOfArray(node, Index);


        for (byte subIndex = 1; subIndex < ArraySize; subIndex++)
        {
            WriteSDO(node, Index, (byte)(subIndex), data[subIndex - 1]);
            Thread.Sleep(1);
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
