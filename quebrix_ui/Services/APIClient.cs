using System.Text;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using quebrix_ui.DTOs;
using quebrix_ui.Helpers;
using RestSharp;

namespace quebrix_ui.Services;

public class ApiClient
{
    private readonly string _baseUrl;
    private readonly RestClient _client;
    public ApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _client = new RestClient(baseUrl);
    }


    public async Task<bool> Authenticate(string userName, string password)
    {
        var url = "/api/login";
        var request = new RestRequest(url, Method.Post);
        var setRequest = new AuthenticateRequest
        {
            password = password,
            UserName = userName,
            Role = string.Empty
        };
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(JsonConvert.SerializeObject(setRequest));
        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<string>>(response.Content);
            if (result.IsSuccess)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    
    public async Task<List<string>> GetAllClusters(string credentials)
    {
        var url = $"/api/get_clusters";
        var request = new RestRequest(url, Method.Get);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<string>>>(response.Content);
                return apiResponse.Data;
            }
            catch
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(response.Content);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {
                    return new List<string>() { apiResponse.Data };
                }
                else
                {
                    return new List<string>() { "no cluster found" };
                }
            }
        }
        else
        {
            throw new Exception($"Error getting clusters: {response.ErrorMessage}");
        }
    }
    
    public async Task<bool> ClearCluster(string cluster,string credentials)
    {
        var url = $"/api/clear_cluster/{cluster.EncodeUrl()}";
        var request = new RestRequest(url, Method.Delete);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
            return true;
        else
            return false;
    }
    
    
    public async Task<bool> SetCluster(string cluster, string credentials)
    {
        var url = $"/api/set_cluster/{cluster.EncodeUrl()}";
        var request = new RestRequest(url, Method.Post);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
            return true;
        else
            return false;
    }
    
    //helpers
    internal string MakeAuth(string username, string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
}