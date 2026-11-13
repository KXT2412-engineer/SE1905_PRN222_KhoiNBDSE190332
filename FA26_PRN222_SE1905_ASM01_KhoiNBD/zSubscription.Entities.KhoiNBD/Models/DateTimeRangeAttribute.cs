using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace zSubscription.Entities.KhoiNBD.Models;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DateTimeRangeAttribute : ValidationAttribute
{
    private readonly DateTime _minimum;
    private readonly DateTime _maximum;

    public DateTimeRangeAttribute(string minimum, string maximum)
    {
        _minimum = DateTime.ParseExact(minimum, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        _maximum = DateTime.ParseExact(maximum, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is DateTime date && date.Date >= _minimum.Date && date.Date <= _maximum.Date;
    }
}
