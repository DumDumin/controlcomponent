using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ControlComponent;

internal class OperationModeCascade : OperationMode
{   
    private Dictionary<string, string> roleToOpMode;

    public OperationModeCascade(string name) : base(name, new Collection<string>() { "ROLE_ONE", "ROLE_TWO" } ) {
        roleToOpMode = new Dictionary<string, string>();
        roleToOpMode.Add("ROLE_ONE", "OpModeOne");
        roleToOpMode.Add("ROLE_TWO", "OpModeTwo");
    }

    private async Task WaitToFinish(CancellationToken token)
    {
        int counter = 0;
        while(!token.IsCancellationRequested && counter < 10)
        {
            await Task.Delay(1);
            counter++;
        }
    }

    private async Task DoTask(Func<CancellationToken, Task> action, CancellationToken token)
    {
        await WaitToFinish(token);

        // If Execution finished normally call base method
        if(!token.IsCancellationRequested)
        {
            await action(token);
        }
    }

    protected override Task Resetting(CancellationToken token)
    {
        foreach (var roleKV in roleToOpMode)
        {
            base.SelectRole(roleKV.Key, roleKV.Value);
        }

        return DoTask(base.Resetting, token);
    }

    protected override Task Starting(CancellationToken token)
    {
        return DoTask(base.Starting, token);
    }

    protected override Task Stopping(CancellationToken token)
    {
        return DoTask(base.Stopping, token);
    }

    protected override Task Execute(CancellationToken token)
    {
        return DoTask(base.Execute, token);
    }

    protected override Task Completing(CancellationToken token)
    {
        return DoTask(base.Completing, token);
    }

    protected override Task Suspending(CancellationToken token)
    {
        return DoTask(base.Suspending, token);
    }

    protected override Task Unsuspending(CancellationToken token)
    {
        return DoTask(base.Unsuspending, token);
    }

    protected override Task Holding(CancellationToken token)
    {
        return DoTask(base.Holding, token);
    }

    protected override Task Unholding(CancellationToken token)
    {
        return DoTask(base.Unholding, token);
    }

    protected override Task Aborting(CancellationToken token)
    {
        return DoTask(base.Aborting, token);
    }
}