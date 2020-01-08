 using UnityEngine;
 using System.Collections;

public class PlayerController : MonoBehaviour
{
    //=============== Camera Orientation Members ===============
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    private float xRotation = 0;


    //==================== Feeding Members ====================
    [SerializeField] private Rigidbody foodPrefab;
    [SerializeField] private int crumbAmountMin = 5, crumbAmountMax = 15 ;
    [SerializeField] private float throwForce = 10f;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up*mouseX);


        if (Input.GetMouseButtonDown(0)) ThrowFood();
    }

    void ThrowFood()
    {
        Vector3 playerPos = this.transform.position;
        int CrumbAmmount = Random.Range(crumbAmountMin, crumbAmountMax);
        
        for (int i = 0; i < CrumbAmmount; i++)
        {
            var directionRandom = transform.forward + new Vector3(Random.value* 0.3f, Random.value * 0.3f, Random.value * 0.3f);
            var food = Instantiate(foodPrefab, playerPos, Quaternion.identity);
            food.AddForce(directionRandom * Random.Range(throwForce * 0.8f, throwForce * 1.2f));
        }

    }
}