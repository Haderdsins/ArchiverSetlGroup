using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest;
/// <summary>
/// Класс управляет синхронизированным доступом к блокам данных для многопоточной записи и чтения.
/// </summary>
public class CollectingData
{
    private static int countThreads =  Environment.ProcessorCount;
    private Dictionary<int,byte[]> dataBlocksToWrite;
    private static object locker = new object();
    private bool completed = false;
    private int index = 0;

    public CollectingData()
    {
        dataBlocksToWrite = new Dictionary<int, byte[]>();
    }
    
    /// <summary>
    /// Метод добавляет пару идентификатор-блок данных в словарь, управляя доступом через блокировку и сигнализацию о возможности добавления новых данных
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bytes"></param>
    public void Add(int id, byte[] bytes)
    {
        lock (locker)
        {
            while (dataBlocksToWrite.Keys.Count>=countThreads*10)
            {
                Monitor.Wait(locker);
            }
            dataBlocksToWrite.Add(id, bytes);
            Monitor.PulseAll(locker);
        }
    }
    
    /// <summary>
    /// Метод извлекает и удаляет данные из словаря по текущему индексу, управляя потоком выполнения с использованием блокировок и условной сигнализации
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool GetValue(out byte[] data)
    {
        lock (locker)
        {
            while (!dataBlocksToWrite.ContainsKey(index))
            {
                if (completed)
                {
                    data = new byte[0];
                    return false;
                }
                Monitor.Wait(locker);
            }
            data = dataBlocksToWrite[index];
            dataBlocksToWrite.Remove(index++);                
            Monitor.PulseAll(locker);
            return true;
        }
    }
    
    /// <summary>
    /// Метод отмечает завершение добавления данных, активируя сигнал для всех ожидающих потоков о возможном завершении операций.
    /// </summary>
    public void Completed()
    {
        lock (locker)
        {
            completed = true;
            Monitor.PulseAll(locker);
        }
    }
}