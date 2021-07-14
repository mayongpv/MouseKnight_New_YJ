using System.Collections;
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
        if (jumpState == JumpStateType.Jump) // ���°� ������ ���� ���ϵ��� - > ������
            return;
        if (Input.GetKeyDown(KeyCode.Mouse1))// �� �����߿��� �� �ڵ尡 ������ �ȵ�
        {
            StartCoroutine(JumpCo());
        } 
    }
    public enum JumpStateType // �������̶�� ���� ���ϰ� �Ϸ��� �߰���. 
    {
        Ground,
        Jump,
    }
    JumpStateType jumpState;
    public float jumpYMultiply = 1;
    public float jumpTimeMultiply = 1;
    private IEnumerator JumpCo() //�����ϴ� Ŭ���� - Ŀ��� ���� : Ŀ�� ����Ʈ ���� ���� ���� ������ �޶�����. 
    {
        jumpState = JumpStateType.Jump; // ������ �ϸ� ���� ����: ������ �ȴ�. 
        float jumpeStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; //����Ʈ�� ����-1 �� �ð� �ϸ� ������ �ð� 
        float jumpEndTime = jumpeStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply);
            y *= jumpYMultiply;
            transform.Translate(0, y, 0);
            yield return null; //��ȯ���� ���� ����?
            sumEvaluateTime += Time.deltaTime;
        }
        jumpState = JumpStateType.Ground;
    }

    private void Move() // ���콺 Ŭ������ �����̴� Ŭ����
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
