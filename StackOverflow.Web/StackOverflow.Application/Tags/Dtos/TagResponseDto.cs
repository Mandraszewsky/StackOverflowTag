namespace StackOverflow.Application.Tags.Dtos;

public class TagResponseDto
{
    public string Name { get; set; } = string.Empty;
    public long Count { get; set; }
    public double Percentage { get; set; }
}
