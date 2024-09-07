using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallData", menuName = "Maze/WallData")]
public class WallData : ScriptableObject
{

    [SerializeField]
    GameObject[] wallPrefabs;

    [Range(0, 1)]
    public float normalWallPercentage = 0.6f;

    private static WallData _instance;

    public static WallData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<WallData>("WallData");
            }
            return _instance;
        }
    }

    public GameObject getRandomWall()
    {
        float wallType = Random.Range(0f, 1f);
        if (wallType < normalWallPercentage)
            return wallPrefabs[0];
        else
            return wallPrefabs[Random.Range(1, wallPrefabs.Length)];
    }
}
