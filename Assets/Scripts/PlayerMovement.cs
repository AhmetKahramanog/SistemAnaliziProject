using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    private float moveSpeed;
    private Vector2 input;
    private Animator anim;
    private Rigidbody2D playerRB;
    [SerializeField] private float jumpForce;
    [SerializeField] private TextMeshProUGUI goldText;
    private int jumpCount;
    private int totalGold;
    private UserManager userManager;
    void Start()
    {
        moveSpeed = walkSpeed;
        anim = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody2D>();
        userManager = FindObjectOfType<UserManager>();
        totalGold = userManager.lastSaveGold;
        if (goldText != null)
        {
            goldText.text = totalGold.ToString();
        }
    }
    void Update()
    {
        Movement();
        Jump();
    }
    private void Movement()
    {
        input.x = Input.GetAxis("Horizontal");
        var moveDirection = new Vector2(input.x, 0f);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        var isMove = Mathf.Abs(input.x) > 0f;
        anim.SetBool("IsMove", isMove);
        if (input.x > 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (input.x < -0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            jumpCount++;
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0f);
            playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetBool("IsJump", true);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            anim.SetBool("IsJump", false);
            jumpCount = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICollectable collactable))
        {
            if (collactable == null) { return; }
            collactable.Interact(this);
        }
    }
    public void GetGold(int amount)
    {
        totalGold += amount;
        goldText.text = totalGold.ToString();
        SaveGoldToDatabase();
    }
    private void SaveGoldToDatabase()
    {
        if (userManager != null)
        {
            userManager.ScoreSave(totalGold);
        }
    }
}
