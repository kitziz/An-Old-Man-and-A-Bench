 using UnityEngine;
 using System.Collections;
 using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    //=============== Camera Orientation Members ===============
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public AudioSource failAudioClip;

    private float xRotation = 0;


    //==================== Feeding Members ====================
    [SerializeField] private Food foodPrefab;
    [SerializeField] private int crumbAmountMin = 5, crumbAmountMax = 15 ;
    [SerializeField] private float throwForce = 10f;

    //==================== Coins ===============================
    public int[] coinValues = new int[] { 2, 5, 10, 15 }; // according to colors array defined in coin class


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        failAudioClip = GetComponent<AudioSource>();
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

        // Trowing Food exactly to where player left-clicks
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "PigeonGround")
                {
                    if (PlayerStats.CoinsNum > 0)
                    {
                        Debug.Log("HIT the ground runing!" + hit.transform.name);
                        PlayerStats.CoinsNum --;

                        ThrowFood(hit.point);
                    }
                }
                else if (hit.transform.gameObject.tag.Contains("Coin"))
                {
                    Debug.Log("HIT Capitalist!" + hit.transform.name);
                    CoinCollected(hit.transform.gameObject);
                }
            }
        }
    }


    void CoinCollected(GameObject go)
    {
        int typeIndex = 0;

        for (int i = 0; i < coinValues.Length; i++)
        {
            if (go.tag.Contains(i.ToString()))
                typeIndex = i;
        }
        PlayerStats.CoinsNum += coinValues[typeIndex];

        //Destroy(go.GetComponentInParent<Transform>().gameObject);
        Destroy(go);
        // instantiate particles        
    }


    void ThrowFood(Vector3 targetPosition)
    {
        Vector3 playerPos = this.transform.position;
        int CrumbAmmount = Random.Range(crumbAmountMin, crumbAmountMax);
        
        for (int i = 0; i < CrumbAmmount; i++)
        {
            var targetPosRnd = targetPosition + new Vector3(Random.value*3, 0, Random.value*3);
            var food = Instantiate(foodPrefab, playerPos, Quaternion.identity);
            food.transform.DOMoveX(targetPosRnd[0], .5f).From(playerPos).SetEase(Ease.OutSine);
            food.transform.DOMoveZ(targetPosRnd[2], .5f).From(playerPos).SetEase(Ease.OutSine);
            food.transform.DOMoveY(.5f , .5f).From(playerPos).SetEase(Ease.InOutCubic).OnComplete(food.EnableCollider);
            //food.AddForce(directionRandom * Random.Range(throwForce * 0 8f, throwForce * 1.2f));
        }
    }
       
}