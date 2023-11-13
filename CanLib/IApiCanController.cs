using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAN_Test.ApiCanController
{
    public interface IApiCanController
    {
        /// <summary>
        /// Метод записывает пользовательские данные по указанным индексу и субиндексу.
        /// </summary>
        /// <typeparam name="T">Generic тип</typeparam>
        /// <param name="Node">Номер узла</param>
        /// <param name="Index">Индекс элемента ОС</param>
        /// <param name="SubIndex">Субиндекс элемента ОС</param>
        /// <param name="Data">Пользователькие данные для записи</param>
        /// <returns>Код-результат выполнения метода</returns>
        int Write<T>(byte Node, ushort Index, byte SubIndex, T Data);


        /// <summary>
        /// Метод записывает массив пользовательских данных по указанным номеру узла, индексу.
        /// </summary>
        /// <typeparam name="T">Generic тип</typeparam>
        /// <param name="Node">Номер узла</param>
        /// <param name="Index">Индекс элемента ОС</param>
        /// <param name="Data">Массив пользовательских данных для записи</param>
        /// <param name="FRC">Код-результат выполнения метода</param>
        /// <returns>Код-результат выполнения метода</returns>
        int WriteArray<T>(byte Node, ushort Index, T[] Data);


        /// <summary>
        /// Метод считывает данные из ОС по указанным номеру узла, индексу и субиндексу.
        /// </summary>
        /// <typeparam name="T">Generic тип</typeparam>
        /// <param name="Node">Номер узла</param>
        /// <param name="Index">Индекс элемента ОС</param>
        /// <param name="SubIndex">Субиндекс элемента ОС</param>
        /// <param name="Data">Хранит в себе считанные из ОС данные</param>
        /// <returns>Cчитанные из ОС данные</returns>
        int Read<T>(byte Node, ushort Index, byte SubIndex, ref T Data);


        /// <summary>
        /// Метод считывает массив из ОС по указанным номеру узла и индексу.
        /// </summary>
        /// <typeparam name="T">Generic тип</typeparam>
        /// <param name="Node">Номер узла</param>
        /// <param name="Index">Индекс элемента ОС</param>
        /// <param name="DataArr">Хранит в себе считанный из ОС массив данных</param>
        /// <returns>Считанный из ОС массив данных</returns>
        int ReadArray<T>(byte Node, ushort Index, out T[] DataArr);


        /// <summary>
        /// Метод отправляет CAN-кадр в сеть.
        /// </summary>
        /// <param name="Data">Данные для отправки</param>
        /// <param name="ID">Идентификатор CAN-кадра</param>
        /// <returns>Код-резутат выполнения метода</returns>
        int FastWrite(byte[] Data, uint ID);


        /// <summary>
        /// Считывает отправленный в сеть CAN-кадр.
        /// </summary>
        /// <param name="Data">Хранит в себе считанный CAN-кадр</param>
        /// <returns>Код-резутат выполнения метода</returns>
        int FastRead(byte[] Data);


        /// <summary>
        /// Считывает Heartbeat заданного узла.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Node">Номер узла</param>
        /// <param name="HBT"></param>
        /// <returns>Код-резутат выполнения метода</returns>
        int GetHBT<T>(byte Node, ref ushort HBT);


        /// <summary>
        /// Устанавливает Heartbeat заданного узла.
        /// </summary>
        /// <param name="Node">Номер узла</param>
        /// <param name="HBT">Heartbeat в мс</param>
        /// <returns>Код-результат выполнения метода</returns>
        int SetHBT(byte Node, ushort HBT);


        /// <summary>
        /// Активирует протокол CANOpen (Baudrate = 125kb/s).
        /// </summary>
        /// <returns>Код-результат выполнения метода</returns>
        int ActivateCanOpen();


        /// <summary>
        /// Завершает работу CanOpen протокола.
        /// </summary>
        /// <returns>Код-результат выполнения метода</returns>
        int DisactivateCanOpen();


        /// <summary>
        /// Выводит информацию о текущем состоянии узла в текстовом виде.
        /// </summary>
        /// <param name="Node">Номер узла</param>
        /// <returns>Текстовое описание состояния узла</returns>
        string GetDeviceStateInfo(byte Node);


        /// <summary>
        /// Выводит информацию о текущем состоянии узла.
        /// </summary>
        /// <param name="Node">Номер узла</param>
        /// <returns>Числовой код состояния узла</returns>
        int GetDeviceState(byte Node);


        /// <summary>
        /// Устанавливает состояние указанному узлу.
        /// </summary>
        /// <param name="Node">Номер узла</param>
        /// <param name="State">Состояние</param>
        /// <returns>Код-результат выполнения метода</returns>
        int SetDeviceState(byte Node, byte State);


        /// <summary>
        /// Считывает и выводит информацию об ID вендора и типе устройства
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Node">Номер узла</param>
        /// <param name="Data">Переменная для хранения данных</param>
        /// <returns>Код-результат выполнения метода</returns>
        int ReadDeviceInfo<T>(byte Node, ref T Data);


        /// <summary>
        /// Метод позволяет изменять значения типа устройства и vendor ID в ОС устройства.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Node">Номер узла</param>
        /// <returns>Код-результат выполнения метода</returns>
        int ChangeDeviceInfo<T>(byte Node);


        /// <summary>
        /// Считывает PDO.
        /// </summary>
        /// <param name="Node">Номер узла</param>
        /// <param name="Index">Индекс</param>
        /// <param name="SubIndex">Субиндекс</param>
        /// <param name="Upd">Флаг обновления данных PDO</param>
        /// <param name="Data">Данные PDO</param>
        /// <returns>Код-результат выполнения</returns>
        int ReadPDO(byte Node, ushort Index, byte SubIndex, ref byte Upd, ref int Data);


        /// <summary>
        /// Выводит текстовую расшифровку кода-результата.
        /// </summary>
        /// <param name="FRC">Код-результат выполнения метода</param>
        /// <returns>Расшифровка кода-результата выполнения</returns>
        string GetErrorInfo(int FRC);





    }
}
