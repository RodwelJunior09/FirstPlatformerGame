using TMPro;
using UnityEngine;

public class DisplayStatus : MonoBehaviour
{
    LifeStatus gameStatus;
    TextMeshProUGUI textLife;

    // Start is called before the first frame update
    void Start()
    {
        gameStatus = FindObjectOfType<LifeStatus>();
        textLife = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textLife.text = gameStatus.GetPlayerHealth().ToString();
    }
}
