using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct DialogueLine
{
    public enum Speaker { Gum, Player }
    public Speaker speaker; // Cümleyi kim söylüyor?
    public Sprite speakerSprite; // O anki yüz ifadesi/sprite'ı
    [TextArea(2, 5)]
    public string sentence; // Konuşma metni
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    public GameObject dialogueBox;
    public Image gumImage;
    public Image playerImage;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Ayar")]
    public float typingSpeed = 0.03f; // Daktilo hızı

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private string currentSentence;

    public void StartDialogue(List<DialogueLine> dialogueList)
    {
        dialogueBox.SetActive(true);
        linesQueue.Clear();

        foreach (DialogueLine line in dialogueList)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Eğer hala metin yazılıyorsa tıklandığında anında cümleyi tamamla
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = linesQueue.Dequeue();
        currentSentence = currentLine.sentence;

        // Konuşana göre Portre ve İsim Güncelleme
        if (currentLine.speaker == DialogueLine.Speaker.Gum)
        {
            speakerNameText.text = "Gum";
            if (currentLine.speakerSprite != null) gumImage.sprite = currentLine.speakerSprite;
            
            // Gum aktif, Player gölgede/saydam
            gumImage.color = Color.white;
            playerImage.color = new Color(0.5f, 0.5f, 0.5f, 0.6f); 
        }
        else
        {
            speakerNameText.text = "Hayalet Kız"; // veya karakterin adı
            if (currentLine.speakerSprite != null) playerImage.sprite = currentLine.speakerSprite;

            // Player aktif, Gum gölgede/saydam
            playerImage.color = Color.white;
            gumImage.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        // Diyalog bitince oyunu başlatma veya devam ettirme kodun
    }

    void Update()
    {
        // Ekra veya Space'e basınca sonraki cümleye geç
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (dialogueBox.activeSelf)
            {
                DisplayNextSentence();
            }
        }
    }
}