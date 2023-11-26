namespace FCMicroservices.Components.Functions;

public interface IFunctionRegistry
{
    IEnumerable<Function> ListFunctions { get; }
    Function FindFunction(string messageName);
    void Init<T>(Func<Type, Function> metaFunction);
    (bool success, Type handlerType) FindHandlerType(string messageName);
    (bool success, Type messageType) FindMessage(string messageName);
    object Info(string domainPrefix);
}