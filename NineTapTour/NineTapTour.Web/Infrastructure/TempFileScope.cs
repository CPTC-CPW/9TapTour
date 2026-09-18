using Microsoft.AspNetCore.Components.Forms;

namespace NineTapTour.Web.Infrastructure;

/// <summary>
/// Saves an uploaded browser file to a temp path with its original extension
/// (the Core import/export services work on file paths) and deletes it on
/// dispose.
/// </summary>
public sealed class TempFileScope : IDisposable
{
    public const long MaxUploadBytes = 20 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".xlsx", ".xlsm", ".xls" };

    private TempFileScope(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public static bool IsAllowedExcelFile(string fileName)
    {
        return AllowedExtensions.Contains(System.IO.Path.GetExtension(fileName));
    }

    public static async Task<TempFileScope> FromBrowserFileAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        if (!IsAllowedExcelFile(file.Name))
        {
            throw new InvalidOperationException("Only Excel files (.xlsx, .xlsm, .xls) can be uploaded.");
        }

        string extension = System.IO.Path.GetExtension(file.Name);
        string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"9tap-upload-{Guid.NewGuid():N}{extension}");

        await using FileStream output = File.Create(path);
        await using Stream input = file.OpenReadStream(MaxUploadBytes, cancellationToken);
        await input.CopyToAsync(output, cancellationToken);

        return new TempFileScope(path);
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
        catch (IOException)
        {
            // Best effort; the temp folder is cleaned by the OS eventually.
        }
    }
}
