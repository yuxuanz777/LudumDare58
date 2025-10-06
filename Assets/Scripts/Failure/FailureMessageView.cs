using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class FailureMessageView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text label;

    [Header("Animation (Enter)")]
    [SerializeField, Min(0.05f)] float fadeDuration = 0.4f;
    [SerializeField] AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] bool pulse = true;
    [SerializeField, Range(1f, 2f)] float pulseScale = 1.08f;
    [SerializeField, Min(0.05f)] float pulseDuration = 0.25f;
    [SerializeField, Min(0.05f)] float pulseRecoverDuration = 0.18f;

    [Header("Colors")]
    [SerializeField] Color targetColor = new(1f, 0.35f, 0f, 1f);
    [SerializeField] bool keepTargetColor = true;

    [Header("Behavior")]
    [SerializeField] bool autoDisableWhenEmpty = false;

    [Header("Loop Blink")]
    [Tooltip("Enable continuous blink after the enter animation finishes.")]
    [SerializeField] bool loopBlink = true;
    [SerializeField, Min(0.1f)] float blinkCycleDuration = 1.2f; // full up+down cycle
    [SerializeField, Range(0f, 1f)] float blinkMinAlpha = 0.35f;
    [SerializeField, Range(0f, 1f)] float blinkMaxAlpha = 1f;
    [SerializeField] AnimationCurve blinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] bool blinkUseUnscaledTime = true;

    [Header("Optional Config Override")]
    [SerializeField] FailureMessageConfig configOverride;

    Color _baseColor;
    Vector3 _baseScale;
    Coroutine _enterRoutine;
    Coroutine _blinkRoutine;
    bool _initialized;
    bool _isShowing;

    void Awake()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();
        CacheBaseState();
        ApplyConfigIfAny();
        ClearImmediate();
    }

    void ApplyConfigIfAny()
    {
        if (configOverride == null) return;
        fadeDuration = configOverride.fadeDuration;
        fadeCurve = configOverride.fadeCurve != null ? configOverride.fadeCurve : fadeCurve;
        pulse = configOverride.pulse;
        pulseScale = configOverride.pulseScale;
        pulseDuration = configOverride.pulseDuration;
        pulseRecoverDuration = configOverride.pulseRecoverDuration;
        targetColor = configOverride.targetColor;
        keepTargetColor = configOverride.keepTargetColor;
    }

    void CacheBaseState()
    {
        if (label != null)
        {
            _baseColor = label.color;
            _baseScale = label.transform.localScale;
            _initialized = true;
        }
    }

    public void Show(string message)
    {
        if (label == null) return;
        if (!_initialized) CacheBaseState();

        _isShowing = true;

        // Stop any running animations
        if (_enterRoutine != null) StopCoroutine(_enterRoutine);
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);

        label.gameObject.SetActive(true);
        label.text = message;

        _enterRoutine = StartCoroutine(AnimateIn());
    }

    public void Clear()
    {
        _isShowing = false;

        if (label == null) return;
        if (_enterRoutine != null) StopCoroutine(_enterRoutine);
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        _enterRoutine = null;
        _blinkRoutine = null;

        label.text = "";
        if (_initialized)
        {
            label.color = _baseColor;
            label.transform.localScale = _baseScale;
        }

        if (autoDisableWhenEmpty)
            label.gameObject.SetActive(false);
    }

    public void ClearImmediate() => Clear();

    IEnumerator AnimateIn()
    {
        // 初始状态
        Color start = targetColor;
        start.a = 0f;
        label.color = start;
        label.transform.localScale = _baseScale;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float n = Mathf.Clamp01(t / fadeDuration);
            float eased = fadeCurve != null ? fadeCurve.Evaluate(n) : n;

            // 颜色淡入
            Color c = Color.Lerp(start, targetColor, eased);
            label.color = c;

            // 脉冲放大（基于前半段时间）
            if (pulse)
            {
                float pulseN = Mathf.Clamp01(t / Mathf.Max(0.0001f, pulseDuration));
                float pulseEase = 1f - Mathf.Pow(1f - pulseN, 3f);
                float scale = Mathf.Lerp(1f, pulseScale, pulseEase);
                label.transform.localScale = _baseScale * scale;
            }

            yield return null;
        }

        // 回落缩放
        if (pulse)
        {
            float r = 0f;
            Vector3 startScale = label.transform.localScale;
            while (r < pulseRecoverDuration)
            {
                r += Time.unscaledDeltaTime;
                float n = Mathf.Clamp01(r / pulseRecoverDuration);
                label.transform.localScale = Vector3.Lerp(startScale, _baseScale, Mathf.SmoothStep(0, 1, n));
                yield return null;
            }
            label.transform.localScale = _baseScale;
        }

        if (!keepTargetColor)
            label.color = _baseColor;
        else
        {
            // ensure final alpha = 1
            var final = label.color;
            final.a = 1f;
            label.color = final;
        }

        _enterRoutine = null;

        if (loopBlink && _isShowing)
            _blinkRoutine = StartCoroutine(BlinkLoop());
    }

    IEnumerator BlinkLoop()
    {
        // Base color used for blinking alpha
        Color baseCol = label.color;
        // Guarantee proper min/max ordering
        float minA = Mathf.Min(blinkMinAlpha, blinkMaxAlpha);
        float maxA = Mathf.Max(blinkMinAlpha, blinkMaxAlpha);
        float period = Mathf.Max(0.1f, blinkCycleDuration);

        float time = 0f;
        while (_isShowing)
        {
            time += (blinkUseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);

            // Normalized (0..1) over cycle, ping-pong pattern
            float raw = (time % period) / period; // 0..1 linear
            // Convert to up & down (0..1..0)
            float pingPong = raw < 0.5f ? (raw * 2f) : (1f - (raw - 0.5f) * 2f); // 0->1->0
            float eased = blinkCurve != null ? blinkCurve.Evaluate(pingPong) : pingPong;

            float a = Mathf.Lerp(minA, maxA, eased);
            var c = baseCol;
            c.a = a;
            label.color = c;

            yield return null;
        }
        _blinkRoutine = null;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();
        if (label != null && !_initialized)
        {
            _baseColor = label.color;
            _baseScale = label.transform.localScale;
        }
        if (pulseScale < 1f) pulseScale = 1f;
        if (blinkMinAlpha > blinkMaxAlpha)
        {
            // auto swap to keep intuitive inspector usage
            float tmp = blinkMinAlpha;
            blinkMinAlpha = blinkMaxAlpha;
            blinkMaxAlpha = tmp;
        }
    }
#endif
}