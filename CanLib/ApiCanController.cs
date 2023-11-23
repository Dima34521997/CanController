using System.Xml.Linq;

namespace CAN_Test.ApiCanController
{
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


        public ushort GetLengthOfArray(byte Node, ushort Index)
        {
            ushort Size = 0;
            Read(Node, Index, 0, ref Size);
            return Size;
        }


        public int FastWrite(canmsg wr) => CHAICanDLL.CanWrite(CanPort, wr);
  

        public int FastRead(ref canmsg rd) =>CHAICanDLL.CanRead(CanPort, ref rd);
   

        public int GetHBT(byte Node, ref ushort HBT) => Read(Node, 0x1017, 0x0000, ref HBT);


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


        public string GetDeviceStateInfo(byte StateNode)
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


            return StateDict[StateNode];
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


        public int ReadPDO(byte Node, ushort Index, byte SubIndex, ref byte Upd, ref int Data)
        {
            int FRC = CANOpenDll.AddNodeObjectToDictionary(Node, Index, SubIndex, (ushort)Defines.CAN_DEFTYPE_UNSIGNED8);
            ushort objDictInd = (ushort)(Defines.CAN_INDEX_RCVPDO_MAP_MIN + (Node - 1) * (int)Defines.CAN_NOF_PDO_TRAN_SLAVE);

            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_ENTER_PRE_OPERATIONAL, Node);
            Thread.Sleep(50);
            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_RESET_NODE, Node);
            CANOpenDll.SetAllPDOsState((byte)Defines.NOT_VALID);
            CANOpenDll.WritePDOMapping(objDictInd, 0, 0);          
            CANOpenDll.WritePDOMapping(objDictInd, 1, 0x60000108);
            CANOpenDll.WritePDOMapping(objDictInd, 0, 1);

            CANOpenDll.SetAllPDOsState((byte) Defines.VALID);
            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_START_REMOTE_NODE, Node);
 
            Thread.Sleep(20);

            FRC = CANOpenDll.ReadNodeObjectToDictionary(Node, Index, SubIndex, ref Upd, ref Data);
            Console.WriteLine(FRC);
            Thread.Sleep(10);

            return FRC;

        }



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

}
