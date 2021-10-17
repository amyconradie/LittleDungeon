using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // references to game variables
    [SerializeField] Transform target;
    MazeVisual mazeVisual;
    GameObject player;
    PathfindingSettings pathSettings;
    CharacterStats cStats;

    // for checking if character gets stuck
    public float timeSinceLastCall;
    Vector3 lastPosition = Vector3.zero;

    // States for fsm
    public enum State { Roaming, ChasePlayer, Hungry, Hurt }
    public State currentState;

    // hunger
    float hungry;


    private void Awake()
    {
        currentState = State.Roaming; // default state is roaming
    }


    void Start()
    {
        // get references to in-game elements
        player = GameObject.Find("Player");
        mazeVisual = GameObject.Find("Level").GetComponent<MazeVisual>();
        pathSettings = GetComponent<PathfindingSettings>();
        cStats = player.GetComponent<CharacterStats>();

        // set hunger amount for when ai needs to start looking for food
        hungry = (cStats.full / 4)*3; // gets hungry at 75% of the hungerBar's maxvalue

        // set starting target
        target = mazeVisual.RandomEmptySpace().transform; ; //random target
    }


    void Update()
    {
        RoamingTargetTimedOut(); // check that character isn't stuck in 1 position

        switch (currentState)
        {
            case State.Roaming:
                Roam();
                break;

            case State.ChasePlayer:
                ChasePlayer();
                break;

            case State.Hungry:
                FindFood();
                break;

            case State.Hurt:
                FindHealthPotion();
                break;

            default:
                break;
        }
        

        // make sure target is updated
        // ... just making double sure
        pathSettings.target = target;
    }


    // State Functions
    //-----------------

    private void Roam()
    {
        // if player exists
        if (player != null)
        {
            //check if player nearby
            DetectPlayer();
        }

        // check if injured
        IsInjured();

        // check if hungry
        IsHungry();

        // Priority 4. Continue Roaming
        // check if character has reached target position
        float reachedRoamPosition = 1f;
        if (Vector3.Distance(transform.position, target.position) < reachedRoamPosition) 
        {
            SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
        }
    }


    private void ChasePlayer()
    {
        // if player exists
        if (player != null)
        {
            // check if player is still within range
            ForgetPlayer();

            // make sure target is the player
            SetTarget(player.transform, State.ChasePlayer);
        }
        else
        {
            // if no player, start roaming again
            SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
        }

    }


    private void FindFood()
    {
        // make sure not full
        if (cStats.getHunger() <= hungry) // if hunger at % or less
        {
            //check if there is still food
            if (mazeVisual.foodObjects.Count > 0)
            {
                TargetNearest(mazeVisual.foodObjects, State.Hungry);
            }
            else
            {
                // if no food, start roaming
                SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
            }
        }
        else
        {
            // if full, start roaming
            SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
        }
    }


    private void FindHealthPotion()
    {

        bool notMaxHealth = cStats.getHealth() < cStats.getMaxHealth() && cStats.getHealth() != cStats.getMaxHealth();
        // make sure not at full health
        if (notMaxHealth)
        {
            if (mazeVisual.potionObjects.Count > 0 && notMaxHealth)
            {
                TargetNearest(mazeVisual.potionObjects, State.Hurt);
            }
            else
            {
                SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
            }
        }
        else
        {
            // if healed, start roaming
            SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
        }
    }
   

    // state switching functions
    //---------------------------

    private void DetectPlayer()
    {
        float targetRange = 10f;

        //player in range, target player
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < targetRange)
        {
            ChasePlayer();
        }
    }

    private void ForgetPlayer()
    {
        float targetRange = 30f; // greater distance needed to shake enemy off

        // Player out of range, get new roaming target
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < targetRange)
        {
            SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
        }
    }

    private void IsInjured()
    {
        if (cStats.currentHeartSegments < cStats.maxHeartSegments) // max hearts * number of segments in a heart = total health
        {
            FindHealthPotion();
        }
    }

    void IsHungry()
    {
        if (cStats.getHunger() <= hungry)
        {
            FindFood();
        }
    }


    // Targeting Functions
    //---------------------

    // for when the ai gets stuck on things
    void RoamingTargetTimedOut()
    {
        timeSinceLastCall += Time.deltaTime;
        if (timeSinceLastCall >= 2f) // if stuck for x seconds, select a new roaming target
        {
            if (gameObject.transform.position == lastPosition)
            {
                SetTarget(mazeVisual.RandomEmptySpace().transform, State.Roaming);
            }

            lastPosition = gameObject.transform.position;
            timeSinceLastCall = 0;
        }
    }
    

    // Basic Functions
    //-----------------

    void SetTarget(Transform target, State state)
    {
        this.target = target;
        pathSettings.target = target;
        currentState = state;
    }

    private void TargetNearest(List<GameObject> gameObjectList, State state)
    {
        // Find nearest item.
        GameObject nearest = null;
        float distance = 0;

        for (int i = 0; i < gameObjectList.Count; i++)
        {
            float tempDistance = Vector3.Distance(transform.position, gameObjectList[i].transform.position);
            if (nearest == null || tempDistance < distance)
            {
                nearest = gameObjectList[i];
                distance = tempDistance;
            }
        }

        SetTarget(nearest.transform, state);
    }


}
