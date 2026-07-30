using UnityEngine;

public class RedEnemy : MonoBehaviour
{
    public enum SpawnSide { Top, Bottom, Left, Right }

    [Header("Enemy State")]
    public SpawnSide currentSide;
    public int lineIndex; // 0, 1, 2, 3

    [Header("Distance & Movement")]
    public float offsetFromEdge = 1.5f;

    [Header("Grid References")]
    public float[] columnXPositions = new float[4] { -2.5f, -0.5f, 1.5f, 3.5f };
    public float[] rowYPositions = new float[4] { -2.5f, -0.5f, 1.5f, 3.5f };

    [Header("Beam Effects (Visuals)")]
    public GameObject chargeBeamObject;
    public GameObject attackBeamObject;

    private void Start()
    {
        ResetBeams();
    }

    /// <summary>
    /// Beat 1: Düşmanı yeni konuma koyar ve ışınları kapatır.
    /// </summary>
    public void SetupPosition(SpawnSide side, int line)
    {
        ResetBeams();

        currentSide = side;
        lineIndex = Mathf.Clamp(line, 0, 3);
        SetPositionAndRotation();
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
                transform.rotation = Quaternion.Euler(0, 0, 90f); // İçe doğru bakar
                break;

            case SpawnSide.Right:
                // DÜZELTME 1: Çıkarma (-) değil Ekleme (+) yapıyoruz ki sağ kenarın DIŞINA çıksın.
                // DÜZELTME 2: Açıyı 270 veya -90 yerine 90 yapıp yüzünü sola (içe) çeviriyoruz.
                transform.position = new Vector3(lastX + offsetFromEdge, rowYPositions[lineIndex], 0f);
                transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
        }
    }

    /// <summary>
    /// Beat 2 ve 3: Şarj ışığını açar
    /// </summary>
    public void OnBeat_Charge()
    {
        if (chargeBeamObject != null) chargeBeamObject.SetActive(true);
        if (attackBeamObject != null) attackBeamObject.SetActive(false);
    }

    /// <summary>
    /// Beat 4: Saldırı ışınını açar ve vuruş kontrolü yapar
    /// </summary>
    public bool OnBeat4_ExecuteAttack(Vector2Int playerGridPos)
    {
        if (chargeBeamObject != null) chargeBeamObject.SetActive(false);
        if (attackBeamObject != null) attackBeamObject.SetActive(true);

        bool isHit = false;

        switch (currentSide)
        {
            case SpawnSide.Top:
            case SpawnSide.Bottom:
                if (playerGridPos.x == lineIndex) isHit = true;
                break;

            case SpawnSide.Left:
            case SpawnSide.Right:
                if (playerGridPos.y == lineIndex) isHit = true;
                break;
        }

        return isHit;
    }

    /// <summary>
    /// Tüm ışınları tamamen kapatır
    /// </summary>
    public void ResetBeams()
    {
        if (chargeBeamObject != null) chargeBeamObject.SetActive(false);
        if (attackBeamObject != null) attackBeamObject.SetActive(false);
    }
}