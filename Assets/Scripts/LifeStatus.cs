using TMPro;
using UnityEngine;

public class LifeStatus : MonoBehaviour
{
    TextMeshProUGUI textLife;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        textLife = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textLife.text = player.GetHealth().ToString();
    }
}
