namespace TypedReflect;

public interface IAttributeVisitor
{
    /// <summary>
    /// Visit each attribute. Returns true to continue, false to stop.
    /// </summary>
    public bool Visit<TAttrType>(ITypeShape<TAttrType> shape, IAttribute attribute);
}

public interface IAttribute
{
    public int ArgumentsCount { get; }
    public void VisitArguments<TVisitor>(ref TVisitor visitor)
        where TVisitor : IAttributeArgumentVisitor;
}

public interface IAttributeArgumentVisitor
{
    public void VisitString(string value) { }
    public void VisitInt(int value) { }
    public void VisitDouble(double value) { }
    public void VisitBool(bool value) { }
    public void VisitType<T>(ITypeShape<T> shape) { }
    public void VisitUnboundType(Type t) { }
}