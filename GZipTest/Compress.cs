using System.IO.Compression;

namespace GZipTest;
/// <summary>
/// Класс сжимает данные из входного файла и записывает их в выходной файл в формате GZip
/// </summary>
public class Compress : AbstractArchiver
    {
        public Compress(string inputFile, string outputFile) : base(inputFile, outputFile)
        {
        }

        protected override void StartReadFile()
        {
            try
            {
                TryStartReadFile();

            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка сжатия файла {0}: {1}", (new FileInfo(InputFile)).Name, e.Message);
                throw new Exception();
            }
        }

        protected override void BlockProcessing(int threadsNumber)
        {
            try
            {
                foreach (var data in processingDataBlocks.GetConsumingEnumerable())
                {
                    var outCompress = CompressBlock(data.Block);
                    dataBlocksToWrite.Add(data.ID, outCompress);
                }
                autoResetEvents[threadsNumber].Set();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка сжатия блока данных: {0}", e.Message);
            }
        }

        /// <summary>
        /// Метод сжимает блок данных, используя GZipStream, и возвращает результат в виде массива байт
        /// </summary>
        /// <param name="dataBlock"></param>
        /// <returns></returns>
        private byte[] CompressBlock(byte[] dataBlock)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream compressionStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    compressionStream.Write(dataBlock, 0, dataBlock.Length);
                }
                return outputStream.ToArray();
            }
        }

        protected override void WriteToFile()
        {
            try
            {
                using (FileStream destinationStream = File.Create(OutputFile + ".gz"))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(destinationStream))
                    {
                        while (dataBlocksToWrite.GetValue(out var data))
                        {
                            binaryWriter.Write(data.Length);
                            binaryWriter.Write(data, 0, data.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка записи в файл: {0}", e.Message);
            }
        }
        /// <summary>
        /// Метод читает данные из файла и добавляет блоки данных в очередь обработки.
        /// </summary>
        private void TryStartReadFile()
        {
            using (FileStream sourceStream = new FileStream(InputFile, FileMode.Open, FileAccess.Read))
            {
                int i = 0;
                while (sourceStream.Position < sourceStream.Length)
                {
                    int blockLength = 0;

                    if (sourceStream.Length - sourceStream.Position > blockSize)
                    {
                        blockLength = blockSize;
                    }
                    else
                    {
                        blockLength = (int)(sourceStream.Length - sourceStream.Position);
                    }
                    byte[] buffer = new byte[blockLength];
                    sourceStream.Read(buffer, 0, blockLength);
                    processingDataBlocks.Add(new Blocks(i, buffer));

                    i++;
                }
                if (sourceStream.Position == sourceStream.Length)
                {
                    processingDataBlocks.CompleteAdding();
                }
            }
        }
    }