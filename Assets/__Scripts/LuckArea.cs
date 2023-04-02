using UnityEngine;

public class LuckArea : MonoBehaviour
{
    private GameObject player;
    private GameObject lastTriggerGo;

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
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;

        lastTriggerGo = go;

        if (go.CompareTag("Ball"))
        {
            if (Random.Range(0, 101) > Random.Range(70, 85))
            {
                float Delay = Random.Range(0.5f, 1f);
                Destroy(go, Delay);
            }
        }
    }
}
