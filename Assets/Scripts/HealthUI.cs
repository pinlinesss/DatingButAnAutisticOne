using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Sol şeritteki 3 can resmini sırasıyla (1. Can, 2. Can, 3. Can) buraya sürükle.")]
    public Image[] heartImages = new Image[3];

    [Header("Face Sprites")]
    public Sprite happyFaceSprite;  // Mutlu Surat Sprite'ı
    public Sprite deadFaceSprite;   // X_x Surat Sprite'ı

    /// <summary>
    /// Can değiştikçe surat görsellerini günceller.
    /// </summary>
    /// <param name="currentHealth">Oyuncunun kalan canı (0 - 3)</param>
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            // Eğer can indeksimiz kalan candan küçükse 'Mutlu', değilse 'X_x' yapıyoruz
            if (i < currentHealth)
            {
                heartImages[i].sprite = happyFaceSprite;
            }
            else
            {
                heartImages[i].sprite = deadFaceSprite;
            }
        }
    }
}