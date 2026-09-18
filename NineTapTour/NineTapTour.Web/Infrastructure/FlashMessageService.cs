namespace NineTapTour.Web.Infrastructure;

/// <summary>
/// Circuit-scoped replacement for the desktop's MessageBox notifications. Pages
/// push messages; the FlashMessages component in the layout renders them as
/// dismissible alerts. Messages survive a navigation within the circuit so a
/// "saved" notice can be shown on the page navigated to.
/// </summary>
public sealed class FlashMessageService
{
    public enum Level
    {
        Success,
        Info,
        Warning,
        Error,
    }

    public sealed record Message(Level Level, string Text)
    {
        public string CssClass => Level switch
        {
            Level.Success => "alert-success",
            Level.Info => "alert-info",
            Level.Warning => "alert-warning",
            _ => "alert-danger",
        };
    }

    private readonly List<Message> messages = [];

    public event Action? Changed;

    public IReadOnlyList<Message> Messages => messages;

    public void Success(string text) => Add(Level.Success, text);

    public void Info(string text) => Add(Level.Info, text);

    public void Warning(string text) => Add(Level.Warning, text);

    public void Error(string text) => Add(Level.Error, text);

    public void Dismiss(Message message)
    {
        if (messages.Remove(message))
        {
            Changed?.Invoke();
        }
    }

    public void Clear()
    {
        if (messages.Count == 0)
        {
            return;
        }
        messages.Clear();
        Changed?.Invoke();
    }

    private void Add(Level level, string text)
    {
        messages.Add(new Message(level, text));
        Changed?.Invoke();
    }
}
