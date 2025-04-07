using Cocona;

namespace Optimus.Cli;

public record CommonParameters : ICommandParameterSet
{
    [Option('q', Description = "Quality of Compression (0-100)")]
    [HasDefaultValue]
    public int Quality { get; set; } = 75;

    [Option("ow", Description = "Overwrite images if they exist")]
    [HasDefaultValue]
    public bool Overwrite { get; set; } = true;

    [Option('j', Description = "Save images as jpeg")]
    [HasDefaultValue]
    public bool Jpeg { get; set; } = false;

    [Option('w', Description = "Set Width of image")]
    [HasDefaultValue]
    public int? Width { get; set; } = null;

    [Option('h', Description = "Set Height of image")]
    [HasDefaultValue]
    public int? Height { get; set; } = null;

    [Option('p', Description = "Set resize percentage of image")]
    [HasDefaultValue]
    public int? Percent { get; set; } = null;
}