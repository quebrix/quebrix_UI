using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using quebrix_ui.Helpers;
using quebrix_ui.Services;
using Toasts = quebrix_ui.Helpers.Toasts;

namespace quebrix_ui.Components.Pages;

public partial class Copy : ComponentBase
{
    [Inject] private IStorageManagment _StorageManagment { get; set; }
    [Inject] private ToastService ToastService { get; set; }
    [Inject] private NavigationManager _navigator { get; set; }
    List<ToastMessage> messages = new List<ToastMessage>();
    private string SRCCluster { get; set; }
    private string DESTCluster { get; set; }
    private List<string> _clusters { get; set; } = new();
    private bool IsLoading { get; set; } = false;
    private ApiClient Client { get; set; } = new("http://localhost:6022");
    private CopyModel Model { get; set; } = new();

    protected async override Task OnInitializedAsync()
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        _clusters = await Client.GetAllClusters(auth);
    }


    private async Task CopyClusterAsync()
    {
        IsLoading = true;
        var auth = await _StorageManagment.GetAsync<string>("auth");
        if (string.IsNullOrEmpty(Model.SRC))
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, $"Source cluster must has value"));
            IsLoading = false;
        }
        else if (string.IsNullOrEmpty(Model.DESC))
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, $"Destination cluster must has value"));
            IsLoading = false;
        }
        else if (Model.SRC == Model.DESC)
        {
            ToastService.Notify(new ToastMessage(ToastType.Warning, $" Source and Destination cluster are same "));
            IsLoading = false;
        }
        else
        {
            var result = await Client.CopyCluster(Model.SRC, Model.DESC, auth);
            if (result)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"copy keys successfully completed"));
                IsLoading = false;
            }
            else
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"some thing went wrong!"));
                IsLoading = false;
            }
        }
    }
}

public class CopyModel
{
    public string SRC { get; set; }
    public string DESC { get; set; }
}