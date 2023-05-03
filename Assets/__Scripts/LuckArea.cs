using UnityEngine;
using System.Threading.Tasks;
using System;

public class LuckArea : MonoBehaviour
{
    private GameObject player;
    private GameObject lastTriggerGo;
    public GameObject explodePrefab;

    void Start()
    {
        player = GameObject.FindWithTag("Player"); // Find the GameObject named Player
    }



    // перемещать объект за Player
    void Update()
    {
        transform.position = player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        //защита от повторного триггера
        Transform rootT = other.gameObject.transform.root;
        GameObject ball = rootT.gameObject;

        if (ball == lastTriggerGo) return;

        lastTriggerGo = ball;

        if (ball.CompareTag("Ball"))
        {
            if (UnityEngine.Random.Range(0, 101) > UnityEngine.Random.Range(65, 75))
            {
                float Delay = UnityEngine.Random.Range(0.5f, 1.5f)*1000;
                int del = (int)Delay;

                BoomVFX(ball, del);

                Destroy(ball, Delay/1000);           
            }
        }
    }

    public async void BoomVFX(GameObject ball, int del)
    {
        await Task.Delay(del-100);
        try {
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
