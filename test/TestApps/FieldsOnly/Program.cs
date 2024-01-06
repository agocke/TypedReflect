using TypedReflect;
using TestTypes;

// See https://aka.ms/new-console-template for more information

Console.WriteLine();
Console.WriteLine("Tuple desc:");
var shape = TupleShapeProvider<int, SubPoint, IntWrap, SubPoint>.Shape;
shape.VisitFields(new PrintFieldsVisitor<(int, SubPoint)>());

struct PrintFieldsVisitor<TReceiver> : IFieldVisitor
{
    public void Visit<T>(ITypeShape<T> shape, IField property)
    {
        Console.WriteLine($"Field {typeof(T)} {property.Name}");
        shape.VisitFields(new PrintFieldsVisitor<T>());
    }
}
