using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MazeVisual : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] floorPrefabs;
    public GameObject[] wallPrefabs;
    public GameObject[] consumables;
    public GameObject[] enemyPrefabs;
    public GameObject stairPrefab;

    public int mazeSize, enemyCount, mushroomCount, potionCount;

    public enum wallTypes { cleanWalls, mossyWalls, both };
    public wallTypes wallType;

    public enum floorTypes { cleanFloors, mossyFloors, both };
    public floorTypes floorType;

    public enum enemyTypes { skull, oni, ogre, croc, frog, cat, all };
    public enemyTypes enemyType;

    private MazeGenerator mazeGenerator;
    private int[,] intMaze;

    private float tileWidth, tileHeight;
    private Vector3 xyzStart;


    public List<GameObject> floorGameObjects, wallGameObjects, enemyGameObjects, mushroomObjects, steakObjects, foodObjects, potionObjects, goals, OpenSpaces;

    System.Random _random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {

        this.mazeGenerator = new MazeGenerator();


        // make sure maze scale is an is odd number
        if (mazeSize % 2 == 0)
        {
            mazeSize += 1;
        }

        if (mazeSize > 23)
        {
            mazeSize = 23;
        }

        this.intMaze = mazeGenerator.CreateMaze(enemyCount, mushroomCount, potionCount, mazeSize);
        floorGameObjects = new List<GameObject>();
        wallGameObjects = new List<GameObject>();
        mushroomObjects = new List<GameObject>();
        steakObjects = new List<GameObject>();
        foodObjects = new List<GameObject>();
        potionObjects = new List<GameObject>();

        enemyGameObjects = new List<GameObject>();
        goals = new List<GameObject>();
        OpenSpaces = new List<GameObject>();

        CreateMaze();

    }


    public int getMazeSize() { return mazeSize; }

    public void setMazeSize(int mazeSize)
    {
        if (mazeSize % 2 == 0)
        {
            mazeSize += 1;
        }

        this.mazeSize = mazeSize;
    }

    public int getEnemyCount() { return enemyCount; }

    public void setEnemyCount(int enemyCount)
    {
        if (enemyCount > 0 && enemyCount < 20)
        {
            this.enemyCount = enemyCount;
        }
    }

    public void setMushroomCount(int mushroomCount)
    {
        if (mushroomCount > 0 && mushroomCount < 20)
        {
            this.mushroomCount = mushroomCount;
        }
    }

    public void setPotionCount(int potionCount)
    {
        if (potionCount > 0 && potionCount < 10)
        {
            this.potionCount = potionCount;
        }
    }

    public void setEnemyType(int e)
    {
        enemyType = (enemyTypes)e;
    }

    void SetTileScale(GameObject go)
    {
        Vector3 extents = go.GetComponent<Renderer>().bounds.extents;
        tileWidth = extents.x + extents.x;
        tileHeight = extents.y + extents.y;
    }

    void SetTileStartPosition(GameObject go)
    {
        Vector3 xyz = go.transform.position;
        Vector3 extents = go.GetComponent<Renderer>().bounds.extents;

        //make sure bottomLeft corner lines up with (0,0) 
        float xCorner = xyz.x;// + (tileWidth / 2);
        float yCorner = xyz.y;// + (tileHeight / 2);

        xyzStart.x = xCorner - ((mazeSize / 2) * tileWidth); 
        xyzStart.y = yCorner - ((mazeSize / 2) * tileHeight); 
        xyzStart.z = 0.0f;
    }


    void CreateMaze()
    {

        setMazeSize(mazeSize);

        SetTileScale(floorPrefabs[0]);
        SetTileStartPosition(floorPrefabs[0]);


        CreateFloors();
        CreateWalls();

        CreateEnemies();
        CreateMushrooms();
        CreatePotions();

        //make a gameobject to store steaks in when they appear
        GameObject Steaks = new GameObject();
        Steaks.transform.parent = GameObject.Find("Level").transform;
        Steaks.name = "Steaks";

        //make a gameobject to store steaks in when they appear
        GameObject Corpses = new GameObject();
        Corpses.transform.parent = GameObject.Find("Level").transform;
        Corpses.name = "Corpses";

        CreateGoal();

    }

    void CreateFloors()
    {
        GameObject FloorTiles = new GameObject();
        FloorTiles.transform.parent = GameObject.Find("Level").transform;
        FloorTiles.name = "Floor Tiles";

        float xTemp = xyzStart.x, yTemp = xyzStart.y;

        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                int random;
                switch (floorType)
                {
                    case floorTypes.cleanFloors:
                        random = _random.Next(0, 1);
                        InstantiateObject(floorPrefabs[random], xTemp, yTemp, 0, i, j, "floor_tile", "Floor Tiles", floorGameObjects);
                        break;

                    case floorTypes.mossyFloors:
                        random = _random.Next(1, floorPrefabs.Length);
                        InstantiateObject(floorPrefabs[random], xTemp, yTemp, 0, i, j, "floor_tile", "Floor Tiles", floorGameObjects);
                        break;

                    case floorTypes.both:
                        random = _random.Next(0, floorPrefabs.Length);
                        InstantiateObject(floorPrefabs[random], xTemp, yTemp, 0, i, j, "floor_tile", "Floor Tiles", floorGameObjects);
                        break;

                    default:
                        break;
                }

                xTemp += tileWidth;
            }

            yTemp += tileHeight;
            xTemp = xyzStart.x;
        }
    }

    void CreateWalls()
    {
        GameObject WallTiles = new GameObject();
        WallTiles.transform.parent = GameObject.Find("Level").transform;
        WallTiles.name = "Wall Tiles";

        float xTemp = xyzStart.x, yTemp = xyzStart.y;
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (intMaze[i, j] == 1)
                {
                    int random;

                    switch (wallType)
                    {
                        case wallTypes.cleanWalls:
                            random = _random.Next(0, 4); // cleean tiles
                            InstantiateObject(enemyPrefabs[random], xTemp, yTemp, 0.1f, i, j, "wall_tile", "WallTiles", wallGameObjects);
                            break;

                        case wallTypes.mossyWalls:
                            random = _random.Next(4, 8); // mossy tiles
                            InstantiateObject(wallPrefabs[random], xTemp, yTemp, 0.1f, i, j, "wall_tile", "WallTiles", wallGameObjects);
                            break;

                        case wallTypes.both:
                            random = _random.Next(0, 8); // all wall tiles
                            InstantiateObject(wallPrefabs[random], xTemp, yTemp, 0.1f, i, j, "wall_tile", "Wall Tiles", wallGameObjects);
                            break;

                        default:
                            break;
                    }

                }
                else
                {
                    OpenSpaces.Add(floorGameObjects[i * mazeSize + j]);
                }

                xTemp += tileWidth;
            }

            yTemp += tileHeight;
            xTemp = xyzStart.x;
        }

    }

    public GameObject RandomEmptySpace()
    {
        int random = _random.Next(0, OpenSpaces.Count);
        return OpenSpaces[random];
    }
  
    void CreateEnemies()
    {
        GameObject Enemies = new GameObject();
        Enemies.transform.parent = GameObject.Find("Level").transform;
        Enemies.name = "Enemies";

        float xTemp = xyzStart.x, yTemp = xyzStart.y;
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (intMaze[i, j] == 2)
                {
                    int random;

                    switch (enemyType)
                    {
                        case enemyTypes.skull:
                            InstantiateObject(enemyPrefabs[0], xTemp, yTemp, 0.4f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;                                             
                        case enemyTypes.oni:                                   
                            InstantiateObject(enemyPrefabs[1], xTemp, yTemp, 0.4f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;                                             
                        case enemyTypes.ogre:                                  
                            InstantiateObject(enemyPrefabs[2], xTemp, yTemp, 0.4f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;                                           
                        case enemyTypes.croc:                                
                            InstantiateObject(enemyPrefabs[3], xTemp, yTemp, 0.4f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;                                             
                        case enemyTypes.frog:                                  
                            InstantiateObject(enemyPrefabs[4], xTemp, yTemp, 0.4f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;                                             
                        case enemyTypes.cat:                                   
                            InstantiateObject(enemyPrefabs[5], xTemp, yTemp, 0.4f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;
                        case enemyTypes.all:
                            random = _random.Next(0, enemyPrefabs.Length); // all enemies
                            InstantiateObject(enemyPrefabs[random], xTemp, yTemp, 0.1f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;
                        default:
                            random = _random.Next(0, enemyPrefabs.Length); // all enemies
                            InstantiateObject(enemyPrefabs[random], xTemp, yTemp, 0.1f, i, j, "enemy", "Enemies", enemyGameObjects);
                            break;
                    }
                }
                xTemp += tileWidth;
            }
            yTemp += tileHeight;
            xTemp = xyzStart.x;
        }
    }


    
    void CreateMushrooms()
    {
        GameObject mushroom = new GameObject();
        mushroom.transform.parent = GameObject.Find("Level").transform;
        mushroom.name = "Mushrooms";

        float xTemp = xyzStart.x, yTemp = xyzStart.y;
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (intMaze[i, j] == 3)
                {
                    GameObject go = InstantiateObject(consumables[1], xTemp, yTemp, 0.2f, i, j, "mushroom", "Mushrooms", mushroomObjects);
                    foodObjects.Add(go);
                }

                xTemp += tileWidth;
            }
            yTemp += tileHeight;
            xTemp = xyzStart.x;
        }

    }

    void CreatePotions()
    {
        GameObject potions = new GameObject();
        potions.transform.parent = GameObject.Find("Level").transform;
        potions.name = "Potions";

        float xTemp = xyzStart.x, yTemp = xyzStart.y;
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (intMaze[i, j] == 4)
                {
                    InstantiateObject( consumables[0], xTemp, yTemp, 0.2f, i, j, "potion", "Potions", potionObjects);
                }

                xTemp += tileWidth;
            }
            yTemp += tileHeight;
            xTemp = xyzStart.x;
        }
    }

    void CreateGoal()
    {
        GameObject stairs = new GameObject();
        stairs.transform.parent = GameObject.Find("Level").transform;
        stairs.name = "Stairs";

        float xTemp = xyzStart.x, yTemp = xyzStart.y;
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (intMaze[i, j] == 5)
                {
                    InstantiateObject(this.stairPrefab, xTemp, yTemp, 0.2f, i, j, "stairs", "Stairs", goals);
                }

                xTemp += tileWidth;
            }
            yTemp += tileHeight;
            xTemp = xyzStart.x;
        }
    }

    GameObject InstantiateObject(
        GameObject gameObject, 
        float x, float y, float z, 
        int i, int j, 
        string gameObjectName, string parentGameObject, 
        List<GameObject> objectList)
    {
        
        GameObject gameObject_clone = Instantiate(gameObject, new Vector3(x, y, -Mathf.Abs(z)), Quaternion.identity).gameObject;

        gameObject_clone.transform.parent = GameObject.Find(parentGameObject).transform;

        gameObject_clone.name = gameObjectName + " index: (" + i+", " + j+ "), pos: ("+x+", "+y+")";

        objectList.Add(gameObject_clone);


        return gameObject_clone;

    }

}
