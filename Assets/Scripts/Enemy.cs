using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Serialize Variables
    [Header("Enemy Attributes")]
    [SerializeField] int health = 100;
    [SerializeField] float enemySpeed = 1f;

    [Header("Enemy Attack Settings")]
    [SerializeField] int enemyAttackValue = 30;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float maxAttackTime = 5f;
    [SerializeField] float minAttackTime = 1f;

    [Header("Other Settings")]
    [SerializeField] bool patrol = false;
    [SerializeField] Transform attackPoint;

    // Components Variables
    Animator _myAnimator;
    Rigidbody2D _myRigidBody2D;
    BoxCollider2D _myEnemyVision;
    CapsuleCollider2D _enemyBody;
    CircleCollider2D _myEnemyAttackRadius;

    // Local Variables
    private float attackCounter;
    private float durationOfDissapearing = 3f;

    // Start is called before the first frame update
    void Start()
    {
        ResetCounters();
        _myAnimator = GetComponent<Animator>();
        _myRigidBody2D = GetComponent<Rigidbody2D>();
        _enemyBody = GetComponent<CapsuleCollider2D>();
        _myEnemyVision = GetComponentInChildren<BoxCollider2D>();
        _myEnemyAttackRadius = GetComponentInChildren<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        IsEnemyVisible();
        CountDownAndAttack();
    }

    void FixedUpdate()
    {
        EnemyWalkPattern();
    }

    void IsEnemyVisible()
    {
        var playerLayer = LayerMask.GetMask("Player");
        if (!_myEnemyVision.IsTouchingLayers(playerLayer)) _myAnimator.SetBool("IsEnemyVisible", false);
        if (_myEnemyVision.IsTouchingLayers(playerLayer))
        {
            _myAnimator.SetBool("IsEnemyVisible", true);
        }
    }

    public void RandomCrouchAnimation()
    {
        var randomNumber = Random.Range(1, 10);
        if (randomNumber % 2 == 0)
            StartCoroutine(FlipSprite(true)); // Flip sprite with crouching animation
        else
            StartCoroutine(FlipSprite()); // Flip sprite with no crouching animation
    }

    void CountDownAndAttack()
    {
        attackCounter -= Time.deltaTime;
        if (attackCounter <= 0f)
        {
            IsInAttackRange();
            ResetCounters();
        }
    }

    void ResetCounters()
    {
        attackCounter = Random.Range(minAttackTime, maxAttackTime);
    }

    void IsInAttackRange()
    {
        var playerLayer = LayerMask.GetMask("Player");
        if (_myEnemyAttackRadius.IsTouchingLayers(playerLayer))
        {
            // Set animation
            _myAnimator.SetTrigger("IsAttacking");
            var collider = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
            collider.GetComponent<Player>().DamageTaken(enemyAttackValue);
        }
    }

    void EnemyWalkPattern()
    {
        var playerLayer = LayerMask.GetMask("Player");
        var isRangeOfVision = _myEnemyVision.IsTouchingLayers(playerLayer);
        var isRangeOfAttack = _myEnemyAttackRadius.IsTouchingLayers(playerLayer);
        if (!isRangeOfAttack && isRangeOfVision)
        {
            Vector2 enemyRididBody;
            _myAnimator.SetBool("IsRunning", true);
            if (transform.localScale.x > 0)
                enemyRididBody = new Vector2(-enemySpeed, _myRigidBody2D.velocity.y);
            else
                enemyRididBody = new Vector2(enemySpeed, _myRigidBody2D.velocity.y);
            _myRigidBody2D.velocity = enemyRididBody;
        }
        if (isRangeOfVision && isRangeOfAttack)
        {
            _myRigidBody2D.velocity = Vector2.zero; // Stop running
            _myAnimator.SetBool("IsRunning", false);
        }
        if (!isRangeOfAttack && !isRangeOfVision && patrol)
        {
            WalkToWayPoint();
        }
    }

    void WalkToWayPoint()
    {
        _myAnimator.SetBool("IsRunning", true);
        EnemyPatrol();
    }

    void EnemyPatrol()
    {
        if (IsFacingRight())
        {
            _myRigidBody2D.velocity = new Vector2(-enemySpeed, 0f);
        }
        else
        {
            _myRigidBody2D.velocity = new Vector2(enemySpeed, 0f);
        }
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    IEnumerator FlipSprite(bool crouchingAnimtion = false)
    {
        if (crouchingAnimtion)
        {
            _myAnimator.SetTrigger("IsSearching");
            yield return new WaitForSeconds(2);
        }
        transform.localScale = new Vector2(Mathf.Sign(_myRigidBody2D.velocity.x), 1f);
    }

    public void GotHit(int damage)
    {
        health -= damage;
        _myAnimator.SetTrigger("IsHit");
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        DisableEnemyColliders();
        _myAnimator.SetBool("IsDead", true);
        Destroy(gameObject, durationOfDissapearing);
    }

    void SetMovementSpeed(float speedInput)
    {
        enemySpeed = speedInput;
    }

    void DisableEnemyColliders()
    {
        _myEnemyVision.enabled = false;
        _myEnemyAttackRadius.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
