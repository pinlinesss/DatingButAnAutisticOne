using System.Collections;
using UnityEngine;
using TMPro;

public class GameOverTextWriter : MonoBehaviour
{
    public TMP_Text gameOverText;
    public CanvasGroup buttonsCanvasGroup; // CanvasGroup ekledik
    public float typingSpeed = 0.05f;

    [TextArea(3, 5)]
    public string fullText = "*Hey! Sooo u just imagining to lay down there and do nothing huh?";

    private void OnEnable()
    {
        StopAllCoroutines(); 
        StartCoroutine(TypeTextRoutine());
    }

    private IEnumerator TypeTextRoutine()
    {
        // 1. Yazıyı temizle
        if (gameOverText != null) gameOverText.text = "";

        // 2. Butonları tamamen görünmez ve tıklanamaz yap
        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.alpha = 0f;
            buttonsCanvasGroup.interactable = false;
            buttonsCanvasGroup.blocksRaycasts = false;
        }

        yield return null;

        // 3. Harf harf daktilo gibi yazdır
        foreach (char letter in fullText)
        {
            if (gameOverText != null)
            {
                gameOverText.text += letter;
            }
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // 4. Yazı bittiği an butonları görünür ve tıklanabilir yap
        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.alpha = 1f;
            buttonsCanvasGroup.interactable = true;
            buttonsCanvasGroup.blocksRaycasts = true;
        }
    }
}