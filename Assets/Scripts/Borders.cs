using UnityEngine;

public class Borders : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gameObjectTag = collision.gameObject.tag;
        if (gameObjectTag.Equals("Enemy") || gameObjectTag.Equals("Boss"))
        {
            collision.gameObject.GetComponent<Enemy>().FlipSprite();
        }
    }
}
