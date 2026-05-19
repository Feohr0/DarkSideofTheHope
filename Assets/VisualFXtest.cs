using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class VisualFXtest : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.6f;
    [SerializeField] private int flashCount = 3;

    private SpriteRenderer sr;
    private Material flashMat;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Instance oluştur — diğer objeleri etkilemesin
        flashMat = new Material(Shader.Find("Custom/FlashWhite"));
        sr.material = flashMat;
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
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float step = flashDuration / flashCount;

        for (int i = 0; i < flashCount; i++)
        {
            flashMat.SetFloat("_FlashAmount", 1f);
            yield return new WaitForSeconds(step * 0.5f);
            flashMat.SetFloat("_FlashAmount", 0f);
            yield return new WaitForSeconds(step * 0.5f);
        }
    }
}
