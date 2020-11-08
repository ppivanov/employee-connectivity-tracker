using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared
{
    public abstract class Mail
    {
        public int Id { get; set; }
        public string Subject { get; set; }
    }
}
