using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Sahne/Bölüm Diyalogları")]
    public List<DialogueLine> dialogueList;

    public void TriggerDialogue()
    {
        // Sahnendeki DialogueManager'ı bulup diyalogu başlatır
        DialogueManager manager = FindFirstObjectByType<DialogueManager>();
        if (manager != null)
        {
            manager.StartDialogue(dialogueList);
        }
    }

    private void Start()
    {
        // Oyun başlar başlamaz diyalogun girmesini istiyorsan:
        TriggerDialogue();
    }
}