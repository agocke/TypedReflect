
public interface IConstructorVisitor<TReceiver>
{
    void Visit(IConstructor<TReceiver> constructor);
}

public interface IConstructor<TReceiver>
{
    Delegate Method { get; }
}