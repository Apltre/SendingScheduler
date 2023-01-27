using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Sending.Queue.Example.Dto;

public record JobDto
{
    public int Type { get; init; }
    public DateTime StartTime { get; init; }
    public JsonElement? Metadata { get; init; }
    [Required]
    public JsonElement Data { get; init; }
    public int ServiceId { get; init; }
    public int HandleOrder { get; init; }
}
