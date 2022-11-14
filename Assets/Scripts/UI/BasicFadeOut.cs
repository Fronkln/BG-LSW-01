using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicFadeOut : MonoBehaviour
{
    public float FadeTime;


    private void Start()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        yield return new WaitForEndOfFrame();

        Image img = GetComponent<Image>();
        float runTime = 0;

        float startAlpha = img.canvasRenderer.GetAlpha();

        while (runTime < FadeTime)
        {

            runTime += Time.deltaTime;
            img.canvasRenderer.SetAlpha(Mathf.Lerp(startAlpha, 0, runTime / FadeTime));

            yield return null;
        }

        Destroy(gameObject);
    }

}
