using Cocona;
using Optimus.Cli;

IReadOnlyList<string> supportedTypes = [".png", ".jpg", ".jpeg"];

var builder = CoconaLiteApp.CreateBuilder();
var app = builder.Build();

// Root - Single Image
app.AddCommand(async (CommonParameters commonParameters,
    [Option('i', Description = "Path of image to process")]
    string input,
    [Option('o', Description = "Output path for processed image. Default is <workingDir>/<filename>")]
    string? output = null) =>
{
    if (!ValidateParameters(commonParameters)) return Task.CompletedTask;

    var inputPath = DirHelper.ProcessRootPath(Directory.GetCurrentDirectory(), input);
    if (!supportedTypes.Contains(Path.GetExtension(inputPath)))
    {
        Console.WriteLine($"Provided Image type not supported");
        return Task.CompletedTask;
    }

    var outputPath = DirHelper.ProcessRootPath(
        Directory.GetCurrentDirectory(),
        output ??
        $"./{Path.GetFileNameWithoutExtension(inputPath)}-optimized{Path.GetExtension(inputPath).ToLowerInvariant()}");

    PrintPaths(inputPath, outputPath);

    Console.WriteLine($"Optimizing Image...");
    Console.WriteLine($"Quality: {commonParameters.Quality}");
    await ImageProcessor.ProcessImages(
        new FileInfo(inputPath),
        commonParameters.Overwrite,
        outputPath,
        commonParameters.Quality,
        commonParameters.Jpeg,
        new ResizeImageOptions(commonParameters.Width, commonParameters.Height, commonParameters.Percent));
    Console.WriteLine("Optimization completed.");

    return Task.CompletedTask;
});

// Directory
app.AddCommand("dir", async (CommonParameters commonParameters,
    [Option('i', Description = "Path of directory to process")]
    string input,
    [Option('o', Description = "Output path for processed images. Default is <workingDir>/optimus-output")]
    string? output = null) =>
{
    if (!ValidateParameters(commonParameters)) return Task.CompletedTask;

    var inputPath = DirHelper.ProcessRootPath(Directory.GetCurrentDirectory(), input);
    var outputPath =
        DirHelper.ProcessRootPath(Directory.GetCurrentDirectory(), output ?? inputPath + "/optimus-output");

    PrintPaths(inputPath, outputPath);

    var imageFiles = DirHelper.GetSupportedImages(supportedTypes, inputPath);

    if (imageFiles.Count == 0)
    {
        Console.WriteLine($"No supported images found. Exiting...");
        return Task.CompletedTask;
    }

    DirHelper.CreateDirectoryIfNotExists(inputPath, outputPath);

    Console.WriteLine($"Optimizing Images...");
    Console.WriteLine($"Quality: {commonParameters.Quality}");

    await ImageProcessor.ProcessImages(
        imageFiles,
        commonParameters.Overwrite,
        outputPath,
        commonParameters.Quality,
        commonParameters.Jpeg,
        new ResizeImageOptions(commonParameters.Width, commonParameters.Height, commonParameters.Percent));

    Console.WriteLine($"Optimizations completed.");

    return Task.CompletedTask;
});

app.Run();
return;

// Helper method to validate parameters
bool ValidateParameters(CommonParameters parameters)
{
    if ((parameters.Width != null || parameters.Height != null) && (parameters.Percent != null))
    {
        Console.WriteLine("Width/Height options are mutually exclusive with Percentage.");
        return false;
    }

    return true;
}


void PrintPaths(string inputPath, string outputPath)
{
    Console.WriteLine($"Input directory: {inputPath}");
    Console.WriteLine($"Output directory: {outputPath}");
}