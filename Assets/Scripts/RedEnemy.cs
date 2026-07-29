using System.Collections;
using UnityEngine;

public class RedEnemy : MonoBehaviour
{
    public enum SpawnSide { Top, Bottom, Left, Right }

    [Header("Enemy State")]
    public SpawnSide currentSide;
    [Range(0, 3)] public int lineIndex; // 0, 1, 2 veya 3. hat/şerit

    [Header("Grid Coordinate Positions")]
    // 4 Sütunun X pozisyonları (En soldan en sağa)
    public float[] columnXPositions = new float[4] { -1.5f, -0.5f, 0.5f, 1.5f };
    // 4 Satırın Y pozisyonları (En alttan en üste)
    public float[] rowYPositions = new float[4] { -1.5f, -0.5f, 0.5f, 1.5f };

    [Header("Outer Boundary Offsets")]
    public float topY = 3.5f;    // Üstteki düşmanların duracağı Y
    public float bottomY = -3.5f; // Alttaki düşmanların duracağı Y
    public float leftX = -3.5f;  // Soldaki düşmanların duracağı X
    public float rightX = 3.5f;  // Sağdaki düşmanların duracağı X

    private bool[,] dangerGrid = new bool[4, 4];

    /// <summary>
    /// Beat 1: Düşmanın kenarını, hattını belirler, rotasyonunu çeker ve matrisi doldurur.
    /// </summary>
    public void InitializeEnemy(SpawnSide side, int index)
    {
        currentSide = side;
        lineIndex = Mathf.Clamp(index, 0, 3);

        CalculateDangerZone();
        SetPositionAndRotation();
    }

    [Header("Distance From Grid Edge")]
    [Tooltip("Grid karelerinden dışarı kaçma mesafesi. Nesne karelere biniyorsa bu değeri büyüt.")]
    public float offsetFromEdge = 2.2f;

    private void SetPositionAndRotation()
    {
        float firstX = columnXPositions[0]; // En sol sütun X
        float lastX = columnXPositions[3];  // En sağ sütun X
        float firstY = rowYPositions[0];    // En alt satır Y
        float lastY = rowYPositions[3];     // En üst satır Y

        switch (currentSide)
        {
            case SpawnSide.Top:
                // Üstte: Tam aşağı bakacak
                transform.position = new Vector3(columnXPositions[lineIndex], lastY + offsetFromEdge, 0f);
                transform.rotation = Quaternion.Euler(0, 0, 0f);
                break;

            case SpawnSide.Bottom:
                // Altta: Tam yukarı bakacak
                transform.position = new Vector3(columnXPositions[lineIndex], firstY - offsetFromEdge, 0f);
                transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;

            case SpawnSide.Left:
                // Solda: Sağa bakacak (90 derece)
                transform.position = new Vector3(firstX - offsetFromEdge, rowYPositions[lineIndex], 0f);
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;

            case SpawnSide.Right:
                // Sağda: Sola bakacak (-90 derece)
                transform.position = new Vector3(lastX + offsetFromEdge, rowYPositions[lineIndex], 0f);
                transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
        }
    }

    public void OnBeat2_EnemyShake()
    {
        Debug.Log($"[Kırmızı Düşman] Beat 2: Titriyor! (Kenar: {currentSide}, Hat: {lineIndex})");
    }

    public void OnBeat3_GroundShake()
    {
        Debug.Log("[Kırmızı Düşman] Beat 3: Yer sallanıyor!");
    }

    public bool OnBeat4_ExecuteAttack(Vector2Int playerGridPos)
    {
        Debug.Log("<color=red>[Kırmızı Düşman] BEAT 4: LAZER PATLADI!</color>");
        return dangerGrid[playerGridPos.x, playerGridPos.y];
    }

    private void CalculateDangerZone()
    {
        System.Array.Clear(dangerGrid, 0, dangerGrid.Length);

        for (int i = 0; i < 4; i++)
        {
            if (currentSide == SpawnSide.Top || currentSide == SpawnSide.Bottom)
            {
                // Dikey Saldırı: 'lineIndex' sütununun tamamı tehlikeli
                dangerGrid[lineIndex, i] = true;
            }
            else // Left veya Right
            {
                // Yatay Saldırı: 'lineIndex' satırının tamamı tehlikeli
                dangerGrid[i, lineIndex] = true;
            }
        }
    }
}