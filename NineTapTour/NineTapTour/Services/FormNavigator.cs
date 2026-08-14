using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NineTapTour.Services;

public class FormNavigator : IFormNavigator
{
    private readonly IServiceProvider provider;
    private readonly Dictionary<Type, Form> openForms = [];
    private Form mdiParent;

    public FormNavigator(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public void RegisterMdiParent(Form mdiParent)
    {
        this.mdiParent = mdiParent;
    }

    public T ShowSingleton<T>() where T : Form
    {
        if (openForms.TryGetValue(typeof(T), out Form form) && !form.IsDisposed)
        {
            form.BringToFront();
            form.Activate();
        }
        else
        {
            form = provider.GetRequiredService<T>();
            form.MdiParent = mdiParent;
            form.FormClosed += (_, _) => openForms.Remove(typeof(T));
            openForms[typeof(T)] = form;
        }

        form.WindowState = FormWindowState.Maximized;
        form.ControlBox = false;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.Show();
        return (T)form;
    }
}
