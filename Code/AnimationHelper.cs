using System;
using System.Collections;
using UnityEngine;

public static class AnimationHelper {
    public static IEnumerator AnimateLocalPosition(float duration, Transform transform, Vector3 startPos,
        Vector3 endPos, AnimationCurve curve = null) {
        var moveVector = endPos - startPos;
        yield return Animate(duration, (t) => { transform.localPosition = startPos + t * moveVector; }, curve);
    }

    public static IEnumerator AnimateLocalScale(float duration, Transform transform, Vector3 startScale,
        Vector3 endScale, AnimationCurve curve = null) {
        var scaleVector = endScale - startScale;
        yield return Animate(duration, (t) => { transform.localScale = startScale + t * scaleVector; }, curve);
    }
    
    public static IEnumerator AnimateLocalEulerAngles(float duration, Transform transform, Vector3 startAngle,
        Vector3 endAngle, AnimationCurve curve = null) {
        var angleVector = endAngle - startAngle;
        yield return Animate(duration, (t) => { transform.localEulerAngles = startAngle + t * angleVector; }, curve);
    }
    
    public static IEnumerator ShakeLocalPosition(float duration, Transform transform, Vector3 startPos, AnimationCurve curve = null,
        float amount = 0.2f, float tremor = 15, float phase = 0) {
        
        var p = Vector3.zero;
        yield return Animate(duration, (t) => {
            p.x = Mathf.Sin(t * tremor + phase) * t * amount;
            p.y = Mathf.Cos(t * tremor * 0.6f + phase) * t * amount;
            transform.localPosition = startPos + p;
        }, curve);
        yield return AnimateLocalPosition(0.05f, transform, transform.localPosition,startPos);
    }

    public static IEnumerator AnimateParallel(MonoBehaviour monoBehaviour, params IEnumerator[] enumerators) {
        int runCount = 0;
        foreach (var enumerator in enumerators) {
            runCount += 1;
            monoBehaviour.StartCoroutine(RunWithCallBack(enumerator, ()=> runCount -= 1));
        }

        while (runCount>0) {
            yield return null;
        }
    }

    public static IEnumerator RunWithCallBack(IEnumerator enumerator, Action whenDone) {
        yield return enumerator;
        whenDone();
    }

    public static IEnumerator Animate(float duration, Action<float> lerpFunction, AnimationCurve curve = null, 
        float tInitialValue = 0, int tDirection = 1) {
        if (curve == null) {
            curve = AnimationCurve.Linear(0, 0, 1, 1);
        }
        
        float t = tInitialValue;
        while (t >= 0 && t <= 1) {
            t += (Time.deltaTime / duration)*tDirection;
           
            lerpFunction(curve.Evaluate(Mathf.Clamp01(t)));

            yield return null;
        }

        if (t < 0) {
            lerpFunction(curve.Evaluate(Mathf.Clamp01(0)));
        }
        else {
            lerpFunction(curve.Evaluate(Mathf.Clamp01(1)));
        }
    }
}
