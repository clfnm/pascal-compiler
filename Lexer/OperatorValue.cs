namespace Lexer
{
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
}