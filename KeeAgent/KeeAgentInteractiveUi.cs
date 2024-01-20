using System;
using System.Threading;
using System.Windows.Forms;

namespace KeeAgent
{
  /// <summary>
  /// Provides a separate UI thread for interactive communication without interlocking with the MainWindow.
  /// </summary>
  public sealed class KeeAgentInteractiveUi : IDisposable
  {
    private const string ComponentName = "KeeAgent Interactive UI";
    private SynchronizationContext _synchronizationContext;
    private ApplicationContext _applicationContext;

    public KeeAgentInteractiveUi()
    {
      var uiThread = new Thread(UiThreadMain) {
        Name = ComponentName,
        IsBackground = true // active background thread will not block the application shutdown process
      };
      uiThread.SetApartmentState(ApartmentState.STA);
      uiThread.Start();
    }

    private void UiThreadMain()
    {
      var mainForm = new Form {
        // actually we don't need the window, but there is no other way to capture the SynchronizationContext
        Name = ComponentName,
        Text = ComponentName,
        // some recommendations from https://stackoverflow.com/a/683991
        FormBorderStyle = FormBorderStyle.FixedToolWindow, // to exclude from appearing in Alt-Tab
        ShowInTaskbar = false, // to exclude from the Taskbar
      };

      mainForm.Load += (sender, args) => {
        // capture SynchronizationContext to send messages to the message loop
        _synchronizationContext = SynchronizationContext.Current;

        // hide the form... we have to postpone the call, otherwise it stays visible
        _synchronizationContext.Post(_ => mainForm.Hide(), null);
      };

      _applicationContext = new ApplicationContext(mainForm);
      Application.Run(_applicationContext);
    }

    /// <summary>
    /// Performs message loop shutdown.
    /// </summary>
    public void Dispose()
    {
      if (_synchronizationContext == null) {
        return;
      }

      var synchronizationContext = _synchronizationContext;
      _synchronizationContext = null;
      synchronizationContext.Post(_ => _applicationContext.ExitThread(), null);
    }

    /// <summary>
    /// Invokes a delegate on a separate UI Thread, dedicated for the KeeAgent Plugin
    /// </summary>
    /// <param name="action">The delegate to call.</param>
    /// <exception cref="InvalidOperationException">The method was called in while the SynchronizationContext is not
    /// captured yet or have been disposed already</exception>
    public void Invoke(Action action)
    {
      if (_synchronizationContext == null) {
        throw new InvalidOperationException(
          "KeeAgentInteractiveUi: the SynchronizationContext is not captured yet or have been disposed already");
      }

      _synchronizationContext.Send(_ => action.Invoke(), null);
    }
  }
}
