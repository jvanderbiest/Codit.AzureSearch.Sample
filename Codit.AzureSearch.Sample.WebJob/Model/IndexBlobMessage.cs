using System.Configuration;

namespace Codit.AzureSearch.Sample.WebJob.Model
{
    public class IndexBlobMessage
    {
        public string Name
        {
            get { return ConfigurationManager.AppSettings.Get("IndexName").ToLowerInvariant(); }
        }

        public string Url
        {
            get { return ConfigurationManager.AppSettings.Get("SearchUrl").TrimEnd('/'); }
        }

        public string AdminApiKey
        {
            get { return ConfigurationManager.AppSettings.Get("AdminApiKey").TrimEnd('/'); }

        }

        public string ApiVersion { get { return ConfigurationManager.AppSettings.Get("ApiVersion"); } }

        public object Content { get; set; }
    }
}