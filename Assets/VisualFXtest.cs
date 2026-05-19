using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class VisualFXtest : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.32f;
    [SerializeField] private int flashCount = 1;

    private SpriteRenderer sr;
    private Material flashMat;
    private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        EnsureInitialized();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerFlash();
        }
    }

    public void TriggerFlash()
    {
        EnsureInitialized();
        if (flashMat == null) return;

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private void EnsureInitialized()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        if (flashMat != null || sr == null)
        {
            return;
        }

        Shader flashShader = Shader.Find("Custom/FlashWhite");
        if (flashShader == null)
        {
            Debug.LogWarning("Custom/FlashWhite shader bulunamadı, damage flash çalışmayacak.", this);
            return;
        }

        // Instance oluştur; sahnedeki diğer sprite'ları etkilemesin.
        flashMat = new Material(flashShader);
        sr.material = flashMat;
    }

    private IEnumerator FlashRoutine()
    {
        float step = flashDuration / flashCount;

        for (int i = 0; i < flashCount; i++)
        {
            flashMat.SetFloat(FlashAmountId, 1f);
            yield return new WaitForSeconds(step * 0.5f);
            flashMat.SetFloat(FlashAmountId, 0f);
            yield return new WaitForSeconds(step * 0.5f);
        }
    }
}
