using Newtonsoft.Json;

namespace quebrix_ui.DTOs;

public class AuthenticateRequest
{
    [JsonProperty("username")]
    public string UserName { get; set; }
    [JsonProperty("password")]
    public string password { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; }
}



public class ApiResponse<T>
{
    [JsonProperty("is_success")]
    public bool IsSuccess { get; set; }

    [JsonProperty("data")]
    public T Data { get; set; }
}