using Newtonsoft.Json;

namespace quebrix_ui.DTOs;

public class SetRequest
{
    [JsonProperty("cluster")]
    public string cluster { get; set; }

    [JsonProperty("key")]
    public string key { get; set; }

    [JsonProperty("value")]
    public string value { get; set; }

    [JsonProperty("ttl")]
    public long? ttl { get; set; }
}