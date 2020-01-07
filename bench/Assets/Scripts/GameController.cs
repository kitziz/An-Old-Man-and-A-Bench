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
    [SerializeField] float fieldRadius;
    [SerializeField] Transform birdsPlayground;

    [SerializeField] float skyRadius = 100f;

    // texts on Canvas - to be dragged from hierarchy
    public GameObject txtPigeons, txtHPigeons;
    public GameObject txtHappy, txtHHappy;
    public GameObject txtAngry, txtHAngry;
    public GameObject txtCoins, txtHCoins;

    // configuring adding pigeons
    public int cycleTimeNewPigeons = 10;
    public int MaxNewPigeonsNum = 8;

    // configuring statistics
    public int pigeonMinHappiness = 1;
    public int pigeonMaxHappiness = 10;
    public int pigeonAngryAt = 2;
    public int pigeonHappyAt = 6;
    public int cycleTimeCalcStatistics = 3;

    //configuring adding coins 
    public float coinsRatioPigeons = 0.3f; //pigeons number divided by created
    public float coinsRatioHappy = 0.2f; //pigeons happy devided by their number 
    public float coinsRatioAngry = 0.2f; //pigeons happy devided by their number
    public float coinsRatioLose = 0.2f; // pigeons exist divided by max - lose coins
    public int CoinRatioFood = 5; // num of food in last cycle
    public float cycleTimeNewCoins = 20f;

    // statistics 
    public int pigeonStatistCreated = 0, pigeonStatistExist = 0, pigeonStatistMax = 0;
    public int pigeonStatistHappy = 0, pigeonStatistMaxH = 0;
    public int pigeonStatistAngry = 0, pigeonStatistMaxA = 0;
    public int foodStatistCreatedPrm = 0, foodStatistCreatedNrm = 0;
    public int lastFoodCntr=0, lastFoodClickedCntr=0, foodClickedCntr=0;
    public int coinsNum = 0, maxCoinsNum=0;


    [SerializeField] private List<Pigeon> pigeons;


    private float timeSinceLastSpawn = 0;//food
    private Food food;
    private float deltaTimeNewP = 0, deltaTimeCalcS = 0, deltaTimeNewCoins = 0;
    private Quaternion rotation = Quaternion.identity;
    private int foodQuality=0;
    private Text textComponent;





    void Start()
    {
        pigeons = new List<Pigeon>();
        SpawnPigeons(numOfPigeons);

        CalcPigeonStatistics();
    }





    void Update() {

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

        TimeForMorePigeons();

        //AutomaticFoodSpawner();

        //TimeForMoreCoins();

        TimeRecalcStatist();


    }



    void TimeForMorePigeons()
    {
        deltaTimeNewP += Time.deltaTime;
        if (deltaTimeNewP > cycleTimeNewPigeons)
        {
            deltaTimeNewP = 0f;
            int num = Random.Range(1, MaxNewPigeonsNum);

            SpawnPigeons(num);
        }
    }



   
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
    

    void SpawnPigeons(int num) {
        for (int i=0; i<num; i++) {
            SpawnPigeon();
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



    void SpawnPigeon()
    {

        Vector2 circlePos = Random.insideUnitCircle.normalized * 50;
        Vector3 pos = new Vector3(circlePos.x, 30, circlePos.y);
        Pigeon p = Instantiate(pigeonPrefab, pos, Quaternion.identity, birdsPlayground);
        pigeons.Add(p);
    }



    //==========================  gathering data, scores and coins  ================================



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
        bool getCoin = false, looseCoin = false;
        deltaTimeNewCoins += Time.deltaTime;
        if (deltaTimeNewCoins > cycleTimeNewCoins)
        {
            deltaTimeNewCoins = 0f;

            if (pigeonStatistExist > 0)
            {
                // if player provides food
                if (lastFoodClickedCntr == foodClickedCntr)
                    looseCoin = true;
                else if (foodStatistCreatedPrm + foodStatistCreatedNrm - lastFoodCntr >= CoinRatioFood)
                    getCoin = true;
                // if pigeon population normal
              
                float cond1 = (float)pigeonStatistExist / pigeonStatistCreated;
                float cond2 = (float)pigeonStatistHappy / pigeonStatistExist;
                float cond3 = (float)pigeonStatistAngry / pigeonStatistExist;
                if (cond1 >= coinsRatioPigeons &&
                         cond2 >= coinsRatioHappy &&
                         cond3 < coinsRatioAngry)
                        getCoin = true;
                else
                {
                     float cond4 = (float)pigeonStatistExist / pigeonStatistMax;
                      if (cond4 < coinsRatioLose)
                            looseCoin = true;
                }
                
                if (getCoin)
                {
                     //get coin
                     coinsNum++;
                     if (coinsNum > maxCoinsNum)
                         maxCoinsNum = coinsNum;
                }
                else if (looseCoin && coinsNum > 0)
                {
                     //loose coin
                     coinsNum--;
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
        UpdateSingleText(txtCoins, coinsNum);
        UpdateSingleText(txtHCoins, maxCoinsNum);
    }


    void UpdateSingleText (GameObject text, int num)
    {
        textComponent = text.GetComponent<Text>();
        textComponent.text = num.ToString();
    }


    int PigeonHappiness(Pigeon pigeon)
    {

        // TBD - Final interface with GamwObject Pigeon 
        // temp random indication

        if (Time.realtimeSinceStartup > cycleTimeNewPigeons)// happiness checks starts in delay
        {
            int happiness = Random.Range(1, 11);
            return happiness;
        }
        else
            return 5;//normal
    }  


}
