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
    public SpriteRenderer beamSpriteRenderer; // Ağız/Göz önündeki Sprite Renderer
    public Sprite[] chargeFrames;            // 6 Karelik Şarj Sprite'ları
    public Sprite[] attackFrames;            // 8 Karelik BEAM Sprite'ları

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
    /// Beat 1: Düşmanı yeni konuma koyar ve ışınları temizler.
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
    /// Beat 2: 6 Karelik Şarj Animasyonunu Oynatır
    /// </summary>
    public void OnBeat_Charge(float beatInterval = 0.5f)
    {
        if (beamSpriteRenderer != null)
        {
            beamSpriteRenderer.enabled = true;

            // Eğer animasyon zaten başlamadıysa başlat
            if (currentAnimCoroutine == null)
            {
                currentAnimCoroutine = StartCoroutine(PlaySeamlessBeamRoutine(beatInterval));
            }
        }
    }

    /// <summary>
    /// Beat 3: 8 Karelik BEAM Saldırı Animasyonunu Oynatır
    /// </summary>
    public void OnBeat_Attack(float beatInterval = 0.5f)
    {
        // Boş - Zaten PlaySeamlessBeamRoutine kesintisiz oynatıyor.
    }

    /// <summary>
    /// Beat 4: Vuruş kontrolü yapar ve ışını gizler.
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

        ResetBeams();
        return isHit;
    }

    /// <summary>
    /// Tüm animasyonları durdurur ve resimleri temizler.
    /// </summary>
    public void ResetBeams()
    {
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null; // Hafızayı boşaltıyoruz!
        }

        if (beamSpriteRenderer != null)
        {
            beamSpriteRenderer.sprite = null;
            beamSpriteRenderer.enabled = false;
        }
    }

    private void PlayAnimation(Sprite[] frames, float duration)
    {
        if (frames == null || frames.Length == 0) return;
        if (currentAnimCoroutine != null) StopCoroutine(currentAnimCoroutine);

        currentAnimCoroutine = StartCoroutine(AnimateRoutine(frames, duration));
    }

    private IEnumerator AnimateRoutine(Sprite[] frames, float duration)
    {
        float frameTime = duration / frames.Length;

        for (int i = 0; i < frames.Length; i++)
        {
            beamSpriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(frameTime);
        }
    }

    private IEnumerator PlayFullBeamSequenceRoutine(float beatInterval)
    {
        // Beat 2 boyunca Şarj karelerini oynatır
        if (chargeFrames != null && chargeFrames.Length > 0)
        {
            float chargeFrameTime = beatInterval / chargeFrames.Length;
            for (int i = 0; i < chargeFrames.Length; i++)
            {
                beamSpriteRenderer.sprite = chargeFrames[i];
                yield return new WaitForSeconds(chargeFrameTime);
            }
        }

        // Kesintisiz şekilde Beat 3 boyunca BEAM karelerini oynatır
        if (attackFrames != null && attackFrames.Length > 0)
        {
            float attackFrameTime = beatInterval / attackFrames.Length;
            for (int i = 0; i < attackFrames.Length; i++)
            {
                beamSpriteRenderer.sprite = attackFrames[i];
                yield return new WaitForSeconds(attackFrameTime);
            }
        }

        currentAnimCoroutine = null;
    }


    private IEnumerator PlaySeamlessBeamRoutine(float beatInterval)
    {
        // 1. Şarj ve Saldırı karelerini tek bir dizide birleştiriyoruz
        Sprite[] allFrames = chargeFrames.Concat(attackFrames).ToArray();

        if (allFrames.Length == 0) yield break;

        // Toplam süre: Beat 2 + Beat 3 (Toplam 2 Beat süresi)
        float totalDuration = beatInterval * 2f;
        float timePerFrame = totalDuration / allFrames.Length;

        // 2. Arada hiçbir duraksama veya silinme olmadan tek döngüde oynatıyoruz
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