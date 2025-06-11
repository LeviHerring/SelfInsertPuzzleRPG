using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public abstract class DialogueSystemParent : MonoBehaviour
{
    protected string currentCharacterName;
    protected string currentSentence;
    protected int speechInt;
    protected bool isTalking;
    protected bool isTyping;
    protected bool canContinue;
    protected Coroutine typingCoroutine;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speechText;
    public Image sprite;

    protected Dictionary<string, int> moodMap = new Dictionary<string, int>
    {
        { "idle", 0 },
        { "happy", 1 },
        { "sad", 2 },
        { "angry", 3 },
        { "shocked", 4 }
    };

    protected struct ParsedDialogueLine
    {
        public string cleanedText;
        public int moodIndex;
    }

    protected ParsedDialogueLine ParseDialogueLine(string rawLine)
    {
        Regex tagRegex = new Regex(@"^\{(\w+)\}");
        Match match = tagRegex.Match(rawLine);

        int moodIndex = 0;
        if (match.Success && moodMap.TryGetValue(match.Groups[1].Value, out int index))
        {
            moodIndex = index;
            rawLine = rawLine.Substring(match.Length);
        }

        return new ParsedDialogueLine
        {
            cleanedText = rawLine,
            moodIndex = moodIndex
        };
    }

    protected IEnumerator TypeWriter(string sentence)
    {
        isTyping = true;
        canContinue = false;
        speechText.text = "";

        foreach (char letter in sentence)
        {
            speechText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
        canContinue = true;
    }

    public abstract void StartDialogue();
    protected abstract void ShowNextSentence();
}
