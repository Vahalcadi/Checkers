using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandProcessor
{
    private Stack<ICommand> commands = new Stack<ICommand>();
    private Stack<ICommand> redoCommands = new Stack<ICommand>();

    public IEnumerator Execute(ICommand command)
    {
        commands.Push(command);
        yield return command.Execute();
    }

    public void Undo()
    {
        ICommand command;
        if (commands.TryPop(out command))
        {
            redoCommands.Push(command);
            command.Undo();
        }
    }

    public void UndoAll()
    {
        while (commands.Count > 0)
        {
            Undo();
        }
    }
}
