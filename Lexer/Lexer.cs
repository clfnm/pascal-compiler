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

        /**
         * Получить следующий символ из потока. Сохраняет символ в буфер. Изменяет поле pos.
         */
        int GetChar()
        {
            // считать символ из потока
            var c = (char)_fileStream.Read();
            _buffer += c;
            // \n - конец строки
            if (c == '\n')
            {
                // увеличиваем позицию по строке и обнуляем по колонкам, если встретили знак окончания строки
                _currentPosition.IncLine();
            }
            else if (c != '\r') // на видоус системах после \n следует \r
            {
                // увеличиваем колонку, если встрели не \n или \r
                _currentPosition.IncColumn();
            }

            return c;
        }

        /**
         * Получить следующий элемент из потока, без перехода на него
         */
        int GetPeek()
        {
            return _fileStream.Peek();
        }

        /**
         * Получить следующую лексему из потока. Вызывать функцию, пока тип лексемы не Eof
         */
        public Lexeme GetNext()
        {
            // взять один символ из потока
            var c = GetChar();

            /* встретили символ конца потока
            if (_fileStream.EndOfStream)
            {
                return new Lexeme(_currentPosition, LexemeType.Eof, "", "");
            }*/

            // пропускаем пробелы и комментарии
            do
            {
                // если это комменатрий типа (* *)
                if ((char)c == '(' && GetPeek() == '*')
                {
                    GetChar(); // взять символ *
                    SkipCommentWithRoundBracket(); // вызов функция для пропуска комментариев
                    c = GetChar(); // взять следующий символ, после комментария
                }
                // если это комменатрий типа { }
                else if ((char)c == '{')
                {
                    SkipCommentWithBrace();
                    c = GetChar(); // взять следующий символ, после комментария
                }
                // если это комменатрий типа //
                else if ((char)c == '/' && GetPeek() == '/')
                {
                    GetChar(); //  взять символ /
                    SkipCommentInLine();
                    c = GetChar(); // взять следующий символ, после комментария
                }
                // если c = табуляции, переносу строки или переносу строки из windows
                else if ((char)c == ' ' || c == '\t' || c == '\n' || c == '\r')
                {
                    c = GetChar();
                }
                else
                {
                    // означает, что с символа c начинается новая лексема
                    break;
                }
            } while (true);

            // Сохраняем позицию начала новой лексемы
            _pos.Set(_currentPosition.CurrentLine, _currentPosition.CurrentColumn);

            // обнуляем буффер перед считываем новой лексемы
            _buffer = "";
            // добавляется один символ, который мы считала перед break из прошлого цикла
            _buffer += (char)c;

            // проверяем с на соотвествие опеределенной лексеме
            switch (c)
            {
                // текущий символ равняется ':'
                case ':':
                    // если следующий символ равняется '=', то это оператор присваивания
                    // в других case по аналогии
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
                    // Если начало соотвествует индентификатору
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
            // пока не встретили *) продолжаем чтение
            while ((char)GetChar() != '*' && (char)GetPeek() != ')')
            {
                // если наткнулись на конец файла (комментарий не закрыт)
                if (_fileStream.EndOfStream)
                {
                    throw new Exception("Comment not closed");
                }
            }

            // переходим на следующий символ
            GetChar(); // get )
        }

        private void SkipCommentWithBrace()
        {
            // по аналогии с SkipCommentWithRoundBracket
            while ((char)GetChar() != '}')
            {
                // если наткнулись на конец файла (комментарий не закрыт)
                if (_fileStream.EndOfStream)
                {
                    throw new Exception("Comment not closed");
                }
            }
        }

        private void SkipCommentInLine()
        {
            var c = (char)GetChar();
            // пока не встретим конец строки или Eof
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
            // проверяем символ соответствует ли он началу индентификатора:
            // латиница, нижнее подчеркивание
            return 'a' <= char.ToLower(c) && char.ToLower(c) <= 'z' || c == '_';
        }


        private Lexeme ScanIdOrKeyword()
        {
            // считываем пока похоже на индентификатор
            while (IsId((char)GetPeek()) || Digit((char)GetPeek(), 10))
            {
                GetChar();
            }

            // проверяем если ли в enum такое слово
            try
            {
                // _buffer.ToUpper() - сделать все буквы заглавными
                // Enum.Parse(Type, String) - преобразует строковое значение в значение из enum
                var value = (KeywordsValue)Enum.Parse(
                    typeof(KeywordsValue),
                    _buffer.ToUpper()
                    ); // выкидывает ошибки, если нет такого ключевого слова
                return new Lexeme(_pos, LexemeType.Keyword, value, _buffer);
            }
            catch (Exception ex) //  ловим ошибку и возвращаем значение индентификатора
            {
                return new Lexeme(_pos, LexemeType.Id, _buffer, _buffer);
            }
            
        }

        private Lexeme ScanString()
        {
            var c = (char)GetPeek();
            // считываем пока, не встретили '
            while (true)
            {
                // если строка не закрыта и мы встретили конец файла, значит ошибка
                if (c == '\n' || c == '\r' || _fileStream.EndOfStream)
                {
                    throw new Exception("Unexpected character");
                }
                
                
                // если встретили еще одну кавычну, значит строковый литерал закончился
                if (c == '\'')
                {
                    return new Lexeme(_pos, LexemeType.String, _buffer, _buffer);
                }

                c = (char)GetChar();
            }
        }

        /**
         * проверяем является ли символ числом в конкретной СС
         */
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
            
            // выбираем систему счисления относительно начального символа
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

            // сохряняем отдельно части числа
            var integerPart = new StringBuilder();
            var afterDot = new StringBuilder();
            var afterE = new StringBuilder();

            // сохраняем целую часть числа
            if (Digit(firstChar, baseNumber))
            {
                integerPart.Append(firstChar);
            }

            var type = LexemeType.Integer;
            
            while (Digit((char)GetPeek(), baseNumber))
            {
                integerPart.Append((char)GetChar());
            }
            
            // если в не 10 системе счисления нет цифр, то выкидываем 
            // ошибку
            //
            // Длина равная 0 говорит о том, что в буфере только префикс:'$', '&', '%'
            if (integerPart.Length == 0 && baseNumber != 10)
            {
                throw new Exception("Invalid integer");
            }

            if (GetPeek() == '.' && baseNumber == 10)
            {
                // записываем символы после точки
                afterDot.Append((char)GetChar());
                type = LexemeType.Double;
                while ('0' <= GetPeek() && GetPeek() <= '9')
                {
                    afterDot.Append((char)GetChar());
                }
            }

            // Например, $10. => валидно
            if (GetPeek() == '.' && baseNumber != 10)
            {
                GetChar();
                type = LexemeType.Double;
            }

            // e может быть только после числе в 10 СС
            var digitsAfterE = -1;
            if ((GetPeek() == 'e' || GetPeek() == 'E') && baseNumber == 10)
            {
                // записываем символы после e
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

            // если после e нет чисел, то ошибка
            if (digitsAfterE == 0 && baseNumber == 10)
            {
                throw new Exception("Unexpected character");
            }

            if (type == LexemeType.Double && baseNumber == 10)
            {
                var value = integerPart.ToString();

                // если есть точка и есть числа после точки, то конкатинируем строки
                if (afterDot.Length >= 2)
                {
                    value = string.Concat(value, afterDot.ToString());
                }

                // если есть e и есть числа после e, то конкатинируем строки
                if (afterE.Length >= 2 && '0' <= afterE[afterE.Length - 1] &&
                    afterE[afterE.Length - 1] <= '9')
                {
                    value = string.Concat(value, afterE.ToString());
                }

                // по умолчанию, double.Parse сканирует число, где разделитель между целой и дробной частями , (запятая)
                // CultureInfo.InvariantCulture позволяет парсеру сканировать число из строки, где разделитель . (точка)
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