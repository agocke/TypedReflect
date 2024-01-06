
using System.Reflection;

namespace TypedReflect;

internal sealed class ReflectionDescriptor<T> : ITypeShape<T>
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

    public void VisitConstructors<TVisitor>(TVisitor visitor)
        where TVisitor : IConstructorVisitor<T>
    {
        var baseVisitMethod = typeof(TVisitor).GetMethod("Visit")!;
        var ctors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        foreach (var ctor in ctors)
        {
        }
    }

    private static void VisitMember(MethodInfo visitMethod, object visitor, MemberInfo member, Type memberType)
    {
        visitMethod = visitMethod
            .MakeGenericMethod(memberType, typeof(ReflectionShapeProvider<>).MakeGenericType(memberType));
        var makeProperty = typeof(ReflectionProperty<,>)
            .MakeGenericType(memberType, typeof(T));
        var refProperty = makeProperty.GetConstructor(new[] { typeof(PropertyInfo) })!.Invoke(new[] { member });
        visitMethod.Invoke(visitor, new[] { refProperty });
    }
}
