using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Optimus.Cli;

public static class ImageProcessor
{
    public static async Task ProcessImages(
        FileInfo imageFile,
        bool overwrite,
        string outputPath,
        int quality,
        bool jpegFormat,
        ResizeImageOptions? resizeOptions)
    {
        try
        {
            if (!overwrite && FileExists(outputPath, imageFile.Name))
            {
                Console.WriteLine($"Output Already Contains: {imageFile.Name}");
                return;
            }

            using var image = await Image.LoadAsync(imageFile.OpenRead());

            MutateImage(image, resizeOptions);

            await image.SaveAsJpegAsync(outputPath, encoder: new JpegEncoder() { Quality = quality });
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            throw new Exception("Failed to optimize images", ex);
        }
    }

    public static async Task ProcessImages(
        IEnumerable<FileInfo> imageFiles,
        bool overwrite,
        string outputPath,
        int quality,
        bool jpegFormat,
        ResizeImageOptions? resizeOptions)
    {
        foreach (var imageFile in imageFiles)
        {
            await ProcessImages(imageFile,
                overwrite,
                $"{outputPath}/{imageFile.Name}{imageFile.Extension.ToLowerInvariant()}",
                quality,
                jpegFormat,
                resizeOptions);
        }
    }


    private static void MutateImage(Image image, ResizeImageOptions? resizeOptions)
    {
        if (resizeOptions == null) return;

        if (resizeOptions.Width.HasValue || resizeOptions.Height.HasValue)
        {
            var setWidth = resizeOptions.Width ?? 0;
            var setHeight = resizeOptions.Height ?? 0;

            image.Mutate(x => x.Resize(setWidth, setHeight));
        }
        else if (resizeOptions.Percent.HasValue)
        {
            var percentValue = ((double)resizeOptions.Percent / 100);
            var setWidth = (int)(image.Width * percentValue);
            var setHeight = (int)(image.Height * percentValue);

            image.Mutate(x => x.Resize(setWidth, setHeight));
        }
    }

    private static bool FileExists(string dirPath, string fileName) =>
        Directory.GetFiles(dirPath).Select(Path.GetFileName).Contains(fileName);
}

public record ResizeImageOptions(int? Width, int? Height, int? Percent);