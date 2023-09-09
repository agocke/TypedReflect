
public interface ITypeDescriptorProvider<TReceiver>
{
    static abstract ITypeDescriptor<TReceiver> Descriptor { get; }
}

public interface ITypeDescriptor<TReceiver>
{
    string Name { get; }
    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<TReceiver>;
    public void VisitFields<TVisitor>(TVisitor visitor)
        where TVisitor : IFieldVisitor<TReceiver>;
}

public record struct PrimitiveDescriptor<T>(string Name) : ITypeDescriptor<T>
{
    public void VisitFields<TVisitor>(TVisitor visitor) where TVisitor : IFieldVisitor<T>
    { }

    public void VisitProperties<TVisitor>(TVisitor visitor)
        where TVisitor : IPropertyVisitor<T>
    { }
}
