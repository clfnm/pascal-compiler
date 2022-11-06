using System;

namespace Lexer
{
    public class Lexeme
    {
        private Position pos; //позиция
        private Position Pos
        {
            get => pos;
        }

        private LexemeType type; //тип

        public LexemeType Type
        {
            get => type;
        }

        private object value; //значение

        /**
         * object - тип, который может хранить любые другие типы данных: строка, число, enum и т.д.
         */
        public object Value
        {
            get => value;
        }

        private string raw_value; //необработанное значение

        public string RawValue
        {
            get => raw_value;
        }

        public Lexeme(Position pos, LexemeType type, Object value, string rawValue)
        {
            this.pos = pos;
            this.type = type;
            this.value = value;
            this.raw_value = rawValue;
        }

        public override string ToString()
        {
            var result = $"{pos.CurrentLine}\t{pos.CurrentColumn}\t{type.ToString()}\t";
            
            // Преобразование значения (value) в зависимости от типа лексемы (type)
            switch (type)
            {
                case LexemeType.Eof:
                    result += "EOF";
                    break;
                case LexemeType.Double:
                    result += $"{(double)value}";
                    break;
                case LexemeType.Integer:
                    result += $"{(int)value}";
                    break;
                case LexemeType.Keyword:
                    result += $"{((KeywordsValue) value).ToString()}";
                    break;
                case LexemeType.Operation:
                    result += $"{((OperatorValue) value).ToString()}";
                    break;
                case LexemeType.Separator:
                    result += $"{((SeparatorValue) value).ToString()}";
                    break;
                default:
                    result += $"{value}";
                    break;
            }

            result += $"\t{raw_value}";
            return result;
        }
    }
}