using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlashController : MonoBehaviour
{
    private static int FlashId = Shader.PropertyToID("_Flash");
    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        mpb = new MaterialPropertyBlock();
        SetFlashValue(0f);
    }

    // immediately set _Flash for all renderers on this object
    public void SetFlashValue(float v)
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            var sr = renderers[i];
            if (sr == null) continue;
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat(FlashId, v);
            sr.SetPropertyBlock(mpb);
        }
    }

    // convenience coroutine to flash: instant -> hold -> fade back
    public Coroutine FlashOnce(float hold = 0.05f, float fade = 0.12f)
    {
        return StartCoroutine(FlashRoutine(hold, fade));
    }

    private IEnumerator FlashRoutine(float hold, float fade)
    {
        SetFlashValue(1f);
        if (hold > 0f) yield return new WaitForSeconds(hold);

        float elapsed = 0f;
        float duration = Mathf.Max(0.0001f, fade);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetFlashValue(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        SetFlashValue(0f);
    }
}