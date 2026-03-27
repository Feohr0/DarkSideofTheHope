// Scripts/UI/CameraController.cs
using UnityEngine;

/// <summary>
/// 3D first-person perspektif kamera.
/// Savaş sırasında hafif sway ve kart hover'da zoom.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("POV Settings")]
    [SerializeField] private float swayAmount    = 0.02f;
    [SerializeField] private float swaySpeed     = 1.5f;
    [SerializeField] private float smoothing     = 5f;

    [Header("Combat Zoom")]
    [SerializeField] private float defaultFOV    = 60f;
    [SerializeField] private float targetFOV     = 60f;
    [SerializeField] private float fovSpeed      = 4f;

    private Vector3 _initialPosition;
    private Camera  _cam;
    private float   _swayTimer;

    private void Awake()
    {
        _cam             = GetComponent<Camera>();
        _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        ApplyIdleSway();
        UpdateFOV();
    }

    private void ApplyIdleSway()
    {
        _swayTimer += Time.deltaTime * swaySpeed;

        Vector3 sway = new Vector3(
            Mathf.Sin(_swayTimer * 0.7f) * swayAmount,
            Mathf.Sin(_swayTimer) * swayAmount * 0.5f,
            0f);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            _initialPosition + sway,
            Time.deltaTime * smoothing);
    }

    private void UpdateFOV()
    {
        _cam.fieldOfView = Mathf.Lerp(
            _cam.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }

    public void SetCombatFocus(bool focused)
    {
        targetFOV = focused ? defaultFOV - 5f : defaultFOV;
    }
}