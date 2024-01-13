using System.Xml.Linq;

namespace CAN_Test.ApiCanController;

public class ApiCanController : IApiCanController
{
    static byte CanOpenPort = 0;
    static byte CanPort = 0;

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


    public int ActivateCanOpen()
    {
        int FRC = 0;
        unsafe
        {
            FRC = CANOpenDll.start_can_master(CanOpenPort, 4);
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


    public string GetDeviceStateInfo(byte Node)
    {
        var StateDict = new Dictionary<int, string>()
        {
            [127] = " предоперационное",
            [0] = " CANopen устройство активировано(boot-up протокол).",
            [5] = " операционное состояние узла.",
            [4] = " cостояние останова CAN узла.",
            [254] = " нет данных о NMT состоянии CAN узла.",
            [255] = " неопределенное состояние CAN узла (произошло событие сердцебиения).",
        };


        return StateDict[CANOpenDll.read_nmt_state(Node)];
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
        

    public void CreatePDO(byte Node, ushort Index, byte Subindex)
        => CANOpenDll.AddNodeObjectToDictionary(Node, Index, Subindex, (ushort)Defines.CAN_DEFTYPE_INTEGER32);


    public uint VirtualIndexBuilder(ushort Index, byte Subindex)
        => Convert.ToUInt32($"{Index.ToString("X")}{Subindex.ToString("X2")}20", 16);


    public void BoundPDO(byte Node, ushort Index, byte Subindex, byte PDONumber=1)
    {
        if (PDONumber < 0 || PDONumber > 4)
        {
            throw new Exception("Неверный номер PDO. Допустимы значения от 1 до 4 включительно");
        }
        else
        {
            ushort objDictInd = (ushort)(Defines.CAN_INDEX_RCVPDO_MAP_MIN + (PDONumber - 1) + (Node - 1) * (int)Defines.CAN_NOF_PDO_TRAN_SLAVE); //вычисляет COBID (откуда читать PDO) (FUNCode+NODEID)
                                                                                                                                                 //TODO: Выяснить у разработчика зачем требуется переключение состояний контроллера
            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_ENTER_PRE_OPERATIONAL, Node);
            Thread.Sleep(50);
            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_RESET_NODE, Node);
            CANOpenDll.SetAllPDOsState((byte)Defines.NOT_VALID);
            CANOpenDll.WritePDOMapping(objDictInd, 0, 0);   // Устанавливаем значение 0 для нулевого субиндекса      
            CANOpenDll.WritePDOMapping(objDictInd, 1, VirtualIndexBuilder(Index, Subindex)); //1 параметр откуда принимать, 2 - суб индекс откуда брать , 3 - это куда мы записываем (т.е в свой виртуальный словарь ) (Index+SubIndex+размер(в битах))
            CANOpenDll.WritePDOMapping(objDictInd, 0, 1);
            CANOpenDll.SetAllPDOsState((byte)Defines.VALID);
            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_START_REMOTE_NODE, Node);

        }
    }


    public int ReadPDO(byte Node, ushort Index, byte SubIndex, ref byte UPD, ref int PDOData)
        => CANOpenDll.ReadNodeObjectToDictionary(Node, Index, SubIndex, ref UPD, ref PDOData);




    public string GetErrorInfo(int FRC)
    {
        var CodesDict = new Dictionary<int, string>()
        {
            [0] = " успешное выполнение",
            [1] = "???",
            [-2] = " устройство или ресурс заняты",
            [-3] = " ошибка памяти",
            [-4] = " метод не может быть использован из текущего состояния контроллера",
            [-5] = " ошибка вызова, метод не может быть вызван для этого объекта",
            [-6] = " переданы некорректные параметры",
            [-7] = " не удаётся получить доступ к ресурсу",
            [-8] = " метод не реализован",
            [-9] = " ошибка ввода/вывода",
            [-10] = " устройство отсутствует",
            [-11] = " вызов был остановлен событием",
            [-12] = " нет ресурсов",
            [-13] = " произошло прерывание",
            [-102] = " объект не существует в ОС",
            [-103] = " ошибка записи в объектный словарь",
        };
        return CodesDict[FRC];
    }

}
