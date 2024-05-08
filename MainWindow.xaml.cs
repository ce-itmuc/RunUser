namespace RunUserFix;

using System.ComponentModel;
using System.Windows;

public partial class MainWindow
{
    private readonly ExecutionStateAdapter _executionStateAdapter = new();
    private readonly NotifyIconAdapter _notifyIconAdapter;
    
    public MainWindow()
    {
        _notifyIconAdapter = new NotifyIconAdapter(this);
        _notifyIconAdapter.SetupNotifyIconAndContextMenu();
    }
    
    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _executionStateAdapter.RequireDisplayOn();
    }

    private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
    {
        _executionStateAdapter.Shutdown();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        _notifyIconAdapter.SendToNotificationArea();
    }
}