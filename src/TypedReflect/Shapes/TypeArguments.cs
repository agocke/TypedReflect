
namespace TypedReflect;

public interface ITypeArgumentVisitor
{
    void Visit<T, TProvider>(ITypeShape<T> shape);
}