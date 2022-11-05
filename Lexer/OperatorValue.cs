namespace Lexer
{
    public enum OperatorValue /*знаки операций*/
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
}