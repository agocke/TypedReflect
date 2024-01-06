
using System;
using System.Reflection;

namespace TypedReflect;

public static class ReflectionProvider
{
    public static ITypeShape<T> GetShape<T>()
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return new PrimitiveShape<T>();
        }
        return new ReflectionDescriptor<T>();
    }
}

internal sealed class ReflectionShapeProvider<T> : ITypeShapeProvider<T>
{
    public static ITypeShape<T> Shape => ReflectionProvider.GetShape<T>();
}