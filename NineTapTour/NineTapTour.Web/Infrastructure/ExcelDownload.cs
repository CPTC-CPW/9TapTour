using Microsoft.JSInterop;

namespace NineTapTour.Web.Infrastructure;

/// <summary>
/// Streams a generated workbook to the browser as a download (the documented
/// Blazor pattern: DotNetStreamReference plus a small JS helper).
/// </summary>
public sealed class ExcelDownload(IJSRuntime js)
{
    public const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string XlsmContentType = "application/vnd.ms-excel.sheet.macroEnabled.12";

    public async Task SendAsync(string fileName, Stream content)
    {
        content.Position = 0;
        using DotNetStreamReference reference = new(content, leaveOpen: true);
        string contentType = fileName.EndsWith(".xlsm", StringComparison.OrdinalIgnoreCase) ? XlsmContentType : XlsxContentType;
        await js.InvokeVoidAsync("nineTap.downloadFileFromStream", fileName, reference, contentType);
    }
}
