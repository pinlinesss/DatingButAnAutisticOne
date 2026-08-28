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

    [Header("Sprite References")]
    public Sprite beatBosSprite;
    public Sprite beat1Sprite;
    public Sprite beat2Sprite;
    public Sprite beat3Sprite;
    public Sprite ekranBosSprite;
    public Sprite ekranPampumSprite;

    [Header("UI Image References")]
    public Image beatImage1;
    public Image beatImage2;
    public Image beatImage3;
    public Image screenImage;

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
        if (beatImage1 != null) beatImage1.sprite = beatBosSprite;
        if (beatImage2 != null) beatImage2.sprite = beatBosSprite;
        if (beatImage3 != null) beatImage3.sprite = beatBosSprite;
        if (screenImage != null) screenImage.sprite = ekranBosSprite;

        switch (currentBeat)
        {
            case 1:
                if (beatImage1 != null) beatImage1.sprite = beat1Sprite;
                break;
            case 2:
                patternManager.ChargeAllEnemies();
                break;

            case 3:
                patternManager.AttackAllEnemies();
                break;
            case 4:
                if (screenImage != null) screenImage.sprite = ekranPampumSprite;
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
                patternManager.ChargeAllEnemies(); // Beat 2: Şarj
                break;

            case 3:
                patternManager.AttackAllEnemies(); // Beat 3: BEAM Patlaması
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
}