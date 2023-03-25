using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KeeAgent.UI
{
  public partial class DecryptProgressDialog : Form
  {
    private class BackgroundProgress : IProgress<double>
    {
      private BackgroundWorker worker;

      public BackgroundProgress(BackgroundWorker worker)
      {
        this.worker = worker;
      }

      public void Report(double value)
      {
        var percent = (int)(100 * value);
        worker.ReportProgress(percent);
      }
    }

    public delegate object DecryptFunc(IProgress<double> progress);

    public DecryptProgressDialog()
    {
      InitializeComponent();
    }

    public int Progress { get { return progressBar.Value; } }

    public Exception Error { get; private set; }

    public object Result { get; private set; }

    public void Start(DecryptFunc decrypt)
    {
      if (decrypt == null) {
        throw new ArgumentNullException("decrypt");
      }

      backgroundWorker.RunWorkerAsync(decrypt);
    }

    private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      var decrypt = (DecryptFunc)e.Argument;
      var progress = new BackgroundProgress(backgroundWorker);
      e.Result = decrypt(progress);
    }

    private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      progressBar.Style = ProgressBarStyle.Continuous;
      progressBar.Value = e.ProgressPercentage;

      // HACK: avoid lag due to animation
      // https://stackoverflow.com/q/6071626/1976323
      if (e.ProgressPercentage > 0) {
        progressBar.Value = e.ProgressPercentage - 1;
        progressBar.Value = e.ProgressPercentage;
      }
    }

    private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Cancelled) {
        throw new NotSupportedException();
      }

      if (e.Error == null) {
        DialogResult = DialogResult.OK;
      }
      else {
        DialogResult = DialogResult.Abort;
      }

      Error = e.Error;
      Result = e.Result;

      Close();
    }
  }
}
