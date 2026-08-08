using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverSequence : MonoBehaviour
{
    [Header("Paneller ve Elemanlar")]
    public CanvasGroup gameOverCanvasGroup; // Ekranın kararması için (Alpha ayarı)
    public GameObject gameOverTitle;
    public GameObject puppetCharacter;
    public GameObject sakizContainer;
    public GameObject buttonsContainer;

    [Header("Zamanlama Ayarları")]
    public float delayBeforeFade = 1.0f; // Karakter ölünce bekleme süresi
    public float fadeDuration = 0.8f;    // Kararma süresi
    public float timeBetweenSteps = 0.5f; // Elemanlar arası geliş süresi

    private void Start()
    {
        // Başlangıçta hepsini gizle
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.gameObject.SetActive(false);
        }
        
        gameOverTitle.SetActive(false);
        puppetCharacter.SetActive(false);
        sakizContainer.SetActive(false);
        buttonsContainer.SetActive(false);
    }

    // Karakterin son canı bittiğinde bu fonksiyon çağrılacak
    public void StartGameOverSequence()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        // 1. Aşama: Karakter ölünce oyun 1 saniyeliğine donar/duraksar
        yield return new WaitForSeconds(delayBeforeFade);

        // 2. Aşama: Ekran Kararır
        gameOverCanvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        gameOverCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(timeBetweenSteps);

        // 3. Aşama: "GAME OVER" ve Sağdaki Baygın Karakter Görünür
        gameOverTitle.SetActive(true);
        puppetCharacter.SetActive(true);

        yield return new WaitForSeconds(timeBetweenSteps + 0.3f);

        // 4. Aşama: Sakız Portresi ve Konuşması Gelir
        sakizContainer.SetActive(true);

        yield return new WaitForSeconds(timeBetweenSteps + 0.5f);

        // 5. Aşama: "AYAĞA KALK" ve "YAT GİTSİN" Butonları Belirir
        buttonsContainer.SetActive(true);
    }

    // Buton Fonksiyonları
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GiveUp()
    {
        Debug.Log("Pes edildi.");
        Application.Quit();
    }
}