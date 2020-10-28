using UnityEngine;

public class CommandMove : Command
{
    public Tile target;

    public CommandMove(Tile target)
    {
        this.target = target;
    }

    public override void Execute(CommandInvoker commandInvoker)
    {
        if (commandInvoker is Character)
        {          
            Character character = (Character)commandInvoker;

            character.RemoveTargetTile(target);
            GameManager.Instance.MoveCharacter(character, target);
        }       
    }
}
