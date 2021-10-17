using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    [Header("Camera Settings")]
    public GameObject mainCam;
    public Game game;
    //public GameObject compositingEffects;
    public bool postProcessingOn = true;
    public float cameraDistance = 40f;
    private CameraTarget followCharacter;
    private Transform following;

    [Header("Character Settings")]
    public GameObject player;
    CharacterStats pStats;

    int enemyCount, mushroomCount, potionCount;
    int currentEnemyType; // 0-6, 6 different enemies + all enemies combined
    int levelNumber;
    public int maxHearts = -1;

    MazeVisual mazeVisual;
    public int mazeSize;
    int[,] tilePositions;

    public Vector2 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        player = InstantiateObject(player, "Player", "Level", player.transform.position.x, player.transform.position.y, player.transform.position.z);
        pStats = player.GetComponent<CharacterStats>();
        mazeVisual = gameObject.GetComponent<MazeVisual>();

        game = GameObject.Find("Game").GetComponent<Game>();
        levelNumber = game.levelNumber;
        pStats.setMaxHearts(levelNumber);
        this.maxHearts = pStats.maxHearts;

        mainCam = GameObject.Find("Main Camera");
        SetUpCamera(player, cameraDistance, false);

        mazeVisual.setMazeSize(mazeSize);
        mazeVisual.setEnemyCount(enemyCount);
        mazeVisual.setMushroomCount(mushroomCount);
        mazeVisual.setPotionCount(potionCount);
        mazeVisual.setEnemyType(currentEnemyType);

        // Player Spawn
        Vector2 playerSpawnPoint = new Vector2();
        playerSpawnPoint.x = player.transform.position.x;
        playerSpawnPoint.y = player.transform.position.y;
        SetSpawnPosition(player, playerSpawnPoint);

    }

    void FixedUpdate() // ~50x per frame
    {
        UpdatePlayerPosition();
    }

    void UpdatePlayerPosition()
    {
        if (player != null)
        {
            playerPos.x = player.transform.position.x;
            playerPos.y = player.transform.position.y;
        }
    }

    public void SetGameStats(int maxHearts, int enemyType, int mazeSize, int enemyCount, int mushroomCount, int potionCount)
    {
        if (this.maxHearts == -1)
        {
            this.maxHearts = maxHearts;
        }

        currentEnemyType = enemyType;
        this.mazeSize = mazeSize;
        this.enemyCount = enemyCount;
        this.mushroomCount = mushroomCount;
        this.potionCount = potionCount;
    }

    public void CopyStats(CharacterStats cStats)
    {
        cStats = pStats;
    }

    void SetSpawnPosition(GameObject spawnObject, Vector2 spawnPoint)
    {
        spawnObject.transform.position = new Vector3(spawnPoint.x, spawnPoint.y, spawnObject.transform.position.z);
    }


    void SetUpCamera(GameObject player, float cameraDistance, bool centered)
    {
        followCharacter = mainCam.GetComponent<CameraTarget>();
        following = player.transform;
        followCharacter.setCharacter(following);
        this.cameraDistance = cameraDistance;
        mainCam.GetComponent<UnityEngine.Camera>().orthographicSize = ((Screen.height / 2) / cameraDistance);
        followCharacter.setCentered(centered);
    }

    GameObject InstantiateObject( GameObject gameObjectToClone, string gameObjectName, string parentGameObject, float x, float y, float z )
    {
        GameObject gameObject_clone = Instantiate(gameObjectToClone, new Vector3(x, y, -Mathf.Abs(z)), Quaternion.identity).gameObject;
        gameObject_clone.transform.parent = GameObject.Find(parentGameObject).transform;
        gameObject_clone.name = gameObjectName;

        return gameObject_clone;
    }
}
