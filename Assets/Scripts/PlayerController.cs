using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject gameOverObject;
    public float Life = 20;
    public float GameTime = 0;
    
    public int shrink_PerTime = 0;
    public int expand_PickUp = 0;
    public int shrink_Impact = 0;

    private Rigidbody rb;
    private float movementX;
    private float movementY;

    private Vector3 shrink = new Vector3(0.05f, 0.05f, 0.05f);//

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        SetCountText();
        winTextObject.SetActive(false);
        gameOverObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText(){//how to win
        countText.text = "Time Left: " + GameTime.ToString("f0") + " s";
        
        if(Life <= 0){
            gameOverObject.SetActive(true);
            GameOver();
        }
        else if(GameTime <= 0){
            winTextObject.SetActive(true);
            GameOver();
        }
    }

    void FixedUpdate()//move
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        GameTime -= Time.deltaTime;
        
        Life -= shrink_PerTime*Time.deltaTime;
        rb.transform.localScale -= shrink_PerTime*Time.deltaTime*shrink;//shrink by time

        SetCountText();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("PickUp")){
            other.gameObject.SetActive(false);

            if((Life + expand_PickUp) < 20){
                Life += expand_PickUp;//life++
                rb.transform.localScale += expand_PickUp*shrink;//grow
            }
            else{
                Life = 20;
                rb.transform.localScale = new Vector3(2, 2, 2);
            }

        }
    }

    private void OnCollisionEnter(Collision other) {
       if(other.gameObject.CompareTag("Magma")){
            //other.gameObject.SetActive(false);
            Life -= shrink_Impact;//life--
            rb.transform.localScale -= shrink_Impact*shrink;//shrink

        } 
    }

    private void GameOver(){
        Time.timeScale = 0;
    }
}
