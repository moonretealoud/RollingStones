using UnityEngine;
using UnityEngine.UI;

public class BallsSpawn : GameController
{
    [Header("set myself")]
    public float idleDelay = 5f;
    public float waveDelay = 1f;
    public static float waves = 4;
    public GameObject prefabBall;
    public Text uitWavesCountInfo;

    private const float padding = 2.0f;

    [Header("set dynamically")]
    private float waveCounter;
    private int q, last, preLast;
    private bool stillRolling = false;
    public static GameMode gameMode;
    private float ballsInARow = 0;

    public Vector3 Current_position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    //private void SetTimeout(Func<bool> function, int timeout)
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
            stillRolling = false;
            gameMode = GameMode.endGame;
            waveCounter = 4;
            Invoke(nameof(RestartGame), 8f);
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
            GameObject go = Instantiate(prefabBall);
            go.transform.position = transform.position;
            Vector3 pos = Vector3.zero;
            switch (q)
            {
                case 1:
                    pos.x = i;
                    pos.y = 1;
                    pos.z = -2;
                    break;
                case 2:
                    pos.x = -2;
                    pos.y = 1;
                    pos.z = i;
                    break;
                case 3:
                    pos.x = i;
                    pos.y = 1;
                    pos.z = 40;
                    break;
                case 4:
                    pos.x = 40;
                    pos.y = 1;
                    pos.z = i;
                    break;
            }

            go.transform.position = pos;

            //уничтожить шар с вероятностью 20-30% через 0.5-1.5 секунд либо гарантированно если образовался ряд из 7 шаров
            if (Random.Range(0, 101) > Random.Range(70, 85) || ballsInARow > 7)
            {
                ballsInARow = 0;
                float Delay = Random.Range(0.5f, 1.5f);
                Destroy(go, Delay);
            } else
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
        q = Random.Range(1, 5);
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

}