using System.Linq.Expressions;
using System.Reflection;

namespace TypedReflect;

internal readonly struct ReflectionProperty<T, TReceiver>(PropertyInfo p) : IProperty
{
    string IProperty.Name => p.Name;

    IMethod? IProperty.GetMethod => p.GetMethod is null ? null : new ReflectionMethod(p.GetMethod);

    IMethod? IProperty.SetMethod => p.SetMethod is null ? null : new ReflectionMethod(p.SetMethod);

    void IProperty.VisitAttributes<TVisitor>(ref TVisitor visitor)
    {
        var visitMethod = typeof(IAttributeVisitor).GetMethod("Visit")!;
        foreach (var attrData in p.CustomAttributes)
        {
            var attrType = attrData.AttributeType;

            // Substitute the generic method with the attribute type
            var subVisit = visitMethod.MakeGenericMethod(attrType);

            // Construct a stub to invoke the method on the visitor without boxing
            var parameters = new ParameterExpression[] {
                Expression.Parameter(typeof(TVisitor).MakeByRefType()),
                Expression.Parameter(typeof(object)),
                Expression.Parameter(typeof(IAttribute))
            };

            var cast = Expression.TypeAs(parameters[1], typeof(ITypeShape<>).MakeGenericType(attrType));

            var call = Expression.Call(parameters[0], subVisit, cast, parameters[2]);
            var @delegate = Expression.Lambda<AttrPredicate<TVisitor>>(call, parameters);

            var attrInstance = new ReflectionAttribute(attrData);
            var shape = ReflectionShapeProvider.GetShape(attrType);
            @delegate.Compile()(ref visitor, shape, attrInstance);
        }
    }

    delegate bool AttrPredicate<TVisitor>(ref TVisitor visitor, object shape, IAttribute attribute)
        where TVisitor : IAttributeVisitor;
}