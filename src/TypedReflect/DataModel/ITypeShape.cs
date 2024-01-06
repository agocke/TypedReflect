
using System.Collections.Immutable;
using System.Reflection;

namespace TypedReflect;

public interface ITypeShapeProvider<TReceiver>
{
    static abstract ITypeShape<TReceiver> Shape { get; }
}

public interface ITypeShape<TReceiver>
{
    string Name { get; }

    public ImmutableArray<IConstructor> GetConstructors(BindingFlags flags)
    {
        var builder = ImmutableArray.CreateBuilder<IConstructor>();
        var visitor = new CtorVisitor(builder, flags);
        VisitConstructors(ref visitor);
        return builder.ToImmutable();
    }

    private struct CtorVisitor(ImmutableArray<IConstructor>.Builder builder, BindingFlags flags) : IConstructorVisitor
    {
        void IConstructorVisitor.Visit(IConstructor ctor)
        {
            switch (flags)
            {
                case BindingFlags.Public when !ctor.IsPublic:
                    return;
            }
            builder.Add(ctor);
        }
    }

    public void Visit<TVisitor>(TVisitor visitor)
        where TVisitor : ITypeVisitor
    { }
    public void VisitBaseType<TVisitor>(TVisitor visitor)
        where TVisitor : ITypeVisitor
    { }
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor
    { }
    public void VisitFields<TVisitor>(TVisitor visitor)
        where TVisitor : IFieldVisitor
    { }
    public void VisitConstructors<TVisitor>(ref TVisitor visitor)
        where TVisitor : IConstructorVisitor
    { }
    public void VisitTypeArguments<TVisitor>(ref TVisitor visitor)
        where TVisitor : ITypeVisitor
    { }
    public void VisitAttributes<TVisitor>(TVisitor visitor)
        where TVisitor : IAttributeVisitor
    { }
    public void VisitElementType<TVisitor>(TVisitor visitor)
        where TVisitor : ITypeVisitor
    { }
    public bool IsArray => false;
}

public interface ITypeVisitor
{
    bool Visit<T>(ITypeShape<T> shape);
}

public record class PrimitiveShape<T>() : ITypeShape<T>
{
    string ITypeShape<T>.Name => typeof(T).Name;

    void ITypeShape<T>.VisitFields<TVisitor>(TVisitor visitor)
    { }

    void ITypeShape<T>.VisitProperties<TVisitor>(TVisitor visitor)
    { }
}

public static class TypeExts
{
}