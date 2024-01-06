
using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace TypedReflect;

internal sealed class ReflectionShape<T> : ITypeShape<T>
{
    // Instances are guaranteed to be reference equal
    public static readonly ReflectionShape<T> Instance = new();

    private ReflectionShape() { }

    public string Name => typeof(T).Name;
    void ITypeShape<T>.VisitProperties<TVisitor>(TVisitor visitor)
    {
        var visitMethod = typeof(IPropertyVisitor).GetMethod("Visit")!;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            VisitMember(visitMethod, visitor, property, property.PropertyType);
        }
    }
    void ITypeShape<T>.VisitFields<TVisitor>(TVisitor visitor)
    {
        var visitMethod = typeof(IFieldVisitor).GetMethod("Visit")!;
        var properties = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            VisitMember(visitMethod, visitor, property, property.FieldType);
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

    void ITypeShape<T>.VisitTypeArguments<TVisitor>(ref TVisitor visitor)
    {
        var visitMethod = typeof(ITypeVisitor).GetMethod("Visit")!;
        foreach (var typeArg in typeof(T).GenericTypeArguments)
        {
            var subVisit = visitMethod
                .MakeGenericMethod(typeArg, typeof(ReflectionShapeProvider<>).MakeGenericType(typeArg));
            subVisit.Invoke(visitor, null);
        }
    }

    void ITypeShape<T>.VisitConstructors<TVisitor>(ref TVisitor visitor)
    {
        var ctors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        foreach (var ctor in ctors)
        {
            visitor.Visit(new ReflectionCtor(ctor));
        }
    }
}
