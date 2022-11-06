using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lexer;

namespace Tests
{
    internal class Program
    {
        //Путь к файлу
        private static string GetFilePathInProject(string file)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "../../" + file;
        }

        //Создание файла .out
        private static string GetOutputFile(string file)
        {
            return file.Substring(0, file.Length - 3) + ".out";
        }

        public static void Main(string[] args)
        {
            var files = new List<string>() { };
            // подготавливаем список всех символов
            foreach (var file in Directory.GetFiles(GetFilePathInProject(""), "*.*", SearchOption.AllDirectories)
                         .Where(s => "*.in".Contains(Path.GetExtension(s).ToLower())))
            {
                files.Add(file);
            }

            // подчитываем общее количество тестов и тестов, в которые лексер выдал неправильный ответ
            int total = 0, failed = 0;

            foreach (var inputFile in files)
            {
                // подсчитываем количество тестов
                ++total;
                Console.WriteLine(inputFile);
                var outputFile = GetOutputFile(inputFile);
                
                // открываем файл
                var inputReader = new StreamReader(inputFile);
                var lexer = new Lexer.Lexer(inputReader);
                Lexeme lexeme;
                
                // записываем в <file>.out, что выдает лексер, если файла не существует
                if (!File.Exists(outputFile))
                {
                    Console.WriteLine("WRITE BY LEXER");
                    var outputWriter = new StreamWriter(outputFile);
                    do
                    {
                        try
                        {
                            // берем следующую лексему
                            lexeme = lexer.GetNext();
                            outputWriter.Write(lexeme.ToString());
                            Console.WriteLine(lexeme.ToString());
                        }
                        catch (Exception ex) // ловим ислючение из lexer.GetNext()
                        {
                            outputWriter.WriteLine(ex.Message);
                            Console.WriteLine(ex.Message);
                            break;
                        }

                        // если встетили Eof, то не пишем
                        if (lexeme.Type != LexemeType.Eof)
                        {
                            outputWriter.WriteLine();
                        }
                    } while (lexeme.Type != LexemeType.Eof);
                    outputWriter.Flush(); // записываем из буфера потока в файл
                    outputWriter.Close(); // закрываем файл
                    continue; // переходим к следующему файлу
                }
                
                var outputReader = new StreamReader(outputFile);
                var failedLocal = false; // упал ли теcт
                while (true)
                {
                    var line = outputReader.ReadLine();
                    try
                    {
                        lexeme = lexer.GetNext();
                    }
                    catch (Exception ex)
                    {
                        if (line != ex.Message)
                        {
                            failedLocal = true; // записываем, что упал тест
                            Console.WriteLine("FAIL,\n{0}\n{1}", line, ex.Message);
                        }
                        break;
                    }

                    // сопоставляем ответ лексера и то, что содержится в <file>.in
                    var lexemeStr = lexeme.ToString();
                    if (line == lexemeStr)
                    {
                        Console.WriteLine("OK,\t{0}", lexemeStr);
                    }
                    else
                    {
                        failedLocal = true; // записываем, что упал тест
                        Console.WriteLine("FAIL,\n{0}\n{1}", line, lexemeStr);
                    }
                    if (lexeme.Type == LexemeType.Eof)
                    {
                        break;
                    }
                }

                if (failedLocal)
                {
                    // увеличиваем счетчик упавших тестов
                    ++failed;
                }
            }
            
            // выводится статистика тестов (все, прошедшие, провалившиеся)
            Console.WriteLine("TOTAL: {0}, SUCCESS: {1}, FAILED: {2}", total, total - failed, failed);
        }
    }
}