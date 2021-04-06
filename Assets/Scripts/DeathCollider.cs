using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<LevelManager>().LoadGameOver();
    }
}
