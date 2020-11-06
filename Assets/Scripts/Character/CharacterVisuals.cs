using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] Renderer clothesRenderer;

    public void SetShirtColor(Color color)
    {
        clothesRenderer.material.color = color;
    }
}
