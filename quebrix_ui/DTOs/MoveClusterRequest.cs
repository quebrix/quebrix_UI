using Newtonsoft.Json;

namespace quebrix_ui.DTOs;

public class MoveClusterRequest
{
    [JsonProperty("src_cluster")]
    public string SrcCluster { get; set; }

    [JsonProperty("desc_cluster")]
    public string DestCluster { get; set; }

}