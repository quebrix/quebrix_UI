using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using quebrix_ui.DTOs;
using quebrix_ui.Helpers;
using quebrix_ui.Services;
using Toasts = quebrix_ui.Helpers.Toasts;

namespace quebrix_ui.Components.Pages;

public partial class Clusters : ComponentBase
{
    [Inject] private IStorageManagment _StorageManagment { get; set; }
    [Inject] private NavigationManager _navigator { get; set; }
    List<ToastMessage> messages = new List<ToastMessage>();
    private ConfirmDialog dialog = default!;
    private ClusterDTO ClusterDto { get; set; } = new();
    [Inject] ToastService ToastService { get; set; } = default!;
    private ApiClient Client { get; set; } = new("http://localhost:6022");
    private List<string> _clusters { get; set; } = new();
    private Modal modal = default!;

    private async Task OnShowModalClick()
    {
        await modal.ShowAsync();
    }

    private async Task OnHideModalClick()
    {
        await modal.HideAsync();
    }

    protected async override Task OnInitializedAsync()
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        _clusters = await Client.GetAllClusters(auth);
        if (_clusters.Any())
        {
            if (_clusters.First() == "No clusters found on this port")
                Toasts.ShowMessage(messages, ToastType.Info, "Clusters", "No clusters found on this port");
            else
                Toasts.ShowMessage(messages, ToastType.Success, "Clusters", "Get successful");
        }
        else
            Toasts.ShowMessage(messages, ToastType.Warning, "Clusters", "There is no cluster set on quebrix");
    }


    private async Task HandleSetCluster()
    {
        if(string.IsNullOrEmpty(ClusterDto.ClusterName))
            ToastService.Notify(new ToastMessage(ToastType.Danger, $"Cluster name can not ne empty."));
        else
        {
            var auth = await _StorageManagment.GetAsync<string>("auth");
            var clusterSetReponse = await Client.SetCluster(ClusterDto.ClusterName, auth);
            if (clusterSetReponse)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Cluster set successful."));
                _clusters = await Client.GetAllClusters(auth);
                StateHasChanged();
                await modal.HideAsync();
            }
            else
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Something went wrong."));
        }
    }

    private async Task ClearClusterAsync(string clusterName)
    {
        var parameters = new Dictionary<string, string>();
        parameters.Add("Cluster", clusterName);

        var confirmation = await dialog.ShowAsync("Are you sure you want to delete this Cluster?",
            "this change is never returnable or recover!", new ConfirmDialogOptions
            {
                NoButtonColor = ButtonColor.Danger,
                YesButtonColor = ButtonColor.Success,
            });
        if (confirmation)
        {
            var auth = await _StorageManagment.GetAsync<string>("auth");
            var clearClusterResponse = await Client.ClearCluster(clusterName, auth);
            if (clearClusterResponse)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Cluster deleted successfully."));
                _clusters = await Client.GetAllClusters(auth);
                StateHasChanged();
            }
            else
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Something went wrong."));
        }
        else
            ToastService.Notify(new ToastMessage(ToastType.Warning, $"Delete cluster action canceled."));
    }

    private async Task RefreshClusters()
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        _clusters = await Client.GetAllClusters(auth);
        StateHasChanged();
        ToastService.Notify(new ToastMessage(ToastType.Success, $"Clusters refreshed successfully."));
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        var auth = await _StorageManagment.GetAsync<string>("auth");
        _clusters = await Client.GetAllClusters(auth);
    }

    private async Task NavigateToKeys(string cluster)
    {
        _navigator.NavigateTo($"/Keys/{cluster}");
    }
    
    
}