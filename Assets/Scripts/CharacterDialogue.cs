using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CharacterDialogue : MonoBehaviour
{
    public string[] speech;
    public string characterName; 
    public OverworldDialogueSystem overworldDialogueSystem;
    bool canTalkTo;
    bool isTalking;
    GameObject player; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canTalkTo && isTalking == false && player != null)
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                if (player.GetComponent<MovementScript>().isTalking == false)
                {
                    overworldDialogueSystem.StartDialogue(this); 
                    player.GetComponent<MovementScript>().isTalking = true;
                    isTalking = true;
                }
               
               
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject; 
            canTalkTo = true; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canTalkTo = false;
        isTalking = false;
        //player.GetComponent<MovementScript>().isTalking = false; 
    }
}
