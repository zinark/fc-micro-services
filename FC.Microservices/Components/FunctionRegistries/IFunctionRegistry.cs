namespace FCMicroservices.Components.FunctionRegistries;

public interface IFunctionRegistry
{
    IEnumerable<Function> ListFunctions { get; }
    void Init<T>(Func<Type, Function> metaFunction);
    (bool success, Type handlerType) FindHandlerType(string messageName);
    (bool success, Type messageType) FindMessage(string messageName);
    object Info();
}