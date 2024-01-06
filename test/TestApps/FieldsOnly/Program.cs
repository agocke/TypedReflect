using TypedReflect;
using TestTypes;

// See https://aka.ms/new-console-template for more information

Console.WriteLine();
Console.WriteLine("Tuple desc:");
var desc2 = TupleShapeProvider<int, SubPoint, IntWrap, SubPoint>.Shape;
desc2.VisitFields(new PrintFieldsVisitor<(int, SubPoint)>());

struct PrintFieldsVisitor<TReceiver> : IFieldVisitor<TReceiver>
{
    public void Visit<T>(ITypeShape<T> shape, IField property)
    {
        Console.WriteLine($"Field {typeof(T)} {property.Name}");
        shape.VisitFields(new PrintFieldsVisitor<T>());
    }
}
