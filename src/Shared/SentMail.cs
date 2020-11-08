using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EctBlazorApp.Shared
{
    public class SentMail : Mail
    {
        public DateTime SentAt { get; set; }

        private List<string> _recipients;

        public List<string> Recipients
        {
            get { return _recipients; }
            set { _recipients = value; }
        }

        public string RecipientsAsString 
        { 
            get
            {
                return string.Join("|", _recipients);
            }
            set
            {
                _recipients = value.Split("|").ToList();
            }
        }
    }
}
