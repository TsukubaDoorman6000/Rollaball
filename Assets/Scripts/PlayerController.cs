using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Material material;
    public TextMeshProUGUI scoreTextUI;
    public GameObject gameOverUI;

    [SerializeField] private float speed = 0;
    private float movementX;
    private float movementY;

    private int score;
    private int scoreRate;
    
    //public GameObject winTextObject;
    //public GameObject gameOverObject;
    public float maxLife = 20;
    public float currentLife;
    [SerializeField] private int deathRate = 0;
    
    //public int expand_PickUp = 0;
    //public int shrink_Impact = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        material = GetComponent<Renderer>().material;

        score = 0;
        scoreRate = 1;
        currentLife = maxLife;
        SetScoreText();
        if (gameOverUI != null) gameOverUI.SetActive(false);       
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()//move
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        HealthCalculation();
        if (!IsAlive())
        {
            EndGame();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("PickUp")){            
            Destroy(other.gameObject);
            score += 100 * scoreRate;

            SetScoreText();
            //if((Life + expand_PickUp) < 20){
            //    Life += expand_PickUp;//life++
            //    rb.transform.localScale += expand_PickUp*shrink;//grow
            //}
            //else{
            //    Life = 20;
            //    rb.transform.localScale = new Vector3(2, 2, 2);
            //}

        }
    }

    void SetScoreText()
    {
        if (score >= 999)
        {
            score = 999;
        }
        scoreTextUI.text = "score: " + score.ToString();
    }

    void HealthCalculation()
    {
        currentLife -= deathRate * Time.deltaTime;
        Color color = material.color;
        color.a = Mathf.Clamp01(currentLife / 20.0f); // 假设selfLife的最大值为20
        material.color = color;
    }

    bool IsAlive()
    {
        return currentLife > (maxLife / 10);
    }
    public void EndGame()
    {
        Time.timeScale = 0; // 暂停游戏
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // 显示游戏结束UI
        }
    }

    //private void OnCollisionEnter(Collision other) {
    //   if(other.gameObject.CompareTag("Magma")){
    //        //other.gameObject.SetActive(false);
    //        Life -= shrink_Impact;//life--
    //        rb.transform.localScale -= shrink_Impact*shrink;//shrink

    //    } 
    //}
}
