using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region parameter
    private Rigidbody rb;
    private Material material;
    private GameObject[] pickUps;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreTextUI;
    public GameObject gameOverUI;

    // forTest
    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthDrainPerSecond;//life loss rate
    [SerializeField] private float healthGainPerPickup;//life gain rate    
    [SerializeField] private float healthLossOnHit = 1f;
    [SerializeField] private float forceReaction;

    // forCalculation
    private readonly int highestScore = 9999999;//score limit
    private float movementX;
    private float movementY;
    private int score;
    private int loopNum;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        material = GetComponent<Renderer>().material;
        score = 0;
        loopNum = 1;
        currentHealth = maxHealth;

        SetScoreText();
        if (gameOverUI != null) gameOverUI.SetActive(false);
        pickUps = GameObject.FindGameObjectsWithTag("PickUp");
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new (movementX, 0.0f, movementY);
        rb.AddForce(movement * moveSpeed);

        currentHealth -= healthDrainPerSecond * Time.deltaTime;        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        //Debug.Log(currentHealth);
        UpdateColorByHealth();
        if (!IsAlive()) EndGame();
    }

    #region rigidbody
    // collision
    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Pomper" || tag == "HarmfulCube")
        {
            PlayAudio(collision.gameObject);

            // calculate the collision point
            ContactPoint contact = collision.contacts[0];
            Vector3 direction = (transform.position - contact.point).normalized;
            rb.AddForce(direction * forceReaction);

            if (tag == "HarmfulCube")
            {
                currentHealth -= healthLossOnHit; // health loss
            }
        }
    }

    //collection
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            PlayAudio(other.gameObject);
            HideObjectVisuals(other.gameObject);

            score += 10 * loopNum;
            currentHealth += healthGainPerPickup;
            SetScoreText();

            float delay = GetClipLength(other.gameObject);
            StartCoroutine(DisableAfterSound(other.gameObject, delay));            
        }
    }
    #endregion

    #region method
    private void PlayAudio(GameObject obj)
    {
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
    private float GetClipLength(GameObject obj)
    {
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        return (audioSource != null && audioSource.clip != null) ? audioSource.clip.length : 0f;
    }
    private void HideObjectVisuals(GameObject obj)
    {
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
    private IEnumerator DisableAfterSound(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        if (AllPickUpsCollected())
        {
            NextLoop();
        }
    }
    private void UpdateColorByHealth()
    {        
        Color color = material.color;
        color.a = Mathf.Clamp01(currentHealth / maxHealth); // health down, transparent down
        material.color = color;
    }
    private void NextLoop()
    {
        loopNum++;
        transform.position = new Vector3(0, 0, -8f); // reset position
        ReactivatePickUps(); // reset pickUps
        SetScoreText(); // change loopNum
    }
    private void ReactivatePickUps()
    {
        foreach (GameObject pickUp in pickUps)
        {
            pickUp.SetActive(true);
            MeshRenderer mesh = pickUp.GetComponent<MeshRenderer>();
            if (mesh != null) mesh.enabled = true;
            Collider col = pickUp.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }
    }

    // judge
    private bool IsAlive()
    {
        return currentHealth > (maxHealth / 15);
    }
    private bool AllPickUpsCollected()
    {
        foreach (GameObject pickUp in pickUps)
        {
            if (pickUp.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    // UI
    private void SetScoreText()
    {
        if (score >= highestScore)
        {
            score = highestScore;
            scoreTextUI.color = Color.blue;
        }
        scoreTextUI.text = "score: " + score.ToString()
            + "\n" + "X " + loopNum.ToString();// Magnification
    }
    private void EndGame()
    {
        Time.timeScale = 0; // stop the game
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // show game over UI
        }
    }
    #endregion
}
