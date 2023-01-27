namespace Sending.Queue.Example.Dto;

public record JobsAddResponseDto
{
    public IEnumerable<long> JobIds { get; init; }
}
