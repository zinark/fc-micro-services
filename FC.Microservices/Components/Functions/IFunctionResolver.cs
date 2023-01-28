using FCMicroservices.Components.Functions;

namespace FCMicroservices.Components.FunctionRegistries;

public interface IFunctionResolver
{
    string Resolve(Function f);
}