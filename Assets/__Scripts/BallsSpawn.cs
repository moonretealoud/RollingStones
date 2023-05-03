using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class BallsSpawn : GameController
{
    [Header("set myself")]
    public float idleDelay = 5f;
    public float waveDelay = 1f;
    public static float waves = 8;

    public GameObject prefabBall;
    public Text uitWavesCountInfo;
    public GameObject explodePrefab;

    private const float padding = 2.0f;

    [Header("set dynamically")]
    private float waveCounter;
    private int q, last, preLast;
    public static bool stillRolling = false;
    public static GameMode gameMode;
    private float ballsInARow = 0;

    public Vector3 Current_position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    //private void SetTimeout(Func<GameObject> function, int timeout)
    //{
    //    Task.Delay(timeout).ContinueWith((Task task) => function());
    //}

    void Awake()
    {   
        waveCounter = waves;
        gameMode = GameMode.idle;
    }

    private void Update()
    {
        if (UIMain.curLife <= 0) gameMode = GameMode.endGame;

        if (gameMode == GameMode.playing && !stillRolling)
        {
            Spawn();
            stillRolling = true;
        }

        if (UIMain.curLife <= 0)
        {
            MovementRealize.canMove = false;
            gameMode = GameMode.endGame;
        }
    }

    void Spawn()
    {
        if (gameMode == GameMode.endGame) return;

        if (waveCounter <= 0)
        {
            Invoke(nameof(Idle), idleDelay);
            return;
        }

        waveCounter--;
        QueueRand();
        
        for (float i = 0; i < 40; i += padding)
        {
            GameObject ball = Instantiate(prefabBall);
            ball.transform.position = transform.position;
            Vector3 pos = Vector3.zero;
            switch (q)
            {
                case 1:
                    pos = new Vector3(i, 1, -2);
                    break;
                case 2:
                    pos = new Vector3(-2, 1, i);
                    break;
                case 3:
                    pos = new Vector3(i, 1, 40);
                    break;
                case 4:
                    pos = new Vector3(40, 1, i);
                    break;
            }

            ball.transform.position = pos;

            //уничтожить шар с вероятностью 15-30% через 0.5-2 секунды либо гарантированно если образовался ряд из 6 шаров
            if (UnityEngine.Random.Range(0, 101) > UnityEngine.Random.Range(80, 90) || ballsInARow > 5)
            {
                ballsInARow = 0;
                float Delay = UnityEngine.Random.Range(0.5f, 2f) * 1000;
                int del = (int)Delay;

                BoomVFX(ball, del);

                Destroy(ball, Delay/1000);
                
                
            }
            else
            {
                ballsInARow++;
            }

        }
        Invoke(nameof(Spawn), waveDelay);
    }

    //афк режим между заходами
    void Idle()
    {
        waves += 2;
        waveCounter = waves;
        stillRolling = false;
        gameMode = GameMode.idle;
    }


    //рандом генерация очереди направления волн
    void QueueRand()
    {
        q = UnityEngine.Random.Range(1, 5);
        if (q == last || q == preLast)
        {
            QueueRand();
        }
        else
        {
            preLast = last;
            last = q;
            return;
        }
    }


    void OnGUI()
    {
        Counter();
    }
    void Counter()
    {
        uitWavesCountInfo.text = "Waves: " + waves;
    }

    public async void BoomVFX(GameObject ball, int del)
    {
        await Task.Delay(del-100);
        try
        {
            GameObject boomVFX = Instantiate(explodePrefab, ball.transform.position, ball.transform.rotation);
            Destroy(boomVFX, 2);
        }
        catch 
        {
            print("ball был рано сломан");
        }
        return;
    }
}