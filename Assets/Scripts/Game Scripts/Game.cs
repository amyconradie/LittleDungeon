using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject mainCam;
    public GameObject levelPrefab;

    [Header("Game Settings")]
    public int levelNumber;
    public int mazeSize;
    public int currentEnemyType;
    public int enemyCount, mushroomCount, potionCount;
    public int maxHearts;

    void Start()
    {
        GameObject level = InstantiateObject(levelPrefab, "Level", "Game", 0, 0, 0);
        mainCam = InstantiateObject(mainCam, "Main Camera", "Game", mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z);
        level.GetComponent<Level>().SetGameStats(maxHearts, currentEnemyType, mazeSize, enemyCount, mushroomCount, potionCount);
    }

    GameObject InstantiateObject(GameObject gameObjectToClone, string gameObjectName, string parentGameObject, float x, float y, float z)
    {
        GameObject gameObject_clone = Instantiate(gameObjectToClone, new Vector3(x, y, -Mathf.Abs(z)), Quaternion.identity).gameObject;
        gameObject_clone.transform.parent = GameObject.Find(parentGameObject).transform;
        gameObject_clone.name = gameObjectName;

        return gameObject_clone;
    }


}
