using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EctWebApp.Models
{
    public abstract class Mail
    {
        public Importance? Importance { get; private set; }                                         // Nullable
        public string Subject { get; private set; }

        protected Mail(Importance? mailImportance, string subject)
        {
            Importance = mailImportance;
            Subject = subject;
        }

        protected static async Task<IList<Message>> GetGraphMessagesFromMessagePagesParm(IMailFolderMessagesCollectionPage messagePages, GraphServiceClient graphClient)
        {
            IList<Message> graphMessages;
            if (messagePages.NextPageRequest != null)                                               // Handle case where there are more than 50
            {
                graphMessages = new List<Message>();
                PageIterator<Message> pageIterator = PageIterator<Message>.CreatePageIterator(      // Create a page iterator to iterate over subsequent pages
                    graphClient, messagePages,                                                      // of results. Build a list from the results
                    (inbox) =>
                    {
                        graphMessages.Add(inbox);
                        return true;
                    }
                );
                await pageIterator.IterateAsync();
            }
            else
                graphMessages = messagePages.CurrentPage;                                             // If only one page - use the result

            return graphMessages;
        }
    }
}