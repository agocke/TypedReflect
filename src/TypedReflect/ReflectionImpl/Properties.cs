
using System.Reflection;

namespace TypedReflect;

internal readonly struct ReflectionProperty<T, TReceiver>(PropertyInfo p) : IProperty
{
    public string Name => p.Name;

    public void VisitAttributes<TVisitor>(ref TVisitor visitor)
        where TVisitor : IAttributeVisitor
    {
    }
}
