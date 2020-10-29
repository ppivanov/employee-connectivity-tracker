// Source: https://github.com/microsoftgraph/msgraph-training-aspnet-core/blob/master/demo/GraphTutorial/

namespace EctWebApp.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
