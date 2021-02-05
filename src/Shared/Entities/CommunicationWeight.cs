using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EctBlazorApp.Shared.Entities
{
    public class CommunicationWeight
    {
        public int Id { get; set; }

        [Required]
        public string Medium { get; set; }

        [Required]
        public int Weight { get; set; }
    }
}
