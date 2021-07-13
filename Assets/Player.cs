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
            float distance = Vector3.Distance(hit.point, transform.position); //�ΰ� ������ �Ÿ��� �����ϰڴ�. 
            if (distance > moveableDistance)
            {
                var dir = hit.point - transform.position;
                dir.Normalize();
                transform.Translate(dir *speed* Time.deltaTime);  ; // �����̴ϱ� �̷��� ���ְ�  dir�� �����̴� ����, 

            }
        }
    }
}
