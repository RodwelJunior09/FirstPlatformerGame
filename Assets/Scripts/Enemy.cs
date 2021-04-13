using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Serialize Variables
    [Header("Enemy Attributes")]
    [SerializeField] int health = 100;
    [SerializeField] public float enemySpeed = 1f;

    [Header("Enemy Attack Settings")]
    [SerializeField] int enemyAttackValue = 30;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float maxAttackTime = 5f;
    [SerializeField] float minAttackTime = 1f;

    [Header("Other Settings")]
    [SerializeField] public bool patrol = false;
    [SerializeField] Transform attackPoint;

    [Header("Boss Variables")]
    [SerializeField] float restingSeconds = 10f;
    [SerializeField] int limitHelperEnemies = 3;

    // Components Variables
    Animator _myAnimator;
    BoxCollider2D _myFeet;
    Rigidbody2D _myRigidBody2D;
    BoxCollider2D _myEnemyVision;
    CapsuleCollider2D _enemyBody;
    CircleCollider2D _myEnemyAttackRadius;

    // Local Variables
    private float attackCounter;
    private float durationOfDissapearing = 3f;
    
    // Boss variables
    private string tagEnemy;
    private Vector2 enemyVelocity;
    private bool crouching = false;
    private bool enemyStage = false;
    private bool isBossAlive = true;
    private int helperEnemiesCount = 0;
    private bool isOnRightSide = false;
    private bool isOnLeftSide = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetCounters();
        tagEnemy = gameObject.tag;
        _myAnimator = GetComponent<Animator>();
        _myFeet = GetComponent<BoxCollider2D>();
        _myRigidBody2D = GetComponent<Rigidbody2D>();
        _enemyBody = GetComponent<CapsuleCollider2D>();
        _myEnemyVision = transform.Find("FieldOfView").GetComponent<BoxCollider2D>();
        _myEnemyAttackRadius = transform.Find("FieldOfView").GetComponent<CircleCollider2D>();
        _myFeet.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndAttack();
        if (!tagEnemy.Contains("Boss")) 
            IsEnemyVisible();
        if(tagEnemy.Contains("Boss")) 
            EnemyStageTransition();
    }

    void FixedUpdate()
    {
        EnemyWalkPattern();
    }

    public void GotHit(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
        else _myAnimator.SetTrigger("IsHit");
    }

    public void EnemyAwareness(Transform playerPosition)
    {
        if (playerPosition.localScale.x < 0 && transform.localScale.x > 0)
        {
            if (tagEnemy.Contains("Boss"))
                transform.localScale = new Vector2(-1.5f, 1.5f);
            else
                FlipSprite();
        }
        if (playerPosition.localScale.x > 0 && transform.localScale.x < 0)
        {
            if (tagEnemy.Contains("Boss"))
                transform.localScale = new Vector2(1.5f, 1.5f);
            else
                FlipSprite();
        }
    }
    
    void EnemyStageTransition()
    {
        if (health <= 250 && !enemyStage)
            StartCoroutine(RestingAnimation());
    }

    public bool BossDied() => isBossAlive;

    // Maybe a refactor down the line. Looks kinda crowded I don't like it.
    IEnumerator RestingAnimation()
    {
        SpawnHelperEnemies();
        yield return new WaitForSeconds(restingSeconds);
        StopSpawningHelperEnemies();
    }

    void SpawnHelperEnemies()
    {
        if (helperEnemiesCount <= limitHelperEnemies)
        {
            crouching = true;
            _myAnimator.SetBool("IsResting", crouching);
            FindObjectOfType<Spawner>().SpawnEnemies();
            DisableEnemyColliders();
            helperEnemiesCount++;
        }
    }


    void StopSpawningHelperEnemies()
    {
        crouching = false;
        _myAnimator.SetBool("IsResting", crouching);
        enemyStage = true;
        ActivateColliders();
    }

    void IsEnemyVisible()
    {
        var playerLayer = LayerMask.GetMask("Player");
        if (!_myEnemyVision.IsTouchingLayers(playerLayer)) 
            _myAnimator.SetBool("IsEnemyVisible", false);
        if (_myEnemyVision.IsTouchingLayers(playerLayer))
            _myAnimator.SetBool("IsEnemyVisible", true);
    }

    void CountDownAndAttack()
    {
        attackCounter -= Time.deltaTime;
        if (attackCounter <= 0f && !crouching)
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
            ApproachThePlayer();
            if(!tagEnemy.Contains("Boss")) 
                patrol = true;
        }
        if (isRangeOfVision && isRangeOfAttack)
            StopRunning();
        if (!isRangeOfAttack && !isRangeOfVision && patrol)
            EnemyPatrol();
    }

    void StopRunning()
    {
        _myRigidBody2D.velocity = Vector2.zero; // Stop running
        _myAnimator.SetBool("IsRunning", false);
    }

    void ApproachThePlayer()
    {
        var playerPos = FindObjectOfType<Player>().transform;
        ChangeOrientationOfMovement(playerPos);
        _myAnimator.SetBool("IsRunning", true);
        if (transform.localScale.x > 0)
            enemyVelocity = new Vector2(-enemySpeed, _myRigidBody2D.velocity.y);
        else
            enemyVelocity = new Vector2(enemySpeed, _myRigidBody2D.velocity.y);
        _myRigidBody2D.velocity = enemyVelocity;
    }

    void ChangeOrientationOfMovement(Transform playerTransform)
    {
        if (transform.position.x < playerTransform.position.x)
        {
            if (!isOnRightSide)
            {
                FlipSprite();
                isOnRightSide = true;
                isOnLeftSide = false;
            }
        }
        if (transform.position.x > playerTransform.position.x)
        {
            if (!isOnLeftSide)
            {
                FlipSprite();
                isOnLeftSide = true;
                isOnRightSide = false;
            }
        }
    }

    void EnemyPatrol()
    {
        _myAnimator.SetBool("IsRunning", true);
        if (IsFacingRight())
            _myRigidBody2D.velocity = new Vector2(-enemySpeed, 0f);
        else
            _myRigidBody2D.velocity = new Vector2(enemySpeed, 0f);
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    public void FlipSprite(bool crouchingAnimation = false)
    {
        if (tagEnemy.Contains("Boss"))
            transform.localScale = new Vector2(Mathf.Sign(_myRigidBody2D.velocity.x) + 0.5f, 1.5f);
        else
            transform.localScale = new Vector2(Mathf.Sign(_myRigidBody2D.velocity.x), 1f);
    }

    void Die()
    {
        DisableEnemyColliders();
        _myAnimator.SetBool("IsDead", true);
        if (tagEnemy.Contains("Boss"))
            FindObjectOfType<LevelManager>().LoadWinLevel();
        Destroy(gameObject, durationOfDissapearing);
    }

    void SetMovementSpeed(float speedInput)
    {
        enemySpeed = speedInput;
    }

    void DisableEnemyColliders()
    {
        patrol = false;
        _myFeet.enabled = true; // This is for the enemy falling off the world when dying.
        _myEnemyVision.enabled = false;
        _enemyBody.enabled = false;
        _myEnemyAttackRadius.enabled = false;
    }

    void ActivateColliders()
    {
        _myEnemyVision.enabled = true;
        _enemyBody.enabled = true;
        _myEnemyAttackRadius.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
