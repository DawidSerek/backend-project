namespace ApplicationCore.Interfaces.Validation;

public interface IKrsValidator
{
    Task<bool> ValidateKrsAsync(string krs);
}