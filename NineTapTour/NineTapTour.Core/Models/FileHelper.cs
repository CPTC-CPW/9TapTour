using System;
using System.Collections.Generic;
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
    }
}
