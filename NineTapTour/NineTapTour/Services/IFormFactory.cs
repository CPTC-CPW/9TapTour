using System.Windows.Forms;

namespace NineTapTour.Services;

/// <summary>
/// Creates form instances with constructor arguments, filling any remaining
/// constructor parameters from the service container. Used for dialogs and
/// forms that take runtime data (for example a Tournament) in their constructor.
/// </summary>
public interface IFormFactory
{
    T Create<T>(params object[] args) where T : Form;
}
