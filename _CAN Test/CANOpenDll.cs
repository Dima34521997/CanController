using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test;

public class CANOpenDll
{
    private static short fnr;
    private static Defines fnrD;

    #region DLL imports


    #region Init import // Перенесены все функции

    // Back Init
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "start_can_master", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short start_can_master(byte chan, byte baudRate);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "stop_can_master", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short stop_can_master();


    // ???
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "canopen_monitor", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void canopen_monitor();
    // ???

    #endregion 

    #region NMT with HBT import // Перенесены все функции

    // NMT Master
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "nmt_master_command", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short nmt_master_command(byte nmtCstateCode, byte node);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_nmt_state", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short read_nmt_state(byte node);


    //HBT Master
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "write_master_hbt", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short write_master_hbt(byte node, ushort hbt);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_master_hbt", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Int16 read_master_hbt(byte node, ref ushort hbt);
    #endregion

    #region SDO import // Перенесены все функции

    // SDO
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_device_object_sdo", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short read_device_object_sdo(byte node, ushort canIndex, byte subind, byte* data, ref uint datasize);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "write_device_object_sdo", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe short write_device_object_sdo(byte node, ushort canIndex, byte subind, byte* data, uint datasize);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "set_sdo_timeout", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void set_sdo_timeout(UInt32 microsecond);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "get_sdo_timeout", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe UInt32 get_sdo_timeout();

    #endregion

    #region PDO import // Перенесены все функции

    // PDO Objects
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "set_all_pdos_state", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short set_all_pdos_state(byte state);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "get_pdo_node", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short get_pdo_node(byte index, ref byte node);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "put_pdo_node", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Int16 put_pdo_node(byte index, byte node);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_pdo_communication", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Int16 read_pdo_communication(byte index, byte subind, ref UInt32 data);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "write_pdo_communication", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Int16 write_pdo_communication(byte index, byte subind, UInt32 data);


    // PDO Mapping
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_pdo_mapping", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short read_pdo_mapping(ushort canIndex, byte subind, ref uint data);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "write_pdo_mapping", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short write_pdo_mapping(ushort canIndex, byte subind, uint data);


    // Master PDO
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "transmit_can_pdo", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short transmit_can_pdo(ushort index);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "receive_can_pdo", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short receive_can_pdo(UInt16 index, ref canmsg cf);




    #endregion

    #region Log import // Перенесены все функции

    // Logger
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_logger_event", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short read_logger_event(ref EventLog ev);
    #endregion

    #region OD import // Перенесены все функции

    // OD settings
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "add_node_object_dictionary", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short add_node_object_dictionary(byte node, ushort index, byte subind, ushort type);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_node_object_dictionary", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short read_node_object_dictionary(byte node, ushort index, byte subind, byte upd, ref Numbers num);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "write_node_object_dictionary", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short write_node_object_dictionary(byte node, ushort index, byte subind, ref Numbers num);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "get_node_updated_object", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe short get_node_updated_object(ref byte node, ref UInt16 index, ref byte subind, ref Numbers num);

    #endregion

    #region SYNC import // Перенесены все функции
    // SYNC functions
    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_sync_num", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe UInt32 read_sync_num();


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "read_sync_object", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Int16 read_sync_object(byte index, ref UInt32 data);


    [DllImport("canopen_dll_commander_x64.dll", EntryPoint = "write_sync_object", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Int16 write_sync_object(byte index, UInt32 data);



    #endregion

    #endregion

    #region Init wrapper // Все функции обёрнуты

    // Back Init

    public static short StartCANMaster(byte chan, byte baudRate)
    {
        return start_can_master(chan, baudRate);
    }

    public static short StopCANMaster()
    {
        return stop_can_master();
    }

    public static void CanOpenMonitor()
    {
        canopen_monitor();
    }

    #endregion

    #region NMT with HBT wrapper // Все функции обёрнуты

    // NMT Master with HBT
    public static short NMTMasterCommand(byte nmtStateCode, byte node)
    {
        return nmt_master_command(nmtStateCode, node);
    }


    public static short ReadNMTState(byte node) //D
    {
        return read_nmt_state(node);
    }


    public static short WriteMasterHBT(byte node, ushort hbt)
    {
        return write_master_hbt(node, hbt);
    }


    public static Int16 ReadMasterHBT(byte node, ref UInt16 hbt) //D
    {
        return read_master_hbt(node, ref hbt);
    }

    #endregion

    #region SDO wrapper // Все функции обёрнуты

    // SDO
    //public static Int16 ReadDeviceObjectSDO(byte node, ushort canIndex, byte subind, byte * data, ref uint datasize)
    //{
    //    return read_device_object_sdo(node, canIndex, subind, ref  data, ref datasize);
    //}


    //public static short WriteDeviceObjectSDO(byte node, ushort canIndex, byte subind, ref byte data, uint datasize)
    //{
    //    return write_device_object_sdo(node, canIndex, subind, ref data, datasize);
    //}


    public static void SetSDOTimeout(UInt32 microseconds)
    {
        set_sdo_timeout(microseconds);
    }


    public static UInt32 GetSDOTimeout()
    {
        return get_sdo_timeout();
    }

    #endregion

    #region PDO wrapper // Все функции обёрнуты
    //PDO Objects
    public static void SetAllPDOsState(byte state)
    {
        set_all_pdos_state(state);
    }


    public static Int16 GetPDONode(byte index, ref byte node)
    {
        return get_pdo_node(index, ref node);
    }


    public static Int16 PutPDONode(byte index, byte node)
    {
        return put_pdo_node(index, node);
    }


    public static Int16 ReadPDOCommunication(byte index, byte subind, ref UInt32 data)
    {
        return read_pdo_communication(index, subind, ref data);
    }


    public static Int16 WritePDOCommunication(byte index, byte subind, UInt32 data)
    {
        return write_pdo_communication(index, subind, data);
    }


    // PDO Mapping
    public static short ReadPDOMapping(ushort canIndex, byte subind, ref uint data)
    {
        return read_pdo_mapping(canIndex, subind, ref data);
    }

    public static short WritePDOMapping(ushort canIndex, byte subind, uint data)
    {
        return write_pdo_mapping(canIndex, subind, data);
    }

    //Master PDO
    public static short TransmitCANPDO(ushort index)
    {
        return transmit_can_pdo(index);
    }


    public static short ReceiveCANPDO(UInt16 index, ref canmsg cf)
    {
        return receive_can_pdo(index, ref cf);
    }

    #endregion

    #region Log wrapper // Все функции обёрнуты

    // Logging
    public static short ReadLoggerEvent(ref EventLog ev)
    {
        return read_logger_event(ref ev);
    }

    #endregion

    #region OD wrapper // Все функции обёрнуты

    // Object Dictionary
    public static short AddNodeObjectToDictionary(byte node, ushort index, byte subind, ushort type)
    {
        return add_node_object_dictionary(node, index, subind, type);
    }

    public static short ReadNodeObjectToDictionary(byte node, ushort index, byte subind, byte upd, ref Numbers num)
    {
        return read_node_object_dictionary(node, index, subind, upd, ref num);
    }

    public static short WriteNodeObjectDictionary(byte node, ushort index, byte subind, ref Numbers num)
    {
        return write_node_object_dictionary(node, index, subind, ref num);
    }


    public static short GetNodeUpdatedObject(ref byte node, ref ushort index, ref byte subind, ref Numbers num)
    {
        return get_node_updated_object(ref node, ref index, ref subind, ref num);
    }

    #endregion

    #region SYNC wrapper // Все функции обёрнуты

    // SYNC functions
    public static UInt32 ReadSYNCNum()
    {
        return read_sync_num();
    }


    public static Int16 ReadSYNCObject(byte index, ref UInt32 data)
    {
        return read_sync_object(index, ref data);
    }


    public static Int16 WriteSYNCObject(byte index, UInt32 data)
    {
        return write_sync_object(index, data);
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Main funcs

    // Main Funcs

    public static void ShowLogger()
    {
        EventLog ev = new();
        if (ReadLoggerEvent(ref ev) == (short)Defines.GEN_RETOK)
            Console.WriteLine("|{0,4}|{1,4}|{2,4}|{3,10}|{4,10:X}|{5,10}|{6,10:X}|",
                               ev.node, ev.cls, ev.type, ev.code, ((ushort)ev.code), ev.info, ((ushort)ev.info));
    }

    public static void ConfigureCANOpenSlave(byte node)
    {
        UInt32 data;
        UInt16 objDictInd;

        Console.WriteLine("\n");

        NMTMasterCommand((byte)Defines.CAN_NMT_ENTER_PRE_OPERATIONAL, node);
        Thread.Sleep(50);

        Console.WriteLine($"Resetting node {node}");
        NMTMasterCommand((byte)Defines.CAN_NMT_RESET_NODE, node);

        Thread.Sleep(500);

        // Set master heartbeat
        fnr = WriteMasterHBT(node, 1200);
        Console.WriteLine($"Master heartbeat set. Status: {fnr}");

        // Set slave heartbeat
        data = 1000;
        // fnr = WriteDeviceObjectSDO(node, 0x1017, 0, (byte) data, 2);
        Console.WriteLine($"Slave heartbeat set. Status: {fnr}");

        // Add node to OD
        fnr = AddNodeObjectToDictionary(node, 0x6000, 1, (ushort)Defines.CAN_DEFTYPE_UNSIGNED8);
        Console.WriteLine($"Add digital inputs OBD entry. Status: {fnr}");

        //	Digital outputs will be set from the inputs

        objDictInd = (ushort)(Defines.CAN_INDEX_RCVPDO_MAP_MIN + (node - 1) * (int)Defines.CAN_NOF_PDO_TRAN_SLAVE);
        // The first master receive PDO (TPDO for the node)
        fnr = WritePDOMapping(objDictInd, 0, 0);
        Console.WriteLine($"Master receive PDO mapping sub0 disable. Status: {fnr}");
        fnr = WritePDOMapping(objDictInd, 1, 0x60000108);
        Console.WriteLine($"Master receive PDO mapping sub1, digital inputs. Status: {fnr}");
        fnr = WritePDOMapping(objDictInd, 0, 1);
        Console.WriteLine($"Master receive PDO mapping sub0 enable. Status: {fnr}");

        objDictInd = (ushort)(Defines.CAN_INDEX_TRNPDO_MAP_MIN + (node - 1) * (int)Defines.CAN_NOF_PDO_RECV_SLAVE);
        // The first master transmit PDO (RPDO for the node)
        fnr = WritePDOMapping(objDictInd, 0, 0);
        Console.WriteLine($"Master receive PDO mapping sub0 disable. Status: {fnr}");
        fnr = WritePDOMapping(objDictInd, 1, 0x60000108);
        Console.WriteLine($"Master receive PDO mapping sub1, digital inputs. Status: {fnr}");
        fnr = WritePDOMapping(objDictInd, 0, 1);
        Console.WriteLine($"Master receive PDO mapping sub0 enable. Status: {fnr}");

        SetAllPDOsState((byte)Defines.VALID);
        Console.WriteLine("Setting all master PDOs to VALID state");

        NMTMasterCommand((byte)Defines.CAN_NMT_START_REMOTE_NODE, node);
        Console.WriteLine($"Starting node {node}\n");
    }

    //public static void ReadDeviceObjects(byte node)
    //{
    //    uint dataSize;
    //    byte data;
    //    string dch = new string('0', 100);

    //    data = 0;
    //    dataSize = 2;
    //    ReadDeviceObjectSDO(node, 0x1017, 0, ref data, ref dataSize);
    //    Console.WriteLine($"Slave heartbeat: {data}");

    //    dataSize = 100;
    //    byte new_data = Convert.ToByte(dch);
    //    ReadDeviceObjectSDO(node, 0x1008, 0, ref new_data, ref dataSize);
    //    dch = dch.Remove(dch.Length - 1, 1) + "0";

    //    Console.WriteLine($"Manufacturer Device Name: {new_data}");
    //    Console.WriteLine($"Symbols read actually: {dataSize}");
    //}

    public static void Monitor()
    {
        TransmitCANPDO(0x1800);
        ShowLogger();
        CanOpenMonitor();
        Thread.Sleep(100);
    }

    // Useful C# structs for C++ code work

    [StructLayout(LayoutKind.Explicit)]
    public struct Numbers
    {
        [FieldOffset(0)] public ulong init;
        [FieldOffset(8)] public byte i8;
        [FieldOffset(9)] public byte uns8;
        [FieldOffset(10)] public short i16;
        [FieldOffset(12)] public ushort uns16;
        [FieldOffset(14)] public int i32;
        [FieldOffset(18)] public uint uns32;
        [FieldOffset(22)] public long i64;
        [FieldOffset(28)] public ulong uns64;
        [FieldOffset(36)] public float re32;
        [FieldOffset(40)] public double re64;
      

    }

    public enum Types
    {
        Typesbyte =0,
        TypesInt = 1

    }

    public struct EventLog
    {
        public long ts;
        public byte node;
        public byte cls;
        public byte type;
        public byte misc;     // Reserved
        public short code;
        public int info;
    }
}
#endregion