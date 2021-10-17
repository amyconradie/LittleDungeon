using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{

    public MazeVisual mazeVisual;
    public GameObject player;

    public HeartsVisual heartsVisual;
    public HungerBar hungerBar;

    public GameObject steakPrefab;
    public GameObject corpsePrefab;

    public bool isPlayer;
    public bool isDead;
    public int heartSegments = 2;
    public int maxHearts;
    public int currentHearts;
    public int maxHeartSegments;
    public int currentHeartSegments;
    public float full;
    public float currentHunger;
    public float maxSpeed = 50f;
    public float moveSpeed = 10f;

    //hunger increase timer
    float lastHungerUpdateTime = 0f;

    void Start()
    {
        // create reference to maze objects
        mazeVisual = GameObject.Find("Level").GetComponent<MazeVisual>();
        player = GameObject.Find("Player");

        // heart bar
        heartsVisual.SetUpHeartsVisual(currentHearts, maxHearts);
        maxHeartSegments = maxHearts * heartSegments;

        // hungerbar
        hungerBar = GameObject.Find(transform.name).GetComponentInChildren<HungerBar>();
        hungerBar.setMaxHunger(10f);
        full = 10f;
        hungerBar.setHunger(10f);
        //this.hungerBar.GetComponent<Slider>().value = 10f;
    }

    private void Update()
    {
        currentHeartSegments = heartsVisual.currentHeartSegments;
        currentHunger = hungerBar.GetComponent<Slider>().value;
        if (heartsVisual.heartSystem.IsDead())
        {
            if (currentHeartSegments >= 0)
            { 
                Die(); 
            }
            else // if there is no player and there are no enemies left, zoom out to show full maze
            {

                GameObject mainCam = GameObject.Find("Main Camera");
                Camera camera = mainCam.GetComponent<Camera>();
                CameraTarget cameraTarget = mainCam.GetComponent<CameraTarget>();

                // set camera focus to center tile
                List<GameObject> floors = GameObject.Find("Level").GetComponent<MazeVisual>().floorGameObjects;
                int center = floors.Count / 2;
                cameraTarget.setCharacter(floors[center].transform);

                //center maze
                cameraTarget.setCentered(true);

                // zoom out to show entire maze
                // multiply maze width or height by 5 to fit whole maze in frame
                camera.orthographicSize = mazeVisual.mazeSize * 5;
            }
        }

        IncreaseHunger();
    }

    // Health Functions
    //------------------

    public void TakeDamage(int value)
    {
        heartsVisual.heartSystem.DamageHearts(value);

        if (heartsVisual.heartSystem.IsDead())
        {
            Die();
        }
    }

    public void Heal(int value)
    {
        heartsVisual.heartSystem.HealHearts(value);
    }


    // Death Functions
    //-----------------

    public void Die()
    {
        
        isDead = true;

        Animator anim = GetComponent<Animator>();
        anim.SetBool("dead", true);

        GetComponent<AIAnimation>().enabled = false;

        // create a steak
        SpawnSteak();

        if (!isPlayer)
        {
            // Delete Object
            FindAndDestroyListItem(mazeVisual.enemyGameObjects, gameObject);
        }
        else
        {
            // Delete Object
            Destroy(gameObject);

        }

        //play audio clip
        FindObjectOfType<AudioManager>().Play("characterDeath");

        GameObject mainCam = GameObject.Find("Main Camera");
        Camera camera = mainCam.GetComponent<Camera>();
        CameraTarget cameraTarget = mainCam.GetComponent<CameraTarget>();

        //set camera focus target
        if (isPlayer) // if the character that just died is the player
        {
            // follow the first living enemy
            GameObject enemies = GameObject.Find("Enemies");
            GameObject firstEnemy = enemies.transform.GetChild(0).gameObject;
            cameraTarget.setCharacter(firstEnemy.transform);
        }

        if (GameObject.Find("Player") == null && mazeVisual.enemyGameObjects.Count >= 0) // if no player but an enemy still exist, focus on the first enemy in the enemies list
        {
            // follow the first living enemy
            GameObject enemies = GameObject.Find("Enemies");
            GameObject firstEnemy = enemies.transform.GetChild(0).gameObject;
            cameraTarget.setCharacter(firstEnemy.transform);
        }


        if (GameObject.Find("Player") == null && mazeVisual.enemyGameObjects.Count == 0)
        {
            // set camera focus to center tile
            List<GameObject> floors = GameObject.Find("Level").GetComponent<MazeVisual>().floorGameObjects;
            int center = floors.Count / 2;
            cameraTarget.setCharacter(floors[center].transform);

            //center maze
            cameraTarget.setCentered(true);

            // zoom out to show entire maze
            // multiply maze width or height by 5 to fit whole maze in frame
            camera.orthographicSize = mazeVisual.mazeSize * 5; 
        }
        
    }


    void SpawnSteak()
    {
        GameObject steak = Instantiate(steakPrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -Mathf.Abs(0.3f)), Quaternion.identity).gameObject;
        mazeVisual.steakObjects.Add(steak);
        mazeVisual.foodObjects.Add(steak);
        steak.transform.parent = GameObject.Find("Steaks").transform;
        steak.name = "steak" + " pos: (" + gameObject.transform.position.x + ", " + gameObject.transform.position.y + ")";
    }


    //// Spawn A dead body where character has died
    //void SpawnCorpse()
    //{
    //    GameObject corpse = Instantiate(corpsePrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -Mathf.Abs(0.01f)), Quaternion.identity).gameObject;
    //    mazeVisual.steakObjects.Add(corpse);
    //    corpse.transform.parent = GameObject.Find("Corpses").transform;
    //    corpse.name = "corpse" + " pos: (" + gameObject.transform.position.x + ", " + gameObject.transform.position.y + ")";
    //}


    // Hunger Functions
    //------------------

    public void IncreaseHunger(float value)
    {
        setHunger(getHunger() - value);
        if (getHunger() == 0)
        {
            heartsVisual.heartSystem.DamageHearts(heartSegments); // remove a full heart
            if (currentHeartSegments > 0)
            {
                setHunger(full);
            }
        }
    }

    public void Eat(float value)
    {
        if (getHunger() == full)
        {
            
            setHunger(full);
        }
        else
        {
            float oldHunger = getHunger();
            setHunger(getHunger() + value);
            if (oldHunger < full && getHunger() == full)
            {
                // if just became full, heal 1 heart
                Heal(getHeartSegments()); 
            }
        }
    }

    void IncreaseHunger()
    {
        lastHungerUpdateTime += Time.deltaTime;
        if (lastHungerUpdateTime >= 6f) // increase hunger every 6 seconds
        {
            IncreaseHunger(1);
            lastHungerUpdateTime = 0;
        }
    }

    // other functions
    //-----------------

    //Remove item from game
    public void FindAndDestroyListItem(List<GameObject> gameObjectList, GameObject gameObject)
    {
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (gameObjectList[i] == gameObject)
            {
                // Remove from list.
                gameObjectList.Remove(gameObject);
                // Destroy object.
                Destroy(gameObject);
            }
        }
    }


    // Getters and Setters
    //---------------------

    public bool getAiStatus() { return isPlayer; }

    public int getHeartSegments() { return heartSegments; }

    public int getMaxHearts() { return maxHearts; }

    public void setMaxHearts(int maxHearts)
    {
        if (maxHearts > 0 && maxHearts <= 10)
            this.maxHearts = maxHearts;
        else if (maxHearts <= 0)
            this.maxHearts = 1;
        else if (maxHearts > 10)
            this.maxHearts = 10;
    }

    public float getHunger()
    {
        if (this.hungerBar != null)
        {
            return this.hungerBar.GetComponent<Slider>().value;
        }
        else
        {
            return -1f;
        }
    }
    public float getHealth()
    {
        return currentHeartSegments;
    }

    public float getMaxHealth()
    {
        return maxHeartSegments;
    }

    public void setHunger(float currentHunger)
    {
        if (currentHunger > full)
            this.hungerBar.GetComponent<Slider>().value = full;
        else if (currentHunger < 0)
            this.hungerBar.GetComponent<Slider>().value = 0f;
        else
            this.hungerBar.GetComponent<Slider>().value = currentHunger;
    }

    public float getSpeed() { return moveSpeed; }

}
