using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
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

    [Header("Enemy Test References")]
    public RedEnemy activeRedEnemy;
    public Vector2Int testPlayerGridPos = new Vector2Int(0, 0);

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
        beatImage1.sprite = beatBosSprite;
        beatImage2.sprite = beatBosSprite;
        beatImage3.sprite = beatBosSprite;
        screenImage.sprite = ekranBosSprite;

        switch (currentBeat)
        {
            case 1:
                beatImage1.sprite = beat1Sprite;
                break;
            case 2:
                beatImage2.sprite = beat2Sprite;
                break;
            case 3:
                beatImage3.sprite = beat3Sprite;
                break;
            case 4:
                screenImage.sprite = ekranPampumSprite;
                break;
        }
    }

    private void TriggerAttackEvents()
    {
        if (activeRedEnemy == null) return;

        switch (currentBeat)
        {
            case 1:
                // 1) 4 kenardan birini rastgele seç (0: Top, 1: Bottom, 2: Left, 3: Right)
                RedEnemy.SpawnSide randomSide = (RedEnemy.SpawnSide)Random.Range(0, 4);

                // 2) 0-3 arası rastgele şerit seç
                int randomLine = Random.Range(0, 4);

                activeRedEnemy.InitializeEnemy(randomSide, randomLine); 
                Debug.Log($"Beat 1: Kırmızı Düşman {randomSide} kenarında, {randomLine + 1}. hatta belirdi!");
                break;

            case 2:
                activeRedEnemy.OnBeat2_EnemyShake();
                break;

            case 3:
                activeRedEnemy.OnBeat3_GroundShake();
                break;

            case 4:
                bool playerKilled = activeRedEnemy.OnBeat4_ExecuteAttack(testPlayerGridPos);
                break;
        }
    }
}