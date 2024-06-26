﻿using System;
using System.IO;

namespace GZipTest;
/// <summary>
/// Класс управляет процессом сжатия или распаковки файлов, проверяя входные параметры и инициируя соответствующий архиватор (Compress или Decompress)
/// </summary>
public class ArchiveController
{
    private string _processArchiver;
    private string _inputFile;
    private string _outputFile;
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
            _processArchiver = args[0].ToLower();
        }
        else
        {
            Console.WriteLine("Некорректно введен первый параметр: \n compress/decompress");
            return false;
        }

        _inputFile = args[1];
        if (_inputFile.Length==0 || !File.Exists(_inputFile))
        {
            Console.WriteLine("Название файла введено некорректно или такого файла не существует");
            return false;
        }
        
        _outputFile = args[2];
        if (_inputFile.Length==0|| File.Exists(_outputFile))
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
                if (_processArchiver.Equals("compress"))
                {
                    abstractArchiver = new Compress(_inputFile, _outputFile);
                }
                else
                {
                    abstractArchiver = new Decompress(_inputFile, _outputFile);
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