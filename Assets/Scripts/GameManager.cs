using System.Collections;
using UnityEngine;
using MK;
using System;

// The game has been programmed with an underlaying design pattern akin to the MVC-pattern (Model-View-Controller).
// The game logic and the visuals are seperated and only interact through a controller.
// This means that the game logic does not care how it is represented visually at all.
// This is very benefitial for games where you want certain systems to run even though they're
// not currently being represented on screen (enemies hidden by fog of war for example). It also makes
// serialization of save data easier since you, in the case of a tile-based game like this, simply
// can serialize a list of tiles and you'll practically have serialized the whole game state 
// (since the game logic classes aren't monobehaviours).

public class GameManager : MonoBehaviorSingleton<GameManager>
{
    [SerializeField] WorldManager world;
    [SerializeField] float ticWaitTime = 0.2f;   

    public Action<Character> onSelectCharacter = delegate { };
    public Action<Character> onExecuteCommand = delegate { };
    public Action<Character> onSelectTileTarget = delegate { };
    public Action<Character> onEndTurn = delegate { };
    public Action<Character[]> onStartGame = delegate { };

    Character characterOnTurn;
    int characterTurnIndex;

    public bool IsExecutingCommands { get; private set; }
    public WorldManager World { get { return world; } }

    private void Awake()
    {
        world.onGeneratedWorld.AddListener(delegate { InitGame(); });
    }

    public void InitGame()
    {
        onStartGame.Invoke(world.AllCharacters);

        characterOnTurn = world.AllCharacters[0];
        onEndTurn.Invoke(characterOnTurn);
    }

    public void EndTurn()
    {
        ExecuteAllCharacterCommands(characterOnTurn, delegate { onEndTurn.Invoke(characterOnTurn); });

        Character[] allAliveCharacters = world.AllCharacters;
        characterTurnIndex = (characterTurnIndex + 1) % allAliveCharacters.Length;
        characterOnTurn = allAliveCharacters[characterTurnIndex];
    }

    /// <summary>
    /// Executes all commands for the character who's turn it currently is. Has a defined wait time in between each command.
    /// </summary>
    public void ExecuteAllCharacterCommands(Character character, Action onFinish)
    {
        if (IsExecutingCommands) return;

        StartCoroutine(DoExecuteAllCharacterCommands(character, onFinish));
    }

    private IEnumerator DoExecuteAllCharacterCommands(Character character, Action onFinish)
    {
        IsExecutingCommands = true;

        while (character.ExecuteCommand())
        {
            onExecuteCommand.Invoke(character);
            Debug.Log("Command Execute");
            yield return new WaitForSeconds(ticWaitTime);
        }

        onFinish.Invoke();

        IsExecutingCommands = false;
    }

    /// <summary>
    /// Executes all commands, in order on all characters, with a defined wait time in between each command.
    /// </summary>
    public void ExecuteAllCommands()
    {
        if (IsExecutingCommands) return;

        StartCoroutine(DoExecuteAllCommands());
    }

    private IEnumerator DoExecuteAllCommands()
    {
        IsExecutingCommands = true;
        foreach (Character character in world.AllCharacters)
        {
            while (character.ExecuteCommand())
            {
                onExecuteCommand.Invoke(character);
                yield return new WaitForSeconds(ticWaitTime);
            }          
        }
        IsExecutingCommands = false;
    }
}
