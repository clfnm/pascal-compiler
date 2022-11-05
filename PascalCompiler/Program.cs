using System;
using System.IO;
using Lexer;

namespace PascalCompiler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Нужно передать файл в качестве параметра");
                Environment.Exit(1);
            }

            // путь к файлу
            var file = args[0];

            if (!File.Exists(file))
            {
                Console.WriteLine("Файл не существует");
                Environment.Exit(1);
            }

            Lexeme lexeme;
            
            var inputReader = new StreamReader(file);
            var lexer = new Lexer.Lexer(inputReader);
            do
            {
                lexeme = lexer.GetNext();
                Console.WriteLine(lexeme.ToString());
            } while (lexeme.Type != LexemeType.Eof);
        }
    }
}