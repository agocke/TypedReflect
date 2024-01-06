
namespace TypedReflect;

public interface IAttribute
{
}

public interface IAttributeVisitor
{
    void Visit<T>(ITypeShape<T> shape, IAttribute attr);
}