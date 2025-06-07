using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
public class OverworldDialogueSystem : MonoBehaviour
{
    public string[] speech;
    public string characterName;
    public GameObject textBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speechText;
    public GameObject player;
    public Image sprite; 

    private int speechInt;
    private bool isTalking;
    private bool isTyping;
    private bool canContinue;
    private Coroutine typingCoroutine;
    private CharacterDialogue speaker;
    private string cleanSentence;
    private string currentSentence;

    private Dictionary<string, int> moodMap = new Dictionary<string, int>
    {
    { "idle", 0 },
    { "happy", 1 },
    { "sad", 2 },
    { "angry", 3 },
    { "shocked", 4 }
    };

    void Update()
    {
        if (isTyping)
        {
            SkipTyping(); // Show full current sentence immediately
        }
        else if (canContinue)
        {
            ShowNextSentence(); // Go to next one
        }
    }

    public void StartDialogue(CharacterDialogue characterDialogue)
    {
        

        speaker = characterDialogue;
        speech = speaker.speech;
        characterName = speaker.characterName;

        textBox.SetActive(true);
        sprite.sprite = characterDialogue.sprite[0]; 
        sprite.gameObject.SetActive(true);
        isTalking = true;
        speechInt = 0;

        nameText.text = characterName;
        string rawSentence = speech[speechInt];
        cleanSentence = ParseMoodTags(ref rawSentence);
        typingCoroutine = StartCoroutine(TypeWriter(cleanSentence));
    }

    private IEnumerator TypeWriter(string sentence)
    {
        isTyping = true;
        canContinue = false;
        speechText.text = "";

        Regex tagRegex = new Regex(@"^\{(\w+)\}");
        Match match = tagRegex.Match(sentence);

        if (match.Success)
        {
            string moodTag = match.Groups[1].Value;

            if (moodMap.TryGetValue(moodTag, out int index))
            {
                sprite.sprite = speaker.sprite[index]; // Switch sprite using your array
                sprite.gameObject.SetActive(true);
            }

            sentence = sentence.Substring(match.Length); // Remove tag from dialogue text
        }


        foreach (char letter in sentence)
        {
            speechText.text += letter;
            yield return new WaitForSeconds(0.03f); // Adjust speed here
        }

        isTyping = false;
        canContinue = true;
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speechText.text = currentSentence; // Use the already parsed version
        isTyping = false;
        canContinue = true;
    }

    private void EndSpeech()
    {
        isTalking = false;
        textBox.SetActive(false);
        sprite.gameObject.SetActive(false);
        player.GetComponent<MovementScript>().isTalking = false;
    }

    private string ParseMoodTags(ref string sentence)
    {
        Regex tagRegex = new Regex(@"^\{(\w+)\}");
        Match match;

        while ((match = tagRegex.Match(sentence)).Success)
        {
            string moodTag = match.Groups[1].Value;

            if (moodMap.TryGetValue(moodTag, out int index))
            {
                sprite.sprite = speaker.sprite[index];
                sprite.gameObject.SetActive(true);
            }

            sentence = sentence.Substring(match.Length);
        }

        return sentence;
    }

    void ShowNextSentence()
    {
        if (speechInt >= speech.Length)
        {
            EndSpeech();
            return;
        }

        string raw = speech[speechInt]; // Don't increment yet
        currentSentence = ParseMoodTags(ref raw);
        typingCoroutine = StartCoroutine(TypeWriter(currentSentence));
        speechInt++; // Only now increment — after you've used this line
    }

}
