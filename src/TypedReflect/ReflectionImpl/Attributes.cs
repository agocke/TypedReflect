
using System.Linq.Expressions;
using System.Reflection;

namespace TypedReflect;

internal class ReflectionAttribute(CustomAttributeData data) : IAttribute
{
    int IAttribute.ArgumentsCount => data.ConstructorArguments.Count;

    delegate void VisitTypeDelegate<TVisitor>(ref TVisitor visitor, object shape);

    void IAttribute.VisitArguments<TVisitor>(ref TVisitor visitor)
    {
        foreach (var arg in data.ConstructorArguments)
        {
            switch (arg.Value)
            {
                case string s:
                    visitor.VisitString(s);
                    break;
                case int i:
                    visitor.VisitInt(i);
                    break;
                case double d:
                    visitor.VisitDouble(d);
                    break;
                case bool b:
                    visitor.VisitBool(b);
                    break;
                case Type t:
                    if (t.IsConstructedGenericType)
                    {
                        // Look for VisitType<T>(ITypeShape<T> shape) and substitute with the attribute type
                        var visitMethod = typeof(TVisitor).GetMethod("VisitType")!.MakeGenericMethod(t);

                        // Construct a stub to invoke the method on the visitor without boxing
                        var parameters = new ParameterExpression[] {
                            Expression.Parameter(typeof(TVisitor).MakeByRefType()),
                            Expression.Parameter(typeof(ITypeShape<>).MakeGenericType(t))
                        };

                        var call = Expression.Call(parameters[0], visitMethod, parameters[1]);

                        var @delegate = Expression.Lambda<VisitTypeDelegate<TVisitor>>(call, parameters);

                        var shape = ReflectionShapeProvider.GetShape(t);
                        @delegate.Compile()(ref visitor, shape);
                    }
                    else
                    {
                        visitor.VisitUnboundType(t);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}