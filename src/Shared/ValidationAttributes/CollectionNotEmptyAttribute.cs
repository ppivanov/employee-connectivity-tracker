using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared.ValidationAttributes
{
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
