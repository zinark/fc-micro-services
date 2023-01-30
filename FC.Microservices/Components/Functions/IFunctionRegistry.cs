using FCMicroservices.Components.Functions;

namespace FCMicroservices.Components.FunctionRegistries;

public interface IFunctionRegistry
{
    IEnumerable<Function> ListFunctions { get; }
    Function FindFunction(string messageName);
    void Init<T>(Func<Type, Function> metaFunction, IEnumerable<Type> allTypes = null);
    (bool success, Type handlerType) FindHandlerType(string messageName);
    (bool success, Type messageType) FindMessage(string messageName);
    object Info();
}