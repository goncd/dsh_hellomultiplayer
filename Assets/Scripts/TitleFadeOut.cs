using System.Collections;
using UnityEngine;

public class TitleFadeOut : MonoBehaviour
{
    float waitTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waitTime = Time.time + 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > waitTime)
        {
            StartCoroutine(FadeOutCanvas(5f));
        }
    }

    IEnumerator FadeOutCanvas(float t)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        while (canvasGroup.alpha > 0.0f)
        {
            canvasGroup.alpha -= Time.deltaTime / t;
            yield return null;
        }
    }
}
