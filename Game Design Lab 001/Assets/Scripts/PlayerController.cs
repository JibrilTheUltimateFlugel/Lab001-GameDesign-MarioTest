using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for UI stuff
using UnityEngine.SceneManagement; // to transition to Game Over Scene

public class PlayerController : MonoBehaviour
{
    // Variables
    public float speed;
    private Rigidbody2D marioBody;
    public float maxSpeed = 10;
    public float upSpeed;

    // Boolean to check if mario is on the Ground
    private bool onGroundState = true;

    // For Flipping Mario Sprite
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    // Scoring Variables
    public Transform enemyLocation;
    public Text scoreText;
    private int score = 0; // initial score = 0
    private bool countScoreState = false;

    // Sound Effects
    AudioSource marioJumpSFX;

    // Game Over and Restart
    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene"); // go to the next scene which is the Game Over Scene
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        // Obtain the Mario sprite by getting the SpriteRenderer component from mario object
        marioSprite = GetComponent<SpriteRenderer>();
        marioJumpSFX = GetComponent<AudioSource>();

    }

    void FixedUpdate()
    {
        // dynamic rigidbody
        float moveHorizontal = Input.GetAxis("Horizontal"); // get input value from A and D keys
        if (Mathf.Abs(moveHorizontal) > 0) // check if there is movement in horizontal direction
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            if (marioBody.velocity.magnitude < maxSpeed) // check if magnitude of the velocity of the mario rigid body has not exceeded maxSpeed
                marioBody.AddForce(movement * speed);
        }

        if (Input.GetKeyUp("a") || Input.GetKeyUp("d")) // check if key is released
        {
            // stop
            marioBody.velocity = Vector2.zero;
        }

        // Jumping Logic
        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioJumpSFX.Play();
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false; // Mario is now in the air until he lands back on the ground
            countScoreState = true; // Mario is now jumping and we need to check if the goomba is underneath
        }
    }

    // Update is called once per frame, does not involve Physics
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        // Flipping Logic - does not require physics
        if (Input.GetKeyDown("a") && faceRightState) //If A key is pressed and Mario is facing right, change the sprite into facing left by enabling flipX, set faceRightState to false
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState) //IF D key is pressed and Mario is facing left (not facing right), change the sprite into facing right and disable the flipX and set the faceRightstate to True
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }

        // when jumping, and Goomba is near Mario and we have not registered our score
        if (!onGroundState && countScoreState)
        {
            if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f)
            {
                countScoreState = false;
                score++;
                Debug.Log(score);
            }
        }

    }

    // Detect collision with the Ground
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            onGroundState = true; // back on ground
            countScoreState = false;
            scoreText.text = "Score: " + score.ToString(); // display added score to UI
        }
    }

    // OnTriggerEnter, triggered when Mario collider collides with the Goomba collider which is a Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) // if mario collides with an "Enemy"
        {
            Debug.Log("Collided with Gomba!"); // print
            GameOver();
        }
    }


}
