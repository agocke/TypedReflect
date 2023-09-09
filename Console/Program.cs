// See https://aka.ms/new-console-template for more information
Console.WriteLine("Point desc:");
var descriptor = Point.Descriptor;
descriptor.VisitProperties(new PrintPropsVisitor<Point>());

Console.WriteLine();
Console.WriteLine("Tuple desc:");
var desc2 = TupleDescriptorProvider<int, SubPoint, IntWrap, SubPoint>.Descriptor;
desc2.VisitFields(new PrintFieldsVisitor<(int, SubPoint)>());

Console.WriteLine();
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

struct PrintFieldsVisitor<TReceiver> : IFieldVisitor<TReceiver>
{
    public void Visit<T, TProvider>(IField<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>
    {
        Console.WriteLine($"Field {typeof(T)} {property.Name}");
        var desc = TProvider.Descriptor;
        desc.VisitProperties(new PrintPropsVisitor<T>());
    }
}

struct TupleDescriptorProvider<T1, T2, T1Provider, T2Provider> : ITypeDescriptorProvider<(T1, T2)>
    where T1Provider : ITypeDescriptorProvider<T1>
    where T2Provider : ITypeDescriptorProvider<T2>
{
    public static ITypeDescriptor<(T1, T2)> Descriptor => new TupleDescriptor<T1, T2, T1Provider, T2Provider>();
}

struct TupleDescriptor<T1, T2, T1Provider, T2Provider> : ITypeDescriptor<(T1, T2)>
    where T1Provider : ITypeDescriptorProvider<T1>
    where T2Provider : ITypeDescriptorProvider<T2>
{
    string ITypeDescriptor<(T1, T2)>.Name
    {
        get
        {
            var t1Desc = T1Provider.Descriptor;
            var t2Desc = T2Provider.Descriptor;
            return $"({t1Desc.Name}, {t2Desc.Name})";
        }
    }

    void ITypeDescriptor<(T1, T2)>.VisitFields<TVisitor>(TVisitor visitor)
    {
        visitor.Visit<T1, T1Provider>(new Item1Field());
        visitor.Visit<T2, T2Provider>(new Item2Field());
    }

    void ITypeDescriptor<(T1, T2)>.VisitProperties<TVisitor>(TVisitor visitor)
    { }

    private struct Item1Field : IField<T1, (T1, T2)>
    {
        public string Name => "Item1";
        public T1 GetValue((T1, T2) obj) => obj.Item1;
    }
    public struct Item2Field : IField<T2, (T1, T2)>
    {
        public string Name => "Item2";
        public T2 GetValue((T1, T2) obj) => obj.Item2;
    }
}

partial struct Point : ITypeDescriptorProvider<Point>
{
    public int X { get; }
    public SubPoint Sub { get; }

    public static ITypeDescriptor<Point> Descriptor => new PointDescriptor();
}

partial record struct SubPoint(int A, string B) : ITypeDescriptorProvider<SubPoint>
{
    public static ITypeDescriptor<SubPoint> Descriptor => new SubPointDescriptor();
}

public record struct IntWrap(int Value) : ITypeDescriptorProvider<int>
{
    public static ITypeDescriptor<int> Descriptor => new PrimitiveDescriptor<int>();
}
public record struct StringWrap(string Value) : ITypeDescriptorProvider<string>
{
    public static ITypeDescriptor<string> Descriptor => new PrimitiveDescriptor<string>();
}

partial struct PointDescriptor : ITypeDescriptor<Point>
{
    public string Name => "Point";

    public void VisitFields<TVisitor>(TVisitor visitor) where TVisitor : IFieldVisitor<Point>
    {
    }

    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<Point>
    {
        visitor.Visit<int, IntWrap>(new XProperty());
        visitor.Visit(new SubPointProperty());
    }
    private struct XProperty : IProperty<int, Point>
    {
        public string Name => "X";
        public int GetValue(Point p) => p.X;
    }
    private struct SubPointProperty : IProperty<SubPoint, Point>
    {
        public string Name => "Sub";
        public SubPoint GetValue(Point p) => p.Sub;
    }
}

struct SubPointDescriptor : ITypeDescriptor<SubPoint>
{
    public string Name => "SubPoint";

    public void VisitFields<TVisitor>(TVisitor visitor) where TVisitor : IFieldVisitor<SubPoint>
    {
    }

    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<SubPoint>
    {
        visitor.Visit<int, IntWrap>(new AProperty());
        visitor.Visit<string, StringWrap>(new BProperty());
    }
    private struct AProperty : IProperty<int, SubPoint>
    {
        public string Name => "A";
        public int GetValue(SubPoint p) => p.A;
    }
    private struct BProperty : IProperty<string, SubPoint>
    {
        public string Name => "B";
        public string GetValue(SubPoint p) => p.B;
    }
}