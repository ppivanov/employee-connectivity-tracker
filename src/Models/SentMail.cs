using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EctWebApp.CommonMethods.CommonStaticMethods;

namespace EctWebApp.Models
{
    public class SentMail : Mail
    {
        public List<string> Recipients { get; private set; }
        public DateTimeOffset? SentDateTime { get; private set; }

        public SentMail(Message outlookMail)
            : base(outlookMail.Importance, outlookMail.Subject)
        {
            Recipients = new List<string>();
            foreach (Recipient recipient in outlookMail.ToRecipients)
            {
                Recipients.Add(FormatNameAndEmailAddressParms(recipient.EmailAddress.Name,
                                                              recipient.EmailAddress.Address));
            }
            SentDateTime = outlookMail.SentDateTime;
        }

        public static async Task<List<SentMail>> GetSentMailForQueryStringParmAsync(string queryString,
                                                                                    GraphServiceClient graphClient)
        {
            var graphMessages = await GetSentGraphMessagesForQueryStringParm(queryString, graphClient);

            List<SentMail> sentMail = new List<SentMail>();
            if (graphMessages != null)
            {
                foreach (Message message in graphMessages)
                {
                    sentMail.Add(new SentMail(message));
                }
            }
            return sentMail;
        }

        private static async Task<IList<Message>> GetSentGraphMessagesForQueryStringParm(string queryString, 
                                                                                         GraphServiceClient graphClient)
        {
            IMailFolderMessagesCollectionPage messagePages = 
                await GetSentMailMessageCollectionPagesForQueryStringParm(queryString, graphClient);

            IList<Message> graphMessages = await GetGraphMessagesFromMessagePagesParm(messagePages, graphClient);
            
            return graphMessages;
        }

        private static async Task<IMailFolderMessagesCollectionPage>
            GetSentMailMessageCollectionPagesForQueryStringParm(string queryString,
                                                                GraphServiceClient graphClient)
        {
            IMailFolderMessagesCollectionPage messagePages =
                await graphClient.Me
                .MailFolders["SentItems"]
                .Messages
                .Request()
                .Filter(queryString)
                .Select(m => new
                {
                    m.Importance,
                    m.ToRecipients,
                    m.SentDateTime,
                    m.Subject
                })
                .Top(50)
                //.OrderBy("sentDateTime DESC")
                .GetAsync();

            return messagePages;
        }
    }
}
