using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test;

public static class Constants
{
    public const string IMPORT_FILE_NAME = "chai.dll";
}

public static class CHAICanDLL
{
    #region Imports
    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiOpen", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiOpen(byte chan, byte flags);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiInit", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiInit();

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiStart", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiStart(byte chan);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiStop", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiStop(byte chan);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiSetFilter", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiSetFilter(byte chan, uint acode, uint amask);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiSetBaud", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiSetBaud(byte chan, byte bt0, byte bt1);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiWrite", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiWrite(byte chan, canmsg mbuf, short cnt = 1);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiWrite", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiWrite(byte chan, canmsg[] mbuf, short cnt = 1);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiRead", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiRead(byte chan, ref canmsg mbuf, short cnt=1);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiRead", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiRead(byte chan, ref canmsg[] mbuf, short cnt=1);


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiTrQueThreshold", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe ushort CiTrQueThreshold(byte chan, short getset, ref ushort thres);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiGetLibVer", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe uint CiGetLibVer();


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiGetLibVer", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiTransmit(byte chan, canmsg[] mbuf);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiClose", CallingConvention = CallingConvention.Cdecl)] //
    public static extern unsafe short CiClose(byte chan);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "msg_setrtr", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe void msg_setrtr(canmsg mbuf);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "msg_setrtr", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe void msg_setrtr(canmsg[] mbuf);


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "msg_isrtr", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short msg_isrtr(canmsg[] mbuf);

    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiWriteTout", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiWriteTout(byte chan, short getset, ref ushort msec);


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiChipStat", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiChipStat(byte chan, ref chipstat_t stat);


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiRcQueThreshold", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiRcQueThreshold(byte chan, short getset, ref ushort thres);


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiRcQueResize", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiRcQueResize(byte chan, ushort size);


    [DllImport(Constants.IMPORT_FILE_NAME, EntryPoint = "CiSetLom", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiSetLom(byte chan, byte mode);








    #endregion


    #region Wrappers
    public static short CanOpen(byte chan, byte flags)
    {
        return CiOpen(chan, flags);
    }

    public static short CanClose(byte chan)
    {
        return CiClose(chan);
    }

    public static short CanInit()
    {
        return CiInit();
    }
    public static short CanStart(byte chan)
    {
        return CiStart(chan);
    }
    public static short CanStop(byte chan)
    {
        return CiStop(chan);
    }
    public static short CanSetFilter(byte chan, uint acode, uint amask)
    {
        return CiSetFilter(chan, acode, amask);
    }
    public static short CanSetBaud(byte chan, byte bt0, byte bt1)
    {
        return CiSetBaud(chan, bt0, bt1);
    }

    public static short CanWrite(byte chan, canmsg mbuf, short cnt = 1)
    {
        return CiWrite(chan, mbuf, 1);
    }

    public static short CanWrite(byte chan, canmsg[] mbuf, short cnt = 1)
    {
        return CiWrite(chan, mbuf, 1);
    }

    public static short CanRead(byte chan, ref canmsg mbuf, short cnt = 1)
    {
        return CiRead(chan, ref mbuf, 1);
    }

    public static short CanRead(byte chan, ref canmsg[] mbuf, short cnt = 1)
    {
        return CiRead(chan, ref mbuf, 1);
    }

    public static ushort CanTrQueThreshold(byte chan, short getset, ref ushort thres)
    {
        return CiTrQueThreshold(chan, getset, ref thres);
    }

    public static short CanTransmit(byte chan, canmsg[] mbuf)
    {
        return CiTransmit(chan, mbuf);
    }

    public static void setrtr_msg(canmsg mbuf)
    {
        msg_setrtr(mbuf);
    }

    public static void setrtr_msg(canmsg[] mbuf)
    {
        msg_setrtr(mbuf);
    }

    public static short isrtr_msg(canmsg[] mbuf)
    {
        return msg_isrtr(mbuf);
    }

    public static short CanWriteTimeOut(byte chan, short getset, ref ushort msec)
    {
        return CiWriteTout(chan, getset, ref msec);
    }

    public static short CanChipStat(byte chan, ref chipstat_t stat)
    {
        return CiChipStat(chan, ref stat);
    }


    public static short CanRcQueThreshold(byte chan, short getset, ref ushort thres)
    { return CiRcQueThreshold(chan, getset, ref thres); }


    public static short CanRcQueResize(byte chan, ushort size)
    { return CiRcQueResize(chan, size); }


    public static short CanSetLom(byte chan, byte mode)
    {
        return CiSetLom(chan, mode);
    }

    #endregion
}
