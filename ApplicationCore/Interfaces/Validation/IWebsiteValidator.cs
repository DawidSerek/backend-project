namespace ApplicationCore.Interfaces.Validation;

public interface IWebsiteValidator
{
    Task<bool> ValidateAsync(string url);
}