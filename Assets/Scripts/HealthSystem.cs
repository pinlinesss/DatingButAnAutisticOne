using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI Reference")]
    public HealthUI healthUI;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.UpdateHealthUI(currentHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Karakter] Hasar aldın! Kesin acımıştır... Kalan Can: {currentHealth}");

        // UI Suratlarını Güncelle
        if (healthUI != null)
        {
            healthUI.UpdateHealthUI(currentHealth);
        }

        // Hasar efekti (Karakter anlık kırmızı yanıp söner)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            StartCoroutine(FlashRed(sr));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed(SpriteRenderer sr)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=red>[Karakter] MEH ÖLDÜN BE DOSTUM!!! </color>");

        // "IncludeInactive.Include" parametresi sahnede kapalı duran GameOverSequence objelerini de bulur!
        GameOverSequence gameOverSequence = FindFirstObjectByType<GameOverSequence>(FindObjectsInactive.Include);

        if (gameOverSequence != null)
        {
            gameOverSequence.StartGameOverSequence();
        }
        else
        {
            Debug.LogError("Sahnede GameOverSequence script'i bulunamadı!");
        }

        // 2. Karakteri yok etmek yerine sadece Sprite/Collider'ını kapat
        // (Böylece üzerindeki kodlar veya Coroutine'ler patlamaz)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}