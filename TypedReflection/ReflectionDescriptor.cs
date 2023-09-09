
using System;
using System.Reflection;

public static class ReflectionDescriptorInfo
{
    public static ITypeDescriptor<T> GetDescriptor<T>()
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
        {
            return new PrimitiveDescriptor<T>(typeof(T).Name);
        }
        return new ReflectionDescriptor<T>();
    }
}

internal class ReflectionDescriptorProvider<T> : ITypeDescriptorProvider<T>
{
    public static ITypeDescriptor<T> Descriptor => ReflectionDescriptorInfo.GetDescriptor<T>();
}

internal class ReflectionDescriptor<T> : ITypeDescriptor<T>
{
    public string Name => typeof(T).Name;
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<T>
    {
        var visitMethod = typeof(TVisitor).GetMethod("Visit")!;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            VisitMember(visitMethod, visitor, property, property.PropertyType);
        }
    }
    public void VisitFields<TVisitor>(TVisitor visitor)
        where TVisitor : IFieldVisitor<T>
    {
        var visitMethod = typeof(TVisitor).GetMethod("Visit")!;
        var properties = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            VisitMember(visitMethod, visitor, property, property.FieldType);
        }
    }

    private static void VisitMember(MethodInfo visitMethod, object visitor, MemberInfo member, Type memberType)
    {
        visitMethod = visitMethod
            .MakeGenericMethod(memberType, typeof(ReflectionDescriptorProvider<>).MakeGenericType(memberType));
        var makeProperty = typeof(ReflectionProperty<,>)
            .MakeGenericType(memberType, typeof(T));
        var refProperty = makeProperty.GetConstructor(new[] { typeof(PropertyInfo) })!.Invoke(new[] { member });
        visitMethod.Invoke(visitor, new[] { refProperty });
    }
}

internal readonly struct ReflectionProperty<T, TReceiver>(PropertyInfo p) : IProperty<T, TReceiver>
{
    public string Name => p.Name;
    public T GetValue(TReceiver obj) => (T)p.GetValue(obj);
}