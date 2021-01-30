using UnityEngine;


[CreateAssetMenu(menuName = "Enemy path config")]

public class EnemyPathConfig : ScriptableObject
{
    [SerializeField] Enemy _enemyPrefab;

    public Enemy GetEnemy => _enemyPrefab;
}
