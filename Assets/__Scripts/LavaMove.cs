using UnityEngine;

public class LavaMove : MonoBehaviour
{
    public float speed = 1f;
    public GameObject plane;

    public Vector3 Current_position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    void Update()
    {
        Move();
    }

    //скрипт бесшовного бесконечного фонового движения текстуры лавового пепла
    void Move()
    {
        Vector3 tempPos = Current_position;
        tempPos.x += speed * Time.deltaTime;

        if (tempPos.x >= 200) tempPos.x = -200;

        Current_position = tempPos;
    }
}
