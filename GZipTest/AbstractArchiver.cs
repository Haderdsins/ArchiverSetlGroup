using System;
using System.Collections.Concurrent;
using System.Threading;

namespace GZipTest;
/// <summary>
/// Класс является абстрактной основой для реализации многопоточных операций архивации файлов, поддерживающей как сжатие, так и распаковку.
/// </summary>
public abstract class AbstractArchiver
{
    protected static int blockSize = 1024 * 1024;
    protected static int countThreads = Environment.ProcessorCount;
    protected string InputFile{get; set; }
    protected string OutputFile{get; set; }
    
    protected BlockingCollection<Blocks> processingDataBlocks = new BlockingCollection<Blocks>(countThreads*10);
    protected CollectingData dataBlocksToWrite = new CollectingData();
    protected AutoResetEvent[] autoResetEvents = new AutoResetEvent[countThreads];

    public AbstractArchiver(string inputFile, string outputFile)
    {
        InputFile = inputFile;
        OutputFile = outputFile;
    }
    
    /// <summary>
    /// Метод читает входной файл и добавляет блоки данных в очередь обработки
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected abstract void StartReadFile();
    
    /// <summary>
    /// Метод распаковывает блоки данных из очереди и добавляет их в коллекцию данных для записи
    /// </summary>
    /// <param name="threadsNumber"></param>
    protected abstract void BlockProcessing(int threadsNumber);
    
    /// <summary>
    /// Метод записывает распакованные данные в выходной файл.
    /// </summary>
    protected abstract void WriteToFile();
    
    /// <summary>
    /// Метод запускает потоки для чтения, обработки и записи данных, координирует синхронизацию работы всех потоков и обеспечивает корректное завершение всех операций.
    /// </summary>
    public void GetProccess()
    {
        Thread[] processThread = new Thread[countThreads];
        Thread threadRead = new Thread(StartReadFile);
        threadRead.Start();

        for (int i = 0; i < processThread.Length; i++)
        {
            autoResetEvents[i] = new AutoResetEvent(false);
            processThread[i] = new Thread(() => BlockProcessing(i));
            processThread[i].Start();
        }

        Thread threadWrite = new Thread(WriteToFile);
        threadWrite.Start();
        WaitHandle.WaitAll(autoResetEvents);
        dataBlocksToWrite.Completed();
        threadWrite.Join();
    }
}