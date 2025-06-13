using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private Vector2 offset = new Vector2(10, -10);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (tooltipText.gameObject.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            tooltipText.rectTransform.position = mousePos + offset;
        }
    }

    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        tooltipText.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipText.gameObject.SetActive(false);
    }
}