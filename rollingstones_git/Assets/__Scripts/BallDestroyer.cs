using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    [Header("Set in")]
    public float scoreHit = 100;
    public float reward = 10;

    
    //упавшие вниз шары уничтожаются когда сталкиваются с плитой
    public void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go.CompareTag("Ball"))
        {
            UIMain.gold += reward;
            UIMain.score += scoreHit;
            Destroy(go);
        }
    }
}
