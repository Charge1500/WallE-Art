using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LegendController : MonoBehaviour
{
    [Header("Objeto del panel de la leyenda")]
    [SerializeField] private GameObject legendObject;

    [Header("Referencias a Componentes")]
    [Tooltip("Referencia al ScriptableObject que contiene los datos del tema del nivel.")]
    [SerializeField] private LevelThemeData levelTheme;

    [Header("UI de la Leyenda")]
    [Tooltip("Texto para mostrar la descripción del objeto actual.")]
    [SerializeField] private TMP_Text descriptionText;

    [Tooltip("Array de imágenes que mostrarán el color. Pueden ser las 9 imágenes de tu grid 3x3.")]
    [SerializeField] private Image[] displayImages;

    [Tooltip("Botón para mostrar el siguiente ítem en la leyenda.")]
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject nextObject;

    [Tooltip("Botón para mostrar el ítem anterior en la leyenda.")]
    [SerializeField] private Button previousButton;
    [SerializeField] private GameObject previousObject;

    [Tooltip("Botón para cerrar leyenda.")]
    [SerializeField] private Button closeButton;
    
    

    private class LegendItem
    {
        public string Description;
        public string ColorName;
        public Color ItemColor;
        public Sprite[] DisplaySprites;
    }

    private List<LegendItem> legendItems = new List<LegendItem>();
    private int currentItemIndex = 0;

    void Start()
    {
        CheckNextAndPrevious();
        PopulateLegendItems();

        nextButton.onClick.AddListener(ShowNextItem);
        previousButton.onClick.AddListener(ShowPreviousItem);
        closeButton.onClick.AddListener(Close);

        UpdateLegendDisplay();
    }

    private void PopulateLegendItems()
    {
        foreach (var mapping in levelTheme.colorToTerrainTileMappings)
        {
            Sprite[] finalSprites = mapping.displaySprites;

            if (finalSprites == null || finalSprites.Length == 0)
            {
                if (mapping.tile is RuleTile ruleTile)
                {
                    List<Sprite> spritesFromRuleTile = new List<Sprite>();
                    foreach (var rule in ruleTile.m_TilingRules)
                    {
                        if (rule.m_Sprites != null && rule.m_Sprites.Length > 0)
                        {
                            spritesFromRuleTile.Add(rule.m_Sprites[0]);
                        }
                    }
                    finalSprites = spritesFromRuleTile.Distinct().ToArray();
                }
                else if (mapping.tile is Tile tileAsset)
                {
                    finalSprites = new Sprite[] { tileAsset.sprite };
                }
            }

            legendItems.Add(new LegendItem { 
                Description = mapping.description, 
                ColorName = mapping.colorName,
                ItemColor = mapping.color, 
                DisplaySprites = finalSprites 
            });
        }

        foreach (var mapping in levelTheme.colorToPrefabMappings)
        {
            legendItems.Add(new LegendItem { 
                Description = mapping.description, 
                ColorName = mapping.colorName,
                ItemColor = mapping.color, 
                DisplaySprites = mapping.displaySprites
            });
        }
    }

    private void UpdateLegendDisplay()
    {
        LegendItem currentItem = legendItems[currentItemIndex];

        string rgbColor = $"RGB({(byte)(currentItem.ItemColor.r * 255)}, {(byte)(currentItem.ItemColor.g * 255)}, {(byte)(currentItem.ItemColor.b * 255)})";
        descriptionText.text = $"{currentItem.Description}\n<color=#{ColorUtility.ToHtmlStringRGB(currentItem.ItemColor)}>({currentItem.ColorName})</color>\n{rgbColor}";
        
        if (currentItem.DisplaySprites == null || currentItem.DisplaySprites.Length == 0)
        {
            foreach (var img in displayImages)
            {
                img.enabled = true;
                img.sprite = null;
                img.color = currentItem.ItemColor;
            }
            return;
        }

        for (int i = 0; i < displayImages.Length; i++)
        {
            Image img = displayImages[i];
            
            if (i < currentItem.DisplaySprites.Length)
            {
                img.enabled = true;
                img.sprite = currentItem.DisplaySprites[i];
                img.color = Color.white;
            }
            else
            {
                img.enabled = false;
            }
        }
    }

    public void ShowNextItem(){
        currentItemIndex++;
        UpdateLegendDisplay();
        CheckNextAndPrevious();
    }
    public void ShowPreviousItem(){
        currentItemIndex--;
        UpdateLegendDisplay();
        CheckNextAndPrevious();
    }
    public void CheckNextAndPrevious(){
        if(currentItemIndex == 0) previousObject.SetActive(false);
        else previousObject.SetActive(true);
        if(currentItemIndex == legendItems.Count-1) nextObject.SetActive(false);
        else nextObject.SetActive(true);
    }

    public void Close(){
        legendObject.SetActive(false);
    }
}
