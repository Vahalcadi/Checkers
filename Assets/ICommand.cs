using System.Collections;

public interface ICommand
{
    public IEnumerator Execute();
    public void Undo();
}
