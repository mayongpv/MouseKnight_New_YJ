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

    public AnimationCurve jumpYac; //jumpYac 점프할 때 y 애니메이션
    private void Jump()
    {
        if (jumpState == JumpStateType.Jump) // 상태가 점프면 점프 못하도록 - > 나간다
            return;
        if (Input.GetKeyDown(KeyCode.Mouse1))// 즉 점프중에는 이 코드가 실행이 안됨
        {
            StartCoroutine(JumpCo());
        } 
    }
    public enum JumpStateType // 점프중이라면 점프 못하게 하려고 추가함. 
    {
        Ground,
        Jump,
    }
    JumpStateType jumpState;
    public float jumpYMultiply = 1;
    public float jumpTimeMultiply = 1;
    private IEnumerator JumpCo() //점프하는 클래스 - 커브로 구현 : 커브 포인트 값에 따라 점프 형식이 달라진다. 
    {
        jumpState = JumpStateType.Jump; // 점프를 하면 점스 상태: 점프가 된다. 
        float jumpeStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; //포인트의 개수-1 의 시간 하면 끝나는 시간 
        float jumpEndTime = jumpeStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply);
            y *= jumpYMultiply;
            transform.Translate(0, y, 0);
            yield return null; //반환값에 대한 개념?
            sumEvaluateTime += Time.deltaTime;
        }
        jumpState = JumpStateType.Ground;
    }

    private void Move() // 마우스 클릭으로 움직이는 클래스
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            mousePointer.position = hitPoint;
            float distance = Vector3.Distance(hitPoint, transform.position); //두개 사이의 거리를 측정하겠다. 
            if (distance > moveableDistance)
            {
                var dir = hitPoint - transform.position;
                dir.Normalize();
                transform.Translate(dir * speed * Time.deltaTime); ; // 움직이니까 이렇게 써주고  dir이 움직이는 방향, 

            }
        }
    }
}
