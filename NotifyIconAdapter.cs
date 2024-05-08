namespace RunUserFix;

using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Drawing;

public class NotifyIconAdapter(Window parentWindow)
{
    private readonly Window _parentWindow = parentWindow ?? throw new ArgumentNullException(nameof(parentWindow));
    
    private readonly NotifyIcon _notifyIcon = new(); 
    private readonly ContextMenu _contextMenu = new();

    public void SetupNotifyIconAndContextMenu()
    {
        _notifyIcon.Icon = RetrieveIconFromResource();
        _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        
        MenuItem maximizeMenuItem = new()
        {
            Header = "Show Window"
        };
        maximizeMenuItem.Click += ContextMenuWindowNormal;
        _contextMenu.Items.Add(maximizeMenuItem);
        
        MenuItem exitMenuItem = new()
        {
            Header = "Exit"
        };
        exitMenuItem.Click += ContextMenuExit;
        _contextMenu.Items.Add(exitMenuItem);
    }

    public void SendToNotificationArea()
    {
        _parentWindow.WindowState = WindowState.Minimized;
        _parentWindow.Hide();
        
        _notifyIcon.Visible = true;
    }

    private static Icon RetrieveIconFromResource()
    {
        const string iconName = "RunUserFix.walkingman_white.ico";
        
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconName);
        if (stream == null)
        {
            throw new ApplicationException($"Resource {iconName} not found in assembly");
        }
        
        return new Icon(stream);
    }
    
    private void ContextMenuWindowNormal(object? sender, EventArgs e)
    {
        HideNotifyIconAndContextMenu();
            
        _parentWindow.Show();
        _parentWindow.WindowState = WindowState.Normal;
    }    

    private void ContextMenuExit(object? sender, EventArgs e)
    {
        HideNotifyIconAndContextMenu();
        
        System.Windows.Application.Current.Shutdown();
    }

    private void HideNotifyIconAndContextMenu()
    {
        _contextMenu.IsOpen = false;
        _notifyIcon.Visible = false;
    }
    
    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        _contextMenu.IsOpen = e.Button switch
        {
            MouseButtons.Left => false,
            MouseButtons.Right => true,
            _ => _contextMenu.IsOpen
        };
    }
}