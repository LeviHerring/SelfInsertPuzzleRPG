using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    float speed = 10f;
    bool isVerticleHeld = false;
    bool isHorizontalHeld = false;
    public bool isTalking; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isTalking == false)
        {
            Move();
        }
       
    }

    void Move()
    {
        if(Input.GetKey(KeyCode.W) && isHorizontalHeld == false)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);
            isVerticleHeld = true; 
        }
        if (Input.GetKey(KeyCode.S) && isHorizontalHeld == false)
        {
            transform.position = new Vector2(transform.position.x,transform.position.y + -speed * Time.deltaTime);
            isVerticleHeld = true; 
        }
        if (Input.GetKey(KeyCode.A) && isVerticleHeld == false)
        {
           transform.position = new Vector2(transform.position.x + -speed * Time.deltaTime, transform.position.y);
            isHorizontalHeld = true; 
        }
        if (Input.GetKey(KeyCode.D) && isVerticleHeld == false)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
            isHorizontalHeld = true; 
        }


        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            isVerticleHeld = false;
        }
        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            isHorizontalHeld = false;
        }
    }
}
