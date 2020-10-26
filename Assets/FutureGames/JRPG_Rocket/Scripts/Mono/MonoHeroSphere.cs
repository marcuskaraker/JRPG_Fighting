using UnityEngine;

namespace FutureGames.JRPG_Rocket
{
    public class MonoHeroSphere : MonoBehaviour
    {
        HeroSphere hero = null;

        private void Start()
        {
            hero = new HeroSphere(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
                hero.AddCommand(new CommandMoveForward());
            if (Input.GetKeyDown(KeyCode.S))
                hero.AddCommand(new CommandMoveBackward());

            if (Input.GetKeyDown(KeyCode.A))
                hero.AddCommand(new CommandMoveLeft());
            if (Input.GetKeyDown(KeyCode.D))
                hero.AddCommand(new CommandMoveRight());

            if (Input.GetKeyDown(KeyCode.Return))
                hero.ExecuteCommands();
        }

        private void OnMouseDown()
        {
            hero.ChangeState(HeroState.Selected);

        }
    }
}