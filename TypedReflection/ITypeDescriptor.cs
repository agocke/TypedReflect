using System.Globalization;
using System.Reflection;

public interface ITypeDescriptorProvider<TReceiver>
{
    static abstract ITypeDescriptor<TReceiver> Descriptor { get; }
}

public interface ITypeDescriptor<TReceiver>
{
    string Name { get; }
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<TReceiver>;
}

public record struct PrimitiveDescriptor<T>(string Name) : ITypeDescriptor<T>
{
    /// <summary>
    /// Primitive types have no properties
    /// </summary>
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<T>
    { }
}


public interface IPropertyVisitor<TReceiver>
{
    void Visit<T, TProvider>(IProperty<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>;
}

public static class IPropertyVisitorExt
{
    public static void Visit<T, TReceiver>(this IPropertyVisitor<TReceiver> visitor, IProperty<T, TReceiver> property)
        where T : ITypeDescriptorProvider<T>
        => visitor.Visit<T, T>(property);
}

public interface IProperty<T, TReceiver>
{
    string Name { get; }
    T GetValue(TReceiver obj);
}

public static class ReflectionDescriptorInfo
{
    public static ITypeDescriptor<T> GetDescriptor<T>()
    {
        if (typeof(T).IsPrimitive)
        {
            return new PrimitiveDescriptor<T>(typeof(T).Name);
        }
        return new ReflectionDescriptor<T>();
    }
}

record class ReflectionDescriptor<T> : ITypeDescriptor<T>
{
    public string Name => typeof(T).Name;
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<T>
    {
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var visitMethod = typeof(TVisitor).GetMethod("Visit")!
                .MakeGenericMethod(property.PropertyType);
            var makeProperty = typeof(ReflectionProperty<,>)
                .MakeGenericType(property.PropertyType, typeof(T));
            var refProperty = makeProperty.GetConstructor(new[] { typeof(PropertyInfo) })!.Invoke(new[] { property });
            visitMethod.Invoke(visitor, new[] { refProperty });
        }
    }

    public readonly struct ReflectionProperty<T, TReceiver>(PropertyInfo p) : IProperty<T, TReceiver>
    {
        public string Name => p.Name;
        public T GetValue(TReceiver obj) => (T)p.GetValue(obj);
    }
}