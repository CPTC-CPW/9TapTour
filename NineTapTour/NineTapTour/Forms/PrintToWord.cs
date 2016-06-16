using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Database;
using System.Globalization;
using NineTapTour.Exceptions;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
/// <summary>
/// Author Julie Edwards
/// </summary>
namespace NineTapTour.Forms
{
    class PrintToWord
    {
        //BEGIN WORD DOCUMENT CREATION

        //extra Info @
        //http://www.dotnetperls.com/streamwriter
        //    using (StreamWriter writer =
        //new StreamWriter("important.txt"))
        //    {
        //        writer.Write("Word ");
        //        writer.WriteLine("word 2");
        //        writer.WriteLine("Line");
        //    }
        //end extra info
        public void CreateWordDoc(String content) {
            string fileName = @"C:\Users\Public\WriteText.txt";
            File.CreateText(fileName);
            FileStream stream = File.Create(fileName);
            //string InsertParagraph = "TEST STRING";
            File.WriteAllText(fileName, content);
            stream.Close();
            
            // Open in Word:
            Process.Start("WINWORD.EXE", fileName);
            //End WORD DOCUMENT CREATION
        }
    }
}
