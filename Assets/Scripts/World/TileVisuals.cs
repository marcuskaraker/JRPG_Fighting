using UnityEngine;

public class TileVisuals : MonoBehaviour
{
    [SerializeField] MeshRenderer grassRenderer;
    [SerializeField] MeshRenderer dirtRenderer;

    public MeshRenderer GrassRenderer { get { return grassRenderer; } }
    public MeshRenderer DirtRenderer { get { return dirtRenderer; } }
}
