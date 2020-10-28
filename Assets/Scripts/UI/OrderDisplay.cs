using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class OrderDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    public int Order { get; private set; }
    public Image BackgroundImage { get; private set; }
    public Canvas Canvas { get; private set; }

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        BackgroundImage = GetComponentInChildren<Image>();
        Canvas = GetComponent<Canvas>();
    }

    public void SetOrder(int order)
    {
        textMesh.text = order.ToString();
        Order = order;
    }
}
