using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; 
public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject guiaPanel;
    [SerializeField] private GameObject walle;

    [SerializeField] private GameObject nextObject;
    [SerializeField] private GameObject previousObject;
    [SerializeField] private Button comeBackGuia;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private TMP_Text guiaTMP;
    [SerializeField] private int guiaTextsIndex=0;
    [SerializeField] private List<string> guiaTexts;
    void Awake(){
        guiaTexts.Add("El cosmos, tan vasto y misterioso, a veces tiene un sentido del humor peculiar. Wall-E flotaba a la deriva, sumido en sus pensamientos ecológicos y soñando con su querida EVA. De repente, un destello verde esmeralda, una estela de polvo de estrellas y un inexplicablemente familiar sonido de \"¡Pling!\" lo envolvieron. Cuando el brillo se desvaneció, Wall-E no estaba en el frío vacío del espacio, sino sobre un vibrante bloque de interrogación flotante, con nubes pixeladas y un castillo reconocible a lo lejos.\nHabía aterrizado, sin querer, en el Reino Champiñón.");
        guiaTexts.Add("Su inocente recolección de \"basura\" (Goombas y Koopa Troopas rotos) no tardó en llamar la atención de un furioso Mario.\"¡Mamma mia! ¡Estás pisoteando mis niveles y usando mis cosas sin licencia!\", exclamó Mario. Wall-E, sin entender, solo podía decir \"¡Wall-E!\". Sin saberlo, había activado la alarma de Nintendo por infracción de derechos de autor. Ahora, este pacífico robot se encontraba en medio de una demanda interdimensional. Su única esperanza de salir de este lío y volver a casa era encontrar lo más preciado que traía consigo: su pequeña bota con la última planta viva.");
        guiaTexts.Add("Con abogados de setas afilando sus plumas, Wall-E debe navegar por los peligrosos niveles de Mario, evitando no solo a los enemigos sino también la factura de la demanda. Su misión: recuperar su bota antes de convertirse en una exhibición legal.\n¿Podrá Wall-E recuperar su bota y escapar de las garras legales del Reino Champiñón? ¿O terminará como una exhibición en el Museo de la Historia de Nintendo por \"uso no autorizado de activos\"?");
        guiaTexts.Add("Controles:\n\n SPACE:Saltar\n\n DOWN ARROW: agacharse\n\n LEFT ARROW: mover hacia la izquierda\n\n Right ARROW: mover hacia la derecha");
        guiaTexts.Add("Creado por:\n\nRaul Roberto Espinosa Poma\n\nAGRADECIMIENTOS:\nAilema Matos\nMeyli Jimenez\nAdrian Estevez\nJavier Fontes\nKarla Yisel\nAbner Abreu\nMiguel Cazorla");
    }
    void Start(){
        nextButton.onClick.AddListener(Next);
        previousButton.onClick.AddListener(Previous);
        comeBackGuia.onClick.AddListener(Regresar);
        guiaTMP.text = guiaTexts[guiaTextsIndex];
    }
    public void Salir(){
        Application.Quit();
    }

    public void Editor(){
        SceneManager.LoadScene("Editor");
    }

    public void Jugar(){
        SceneManager.LoadScene("Jugar");
    }
    public void Guia(){
        guiaPanel.SetActive(true);
        walle.GetComponent<Player>().dead=true;
        walle.GetComponent<Player>().WalleStop();
        CheckNextAndPrevious();
    }
    public void Regresar(){
        guiaPanel.SetActive(false);
        walle.GetComponent<Player>().dead=false;
    }
    public void Next(){
        guiaTextsIndex++;
        guiaTMP.text = guiaTexts[guiaTextsIndex];
        CheckNextAndPrevious();
    }
    public void Previous(){
        guiaTextsIndex--;
        guiaTMP.text = guiaTexts[guiaTextsIndex];
        CheckNextAndPrevious();
    }
    public void CheckNextAndPrevious(){
        if(guiaTextsIndex == 0) previousObject.SetActive(false);
        else previousObject.SetActive(true);
        if(guiaTextsIndex == guiaTexts.Count-1) nextObject.SetActive(false);
        else nextObject.SetActive(true);
    }
}
