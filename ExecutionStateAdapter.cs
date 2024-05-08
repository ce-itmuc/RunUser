namespace RunUserFix;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public class ExecutionStateAdapter
{
    #region Kernel32 Reference
    
    [Flags]
    private enum ExecutionState : uint
    {
        EsContinuous = 0x80000000,
        EsDisplayRequired = 0x00000002,
        EsSystemRequired = 0x00000001
    }
    
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern uint SetThreadExecutionState(ExecutionState esFlags);
    
    #endregion

    private readonly AutoResetEvent _resetEvent = new (false);
    private readonly TaskFactory _taskFactory = new ();

    public void RequireDisplayOn()
    {
        _taskFactory.StartNew(() =>
        {
            SetThreadExecutionState(
                ExecutionState.EsContinuous
                | ExecutionState.EsDisplayRequired
                | ExecutionState.EsSystemRequired);
            _resetEvent.WaitOne();

        }, TaskCreationOptions.LongRunning);
    }

    public void Shutdown()
    {
        _resetEvent.Set();
    }
}
