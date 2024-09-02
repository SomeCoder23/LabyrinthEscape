using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Enemy enemyPrefab;
    public Pill[] pills;
    public Collectable diamondPrefab;
    public int initialEnemyCount;
    public int initialPillsCount;
    [Tooltip("The value enemyCount will be multiplied by for the next levels")]
    public float enemyMultiplier = 2;
    [Tooltip("The value pillsCount will be multiplied by for the next levels")]
    public float pillsMultiplier = 1.5f;
    public int level { get; private set; }

    int diamondCount;
    int enemiesSpawned = 0, pillsSpawned = 0, emptyCellsCount, diamondSpawned = 0;
    int pillsCount = 0, enemyCount = 0;

    private void Start()
    {
        level = 1;
    }

    public void CalculateEmptyCells(int mazeCells)
    {
        emptyCellsCount = (mazeCells - (enemyCount + pillsCount) - 1);
        diamondCount = emptyCellsCount / 2;
    }

    public GameObject getRandomObject()
    {
        if (enemiesSpawned == enemyCount && pillsSpawned == pillsCount)
            return diamondPrefab.gameObject;

        int maxNumber = emptyCellsCount > 0 ? 3 : 2;
        int ranObj = Random.Range(0, maxNumber);
        return getObject(ranObj);

    }

    GameObject getObject(int ranObject)
    {
        switch (ranObject)
        {
            case 0:
                if (enemyCount == enemiesSpawned)
                    return getObject(1);
                enemiesSpawned++;
                return enemyPrefab.gameObject;

            case 1:
                if (pillsCount == pillsSpawned)
                    return getObject(0);
                pillsSpawned++;
                return pills[Random.Range(0, pills.Length)].gameObject;

            default:
                emptyCellsCount--;
                if (diamondSpawned < diamondCount)
                {
                    diamondSpawned++;
                    return diamondPrefab.gameObject;
                }
                else return null;
        }
    }

    public void UpdateSpawnCounts()
    {
        enemyCount = (int)(enemyMultiplier * level/* * initialEnemyCount*/);
        pillsCount = (int)(pillsMultiplier * level/* * initialPillsCount*/);
        level++;
        enemiesSpawned = 0;
        pillsSpawned = 0;
    }

    public void ResetValues()
    {
        level = 0;
        UpdateSpawnCounts();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject item in objs)
            Destroy(item);
    }
}
