using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace quebrix_ui.DTOs;

public class ClusterDTO
{
    [Required]
    public string ClusterName { get; set; }
}