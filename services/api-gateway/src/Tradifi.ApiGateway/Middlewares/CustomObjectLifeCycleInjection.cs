using ApiGateway.Interfaces;

namespace ApiGateway.Middlewares;

public static class CustomObjectLifeCycleInjection
{
    public static void InjectObjectLifeCycle( this IServiceCollection services)
    {
       services.AddSingleton<ICustomSqlInjectionChecker, CustomSqlInjectionChecker>();
    }
}