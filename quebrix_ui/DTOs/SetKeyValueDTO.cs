namespace quebrix_ui.DTOs;

public class SetKeyValueDTO
{
    public string Key { get; set; }
    public string Value { get; set; }
    public TimeOnly ttl { get; set; }
}