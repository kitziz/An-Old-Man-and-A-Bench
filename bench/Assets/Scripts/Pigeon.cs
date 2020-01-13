using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Pigeon : MonoBehaviour
{
    // self destruction
    public float expirationTime = 20f; //0 for no expiration
    
    public Texture[] myTexture;

    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Slider hugerSlider;
    [SerializeField] private float hungerRate = 0.01f;


    private GameObject target;
    private NavMeshAgent agent;

    private enum State { IDLE, FOOD, EAT, FLY, FLY_IN, FLY_OUT};
    private State state;
    private float timeSiceLastSpawn;
    public Food myFood;
    private float hunger;
    private Animator anim;
    
    private float walkRadius = 10f;

    // count till self destruction or new life
    float timeAlive;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();     
        hugerSlider = GetComponentInChildren<Slider>();
        anim = GetComponentInChildren<Animator>();
        target = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
                
        SetState( State.FLY_IN);  
 
    }




    void Update()
    {        
        NavAnimationVelocity();
        hunger -= hungerRate * Time.deltaTime;         updateHungerBar(); 
        if (1 < transform.position.y && transform.position.y < 2) anim.SetTrigger("touchDown");

        switch (state) {
            case State.IDLE:
                IdleMode();                
                break;
            case State.FOOD:
                FoodMode();
                break;
            case State.EAT:
                break;
            case State.FLY:
                break;
        }
       
    }




    void SetupBird()
    {
        agent.enabled = true;

        agent.speed = Random.Range(4f, 10f);
        agent.angularSpeed = Random.Range(300f, 700f);
        float rand = Mathf.Floor(Random.Range(0f, 3f));
       
            SkinnedMeshRenderer mr = GetComponentInChildren<SkinnedMeshRenderer>();
            mr.materials[0].mainTexture = myTexture[(int)rand];
    }


    
    void IdleMode()
    {
        if (agent.isActiveAndEnabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewTarget();
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {}
            }
        }


        //LOOK FOR FOOD
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Food")
            {
                myFood = hitColliders[i].GetComponent<Food>() ;               
            }
            i++;
        }
        if (myFood != null)
        {
            SetState(State.FOOD);
        }

    }
    


    void FoodMode()
    {
        if (!myFood)
        {
            SetState(State.IDLE);
        }


        if (agent.isActiveAndEnabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                SetState(State.EAT); 
                  
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) { }
            }
        }
    }
       


    void SetState(State _state)
    {
        Debug.Log("Set State To" + _state);
        switch (_state)
        {

            case State.FLY_IN:
                agent.enabled = false;
                hunger = 1;
                FlyIn();
                break;
            case State.IDLE:               
                agent.enabled = true;
                SetNewTarget();
                agent.SetDestination(target.transform.position);
                break;
            case State.FOOD:
                agent.SetDestination(myFood.transform.position);
                break;
            case State.EAT:                
                 StartCoroutine(Eating());
                break;
            case State.FLY:
                break;
            case State.FLY_OUT:
                anim.SetTrigger("takeOff");
                break;
        }
        state = _state;
    }



 
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
        //transform.DOMoveY(0, 0.5f);
        SetState(State.IDLE);
        //anim.SetTrigger("touchDown");
    }


    
    public void FlyOut()
    {         
        Vector3 fromPos = transform.position;
        Vector2 circlePos = Random.insideUnitCircle.normalized * 50;
        Vector3 toPos = new Vector3(circlePos.x, 30, circlePos.y);
        float flightTime = Random.Range(2, 4);
        target.transform.position = toPos;
        transform.LookAt(target.transform);

        transform.DOMove(toPos, flightTime).From(fromPos).OnComplete(DestroyPigeon).SetEase(Ease.InCubic);
        
    }

    void DestroyPigeon()
    {
        Destroy(this.gameObject);
    }

    /*
    void SelfDestroyOnLifeTime()
    {
        // self destruction by lifeTime parameter plus chance factor
        timeAlive += Time.deltaTime;
        if (expirationTime > 0f && timeAlive > expirationTime)
        {
            Debug.Log("------lifeTime----" + gameObject);
            int moreCycles = Random.Range(0, 2);// destroy chance is 0.50 each cycle 
            if (moreCycles == 0)
            {
                Debug.Log("------self destroying----" + gameObject);
                anim.SetTrigger("takeOff");
                // FlyOut() is called through animation
            }
            else
                timeAlive = 0f; // get new life cycle
        }
    }
    */


    private void NavAnimationVelocity()
    {
        float velocity = agent.velocity.magnitude / agent.speed;
        anim.SetFloat("walkSpeed", velocity);
        // changes the type of idle if pegion wlked at avarage speed or above
        if (velocity > 0.4f) anim.SetFloat("idleType", Random.value);
    }


    void SetNewTarget() {
        Debug.Log("SetNewTarget");
        Vector2 pos = Random.insideUnitCircle * walkRadius;
       // target.transform.position = transform.position + new Vector3(pos.x, 0, pos.y);
        agent.SetDestination (PickPointOnMesh());
        StartCoroutine(standing());
        //var rand = Random.Range(0, 4);
        //if (rand < 1) { StartCoroutine(standing()); }
        //else if (rand < 1.2) { StartCoroutine(flapping()); }

    }

    IEnumerator standing()
    {
        //anim.SetBool("isStanding", true);
        if (agent) agent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        if (agent) agent.isStopped = false;
        //anim.SetBool("isStanding", false);
    }
    
    /*
    IEnumerator flapping() {
        //anim.SetBool("isFlapping", true);
        if (agent) agent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        if (agent) agent.isStopped = false;
        //anim.SetBool("isFlapping", false);
    }
    */

    IEnumerator Eating() {
       
        if (agent) agent.isStopped = true;
        anim.SetBool("isEating", true);
        //myFood.Eaten() is called through animation
        //AddEnergy() is called through animation
        
        yield return new WaitForSeconds(Random.Range(3f, 4f));

        if (agent) agent.isStopped = false;
        anim.SetBool("isEating", false);
        SetState(State.IDLE);
    }


    #region helpers

    void OnTriggerEnter(Collider col)
    {
 
        Debug.Log("OnTriggerEnter" + col.gameObject.name);
    }



    void OnCollisionEnter(Collision col)
    {

    }



    void updateHungerBar()
    {
        hugerSlider.value = hunger;
        if (hunger <= 0 && state != State.FLY_OUT) SetState(State.FLY_OUT);
        
    }

    public void AddEnergy()
    {
        hunger += hungerRate * 20;
    }

    Vector3 PickPointOnMesh() {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius,1);
        return hit.position;

    }
    #endregion




}
