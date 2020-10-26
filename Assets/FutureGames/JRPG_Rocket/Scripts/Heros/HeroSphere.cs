using UnityEngine;

namespace FutureGames.JRPG_Rocket
{
    public class HeroSphere : Hero
    {
        float moveAmount = 1f;

        public HeroSphere(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public override void AddCommand(Command command)
        {
            commands.Enqueue(command);
        }

        public override void ChangeState(HeroState newState)
        {
            heroState = newState;
        }

        public override void ExecuteCommands()
        {
            foreach (Command t in commands)
            {
                t.Execute(this);
            }
            commands.Clear();
        }

        public override void MoveBackward()
        {
            Move(Vector3.back * moveAmount);
        }

        public override void MoveForward()
        {
            Move(Vector3.forward * moveAmount);
        }

        public override void MoveLeft()
        {
            Move(Vector3.left * moveAmount);
        }

        public override void MoveRight()
        {
            Move(Vector3.right * moveAmount);
        }
    }
}