using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    [Header("Set in")]
    public static int scoreHit = 100;
    public static float reward = 10;
    public GameObject vfxExplode;
    public static GameObject vfxExplPrefab;


    private void Start()
    {
        vfxExplPrefab = vfxExplode;
    }


    //упавшие вниз шары уничтожаются когда сталкиваются с плитой
    public void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go.CompareTag("Ball"))
        {
            UIMain.gold += reward;
            UIMain.score += scoreHit;

            GameObject explode = Instantiate(vfxExplode);
            explode.transform.position = go.transform.position;
            
            Destroy(go);
            Destroy(explode, 2);
        }
    }
}
