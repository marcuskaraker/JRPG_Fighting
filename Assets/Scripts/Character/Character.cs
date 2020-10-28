
using UnityEngine;

public class Character : CommandInvoker
{
    public string characterName = "DefaultCharacter";
    public Vector3 position;
    public Destructible destructible;

    public Character(string characterName, Vector3 position, Destructible destructible)
    {
        this.characterName = characterName;
        this.position = position;
        this.destructible = destructible;
    }
}
