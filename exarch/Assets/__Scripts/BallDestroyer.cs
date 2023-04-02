using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    [Header("Set in")]
    public float scoreHit = 100;
    public float reward = 10;

    
    //������� ���� ���� ������������ ����� ������������ � ������
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
