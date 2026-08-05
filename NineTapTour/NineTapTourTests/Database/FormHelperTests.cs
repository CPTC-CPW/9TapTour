using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Calculations;
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
            bool isValid = ValidationHelper.IsAverageValid(testData);
            Assert.IsTrue(isValid);
        }
    }
}