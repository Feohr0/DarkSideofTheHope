// Scripts/Map/MapLineUI.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// İki node arasındaki bağlantı çizgisi.
/// UI Image'ı döndürüp uzatarak çizgi elde eder.
/// </summary>
public class MapLineUI : MonoBehaviour
{
    [SerializeField] private RectTransform lineRect;
    [SerializeField] private Image         lineImage;

    [SerializeField] private Color defaultColor   = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color completedColor = new Color(1f, 1f, 1f, 0.8f);

    public void Setup(Vector2 from, Vector2 to)
    {
        if (lineRect == null)
            lineRect = GetComponent<RectTransform>();

        Vector2 direction = to - from;
        float   distance  = direction.magnitude;
        float   angle     = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        lineRect.anchoredPosition = from + direction * 0.5f;
        lineRect.sizeDelta        = new Vector2(distance, 3f);
        lineRect.localRotation    = Quaternion.Euler(0f, 0f, angle);

        if (lineImage != null)
            lineImage.color = defaultColor;
    }

    public void SetCompleted(bool completed)
    {
        if (lineImage != null)
            lineImage.color = completed ? completedColor : defaultColor;
    }
}