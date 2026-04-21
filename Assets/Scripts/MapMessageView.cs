using System.Collections;
using TMPro;
using UnityEngine;

public class MapMessageView : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI messageText;

    [Header("Behavior")]
    public float defaultDurationSeconds = 2.5f;

    Coroutine running;

    public void Show(string message, float? durationSeconds = null)
    {
        if (messageText == null) return;

        messageText.text = message;
        messageText.gameObject.SetActive(true);

        if (running != null) StopCoroutine(running);
        float duration = durationSeconds ?? defaultDurationSeconds;
        if (duration > 0f)
            running = StartCoroutine(HideAfter(duration));
    }

    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (messageText != null) messageText.gameObject.SetActive(false);
        running = null;
    }
}

