using System.Threading.Tasks;

namespace EctBlazorApp.Client.Graph
{
    public interface IControllerConnection
    {
        Task<string> GetCalendarEventsForEmail(string userEmail);

        Task<string> UpdateDatabaseRecords(string userEmail);
    }
}
