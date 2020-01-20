using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Pigeon : MonoBehaviour
{
    public Texture[] myTexture;                             // Defining pigeon oclors texture

    [SerializeField] private GameObject targetPrefab;       // a "null" taget prefab for the navmesh agent
    [SerializeField] private Slider hungerSlider;            // a canvas that visulizes Hanger Rate
    [SerializeField] private float hungerDeterior = 0.01f;  // Hanger deterioration per sec
    [SerializeField] private float coinStandingTime = 0.4f;
    [SerializeField] private float coinMinHungerRate = 0.3f;


    // navigation members
    private NavMeshAgent agent;
    private GameObject target; // helps seting a new target for navmesh
    private float targetReachTime; // helps counting standing time
    private float walkRadius = 12f; // the radius for searching targets
    private float PigeonSpeed; // remebers original agent.speed and caculate relative speeds

    // related assets 
    public Food myFood; //current food to go to
    public Coin coinPrephab;
    //public int coinDropRate = 1000; // a random chance of droping coins while searching food
    public int [] coinTypeRates = new int[] { 10, 20, 50, 100 }; // a random chance of droping coins while searching food
    private bool coinDropped = true; // prevents multiple coin drops at once


    // various rates & measurments
    private float timeSiceLastSpawn;
    private float hungerRate;
    
    
    // states and animations
    private Animator anim;
    private enum State { FLY_IN, WANDER, FOUND_FOOD, EATING, FLY_OUT, FLYING };
    private State myState;
         


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();     
        hungerSlider = GetComponentInChildren<Slider>();
        anim = GetComponentInChildren<Animator>();
        target = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);

        SetupBird();

        SetState( State.FLY_IN);  
 
    }




    void Update()
    {
        hungerRate = Mathf.Clamp(hungerRate, -0.5f, 1.2f);
        NavAnimationVelocity();

        switch (myState) {
            case State.FLY_IN:
                if (transform.position.y <= 1) anim.SetTrigger("touchDown");
                hungerRate = 1;
                break;
            case State.WANDER:
                hungerRate -= hungerDeterior * Time.deltaTime;
                updateHungerBar();
                FoodSearching();
                DropCoin();
                break;
            case State.FOUND_FOOD:
                updateHungerBar();
                FoodMode();
                break;
            case State.EATING:
                updateHungerBar();
                Eating();
                break;
            case State.FLY_OUT:
                break;
            case State.FLYING:
                break;
        }
       
    }

    //==================================== Defining each state behaiour ==========================================//
    void SetState(State newState)
    {
        Debug.Log("Set State To" + newState);
        switch (newState)
        {

            case State.FLY_IN:
                agent.enabled = false;
                FlyIn();
                break;
            case State.WANDER:               
                agent.enabled = true;
                SetNewTarget();
                agent.SetDestination(target.transform.position);
                break;
            case State.FOUND_FOOD:
                break;
            case State.EATING:                
                break;
            case State.FLY_OUT:
                anim.SetTrigger("takeOff");
                break;
            case State.FLYING:
                break;
        }
        myState = newState;
    }

    private void NavAnimationVelocity() //defining walk and idle animations according to movemevt and speed
    {
        float velocity = agent.velocity.magnitude / agent.speed;
        anim.SetFloat("walkSpeed", velocity);
        
        // change type of idle after pigeon wlked at avarage speed or above
        if (velocity > 0.4f) anim.SetFloat("idleType", Random.value);
    }

    // ==================== flying in/out methods ========================= 

    public void FlyIn()
    {
        Vector3 fromPos = transform.position;
        Vector3 landingCircle = Random.insideUnitSphere * 10;
        Vector3 toPos = new Vector3(landingCircle.x, 0, landingCircle.z);
        float flightTime = Random.Range(2f, 4f);
        target.transform.position = toPos;

        transform.DOMoveX(toPos.x, flightTime).From(fromPos.x).SetEase(Ease.InOutSine);
        transform.DOMoveZ(toPos.z, flightTime).From(fromPos.z).SetEase(Ease.InOutSine);
        transform.DOMoveY(toPos.y, flightTime).From(fromPos.y).OnComplete(Land).SetEase(Ease.InOutQuad);

        var lookTowards = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.x);
        transform.LookAt(lookTowards);
                
        //var direction = (target.transform.position - fromPos).normalized;
        //var directionFlat = direction - new Vector3(direction.x ,0,direction.z);
        //transform.DORotate(directionFlat, flightTime).From(direction).SetEase(Ease.InQuint);

    }

    void Land()
    {
        SetState(State.WANDER);
        targetReachTime = Time.realtimeSinceStartup;
    }

    public void FlyOut()
    {         
        Vector3 fromPos = transform.position;
        Vector2 circlePos = Random.insideUnitCircle.normalized * 50;
        Vector3 toPos = new Vector3(circlePos.x, 30, circlePos.y);
        float flightTime = Random.Range(2, 4);
        target.transform.position = toPos;

        var lookTowards = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.x);
        transform.LookAt(lookTowards);
        

        transform.DOMove(toPos, flightTime).From(fromPos).OnComplete(DestroyPigeon).SetEase(Ease.InCubic);
        
    }

    void DestroyPigeon()
    {
        Destroy(this.gameObject);
    }


    void SetupBird()
    {
        hungerRate = 1;
        agent.speed = Random.Range(4f, 6f);
        PigeonSpeed = agent.speed;
        //agent.angularSpeed = Random.Range(3000, 4000);
        agent.acceleration = PigeonSpeed * 0.8f;
        float rand = Mathf.Floor(Random.Range(0f, 3f));
        updateHungerBar();

            //SkinnedMeshRenderer mr = GetComponentInChildren<SkinnedMeshRenderer>();
            //mr.materials[0].mainTexture = myTexture[(int)rand];
    }


    // =================== searching food methods ==========================
    void FoodSearching()
    {
        if (agent.isActiveAndEnabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewTarget();
                targetReachTime = Time.realtimeSinceStartup;
            }
        }
        CheckForFood();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (myState == State.WANDER || myState == State.FOUND_FOOD)
        {
            if (other.tag == "Food")
            {
                //myFood = other.GetComponent<Food>();
                //SetState(State.FOUND_FOOD);
                if (myFood != null && Random.Range(0,4) != 1)
                    {
                        var otherFoodDist = (other.transform.position - transform.position).magnitude;
                        var myFoodDist = (myFood.transform.position - transform.position).magnitude;
                    
                        if(otherFoodDist < myFoodDist) myFood = other.GetComponent<Food>();
                    }
                else myFood = other.GetComponent<Food>();
                agent.isStopped = false;
                SetState(State.FOUND_FOOD);
            }
        }

    }

    //check for food in radius and go for it
    void CheckForFood() {
        /*
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 30);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Food")
            {
                if (myFood != null && Random.Range(0,5) != 1)
                {
                    var iFoodDist = (hitColliders[i].transform.position - transform.position).magnitude;
                    var myFoodDist = (myFood.transform.position - transform.position).magnitude;
                    
                    if(iFoodDist < myFoodDist) myFood = hitColliders[i].GetComponent<Food>();
                }
                else myFood = hitColliders[i].GetComponent<Food>();
            }
            i++;
        }
        if (myFood != null)
        {
            agent.isStopped = false;
            SetState(State.FOUND_FOOD);
        }
        */
    }


    
    void SetNewTarget()
    {
        var standingTime = Random.Range(0f,2f);
        
        if (myState == State.WANDER)
        {
            if (standingTime > Time.realtimeSinceStartup - targetReachTime)
            {
                //Debug.Log("is stanting.........");
                agent.isStopped = true;           
            }
            else
            {
                //Debug.Log("SetNewTarget");
                Vector2 pos = Random.insideUnitCircle * walkRadius;
                coinDropped = false;
                agent.isStopped = false;
                agent.SetDestination(RandomNavmeshLocation(walkRadius));
            }
        }
    }

    void DropCoin()
    {


        if (Random.Range(1, coinTypeRates[0]) == 1 && coinDropped == false)
        {

            if (coinStandingTime > Time.realtimeSinceStartup - targetReachTime &&
                (hungerRate > coinMinHungerRate))
            {

                // random timing for a coin of any type

                int typeI = 0;//default is "Coin0"
                for (int i = 1; i < coinTypeRates.Length; i++)
                {
                    // random timing for higher value coins

                    if (Random.Range(1, coinTypeRates[i]) == 1)
                    {
                        typeI = i;
                    }
                }

                Coin newCoin = Instantiate(coinPrephab, this.transform.position, Quaternion.identity);
                newCoin.tag = "Coin" + typeI; // tag will define the variation in material (Coin class) and value (Player class)

                /* replaced
                var newCoin = Instantiate(coinPrephab, this.transform.position, Quaternion.identity);
                newCoin.SetCoinType(0);
                */

                agent.isStopped = true;
                coinDropped = true;
              
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(RandomNavmeshLocation(walkRadius));
            }
        }
    }

    //Vector3 PickPointOnMesh()
    //{
    //    Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
    //    randomDirection += transform.position;
    //    NavMeshHit hit;
    //    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
    //    return hit.position;
    //}

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    void FoodMode()
    {
        if (!myFood) SetState(State.WANDER);
        else agent.SetDestination(myFood.transform.position);

        CheckForFood();

        if (agent.isActiveAndEnabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetState(State.EATING); 
            }
        }

        
    }
    


    private void Eating()
    {
        if (!myFood)
        {
            anim.SetBool("isEating", false);
            SetState(State.WANDER);
        }

        else 
        {
            agent.isStopped = true;
            anim.SetBool("isEating", true);
            // myFood.Eaten() & AddEnergy() are called through animation
        }
    }

    void updateHungerBar()
    {
        {
            //hungerRate -= hungerDeterior * Time.deltaTime;
            hungerSlider.value = hungerRate + 0.1f;
            agent.speed = PigeonSpeed * (2 - Mathf.Clamp(hungerRate, 0.5f, 1.5f));
            anim.speed = 2 - (hungerRate*0.5f);
            // transform.localScale = new Vector3 (0.3f + (0.5f * hungerRate), transform.localScale[1], transform.localScale[2]);
            if (hungerRate < 0 && myState != State.EATING)
            {
                anim.speed = 1.5f;
                SetState(State.FLY_OUT);
            }
        }
        
    }

    public void AddEnergy()
    {
        hungerRate += hungerDeterior * 10;
    }


  
}
