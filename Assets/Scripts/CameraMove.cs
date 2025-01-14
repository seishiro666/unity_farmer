using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float velocityMove = 0.01f;
    bool isMove = false;
    public List<Transform> edges = new List<Transform>();

    // Ограничения по осям
    private float minX = -20f;
    private float maxX = 20f;
    private float minZ = -15f;
    private float maxZ = 20f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isMove = !isMove;
        }

        if (isMove)
        {
            Vector3 newPosition = transform.position;

            if (Input.mousePosition.x < edges[0].position.x && newPosition.x >= minX)
            {
                newPosition.x -= velocityMove;
            }
            else if (Input.mousePosition.x > edges[1].position.x && newPosition.x <= maxX)
            {
                newPosition.x += velocityMove;
            }

            if (Input.mousePosition.y < edges[2].position.y && newPosition.z >= minZ)
            {
                newPosition.z -= velocityMove;
            }
            else if (Input.mousePosition.y > edges[3].position.y && newPosition.z <= maxZ)
            {
                newPosition.z += velocityMove;
            }

            transform.position = new Vector3(Mathf.Clamp(newPosition.x, minX, maxX), newPosition.y, Mathf.Clamp(newPosition.z, minZ, maxZ));
        }
    }
}
