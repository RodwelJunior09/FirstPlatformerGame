using UnityEngine;

public class Borders : MonoBehaviour
{ 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            other.GetComponent<Enemy>().RandomCrouchAnimation();
        }
    }
}
