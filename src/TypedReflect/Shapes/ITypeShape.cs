
namespace TypedReflect;

public interface ITypeShapeProvider<TReceiver>
{
    static abstract ITypeShape<TReceiver> Shape { get; }
}

public interface ITypeShape<TReceiver>
{
    string Name { get; }
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<TReceiver>;
    public void VisitFields<TVisitor>(TVisitor visitor)
        where TVisitor : IFieldVisitor<TReceiver>;
    public void VisitConstructors<TVisitor>(TVisitor visitor)
        where TVisitor : IConstructorVisitor<TReceiver>
    { }
    public void VisitTypeArguments<TVisitor>(TVisitor visitor)
        where TVisitor : ITypeArgumentVisitor
    { }
}