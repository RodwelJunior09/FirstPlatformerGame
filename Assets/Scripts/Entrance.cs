using UnityEngine;

public class Entrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Player"))
        {
            FindObjectOfType<LevelManager>().LoadNextScene();
        }
    }
}
