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
 
            _currentLine += 1;
            _currentColumn = 0;
        }
        
        public void IncColumn()
        {

            _currentColumn += 1;
        }

        public void Set(uint line, uint column)
        {

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