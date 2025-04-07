using System.CommandLine;

namespace Optimus.Cli.Commands;

public class RootCliCommand : ICliCommand
{
    public static RootCommand BuildCommand()
    {
        // Define command line options
        var inputOption = new Option<string>(["--input", "-i"], () => "./", "Input directory or image");
        var outputOption = new Option<string>(["--output", "-o"], () => "./optimus-images", "Output directory");
        var qualityOption =
            new Option<int>(["--quality", "-q"], () => 75, "Quality of Compression (0-100). Default: 75");
        var overwriteOption = new Option<bool>(["--overwrite", "-ow"], () => true,
            "Overwrite images if they exist. Default: true");
        var formatOption = new Option<bool>(["--jpeg", "-j"], () => false, "Save images as jpeg. Default: false");
        var widthOption = new Option<int?>(["--width", "-w"], () => null, "Set Width of image. Default: none");
        var heightOption = new Option<int?>(["--height", "-h"], () => null, "Set Height of image. Default: none");
        var percentOption = new Option<int?>(["--percent", "-p"], () => null,
            "Set resize percentage of image. Default: none");

        // Build root command
        var rootCommand = new RootCommand("Image Optimization Tool");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(qualityOption);
        rootCommand.AddOption(overwriteOption);
        rootCommand.AddOption(formatOption);
        rootCommand.AddOption(widthOption);
        rootCommand.AddOption(heightOption);
        rootCommand.AddOption(percentOption);

        // Set command handler
        rootCommand.SetHandler(Handle,
            inputOption, outputOption, qualityOption, overwriteOption, formatOption,
            widthOption, heightOption, percentOption);

        return rootCommand;
    }

    private static async Task Handle(
        string input,
        string output,
        int quality,
        bool overwrite,
        bool format,
        int? width,
        int? height,
        int? percent)
    {
        IReadOnlyList<string> supportedTypes = [".png", ".jpg", ".jpeg"];

        if ((width != null || height != null) && (percent != null))
        {
            Console.WriteLine("Width/Height options are mutually exclusive with Percentage.");
            return;
        }

        var inputPath = DirHelper.ProcessRootPath(Directory.GetCurrentDirectory(), input);
        var outputPath = DirHelper.ProcessRootPath(Directory.GetCurrentDirectory(), output);

        Console.WriteLine($"Input directory: {inputPath}");
        Console.WriteLine($"Output directory: {outputPath}");

        var imageFiles = DirHelper.GetSupportedImages(supportedTypes, inputPath);

        if (imageFiles.Count == 0)
        {
            Console.WriteLine($"No supported images found. Exiting...");
            return;
        }

        DirHelper.CreateDirectoryIfNotExists(inputPath, outputPath);

        Console.WriteLine($"Optimizing Images...");
        Console.WriteLine($"Quality: {quality}");

        await ImageProcessor.ProcessImages(
            imageFiles,
            overwrite,
            outputPath,
            quality,
            format,
            new ResizeImageOptions(width, height, percent));

        Console.WriteLine($"Optimizations completed.");
    }
}