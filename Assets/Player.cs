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
    private IEnumerator JumpCo() //�����ϴ� Ŭ���� - Ŀ��� ���� : Ŀ�� ����Ʈ ���� ���� ���� ������ �޶�����. 
    {
        jumpState = JumpStateType.Jump; // ������ �ϸ� ���� ����: ������ �ȴ�. 
        State = StateType.JumpUp;
        float jumpeStartTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time; //����Ʈ�� ����-1 �� �ð� �ϸ� ������ �ð� 
        float jumpEndTime = jumpeStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        float previousY = 0;

        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply);
            y *= jumpYMultiply;
            transform.Translate(0, y, 0);
            yield return null; //��ȯ���� ���� ����?

            if (previousY > y)
            {
                //�������� ������� �ٲ���
                State = StateType.JumpDown;
            }
            previousY = y;

            sumEvaluateTime *= Time.deltaTime;

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
                transform.Translate(dir * speed * Time.deltaTime, Space.World); ; // �����̴ϱ� �̷��� ���ְ�  dir�� �����̴� ����, 

                //����(dir) ���� 
                // �������̸� y :0. sprite x = 45
                //���� Y : 180, sprite x = -45;

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
