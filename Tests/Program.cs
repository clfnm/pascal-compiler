using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lexer;

namespace Tests
{
    internal class Program
    {
        private static string GetFilePathInProject(string file)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "../../" + file;
        }

        private static string GetOutputFile(string file)
        {
            return file.Substring(0, file.Length - 3) + ".out";
        }

        public static void Main(string[] args)
        {
            var files = new List<string>() { };
            foreach (var file in Directory.GetFiles(GetFilePathInProject(""), "*.*", SearchOption.AllDirectories)
                         .Where(s => "*.in".Contains(Path.GetExtension(s).ToLower())))
            {
                files.Add(file);
            }

            int total = 0, failed = 0;

            foreach (var inputFile in files)
            {
                // подсчитываем количество тестов
                ++total;
                Console.WriteLine(inputFile);
                var outputFile = GetOutputFile(inputFile);
                
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
                        lexeme = lexer.GetNext();
                        outputWriter.Write(lexeme.ToString());
                        Console.WriteLine(lexeme.ToString());
                        if (lexeme.Type != LexemeType.Eof)
                        {
                            outputWriter.WriteLine();
                        }
                    } while (lexeme.Type != LexemeType.Eof);
                    outputWriter.Flush();
                    outputWriter.Close();
                    continue;
                }
                
                var outputReader = new StreamReader(outputFile);
                var failedLocal = false;
                while (true)
                {
                    var line = outputReader.ReadLine();
                    lexeme = lexer.GetNext();
                    // сопоставляем ответ лексера и то, что содержится в <file>.in
                    var lexemeStr = lexeme.ToString();
                    if (line == lexemeStr)
                    {
                        Console.WriteLine("OK,\t{0}", lexemeStr);
                    }
                    else
                    {
                        failedLocal = true;
                        Console.WriteLine("FAIL,\n{0}\n{1}", line, lexemeStr);
                    }
                    if (lexeme.Type == LexemeType.Eof)
                    {
                        break;
                    }
                }

                if (failedLocal)
                {
                    ++failed;
                }
            }
            
            Console.WriteLine("TOTAL: {0}, SUCCESS: {1}, FAILED: {2}", total, total - failed, failed);
        }
    }
}