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
        // Objenin SetActive'ini kapatmıyoruz! 
        // Sadece ekranı tamamen görünmez yapıp tıklamaları engelliyoruz.
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0f;
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
        }

        // İç elemanları başlangıçta gizle
        if (gameOverTitle != null) gameOverTitle.SetActive(false);
        if (puppetCharacter != null) puppetCharacter.SetActive(false);
        if (sakizContainer != null) sakizContainer.SetActive(false);
        if (buttonsContainer != null) buttonsContainer.SetActive(false);
    }
    // Karakterin son canı bittiğinde bu fonksiyon çağrılacak
    public void StartGameOverSequence()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        // 1. Aşama: Karakter ölünce 1 saniye donma/bekleme
        yield return new WaitForSeconds(delayBeforeFade);

        // Tıklamaları ve görünürlüğü aç
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.interactable = true;
            gameOverCanvasGroup.blocksRaycasts = true;
        }

        // 2. Aşama: Ekran Yavaşça Kararır (Alpha 0 -> 1)
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (gameOverCanvasGroup != null)
            {
                gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            }
            yield return null;
        }
        if (gameOverCanvasGroup != null) gameOverCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(timeBetweenSteps);

        // 3. Aşama: GAME OVER ve Baygın Karakter Görünür
        if (gameOverTitle != null) gameOverTitle.SetActive(true);
        if (puppetCharacter != null) puppetCharacter.SetActive(true);

        yield return new WaitForSeconds(timeBetweenSteps + 0.3f);

        // 4. Aşama: Sakız Portresi ve Konuşması Belirir
        if (sakizContainer != null) sakizContainer.SetActive(true);

        yield return new WaitForSeconds(timeBetweenSteps + 0.5f);

        // 5. Aşama: AYAĞA KALK / YAT GİTSİN Butonları Açılır
        if (buttonsContainer != null) buttonsContainer.SetActive(true);
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