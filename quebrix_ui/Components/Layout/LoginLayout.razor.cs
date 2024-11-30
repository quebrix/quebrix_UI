using System.Text;
using BlazorBootstrap;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using quebrix_ui.DTOs;
using quebrix_ui.Services;
using quebrix_ui.Helpers;
using Toasts = quebrix_ui.Helpers.Toasts;

namespace quebrix_ui.Components.Layout;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

public partial class LoginLayout : LayoutComponentBase
{
    [Inject] private IStorageManagment? _storageManagment { get; set; }
    [Inject] private NavigationManager _navigator { get; set; }
    [SupplyParameterFromForm] private LoginDTO LoginModel { get; set; } = new LoginDTO();
    
    private ApiClient _client = new("http://127.0.0.1:6022");
    private string passwordInputType = "password";
    private string passwordIconClass = "fas fa-eye";

    List<ToastMessage> messages = new List<ToastMessage>();
    private void TogglePasswordVisibility()
    {
        if (passwordInputType == "password")
        {
            passwordInputType = "text";
            passwordIconClass = "fas fa-eye-slash";
        }
        else
        {
            passwordInputType = "password";
            passwordIconClass = "fas fa-eye";
        }
    }

    private async Task HandleLogin()
    {
        var auth = await _client.Authenticate(LoginModel.UserName, LoginModel.Password);
        if (auth)
        {
            Toasts.ShowMessage(messages,ToastType.Success, "Authentication","Login successful");
            await _storageManagment.SetAsync("auth",_client.MakeAuth(LoginModel.UserName,LoginModel.Password));
            _navigator.NavigateTo("/Quebrix");
        }
        else
            Toasts.ShowMessage(messages,ToastType.Danger, "Authentication","Login failed");
    }
}

public class LoginDTO
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}