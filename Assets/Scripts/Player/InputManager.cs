﻿using UnityEngine;

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

            if (Input.GetMouseButtonDown(0))
            {
                SelectTile(hit.transform);
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
    }

    private bool SelectTile(Transform transform)
    {
        TileVisuals selectedTileVisuals = transform.GetComponent<TileVisuals>();
        if (selectedTileVisuals != null)
        {
            lastSelectedTile = GameManager.Instance.GetTileFromVisuals(selectedTileVisuals);
            if (lastSelectedTile != null)
            {
                if (lastSelectedTile.characterOnTile != null)
                {
                    selectedCharater = lastSelectedTile.characterOnTile;
                }

                return true;
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