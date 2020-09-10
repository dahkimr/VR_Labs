using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed, jumpForce;
    public Text timerText, winText, countText;
    public Camera mainCam, fpCam;


    private Rigidbody rb;
    private float time, finishTime, distToGround;
    private Vector3 jump;
    private string min, sec;
    private int count;
    private bool gameEnded;
    private int numOfPickUps;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        jump = new Vector3(0.0f, 1.0f, 0.0f);
        count = 0;
        numOfPickUps = GameObject.FindGameObjectsWithTag("Pick Up").Length;
        winText.text = "";
        countText.text = "Items Left: " + numOfPickUps;
        gameEnded = false;
        mainCam.enabled = true;
        fpCam.enabled = false;
    }

	void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        
        rb.AddForce(movement * speed);
    }

    void Update()
    {
        if (!gameEnded)
        {
            time = Time.time;

            min = ((int)time / 60).ToString();
            sec = (time % 60).ToString("f1");

            timerText.text = min + ":" + sec;
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (mainCam.enabled)
            {
                mainCam.enabled = false;
                fpCam.enabled = true;
            }
            else
            {
                mainCam.enabled = true;
                fpCam.enabled = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
        else if (other.gameObject.CompareTag("Goal"))
        {
            CheckWin();
            if (gameEnded)
            {
                other.gameObject.SetActive(false);
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void CheckWin()
    {
        if (count >= numOfPickUps)
        {
            gameEnded = true;
            winText.text = "You Win!";
        }
    }

    void SetCountText()
    {
        countText.text = "Items Left: " + (numOfPickUps - count);
    }
}
