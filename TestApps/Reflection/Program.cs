using TestTypes;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("RefDesc");
var refDesc = ReflectionDescriptorInfo.GetDescriptor<Point>();
Console.WriteLine(refDesc.Name);
refDesc.VisitProperties(new PrintPropsVisitor<Point>());


struct PrintPropsVisitor<TReceiver> : IPropertyVisitor<TReceiver>
{
    public void Visit<T, TProvider>(IProperty<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>
    {
        Console.WriteLine($"Property {typeof(T)} {property.Name}");
        var desc = TProvider.Descriptor;
        desc.VisitProperties(new PrintPropsVisitor<T>());
    }
}