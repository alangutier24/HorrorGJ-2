using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlickerLight2D : MonoBehaviour
{
    public enum FlickerMode { Smooth, Faulty, Fluorescent }

    [SerializeField] private FlickerMode mode = FlickerMode.Faulty;

    [Header("Intensidad")]
    [Tooltip("Fracción de la intensidad base cuando la luz está 'apagada' (0 = negro total).")]
    [SerializeField, Range(0f, 1f)] private float offIntensityFactor = 0f;

    [Header("Smooth (vela / fuego)")]
    [SerializeField, Range(0f, 1f)] private float smoothAmplitude = 0.3f;
    [SerializeField] private float smoothSpeed = 3f;

    [Header("Faulty (bombilla fallando)")]
    [SerializeField] private Vector2 timeOnRange = new Vector2(0.5f, 3f);
    [SerializeField] private Vector2Int flickerCountRange = new Vector2Int(1, 4);
    [SerializeField] private Vector2 flickerDurationRange = new Vector2(0.03f, 0.12f);
    [SerializeField, Range(0f, 1f)] private float blackoutChance = 0.15f;
    [SerializeField] private Vector2 blackoutDurationRange = new Vector2(0.5f, 2f);

    [Header("Fluorescent (ráfagas + zumbido)")]
    [SerializeField] private Vector2 burstIntervalRange = new Vector2(1f, 5f);
    [SerializeField] private Vector2Int burstFlashesRange = new Vector2Int(3, 8);
    [SerializeField] private Vector2 flashDurationRange = new Vector2(0.02f, 0.06f);
    [SerializeField, Range(0f, 1f)] private float buzzAmplitude = 0.05f;
    [SerializeField] private float buzzSpeed = 40f;

    [Header("Eventos")]
    public UnityEvent onLightOff;
    public UnityEvent onLightOn;

    /// <summary>True si la luz está encendida (útil para lógica de daño).</summary>
    public bool IsOn { get; private set; } = true;

    private Light2D light2D;
    private float baseIntensity;
    private float noiseSeed;
    private Coroutine routine;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        baseIntensity = light2D.intensity;
        noiseSeed = Random.value * 100f; // evita que todas las luces vayan sincronizadas
    }

    private void OnEnable() => StartRoutine();

    private void OnDisable()
    {
        StopRoutine();
        IsOn = true;
        light2D.intensity = baseIntensity;
    }

    private void Update()
    {
        if (!IsOn)
        {
            light2D.intensity = baseIntensity * offIntensityFactor;
            return;
        }

        float mod = 1f;
        switch (mode)
        {
            case FlickerMode.Smooth:
                mod = 1f - smoothAmplitude * Mathf.PerlinNoise(noiseSeed, Time.time * smoothSpeed);
                break;
            case FlickerMode.Fluorescent:
                mod = 1f - buzzAmplitude * Mathf.PerlinNoise(noiseSeed, Time.time * buzzSpeed);
                break;
        }
        light2D.intensity = baseIntensity * mod;
    }

    public void SetMode(FlickerMode newMode)
    {
        mode = newMode;
        if (isActiveAndEnabled) StartRoutine();
    }

    /// <summary>Cambia la intensidad base en runtime (p. ej. si otro sistema la modifica).</summary>
    public void SetBaseIntensity(float value) => baseIntensity = value;

    private void StartRoutine()
    {
        StopRoutine();
        SetOn(true);
        switch (mode)
        {
            case FlickerMode.Faulty: routine = StartCoroutine(FaultyRoutine()); break;
            case FlickerMode.Fluorescent: routine = StartCoroutine(FluorescentRoutine()); break;
        }
    }

    private void StopRoutine()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator FaultyRoutine()
    {
        while (true)
        {
            SetOn(true);
            yield return Wait(timeOnRange);

            if (Random.value < blackoutChance)
            {
                SetOn(false);
                yield return Wait(blackoutDurationRange);
                continue;
            }

            int count = Random.Range(flickerCountRange.x, flickerCountRange.y + 1);
            for (int i = 0; i < count; i++)
            {
                SetOn(false);
                yield return Wait(flickerDurationRange);
                SetOn(true);
                yield return Wait(flickerDurationRange);
            }
        }
    }

    private IEnumerator FluorescentRoutine()
    {
        while (true)
        {
            SetOn(true);
            yield return Wait(burstIntervalRange);

            int flashes = Random.Range(burstFlashesRange.x, burstFlashesRange.y + 1);
            for (int i = 0; i < flashes; i++)
            {
                SetOn(false);
                yield return Wait(flashDurationRange);
                SetOn(true);
                yield return Wait(flashDurationRange);
            }
        }
    }

    private void SetOn(bool on)
    {
        if (IsOn == on) return;
        IsOn = on;
        if (on) onLightOn?.Invoke();
        else onLightOff?.Invoke();
    }

    private static WaitForSeconds Wait(Vector2 range) =>
        new WaitForSeconds(Random.Range(range.x, range.y));
}
