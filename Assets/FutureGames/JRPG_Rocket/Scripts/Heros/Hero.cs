using System.Collections.Generic;
using UnityEngine;

namespace FutureGames.JRPG_Rocket
{
    public enum HeroState
    {
        Sleeping,
        Selected,
        Active,
    }

    public abstract class Hero
    {
        protected HeroState heroState = HeroState.Sleeping;

        protected GameObject gameObject = null;

        protected Queue<Command> commands = new Queue<Command>();

        public abstract void AddCommand(Command command);

        public abstract void ExecuteCommands();

        public abstract void MoveForward();
        public abstract void MoveBackward();
        public abstract void MoveRight();
        public abstract void MoveLeft();

        public abstract void ChangeState(HeroState newState);

        protected void Move(Vector3 amount)
        {
            gameObject.transform.Translate(amount);
        }
    }
}