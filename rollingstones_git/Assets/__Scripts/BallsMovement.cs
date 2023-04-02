using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsMovement : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float speed = 6.3f;
    public float rotSpeed = 0.3f;
    public GameObject prefabBall;
    private float xA, zA;
    private bool outOfBounds = false;

    Rigidbody rb;
    public Vector3 Current_position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        OutOfBounds(); //проверка на выход за пределы арены.

        if (!outOfBounds) Move();  //шары катятся до края арены
    }

    public virtual void Move()
    {
        Vector3 tempPos = Current_position;
        if (tempPos.z == -2f)        { xA = 0; zA = 1; }//Right Down
        if (tempPos.x == -2f)        { xA = 1; zA = 0; }//Left Down
        if (tempPos.z == 40f)        { xA = 0; zA = -1; }//Right Up
        if (tempPos.x == 40f)        { xA = -1; zA = 0; }//Left Up


        tempPos.z += speed * Time.deltaTime * zA;
        tempPos.x += speed * Time.deltaTime * xA;
        

        if (xA == 1 || xA == -1)
        {
            float rAxis = (rotSpeed * Time.time * 360) % 360f;
            transform.rotation = Quaternion.Euler(0, 0, rAxis * -xA);
        }
        if (zA == 1 || zA == -1)
        {
            float rAxis = (rotSpeed * Time.time * 360) % 360f;
            transform.rotation = Quaternion.Euler(rAxis * zA, 0, 0);
        }

        Current_position = tempPos;        
    }

    void OutOfBounds()
    {
        Vector3 tempPos = Current_position;
        if ((tempPos.z < -3f) || (tempPos.x < -3f) || (tempPos.z > 41f) || (tempPos.x > 41f)) 
        {
            speed = 3.0f;//замедление скорости во время падения для более натуральной траектории
            rb.isKinematic = false;//выключение кинематичености для эффекта гравитации
            
        } 
    }
}
