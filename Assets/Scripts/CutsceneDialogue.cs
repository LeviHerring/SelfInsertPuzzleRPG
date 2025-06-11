using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class CutsceneDialogue : DialogueSystemParent
{
    public TextAsset dialogueTextFile;
    public GameObject dialogueUI;
    [SerializeField]
    public Dictionary<string, Sprite[]> characterSprites;

    private List<DialogueLine> dialogueLines = new List<DialogueLine>();
    private int lineIndex = 0;

    [System.Serializable]
    private struct DialogueLine
    {
        public string speakerName;
        public string text;
        public int moodIndex;

        public DialogueLine(string speakerName, string text, int moodIndex)
        {
            this.speakerName = speakerName;
            this.text = text;
            this.moodIndex = moodIndex;
        }
    }

    void Start()
    {
        ParseTextFile();
        StartDialogue();
    }

    public override void StartDialogue()
    {
        if (dialogueLines.Count == 0)
        {
            Debug.LogWarning("No dialogue lines found!");
            return;
        }

        lineIndex = 0;
        dialogueUI.SetActive(true);
        ShowNextSentence();
    }

    protected override void ShowNextSentence()
    {
        if (lineIndex >= dialogueLines.Count)
        {
            EndCutscene();
            return;
        }

        DialogueLine line = dialogueLines[lineIndex++];
        currentCharacterName = line.speakerName;
        nameText.text = currentCharacterName;
        currentSentence = line.text;

        // Set speaker sprite
        if (characterSprites.TryGetValue(line.speakerName, out Sprite[] sprites))
        {
            if (line.moodIndex >= 0 && line.moodIndex < sprites.Length)
            {
                sprite.sprite = sprites[line.moodIndex];
                sprite.gameObject.SetActive(true);
            }
        }

        typingCoroutine = StartCoroutine(TypeWriter(currentSentence));
    }

    private void ParseTextFile()
    {
        if (!dialogueTextFile) return;

        string[] lines = dialogueTextFile.text.Split('\n');
        Regex pattern = new Regex(@"^(\w+):\s*""(.+?)""(?:\s*\{(\w+)\})?");

        foreach (string line in lines)
        {
            Match match = pattern.Match(line.Trim());
            if (match.Success)
            {
                string speaker = match.Groups[1].Value;
                string text = match.Groups[2].Value;
                string mood = match.Groups[3].Success ? match.Groups[3].Value : "idle";
                int moodIndex = moodMap.TryGetValue(mood, out int index) ? index : 0;

                dialogueLines.Add(new DialogueLine(speaker, text, moodIndex));
            }
        }
    }

    private void EndCutscene()
    {
        dialogueUI.SetActive(false);
        Debug.Log("Cutscene finished — transition to puzzle or overworld.");

        // You could replace this with:
        // SceneManager.LoadScene("PuzzleScene");
        // Or invoke any event/callback to continue game flow.
    }

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

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speechText.text = currentSentence;
        isTyping = false;
        canContinue = true;
    }
}
