using System;
using UnityEngine;
using UnityEngine.UI;


public class UIMain : MonoBehaviour
{
    [Header("---")]
    public static float gold = 0;
    public float speedMultiplier = 1f;
    public float extraMaxLife = 200f;
    public static bool dashAviable = false;
    public float speedPrice = 100f;
    public float dashPrice = 1000f;
    public float dashCDPrice = 2000;
    public float maxLifePrice = 1000f;
    public float lifeRegenPrice = 500f;
    public float penalty = 100f;

    public Text uitLife;
    public Text uitGold;
    public Text uitScore;
    public Text uitDash;
    public Text uitGameOver;

    [Header("set dynamically")]
    public static float curLife = 1000.0f;
    public static float maxLife = 1000.0f;
    public static float lifeRegen = 0;
    private float dashCDRemaining;
    private string dashCDRemString;
    private bool shoppingTime = false;
    public static bool refresh = false;
    public static float score;


    private void Awake()
    {
        uitGameOver.enabled = false;
    }
    void OnGUI()
    {
        Upd();
        Shopping();
        DashCheck();
        WaveCaller();
    }

    void WaveCaller()
    {
        if (BallsSpawn.gameMode == GameMode.idle)
        {
            if (GUI.Button(new Rect((Screen.width / 2 - 40), Screen.height / 2 + 30, 80, 40), "Play"))
            {
                BallsSpawn.gameMode = GameMode.playing;
            }
        }
    }

    public void Reseter()
    {
        gold = 0;
        speedMultiplier = 1f;
        extraMaxLife = 200f;
        dashAviable = false;
        speedPrice = 100f;
        dashPrice = 1000f;
        dashCDPrice = 2000;
        maxLifePrice = 1000f;
        lifeRegenPrice = 500f;
        score = 0;
        lifeRegen = 0;
        curLife = 1000f;
        maxLife = 1000f;
        BallsSpawn.waves = 4;
        refresh = false;
        uitGameOver.enabled = false;
        MovementRealize.canMove = true;

    }

    // Update is called once per frame
    void Upd()
    {
        uitScore.text = score + "pts.";
        uitGold.text = gold + " •";
        uitLife.text = "Life: " + (curLife - curLife % 1) + "/" + maxLife;
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
            uitGameOver.enabled = true;
        }

        //если получен урон, вычитаем урон и отмечаем что он получен, что бы не получить его повторно
        if (MovementRealize.wasDamaged)
        {
            curLife -= 333;
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
        if (dashAviable)
        {
            if (MovementRealize.dashIsReady)
            {
                uitDash.color = Color.green;
                uitDash.text = "Dash ready.";
            }
            if (!MovementRealize.dashIsReady)
            {
                if (dashCDRemaining > 2)
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
        else
        {
            uitDash.color = Color.red;
            uitDash.text = "Dash not ready.";
        }
    }

    void Shopping()
    {
        GUI.backgroundColor = Color.blue;

        //открытие окна магазина
        //повторное нажатие должно закрывать окно магазина
        if (!shoppingTime) { 
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
                if (GUI.Button(new Rect(Screen.width / 2 - 145, Screen.height - 160, 140, 25), "Speed " + speedPrice + "•")) // If YES is pressed
                {
                    if (gold >= speedPrice)
                    {
                        MovementRealize.speedMultiplier += 0.1f;
                        gold -= speedPrice;
                        speedPrice *= 2;
                    }
                }
            }

            //покупка возможности делать рывок, и дальнейшее уменьшение КД.
            GUI.backgroundColor = Color.green;
            if (!dashAviable)
            {
                if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height - 160, 140, 25), "Dash " + dashPrice + "•")) // if NO is pressed
                {
                    if (gold >= dashPrice)
                    {
                        dashAviable = true;
                        MovementRealize.dashIsReady = true;
                        gold -= dashPrice;
                    }
                }
            }

            if (dashAviable && MovementRealize.dashCD > 3)
            {
                if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height - 160, 140, 25), "Dash CD -1s " + dashCDPrice + "•"))
                {
                    if (gold >= dashCDPrice)
                    {
                        MovementRealize.dashCD -= 1f;
                        gold -= dashCDPrice;
                        dashCDPrice *= 2;
                    }
                }
            }

            //покупка максимального здоровья. Цена как и получаемое здоровье удваиваются после каждой покупки
            GUI.backgroundColor = Color.red;
            if (true)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 145, Screen.height - 120, 140, 25), "Max life +" + extraMaxLife + " " + maxLifePrice + "•")) // If YES is pressed
                {
                    if (gold >= maxLifePrice)
                    {
                        maxLife += extraMaxLife;
                        gold -= maxLifePrice;
                        maxLifePrice *= 2;
                        extraMaxLife *= 2;
                    }
                }
            }

            //покупка регенерации здоровья, аналогично здоровью
            if (true)
            {
                if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height - 120, 140, 25), "Regen +0.5% p/s " + lifeRegenPrice + "•"))
                {
                    if (gold >= lifeRegenPrice)
                    {
                        lifeRegen += 0.5f;
                        gold -= lifeRegenPrice;
                        lifeRegenPrice *= 2;
                    }
                }
            }
        }
    }
}
