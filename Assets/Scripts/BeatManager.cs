using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    [Header("Player References")]
    public HealthSystem playerHealth;
    public GridPlayerController playerMovement;

    [Header("Pattern Manager Reference")]
    public PatternManager patternManager;

    [Header("Tower Beat Sprites")]
    public Sprite beatBosSprite; // Hiçbiri yanmayan (Başlangıç/Sıfır durumu)
    public Sprite beat1Sprite;   // En üstteki göz kırmızı
    public Sprite beat2Sprite;   // Ortadaki göz kırmızı
    public Sprite beat3Sprite;   // En alttaki göz kırmızı
    public Sprite beat4Sprite;   // En alttaki mor ağız/küre aktif (Vuruş anı)

    [Header("UI Image Reference")]
    public Image towerBeatImage; // UI'daki tekli kule göstergesi

    [Header("Rhythm Settings")]
    public float beatInterval = 0.5f;

    private int currentBeat = 0;

    private void Start()
    {
        StartCoroutine(RhythmRoutine());
    }

    private IEnumerator RhythmRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(beatInterval);
            AdvanceBeat();
        }
    }

    private void AdvanceBeat()
    {
        currentBeat++;
        if (currentBeat > 4)
        {
            currentBeat = 1;
        }

        UpdateVisuals();
        TriggerAttackEvents();
    }

    private void UpdateVisuals()
    {
        if (towerBeatImage == null) return;

        switch (currentBeat)
        {
            case 1:
                towerBeatImage.sprite = beat1Sprite;
                break;
            case 2:
                towerBeatImage.sprite = beat2Sprite;
                break;
            case 3:
                towerBeatImage.sprite = beat3Sprite;
                break;
            case 4:
                towerBeatImage.sprite = beat4Sprite;
                break;
            default:
                towerBeatImage.sprite = beatBosSprite;
                break;
        }
    }

    private void TriggerAttackEvents()
    {
        if (patternManager == null) return;

        switch (currentBeat)
        {
            case 1:
                patternManager.SpawnRandomPattern();
                break;

            case 2:
                patternManager.ChargeAllEnemies();
                break;

            case 3:
                patternManager.AttackAllEnemies();
                break;

            case 4:
                if (playerMovement != null)
                {
                    Vector2Int gridPosition = playerMovement.currentGridPos;
                    bool isHit = patternManager.ExecuteAllAttacks(gridPosition);

                    if (isHit && playerHealth != null)
                    {
                        playerHealth.TakeDamage(1);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Gerekirse kuleyi baştan sarmak veya durdurmak için kullanılabilir.
    /// </summary>
    public void ResetTowerVisual()
    {
        if (towerBeatImage != null)
        {
            towerBeatImage.sprite = beatBosSprite;
        }
    }
}