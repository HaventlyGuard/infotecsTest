using System.ComponentModel.DataAnnotations;
using InfotecsBackend.Models.DTO.Session;
using Semver;

namespace InfotecsBackend.Models.DTO.Validations;

public class ValidSessionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var session = validationContext.ObjectInstance as SessionResponse;
        
        if (session == null)
            return ValidationResult.Success;
        
        if (session.StartTime >= session.EndTime)
            return new ValidationResult("Время включения должно быть раньше времени выключения");

        if (!SemVersion.TryParse(session.Version, SemVersionStyles.Any, out _))
            return new ValidationResult("Версия должна быть в формате SemVer");
        
        return ValidationResult.Success;
    }
}