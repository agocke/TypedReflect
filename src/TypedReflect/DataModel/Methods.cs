
using System.Collections.Immutable;

namespace TypedReflect;

public interface IConstructorVisitor
{
    void Visit(IConstructor ctor);
}

public interface IConstructor
{
    public bool IsPublic { get; }
    Delegate Method { get; }
    public int ParameterCount { get; }
    public ImmutableArray<IParameter> GetParameters();
    public object Invoke(params object?[]? args);
}

public interface IParameter
{
    public string Name { get; }
    public void VisitType<TVisitor>(TVisitor visitor)
        where TVisitor : ITypeVisitor;
}

public interface IMethod
{
    public bool IsPublic { get; }
    Delegate Method { get; }
    public int ParameterCount { get; }

    public object? Invoke(object? receiver, params object?[]? args);
}