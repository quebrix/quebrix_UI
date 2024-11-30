using BlazorBootstrap;

namespace quebrix_ui.Helpers;

public class Toasts
{
    internal static ToastMessage CreateWarningToast(string title, string message) =>
        new ToastMessage()
        {
            Title = title,
            Message = $"{message} \n Time: {DateTime.Now}",
            AutoHide = true,
            HelpText = $"{DateTime.Now}",
            Type = ToastType.Warning
        };

    internal static ToastMessage CreateSuccessToast(string title, string message) =>
        new ToastMessage()
        {
            Title = title,
            Message = $"{message} \n Time: {DateTime.Now}",
            AutoHide = true,
            HelpText = $"{DateTime.Now}",
            Type = ToastType.Success
        };

    internal static ToastMessage CreateErrorToast(string title, string message) =>
        new ToastMessage()
        {
            Title = title,
            Message = $"{message} \n Time: {DateTime.Now}",
            AutoHide = true,
            HelpText = $"{DateTime.Now}",
            Type = ToastType.Danger
        };

    public static void ShowMessage(List<ToastMessage> messages, ToastType toastType, string title, string message)
    {
        switch (toastType)
        {
            case ToastType.Success:
                messages.Add(CreateSuccessToast(title, message));
                break;
            case ToastType.Warning:
                messages.Add(CreateWarningToast(title, message));
                break;
            case ToastType.Danger:
                messages.Add(CreateErrorToast(title, message));
                break;
        }
    }
}