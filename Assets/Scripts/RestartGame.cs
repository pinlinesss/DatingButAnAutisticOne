using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public float delayTime = 0.8f; 

    public void RestartCurrentSceneWithDelay()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quit butonu için yeni fonksiyon
    public void QuitGameWithDelay()
    {
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        // Patlama animasyonu bitene kadar bekle
        yield return new WaitForSeconds(delayTime);

        // Eğer Unity Editör içindeysek Play modunu durdurur
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // Eğer exe / build alınmış oyundaysak uygulamayı kapatır
        Application.Quit();
    }
}