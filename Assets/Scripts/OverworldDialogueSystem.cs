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
        if(Input.GetKeyDown(KeyCode.P) && isTalking == true)
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
        
    }

    public void StartDialogue(CharacterDialogue characterDialogue)
    {

        speechInt = 0;
        speaker = characterDialogue;
        speech = (string[])speaker.speech.Clone();
        characterName = speaker.characterName;

        textBox.SetActive(true);
        sprite.sprite = characterDialogue.sprite[0]; 
        sprite.gameObject.SetActive(true);
        isTalking = true;
        

        nameText.text = characterName;
        speechInt = 0;
        string rawSentence = speech[speechInt];
        Debug.Log($"Starting dialogue for: {characterName}, speech count: {speech.Length}");
        ParsedDialogueLine parsed = ParseDialogueLine(rawSentence);
        sprite.sprite = speaker.sprite[parsed.moodIndex]; // set sprite at start
        currentSentence = parsed.cleanedText;
        typingCoroutine = StartCoroutine(TypeWriter(currentSentence));
        speechInt++;
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
        Nullifier();
        textBox.SetActive(false);
        sprite.gameObject.SetActive(false);
        speechInt = 0;
       
        player.GetComponent<MovementScript>().isTalking = false;
    }

    private string ParseMoodTags(string sentence)
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

        ParsedDialogueLine parsed = ParseDialogueLine(speech[speechInt]);
        speechInt++; // Advance the index immediately

        sprite.sprite = speaker.sprite[parsed.moodIndex];
        currentSentence = parsed.cleanedText;
        typingCoroutine = StartCoroutine(TypeWriter(currentSentence));
    }

    void Nullifier()
    {
        speaker = null; 
        speech = null;
        characterName = null;
        speechText.text = "";
        nameText.text = "";

    }

    public struct ParsedDialogueLine
    {
        public string cleanedText;
        public int moodIndex;

        public ParsedDialogueLine(string cleanedText, int moodIndex)
        {
            this.cleanedText = cleanedText;
            this.moodIndex = moodIndex;
        }
    }

    private ParsedDialogueLine ParseDialogueLine(string rawLine)
    {
        int moodIndex = 0; // Default mood (e.g., idle)

        Regex tagRegex = new Regex(@"^\{(\w+)\}");
        Match match = tagRegex.Match(rawLine);

        if (match.Success)
        {
            string moodTag = match.Groups[1].Value;

            if (moodMap.TryGetValue(moodTag, out int mappedIndex))
            {
                moodIndex = mappedIndex;
            }

            rawLine = rawLine.Substring(match.Length); // Remove the tag
        }

        return new ParsedDialogueLine(rawLine, moodIndex);
    }

}
