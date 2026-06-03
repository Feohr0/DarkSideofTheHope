using UnityEngine;
using System.Collections;

/// <summary>
/// Herhangi bir RectTransform'a (can barı, panel vb.) veya normal Transform'a (karakter sprite)
/// hasar alındığında sarsılma efekti uygular.
/// Inspector'dan ayarlanabilir şiddet, süre ve frekans değerleri ile çalışır.
/// </summary>
public class UIShake : MonoBehaviour
{
    [Header("Sarsılma Ayarları")]
    [Tooltip("Sarsılma süresi (saniye)")]
    [SerializeField] private float shakeDuration = 0.35f;

    [Tooltip("Maksimum sarsılma mesafesi (piksel / birim)")]
    [SerializeField] private float shakeIntensity = 1;

    [Tooltip("Sarsılma sırasında saniyedeki titreşim sayısı")]
    [SerializeField] private float shakeFrequency = 10f;

    [Tooltip("true ise şiddet süre boyunca azalır (daha doğal görünüm)")]
    [SerializeField] private bool decayOverTime = true;

    private Vector3 originalLocalPosition;
    private Coroutine activeShake;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Sarsılma efektini tetikler. Zaten oynayan bir sarsılma varsa
    /// durdurup baştan başlatır.
    /// </summary>
    public void Shake()
    {
        if (activeShake != null)
        {
            StopCoroutine(activeShake);
            transform.localPosition = originalLocalPosition;
        }

        activeShake = StartCoroutine(ShakeRoutine(shakeIntensity));
    }

    /// <summary>
    /// Hasar miktarına göre orantılı sarsılma (isteğe bağlı).
    /// normalizedDamage: 0..1 arası (hasar / maxHP gibi).
    /// </summary>
    public void Shake(float normalizedDamage)
    {
        float intensity = Mathf.Lerp(shakeIntensity * 0.3f, shakeIntensity, Mathf.Clamp01(normalizedDamage));

        if (activeShake != null)
        {
            StopCoroutine(activeShake);
            transform.localPosition = originalLocalPosition;
        }

        activeShake = StartCoroutine(ShakeRoutine(intensity));
    }

    private IEnumerator ShakeRoutine(float intensity)
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float t = elapsed / shakeDuration;
            float currentIntensity = decayOverTime ? intensity * (1f - t) : intensity;

            // Perlin-tabanlı yumuşak sallanma
            float offsetX = (Mathf.PerlinNoise(elapsed * shakeFrequency, 0f) - 0.5f) * 2f * currentIntensity;
            float offsetY = (Mathf.PerlinNoise(0f, elapsed * shakeFrequency) - 0.5f) * 2f * currentIntensity;

            transform.localPosition = originalLocalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Orijinal konuma geri dön
        transform.localPosition = originalLocalPosition;
        activeShake = null;
    }
}
