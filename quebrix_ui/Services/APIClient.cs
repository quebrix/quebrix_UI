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
    
    
    public async Task<List<string>> GetKeysOfCluster(string clusterName, string credentials)
    {
        var url = $"/api/get_keys/{clusterName.EncodeUrl()}";
        var request = new RestRequest(url, Method.Get);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<string>>>(response.Content);
            return apiResponse.Data;
        }
        else
        {
            throw new Exception($"Error getting keys of  cluster: {response.ErrorMessage}");
        }
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


    public async Task<string> Get(string cluster, string key, string credentials)
    {
        try
        {
            var url = $"/api/get/{cluster.EncodeUrl()}/{key.EncodeUrl()}";
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("Authorization", $"{credentials}");
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ValueResult>>(response.Content);
                if (apiResponse.IsSuccess)
                {
                    var result = string.Empty;
                    if (apiResponse.Data.ValueType == "Int")
                        result = BitConverter.ToInt32(apiResponse.Data.Value).ToString();
                    else
                    {
                        result = Encoding.UTF8.GetString(apiResponse.Data.Value);
                    }

                    return result;
                }
                else
                {
                    return string.Empty;
                }
            }

            return response.ErrorMessage;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting value: {ex.Message}");
        }
    }
    
    
    public async Task<bool> Set(string cluster, string key, string value, string credentials, long? expireTime = null)
    {
        var url = "/api/set";
        var request = new RestRequest(url, Method.Post);
        var setRequest = new SetRequest
        {
            cluster = cluster,
            key = key,
            value = value,
            ttl = expireTime
        };
        var jsonBody = JsonConvert.SerializeObject(setRequest);
        request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
        request.AddHeader("Authorization", $"{credentials}");

        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<string>>(response.Content);
            if (result.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    
    
    public async Task<bool> Delete(string cluster, string key, string credentials)
    {
        var url = $"/api/delete/{cluster.EncodeUrl()}/{key.EncodeUrl()}";
        var request = new RestRequest(url, Method.Delete);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //helpers
    internal string MakeAuth(string username, string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
}