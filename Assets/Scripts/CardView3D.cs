// Scripts/UI/CardView3D.cs
using UnityEngine;

/// <summary>
/// Tek bir kartın 3D sahnedeki görünümü.
/// Smooth hareket, hover ve tıklama yönetir.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class CardView3D : MonoBehaviour
{
    public CardData Data { get; private set; }
    public Vector3    TargetPosition { get; private set; }
    public Quaternion TargetRotation { get; private set; }

    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotSpeed  = 12f;
    [SerializeField] private LayerMask enemyLayer;

    private CardHandUI  _handUI;
    private int         _sortOrder;
    private bool        _isDragging;
    private Vector3     _dragOffset;
    private Camera      _cam;

    // UI referansları (TextMeshPro vb.)
    [Header("Card Face")]
    [SerializeField] private TMPro.TextMeshPro nameText;
    [SerializeField] private TMPro.TextMeshPro descText;
    [SerializeField] private TMPro.TextMeshPro costText;
    [SerializeField] private UnityEngine.UI.RawImage artworkImage;

    private void Awake()
    {
        _cam = Camera.main;
    }

    public void Initialize(CardData data, CardHandUI handUI)
    {
        Data    = data;
        _handUI = handUI;

        nameText.text = data.CardName;
        descText.text = data.GetFormattedDescription();
        costText.text = data.EnergyCost.ToString();

        if (data.Artwork != null)
            artworkImage.texture = data.Artwork.texture;
    }

    public void SetTargetTransform(Vector3 pos, Quaternion rot, int order)
    {
        TargetPosition = pos;
        TargetRotation = rot;
        _sortOrder     = order;
    }

    private void Update()
    {
        if (_isDragging) return;

        transform.position = Vector3.Lerp(
            transform.position, TargetPosition, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, TargetRotation, Time.deltaTime * rotSpeed);
    }

    // --- Mouse Events ---
    private void OnMouseEnter() => _handUI.OnCardHoverEnter(this);
    private void OnMouseExit()  => _handUI.OnCardHoverExit(this);

    private void OnMouseDown()
    {
        _isDragging = true;
        _handUI.ArrangeCards(ignoredCard: this);
    }

    private void OnMouseDrag()
    {
        // Kartı mouse'a doğru taşı (düzlemde)
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(_cam.transform.forward, transform.position);

        if (plane.Raycast(ray, out float dist))
            transform.position = ray.GetPoint(dist);
    }

    private void OnMouseUp()
    {
        _isDragging = false;

        // Düşman hedef tespiti
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyLayer))
        {
            var enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && enemy.IsAlive)
            {
                _handUI.OnCardPlayed(this, enemy);
                return;
            }
        }

        // Hedef bulunamadı, geri döndür
        _handUI.ArrangeCards();
    }
}