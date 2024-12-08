using System.Text.Json;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using quebrix_ui.DTOs;
using quebrix_ui.Helpers;
using quebrix_ui.Services;

namespace quebrix_ui.Components.Pages;

public partial class Keys : ComponentBase
{
    [Inject] private IStorageManagment _StorageManagment { get; set; }
    [Inject] private ToastService ToastService { get; set; } = default!;
    private long TimeTTl { get; set; }
    private bool KeysAreZero = false;
    List<ToastMessage> messages = new List<ToastMessage>();
    private Modal modal = default!;
    private string SelectedUnit = "Seconds";
    private SetKeyValueDTO KeyValueSetter { get; set; } = new();
    private bool Hasttl { get; set; }
    [Parameter] public string ClusterName { get; set; }
    private ApiClient _client { get; set; } = new("http://localhost:6022");
    private List<string> Keyss { get; set; } = new List<string>();
    private string FilterText = string.Empty;
    private string SelectedKey = string.Empty;
    private string LastRefreshedTime { get; set; }
    private Timer _timer;
    private string EditableJson = string.Empty;
    protected bool IsRefreshing { get; set; } = false;

    private void SwitchHasttlChanged(bool value) => Hasttl = value;


    private async Task OnShowModalClick()
    {
        await modal.ShowAsync();
    }

    private async Task OnHideModalClick()
    {
        await modal.HideAsync();
    }

    private IEnumerable<string> FilteredKeys =>
        Keyss.Where(key => key.Contains(FilterText, StringComparison.OrdinalIgnoreCase));

    private bool IsLoading { get; set; }

    private async Task RefreshKeysAsync()
    {
        IsLoading = true;
        var auth = await _StorageManagment.GetAsync<string>("auth");
        Keyss = await _client.GetKeysOfCluster(ClusterName, auth);
        if (Keyss.Count == 0)
            KeysAreZero = true;
        else
            KeysAreZero = false;
                
        IsLoading = false;
        StateHasChanged();
    }

    protected async override Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(ClusterName))
        {
            var auth = await _StorageManagment.GetAsync<string>("auth");
            Keyss = await _client.GetKeysOfCluster(ClusterName, auth);
        }
        else
        {
            Keyss = new List<string>();
        }
        StartRefresh();
    }

    private async void RefreshKeys(object? state)
    {
        await InvokeAsync(() =>
        {
            LastRefreshedTime = DateTime.Now.ToString("hh:mm:ss tt"); 
            RefreshKeysAsync().ConfigureAwait(false);
        });
    }

    private void StartRefresh()
    {
        if (!IsRefreshing)
        {
            IsRefreshing = true;
            _timer = new Timer(RefreshKeys, null, 0, 5000); 
        }
    }


    private async Task HandleInsertkey()
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        if (string.IsNullOrEmpty(KeyValueSetter.Key))
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, IconName.X, "Create key value",
                "key must have value"));
        }
        else if (string.IsNullOrEmpty(KeyValueSetter.Value))
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, IconName.X, "Create key value",
                "value must have value"));
        }
        else
        {
            if (Hasttl)
            {
                switch (SelectedUnit)
                {
                    case "Seconds":
                        KeyValueSetter.ttl = TimeTTl * 1000;
                        break;
                    case "Minutes":
                        KeyValueSetter.ttl = TimeTTl * 60 * 1000;
                        break;
                    case "Hours":
                        KeyValueSetter.ttl = TimeTTl * 60 * 60 * 1000;
                        break;
                }
            }
           

            var insertResponse = await _client.Set(ClusterName, KeyValueSetter.Key, KeyValueSetter.Value, auth,
                Hasttl?KeyValueSetter.ttl:null);
            if (insertResponse)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, IconName.Check2, "Create key value",
                    "succeed"));
                await RefreshKeysAsync();
                await OnHideModalClick();
                KeyValueSetter = new SetKeyValueDTO();
            }
            else
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, IconName.X, "Create key value",
                    "some thing went wrong check logs"));
            }
        }
    }

    private async Task LoadContent(string key)
    {
        SelectedKey = key;
        var auth = await _StorageManagment.GetAsync<string>("auth");
        var value = await _client.Get(ClusterName, SelectedKey, auth);
        EditableJson = value;
    }


    private async Task DeleteKey()
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        var delRes = await _client.Delete(ClusterName, SelectedKey, auth);
        if (delRes)
        {
            ToastService.Notify(new ToastMessage(ToastType.Success, IconName.Check2, "Delete key",
                "succeed!"));
            await RefreshKeysAsync();
            SelectedKey = string.Empty;
            StateHasChanged();
        }
        else
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, IconName.X, "Delete key",
                "some thing went wrong check logs"));
        }
           

    }
    private async Task SaveContent()
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        var insertRes = await _client.Set(ClusterName,SelectedKey,EditableJson,auth);
        if (insertRes)
        {
            ToastService.Notify(new ToastMessage(ToastType.Success, IconName.Check2, "Create key value",
                "succeed"));
            await RefreshKeysAsync();
        }
        else
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, IconName.X, "Create key value",
                "some thing went wrong check logs"));
        }
    }


    private void FormatJson()
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(EditableJson);
            EditableJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions {WriteIndented = true});
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
        json = json.Replace("\"", "<span class='json-string'>\"</span>");
        json = json.Replace(": ", "<span class='json-key'>: </span>");
        json = json.Replace(",", "<span class='json-value'>, </span>");
        return json;
    }
}