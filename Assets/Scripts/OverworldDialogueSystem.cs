using System.Collections;
using UnityEngine;

public class OverworldDialogueSystem : DialogueSystemParent
{
    public string[] speech;
    public GameObject textBox;
    public GameObject player;

    private CharacterDialogue speaker;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isTalking)
        {
            if (isTyping)
                SkipTyping();
            else if (canContinue)
                ShowNextSentence();
        }
    }

    public void StartDialogue(CharacterDialogue characterDialogue)
    {
        speechInt = 0;
        speaker = characterDialogue;
        speech = (string[])speaker.speech.Clone();
        currentCharacterName = speaker.characterName;

        textBox.SetActive(true);
        sprite.sprite = speaker.sprite[0];
        sprite.gameObject.SetActive(true);

        nameText.text = currentCharacterName;

        isTalking = true;

        ShowNextSentence(); // Start with first line
    }

    public override void StartDialogue()
    {
        // Not used here because we rely on the overload with CharacterDialogue input
    }

    protected override void ShowNextSentence()
    {
        if (speechInt >= speech.Length)
        {
            EndSpeech();
            return;
        }

        ParsedDialogueLine parsed = ParseDialogueLine(speech[speechInt++]);
        sprite.sprite = speaker.sprite[parsed.moodIndex];
        currentSentence = parsed.cleanedText;
        typingCoroutine = StartCoroutine(TypeWriter(currentSentence));
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speechText.text = currentSentence;
        isTyping = false;
        canContinue = true;
    }

    private void EndSpeech()
    {
        isTalking = false;
        NullifyDialogue();
        textBox.SetActive(false);
        sprite.gameObject.SetActive(false);
        speechInt = 0;

        if (player != null)
            player.GetComponent<MovementScript>().isTalking = false;
    }

    private void NullifyDialogue()
    {
        speaker = null;
        speech = null;
        currentCharacterName = null;
        speechText.text = "";
        nameText.text = "";
    }
}