using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Pigeon : MonoBehaviour
{      
    public Texture[] myTexture;                             // Defining pigeon oclors texture

    [SerializeField] private GameObject targetPrefab;       // a "null" taget prefab for the navmesh agent
    [SerializeField] private Slider hugerSlider;            // a canvas that visulizes Hanger Rate
    [SerializeField] private float hungerDeterior = 0.01f;  // Hanger deterioration per sec

    // navigation members
    private NavMeshAgent agent;
    private GameObject target;
    private float targetReachTime;
    private float walkRadius = 12f;
    private float PigeonSpeed = 8;

    // related assets 
    public Food myFood;
    public GameObject coinPrephab;

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
        hugerSlider = GetComponentInChildren<Slider>();
        anim = GetComponentInChildren<Animator>();
        target = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity); // why do we need it?

        SetupBird();

        SetState( State.FLY_IN);  
 
    }




    void Update()
    {        
        NavAnimationVelocity();
        
        switch (myState) {
            case State.FLY_IN:
                if (transform.position.y <= 2) anim.SetTrigger("touchDown");
                hungerRate = 1;
                break;
            case State.WANDER:
                updateHungerBar();
                FoodSearching();                
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
        Vector3 toPos = new Vector3(Random.Range(-20,20), 0, Random.Range(-30, 10));
        float flightTime = Random.Range(2, 4);
        target.transform.position = toPos;

        transform.DOMove(toPos, flightTime).From(fromPos).OnComplete(Land).SetEase(Ease.OutQuad);

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
        agent.speed = Random.Range(3f, 6f);
        PigeonSpeed = agent.speed;
        agent.angularSpeed = Random.Range(500f, 900f);
        agent.acceleration = agent.speed / 2;
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
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {}
            }
        }


        //LOOK FOR FOUND_FOOD
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Food")
            {
                myFood = hitColliders[i].GetComponent<Food>();
            }
            i++;
        }
        if (myFood != null)
        {
            agent.isStopped = false;
            SetState(State.FOUND_FOOD);
        }

    } 
    
    void SetNewTarget()
    {
        var standingTime = Random.Range(0f, 5f);

        if (standingTime > Time.realtimeSinceStartup - targetReachTime)
        {
            Debug.Log("is stanting.........");
            agent.isStopped = true;
            if (Random.Range(0,1000) == 500) Instantiate(coinPrephab, this.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("SetNewTarget");
            Vector2 pos = Random.insideUnitCircle * walkRadius;
            agent.isStopped = false;
            agent.SetDestination(PickPointOnMesh());
        }
       
    }

    Vector3 PickPointOnMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius,1);
        return hit.position;
    }

    

    void FoodMode()
    {
        if (!myFood) SetState(State.WANDER);
        else agent.SetDestination(myFood.transform.position);


        if (agent.isActiveAndEnabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetState(State.EATING); 
                  
                //if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) { }
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

        else if (agent)
        {
            agent.isStopped = true;
            anim.SetBool("isEating", true);
            // myFood.Eaten() & AddEnergy() are called through animation
        }
    }

    void updateHungerBar()
    {
        {
            hungerRate -= hungerDeterior * Time.deltaTime;
            hugerSlider.value = hungerRate;
            agent.speed = PigeonSpeed / Mathf.Clamp(hungerRate, 0.3f, 1.0f);
            transform.localScale = new Vector3 ((0.4f + hungerRate / 1.5f), (0.6f + hungerRate / 6f), 0.6f);
            if (hungerRate <= 0) SetState(State.FLY_OUT);
        }
        
    }

    public void AddEnergy()
    {
        hungerRate += hungerDeterior * 20;
    }


    
    #region helpers

    void OnTriggerEnter(Collider col)
    {
         // Debug.Log("OnTriggerEnter" + col.gameObject.name);
    }

    void OnCollisionEnter(Collision col)
    {

    }

    #endregion




}
