using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EctWebApp.CommonMethods.CommonStaticMethods;

namespace EctWebApp.Models
{
    public class ReceivedMail : Mail
    {
        public string From { get; private set; }
        public bool? IsRead { get; private set; }                                                   // Nullable
        public DateTimeOffset? ReceivedDateTime { get; private set; }                               // Nullable

        public ReceivedMail(Message outlookMail)
            : base(outlookMail.Importance, outlookMail.Subject)
        {
            From = FormatNameAndEmailAddressParms(outlookMail.From.EmailAddress.Name,
                                                  outlookMail.From.EmailAddress.Address);
            IsRead = outlookMail.IsRead;
            ReceivedDateTime = outlookMail.ReceivedDateTime;
        }

        public static async Task<List<ReceivedMail>> GetReceivedMailForQueryStringParmAsync(string queryString,
                                                                                            GraphServiceClient graphClient)
        {
            var graphMessages = await GetReceivedGraphMessagesForQueryStringParm(queryString, graphClient);
            List<ReceivedMail> receivedMail = new List<ReceivedMail>();
            if (graphMessages != null)
            {
                foreach (Message message in graphMessages)
                {
                    receivedMail.Add(new ReceivedMail(message));
                }
            }
            return receivedMail;
        }

        private static async Task<IList<Message>> GetReceivedGraphMessagesForQueryStringParm(string queryString, 
                                                                                             GraphServiceClient graphClient)
        {
            IMailFolderMessagesCollectionPage messagePages = await GetReceivedMailMessageCollectionPagesForQueryStringParm(queryString, graphClient);
            IList<Message> graphMessages = await GetGraphMessagesFromMessagePagesParm(messagePages, graphClient);

            return graphMessages;
        }

        private static async Task<IMailFolderMessagesCollectionPage> 
            GetReceivedMailMessageCollectionPagesForQueryStringParm(string queryString,
                                                                    GraphServiceClient graphClient)
        {
            IMailFolderMessagesCollectionPage messagePages =
                await graphClient.Me
                .MailFolders["Inbox"]                                                               // String can be refactored to be passed in as a parm
                .Messages
                .Request()
                .Filter(queryString)
                .Select(m => new
                {
                    m.From,
                    m.IsRead,
                    m.Importance,
                    m.ReceivedDateTime,
                    m.Subject
                })
                .Top(50)
                //.OrderBy("receivedDateTime DESC")
                .GetAsync();

            return messagePages;
        }

        
    }
}
