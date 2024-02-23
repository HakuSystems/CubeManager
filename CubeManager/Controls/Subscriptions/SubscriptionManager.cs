using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CubeManager.Controls.Subscriptions;

public partial class SubscriptionManager
{
    public enum PeriodType
    {
        Day,
        Week,
        Month,
        Year
    }
    
    
    public static bool ValidateString(string stringToValidate)
    {
        var invalidChars = SpecialChars();
        return !invalidChars.IsMatch(stringToValidate);
    }

    [GeneratedRegex("[0-9!@#$%^&*()_+=\\[{\\]};:<>|./?,-]")]
    private static partial Regex SpecialChars();
    
    //Validate Date
    public static bool ValidateDate(string date)
    {
        return DateTime.TryParse(date, out _);
    }
    
    //Validate Only Nubers allowed
    public static bool ValidateNumber(string number)
    {
        return int.TryParse(number, out _);
    }
}