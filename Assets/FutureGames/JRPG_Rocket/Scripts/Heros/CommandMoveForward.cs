namespace FutureGames.JRPG_Rocket
{
    public class CommandMoveForward : Command
    {
        public override void Execute(Hero hero)
        {
            hero.MoveForward();
        }
    }
}