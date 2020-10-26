using UnityEngine;
using System.Collections.Generic;

public class CommandInvoker
{
    Queue<Command> commands = new Queue<Command>();

    public void AddCommand(Command command)
    {
        commands.Enqueue(command);
    }

    public bool ExecuteCommand()
    {
        if (commands.Count > 0)
        {
            Command command = commands.Dequeue();
            command.Execute(this);
            return true;
        }

        return false;
    }
}
