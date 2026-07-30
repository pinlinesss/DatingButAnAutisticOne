using System.Collections.Generic;
using UnityEngine;

// Düşmanın tipini seçmek için (Kırmızı mı Mavi mi?)
public enum EnemyType { Red, Blue }

[System.Serializable]
public struct EnemySpawnData
{
    public EnemyType enemyType; // Kırmızı mı Mavi mi?
    public RedEnemy.SpawnSide side; // Hangi kenar?
    public int lineIndex; // 0, 1, 2, 3
}

[System.Serializable]
public struct Pattern
{
    public string patternName;
    public bool isActive; // Tik kutusu: Açık mı Kapalı mı?
    public List<EnemySpawnData> activeEnemies;
}

public class PatternManager : MonoBehaviour
{
    [Header("Enemies in Scene")]
    public List<RedEnemy> redEnemies;   // Sahnedeki Kırmızı Düşman Havuzu
    public List<BlueEnemy> blueEnemies; // Sahnedeki Mavi Düşman Havuzu

    [Header("Pattern Pool")]
    public List<Pattern> patternPool = new List<Pattern>();

    /// <summary>
    /// Beat 1: Tüm efektleri temizler ve yeni pattern'deki düşmanları yerleştirir.
    /// </summary>
    public void SpawnRandomPattern()
    {
        // 1. Düşmanları ve efektleri temizle (Aynı kalıyor)
        foreach (var red in redEnemies)
        {
            if (red != null)
            {
                red.ResetBeams();
                red.gameObject.SetActive(false);
            }
        }

        foreach (var blue in blueEnemies)
        {
            if (blue != null)
            {
                blue.ResetEffects();
                blue.gameObject.SetActive(false);
            }
        }

        // 2. SADECE AKTİF (TİK ATILMIŞ) PATTERN'LERİ LİSTEYE TOPLA
        List<Pattern> activePatterns = new List<Pattern>();
        foreach (var pattern in patternPool)
        {
            if (pattern.isActive)
            {
                activePatterns.Add(pattern);
            }
        }

        // Eğer hiç aktif pattern yoksa çık
        if (activePatterns.Count == 0) return;

        // 3. Sadece aktif olanlar arasından rastgele birini seç!
        int randomIndex = Random.Range(0, activePatterns.Count);
        Pattern selectedPattern = activePatterns[randomIndex];

        int currentRedIndex = 0;
        int currentBlueIndex = 0;

        // 4. Seçilen pattern'ı sahneye yerleştir (Aynı kalıyor)
        foreach (var spawnData in selectedPattern.activeEnemies)
        {
            if (spawnData.enemyType == EnemyType.Red)
            {
                if (currentRedIndex < redEnemies.Count && redEnemies[currentRedIndex] != null)
                {
                    RedEnemy red = redEnemies[currentRedIndex];
                    red.gameObject.SetActive(true);
                    red.SetupPosition(spawnData.side, spawnData.lineIndex);
                    currentRedIndex++;
                }
            }
            else if (spawnData.enemyType == EnemyType.Blue)
            {
                if (currentBlueIndex < blueEnemies.Count && blueEnemies[currentBlueIndex] != null)
                {
                    BlueEnemy blue = blueEnemies[currentBlueIndex];
                    blue.gameObject.SetActive(true);

                    BlueEnemy.SpawnSide blueSide = (BlueEnemy.SpawnSide)spawnData.side;
                    blue.SetupPosition(blueSide, spawnData.lineIndex);

                    currentBlueIndex++;
                }
            }
        }
    }
    /// <summary>
    /// Beat 2 ve 3: Tüm aktif düşmanlara şarj efekti verdirir
    /// </summary>
    public void ChargeAllEnemies()
    {
        foreach (var red in redEnemies)
        {
            if (red != null && red.gameObject.activeSelf) red.OnBeat_Charge();
        }

        foreach (var blue in blueEnemies)
        {
            if (blue != null && blue.gameObject.activeSelf) blue.OnBeat_Charge();
        }
    }

    /// <summary>
    /// Beat 4: Tüm aktif düşmanlar aynı anda saldırır
    /// </summary>
    public bool ExecuteAllAttacks(Vector2Int playerGridPos)
    {
        bool hitPlayer = false;

        foreach (var red in redEnemies)
        {
            if (red != null && red.gameObject.activeSelf)
            {
                if (red.OnBeat4_ExecuteAttack(playerGridPos)) hitPlayer = true;
            }
        }

        foreach (var blue in blueEnemies)
        {
            if (blue != null && blue.gameObject.activeSelf)
            {
                if (blue.OnBeat4_ExecuteAttack(playerGridPos)) hitPlayer = true;
            }
        }

        return hitPlayer;
    }
}