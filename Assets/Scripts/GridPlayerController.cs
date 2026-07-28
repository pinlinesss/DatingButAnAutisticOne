using UnityEngine;

public class GridPlayerController : MonoBehaviour
{
    [Header("Grid Layout Settings")]
    public float gridSize = 1.0f; // Karelerin merkezleri arasındaki mesafe
    public Vector2Int gridSizeDimensions = new Vector2Int(4, 4); // 4x4 Grid (16 kare)

    [Header("World Position Offset")]
    // Sol alt karenin dünya üzerindeki (-2.5, -2.5) pozisyonu buraya yazılır
    public Vector2 gridOriginOffset = new Vector2(-2.5f, -2.5f);

    [Header("Starting Grid Position")]
    // Grid üzerindeki indeksler: (0,0) sol alt kare, (3,3) sağ üst karedir
    public Vector2Int currentGridPos = new Vector2Int(0, 0);

    private void Start()
    {
        // Başlangıçta karakteri belirlenen grid indeksine anında ışınla
        UpdateWorldPosition();
    }

    private void Update()
    {
        Vector2Int input = Vector2Int.zero;

        // Tek tıkla hareket etmesi için GetKeyDown kullanıyoruz
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) input.y = 1;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) input.y = -1;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) input.x = -1;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) input.x = 1;

        if (input != Vector2Int.zero)
        {
            TryMove(input);
        }
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetPos = currentGridPos + direction;

        // 4x4 alanın sınırları içinde mi kontrolü (0, 1, 2, 3 indeksleri)
        if (targetPos.x >= 0 && targetPos.x < gridSizeDimensions.x &&
            targetPos.y >= 0 && targetPos.y < gridSizeDimensions.y)
        {
            currentGridPos = targetPos;
            UpdateWorldPosition();
        }
    }

    private void UpdateWorldPosition()
    {
        // Grid indeksini dünya koordinatına dönüştürürken offset ekliyoruz
        float targetX = (currentGridPos.x * gridSize) + gridOriginOffset.x;
        float targetY = (currentGridPos.y * gridSize) + gridOriginOffset.y;

        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}