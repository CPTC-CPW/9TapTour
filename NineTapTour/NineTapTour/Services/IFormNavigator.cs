using System.Windows.Forms;

namespace NineTapTour.Services;

/// <summary>
/// Opens MDI child forms resolved from the service container.
/// Replaces FrmMain.OpenOrDisplayForm and Application.OpenForms lookups.
/// </summary>
public interface IFormNavigator
{
    /// <summary>
    /// Registers the MDI parent that singleton forms are attached to.
    /// Called once by FrmMain when it is constructed.
    /// </summary>
    void RegisterMdiParent(Form mdiParent);

    /// <summary>
    /// Shows the form of type T, creating it from the container if it is not
    /// already open, or bringing the existing instance to the front if it is.
    /// Returns the instance that was shown.
    /// </summary>
    T ShowSingleton<T>() where T : Form;
}
