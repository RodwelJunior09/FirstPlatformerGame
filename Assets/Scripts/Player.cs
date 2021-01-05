using UnityEngine;

public class Player : MonoBehaviour
{
    // Config Variables
    [SerializeField] float playerSpeed = 10f;

    // Cache components references
    Rigidbody2D myridigBody2D;
    Animator myAnimator;

    private void Start()
    {
        myridigBody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        Run();
        FlipSprite();
    }

    private void Run()
    {
        float flowControl = Input.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(flowControl * playerSpeed, myridigBody2D.velocity.y);
        myridigBody2D.velocity = playerVelocity;
        bool playerHorizontalSpeed = Mathf.Abs(myridigBody2D.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRunning", playerHorizontalSpeed);
    }

    private void FlipSprite()
    {
        bool playerHorizontalSpeed = Mathf.Abs(myridigBody2D.velocity.x) > Mathf.Epsilon;
        if (playerHorizontalSpeed)
        {
            float rotation = Mathf.Sign(myridigBody2D.velocity.x);
            if (rotation == 1) transform.eulerAngles = new Vector3(0, 0, 0);
            if (rotation == -1) transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
