using UnityEngine;

public class LifeStatus : MonoBehaviour
{
    int playerHealth;

    private void Awake()
    {
        SaveData();
        playerHealth = FindObjectOfType<Player>().GetHealth();
    }

    public void DecreasePlayerHealth(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
            playerHealth = 0;
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }

    public int GetPlayerHealth() => playerHealth;

    private void SaveData()
    {
        int amountOfObjects = FindObjectsOfType<LifeStatus>().Length;
        if (amountOfObjects > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }
}
