using UnityEngine;

public class CameraTopDown : MonoBehaviour
{
    public float xz = -7.5f;
    public float y = 16;
    public float rotate = 58f;
    private GameObject player;
    private Vector3 offsetPos;

    void Start()
    {
        player = GameObject.FindWithTag("Player"); // Find the GameObject named Player
        offsetPos = new Vector3(xz, y, xz); // Set the camera's offset position
    }

    void OnEnable()
    {
        gameObject.transform.parent = null; // This makes the camera a parent object rather than a child
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(rotate, 45f, 0); // Set the camera's rotation
        transform.position = player.transform.position + offsetPos; // Set cameras final position     
        offsetPos = new Vector3(xz, y, xz);//отладка
    }
}