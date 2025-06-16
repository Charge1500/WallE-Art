using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GuideUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [Header("Objeto del panel de la guia")]
    [SerializeField] protected GameObject guidePanel;
    [SerializeField] protected TMP_Text textContent;

    [Tooltip("Botón para mostrar el siguiente ítem en la guia.")]
    [SerializeField] protected GameObject nextObject;
    [SerializeField] protected Button nextButton;

    [Tooltip("Botón para mostrar el ítem anterior en la guia.")]
    [SerializeField] protected Button previousButton;
    [SerializeField] protected GameObject previousObject;

    [Tooltip("Botón para cerrar la guia.")]
    [SerializeField] protected Button closeButton;

    [Header("Guide Content")]

    [SerializeField] protected List<string> guidePages;
    [SerializeField] protected int currentItemIndex = 0;
    [SerializeField] protected int large = 0;

    void Awake(){
        nextButton.onClick.AddListener(ShowNextPage);
        previousButton.onClick.AddListener(ShowPreviousPage);
        closeButton.onClick.AddListener(Hide);
        currentItemIndex = 0;
    }
    void Start()
    {
        guidePages.Add("El cosmos, tan vasto y misterioso, a veces tiene un sentido del humor peculiar. Wall-E flotaba a la deriva, sumido en sus pensamientos ecológicos y soñando con su querida EVA. De repente, un destello verde esmeralda, una estela de polvo de estrellas y un inexplicablemente familiar sonido de \"¡Pling!\" lo envolvieron. Cuando el brillo se desvaneció, Wall-E no estaba en el frío vacío del espacio, sino sobre un vibrante bloque de interrogación flotante, con nubes pixeladas y un castillo reconocible a lo lejos.\nHabía aterrizado, sin querer, en el Reino Champiñón.");
        guidePages.Add("Su inocente recolección de \"basura\" (Goombas y Koopa Troopas rotos) no tardó en llamar la atención de un furioso Mario.\"¡Mamma mia! ¡Estás pisoteando mis niveles y usando mis cosas sin licencia!\", exclamó Mario. Wall-E, sin entender, solo podía decir \"¡Wall-E!\". Sin saberlo, había activado la alarma de Nintendo por infracción de derechos de autor. Ahora, este pacífico robot se encontraba en medio de una demanda interdimensional. Su única esperanza de salir de este lío y volver a casa era encontrar lo más preciado que traía consigo: su pequeña bota con la última planta viva.");
        guidePages.Add("Con abogados de setas afilando sus plumas, Wall-E debe navegar por los peligrosos niveles de Mario, evitando no solo a los enemigos sino también la factura de la demanda. Su misión: recuperar su bota antes de convertirse en una exhibición legal.\n¿Podrá Wall-E recuperar su bota y escapar de las garras legales del Reino Champiñón? ¿O terminará como una exhibición en el Museo de la Historia de Nintendo por \"uso no autorizado de activos\"?");
        guidePages.Add("Controles:\n\n SPACE:Saltar\n\n DOWN ARROW: agacharse\n\n LEFT ARROW: mover hacia la izquierda\n\n Right ARROW: mover hacia la derecha");
        guidePages.Add("Creado por:\n\nRaul Roberto Espinosa Poma\n\nAGRADECIMIENTOS:\nAilema Matos\nMeyli Jimenez\nAdrian Estevez\nJavier Fontes\nKarla Yisel\nAbner Abreu\nMiguel Cazorla");
        large = guidePages.Count-1;
        UpdateContent();
    }

    protected void Show()
    {
        guidePanel.SetActive(true);
        //currentItemIndex = 0;
        UpdateContent();
    }

    protected void Hide()
    {
        guidePanel.SetActive(false);
    }

    protected void ShowNextPage()
    {
        currentItemIndex++;
        UpdateContent();
        
    }

    protected void ShowPreviousPage()
    {
        currentItemIndex--;
        UpdateContent();   
    }

    protected void UpdateContent()
    {
        ShowContent();
        previousButton.gameObject.SetActive(currentItemIndex > 0);
        nextButton.gameObject.SetActive(currentItemIndex < large);
    }
    protected virtual void ShowContent(){
        textContent.text = guidePages[currentItemIndex];
    }
}
