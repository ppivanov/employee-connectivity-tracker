using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

    public class CollectionNotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;

            var collection = value as IEnumerable<object>;

            return collection.Any();
        }
    }
}
