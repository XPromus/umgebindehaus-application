using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIFader : MonoBehaviour
{

    [SerializeField]
    private float FadeTime = 0.5f;

    [SerializeField]
    private CanvasGroup uiElement;

    public UnityEvent StartBlendInAction;
    public UnityEvent StartBlendOutAction;
    public UnityEvent BlendInEndAction;
    public UnityEvent BlendOutEndAction;

    public void FadeIn()
    {
        StartBlendInAction.Invoke();
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 1, FadeTime));
    }

    public void FadeOut()
    {
        StartBlendOutAction.Invoke();
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0, FadeTime));
    }

    public void ReseIin()
    {
        uiElement.alpha = 1f;
        uiElement.interactable = true;
        uiElement.blocksRaycasts = true;
    }

    public void ResetOut()
    {
        uiElement.alpha = 0f;
        uiElement.interactable = false;
        uiElement.blocksRaycasts = false;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForFixedUpdate();
        }
        if (cg.alpha == 0f)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
            BlendOutEndAction.Invoke();

        }

        if (cg.alpha == 1f)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
            BlendInEndAction.Invoke();
        }

        //print("done");
    }
}
