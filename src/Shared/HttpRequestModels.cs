using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class GraphUserRequestDetails
    {
        public string GraphToken { get; set; }
    }

    public class EctTeamRequestDetails
    {
        [Required (ErrorMessage = "Please, enter a team name.")]
        [StringLength(50, ErrorMessage = "Team name is too long. Please, use less than 50 characters.")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Please, select a team leader.")]
        public string LeaderEmail { get; set; }
        [CollectionNotEmpty(ErrorMessage = "You must select at least one member.")]
        public List<string> MemberEmails { get; set; }

        public bool AreDetailsValid() 
        {
            if (string.IsNullOrEmpty(Name) || Name.Length > 50)
                return false;

            if (!LeaderEmail.IsValidEmail())
                return false;

            if (MemberEmails == null || MemberEmails.Count < 1)
                return false;

            foreach (var memberEmail in MemberEmails)
            {
                if (!memberEmail.IsValidEmail())
                    return false;
            }
            return true;
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

    public class NoStringInListBiggerThanAttribute : ValidationAttribute
    {
         readonly string message;
        private string CustomErrorMessage
        {
            get
            {
                return message != null ? message : "At least one selection is required.";
            }
        }

        public NoStringInListBiggerThanAttribute() { }

        public NoStringInListBiggerThanAttribute(string message)
        {
            this.message = message;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strings = value as IEnumerable<string>;
            if (strings == null || strings.Any() == false)
                return new ValidationResult(CustomErrorMessage);

            return ValidationResult.Success;
        }
    }
}
