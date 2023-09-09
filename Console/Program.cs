// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
//var descriptor = Point.Descriptor;
//descriptor.VisitProperties(new PrintVisitor<Point>());
var descriptor = ReflectionDescriptorInfo.GetDescriptor<Point>();
descriptor.VisitProperties(new PrintVisitor<Point>());

struct PrintVisitor<TReceiver> : IPropertyVisitor<TReceiver>
{
    public void Visit<T, TProvider>(IProperty<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>
    {
        Console.WriteLine($"{property.Name}");
        var desc = TProvider.Descriptor;
        desc.VisitProperties(new PrintVisitor<T>());
    }
}

record struct IntWrap(int Value) : ITypeDescriptorProvider<IntWrap>
{
    public static ITypeDescriptor<IntWrap> Descriptor => new PrimitiveDescriptor<IntWrap>("Int");
}

record struct StringWrap(string Value) : ITypeDescriptorProvider<StringWrap>
{
    public static ITypeDescriptor<StringWrap> Descriptor => new PrimitiveDescriptor<StringWrap>("String");
}

partial struct Point : ITypeDescriptorProvider<Point>
{
    public IntWrap X;
    public SubPoint Sub;

    public static ITypeDescriptor<Point> Descriptor => new PointDescriptor();
}

partial record struct SubPoint(IntWrap A, StringWrap B) : ITypeDescriptorProvider<SubPoint>
{
    public static ITypeDescriptor<SubPoint> Descriptor => new SubPointDescriptor();
}

partial struct PointDescriptor : ITypeDescriptor<Point>
{
    public string Name => "Point";
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<Point>
    {
        visitor.Visit(new XProperty());
        visitor.Visit(new SubPointProperty());
    }
    private struct XProperty : IProperty<IntWrap, Point>
    {
        public string Name => "X";
        public IntWrap GetValue(Point p) => p.X;
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
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<SubPoint>
    {
        visitor.Visit(new AProperty());
        visitor.Visit(new BProperty());
    }
    private struct AProperty : IProperty<IntWrap, SubPoint>
    {
        public string Name => "A";
        public IntWrap GetValue(SubPoint p) => p.A;
    }
    private struct BProperty : IProperty<StringWrap, SubPoint>
    {
        public string Name => "B";
        public StringWrap GetValue(SubPoint p) => p.B;
    }
}