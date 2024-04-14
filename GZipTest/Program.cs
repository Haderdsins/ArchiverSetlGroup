using GZipTest;

Console.WriteLine("GZipTest.exe compress/decompress [имя исходного файла] [имя результирующего файла]");
string input = Console.ReadLine();
string[] items = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

ArchiveController manager = new ArchiveController(items);
manager.Start();

Console.ReadLine();