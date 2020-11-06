using MK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    [SerializeField] Vector2Int worldSize;
    [SerializeField] bool generateWorldOnAwake = true;

    [Header("Tile Settings")]
    [SerializeField] TileVisuals tilePrefab;
    [SerializeField] TileVisuals altTilePrefab;

    [Space]
    [SerializeField] float perlinScale = 0.001f;
    [SerializeField] float tileLerpSpeed = 10f;
    [SerializeField] float tileSpawnYPos = -5f;
    [SerializeField] float tileSpawnWaitTime = 0.1f;
    [SerializeField] MinMax minMaxTileHeight = new MinMax(0, 10);

    [Header("Game Settings")]
    [SerializeField] CharacterVisuals characterPrefab;

    [SerializeField] int playerCharacterCount = 3;
    [SerializeField] int enemyCharacterCount = 3;
    [SerializeField] float characterLerpSpeed = 10f;

    [SerializeField] Color[] teamColors;

    Tile[,] tiles;

    Dictionary<Tile, TileVisuals> tileToVisuals;
    Dictionary<TileVisuals, Tile> visualsToTile;
    Dictionary<Character, Tile> characterToTile;
    Dictionary<Character, CharacterVisuals> characterToVisuals;
    Dictionary<CharacterVisuals, Character> visualsToCharacter;

    List<Character> allCharacters;

    public UnityEvent onDestroyWorld;
    public UnityEvent onGeneratedWorld;

    public bool IsGeneratingWorld { get; private set; }
    public Character[] AllCharacters { get { return allCharacters.ToArray(); } }

    private void Awake()
    {
        if (generateWorldOnAwake)
        {
            GenerateWorld(tileSpawnWaitTime);
        }
    }

    private void Update()
    {
        UpdateTileVisuals();
    }

    public void UpdateTileVisuals()
    {
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                Tile tile = tiles[x, y];
                if (tile != null)
                {
                    UpdateTileLerps(tile, GetVisualsFromTile(tile), tileLerpSpeed);
                    UpdateTileCharacterVisuals(tile);
                }
            }
        }
    }

    public void UpdateTileCharacterVisuals(Tile tile)
    {
        if (tile.characterOnTile != null)
        {
            CharacterVisuals characterVisuals = GetVisualsFromCharacter(tile.characterOnTile);
            if (characterVisuals != null)
            {
                UpdateCharacterLerps(tile.characterOnTile, characterVisuals, characterLerpSpeed);
            }
            else
            {
                // There is a character here in data but not visible in the level. Spawn one.
                SpawnCharacter(tile.characterOnTile);
            }
        }
    }

    public void UpdateTileLerps(Tile tile, TileVisuals tileVisuals, float speed)
    {
        tileVisuals.transform.position = new Vector3
        (
            tile.position.x,
            Mathf.Lerp(tileVisuals.transform.position.y, tile.position.y, Time.deltaTime * speed),
            tile.position.z
        );
    }

    public void UpdateCharacterLerps(Character character, CharacterVisuals characterVisuals, float speed)
    {
        characterVisuals.transform.position = Vector3.Lerp(characterVisuals.transform.position, character.position, Time.deltaTime * speed);
    }

    private void InitCharacter(Character character, Tile tile)
    {
        characterToTile.Add(character, tile);
        tile.characterOnTile = character;
        allCharacters.Add(character);
    }

    public void MoveCharacter(Character character, Tile targetTile)
    {
        RemoveCharacterFromTile(character);
        targetTile.characterOnTile = character;
        characterToTile.Add(character, targetTile);
        character.position = targetTile.position;
    }

    public void GenerateWorld()
    {
        GenerateWorld(tileSpawnWaitTime);
    }

    public void GenerateWorld(float spawnWaitTime)
    {
        if (IsGeneratingWorld) return;

        StartCoroutine(DoGenerateWorld(spawnWaitTime));
    }

    private IEnumerator DoGenerateWorld(float waitTime)
    {
        IsGeneratingWorld = true;

        DestroyAllTiles();

        tileToVisuals = new Dictionary<Tile, TileVisuals>();
        visualsToTile = new Dictionary<TileVisuals, Tile>();
        characterToTile = new Dictionary<Character, Tile>();
        characterToVisuals = new Dictionary<Character, CharacterVisuals>();
        visualsToCharacter = new Dictionary<CharacterVisuals, Character>();

        allCharacters = new List<Character>();

        tiles = new Tile[worldSize.x, worldSize.y];

        Vector2 perlinOrigin = new Vector2
        (
            UnityEngine.Random.Range(-1000, 1000),
            UnityEngine.Random.Range(-1000, 1000)
        );

        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                yield return new WaitForSeconds(waitTime);

                float perlinHeight = Mathf.PerlinNoise(perlinOrigin.x + x * perlinScale, perlinOrigin.y + y * perlinScale);
                perlinHeight = Mathfs.Remap(0f, 1f, minMaxTileHeight.min, minMaxTileHeight.max, perlinHeight);
                perlinHeight = Mathf.FloorToInt(perlinHeight);

                tiles[x, y] = new Tile(new Vector3(x, perlinHeight, y));

                bool oddTile = (x + y) % 2 == 0f;
                SpawnTileVisual(tiles[x, y], oddTile);
            }
        }

        // Create player characters
        for (int i = 0; i < playerCharacterCount; i++)
        {
            Tile randomTile = GetRandomTile();
            Character character = new Character("P_Character " + i, randomTile.position, new Destructible(20f, 20f));
            character.destructible.onDeath += delegate { DestroyCharacter(character); };
            InitCharacter(character, randomTile);
        }

        // Create enemy characters
        for (int i = 0; i < enemyCharacterCount; i++)
        {
            Tile randomTile = GetRandomTile();
            Character character = new Character("E_Character " + i, randomTile.position, new Destructible(20f, 20f));
            character.destructible.onDeath += delegate { DestroyCharacter(character); };
            character.factionID = 1;
            InitCharacter(character, randomTile);
        }

        onGeneratedWorld.Invoke();

        IsGeneratingWorld = false;
    }

    private void DestroyAllTiles()
    {
        if (tiles == null || tiles.Length <= 0) return;

        onDestroyWorld.Invoke();

        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                DestroyTile(tiles[x, y]);
            }
        }

        tileToVisuals.Clear();
        visualsToTile.Clear();
        tiles = null;
    }

    private void DestroyTile(Tile tile)
    {
        // Destroy character
        if (tile.characterOnTile != null)
        {
            CharacterVisuals characterVisuals = GetVisualsFromCharacter(tile.characterOnTile);
            Destroy(characterVisuals.gameObject);
        }

        // Dedstory tile visuals
        TileVisuals tileVisuals = GetVisualsFromTile(tile);

        if (tileVisuals != null)
        {
            Destroy(tileVisuals.gameObject);
        }
    }

    private Tile GetRandomTile()
    {
        if (tiles == null || tiles.Length <= 0) return null;

        Vector2Int randomCoordinate = new Vector2Int
        (
            UnityEngine.Random.Range(0, worldSize.x),
            UnityEngine.Random.Range(0, worldSize.y)
        );

        return tiles[randomCoordinate.x, randomCoordinate.y];
    }

    private TileVisuals SpawnTileVisual(Tile tile, bool alternativeTile = false)
    {
        Vector3 spawnPosition = new Vector3(tile.position.x, tileSpawnYPos, tile.position.z);
        TileVisuals spawnedTileVisuals = alternativeTile ?
            Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform) :
            Instantiate(altTilePrefab, spawnPosition, Quaternion.identity, transform);

        spawnedTileVisuals.gameObject.name = "Tile " + tile.position.ToString();

        tileToVisuals.Add(tile, spawnedTileVisuals);
        visualsToTile.Add(spawnedTileVisuals, tile);

        return spawnedTileVisuals;
    }

    private CharacterVisuals SpawnCharacter(Character character)
    {
        Vector3 spawnPos = character.position;
        spawnPos.y = tileSpawnYPos;

        CharacterVisuals spawnedCharacter = Instantiate(characterPrefab, spawnPos, Quaternion.identity, transform);
        spawnedCharacter.gameObject.name = character.characterName;

        spawnedCharacter.SetShirtColor(teamColors[character.factionID % teamColors.Length]);

        characterToVisuals.Add(character, spawnedCharacter);
        visualsToCharacter.Add(spawnedCharacter, character);

        return spawnedCharacter;
    }

    public void DestroyCharacter(Character character)
    {
        allCharacters.Remove(character);
        RemoveCharacterFromTile(character);

        CharacterVisuals charVis = GetVisualsFromCharacter(character);
        if (charVis)
        {
            characterToVisuals.Remove(character);
            visualsToCharacter.Remove(charVis);
            Destroy(charVis.gameObject);
        }
    }

    public bool RemoveCharacterFromTile(Character character)
    {
        Tile tile = null;
        if (characterToTile.TryGetValue(character, out tile))
        {
            characterToTile.Remove(character);
            tile.characterOnTile = null;
            return true;
        }

        return false;
    }

    #region DictionaryGetters
    public TileVisuals GetVisualsFromTile(Tile tile)
    {
        TileVisuals tileVisuals = null;
        if (tileToVisuals.TryGetValue(tile, out tileVisuals))
        {
            return tileVisuals;
        }

        return null;
    }

    public Tile GetTileFromVisuals(TileVisuals tileVisuals)
    {
        Tile tile = null;
        if (visualsToTile.TryGetValue(tileVisuals, out tile))
        {
            return tile;
        }

        return null;
    }

    public CharacterVisuals GetVisualsFromCharacter(Character character)
    {
        CharacterVisuals characterVisuals = null;
        if (characterToVisuals.TryGetValue(character, out characterVisuals))
        {
            return characterVisuals;
        }

        return null;
    }

    public Character GetCharacterFromVisuals(CharacterVisuals characterVisuals)
    {
        Character character = null;
        if (visualsToCharacter.TryGetValue(characterVisuals, out character))
        {
            return character;
        }

        return null;
    }

    public Tile GetTileFromCharacter(Character character)
    {
        Tile tile = null;
        if (characterToTile.TryGetValue(character, out tile))
        {
            return tile;
        }

        return null;
    }
    #endregion
}
