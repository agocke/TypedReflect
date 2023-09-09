

public interface IFieldVisitor<TReceiver>
{
    void Visit<T, TProvider>(IField<T, TReceiver> property)
        where TProvider : ITypeDescriptorProvider<T>;
}

public interface IField<T, TReceiver>
{
    string Name { get; }
    T GetValue(TReceiver obj);
}

public static class IFieldVisitorExt
{
    public static void Visit<T, TReceiver>(this IFieldVisitor<TReceiver> visitor, IField<T, TReceiver> property)
        where T : ITypeDescriptorProvider<T>
        => visitor.Visit<T, T>(property);
}
