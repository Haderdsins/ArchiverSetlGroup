using System.IO.Compression;

namespace GZipTest;

    /// <summary>
    /// Класс распаковки из воходного файла GZip и запись в выходной файл
    /// </summary>
    public class Decompress : AbstractArchiver
    {
        public Decompress(string inputFile, string outputFile) : base(inputFile, outputFile)
        {
        }
        /// <summary>
        /// Метод читает входной файл и добавляет блоки данных в очередь обработки
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void StartReadFile()
        {
            try
            {
                TryStartReadFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка извлечения файла {0}: {1}", (new FileInfo(InputFile)).Name, e.Message);
                throw new Exception();
            }
        }
        /// <summary>
        /// Метод читает сжатые блоки данных из файла и добавляет их в очередь для последующей обработки
        /// </summary>
        private void TryStartReadFile()
        {
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(sourceStream))
                {
                    int i = 0;
                    FileInfo file = new FileInfo(InputFile);
                    var sizeFileInput = file.Length;
                    while (sourceStream.Position < sourceStream.Length)
                    {
                        int sizeCompressBlock = binaryReader.ReadInt32();
                        byte[] buffer = binaryReader.ReadBytes(sizeCompressBlock);
                        processingDataBlocks.Add(new Blocks(i, buffer));
                        sizeFileInput = sizeFileInput - (sizeCompressBlock + 4);
                        i++;
                    }
                    if (sourceStream.Position == sourceStream.Length)
                    {
                        processingDataBlocks.CompleteAdding();
                    }
                }
            }
        }

        protected override void BlockProcessing(int threadsNumber)
        {
            try
            {
                foreach (var data in processingDataBlocks.GetConsumingEnumerable())
                {
                    var outCompress = DecompressBlock(data.Block);
                    dataBlocksToWrite.Add(data.ID, outCompress);
                }
                autoResetEvents[threadsNumber].Set();


            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка распаковки блока данных: {0}", e.Message);
            }
        }
        /// <summary>
        /// Метод распаковывает массив байтов, представляющий сжатые данные, с использованием GZip-стрима.
        /// </summary>
        /// <param name="dataBlock"></param>
        /// <returns></returns>
        private byte[] DecompressBlock(byte[] dataBlock)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (MemoryStream inputStream = new MemoryStream(dataBlock))
                {
                    using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[dataBlock.Length];
                        int read;
                        while ((read = decompressionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, read);
                        }
                        return outputStream.ToArray();
                    }
                }
            }
        }
        protected override void WriteToFile()
        {
            try
            {
                using (FileStream destinationStream = File.Create(OutputFile))
                {
                    while (dataBlocksToWrite.GetValue(out var data))
                    {
                        destinationStream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка записи в файл: {0}", e.Message);
            }
        }
    }