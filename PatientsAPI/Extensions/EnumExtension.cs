using System.ComponentModel.DataAnnotations;

namespace P10___MédiLabo___Patients_API.Extensions;

public static class EnumExtension
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetField(enumValue.ToString())
            ?.GetCustomAttributes(typeof(DisplayAttribute), false)
            .FirstOrDefault() as DisplayAttribute;

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}