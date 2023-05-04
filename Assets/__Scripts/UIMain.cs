using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    [Header("---")]
    public static float gold = 0;
    public float healCurrLife = 100f;
    public float speedPrice = 200f;
    public float dashCDPrice = 2000f;
    public float healLifePrice = 100f;
    public float lifeRegenPrice = 500f;
    public float penalty = 50f;

    public Text uitLife;
    public Text uitGold;
    public Text uitScore;
    public Text uitDash;
    public Text uitGameOver;
    public Text uitMS;
    public Text uitHelp;
    public Text uitBestScore;

    [Header("set dynamically")]
    public static float curLife = 1000.0f;
    public static float maxLife = 1000.0f;
    public static float lifeRegen = 0;
    private float dashCDRemaining;
    private string dashCDRemString;

    private bool shoppingTime = false;
    public static bool refresh = false;
    public static int score;
    public float damageHit = 400;
    private bool difChoosen = false;

    private int bestScore;
    public string filename;
    private bool scoreSaved = false;

    private void Awake()
    {
        if (filename == "") filename = "RollingStones_Data/BestScore.tt";
        StreamReader sr = new(filename);
        if (sr != null) bestScore = Convert.ToInt32(sr.ReadLine());
        uitGameOver.enabled = false;
    }
    void OnGUI()
    {
        Upd();
        Shopping();
        DashCheck();
        WaveCaller();
        DifficultyChoose();
    }

    void DifficultyChoose()
    {
        if (!difChoosen)
        {
            if (GUI.Button(new Rect((Screen.width / 2 - 360), Screen.height / 2 - 20, 160, 40), "Arcade Mode"))
            {
                damageHit = 400;
                BallDestroyer.scoreHit = 100;
                BallDestroyer.reward = 2;
                difChoosen = true;
                uitHelp.enabled = false;
            }

            if (GUI.Button(new Rect((Screen.width / 2 + 200), Screen.height / 2 - 20, 160, 40), "Zen Mode"))
            {
                damageHit = 1;
                BallDestroyer.scoreHit = 1;
                BallDestroyer.reward = 50;
                difChoosen = true;
                uitHelp.enabled = false;
            }
        }
    }

    void WaveCaller()
    {
        if (BallsSpawn.gameMode == GameMode.idle && difChoosen)
        {
            if (GUI.Button(new Rect((Screen.width / 2 - 40), Screen.height / 2 + 30, 80, 40), "Play"))
            {
                BallsSpawn.gameMode = GameMode.playing;
                shoppingTime = false;
            }
        }
    }

    public void Reseter()
    {
        BallsSpawn.gameMode = GameMode.idle;
        gold = 0;
        MovementRealize.speedMultiplier = 1.2f;
        MovementRealize.canMove = true;
        healCurrLife = 100f;
        speedPrice = 200f;
        dashCDPrice = 2000;
        healLifePrice = 100f;
        lifeRegenPrice = 500f;
        score = 0;
        lifeRegen = 0;
        curLife = 1000f;
        maxLife = 1000f;
        BallsSpawn.waves = 8;
        BallsSpawn.stillRolling = false;
        refresh = false;
        uitGameOver.enabled = false;
        difChoosen = false;
        scoreSaved = false;
    }

    // Update is called once per frame
    void Upd()
    {
        uitScore.text = score + "pts.";
        uitBestScore.text = "Best Score: " + bestScore + " pts.";
        uitGold.text = gold + " •";
        uitLife.text = "Life: " + (curLife - curLife % 1) + "/" + maxLife + " +" + lifeRegen + "% p/s";
        uitMS.text = "Movement speed: " + 100 * MovementRealize.speedMultiplier + "%";

        if (score > bestScore && score != 0) bestScore = score;
    }

    private void FixedUpdate()
    {
        //если хп меньше максимума, то применяется реген. Текущее хп += Сила регена * 1% хп * дельтатайм.  
        if (lifeRegen > 0 && curLife <= maxLife && BallsSpawn.gameMode == GameMode.playing)
            curLife += lifeRegen * (maxLife / 100) * Time.deltaTime;
        //если хп первышает максимум, то применить максимум
        if (curLife > maxLife) curLife = maxLife;
        //если хп ниже нуля, то это 0
        if (curLife <= 0)
        {
            curLife = 0;
            BallDestroyer.scoreHit = 0;

            if (!scoreSaved) ScoreSaver();

            uitGameOver.enabled = true;
        }

        //если получен урон, вычитаем урон и отмечаем что он получен, что бы не получить его повторно
        if (MovementRealize.wasDamaged)
        {
            curLife -= damageHit;
            MovementRealize.wasDamaged = false;
            gold -= penalty;
            if (gold < 0) gold = 0;
        }


        //если пошел ресет, нужно обновить переменные до изначальных
        if (refresh) Reseter();
    }

    void DashCheck()
    {
        //разница текущего времени с временем когда был совершен рывок будет увеличиваться со временем, поэтому нас интересует разность этого времени и КД рывка, для отсчета
        dashCDRemaining = MovementRealize.dashCD - (Time.time - MovementRealize.dashCDTimer);
        //каким то невероятным магическим образом я смог заставить работать эту штуку.
        //0 это порядковый номер переменной что будет аргументом далее, 5 это к-во знаков, после двоеточия идёт шаблон
        dashCDRemString = String.Format("{0,5:#0.00}", dashCDRemaining);


        if (dashCDRemaining <= 0) MovementRealize.dashIsReady = true;

        if (MovementRealize.dashIsReady)
        {
            uitDash.color = Color.green;
            uitDash.text = "Dash ready";
        }
        if (!MovementRealize.dashIsReady)
        {
            if (dashCDRemaining > 1.5f)
            {
                uitDash.color = Color.red;
            }
            else if (dashCDRemaining > 0)
            {
                uitDash.color = Color.yellow;
            }
            uitDash.text = "Dash CD " + dashCDRemString + "s.";
        }
    }

    void Shopping()
    {
        GUI.backgroundColor = Color.blue;

        //открытие окна магазина
        //повторное нажатие должно закрывать окно магазина
        if (!shoppingTime)
        {
            if (GUI.Button(new Rect((Screen.width / 2 - 40), Screen.height - 60, 80, 40), "Shop"))
            {
                shoppingTime = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect((Screen.width / 2 - 40), Screen.height - 60, 80, 40), "Shop"))
            {
                shoppingTime = false;
            }
        }

        if (shoppingTime) // if tap shop button
        {
            GUI.Box(new Rect(Screen.width / 2 - 160, Screen.height - 190, 320, 110), "What you want to buy?");
            GUI.backgroundColor = Color.blue;

            //покупка скорости перемещения, по +10%, что бы было максимум 150% мс. Цена удваивается каждый раз.
            if (MovementRealize.speedMultiplier < 1.5f)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 145, Screen.height - 160, 140, 25), "Speed +10% " + speedPrice + " •")) // If YES is pressed
                {
                    if (gold >= speedPrice)
                    {
                        MovementRealize.speedMultiplier += 0.1f;
                        gold -= speedPrice;
                        speedPrice *= 2;
                    }
                }
            }

            if (MovementRealize.speedMultiplier >= 1.5f)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 145, Screen.height - 160, 140, 25), "Max life +100 " + 500 + " •")) // If YES is pressed
                {
                    if (gold >= 500)
                    {
                        gold -= 500;
                        curLife += 100;
                        maxLife += 100;
                    }
                }
            }


            //покупка возможности делать рывок, и дальнейшее уменьшение КД.
            GUI.backgroundColor = Color.green;

            if (MovementRealize.dashCD > 1)
            {
                if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height - 160, 140, 25), "Dash CD -1s " + dashCDPrice + " •"))
                {
                    if (gold >= dashCDPrice)
                    {
                        MovementRealize.dashCD -= 1f;
                        gold -= dashCDPrice;
                        dashCDPrice *= 2;
                    }
                }
            }

            //покупка хила 100 здоровья.
            GUI.backgroundColor = Color.red;
            if (true)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 145, Screen.height - 120, 140, 25), "Heal " + healCurrLife + " life " + healLifePrice + " •")) // If YES is pressed
                {
                    if (gold >= healLifePrice)
                    {
                        curLife += healCurrLife;
                        gold -= healLifePrice;
                    }
                }
            }

            //покупка регенерации здоровья, аналогично здоровью
            if (true)
            {
                if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height - 120, 140, 25), "Life +1% p/s " + lifeRegenPrice + " •"))
                {
                    if (gold >= lifeRegenPrice)
                    {
                        lifeRegen += 1f;
                        gold -= lifeRegenPrice;
                        lifeRegenPrice *= 2;
                    }
                }
            }
        }
    }

    void ScoreSaver()
    {
        scoreSaved = true;
        StreamWriter sw = new(filename, false);
        sw.WriteLine(bestScore);
        sw.Close();
        
    }
}
