using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{

    [SerializeField] static public Vector2 fieldSize = new Vector2(14, 15);
    [SerializeField] int numOfPigeons = 50;
    [SerializeField] float foodRate = 10f;
    [SerializeField] Pigeon pigeonPrefab;
    [SerializeField] Food foodPrefab;
    //...[SerializeField] float fieldRadius;
    [SerializeField] Transform birdsPlayground;

    //...[SerializeField] float skyRadius = 100f;


    // player buying
    public int[] pigeonPrice = new int[] { 2, 5, 8 };
    public int[] foodPrice = new int[] { 1, 2 };
    public int initFood = 10;
    public int initCoinsNum = 10;//...
    public int maxCoinsNum = 0;
    public AudioSource failAudioClip;

    //public int[] foodAvailable = new int[] { 0, 0 };kept in PlayerStats.CoinsNum
    //public int coinsNum = 0; - kept in PlayerStats.CoinsNum


    // texts on Canvas - to be dragged from hierarchy
    public GameObject txtPigeons, txtHPigeons;
    public GameObject txtHappy, txtHHappy;
    public GameObject txtAngry, txtHAngry;
    public GameObject txtCoins, txtHCoins;
    public GameObject txtFood, txtHFood;


    // configuring adding pigeons
    public float cycleTimeNewPigeons = 0f;
    public int MaxNewPigeonsNum = 0;

    // configuring statistics
    public int pigeonMinHappiness = 1;
    public int pigeonMaxHappiness = 10;
    public float pigeonAngryAt = 0.3f;
    public float pigeonHappyAt = 0.6f;
    public float cycleTimeCalcStatistics = 0.5f;

    //configuring adding coins 
    //configuring adding coins 
    public float coinGrntAtHappyRate = 0.7f; //pigeons happy devided by their number 
    public float coinLostAtAngryRate = 0.5f; // pigeons angry divided by exist - loose coins
    public float cycleTimeNewCoins = 0f;

    // statistics 
    public int pigeonStatistCreated = 0, pigeonStatistExist = 0, pigeonStatistMax = 0;
    public int pigeonStatistHappy = 0, pigeonStatistMaxH = 0;
    public int pigeonStatistAngry = 0, pigeonStatistMaxA = 0;
    public int foodStatistCreatedPrm = 0, foodStatistCreatedNrm = 0;
    public int lastFoodCntr=0, lastFoodClickedCntr=0, foodClickedCntr=0;
    public int maxFood = 0;


    [SerializeField] private List<Pigeon> pigeons;


    private float timeSinceLastSpawn = 0;//food
    private Food food;
    private float deltaTimeNewP = 0, deltaTimeCalcS = 0, deltaTimeNewCoins = 0;
    private Quaternion rotation = Quaternion.identity;
    private int foodQuality=0;
    private Text textComponent;
    





    void Start()
    {
        // pigeons on init
        pigeons = new List<Pigeon>();
        SpawnPigeons(numOfPigeons);

        // prepare for player buying inputs
        PlayerStats.CoinsNum = initCoinsNum;
        PlayerStats.FoodAvailable[0] = initFood;
        failAudioClip = GetComponent<AudioSource>();

        CalcPigeonStatistics();
    }


    void Update() {



        // pressing a number key 
        bool success = true;
        if (Input.GetKeyDown("1"))
        {
            success = BuyPigeon(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            success = BuyPigeon(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            success = BuyPigeon(2);
        }
        else if (Input.GetKeyDown("4"))
        {
            success = BuyFood(0);
        }
        else if (Input.GetKeyDown("5"))
        {
            success = BuyFood(1);
        }
        if (!success)
        {
            failAudioClip.Play(0);
        }

        // if collecting coin


        /* food spawning moved to PlayerController script
        if (Input.GetMouseButtonDown(0)) {
               RaycastHit hit;
               Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
               if (Physics.Raycast(ray, out hit, 60f))
               {
                   //suppose i have two objects here named obj1 and obj2.. how do i select obj1 to be transformed 
                   if (hit.transform != null)
                   {
                       Debug.Log("HIT" + hit.transform.name);
                       foodClickedCntr++;
                       CreateFood(hit.point);
                   }
               }
        }
        */

        if (cycleTimeCalcStatistics > 0f) TimeRecalcStatist();
        if (cycleTimeNewPigeons > 0f) TimeForMorePigeons();
        if (cycleTimeNewCoins > 0f) TimeForMoreCoins();

        /* food spawning moved to PlayerController script
        AutomaticFoodSpawnr();
        */
    }


    //=========== Pigeons ==========
    bool BuyPigeon(int type)
    {
        if (PlayerStats.CoinsNum >= pigeonPrice[type])
        {
            PlayerStats.CoinsNum -= pigeonPrice[type];
            Pigeon p = SpawnPigeon();
            ChangePigeonByType(p, type);
            return true;
        }
        else 
            return false;
    }

    void ChangePigeonByType(Pigeon p, int type)
    {
        if (type > 0)//type is not standard
        {
            p.transform.localScale += new Vector3(0, type * 0.8f, 0);
        }
    }

    Pigeon SpawnPigeon()
    {
        Vector2 circlePos = Random.insideUnitCircle.normalized * 50;
        Vector3 pos = new Vector3(circlePos.x, 30, circlePos.y);
        Pigeon p = Instantiate(pigeonPrefab, pos, Quaternion.identity, birdsPlayground);
        pigeons.Add(p);
        return (p);
    }

    void SpawnPigeons(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Pigeon p = SpawnPigeon();
        }
    }

    void TimeForMorePigeons()
    {
        deltaTimeNewP += Time.deltaTime;
        if (cycleTimeNewPigeons > 0f && deltaTimeNewP > cycleTimeNewPigeons)
        {
            deltaTimeNewP = 0f;
            int num = Random.Range(1, MaxNewPigeonsNum);

            SpawnPigeons(num);
        }
    }


    //================ Food ===========================

    bool BuyFood(int type)
    {
        if (PlayerStats.CoinsNum >= foodPrice[type])
        {
            PlayerStats.CoinsNum -= foodPrice[type];
            PlayerStats.FoodAvailable[type]++;
            return true;
        }
        return false;
    }

    /* food spawning moved to PlayerController script
    void AutomaticFoodSpawner()
    {
    timeSinceLastSpawn += Time.deltaTime;

    if (timeSinceLastSpawn > foodRate) {
        timeSinceLastSpawn = 0;
        Debug.Log("RESPAWN FOOD");

        Vector2 pos = Random.insideUnitCircle * 30;
        CreateFood (new Vector3(pos.x, 1f, pos.y));
    }
    }
    void CreateFood(Vector3 pos) {

    food = Instantiate(foodPrefab, pos, Quaternion.identity, transform);
    //   called before with birdsPlayground instead of transformed - but was not active
    //   rb.velocity = Vector3.zero;

    if (foodQuality > 0)
        foodStatistCreatedPrm++;
    else
        foodStatistCreatedNrm++;

    }
    */



    //==========================  data & scores (statistics) ================================

    void TimeRecalcStatist()
    {
        deltaTimeCalcS += Time.deltaTime;
        if (deltaTimeCalcS > cycleTimeCalcStatistics)
        {
            deltaTimeCalcS = 0f;

            CalcPigeonStatistics();
        }
    }


    void GetPigeonStatistics()
    {
        // TBD - recalc statistics can be called from another module
        CalcPigeonStatistics();

        // TBD - if to return a structure to game manager oate public parametersr just upd
    }


    void TimeForMoreCoins()
    {
        deltaTimeNewCoins += Time.deltaTime;
        if (deltaTimeNewCoins > cycleTimeNewCoins)
        {
            deltaTimeNewCoins = 0f;

            if (pigeonStatistExist > 0)// there are pigeons in the game
            {
                float cond1 = (float)pigeonStatistHappy / pigeonStatistExist;
                if (cond1 >= coinGrntAtHappyRate)
                {
                    //get coin
                    PlayerStats.CoinsNum++;
                    if (PlayerStats.CoinsNum > maxCoinsNum)
                        maxCoinsNum = PlayerStats.CoinsNum;
                }
                else
                {
                    float cond2 = (float)pigeonStatistAngry / pigeonStatistExist;
                    if (cond2 < coinLostAtAngryRate && PlayerStats.CoinsNum > 0)
                    {
                        //loose coin
                        PlayerStats.CoinsNum--;
                    }
                }
            }
            lastFoodClickedCntr = foodClickedCntr;
            Debug.Log("food " + lastFoodCntr);
        }
    }



    void CalcPigeonStatistics()
    {

        int existsNum = 0, happyNum = 0, angryNum = 0;

        pigeonStatistCreated = pigeons.Count;

        for (int i = 0; i < pigeonStatistCreated; i++)
        {
            Pigeon pigeon = pigeons[i];

            // if pigeon still exists, otherwise pigeon==null
            if (pigeon != null)
            {
                existsNum++;

                if (PigeonHappiness(pigeon) >= pigeonHappyAt)
                        happyNum++;

                else if (PigeonHappiness(pigeon) <= pigeonAngryAt)
                        angryNum++;
                
            }
        }

        pigeonStatistExist = existsNum;
        pigeonStatistHappy = happyNum;
        pigeonStatistAngry = angryNum;

        if (pigeonStatistExist > pigeonStatistMax)
            pigeonStatistMax = pigeonStatistExist;
        if (pigeonStatistHappy > pigeonStatistMaxH)
            pigeonStatistMaxH = pigeonStatistHappy;
        if (pigeonStatistAngry > pigeonStatistMaxA)
            pigeonStatistMaxA = pigeonStatistAngry;


        // UI scores
        UpdateTexts();
    }


    public void UpdateTexts()
    {
        UpdateSingleText(txtPigeons, pigeonStatistExist);
        UpdateSingleText(txtHPigeons, pigeonStatistMax);
        UpdateSingleText(txtHappy, pigeonStatistHappy);
        UpdateSingleText(txtHHappy, pigeonStatistMaxH);
        UpdateSingleText(txtAngry, pigeonStatistAngry);
        UpdateSingleText(txtHAngry, pigeonStatistMaxA);
        UpdateSingleText(txtCoins, PlayerStats.CoinsNum);
        if (PlayerStats.CoinsNum > maxCoinsNum)
           maxCoinsNum = PlayerStats.CoinsNum;
        UpdateSingleText(txtHCoins, maxCoinsNum);
        UpdateSingleText(txtFood, PlayerStats.FoodAvailable[0]);
        if (PlayerStats.FoodAvailable[0] > maxFood)
            maxFood = PlayerStats.FoodAvailable[0];
        UpdateSingleText(txtHCoins, maxFood);

    }


    void UpdateSingleText (GameObject text, int num)
    {
        textComponent = text.GetComponent<Text>();
        textComponent.text = num.ToString();
    }


    float PigeonHappiness(Pigeon pigeon)
    {
        Slider hugerSlider = pigeon.GetComponentInChildren<Slider>();
        if (hugerSlider != null)
        {
            return hugerSlider.value;
        }
        else return 0f;
    }


}
