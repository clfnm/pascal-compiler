namespace Lexer
{
    public enum LexemeType
    {
        Invalid, /*однострочные и многострочные комментарии*/
        Eof, /*конец файла*/
        Id, /*идентификаторы*/
        Keyword, /*ключевые слова*/
        Operation, /*знаки операций*/
        Separator, /*разделитель*/
        Integer, /*целое*/
        Double, /*вещественный*/
        String, /*строка*/
    }
}