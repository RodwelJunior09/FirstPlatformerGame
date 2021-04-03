using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private string musicPlayerTag;
    private void Awake()
    {
        int musicPlayerCount = FindObjectsOfType<MusicPlayer>().Length;

        if (musicPlayerCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            musicPlayerTag = FindObjectOfType<MusicPlayer>().tag;
            DontDestroyOnLoad(gameObject);
        }

    }
}
