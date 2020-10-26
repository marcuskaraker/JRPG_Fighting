namespace FutureGames.JRPG_Rocket
{
    public class CommandMoveRight : Command
    {
        public override void Execute(Hero hero)
        {
            hero.MoveRight();
        }
    }
}