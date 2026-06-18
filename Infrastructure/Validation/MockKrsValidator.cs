using ApplicationCore.Interfaces.Validation;

namespace Infrastructure.Validation;

public class MockKrsValidator : IKrsValidator
{
    public Task<bool> ValidateKrsAsync(string krs)
    {
        var valid = !string.IsNullOrEmpty(krs) && krs.Length == 10 && krs.All(char.IsDigit);
        return Task.FromResult(valid);
    }
}