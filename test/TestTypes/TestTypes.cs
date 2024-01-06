using TypedReflect;

namespace TestTypes;

public partial struct Point : ITypeShapeProvider<Point>
{
    public int X { get; }
    public SubPoint Sub { get; }

    public static ITypeShape<Point> Shape => new PointShape();
}

public partial record struct SubPoint(int A, string B) : ITypeShapeProvider<SubPoint>
{
    public static ITypeShape<SubPoint> Shape => new SubPointShape();
}

public record struct IntWrap(int Value) : ITypeShapeProvider<int>
{
    public static ITypeShape<int> Shape => new PrimitiveShape<int>();
}
public record struct StringWrap(string Value) : ITypeShapeProvider<string>
{
    public static ITypeShape<string> Shape => new PrimitiveShape<string>();
}

partial struct PointShape : ITypeShape<Point>
{
    public string Name => "Point";

    public void VisitFields<TVisitor>(TVisitor visitor) where TVisitor : IFieldVisitor
    {
    }

    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor
    {
        visitor.Visit(IntWrap.Shape, new XProperty());
        visitor.Visit(SubPoint.Shape, new SubPointProperty());
    }
    private struct XProperty : IProperty
    {
        public string Name => "X";
        IMethod? IProperty.GetMethod => new MethodShape(
            isPublic: true,
            method: new Func<Point, int>(GetX),
            paramCount: 0,
            invoke: (receiver, args) => GetX((Point)receiver!)
        );
        private static int GetX(Point p) => p.X;
    }
    private struct SubPointProperty : IProperty
    {
        public string Name => "Sub";
        IMethod? IProperty.GetMethod => new MethodShape(
            isPublic: true,
            method: new Func<Point, SubPoint>(GetSub),
            paramCount: 0,
            invoke: (receiver, args) => GetSub((Point)receiver!)
        );
        private static SubPoint GetSub(Point p) => p.Sub;
    }
}

struct SubPointShape : ITypeShape<SubPoint>
{
    public string Name => "SubPoint";

    public void VisitFields<TVisitor>(TVisitor visitor) where TVisitor : IFieldVisitor
    {
    }

    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor
    {
        visitor.Visit(IntWrap.Shape, new AProperty());
        visitor.Visit(StringWrap.Shape, new BProperty());
    }
    private struct AProperty : IProperty
    {
        public string Name => "A";
        IMethod? IProperty.GetMethod => new MethodShape(
            isPublic: true,
            method: new Func<SubPoint, int>(GetA),
            paramCount: 0,
            invoke: (receiver, args) => GetA((SubPoint)receiver!)
        );

        private static int GetA(SubPoint p) => p.A;
    }
    private struct BProperty : IProperty
    {
        public string Name => "B";
        IMethod? IProperty.GetMethod => new MethodShape(
            isPublic: true,
            method: GetB,
            paramCount: 0,
            invoke: (receiver, args) => GetB((SubPoint)receiver!)
        );
        private static string GetB(SubPoint p) => p.B;
    }
}

sealed class MethodShape(
    bool isPublic,
    Delegate method,
    int paramCount,
    Func<object?, object?[]?, object?> invoke) : IMethod
{
    bool IMethod.IsPublic => isPublic;

    Delegate IMethod.Method => method;

    int IMethod.ParameterCount => paramCount;

    object? IMethod.Invoke(object? receiver, params object?[]? args) => invoke(receiver, args);
}

public struct TupleShapeProvider<T1, T2, T1Provider, T2Provider> : ITypeShapeProvider<(T1, T2)>
    where T1Provider : ITypeShapeProvider<T1>
    where T2Provider : ITypeShapeProvider<T2>
{
    public static ITypeShape<(T1, T2)> Shape => new TupleShape<T1, T2, T1Provider, T2Provider>();
}

public struct TupleShape<T1, T2, T1Provider, T2Provider> : ITypeShape<(T1, T2)>
    where T1Provider : ITypeShapeProvider<T1>
    where T2Provider : ITypeShapeProvider<T2>
{
    string ITypeShape<(T1, T2)>.Name
    {
        get
        {
            var t1Desc = T1Provider.Shape;
            var t2Desc = T2Provider.Shape;
            return $"({t1Desc.Name}, {t2Desc.Name})";
        }
    }

    void ITypeShape<(T1, T2)>.VisitFields<TVisitor>(TVisitor visitor)
    {
        visitor.Visit(T1Provider.Shape, new Item1Field());
        visitor.Visit(T2Provider.Shape, new Item2Field());
    }

    void ITypeShape<(T1, T2)>.VisitProperties<TVisitor>(TVisitor visitor)
    { }

    private struct Item1Field : IField
    {
        public string Name => "Item1";
        public T1 GetValue((T1, T2) obj) => obj.Item1;
    }
    public struct Item2Field : IField
    {
        public string Name => "Item2";
        public T2 GetValue((T1, T2) obj) => obj.Item2;
    }
}