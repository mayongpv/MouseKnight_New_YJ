using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float moveableDistance = 3;
    public Transform mousePointer;

    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    void Update()
    {
        Move();
        Jump();

    }

    public AnimationCurve jumpYac; //jumpYac ������ �� y �ִϸ��̼�
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartCoroutine(JumpCo());
        }
    }
    private IEnumerator JumpCo()
    {
        float jumpeStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; 
        float jumpEndTime = jumpeStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime);
            transform.Translate(0, y, 0);
            yield return null;
            sumEvaluateTime += Time.deltaTime;
        }
    }

    private void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            mousePointer.position = hitPoint;
            float distance = Vector3.Distance(hitPoint, transform.position); //�ΰ� ������ �Ÿ��� �����ϰڴ�. 
            if (distance > moveableDistance)
            {
                var dir = hitPoint - transform.position;
                dir.Normalize();
                transform.Translate(dir * speed * Time.deltaTime); ; // �����̴ϱ� �̷��� ���ְ�  dir�� �����̴� ����, 

            }
        }
    }
}
