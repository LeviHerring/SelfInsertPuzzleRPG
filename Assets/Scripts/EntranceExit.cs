using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceExit : MonoBehaviour
{
    public EntranceExit target;
    GameObject player;
    bool canTP;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(canTP == true)
        {
            player.transform.position = target.transform.position;
            target.canTP = false;
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canTP = true; 
    }
}
