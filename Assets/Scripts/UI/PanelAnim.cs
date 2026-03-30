using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;

    private bool isShowed = false;

    private void Awake()
    {
        this.transform.localScale = Vector3.one * showCurve.Evaluate(0);
    }

    IEnumerator ShowPanel()
    {
        float timer = 0;
        while (timer <= 1)
        {
            this.transform.localScale = Vector3.one*showCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

    IEnumerator HidePanel()
    {
        float timer = 0;
        while (timer <= 1)
        {
            this.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

    public void  Show()
    {
        Debug.Log("ÏÔÊ¾");
        if (!isShowed)
        {
            StartCoroutine(ShowPanel());
            isShowed = true;
            this.gameObject.SetActive(true);
        }
        
    }
    public void Hide()
    {
        if (isShowed)
        {
            StartCoroutine(HidePanel());
            isShowed = false;
            this.gameObject.SetActive(false);
        }
        
    }

    public void Hide_Dia()
    {
        if (!isShowed)
        {
            StartCoroutine(HidePanel());
            isShowed = false;
            this.gameObject.SetActive(false);
        }
    }
}