
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
    void Visit<T>(IProperty<T, TReceiver> property)
        where T : ITypeDescriptorProvider<T>;
}

public interface IProperty<T, TReceiver>
{
    string Name { get; }
    T GetValue(TReceiver obj);
}