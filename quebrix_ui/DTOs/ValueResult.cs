using Newtonsoft.Json;

namespace quebrix_ui.DTOs;

public class ValueResult
{
    [JsonProperty("value")]
    public byte[] Value { get; set; }
    [JsonProperty("value_type")]
    public string ValueType { get; set; }
}