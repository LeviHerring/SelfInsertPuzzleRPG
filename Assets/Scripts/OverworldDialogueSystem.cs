using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class OverworldDialogueSystem : MonoBehaviour
{
    public string[] speech;
    public string characterName;
    public GameObject textBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speechText;
    public int speechInt;
    bool isTalking;
    public GameObject player; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTalking) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (speechInt < speech.Length)
            {
                speechText.text = speech[speechInt++];
            }
            else
            {
                EndSpeech();
            }
        }
    }

    public void Speech()
    {
        textBox.SetActive(true); 
        isTalking = true;
        speechInt = 0;
        nameText.text = characterName;  
        speechText.text = speech[speechInt]; 
    }

    void EndSpeech()
    {
        isTalking = false;
        textBox.SetActive(false);
        player.GetComponent<MovementScript>().isTalking = false; 
    }

    public void StartDialogue(CharacterDialogue characterDialogue)
    {
        textBox.SetActive(true);
        isTalking = true;
        speechInt = 0;
        nameText.text = characterDialogue.characterName;
        speechText.text = characterDialogue.speech[speechInt];
        speech = characterDialogue.speech; 
    }
}
