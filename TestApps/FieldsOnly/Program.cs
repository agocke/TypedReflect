using TestTypes;

// See https://aka.ms/new-console-template for more information

Console.WriteLine();
Console.WriteLine("Tuple desc:");
var desc2 = TupleDescriptorProvider<int, SubPoint, IntWrap, SubPoint>.Descriptor;
desc2.VisitFields(new PrintFieldsVisitor<(int, SubPoint)>());

struct PrintFieldsVisitor<TReceiver> : IFieldVisitor<TReceiver>
{
    public void Visit<T, TProvider>(IField<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>
    {
        Console.WriteLine($"Field {typeof(T)} {property.Name}");
        var desc = TProvider.Descriptor;
        desc.VisitFields(new PrintFieldsVisitor<T>());
    }
}
