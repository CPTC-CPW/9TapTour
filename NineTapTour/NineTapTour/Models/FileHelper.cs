using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models
{
    public static class FileHelper
    {
        /// <summary>
        /// Returns a string for OpenFileDialog and SaveFileDialog Filters
        /// to filter for Excel file extensions
        /// </summary>
        /// <returns></returns>
        public static string GetExcelFilterStringForFileDialogs()
        {
            return "Excel Files (*.xls;*.xlsx;*.xlsm)|*.xls;*.xlsx;*.xlsm";
        }

        /// <summary>
        /// Checks if the given extension is a valid Excel extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsValidExcelExtension(string extension)
        {
            return extension == ".xlsx" || extension == ".xlsm";
        }

        /// <summary>
        /// Copies the drawing parts (xl/drawings/*) from <paramref name="templatePath"/> into
        /// <paramref name="outputPath"/> verbatim, replacing whatever ClosedXML wrote there.
        ///
        /// ClosedXML scrambles picture anchors on save when a worksheet contains multiple
        /// pictures sharing the same shape name (its picture collection is keyed by name).
        /// The client check templates have three check signature images per sheet, all named
        /// "Picture 8", so after export the signature images end up stacked on the wrong
        /// checks and Excel drops the resulting dangling references. We never modify
        /// drawings — only cell values/formulas — so restoring the template's drawing parts
        /// byte-for-byte is always safe. Image media and relationship parts are untouched
        /// by ClosedXML and keep their original names, so only xl/drawings/* needs restoring.
        /// </summary>
        public static void RestoreTemplateDrawings(string templatePath, string outputPath)
        {
            // Saving over the template itself: the original drawings are gone, nothing to restore.
            if (string.Equals(System.IO.Path.GetFullPath(templatePath),
                              System.IO.Path.GetFullPath(outputPath),
                              StringComparison.OrdinalIgnoreCase))
                return;

            using var templateZip = ZipFile.OpenRead(templatePath);
            using var outputZip = ZipFile.Open(outputPath, ZipArchiveMode.Update);

            foreach (var templateEntry in templateZip.Entries.Where(e =>
                         e.FullName.StartsWith("xl/drawings/", StringComparison.OrdinalIgnoreCase)))
            {
                var outputEntry = outputZip.GetEntry(templateEntry.FullName);
                if (outputEntry == null) continue;

                outputEntry.Delete();
                var restored = outputZip.CreateEntry(templateEntry.FullName);
                using var srcStream = templateEntry.Open();
                using var dstStream = restored.Open();
                srcStream.CopyTo(dstStream);
            }
        }
    }
}
