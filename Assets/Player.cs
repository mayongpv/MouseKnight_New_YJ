using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    public float speed = 5;
    public float moveableDistance = 3;
    public Transform mousePointer;
    public Transform spriteTr;
    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteTr = GetComponentInChildren<SpriteRenderer>().transform;
    }

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
    public enum StateType
    {
        Idle,
        JumpUp,
        JumpDown,
        Attack,
        Walk,
    }
    [SerializeField] StateType state = StateType.Idle;
    StateType State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;


            state = value;
            animator.Play(state.ToString());
        }
    }
    Animator animator;
    JumpStateType jumpState;
    public float jumpYMultiply = 1;
    public float jumpTimeMultiply = 1;
    private IEnumerator JumpCo() //점프하는 클래스 - 커브로 구현 : 커브 포인트 값에 따라 점프 형식이 달라진다. 
    {
        jumpState = JumpStateType.Jump; // 점프를 하면 점스 상태: 점프가 된다. 
        State = StateType.JumpUp;
        float jumpeStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; //포인트의 개수-1 의 시간 하면 끝나는 시간 
        float jumpEndTime = jumpeStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        float previousY = 0;

        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply);
            y *= jumpYMultiply;
            transform.Translate(0, y, 0);
            yield return null; //반환값에 대한 개념?

            if (previousY > y)
            {
                //떨어지는 모션으로 바꾸자
                State = StateType.JumpDown;
            }
            previousY = y;

            sumEvaluateTime *= Time.deltaTime;

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
                transform.Translate(dir * speed * Time.deltaTime, Space.World); ; // 움직이니까 이렇게 써주고  dir이 움직이는 방향, 

                //방향(dir) 따라서 
                // 오른쪽이면 y :0. sprite x = 45
                //왼쪽 Y : 180, sprite x = -45;

                bool isRightSide = dir.x > 0;
                if (isRightSide)
                {
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    spriteTr.rotation = Quaternion.Euler(45, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    spriteTr.rotation = Quaternion.Euler(-45, 180, 0);
                }

                if (jumpState != JumpStateType.Jump)
                    State = StateType.Walk;

            }
            else
            {
                if (jumpState != JumpStateType.Jump)
                    State = StateType.Idle; 
            }
        }
    }
}
