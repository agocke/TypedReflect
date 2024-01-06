
namespace TypedReflect;

public interface IProperty
{
    string Name { get; }

    void VisitAttributes<TVisitor>(ref TVisitor visitor)
        where TVisitor : IAttributeVisitor
    { }
}

public interface IField
{
    string Name { get; }
}

public interface IPropertyVisitor<TReceiver>
{
    void Visit<T>(ITypeShape<T> shape, IProperty property);
}

public interface IFieldVisitor<TReceiver>
{
    void Visit<T>(ITypeShape<T> shape, IField field);
}