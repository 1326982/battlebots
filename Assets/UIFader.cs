using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour {
    private CanvasGroup _element; //The canvas group controlling the element
    private bool _isFading = false;

    void Start() {
        _element = GetComponent<CanvasGroup>();
    }
    public void fade(FadeTransition fadeType,float seconds = 0.5f){
        if(_isFading){ StopAllCoroutines();}
        StartCoroutine(doFade(fadeType,seconds));
    }
    private IEnumerator doFade(FadeTransition fadeType,float seconds = 0.5f){
        float currentAlpha = _element.alpha;
        float dir = (fadeType== FadeTransition.In)?1:-1;
        float targetAlpha = (fadeType == FadeTransition.In)?1f:0f;
        float timeRemeaning = seconds;
        while(currentAlpha != targetAlpha){
            float timeDivision = timeRemeaning/Time.deltaTime;
            float fadeValue = 1/timeDivision;
            currentAlpha = Mathf.Clamp01(currentAlpha+(fadeValue*dir));
            _element.alpha = currentAlpha;
            timeRemeaning-= Time.deltaTime;
            yield return 0;
        }
        yield return null;
    }
}
public enum FadeTransition {
    Out,In
}
