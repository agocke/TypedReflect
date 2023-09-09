

public interface IPropertyVisitor<TReceiver>
{
    void Visit<T, TProvider>(IProperty<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>;
}

public interface IProperty<T, TReceiver>
{
    string Name { get; }
    T GetValue(TReceiver obj);
}

public static class IPropertyVisitorExt
{
    public static void Visit<T, TReceiver>(this IPropertyVisitor<TReceiver> visitor, IProperty<T, TReceiver> property)
        where T : ITypeDescriptorProvider<T>
        => visitor.Visit<T, T>(property);
}
