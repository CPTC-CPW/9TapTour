using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Services;
using System;
using System.Threading;
using System.Windows.Forms;

namespace NineTapTourTests.Services
{
    [TestClass]
    public class FormNavigatorTests
    {
        private class TestForm : Form
        {
        }

        [TestMethod]
        public void ShowSingleton_SecondCall_ReturnsSameInstance()
        {
            RunSta(() =>
            {
                using ServiceProvider provider = BuildProvider();
                FormNavigator navigator = new(provider);

                TestForm first = navigator.ShowSingleton<TestForm>();
                TestForm second = navigator.ShowSingleton<TestForm>();

                Assert.AreSame(first, second);
                first.Close();
            });
        }

        [TestMethod]
        public void ShowSingleton_AfterClose_CreatesNewInstance()
        {
            RunSta(() =>
            {
                using ServiceProvider provider = BuildProvider();
                FormNavigator navigator = new(provider);

                TestForm first = navigator.ShowSingleton<TestForm>();
                first.Close();
                TestForm second = navigator.ShowSingleton<TestForm>();

                Assert.AreNotSame(first, second);
                second.Close();
            });
        }

        private static ServiceProvider BuildProvider()
        {
            ServiceCollection services = new();
            services.AddTransient<TestForm>();
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// WinForms controls require an STA thread; MSTest runs tests on MTA
        /// threads by default, so form tests run on a dedicated STA thread.
        /// </summary>
        private static void RunSta(Action action)
        {
            Exception failure = null;
            Thread thread = new(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (failure != null)
            {
                throw failure;
            }
        }
    }
}
