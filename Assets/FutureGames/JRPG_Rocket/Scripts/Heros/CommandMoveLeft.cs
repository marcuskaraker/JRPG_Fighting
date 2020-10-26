namespace FutureGames.JRPG_Rocket
{
    public class CommandMoveLeft : Command
    {
        public override void Execute(Hero hero)
        {
            hero.MoveLeft();
        }
    }
}