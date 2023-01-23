namespace fc.micro.services.Components.FunctionRegistries
{
    public interface IFunctionRegistry
    {
        void Init<T>(Func<Type, Function> metaFunction);
        IEnumerable<Function> ListFunctions { get; }
        (bool success, Type handlerType) FindHandlerType(string messageName);
        (bool success, Type messageType) FindMessage(string messageName);
        object Info();
    }
}