using CAN_Test;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using static CAN_Test.CANOpenDll;

namespace TEST
{
    internal class Program
    {

        static void PDOSettings(byte node)
        {
            CANOpenDll.NMTMasterCommand((byte)Defines.CAN_NMT_START_REMOTE_NODE, node);
            Console.WriteLine($"Starting node {node}\n");

                
            Int16 fnr = CANOpenDll.AddNodeObjectToDictionary(node, 0x6000, 1, (UInt16)Defines.CAN_DEFTYPE_INTEGER8);
            UInt16 objDictInd = (UInt16)(Defines.CAN_INDEX_RCVPDO_MAP_MIN + (node - 1) * (int)Defines.CAN_NOF_PDO_TRAN_SLAVE);
            Console.WriteLine($"Добавление объекта в OD. Status: {fnr}");


            fnr = CANOpenDll.WritePDOMapping(objDictInd, 0, 0);
            Console.WriteLine($"Master receive PDO mapping sub0 disable. Status: {fnr}");
            //Запрещено PDO отображение, установлено значение 0 для нулевого субиндекса.При этом PDO будет переведено в недействительное состояние.


            fnr = CANOpenDll.WritePDOMapping(objDictInd, 1, 0x60000108); // 0x60000108 - ДИЧЬ+
            Console.WriteLine($"Master receive PDO mapping sub1, digital inputs. Status: {fnr}");
            //Изменено PDO отображение, модифицированы соответствующие субиндексы.


            fnr = CANOpenDll.WritePDOMapping(objDictInd, 0, 1);
            Console.WriteLine($"Master receive PDO mapping sub0 enable. Status: {fnr}");
            //

            //fnr = CANOpenDll.RecieveCANPDO(0x6100, ref ) /// ????

            

        }



        public static void PrintArr<T>(T[] arr)
        {
            foreach(T b in arr) Console.Write($"{b} ");
            Console.WriteLine("\n");
        }





        static void Main(string[] args)
        {

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            CANOpenDll.StartCANMaster(0, 0);

            ulong data = 0;
            byte Node = 99;
            UInt16 Index = 0x6100;
            byte SubIndex = 7;

            //UInt16 ArrayLength = SDO.GetLengthOfArray(Node, Index);
            //Console.WriteLine($"Длина массива = {ArrayLength}");
          //  Console.WriteLine("");

            byte[] arr = new byte[255];




            //Console.WriteLine("Read T data");
            arr = SDOcommunication.ReadArraySDO(Node, Index, SubIndex, arr);
          

            stopwatch.Stop();
            //смотрим сколько миллисекунд было затрачено на выполнение
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Console.WriteLine(new String('-', 35));
             PrintArr(arr);

            /*
            SDO.WriteArraySDO(Node, Index, arr);
            Console.Write($"Записанный массив: ");
            PrintArr(arr);

            SDO.ReadArraySDO(Node, Index, out arr);
            Console.Write("Считанный массив: ");
            PrintArr(arr);
            */

            //PrintArr(dataArr);

            //Console.WriteLine($"Размерность массива: {ArrayLength}");

            //Console.Write($"Считанное значение data = ");
            //foreach (byte i in data) Console.Write($"{i} ");
            //Console.WriteLine("\n");
        }
    }
}