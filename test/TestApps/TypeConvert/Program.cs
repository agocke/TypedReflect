// See https://aka.ms/new-console-template for more information

using TypedReflect;

var x = new IntWrap(5);
Serializer.Serialize(x);
var p = new TypeWithConvert() { P1 = 3, P = 11 };
Serializer.Serialize(p);

static class Serializer
{
    public static void Serialize<T>(T t)
    {
        if (t is IFakeSerialize s)
        {
            s.Serialize();
            return;
        }

        // Primitives
        if (t is int i)
        {
            Console.Write(i);
        }

        // Serialize each of the properties
        var desc = ReflectionShapeProvider.GetShape<T>();
        var propVisitor = new SerializePropsVisitor<T>(t);
        desc.VisitProperties(propVisitor);
    }
}

interface IFakeSerialize
{
    void Serialize();
}

record struct IntWrap(int Value) : IFakeSerialize
{
    public void Serialize()
    {
        Console.WriteLine("WrappedInt: " + Value);
    }
}

class TypeWithConvert
{
    public required int P1 { get; init; }

    [Converter<IntWrap>]
    public int P { get; init; }
}

class Converter<T> : Attribute;

struct SerializePropsVisitor<TReceiver> : IPropertyVisitor
{
    private TReceiver _receiver;
    public SerializePropsVisitor(TReceiver receiver) => _receiver = receiver;

    public void Visit<T>(ITypeShape<T> shape, IProperty property)
    {
        Console.Write($"Property {property.Name}, type {shape.Name}: ");

        var propVal = (T)property.GetMethod!.Invoke(_receiver)!;

        var attrCheck = new AttributeCheckVisitor<T>(propVal);
        property.VisitAttributes(ref attrCheck);

        if (!attrCheck.Found)
        {
            Serializer.Serialize(propVal);
        }
        Console.WriteLine();
    }
}

class AttributeCheckVisitor<TArg>(TArg arg) : IAttributeVisitor
{
    public bool Found = false;
    public bool Visit<TAttr>(ITypeShape<TAttr> shape, IAttribute attr)
    {
        if (shape.Name == "Converter`1")
        {
            var argsVisitor = new ArgsVisitor(arg);
            shape.VisitTypeArguments(ref argsVisitor);
            Found = true;
            return false;
        }
        return true;
    }

    class ArgsVisitor(TArg arg2) : ITypeVisitor
    {
        private int _count = 0;
        bool ITypeVisitor.Visit<T>(ITypeShape<T> shape)
        {
            if (_count != 0)
            {
                throw new InvalidOperationException("Expected only one type argument");
            }
            _count++;
            var ctorVisitor = new CtorVisitor<Func<TArg, T>>();
            shape.VisitConstructors(ref ctorVisitor);
            if (ctorVisitor.Func is not null)
            {
                Serializer.Serialize(ctorVisitor.Func(arg2));
            }
            return false;
        }

        class CtorVisitor<TFunc> : IConstructorVisitor
            where TFunc : Delegate
        {
            public TFunc? Func { get; private set; }
            void IConstructorVisitor.Visit(IConstructor ctor)
            {
                if (Func is not null)
                {
                    return;
                }
                if (ctor.Method is TFunc func)
                {
                    Func = func;
                }
            }
        }
    }
}