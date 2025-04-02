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
    
    public float maxLife;
    public float currentLife;
    [SerializeField] private int deathRate = 0;
    public float forceReaction;

    private readonly int highestScore = 999;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pomper") || collision.gameObject.CompareTag("HarmfulCube"))
        {
            // 获取碰撞点
            ContactPoint contact = collision.contacts[0];
            Vector3 collisionPoint = contact.point;

            // 计算反作用力的方向和大小
            Vector3 direction = (transform.position - collisionPoint).normalized;
            Vector3 force = direction * forceReaction;

            // 施加反作用力
            rb.AddForce(force);
            if (collision.gameObject.CompareTag("HarmfulCube"))
            {
                currentLife -= 1f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            Destroy(other.gameObject);
            score += 100 * scoreRate;
            currentLife += 1f;
            SetScoreText();
        }
    }

    void SetScoreText()
    {
        if (score >= highestScore)
        {
            score = highestScore;
            scoreTextUI.color = Color.red;
        }
        scoreTextUI.text = "score: " + score.ToString();
    }

    void HealthCalculation()
    {
        currentLife -= deathRate * Time.deltaTime;
        Color color = material.color;
        color.a = Mathf.Clamp01(currentLife / maxLife); // 假设selfLife的最大值为20
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
}
