namespace Pizzeria.Application.Core
{
    public abstract class BaseService
    {
        protected static ServiceResult Ok(object? data = null, string? message = null)
        {
            return new ServiceResult { Success = true, Message = message, Data = data };
        }

        protected static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }

        protected static ServiceResult Fail(IEnumerable<string> errors)
        {
            return Fail(string.Join(" ", errors));
        }

        protected static void ValidateRequired(List<string> errors, string? value, string field, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"The {field} field is required.");
                return;
            }

            ValidateLength(errors, value, field, maxLength);
        }

        protected static void ValidateLength(List<string> errors, string? value, string field, int maxLength)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maxLength)
                errors.Add($"The {field} field cannot exceed {maxLength} characters.");
        }

        protected static void ValidateId(List<string> errors, int value, string field)
        {
            if (value <= 0)
                errors.Add($"The {field} field must be greater than zero.");
        }

        protected static void ValidateEnum<TEnum>(List<string> errors, TEnum value, string field)
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
                errors.Add($"The {field} field does not have a valid value.");
        }
    }
}
