using UnityEngine;
using System.Collections;

public class ButtonEffect : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent onActionButtonPressed;
    [SerializeField] private CanvasGroup infoPanelCanvasGroup;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private GameObject gameObject;
    [SerializeField] private Animator animator;
    [SerializeField] private Coroutine fadeCoroutine = null;
    [SerializeField] private bool playerIsInside = false;
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    
        infoPanelCanvasGroup.alpha = 0f;
        infoPanelCanvasGroup.blocksRaycasts = false;
        infoPanelCanvasGroup.interactable = false;
    }

    void Update() 
    {
    
        if (playerIsInside && Input.GetKeyDown(KeyCode.Return))
        {
            onActionButtonPressed.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(!gameObject.activeSelf) gameObject.SetActive(true);
            animator.SetBool("Appear",true);
            playerIsInside=true;
            FadePanel(true); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Appear",false);
            playerIsInside=false;
            FadePanel(false);
        }
    }

    private void FadePanel(bool fadeIn)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        float targetAlpha = fadeIn ? 1f : 0f;
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(infoPanelCanvasGroup, targetAlpha, fadeSpeed));

        infoPanelCanvasGroup.blocksRaycasts = fadeIn;
        infoPanelCanvasGroup.interactable = fadeIn;
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float speed)
    {
        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, speed * Time.unscaledDeltaTime);
            yield return null;
        }
        cg.alpha = targetAlpha;
        fadeCoroutine = null; 
    }
}
