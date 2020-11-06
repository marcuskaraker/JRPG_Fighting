using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] LayerMask terrainMask;
    [SerializeField] GameObject selectorPrefab;
    [SerializeField] GameObject characterSelectorPrefab;
    [SerializeField] GameObject characterAttackSelectorPrefab;

    [Space]
    [SerializeField] bool lerpSelectorPos;
    [SerializeField] float selectorLerpSpeed = 10f;

    GameObject selector;
    GameObject characterSelector;
    GameObject characterAttackSelector;

    Tile lastSelectedTile;
    Character selectedCharater;
    Character selectedCharacterForAttack;

    private void Awake()
    {
        selector = Instantiate(selectorPrefab, transform);
        characterSelector = Instantiate(characterSelectorPrefab, transform);
        characterAttackSelector = Instantiate(characterAttackSelectorPrefab, transform);

        selector.SetActive(false);
        characterSelector.SetActive(false);

        GameManager.Instance.World.onDestroyWorld.AddListener(delegate { ResetInputManager(); });
        GameManager.Instance.onEndTurn += SelectCharacter;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, terrainMask))
        {
            selector.SetActive(true);
            MoveSelector(selector, hit.transform.position, lerpSelectorPos);

            if (Input.GetMouseButtonDown(0)) // Left click
            {
                //SelectTile(hit.transform);
            }
            else if (Input.GetMouseButtonDown(1)) // Right click
            {
                SelectTile(hit.transform, true);
            }
        }
        else
        {
            selector.SetActive(false);
        }

        if (selectedCharater != null)
        {
            MoveSelector(characterSelector, selectedCharater.position, true);
            characterSelector.SetActive(true);
        }
        else
        {
            characterSelector.SetActive(false);
        }

        if (selectedCharacterForAttack != null)
        {
            MoveSelector(characterAttackSelector, selectedCharacterForAttack.position, true);
            characterAttackSelector.SetActive(true);
        }
        else
        {
            characterAttackSelector.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.EndTurn();
        }

        // Debug Input
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameManager.Instance.World.GenerateWorld();
        }
    }

    private bool SelectTile(Transform transform, bool selectForTarget = false)
    {
        TileVisuals selectedTileVisuals = transform.GetComponent<TileVisuals>();
        if (selectedTileVisuals != null)
        {
            lastSelectedTile = GameManager.Instance.World.GetTileFromVisuals(selectedTileVisuals);
            if (lastSelectedTile != null)
            {
                if (selectForTarget && selectedCharater != null)
                {
                    if (lastSelectedTile.characterOnTile != null)
                    {
                        // Attack character on tile (if enemy)
                        selectedCharacterForAttack = lastSelectedTile.characterOnTile;
                        selectedCharater.AddCommand(new CommandAttack(selectedCharacterForAttack, 10f));
                    }
                    else
                    {
                        selectedCharater.AddTargetTile(lastSelectedTile);
                        selectedCharater.AddCommand(new CommandMove(lastSelectedTile));
                        GameManager.Instance.onSelectTileTarget(selectedCharater);
                    }
                }
                else
                {
                    if (lastSelectedTile.characterOnTile != null)
                    {
                        SelectCharacter(lastSelectedTile.characterOnTile);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public void SelectCharacter(Character character)
    {
        selectedCharater = character;
        selectedCharacterForAttack = null;
        GameManager.Instance.onSelectCharacter(character);
    }

    public void MoveSelector(GameObject selector, Vector3 targetPos, bool lerp)
    {
        if (lerp)
        {
            selector.transform.position = Vector3.Lerp(selector.transform.position, targetPos, Time.deltaTime * selectorLerpSpeed);
        }
        else
        {
            selector.transform.position = targetPos;
        }
    }

    public void ResetInputManager()
    {
        lastSelectedTile = null;
        selectedCharater = null;

        selector.SetActive(false);
        characterSelector.SetActive(false);
    }
}
