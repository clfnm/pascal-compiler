/*класс счетчик, чтобы понимать, на какой строке и колонке находится лексема*/

//using System;

namespace Lexer
{
    public class Position
    {
        private uint _currentLine, _currentColumn;
        public uint CurrentLine
        {
            get => _currentLine;
        }
        public uint CurrentColumn
        {
            get => _currentColumn;
        }

        public void IncLine()
        {
            // увеличиваем количество строк, обнуляем количество колонок после \n
            _currentLine += 1;
            _currentColumn = 0;
        }
        
        public void IncColumn()
        {
            // увеличиваем количество колонок после символа, который не равен символам \n и \r
            _currentColumn += 1;
        }

        public void Set(uint line, uint column)
        {
            // устанавливаем значения строки и колонки
            _currentLine = line;
            _currentColumn = column;
        }

        public Position(uint currentLine, uint currentColumn)
        {
            _currentLine = currentLine;
            _currentColumn = currentColumn;
        }
    }
}