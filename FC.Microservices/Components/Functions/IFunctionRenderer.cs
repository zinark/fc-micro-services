using FCMicroservices.Components.FunctionRegistries;

namespace FCMicroservices.Components.Functions;

public interface IFunctionRenderer
{
    public string Render(Function f);
}