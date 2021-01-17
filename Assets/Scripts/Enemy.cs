using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Serialize Variables
    [SerializeField] float enemySpeed = 1f;

    // Components Variables
    Animator _myAnimator;
    Rigidbody2D _myRigidBody2D;
    BoxCollider2D _myEnemyVision;
    CircleCollider2D _myEnemyAttackRadius;
    // Local Variables
    
    // Start is called before the first frame update
    void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _myRigidBody2D = GetComponent<Rigidbody2D>();
        _myEnemyVision = GetComponent<BoxCollider2D>();
        _myEnemyAttackRadius = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RunToThePlayer();
        IsEnemyVisible();
        IsInAttackRange();
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

    void IsInAttackRange()
    {
        var playerLayer = LayerMask.GetMask("Player");
        if (!_myEnemyAttackRadius.IsTouchingLayers(playerLayer)) _myAnimator.SetBool("IsAttacking", false);
        if (_myEnemyAttackRadius.IsTouchingLayers(playerLayer))
        {
            _myAnimator.SetBool("IsAttacking", true);
        }
    }

    void RunToThePlayer()
    {
        var playerLayer = LayerMask.GetMask("Player");
        var isRangeOfVision = _myEnemyVision.IsTouchingLayers(playerLayer);
        var isRangeOfAttack = _myEnemyAttackRadius.IsTouchingLayers(playerLayer);
        if (!isRangeOfAttack && isRangeOfVision)
        {
            _myAnimator.SetBool("IsRunning", true);
            Vector2 enemyRididBody = new Vector2(-enemySpeed, _myRigidBody2D.velocity.y);
            _myRigidBody2D.velocity = enemyRididBody;
        }
        if (isRangeOfVision && isRangeOfAttack)
        {
            _myRigidBody2D.velocity = Vector2.zero; // Stop running
            _myAnimator.SetBool("IsRunning", false);
        }
        if (!isRangeOfAttack && !isRangeOfVision)
        {
            _myRigidBody2D.velocity = Vector2.zero;
            _myAnimator.SetBool("IsRunning", false);
        }
    }
}
