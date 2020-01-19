using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRigidbody : MonoBehaviour
{

    [SerializeField] private float rotatationRate = 1f; 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] GameObject food;
    [SerializeField] GameObject foodPrefab;


    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CreateFood();
    }

    // Update is called once per frame
    void Update()
    {
        float moveAxis = Input.GetAxis("Vertical");
        float turnAxis = Input.GetAxis("Horizontal");
        ApplyInput(moveAxis, turnAxis);



        float step = moveSpeed * Time.deltaTime; // calculate distance to move
                                                 //var dir = Vector3.MoveTowards(transform.position, food.position, step);

        Vector3 dir = (food.transform.position - transform.position).normalized;
        dir.y = 0;
        rb.AddForce(dir*moveSpeed*Time.deltaTime);
        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, food.transform.position) < 0.001f) {
            // Swap the position of the cylinder.
           // food.position *= -1.0f;
        }


        Vector3 targetDirection = dir;

        // The step size is equal to speed times frame time.
        float singleStep = rotatationRate * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        rb.rotation = Quaternion.LookRotation(newDirection);
 



    }

    private void ApplyInput(float moveInput, float turnInput) {
        Move(moveInput);
        Turn(turnInput);
    }
    
    void OnTriggerEnter(Collider col) {
        CreateFood();
    }


    private void Move(float input) {
        transform.Translate(Vector3.forward * input * moveSpeed * Time.deltaTime);
    }

    private void Turn(float input) {
        transform.Rotate(0, input * rotatationRate * Time.deltaTime, 0);
    }

    void CreateFood() {
        Destroy(food);
        if (food != null) {
           
        }
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        
        food = Instantiate(foodPrefab, new Vector3(x, 1f, z),Quaternion.identity);
        rb.AddForce(-rb.velocity,ForceMode.Impulse);
        //   rb.velocity = Vector3.zero;
    }

    void CreateRandomTarget() {
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
    }
}
