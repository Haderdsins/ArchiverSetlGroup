namespace GZipTest;
/// <summary>
/// Класс управляет процессом сжатия или распаковки файлов, проверяя входные параметры и инициируя соответствующий архиватор (Compress или Decompress)
/// </summary>
public class ArchiveController
    {
        private string processArchiver;
        private string inputFile;
        private string outputFile;
        private string[] args;
        public ArchiveController(string[] args)
        {
            this.args = args;
        }
        /// <summary>
        /// Проверяет корректность и полноту переданных аргументов для операций сжатия или распаковки файлов
        /// </summary>
        /// <returns></returns>
        private bool IsCorrectParametrs()
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Введены не все параметры:\n compress/decompress [имя исходного файла] [имя результирующег");
                return false;
            }

            if (args[0].ToLower().Equals("compress")||args[0].ToLower().Equals("decompress"))
            {
                processArchiver = args[0].ToLower();
            }
            else
            {
                Console.WriteLine("Некорректно введен первый параметр: \n compress/decompress");
                return false;
            }

            inputFile = args[1];
            if (inputFile.Length==0 || !File.Exists(inputFile))
            {
                Console.WriteLine("Название файла введено некорректно или такого файла не существует");
                return false;
            }
            
            outputFile = args[2];
            if (inputFile.Length==0|| File.Exists(outputFile))
            {
                Console.WriteLine("Файл с таким выходным названием уже существует");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Метод инициирует процесс сжатия или распаковки на основе проверенных параметров, управляя выбором и запуском соответствующего архиватора
        /// </summary>
        public void Start()
        {
            try
            {
                if (IsCorrectParametrs())
                {
                    AbstractArchiver abstractArchiver;
                    if (processArchiver.Equals("compress"))
                    {
                        abstractArchiver = new Compress(inputFile, outputFile);
                    }
                    else
                    {
                        abstractArchiver = new Decompress(inputFile, outputFile);
                    }
                    abstractArchiver.GetProccess();
                }
                else
                {
                    Console.WriteLine(1);
                    return;
                }
                Console.WriteLine(0);
            }
            catch(Exception)
            {
                Console.WriteLine(1);
                Console.WriteLine("sda");
            }
        }
    }