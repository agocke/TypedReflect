
namespace TypedReflect;

public record struct PrimitiveShape<T>() : ITypeShape<T>
{
    public string Name => typeof(T).Name;

    public void VisitFields<TVisitor>(TVisitor visitor) where TVisitor : IFieldVisitor<T>
    { }

    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<T>
    { }

    public void VisitConstructors<TVisitor>(TVisitor visitor)
        where TVisitor : IConstructorVisitor<T>
    { }
}