using TypedReflect;
using TestTypes;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("RefDesc");
var refDesc = ReflectionShapeProvider.GetShape<Point>();
Console.WriteLine(refDesc.Name);
refDesc.VisitProperties(new PrintPropsVisitor<Point>());


struct PrintPropsVisitor<TReceiver> : IPropertyVisitor
{
    public void Visit<T>(ITypeShape<T> shape, IProperty property)
    {
        Console.WriteLine($"Property {typeof(T)} {property.Name}");
        shape.VisitProperties(new PrintPropsVisitor<T>());
    }
}