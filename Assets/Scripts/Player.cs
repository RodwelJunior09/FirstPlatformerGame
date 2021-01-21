using UnityEngine;

public class Player : MonoBehaviour
{
    // Config Variables
    [Header("Player Info")]
    [SerializeField] int health = 100;
    [SerializeField] float playerSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;

    [Header("Player Attack Variables")]
    [SerializeField] int attackDamage = 30;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float attackRange = 1f;

    [Header("Other Attributes")]
    [SerializeField] Transform attackPoint;

    // Cache components references
    Animator myAnimator;
    Rigidbody2D myridigBody2D;
    BoxCollider2D _myFeetCollider;
    CapsuleCollider2D _bodyCollider;

    // Local variables
    private float timeToDestroy = 5f;
    private float nextAttackTime = 0f;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        myridigBody2D = GetComponent<Rigidbody2D>();
        _myFeetCollider = GetComponent<BoxCollider2D>();
        _bodyCollider = GetComponent<CapsuleCollider2D>();
    }
    void Update()
    {
        Run();
        Jump();
        Falling();
        FlipSprite();
        Attack();
    }

    private void Run()
    {
        float flowControl = Input.GetAxis("Horizontal") * playerSpeed;
        Vector2 playerVelocity = new Vector2(flowControl, myridigBody2D.velocity.y);
        myridigBody2D.velocity = playerVelocity;
        bool playerHorizontalSpeed = Mathf.Abs(myridigBody2D.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRunning", playerHorizontalSpeed);
    }

    private void Jump()
    {
        var ground = LayerMask.GetMask("Ground");
        if (!_myFeetCollider.IsTouchingLayers(ground)) { return; }
        if (Input.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myridigBody2D.velocity += jumpVelocityToAdd;
            myAnimator.SetTrigger("IsJumping");
        }
    }

    private void Falling()
    {
        if (myridigBody2D.velocity.y < -0.1)
        {
            myAnimator.SetBool("IsFalling", true);
        }
        else
        {
            myAnimator.SetBool("IsFalling", false);
        }
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                AttackProcess();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void AttackProcess()
    {
        var enemyLayer = LayerMask.GetMask("Enemy");
        // Activate animation
        myAnimator.SetTrigger("IsAttacking");

        // Look for the collision
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (var colliderEnemy in hitEnemies)
        {
            colliderEnemy.GetComponent<Enemy>().GotHit(attackDamage);
        }
    }

    public void DamageTaken(int damageAmount)
    {
        health -= damageAmount;
        myAnimator.SetTrigger("IsHit");
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        _bodyCollider.enabled = false;

        myAnimator.SetTrigger("IsDead");
        Destroy(gameObject, timeToDestroy);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void FlipSprite()
    {
        bool playerHorizontalSpeed = Mathf.Abs(myridigBody2D.velocity.x) > Mathf.Epsilon;
        if (playerHorizontalSpeed)
        {
            float rotation = Mathf.Sign(myridigBody2D.velocity.x);
            transform.localScale = new Vector3(rotation, 1f, 1f);
        }
    }
}
