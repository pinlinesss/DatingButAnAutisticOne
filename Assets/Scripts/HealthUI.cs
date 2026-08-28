using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Sol şeritteki 3 can resmini sırasıyla (1. Can, 2. Can, 3. Can) buraya sürükle.")]
    public Image[] heartImages = new Image[3];

    [Header("Face Sprites")]
    public Sprite happyFaceSprite;   // Canlı olduğu anki sabit/düz görsel

    [Header("Dead Animation Settings")]
    [Tooltip("Can ikonlarının üzerindeki Animator bileşenleri (aynı Image nesnelerindeki).")]
    public Animator[] heartAnimators = new Animator[3];
    
    [Tooltip("Öldüğünde çalışacak animasyon klibinin Animator'daki tam adı")]
    public string deadAnimationState = "Dead";

    /// <summary>
    /// Can değiştikçe canlı sprite'ı gösterir veya ölü animasyonunu tetikler.
    /// </summary>
    /// <param name="currentHealth">Oyuncunun kalan canı (0 - 3)</param>
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            if (i < currentHealth)
            {
                // Canlıysa: Animator'ı kapat/durdur ve canlı sprite'ını bas
                if (heartAnimators[i] != null)
                {
                    heartAnimators[i].enabled = false;
                }
                heartImages[i].sprite = happyFaceSprite;
            }
            else
            {
                // Öldüyse: Animator'ı aktif et ve ölü animasyonunu oynat
                if (heartAnimators[i] != null)
                {
                    heartAnimators[i].enabled = true;
                    heartAnimators[i].Play(deadAnimationState, -1, 0f);
                }
            }
        }
    }
}