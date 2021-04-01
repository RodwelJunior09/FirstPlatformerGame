using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] bool keepSpawning;
    [SerializeField] float minSpawnDelay = 0f;
    [SerializeField] float maxSpawnDelay = 5f;

    //[SerializeField] Attacker _enemyPrefab;
    [SerializeField] Enemy[] _enemiesPrefabs;

    private float speed = 3.5f;

    public void StopSpawn() => keepSpawning = false;

    public IEnumerator SetEnemies()
    {
        var randomNumber = Random.Range(minSpawnDelay, maxSpawnDelay);
        SpawnEnemies();
        yield return new WaitForSeconds(randomNumber);
    }

    public void SpawnEnemies()
    {
        var randomIndex = Random.Range(0, _enemiesPrefabs.Length - 1);
        Spawn(randomIndex);
    }

    private void Spawn(int index)
    {
        speed -= 0.5f;
        Enemy attacker = Instantiate(_enemiesPrefabs[index], transform.position, transform.rotation) as Enemy;
        attacker.enemySpeed = speed;
        attacker.GetComponent<Enemy>().patrol = true;
        attacker.transform.localScale = new Vector2(-1f, 1f);
        attacker.transform.parent = transform;
    }
}
