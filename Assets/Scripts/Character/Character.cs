
using UnityEngine;

public class Character : CommandInvoker
{
    public string characterName = "DefaultCharacter";
    public Vector3 position;

    public Character(string characterName, Vector3 position)
    {
        this.characterName = characterName;
        this.position = position;
    }
}
