using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;

    GameObject cellObject;

    public bool IsVisited { get; private set; }
    public bool hasObject { get; private set; }

    private void Start()
    {
        InitializeWalls();
    }

    void InitializeWalls()
    {
        GenerateNewWall(_leftWall.transform);
        GenerateNewWall(_frontWall.transform);
        GenerateNewWall(_backWall.transform);
        GenerateNewWall(_rightWall.transform);
    }

    void GenerateNewWall(Transform wallParent)
    {
        GameObject newWall = WallData.Instance.getRandomWall();
        if (wallParent.childCount >= 1)
            Destroy(wallParent.GetChild(0));
        Instantiate(newWall, wallParent);
    }

    public void Visit()
    {
        IsVisited = true;

    }

    public void SpawnCellObject(GameObject spawnObject, float posMultiplier)
    {
        if(!hasObject && spawnObject != null)
        {
            hasObject = true;
            Vector3 pos = transform.position * posMultiplier;
            cellObject = spawnObject;
            if (spawnObject.gameObject.tag != "Portal")
            {
                pos.y = 0.8f;
                cellObject = Instantiate(spawnObject, pos, Quaternion.identity);
            }
            else
            {
                pos.y = 0.5f;
                cellObject.transform.position = pos;
            }
        }
    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }

    public void Reset()
    {
        _frontWall.SetActive(true);
        _backWall.SetActive(true);
        _rightWall.SetActive(true);
        _leftWall.SetActive(true);
        hasObject = false;
        IsVisited = false;

        if (cellObject != null && cellObject.gameObject.tag != "Portal")
            Destroy(cellObject);
        
    }
}
