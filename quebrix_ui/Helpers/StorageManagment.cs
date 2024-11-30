using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;

namespace quebrix_ui.Helpers;


public interface IStorageManagment
{
    Task SetAsync<T>(string key, T value);
    Task<T> GetAsync<T>(string key);
    Task SetAsync(string key, object value);
    Task<string> GetAsync(string key);
}
public class StorageManagment(ProtectedSessionStorage storage):IStorageManagment
{
    public async Task SetAsync<T>(string key, T value)
    { 
        await storage.SetAsync(key, JsonConvert.SerializeObject(value));
    }

    public async Task<T> GetAsync<T>(string key)
    {
        try
        {
            var result = await storage.GetAsync<T>(key)
                .ConfigureAwait(false);
            if(result.Success)
                return JsonConvert.DeserializeObject<T>(result.Value.ToString());
            else
                return default;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
      
    }

    public async Task SetAsync(string key, object value)
    {
        await storage.SetAsync(key, value);
    }

    public async Task<string> GetAsync(string key)
    {
        var result = await storage.GetAsync<object>(key)
            .ConfigureAwait(false);
        if(result.Success)
            return result.Value.ToString();
        else
            return default;
    }
}