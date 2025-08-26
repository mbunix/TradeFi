using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using ApiGateway.Interfaces;
using ApiGateway.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace ApiGateway.Middlewares;

public class CustomSqlInjectionChecker : ICustomSqlInjectionChecker
{
    private readonly SqlInjectionParams _Injectionparams;
    private readonly ILogger<CustomSqlInjectionChecker> _logger;

    public CustomSqlInjectionChecker(IOptions<SqlInjectionParams> Injectionparams, ILogger<CustomSqlInjectionChecker> logger)
    {
        _Injectionparams = Injectionparams.Value;
        _logger = logger;
    }

    public bool IsVulnerableToSqlInjection(string input)
    {
        _logger.LogInformation("Starting Validating IsVulnerableToSqlInjection {@input}", input);

        bool attackFound = false;

        // Check for common SQL injection patterns using regex
        if (BeginCheck(input))
        {
            _logger.LogError("SqlInjection attack: {@PropertyValue}", input);

            attackFound = true;
        }

        _logger.LogInformation("Completed Validating IsVulnerableToSqlInjection");

        return attackFound;
    }

    public bool IsVulnerableToSqlInjection<TModel>(TModel model) where TModel : class
    {
        _logger.LogInformation("Starting Validating IsVulnerableToSqlInjection {@model}", model);

        foreach (var property in typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.PropertyType == typeof(string))
            {
                var value = property.GetValue(model) as string;
                if (value != null && IsVulnerableToSqlInjection(value))
                {
                    _logger.LogError("SQL Injection attempt detected on property: {PropertyName} with value: {PropertyValue}", property.Name, value);
                    return true;
                }
            }
        }

        _logger.LogInformation("Completed Validating IsVulnerableToSqlInjection");

        return false;
    }

    public bool IsVulnerableToSqlInjection(string jsonBody ,out string attack)
    {
        _logger.LogInformation("Validating isVulnerableToSqlInjection {@model}", jsonBody);
        bool attackFound =  false;
        attack = "";
       
        var dynamicObject = JsonConvert.DeserializeObject<ExpandoObject>(jsonBody);
        foreach (var property in(IDictionary<string, object>)dynamicObject)
        {
            var propertyName = property.Key;
            var propertyValue = property.Value?.ToString();
            // Check for common SQL injection patterns using regex
            if (BeginCheck(propertyValue))
            {
                _logger.LogError("SqlInjection attack: {@PropertyName} {@PropertyValue}", propertyName, propertyValue);
                
                attack = propertyValue;
                attackFound = true;
            }
            if(attackFound)break;
        }
        _logger.LogInformation("Completed Validating isVulnerableToSqlInjection");
        return attackFound;
    }

    private bool BeginCheck(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false; 
        // Skip check if input contains only whitelisted keywords
        if (IsWhitelisted(input)) return false;

        return ContainsInjectionCharsOrKeywords(input) || IsDynamicSqlConstruction(input);
    }

    private bool IsDynamicSqlConstruction(string input)
    {
        var regex = new Regex($@"({string.Join("|", _Injectionparams.InjectionChars.Select(Regex.Escape))}|{string.Join("|", _Injectionparams.InjectionKeywords)})", RegexOptions.IgnoreCase);
        return regex.IsMatch(input);
    }

    private bool IsWhitelisted(string input)
    {
        return _Injectionparams.WhiteListKeywords.Any(keyword =>
            input.Equals(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private bool ContainsInjectionCharsOrKeywords(string input)
    {
        // Use a single regex to check for injection chars and keywords
        var regex = new Regex($@"({string.Join("|", _Injectionparams.InjectionChars.Select(Regex.Escape))}|{string.Join("|", _Injectionparams.InjectionKeywords)})", RegexOptions.IgnoreCase);
        return regex.IsMatch(input);
    }
}