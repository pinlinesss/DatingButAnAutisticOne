using UnityEngine;

public class BlueEnemy : MonoBehaviour
{
    public enum SpawnSide { Top, Bottom, Left, Right }

    [Header("Enemy State")]
    public SpawnSide currentSide;
    public int lineIndex; // 0, 1, 2, 3

    [Header("Distance & Movement")]
    public float offsetFromEdge = 1.5f;

    [Header("Grid References")]
    public float[] columnXPositions = new float[4];
    public float[] rowYPositions = new float[4];

    [Header("Visual Effects")]
    [Tooltip("Beat 2 ve 3'te düşmanın ağzında yanan şarj efekti")]
    public GameObject chargeEffect;

    [Tooltip("2. kare için sarkıt objesi (AttackThorn_2nd)")]
    public GameObject attackTile2;

    [Tooltip("4. kare için sarkıt objesi (AttackThorn_4th)")]
    public GameObject attackTile4;

    private void Start()
    {
        ResetEffects();
    }

    /// <summary>
    /// Beat 1: Düşmanı konumlandırır, sarkıtları tam karelere yerleştirir ve efektleri kapatır.
    /// </summary>
    public void SetupPosition(SpawnSide side, int line)
    {
        currentSide = side;
        lineIndex = Mathf.Clamp(line, 0, 3);
        SetPositionAndRotation();
        PositionTargetTiles(); // Sarkıtları karelerin tam üstüne oturttuğumuz yeni metot!
        ResetEffects();
    }

    private void SetPositionAndRotation()
    {
        float firstX = columnXPositions[0];
        float lastX = columnXPositions[3];
        float firstY = rowYPositions[0];
        float lastY = rowYPositions[3];

        switch (currentSide)
        {
            case SpawnSide.Top:
                transform.position = new Vector3(columnXPositions[lineIndex], lastY + offsetFromEdge, 0f);
                transform.rotation = Quaternion.Euler(0, 0, 0f);
                break;

            case SpawnSide.Bottom:
                transform.position = new Vector3(columnXPositions[lineIndex], firstY - offsetFromEdge, 0f);
                transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;

            case SpawnSide.Left:
                transform.position = new Vector3(firstX - offsetFromEdge, rowYPositions[lineIndex], 0f);
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;

            case SpawnSide.Right:
                transform.position = new Vector3(lastX + offsetFromEdge, rowYPositions[lineIndex], 0f);
                transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
        }
    }

    /// <summary>
    /// Sarkıt objelerini düşmanın dönmesinden etkilenmeyecek şekilde GRID KARELERİNE hizalar.
    /// </summary>
    private void PositionTargetTiles()
    {
        Vector3 pos2 = Vector3.zero;
        Vector3 pos4 = Vector3.zero;

        switch (currentSide)
        {
            case SpawnSide.Top:
                // Üstten 2. kare = Y dizini 2 | Üstten 4. kare = Y dizini 0
                pos2 = new Vector3(columnXPositions[lineIndex], rowYPositions[2], 0f);
                pos4 = new Vector3(columnXPositions[lineIndex], rowYPositions[0], 0f);
                break;

            case SpawnSide.Bottom:
                // Alttan 2. kare = Y dizini 1 | Alttan 4. kare = Y dizini 3
                pos2 = new Vector3(columnXPositions[lineIndex], rowYPositions[1], 0f);
                pos4 = new Vector3(columnXPositions[lineIndex], rowYPositions[3], 0f);
                break;

            case SpawnSide.Left:
                // Soldan 2. kare = X dizini 1 | Soldan 4. kare = X dizini 3
                pos2 = new Vector3(columnXPositions[1], rowYPositions[lineIndex], 0f);
                pos4 = new Vector3(columnXPositions[3], rowYPositions[lineIndex], 0f);
                break;

            case SpawnSide.Right:
                // Sağdan 2. kare = X dizini 2 | Sağdan 4. kare = X dizini 0
                pos2 = new Vector3(columnXPositions[2], rowYPositions[lineIndex], 0f);
                pos4 = new Vector3(columnXPositions[0], rowYPositions[lineIndex], 0f);
                break;
        }

        // Sarkıtların rotasyonunu sıfırlayıp tam hesaplanan X, Y noktasına koyuyoruz
        if (attackTile2 != null)
        {
            attackTile2.transform.position = pos2;
            attackTile2.transform.rotation = Quaternion.identity;
        }

        if (attackTile4 != null)
        {
            attackTile4.transform.position = pos4;
            attackTile4.transform.rotation = Quaternion.identity;
        }
    }

    public void ResetEffects()
    {
        if (chargeEffect != null) chargeEffect.SetActive(false);
        if (attackTile2 != null) attackTile2.SetActive(false);
        if (attackTile4 != null) attackTile4.SetActive(false);
    }

    /// <summary>
    /// Beat 2 ve 3: Düşmanın ağzında şarj ışığı yanar.
    /// </summary>
    public void OnBeat_Charge()
    {
        if (chargeEffect != null) chargeEffect.SetActive(true);
        if (attackTile2 != null) attackTile2.SetActive(false);
        if (attackTile4 != null) attackTile4.SetActive(false);
    }

    /// <summary>
    /// Beat 4: Her iki sarkıt da görünür olur ve vuruş kontrolü yapar.
    /// </summary>
    public bool OnBeat4_ExecuteAttack(Vector2Int playerGridPos)
    {
        if (chargeEffect != null) chargeEffect.SetActive(false);
        
        // Saldırmadan önce her ihtimale karşı pozisyonları tekrar tazeliyoruz
        PositionTargetTiles();

        if (attackTile2 != null) attackTile2.SetActive(true);
        if (attackTile4 != null) attackTile4.SetActive(true);

        int targetIndex2 = -1;
        int targetIndex4 = -1;

        switch (currentSide)
        {
            case SpawnSide.Top:
                targetIndex2 = 2; // Y: 2. kare
                targetIndex4 = 0; // Y: 4. kare
                return (playerGridPos.x == lineIndex) && (playerGridPos.y == targetIndex2 || playerGridPos.y == targetIndex4);

            case SpawnSide.Bottom:
                targetIndex2 = 1; // Y: 2. kare
                targetIndex4 = 3; // Y: 4. kare
                return (playerGridPos.x == lineIndex) && (playerGridPos.y == targetIndex2 || playerGridPos.y == targetIndex4);

            case SpawnSide.Left:
                targetIndex2 = 1; // X: 2. kare
                targetIndex4 = 3; // X: 4. kare
                return (playerGridPos.y == lineIndex) && (playerGridPos.x == targetIndex2 || playerGridPos.x == targetIndex4);

            case SpawnSide.Right:
                targetIndex2 = 2; // X: 2. kare
                targetIndex4 = 0; // X: 4. kare
                return (playerGridPos.y == lineIndex) && (playerGridPos.x == targetIndex2 || playerGridPos.x == targetIndex4);
        }

        return false;
    }
}