using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] LayerMask terrainMask;
    [SerializeField] GameObject selectorPrefab;
    [SerializeField] GameObject characterSelectorPrefab;

    [Space]
    [SerializeField] bool lerpSelectorPos;
    [SerializeField] float selectorLerpSpeed = 10f;

    GameObject selector;
    GameObject characterSelector;

    Tile lastSelectedTile;
    Character selectedCharater;

    private void Awake()
    {
        selector = Instantiate(selectorPrefab, transform);
        characterSelector = Instantiate(characterSelectorPrefab, transform);

        selector.SetActive(false);
        characterSelector.SetActive(false);

        GameManager.Instance.onDestroyWorld.AddListener(delegate { ResetInputManager(); });
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, terrainMask))
        {
            selector.SetActive(true);
            MoveSelector(hit.transform.position, lerpSelectorPos);

            if (Input.GetMouseButtonDown(0)) // Left click
            {
                SelectTile(hit.transform);
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
            MoveCharacterSelector(selectedCharater.position, true);
            characterSelector.SetActive(true);
        }
        else
        {
            characterSelector.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.ExecuteAllCommands();
        }
    }

    private bool SelectTile(Transform transform, bool selectForTarget = false)
    {
        TileVisuals selectedTileVisuals = transform.GetComponent<TileVisuals>();
        if (selectedTileVisuals != null)
        {
            lastSelectedTile = GameManager.Instance.GetTileFromVisuals(selectedTileVisuals);
            if (lastSelectedTile != null)
            {
                if (selectForTarget && selectedCharater != null)
                {
                    if (lastSelectedTile.characterOnTile != null)
                    {
                        
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
                        selectedCharater = lastSelectedTile.characterOnTile;
                        GameManager.Instance.onSelectCharacter(selectedCharater);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public void MoveSelector(Vector3 targetPos, bool lerp)
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

    public void MoveCharacterSelector(Vector3 targetPos, bool lerp)
    {
        if (lerp)
        {
            characterSelector.transform.position = Vector3.Lerp(characterSelector.transform.position, targetPos, Time.deltaTime * selectorLerpSpeed);
        }
        else
        {
            characterSelector.transform.position = targetPos;
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
