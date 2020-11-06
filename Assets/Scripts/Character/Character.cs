
using UnityEngine;

public class Character : CommandInvoker
{
    public string characterName = "DefaultCharacter";
    public Vector3 position;
    public Destructible destructible;
    public int factionID;

    public Character(string characterName, Vector3 position, Destructible destructible)
    {
        this.characterName = characterName;
        this.position = position;
        this.destructible = destructible;
    }

    public bool IsEnemy(Character character)
    {
        return character.factionID != factionID;
    }
}
