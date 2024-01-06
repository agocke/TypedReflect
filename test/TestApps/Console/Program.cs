using TestTypes;
using TypedReflect;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Point desc:");
//var descriptor = Point.Descriptor;
//descriptor.VisitProperties(new PrintPropsVisitor<Point>());

Console.WriteLine();
Console.WriteLine("Tuple desc:");
var shape = TupleShapeProvider<int, SubPoint, IntWrap, SubPoint>.Shape;
shape.VisitProperties(new PrintPropsVisitor<(int, SubPoint)>());
shape.VisitFields(new PrintFieldsVisitor<(int, SubPoint)>());

struct PrintPropsVisitor<TReceiver> : IPropertyVisitor
{
    public void Visit<T>(ITypeShape<T> shape, IProperty property)
    {
        Console.WriteLine($"Property {typeof(T)} {property.Name}");
        shape.VisitProperties(new PrintPropsVisitor<T>());
    }
}

struct PrintFieldsVisitor<TReceiver> : IFieldVisitor
{
    public void Visit<T>(ITypeShape<T> shape, IField field)
    {
        Console.WriteLine($"Field {typeof(T)} {field.Name}");
        shape.VisitFields(new PrintFieldsVisitor<T>());
    }
}
