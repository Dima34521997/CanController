using CAN_Test;

void CHAI()
{
    const uint mask = 0x0;
    const uint code = 0x0; //0xFFFF;

    const byte chan0 = 0x0;
    const byte chan1 = 0x1;

    // Init CHAI
    int errorCode = CHAICanDLL.CanInit();
    Console.WriteLine(errorCode == (int)CHAICodes.ECIOK
        ? $"{errorCode}: Запуск успешен"
        : $"{errorCode}: Ошибка запуска");

    errorCode = CHAICanDLL.CanOpen(chan0, 4);
    Console.WriteLine("Канал 0:" + errorCode);
    errorCode = CHAICanDLL.CanOpen(chan1, 4);
    Console.WriteLine("Канал 1:" + errorCode);

    errorCode = CHAICanDLL.CanSetBaud(chan0, bt0: 0x03, bt1: 0x1c);
    Console.WriteLine("Канал 0 Бод-Рейт:" + errorCode);
    errorCode = CHAICanDLL.CanSetBaud(chan1, bt0: 0x03, bt1: 0x1c);
    Console.WriteLine("Канал 1 Бод-Рейт:" + errorCode);

    errorCode = CHAICanDLL.CanSetFilter(chan0, code, mask);
    Console.WriteLine("CAN Filter chan0 = " + errorCode);
    errorCode = CHAICanDLL.CanSetFilter(chan1, code, mask);
    Console.WriteLine("CAN Filter chan1 = " + errorCode);

    errorCode = CHAICanDLL.CanStart(0);
    Console.WriteLine("Открытие канала 0:" + errorCode);
    errorCode = CHAICanDLL.CanStart(1);
    Console.WriteLine("Открытие канала 1:" + errorCode);

    // Write message

    canmsg_t[] canmsgW = new canmsg_t[1];
    canmsgW[0].id = 0x50;
    canmsgW[0].data = new byte[8] { 0x01, 0x02, 0x03, 0x04,
                            0x05, 0x06, 0x07, 0x08 };
    canmsgW[0].len = 8;
    canmsgW[0].flags = 4;
    CHAICanDLL.setrtr_msg(canmsgW);

    errorCode = CHAICanDLL.CanWrite(0, canmsgW, 1);
    Console.WriteLine("Отправка кадра: " + errorCode);

    // Read message

    canmsg_t[] canmsgR = new canmsg_t[1];

    errorCode = CHAICanDLL.CanRead(1, canmsgR, 1);
    Console.WriteLine("Получение кадра: " + errorCode);
    Console.WriteLine("Содержание:");
    Console.WriteLine("Данные:");
    foreach (byte data in canmsgR[0].data)
    {
        Console.WriteLine($"    {data}");
    }
    Console.WriteLine("Длина - " + canmsgR[0].len);
    Console.WriteLine("Флаги - " + canmsgR[0].flags);
    Console.WriteLine("Таймаут - " + canmsgR[0].ts);
    Console.WriteLine("ID - " + canmsgR[0].id);

    // Close

    CHAICanDLL.CanClose(0);
    CHAICanDLL.CanClose(1);
}

//static int CANOpen()
//{
//    short fnr = CANOpenDll.StartCANMaster(1, (byte)Defines.CIA_BITRATE_INDEX_500);
//    if (fnr != (short)Defines.GEN_RETOK)
//    {
//        Console.WriteLine($"Master NOT started, error: {fnr}");
//        return (int)Defines.CAN_ERRET_MAIN;
//    }

//    CANOpenDll.ConfigureCANOpenSlave(2);
//    CANOpenDll.ReadDeviceObjects(2);

//    CANOpenDll.ConfigureCANOpenSlave(127);
//    CANOpenDll.ReadDeviceObjects(127);

//    Console.WriteLine(new string('-', 10 + 10 + 10 + 10 + 10 + 10 + 10 + 5));
//    Console.WriteLine("|{0,4}|{1,4}|{2,4}|{3,10}|{4,10}|{5,10}|{6,12}|",
//        "Node", "CLS", "Type", "Code", "Code(hex)", "Info", "Info(hex)");
//    Console.WriteLine(new string('-', 10 + 10 + 10 + 10 + 10 + 10 + 10 + 5));

//    for (int i = 0; i < 16; i++) CANOpenDll.Monitor();
//    if (CANOpenDll.StopCANMaster() != (int)Defines.CAN_RETOK) return (int)Defines.CAN_ERRET_MAIN;
//    return (int)Defines.CAN_RETURN_MAIN;
//}

//Console.WriteLine($"Program ended with {CANOpen()} code"); 