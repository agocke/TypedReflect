
namespace TypedReflect;

public interface IPropertyVisitor
{
    void Visit<TProp>(ITypeShape<TProp> shape, IProperty property);
}

public interface IProperty
{
    string Name { get; }
    public IMethod? GetMethod { get; }
    public IMethod? SetMethod => null;

    public void VisitAttributes<TVisitor>(ref TVisitor visitor)
        where TVisitor : IAttributeVisitor
    { }
}