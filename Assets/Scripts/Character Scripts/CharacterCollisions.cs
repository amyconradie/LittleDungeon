using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCollisions : MonoBehaviour
{
    public Game game;
    public CharacterStats cStats;
    public MazeVisual mazeVisual;
    bool keyDown;
    int level;

    public int mushroomValue = 2;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        level = game.levelNumber;
        cStats = gameObject.GetComponent<CharacterStats>();
        mazeVisual = GameObject.Find("Level").GetComponent<MazeVisual>();
    }

    private void Update()
    {
        keyDown = Input.GetKeyDown(KeyCode.E);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if character is an enemy, damage other enemies and the player on contact
        CollideWithEnemy(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DoTriggerStuff(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        DoTriggerStuff(other);
    }


    // Get states from FSM
    void DoTriggerStuff(Collider2D other)
    {
        if (cStats.isPlayer) // if player collides with objects
        {
            GoToNextLevel(other.gameObject, keyDown);
            CanConsumeObject(other.gameObject, keyDown);
        }
        else // if AI collides with objects
        {
            switch (GetComponent<EnemyAI>().currentState)
            {
                case EnemyAI.State.Hungry:
                    //if hungry and collision object is a mushroom or a steak
                    if (other.gameObject.tag == "Mushroom" || other.gameObject.tag == "Steak")
                    {
                        CanConsumeObject(other.gameObject, true);
                    }                    
                    break;
                case EnemyAI.State.Hurt:
                    //if hurt and collision object is a potion
                    if (other.gameObject.tag == "Health Potion")
                    {
                        CanConsumeObject(other.gameObject, true);
                    }
                    break;
                default:
                    // do nothing
                    break;
            }
        }
    }

    void CollideWithEnemy(GameObject gameObject) // this is for the ai characters
    {
        if (gameObject.tag == "Enemy" || gameObject.tag == "Player")
        {
            // enemy will eat 1 when colliding with characters,
            // but other enemies will eat from it too
            if (!cStats.isPlayer)
            {
                // damage other
                CharacterStats otherCStats = gameObject.GetComponent<CharacterStats>();
                otherCStats.TakeDamage(1);

                // if collide with other character remove some health
                cStats.Eat(1);

                // play sound effect
                FindObjectOfType<AudioManager>().Play("bite");
            }
        }
    }

    void GoToNextLevel(GameObject gameObject, bool inputCondition) // touch stairs go to next level
    {
        StartCoroutine(NextLevel(gameObject, inputCondition));
    }

    private IEnumerator NextLevel(GameObject gameObject, bool inputCondition)
    {
        if (gameObject.tag == "Stairs" && inputCondition)
        {
            var originalTime = Time.timeScale;
            Time.timeScale = 0f;

            // play sound effect
            FindObjectOfType<AudioManager>().Play("steps");
            yield return new WaitForSecondsRealtime(2);
            Time.timeScale = originalTime;

            switch (level)
            {
                case 1:
                    SceneManager.LoadSceneAsync("Level_2", LoadSceneMode.Single);
                    break;
                case 2:
                    SceneManager.LoadSceneAsync("Level_3", LoadSceneMode.Single);
                    break;
                case 3:
                    SceneManager.LoadSceneAsync("Level_4", LoadSceneMode.Single);
                    break;
                case 4:
                    SceneManager.LoadSceneAsync("Level_5", LoadSceneMode.Single);
                    break;
                case 5:
                    SceneManager.LoadSceneAsync("Level_6", LoadSceneMode.Single);
                    break;
                case 6:
                    SceneManager.LoadSceneAsync("Level_7", LoadSceneMode.Single);
                    break;
                case 7:
                    SceneManager.LoadSceneAsync("Level_7", LoadSceneMode.Single);
                    break;
                default:
                    //if something weird happens just go to level 1
                    SceneManager.LoadSceneAsync("Level_1", LoadSceneMode.Single);
                    break;
            }
        }
    }


    void CanConsumeObject(GameObject consumable, bool inputCondition)
    {
        if (inputCondition)
        {
            switch (consumable.tag)
            {
                case "Health Potion":
                    RemoveItemFromList(mazeVisual.potionObjects, consumable);

                    // Destroy object.
                    Destroy(consumable);


                    // heal all hearts
                    cStats.Heal(cStats.getMaxHearts() * cStats.heartSegments);


                    // play sound effect
                    FindObjectOfType<AudioManager>().Play("pickupItem");
                    break;

                case "Mushroom":
                    RemoveItemFromList(mazeVisual.mushroomObjects, consumable);
                    RemoveItemFromList(mazeVisual.foodObjects, consumable);

                    // Destroy object.
                    Destroy(consumable);

                    // decrease hunger by 1
                    cStats.Eat(mushroomValue);


                    // play sound effect
                    FindObjectOfType<AudioManager>().Play("pickupItem");
                    break;

                case "Steak":
                    
                    RemoveItemFromList(mazeVisual.steakObjects, consumable);
                    RemoveItemFromList(mazeVisual.foodObjects, consumable);

                    // Destroy object.
                    Destroy(consumable);

                    // character becomes completely full when they eat a steak
                    cStats.Eat((int)cStats.full);

                    // play sound effect
                    FindObjectOfType<AudioManager>().Play("pickupItem");
                    break;

                default:
                    // do nothing
                    break;
            }
        }
    }



    //Remove item from game
    void RemoveItemFromList(List<GameObject> gameObjectList, GameObject gameObject)
    {
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (gameObjectList[i] == gameObject)
            {
                // Remove from list.
                gameObjectList.Remove(gameObject);
            }
        }
    }

}
