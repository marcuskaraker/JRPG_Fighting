using UnityEngine;

public class CommandAttack : Command
{
    public Character target;
    public float damage = 10f;

    public CommandAttack(Character target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    public override void Execute(CommandInvoker commandInvoker)
    {
        if (commandInvoker is Character)
        {
            Character character = (Character)commandInvoker;

            Tile targetTile = GameManager.Instance.World.GetTileFromCharacter(target);
            bool targetDied = target.destructible.Hurt(10f);
            if (targetDied)
            {
                // If the target died. Move to their position (like chess).
                CommandMove move = new CommandMove(targetTile);
                move.Execute(commandInvoker);
            }

            Debug.Log(character.characterName + " hurt " + target.characterName + " with " + damage + " dmg");
        }
    }
}