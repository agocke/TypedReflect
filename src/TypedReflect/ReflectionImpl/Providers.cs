
namespace TypedReflect;

public static class ReflectionShapeProvider
{
    public static ITypeShape<T> GetShape<T>()
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return new PrimitiveShape<T>();
        }
        return ReflectionShape<T>.Instance;
    }

    internal static object GetShape(Type type)
    {
        if (type.IsPrimitive || type == typeof(string))
        {
            Activator.CreateInstance(typeof(PrimitiveShape<>).MakeGenericType(type));
        }
        return typeof(ReflectionShape<>).MakeGenericType(type).GetField("Instance")!.GetValue(null)!;
    }
}

internal class ReflectionShapeProvider<T> : ITypeShapeProvider<T>
{
    public static ITypeShape<T> Shape => ReflectionShapeProvider.GetShape<T>();
}