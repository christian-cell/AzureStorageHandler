using FluentValidation;
using NetNinja.AzureStorageHandler.Constants;

namespace NetNinja.AzureStorageHandler.Validators
{
    public class ContainerCreateValidator : AbstractValidator<string>
    {
        public ContainerCreateValidator()
        {
            RuleFor(name => name)
                .NotNull().WithErrorCode(ErrorCode.Null)
                .NotEmpty().WithMessage("The name must not be empty.").WithErrorCode(ErrorCode.Empty)
                .MinimumLength(3).WithMessage("The name must be between 3 and 63 characters long.").WithErrorCode(ErrorCode.Invalid)
                .MaximumLength(63).WithMessage("The name must be between 3 and 63 characters long.").WithErrorCode(ErrorCode.Invalid)
                .Must(name => name.All(char.IsLower)).WithMessage("The name may only contain lowercase letters.").WithErrorCode(ErrorCode.Invalid)
                .Must(name => name.All(c => char.IsLetterOrDigit(c) || c == '-')).WithErrorCode(ErrorCode.Invalid).WithMessage("The name may only contain lowercase letters, numbers, and hyphens.")
                .Must(name => !name.StartsWith('-') && !name.EndsWith('-')).WithErrorCode(ErrorCode.Invalid).WithMessage("Each hyphen must be preceded and followed by a non-hyphen character.")
                .Must(name => !name.Contains("--")).WithErrorCode(ErrorCode.Invalid).WithMessage("Each hyphen must be preceded and followed by a non-hyphen character.")
                .Must(name => name.Any(char.IsLetterOrDigit)).WithErrorCode(ErrorCode.Invalid).WithMessage("The name must begin with a letter or a number.");
        }
    }
};

