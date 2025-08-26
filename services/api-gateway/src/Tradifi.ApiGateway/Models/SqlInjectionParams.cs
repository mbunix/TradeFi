namespace ApiGateway.Models;

public class SqlInjectionParams
{
    public string[] InjectionChars { get; set; }
    public string[] InjectionKeywords { get; set; }
    public string[] DynamicSqlPatterns { get; set; }
    public string[] WhiteListKeywords { get; set; }
}