using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float moveableDistance = 3;
    public Transform mousePointer;

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mousePointer.position = hit.point;
            float distance = Vector3.Distance(hit.point, transform.position); //두개 사이의 거리를 측정하겠다. 
            if (distance > moveableDistance)
            {
                var dir = hit.point - transform.position;
                dir.Normalize();
                transform.Translate(dir *speed* Time.deltaTime);  ; // 움직이니까 이렇게 써주고  dir이 움직이는 방향, 

            }
        }
    }
}
