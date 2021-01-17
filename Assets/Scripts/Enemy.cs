using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Serialize Variables
    [SerializeField] int health = 100;
    [SerializeField] float enemySpeed = 1f;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 2f;
    [SerializeField] int attackEnemy = 30;
    [SerializeField] float attackRate = 10f;
    // Components Variables
    Animator _myAnimator;
    Rigidbody2D _myRigidBody2D;
    BoxCollider2D _myEnemyVision;
    CircleCollider2D _myEnemyAttackRadius;
    // Local Variables
    private float durationOfDissapearing = 5f;

    // Start is called before the first frame update
    void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _myRigidBody2D = GetComponent<Rigidbody2D>();
        _myEnemyVision = GetComponentInChildren<BoxCollider2D>();
        _myEnemyAttackRadius = GetComponentInChildren<CircleCollider2D>();
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
            // Set animation
            StartCoroutine(HitPlayer(playerLayer));
        }
    }

    private IEnumerator HitPlayer(int layerPlayer)
    {
        // Set animation
        _myAnimator.SetTrigger("IsAttacking");

        // Look for collision
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, layerPlayer);

        foreach (var colliderEnemy in hitEnemies)
        {
            colliderEnemy.GetComponent<Player>().DamageTaken(attackEnemy);
        }

        yield return new WaitForSeconds(attackRate);
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
        _myAnimator.SetBool("IsDead", true);
        Destroy(gameObject, durationOfDissapearing);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
