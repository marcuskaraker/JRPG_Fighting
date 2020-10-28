using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] OrderDisplay orderDisplayPrefab;

    Dictionary<Character, List<OrderDisplay>> characterOrderDisplayMap = new Dictionary<Character, List<OrderDisplay>>();
    List<OrderDisplay> allOrderDisplays = new List<OrderDisplay>();

    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;     
    }

    private void OnEnable()
    {
        GameManager.Instance.onSelectCharacter += ShowOrderDisplays;
        GameManager.Instance.onSelectTileTarget += ShowOrderDisplays;
        GameManager.Instance.onExecuteCommand += ShowOrderDisplays;
        GameManager.Instance.onStartGame += InitCharacterDisplays;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.onSelectCharacter -= ShowOrderDisplays;
        GameManager.Instance.onSelectTileTarget -= ShowOrderDisplays;
        GameManager.Instance.onExecuteCommand -= ShowOrderDisplays;
        GameManager.Instance.onStartGame -= InitCharacterDisplays;
    }

    public void InitCharacterDisplays(Character[] characters)
    {
        ResetAllOrderDisplays();

        for (int i = 0; i < characters.Length; i++)
        {
            characterOrderDisplayMap.Add(characters[i], new List<OrderDisplay>());
        }
    }

    public void ResetAllOrderDisplays()
    {
        for (int i = 0; i < allOrderDisplays.Count; i++)
        {
            Destroy(allOrderDisplays[i].gameObject);
        }

        allOrderDisplays.Clear();
        characterOrderDisplayMap.Clear();
    }

    public void ShowOrderDisplays(Character character)
    {
        for (int i = 0; i < allOrderDisplays.Count; i++)
        {
            allOrderDisplays[i].gameObject.SetActive(false);
        }

        List<OrderDisplay> orderDisplays = GetCharacterOrderDisplays(character);
        if (orderDisplays != null)
        {
            if (orderDisplays.Count < character.targetTiles.Count) // Too few, spawn new ones.
            {
                for (int i = orderDisplays.Count; i < character.targetTiles.Count; i++)
                {
                    SpawnOrderDisplay(orderDisplays, character, character.targetTiles[i].targetTile, i);
                }
            }
            else if (orderDisplays.Count > character.targetTiles.Count) // Too many, destroy a few.
            {
                for (int i = 0; i < orderDisplays.Count; i++)
                {
                    bool tileForOrderExists = false;
                    for (int j = 0; j < character.targetTiles.Count; j++)
                    {
                        if (orderDisplays[i].Order == character.targetTiles[j].orderID)
                        {
                            tileForOrderExists = true;
                            break;
                        }
                    }

                    if (!tileForOrderExists)
                    {
                        DestroyOrderDisplay(i, orderDisplays);
                    }
                }
            }

            for (int i = 0; i < orderDisplays.Count; i++)
            {
                orderDisplays[i].gameObject.SetActive(true);
                orderDisplays[i].transform.forward = mainCamera.transform.forward;
            }
        }
    }

    public OrderDisplay SpawnOrderDisplay(List<OrderDisplay> orderDisplays, Character character, Tile tile, int number)
    {
        if (!characterOrderDisplayMap.ContainsKey(character))
        {
            Debug.LogError("Tried to spawn display for character that does not exist in the UI references."); 
            return null;
        }

        OrderDisplay spawnedOrderDisplay = Instantiate(orderDisplayPrefab, tile.position, Quaternion.identity);
        orderDisplays.Add(spawnedOrderDisplay);
        allOrderDisplays.Add(spawnedOrderDisplay);

        spawnedOrderDisplay.SetOrder(number);

        return spawnedOrderDisplay;
    }

    public void DestroyOrderDisplay(int i, List<OrderDisplay> orderDisplays)
    {
        OrderDisplay orderDisplay = orderDisplays[i];
        orderDisplays.RemoveAt(i);
        allOrderDisplays.Remove(orderDisplay);
        Destroy(orderDisplay.gameObject);
    }

    private List<OrderDisplay> GetCharacterOrderDisplays(Character character)
    {
        List<OrderDisplay> orderDisplays = null;
        if (characterOrderDisplayMap.TryGetValue(character, out orderDisplays))
        {
            return orderDisplays;
        }

        return null;
    }
}
