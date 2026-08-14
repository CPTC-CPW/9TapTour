using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace NineTapTour.Services;

public class FormFactory : IFormFactory
{
    private readonly IServiceProvider provider;

    public FormFactory(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public T Create<T>(params object[] args) where T : Form
    {
        return ActivatorUtilities.CreateInstance<T>(provider, args);
    }
}
