using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lexer
{
    public class Lexer
    {
        private readonly Position _currentPosition = new Position(1, 0);
        private readonly Position _pos = new Position(1, 0);
        private readonly StreamReader _fileStream;
        private string _buffer = "";

        public Lexer(StreamReader fileStream)
        {
            _fileStream = fileStream;
        }


        int GetChar()
        {

            var c = (char)_fileStream.Read();
            _buffer += c;

            if (c == '\n')
            {

                _currentPosition.IncLine();
            }
            else if (c != '\r')
            {

                _currentPosition.IncColumn();
            }

            return c;
        }


        int GetPeek()
        {
            return _fileStream.Peek();
        }


        public Lexeme GetNext()
        {

            var c = GetChar();




            do
            {

                if ((char)c == '(' && GetPeek() == '*')
                {
                    GetChar();
                    SkipCommentWithRoundBracket();
                    c = GetChar();
                }

                else if ((char)c == '{')
                {
                    SkipCommentWithBrace();
                    c = GetChar();
                }

                else if ((char)c == '/' && GetPeek() == '/')
                {
                    GetChar();
                    SkipCommentInLine();
                    c = GetChar();
                }

                else if ((char)c == ' ' || c == '\t' || c == '\n' || c == '\r')
                {
                    c = GetChar();
                }
                else
                {

                    break;
                }
            } while (true);


            _pos.Set(_currentPosition.CurrentLine, _currentPosition.CurrentColumn);


            _buffer = "";

            _buffer += (char)c;


            switch (c)
            {

                case ':':

                    if (GetPeek() != '=')
                        return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.Colon, _buffer);
                    GetChar();
                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.Assigment, _buffer);
                case '<':
                    if (GetPeek() == '>')
                    {
                        GetChar();
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.NotEqual, _buffer);
                    }

                    if (GetPeek() == '<')
                    {
                        GetChar();
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.BitwiseShiftToTheLeft,
                            _buffer);
                    }

                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.LessThan, _buffer);
                case '>':
                    if (GetPeek() == '=')
                    {
                        GetChar();
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.LessThanOrEqual,
                            _buffer);
                    }

                    if (GetPeek() == '>')
                    {
                        GetChar();
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.BitwiseShiftToTheRight,
                            _buffer);
                    }

                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.GreaterThan, _buffer);
                case '+':
                    if (GetPeek() != '=')
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.Addition, _buffer);
                    GetChar();
                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.AdditionWithAssigment,
                        _buffer);
                case '-':
                    if (GetPeek() != '=')
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.Subtraction, _buffer);
                    GetChar();
                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.SubtractionWithAssigment,
                        _buffer);
                case '*':
                    if (GetPeek() != '=')
                        return new Lexeme(_pos, LexemeType.Operation,
                            OperatorValue.MultiplicationWithAssigment,
                            _buffer);
                    GetChar();
                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.Multiplication, _buffer);
                case '/':
                    if (GetPeek() != '=')
                        return new Lexeme(_pos, LexemeType.Operation, OperatorValue.DivisionWithAssigment,
                            _buffer);
                    GetChar();
                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.Division, _buffer);
                case '=':
                    return new Lexeme(_pos, LexemeType.Operation, OperatorValue.Equal, _buffer);
                case '(':
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.LeftParenthesis, _buffer);
                case ')':
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.RightParenthesis, _buffer);
                case '[':
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.LeftSquareBracket,
                        _buffer);
                case ']':
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.RightSquareBracket,
                        _buffer);
                case ',':
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.Comma, _buffer);
                case '.':
                    if (GetPeek() != '.')
                    {
                        return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.Dot, _buffer);
                    }

                    GetChar();
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.Range, _buffer);
                case ';':
                    return new Lexeme(_pos, LexemeType.Separator, SeparatorValue.Semicolon, _buffer);
                // начало строкового литерала 
                case '\'':
                    return ScanString();
                default:

                    // Если начало соответствует числу
                    if ('0' <= c && c <= '9' || c == '$' || c == '&' || c == '%')
                    {
                        return ScanNumber((char)c);
                    }

                    // индентификатор или ключевое слово

                    if (IsId((char)c))
                    {
                        return ScanIdOrKeyword();
                    }
                    break;
            }
            
            // встретили символ конца потока
            if (_fileStream.EndOfStream)
            {
                return new Lexeme(_currentPosition, LexemeType.Eof, "", "");
            }


            // встретили неизвестную лексему
            return new Lexeme(_pos, LexemeType.Invalid, "", "");
        }

        private void SkipCommentWithRoundBracket()
        {

            while ((char)GetChar() != '*' && (char)GetPeek() != ')')
            {

                if (_fileStream.EndOfStream)
                {
                    throw new Exception("Comment not closed");
                }
            }


            GetChar(); // get )
        }

        private void SkipCommentWithBrace()
        {

            while ((char)GetChar() != '}')
            {

                if (_fileStream.EndOfStream)
                {
                    throw new Exception("Comment not closed");
                }
            }
        }

        private void SkipCommentInLine()
        {
            var c = (char)GetChar();

            while (true)
            {
                c = (char)GetChar();
                if (c == '\r' || c == '\n' || _fileStream.EndOfStream)
                {
                    break;
                }
            }
        }

        private bool IsId(char c)
        {

            return 'a' <= char.ToLower(c) && char.ToLower(c) <= 'z' || c == '_';
        }


        private Lexeme ScanIdOrKeyword()
        {

            while (IsId((char)GetPeek()) || Digit((char)GetPeek(), 10))
            {
                GetChar();
            }


            try
            {

                var value = (KeywordsValue)Enum.Parse(
                    typeof(KeywordsValue),
                    _buffer.ToUpper()
                    );
                return new Lexeme(_pos, LexemeType.Keyword, value, _buffer);
            }
            catch (Exception ex)
            {
                return new Lexeme(_pos, LexemeType.Id, _buffer, _buffer);
            }
            
        }

        private Lexeme ScanString()
        {
            var c = (char)GetPeek();

            while (true)
            {

                if (c == '\n' || c == '\r' || _fileStream.EndOfStream)
                {
                    throw new Exception("Unexpected character");
                }
                
                

                if (c == '\'')
                {
                    return new Lexeme(_pos, LexemeType.String, _buffer, _buffer);
                }

                c = (char)GetChar();
            }
        }


        private static bool Digit(char c, int baseNumber)
        {
            return (baseNumber == 16 && ('a' <= char.ToLower(c) && char.ToLower(c) <= 'f')) ||
                   (baseNumber == 10 && ('0' <= c && c <= '9')) ||
                   (baseNumber == 8 && ('0' <= c && c <= '7')) ||
                   (baseNumber == 2 && (c == '0' || c == '1'));
        }

        private Lexeme ScanNumber(char firstChar)
        {
            var baseNumber = 10;
            

            switch (firstChar)
            {
                case '$':
                    baseNumber = 16;
                    break;
                case '&':
                    baseNumber = 8;
                    break;
                case '%':
                    baseNumber = 2;
                    break;
            }


            var integerPart = new StringBuilder();
            var afterDot = new StringBuilder();
            var afterE = new StringBuilder();


            if (Digit(firstChar, baseNumber))
            {
                integerPart.Append(firstChar);
            }

            var type = LexemeType.Integer;
            
            while (Digit((char)GetPeek(), baseNumber))
            {
                integerPart.Append((char)GetChar());
            }
            

            if (integerPart.Length == 0 && baseNumber != 10)
            {
                throw new Exception("Invalid integer");
            }

            if (GetPeek() == '.' && baseNumber == 10)
            {

                afterDot.Append((char)GetChar());
                type = LexemeType.Double;
                while ('0' <= GetPeek() && GetPeek() <= '9')
                {
                    afterDot.Append((char)GetChar());
                }
            }


            if (GetPeek() == '.' && baseNumber != 10)
            {
                GetChar();
                type = LexemeType.Double;
            }


            var digitsAfterE = -1;
            if ((GetPeek() == 'e' || GetPeek() == 'E') && baseNumber == 10)
            {

                afterE.Append((char)GetChar());
                digitsAfterE = 0;
                type = LexemeType.Double;

                if (GetPeek() == '+' || GetPeek() == '-')
                {
                    afterE.Append((char)GetChar());
                }

                while ('0' <= GetPeek() && GetPeek() <= '9')
                {
                    ++digitsAfterE;
                    afterE.Append((char)GetChar());
                }
            }


            if (digitsAfterE == 0 && baseNumber == 10)
            {
                throw new Exception("Unexpected character");
            }

            if (type == LexemeType.Double && baseNumber == 10)
            {
                var value = integerPart.ToString();


                if (afterDot.Length >= 2)
                {
                    value = string.Concat(value, afterDot.ToString());
                }


                if (afterE.Length >= 2 && '0' <= afterE[afterE.Length - 1] &&
                    afterE[afterE.Length - 1] <= '9')
                {
                    value = string.Concat(value, afterE.ToString());
                }


                try
                {
                    return new Lexeme(_pos, type, double.Parse(value, CultureInfo.InvariantCulture), _buffer);
                }
                catch (Exception)
                {
                    
                    return new Lexeme(_pos, type, double.PositiveInfinity, _buffer);
                }
            }

            if (baseNumber == 10)
            {
                try
                {
                    return new Lexeme(_pos, type, int.Parse(_buffer), _buffer);
                }
                catch (Exception) // int.Parse - выкинул исключение переполнения
                {
                    throw new Exception("Integer overflow");
                }
            }
            {
                try
                {
                    var value = Convert.ToInt32(integerPart.ToString(), baseNumber);
                    return type == LexemeType.Double
                        ? new Lexeme(_pos, type, (double)value, _buffer)
                        : new Lexeme(_pos, type, value, _buffer);
                }
                catch (Exception)
                {
                    throw new Exception("Integer overflow");
                }
            }
        }
    }
}