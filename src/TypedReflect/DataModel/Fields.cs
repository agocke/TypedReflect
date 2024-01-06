
namespace TypedReflect;

public interface IFieldVisitor
{
    void Visit<T>(ITypeShape<T> shape, IField property);
}

public interface IField
{
    string Name { get; }
}