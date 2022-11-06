using System;


namespace Lexer
{
    public enum KeywordsValue
    {
        AND,
        ARRAY,
        ASM,
        BEGIN,
        BREAK,
        CASE,
        CONST,
        CONSTRUCTOR,
        CONTINUE,
        DESTRUCTOR,
        DIV,
        DO,
        DOWNTO,
        ELSE,
        END,
        FALSE,
        FILE,
        FOR,
        FUNCTION,
        GOTO,
        IF,
        IMPLEMENTATION,
        IN,
        INLINE,
        INTERFACE,
        LABEL,
        MOD,
        NIL,
        NOT,
        OBJECT,
        OF,
        OPERATOR,
        OR,
        PACKED,
        PROCEDURE,
        PROGRAM,
        RECORD,
        REPEAT,
        SET,
        SHL,
        SHR,
        STRING,
        THEN,
        TO,
        TRUE,
        TYPE,
        UNIT,
        UNTIL,
        USES,
        VAR,
        WHILE,
        WITH,
        XOR,
        AS,
        CLASS,
        CONSTREF,
        DISPOSE,
        EXCEPT,
        EXIT,
        EXPORTS,
        FINALIZATION,
        FINALLY,
        INHERITED,
        INITIALIZATION,
        IS,
        LIBRARY,
        NEW,
        ON,
        OUT,
        PROPERTY,
        RAISE,
        SELF,
        THREADVAR,
        TRY,
        ABSOLUTE,
        ABSTRACT,
        ALIAS,
        ASSEMBLER,
        CDECL,
        CPPDECL,
        DEFAULT,
        EXPORT,
        EXTERNAL,
        FORWARD,
        GENERIC,
        INDEX,
        LOCAL,
        NAME,
        NOSTACKFRAME,
        OLDFPCCALL,
        OVERRIDE,
        PASCAL,
        PRIVATE,
        PROTECTED,
        PUBLIC,
        PUBLISHED,
        READ,
        REGISTER,
        REINTRODUCE,
        SAFECALL,
        SOFTFLOAT,
        SPECIALIZE,
        STDCALL,
        VIRTUAL,
        WRITE
    }
    
    public enum LexemeType
    {
        Invalid,
        Eof,
        Id,
        Keyword,
        Operation,
        Separator,
        Integer,
        Double,
        String,
    }
    
    public enum OperatorValue
    {
        Assigment, // :=
        Equal, // =
        NotEqual, // <>
        LessThan, // <
        GreaterThan, // >
        LessThanOrEqual, // >=
        Addition, // +
        AdditionWithAssigment, // +=
        Subtraction, // -
        SubtractionWithAssigment, // +=
        Multiplication, // *
        MultiplicationWithAssigment, // +=
        Division, // /
        DivisionWithAssigment, // +=
        BitwiseShiftToTheLeft, // <<
        BitwiseShiftToTheRight, // >>
    }
    
    public enum SeparatorValue
    {
        LeftParenthesis, // (
        RightParenthesis, // )
        LeftSquareBracket, // [
        RightSquareBracket, // ]
        Comma, // ,
        Dot, // .
        Range, // .. 
        Semicolon, // ;
        Colon // :
    }
    
    public class Lexeme
    {
        private Position pos;

        private Position Pos
        {
            get => pos;
        }

        private LexemeType type;

        public LexemeType Type
        {
            get => type;
        }

        private object value;


        public object Value
        {
            get => value;
        }

        private string raw_value;

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