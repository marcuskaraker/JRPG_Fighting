namespace FutureGames.JRPG_Rocket
{
    public class CommandMoveBackward : Command
    {
        public override void Execute(Hero hero)
        {
            hero.MoveBackward();
        }
    }
}