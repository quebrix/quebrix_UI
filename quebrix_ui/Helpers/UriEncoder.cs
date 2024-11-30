using System.Web;

namespace quebrix_ui.Helpers;

public static class UriEncoder
{
    public static string EncodeUrl(this string text) => HttpUtility.UrlEncode(text);

    public static string DecodeUri(this string text) => HttpUtility.UrlDecode(text);
}