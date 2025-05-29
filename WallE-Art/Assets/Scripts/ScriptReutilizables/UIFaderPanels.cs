using UnityEngine;
using System.Collections; 
using UnityEngine.UI;   

public class UIFader : MonoBehaviour
{
    [SerializeField] private GameObject objectCanvasGroup;
    [SerializeField] private CanvasGroup targetCanvasGroup;
    [SerializeField] private float defaultFadeDuration = 1.0f; 

    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(0f, 1f, defaultFadeDuration));
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeCoroutine(0f, 1f, duration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(1f, 0f, defaultFadeDuration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeCoroutine(1f, 0f, duration));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration)
    {
        StopAllCoroutines();
        objectCanvasGroup.SetActive(true);
        float timer = 0f;
        targetCanvasGroup.alpha = startAlpha; 

        if (endAlpha > startAlpha)
        {
            targetCanvasGroup.blocksRaycasts = true;
            targetCanvasGroup.interactable = true;
        }

        while (timer < duration)
        {
            targetCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        targetCanvasGroup.alpha = endAlpha;

        if (endAlpha == 0f)
        {
            targetCanvasGroup.blocksRaycasts = false;
            targetCanvasGroup.interactable = false;
            objectCanvasGroup.SetActive(false);
        }
    }

    public void SetAlpha(float alpha)
    {
        targetCanvasGroup.alpha = alpha;
        targetCanvasGroup.blocksRaycasts = (alpha > 0f);
        targetCanvasGroup.interactable = (alpha > 0f);  
    }
}