using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database.Tests
{
    [TestClass()]
    public class FormHelperTests
    {
        [TestMethod()]
        [DataRow("300")]
        [DataRow("0")]
        [DataRow("090")]
        [DataRow("90")]
        public void IsAverageValidTest(string testData)
        {
            bool isValid = FormHelper.IsAverageValid(testData);
            Assert.IsTrue(isValid);
        }
    }
}