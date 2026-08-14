using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace NineTapTourTests.Architecture
{
    [TestClass]
    public class CoreDependencyTests
    {
        /// <summary>
        /// NineTapTour.Core must stay consumable by a future ASP.NET website:
        /// it may never reference WinForms or GDI+ drawing assemblies. This
        /// gate fails the build if a UI dependency sneaks into Core.
        /// </summary>
        [TestMethod]
        public void Core_DoesNotReferenceWinFormsOrDrawing()
        {
            var coreAssembly = typeof(NineTapTour.Core.Data.NineTapDb).Assembly;
            string[] forbidden = ["System.Windows.Forms", "System.Drawing", "System.Drawing.Common"];

            var offending = coreAssembly.GetReferencedAssemblies()
                .Where(reference => forbidden.Contains(reference.Name, StringComparer.OrdinalIgnoreCase))
                .Select(reference => reference.Name)
                .ToList();

            Assert.AreEqual(0, offending.Count,
                "NineTapTour.Core references UI assemblies: " + string.Join(", ", offending));
        }
    }
}
