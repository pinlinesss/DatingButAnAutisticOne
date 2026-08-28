using System.Collections.Generic;
using UnityEngine;

// Düşmanın tipini seçmek için (Kırmızı mı Mavi mi?)
public enum EnemyType { Red, Blue, Green } // Green eklendi

[System.Serializable]
public struct EnemySpawnData
{
    public EnemyType enemyType;
    public RedEnemy.SpawnSide side; // Kırmızı ve Mavi için
    public GreenEnemy.SpawnCorner corner; // Yeşil için köşe seçimi!
    public int lineIndex;
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
    public List<RedEnemy> redEnemies;
    public List<BlueEnemy> blueEnemies;
    public List<GreenEnemy> greenEnemies; // Yeşil Düşman havuzu!
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

        foreach (var green in greenEnemies)
        {
            if (green != null)
            {
                green.ResetEffects();
                green.gameObject.SetActive(false);
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
        int currentGreenIndex = 0;

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
            else if (spawnData.enemyType == EnemyType.Green)
            {
                if (currentGreenIndex < greenEnemies.Count && greenEnemies[currentGreenIndex] != null)
                {
                    GreenEnemy green = greenEnemies[currentGreenIndex];
                    green.gameObject.SetActive(true);
                    green.SetupPosition(spawnData.corner);
                    currentGreenIndex++;
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

        foreach (var green in greenEnemies)
        {
            if (green != null && green.gameObject.activeSelf)
            {
                green.OnBeat_Charge();
            }
        }
    }

    /// <summary>
    /// Beat 4: Tüm aktif düşmanlar aynı anda saldırır
    /// </summary>
    public void AttackAllEnemies()
    {
        foreach (var red in redEnemies)
        {
            if (red != null && red.gameObject.activeSelf) red.OnBeat_Attack(); // Burası OnBeat_Attack olmalı!
        }

        foreach (var blue in blueEnemies)
        {
            if (blue != null && blue.gameObject.activeSelf) blue.OnBeat_Charge();
        }

        foreach (var green in greenEnemies)
        {
            if (green != null && green.gameObject.activeSelf) green.OnBeat_Charge();
        }
    }
    
    public bool ExecuteAllAttacks(Vector2Int playerPos)
    {
        // EN BAŞA BU SATIRI EKLİYORUZ (Değişkeni tanımlıyoruz)
        bool hit = false;

        // Kırmızı Düşmanlar
        foreach (var red in redEnemies)
        {
            if (red != null && red.gameObject.activeSelf)
            {
                if (red.OnBeat4_ExecuteAttack(playerPos)) hit = true;
            }
        }

        // Mavi Düşmanlar
        foreach (var blue in blueEnemies)
        {
            if (blue != null && blue.gameObject.activeSelf)
            {
                if (blue.OnBeat4_ExecuteAttack(playerPos)) hit = true;
            }
        }

        // Yeşil Düşmanlar
        foreach (var green in greenEnemies)
        {
            if (green != null && green.gameObject.activeSelf)
            {
                if (green.OnBeat4_ExecuteAttack(playerPos)) hit = true;
            }
        }

        return hit;
    }
}