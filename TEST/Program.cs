using CAN_Test;

namespace TEST
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CANOpenDll.StartCANMaster(99, 125);

        }
    }
}