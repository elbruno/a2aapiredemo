using StoreRealtime.Services;
using System.ComponentModel;
using System.Text.Json;

namespace StoreRealtime.ContextManagers;

public class DataSourcesUrlContext(DataSourcesService dataSourcesService)
{
    [Description("Search the indexes pages indexes pages database based on a search criteria provided by the user using natural language.")]
    public async Task<string> SemanticSearchDataSourcesAsync(string searchCriteria)
    {
        var response = await dataSourcesService.SearchAsync(searchCriteria);
        return JsonSerializer.Serialize(response);
    }
}