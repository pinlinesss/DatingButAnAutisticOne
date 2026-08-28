using System.Collections;
using System.Linq;
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

    [Header("Beam Animation Setup")]
    public SpriteRenderer beamSpriteRenderer; // Beat 2 ve 3'teki şarj/uzama animasyonları için
    public Sprite[] chargeFrames;            // Şarj Sprite'ları
    public Sprite[] attackFrames;            // Hazırlık / Uzama Sprite'ları

    [Header("Beat 4 Child Object")]
    public GameObject beat4BeamObject;      // Beat 4'te açılacak Child GameObject

    private Coroutine currentAnimCoroutine;

    private void Start()
    {
        ResetBeams();
    }

    private void OnEnable()
    {
        ResetBeams();
    }

    /// <summary>
    /// Beat 1: Düşmanı yeni konuma koyar ve tüm görsel nesneleri gizler.
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
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;

            case SpawnSide.Right:
                transform.position = new Vector3(lastX + offsetFromEdge, rowYPositions[lineIndex], 0f);
                transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
        }
    }

    /// <summary>
    /// Beat 2: Şarj ve hazırlık animasyon döngüsünü başlatır.
    /// </summary>
    public void OnBeat_Charge(float beatInterval = 0.5f)
    {
        if (beamSpriteRenderer != null)
        {
            beamSpriteRenderer.enabled = true;

            if (currentAnimCoroutine == null)
            {
                currentAnimCoroutine = StartCoroutine(PlaySeamlessBeamRoutine(beatInterval));
            }
        }
    }

    /// <summary>
    /// Beat 3: Animasyon coroutine içinden akmaya devam eder.
    /// </summary>
    public void OnBeat_Attack(float beatInterval = 0.5f)
    {
        // Beat 2'de başlayan Coroutine akışı devam ettirir.
    }

    /// <summary>
    /// Beat 4: Vuruş kontrolü yapar, şarj animasyonunu kapatıp Beat 4 Child Object'ini aktif eder.
    /// </summary>
    public bool OnBeat4_ExecuteAttack(Vector2Int playerGridPos)
    {
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

        // Önceki animasyonu durdur ve şarj renderer'ını kapat
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null;
        }

        if (beamSpriteRenderer != null)
        {
            beamSpriteRenderer.sprite = null;
            beamSpriteRenderer.enabled = false;
        }

        // Beat 4 Child Object'ini aç
        if (beat4BeamObject != null)
        {
            beat4BeamObject.SetActive(true);
        }

        return isHit;
    }

    /// <summary>
    /// Tüm animasyonları durdurur ve hem sprite renderer'ı hem de child objeyi gizler.
    /// </summary>
    public void ResetBeams()
    {
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null;
        }

        if (beamSpriteRenderer != null)
        {
            beamSpriteRenderer.sprite = null;
            beamSpriteRenderer.enabled = false;
        }

        if (beat4BeamObject != null)
        {
            beat4BeamObject.SetActive(false);
        }
    }

    private IEnumerator PlaySeamlessBeamRoutine(float beatInterval)
    {
        Sprite[] allFrames = chargeFrames.Concat(attackFrames).ToArray();

        if (allFrames.Length == 0) yield break;

        float totalDuration = beatInterval * 2f;
        float timePerFrame = totalDuration / allFrames.Length;

        for (int i = 0; i < allFrames.Length; i++)
        {
            if (beamSpriteRenderer != null)
            {
                beamSpriteRenderer.sprite = allFrames[i];
            }
            yield return new WaitForSeconds(timePerFrame);
        }

        currentAnimCoroutine = null;
    }
}