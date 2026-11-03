using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NineTapTour.Deployment;

public static class FileDownload
{
    /// <summary>
    /// Saves a file from a URL, or copies it from a folder, into the temp folder and returns its path.
    /// </summary>
    public static async Task<string> ToTempAsync(string source, string fileName)
    {
        string path = Path.Combine(Path.GetTempPath(), fileName);

        if (!source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(Path.Combine(source, fileName), path, overwrite: true);
            return path;
        }

        string url = source.TrimEnd('/') + "/" + fileName;
        using HttpClient http = new();
        using HttpResponseMessage response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using FileStream file = File.Create(path);
        await response.Content.CopyToAsync(file);
        return path;
    }
}
