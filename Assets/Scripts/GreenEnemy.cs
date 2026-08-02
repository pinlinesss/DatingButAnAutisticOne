using UnityEngine;
using System.Collections.Generic;

public class GreenEnemy : MonoBehaviour
{
    public enum SpawnCorner { TopLeft, TopRight, BottomLeft, BottomRight }

    [Header("Enemy State")]
    public SpawnCorner currentCorner;

    [Header("Distance & Offset")]
    public float offsetFromCorner = 1.5f;

    [Header("Grid References")]
    public float[] columnXPositions = new float[4];
    public float[] rowYPositions = new float[4];

    [Header("Visual Effects")]
    public GameObject chargeEffect;
    public List<GameObject> acidTileObjects = new List<GameObject>();

    // 4x4 Matrix
    private readonly int[,] patternTopLeft = new int[4, 4] {
        { 1, 1, 1, 0 },
        { 1, 0, 1, 0 },
        { 1, 1, 1, 1 },
        { 0, 0, 1, 1 }
    };

    /// <summary>
    /// Beat 1: Köşeye yerleş, merkeze kilitlen, efektleri gizle
    /// </summary>
    public void SetupPosition(SpawnCorner corner)
    {
        currentCorner = corner;
        SetPositionAndRotation();
        ResetEffects(); // Şarjı ve asitleri kapatır
    }

    private void SetPositionAndRotation()
    {
        if (columnXPositions == null || columnXPositions.Length < 4 ||
            rowYPositions == null || rowYPositions.Length < 4) return;

        float firstX = columnXPositions[0];
        float lastX = columnXPositions[3];
        float firstY = rowYPositions[0];
        float lastY = rowYPositions[3];

        switch (currentCorner)
        {
            case SpawnCorner.TopLeft:
                transform.position = new Vector3(firstX - offsetFromCorner, lastY + offsetFromCorner, 0f);
                break;
            case SpawnCorner.TopRight:
                transform.position = new Vector3(lastX + offsetFromCorner, lastY + offsetFromCorner, 0f);
                break;
            case SpawnCorner.BottomLeft:
                transform.position = new Vector3(firstX - offsetFromCorner, firstY - offsetFromCorner, 0f);
                break;
            case SpawnCorner.BottomRight:
                transform.position = new Vector3(lastX + offsetFromCorner, firstY - offsetFromCorner, 0f);
                break;
        }

        // Haritanın merkezine kitlenme (Bakış Açısı Düzeltmesi)
        float centerX = (columnXPositions[0] + columnXPositions[3]) / 2f;
        float centerY = (rowYPositions[0] + rowYPositions[3]) / 2f;
        Vector3 centerPoint = new Vector3(centerX, centerY, 0f);

        Vector3 direction = centerPoint - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    private void PositionAcidTiles()
    {
        int[,] currentMatrix = GetCurrentMatrix();
        int tileIndex = 0;

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (currentMatrix[y, x] == 1)
                {
                    if (tileIndex < acidTileObjects.Count && acidTileObjects[tileIndex] != null)
                    {
                        // Z: -0.5f vererek mor karelerin kesin üzerine çıkarıyoruz
                        Vector3 tilePos = new Vector3(columnXPositions[x], rowYPositions[y], -0.5f);
                        acidTileObjects[tileIndex].transform.position = tilePos;
                        acidTileObjects[tileIndex].transform.rotation = Quaternion.identity;
                        tileIndex++;
                    }
                }
            }
        }
    }

    private int[,] GetCurrentMatrix()
    {
        int[,] matrix = new int[4, 4];
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                switch (currentCorner)
                {
                    // Sol Üst durumunda matrisin sağ/solunu ters çeviriyoruz (3 - x)
                    case SpawnCorner.TopLeft:
                        matrix[y, x] = patternTopLeft[y, 3 - x];
                        break;

                    // Sağ Üst durumunda düz halini alıyoruz
                    case SpawnCorner.TopRight:
                        matrix[y, x] = patternTopLeft[y, x];
                        break;

                    // Sol Alt
                    case SpawnCorner.BottomLeft:
                        matrix[y, x] = patternTopLeft[3 - y, 3 - x];
                        break;

                    // Sağ Alt
                    case SpawnCorner.BottomRight:
                        matrix[y, x] = patternTopLeft[3 - y, x];
                        break;
                }
            }
        }
        return matrix;
    }

    public void ResetEffects()
    {
        // Tüm efektleri sıfırla
        if (chargeEffect != null)
            chargeEffect.SetActive(false);

        foreach (var tile in acidTileObjects)
        {
            if (tile != null) tile.SetActive(false);
        }
    }

    /// <summary>
    /// Beat 2 & 3: Şarj Efekti
    /// </summary>
    public void OnBeat_Charge()
    {
        if (chargeEffect != null)
            chargeEffect.SetActive(true);
    }

    /// <summary>
    /// Beat 4: Asitleri yak ve oyuncuya vurup vurmadığını kontrol et
    /// </summary>
    public bool OnBeat4_ExecuteAttack(Vector2Int playerGridPos)
    {
        // 1. Şarj efektini KAPAT (Beat 4'te patlama anında yok olsun)
        if (chargeEffect != null)
            chargeEffect.SetActive(false);

        // 2. Zemin asitlerini YAK
        PositionAcidTiles();
        foreach (var tile in acidTileObjects)
        {
            if (tile != null) tile.SetActive(true);
        }

        // 3. Vuruş Kontrolü
        int[,] currentMatrix = GetCurrentMatrix();
        if (playerGridPos.x >= 0 && playerGridPos.x < 4 && playerGridPos.y >= 0 && playerGridPos.y < 4)
        {
            return currentMatrix[playerGridPos.y, playerGridPos.x] == 1;
        }

        return false;
    }
}