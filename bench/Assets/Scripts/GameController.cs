using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{

    [SerializeField] static public Vector2 fieldSize = new Vector2(14, 15);
    [SerializeField] int numOfPigeons = 50;
    [SerializeField] Pigeon pigeonPrefab;
    //...[SerializeField] float fieldRadius;
    [SerializeField] Transform birdsPlayground;

    //...[SerializeField] float skyRadius = 100f;


    // player buying
    public int[] pigeonPrice = new int[] { 2, 5, 8 };
    public int initCoinsNum = 10;
    public int maxCoinsNum = 0;
    public int lastCoinsNum = 0, totalCoinsNum = 0;
    public AudioSource failAudioClip;

    // texts on Canvas - to be dragged from hierarchy
    public GameObject txtPigeons, txtHPigeons;
    public GameObject txtHappy, txtHHappy;
    public GameObject txtAngry, txtHAngry;
    public GameObject txtCoins, txtHCoins;

    // configuring adding pigeons
    public float cycleTimeNewPigeons = 0f;
    public int MaxNewPigeonsNum = 0;

    // configuring statistics
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
  
    [SerializeField] private List<Pigeon> pigeons;


    private float deltaTimeNewP = 0, deltaTimeCalcS = 0, deltaTimeNewCoins = 0;
    private Quaternion rotation = Quaternion.identity;
    private Text textComponent;


    void Start()
    {
        // pigeons on init
        pigeons = new List<Pigeon>();
        SpawnPigeons(numOfPigeons);

        // prepare for player buying inputs
        PlayerStats.CoinsNum = initCoinsNum;
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
        if (!success)
        {
            failAudioClip.Play(0);
        }

        // high Coins score refers to total number ever collected
        if (PlayerStats.CoinsNum > lastCoinsNum)
            totalCoinsNum += PlayerStats.CoinsNum - lastCoinsNum;
        lastCoinsNum = PlayerStats.CoinsNum;


        if (cycleTimeCalcStatistics > 0f) TimeRecalcStatist();
        if (cycleTimeNewPigeons > 0f) TimeForMorePigeons();
        if (cycleTimeNewCoins > 0f) TimeForMoreCoins();
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
        if (deltaTimeNewCoins > cycleTimeNewCoins && cycleTimeNewCoins > 0)
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
        UpdateSingleText(txtHCoins, totalCoinsNum);// high score for coins refers to total number ever collected
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
