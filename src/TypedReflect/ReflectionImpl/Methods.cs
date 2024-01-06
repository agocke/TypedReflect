
using System.Collections.Immutable;
using System.Reflection;

namespace TypedReflect;

internal readonly struct ReflectionMethod(MethodInfo methodInfo) : IMethod
{
    public Delegate Method => null!;

    public bool IsPublic => methodInfo.IsPublic;

    public int ParameterCount => methodInfo.GetParameters().Length;

    public ImmutableArray<IParameter> GetParameters()
    {
        throw new NotImplementedException();
    }

    public object? Invoke(object? receiver, params object?[]? args) => methodInfo.Invoke(receiver, args);
}

internal readonly struct ReflectionCtor(ConstructorInfo ctorInfo) : IConstructor
{
    public Delegate Method => null!;

    public bool IsPublic => ctorInfo.IsPublic;

    public int ParameterCount => ctorInfo.GetParameters().Length;

    public ImmutableArray<IParameter> GetParameters()
    {
        throw new NotImplementedException();
    }

    public object Invoke(params object?[]? args) => ctorInfo.Invoke(args);
}