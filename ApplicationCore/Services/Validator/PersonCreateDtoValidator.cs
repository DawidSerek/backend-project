using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models.Create;
using FluentValidation;

namespace ApplicationCore.Services.Validator;

public class PersonCreateDtoValidator : AbstractValidator<PersonCreateDto>
{
    public PersonCreateDtoValidator(IPositionRepository positions)
    {

        _ = RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required")
            .MustAsync(async (pos, _) => await positions.ExistsAsync(pos ?? ""))
            .WithMessage("Position '{PropertyValue}' is not in the system. " +
            "Call GET /api/positions for the list of valid positions.");
    }
}

