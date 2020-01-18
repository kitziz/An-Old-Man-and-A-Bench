 using UnityEngine;
 using System.Collections;
 using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    //=============== Camera Orientation Members ===============
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    private float xRotation = 0;


    //==================== Feeding Members ====================
    [SerializeField] private Food foodPrefab;
    [SerializeField] private int crumbAmountMin = 5, crumbAmountMax = 15 ;
    //[SerializeField] private float throwForce = 10f;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Makes Camera Follow Mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up*mouseX);

        // Trowing Food exactly to where player clicks
        if (Input.GetMouseButtonDown(0))
        {
            if (PlayerStats.FoodAvailable[0] > 0)
            { 
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag == "PigeonGround")
                    {
                        Debug.Log("HIT the ground runing!" + hit.transform.name);
                        ThrowFood(hit.point);
                    }
                    else if (hit.transform.tag == "Coin")
                    {
                        Debug.Log("HIT Capitalist!" + hit.transform.name);
                        CoinCollected(hit.transform.gameObject);
                    }
                }
            }
        }

        // right-click for collecting food
        /*
         * if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 60f))
            {                
                if (hit.transform != null)
                {
                    Debug.Log("HIT" + hit.transform.name);
                    ThrowFood(hit.point);
                    //foodClickedCntr++;
                }
            }
        }
        */

        //if (Input.GetMouseButtonDown(0)) ThrowFood();
    }

    void ThrowFood(Vector3 targetPosition)
    {
        //PlayerStats.FoodAvailable[0]--;
        PlayerStats.CoinsNum -= 1;

        Vector3 playerPos = this.transform.position;
        int CrumbAmmount = Random.Range(crumbAmountMin, crumbAmountMax);
        
        for (int i = 0; i < CrumbAmmount; i++)
        {
            var targetPosRnd = targetPosition; // + new Vector3(Random.value*3, 0, Random.value*3);
            var food = Instantiate(foodPrefab, playerPos, Quaternion.identity);
            food.transform.DOMoveX(targetPosRnd[0], .5f).From(playerPos).SetEase(Ease.OutSine);
            food.transform.DOMoveZ(targetPosRnd[2], .5f).From(playerPos).SetEase(Ease.OutSine);
            food.transform.DOMoveY(.5f , .5f).From(playerPos).SetEase(Ease.InOutCubic).OnComplete(food.EnableCollider);
            //food.AddForce(directionRandom * Random.Range(throwForce * 0 8f, throwForce * 1.2f));
        }

    }
       
}