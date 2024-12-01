using System.Text.Json;
using Microsoft.AspNetCore.Components;
using quebrix_ui.Helpers;
using quebrix_ui.Services;

namespace quebrix_ui.Components.Pages;

public partial class Keys : ComponentBase
{
    [Inject] private IStorageManagment _StorageManagment { get; set; }
    [Parameter] public string ClusterName { get; set; }
    private ApiClient _client { get; set; } = new("http://localhost:6022");
    private List<string> Keyss { get; set; } = new List<string>();
    private string FilterText = string.Empty;
    private string SelectedKey = string.Empty;
    private string EditableJson = string.Empty;

    private IEnumerable<string> FilteredKeys =>
        Keyss.Where(key => key.Contains(FilterText, StringComparison.OrdinalIgnoreCase));

   

    protected async override Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(ClusterName))
        {
            var auth = await _StorageManagment.GetAsync<string>("auth");
            Keyss = await _client.GetKeysOfCluster(ClusterName, auth);
        }
        else
        {
            Keyss = new List<string> ();
        }
    }

    private async Task LoadContent(string key)
    {
        SelectedKey = key;
        var auth = await _StorageManagment.GetAsync<string>("auth");
        var value = await _client.Get(ClusterName, SelectedKey,auth);
        EditableJson = value;
    }

    private void SaveContent()
    {
        // Logic to save changes made in the EditableJson field
        Console.WriteLine($"Saving content for key '{SelectedKey}': {EditableJson}");
    }
    
    
    private void FormatJson()
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(EditableJson);
            EditableJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            EditableJson = "Invalid JSON!";
        }
    }

    private RenderFragment RenderJsonContent(string json) => builder =>
    {
        builder.AddMarkupContent(0, "<pre class='json-content'>");
        builder.AddMarkupContent(1, FormatJsonWithColors(json));
        builder.AddMarkupContent(2, "</pre>");
    };

    private string FormatJsonWithColors(string json)
    {
        // Simple formatting for keys and values
        json = json.Replace("\"", "<span class='json-string'>\"</span>"); // Highlight all strings in JSON
        json = json.Replace(": ", "<span class='json-key'>: </span>"); // Color the colon separator
        json = json.Replace(",", "<span class='json-value'>, </span>"); // Color the comma separators

        // Add more detailed handling for keys/values if necessary
        return json;
    }
}