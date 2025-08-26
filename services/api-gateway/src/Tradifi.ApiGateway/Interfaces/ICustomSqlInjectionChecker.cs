namespace ApiGateway.Interfaces;

public interface ICustomSqlInjectionChecker
{
    bool IsVulnerableToSqlInjection(string input);
    bool IsVulnerableToSqlInjection<TModel>(TModel model) where TModel : class;
    bool IsVulnerableToSqlInjection(string jsonBody, out string attack); 
}