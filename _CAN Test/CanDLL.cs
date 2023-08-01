using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test;

internal partial class CHAICanDLL
{
    [DllImport("chai.dll", EntryPoint = "CiOpen", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiOpen(byte chan, byte flags);

    [DllImport("chai.dll", EntryPoint = "CiInit", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiInit();

    [DllImport("chai.dll", EntryPoint = "CiStart", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiStart(byte chan);

    [DllImport("chai.dll", EntryPoint = "CiStop", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiStop(byte chan);

    [DllImport("chai.dll", EntryPoint = "CiSetFilter", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiSetFilter(byte chan, uint acode, uint amask);

    [DllImport("chai.dll", EntryPoint = "CiSetBaud", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiSetBaud(byte chan, byte bt0, byte bt1);

    [DllImport("chai.dll", EntryPoint = "CiWrite", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiWrite(byte chan, canmsg_t[] mbuf, short cnt = 1);

    [DllImport("chai.dll", EntryPoint = "CiRead", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiRead(byte chan, ref canmsg_t mbuf, short cnt);


    [DllImport("chai.dll", EntryPoint = "CiTrQueThreshold", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe ushort CiTrQueThreshold(byte chan, short getset, ref short thres);

    [DllImport("chai.dll", EntryPoint = "CiGetLibVer", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe uint CiGetLibVer();


    [DllImport("chai.dll", EntryPoint = "CiGetLibVer", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiTransmit(byte chan, canmsg_t[] mbuf);

    [DllImport("chai.dll", EntryPoint = "CiClose", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short CiClose(byte chan);

    [DllImport("chai.dll", EntryPoint = "msg_setrtr", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe void msg_setrtr(canmsg_t[] mbuf);


    [DllImport("chai.dll", EntryPoint = "msg_isrtr", CallingConvention = CallingConvention.Cdecl)] //
    private static extern unsafe short msg_isrtr(canmsg_t[] mbuf);

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

    public static short CanWrite(byte chan, canmsg_t[] mbuf, short cnt = 1)
    {
        return CiWrite(chan, mbuf, cnt);
    }

    public static short CanRead(byte chan, canmsg_t[] mbuf, short cnt = 1)
    {
        return CiRead(chan, ref mbuf[0], cnt);
    }

    public static ushort TrQueThreshold(byte chan, short getset, ref short thres)
    {
        return CiTrQueThreshold(chan, getset, ref thres);
    }

    public static short CanTransmit(byte chan, canmsg_t[] mbuf)
    {
        return CiTransmit(chan, mbuf);
    }

    public static void setrtr_msg(canmsg_t[] mbuf)
    {
        msg_setrtr(mbuf);
    }

    public static short isrtr_msg(canmsg_t[] mbuf)
    {
        return msg_isrtr(mbuf);
    }
}
