using System.ComponentModel.DataAnnotations;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Shared.ValidationAttributes
{
    public class MemberFormatAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue)) return false;

            return IsStringInMemberFormat(stringValue);
        }
    }
}
