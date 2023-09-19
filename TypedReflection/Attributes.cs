
public interface IAttributeVisitor
{
    public void Visit<TAttribute, TProvider>(TAttribute visitor)
        where TAttribute : IAttribute
        where TProvider : ITypeDescriptorProvider<TAttribute>;
}

public interface IAttribute
{
    string Name { get; }
}