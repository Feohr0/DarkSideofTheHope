using UnityEngine;

/// <summary>
/// Halka objesine yumuşak nabız (pulse) animasyonu verir.
/// </summary>
public class RingPulse : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float minScale = 0.90f;
    [SerializeField] private float maxScale = 1.10f;

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float s = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = new Vector3(s, s, 1f);
    }
}
