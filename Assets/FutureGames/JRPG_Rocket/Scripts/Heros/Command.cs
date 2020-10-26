using System;

namespace FutureGames.JRPG_Rocket
{
    public abstract class Command
    {
        public abstract void Execute(Hero hero);
    }
}