using UnityEngine;

public class CommandAttack : Command
{
    public Character target;

    public CommandAttack(Character target)
    {
        this.target = target;
    }

    public override void Execute(CommandInvoker commandInvoker)
    {
        if (commandInvoker is Character)
        {

        }
    }
}