namespace Optimus.Cli;

public static class DirHelper
{
    public static string ProcessRootPath(string workingDir, string inputPath)
    {
        if (!Path.IsPathRooted(inputPath))
        {
            inputPath = Path.Combine(workingDir, inputPath);
        }

        return Path.GetFullPath(inputPath);
    }

    public static void CreateDirectoryIfNotExists(params string[] directories)
    {
        foreach (var directory in directories)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }

    public static List<FileInfo> GetSupportedImages(IEnumerable<string> supportedTypes, string dirPath) =>
        Directory.GetFiles(dirPath)
            .Select(file => new FileInfo(file))
            .Where(file => supportedTypes.Contains(Path.GetExtension(file.FullName)))
            .ToList();
}
