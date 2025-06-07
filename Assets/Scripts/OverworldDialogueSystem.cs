using System.Collections;
using UnityEngine;
using TMPro;

public class OverworldDialogueSystem : MonoBehaviour
{
    public string[] speech;
    public string characterName;
    public GameObject textBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speechText;
    public GameObject player;

    private int speechInt;
    private bool isTalking;
    private bool isTyping;
    private bool canContinue;
    private Coroutine typingCoroutine;
    private CharacterDialogue speaker;

    void Update()
    {
        if (!isTalking) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            // If still typing: skip to full line
            if (isTyping)
            {
                SkipTyping();
            }
            // If done typing: go to next line
            else if (canContinue)
            {
                if (speechInt < speech.Length)
                {
                    typingCoroutine = StartCoroutine(TypeWriter(speech[speechInt++]));
                }
                else
                {
                    EndSpeech();
                }
            }
        }
    }

    public void StartDialogue(CharacterDialogue characterDialogue)
    {
        speaker = characterDialogue;
        speech = speaker.speech;
        characterName = speaker.characterName;

        textBox.SetActive(true);
        isTalking = true;
        speechInt = 0;

        nameText.text = characterName;
        typingCoroutine = StartCoroutine(TypeWriter(speech[speechInt++]));
    }

    private IEnumerator TypeWriter(string sentence)
    {
        isTyping = true;
        canContinue = false;
        speechText.text = "";

        foreach (char letter in sentence)
        {
            speechText.text += letter;
            yield return new WaitForSeconds(0.03f); // Adjust speed here
        }

        isTyping = false;
        canContinue = true;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Show full current sentence
        speechText.text = speech[speechInt - 1];
        isTyping = false;
        canContinue = true;
    }

    private void EndSpeech()
    {
        isTalking = false;
        textBox.SetActive(false);
        player.GetComponent<MovementScript>().isTalking = false;
    }
}
