using System;
using System.Threading;
using System.Windows.Forms;

namespace KeeAgent
{
  /// <summary>
  /// Provides a separate UI thread for interactive communication without interlocking with the MainWindow.
  /// </summary>
  public sealed class KeeAgentUiThread : IDisposable
  {
    private const string ComponentName = "KeeAgent Interactive UI";
    private SynchronizationContext _synchronizationContext;
    private ApplicationContext _applicationContext;

    public KeeAgentUiThread()
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
      _synchronizationContext = new WindowsFormsSynchronizationContext();
      SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
      _applicationContext = new ApplicationContext();
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
      var applicationContext = _applicationContext;
      _applicationContext = null;
      synchronizationContext.Post(_ => applicationContext.ExitThread(), null);
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
