using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public static Player instance;
    private void Awake()
    {
        instance = this;
        state = StateType.NotBegin;
    }


    [SerializeField] StateType state = StateType.Idle;

    public float speed = 5;
    float normalSpeed;
    public float walkDiastance = 12;
    public float stopdistance = 7;
    public Transform mousePointer;
    public Transform spriteTr;
    Plane plane = new Plane(new Vector3(0, 1, 0), 0);

    StateType State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;

            if (EditorOption.Options[OptionType.Player상태변화로그])
                Debug.Log($"state:{state}=>value:{value}");

            state = value;
            animator.Play(state.ToString());
        }
    }


    NavMeshAgent agent;

    private void Start()
    {
        normalSpeed = speed;
        animator = GetComponentInChildren<Animator>();
        spriteTr = GetComponentInChildren<SpriteRenderer>().transform;
        agent = GetComponent<NavMeshAgent>();
        spriteTrailRenderer = GetComponentInChildren<SpritTrailRenderer>();
        spriteTrailRenderer.enabled = false;


    }

    void Update()
    {
        if (CanMoceState())
        {
            Move();
            Jump();
        }

        bool isSucceedDash = Dash();
        Attack(isSucceedDash);
    }

    private bool CanMoceState()
    {
        if (State == StateType.Attack)
            return false;
        if (State == StateType.TakeHit)
            return false;
        if (State == StateType.Death)
            return false;

        return true;
    }
    private void Attack(bool isSucceedDash)
    {
        if (isSucceedDash)
            return;

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StartCoroutine(AttackCo());
        }

    }
    public float attackTime = 1;
    public float attackApplyTime = 0.2f;
    public LayerMask enermyLayer;
    public SphereCollider attackCollider;
    public float power = 10;

    private IEnumerator AttackCo()
    {
        State = StateType.Attack;
        yield return new WaitForSeconds(attackApplyTime);

        Collider[] enemyColliders = Physics.OverlapSphere(
            attackCollider.transform.position, attackCollider.radius.enemyLayer);
        foreach (var item in enemyColliders)
        {
            item.GetComponent<Goblin>().TakeHit(power);
        }

        yield return new WaitForSeconds(attackTime);
        State = StateType.Idle;
    }
    [Foldout("대시")] public float dashableDistance = 10;
    [Foldout("대시")] public float dashableTime = 0.4f;

    float mouseDownTime;
    Vector3 mouseDownPosition;
    private bool Dash()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseDownTime = Time.time;
            mouseDownPosition = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            bool isDashDrag = IsSucceesDashDrag();
            if (isDashDrag)
            {
                StartCoroutine(DashCo());
                return true;
            }
        }

        return false;
    }

    [Foldout("대사")] public float dashTime = 0.3f;

    public float hp = 100;
    internal void TakeHit(int damage)
    {
        if (State == StateType.Death)
            return;

        hp -= damage;
        StartCoroutine(TakeHitCo());

    }

    public float takeHitTime = 0.3f;
    private IEnumerator TakeHitCo()
    {
        State = StateType.TakeHit;
        yield return new WaitForSeconds(takeHitTime);

        if (hp > 0)
            State = StateType.Idle;
        else
            StartCoroutine(DeathCo());
    }

    public float deathTime = 0.5f;

    private IEnumerator DeathCo()
    {
        State = StateType.Death;
        yield return new WaitForSeconds(deathTime);
        Debug.LogWarning("게임종료");
    }

    SpriteTrailRenderer.SpriteTrailRenderer spriteTrailRenderer;
    [Foldout("대시")] public float dashSpeedMultiplySpeed = 4f;
    Vector3 dashDirection;
    private IEnumerator DashCo()
    {
        /* 방향을 바꿀 수 없게 -> 진행방향으로 이동 -> 대각선으로 이동-> 드래그 방향 이동할건지
         * 플레이어 이동방향 X 이동할건지
         * DashDirection x 방향만 사용
         */
        spriteTrailRenderer.enabled = true;
        dashDirection = Input.mousePosition - mouseDownPosition;
        dashDirection.y = 0;
        dashDirection.x = 0;
        dashDirection.Normalize();
        speed = normalSpeed * dashSpeedMultiplySpeed;
        State = StateType.Dash;
        yield return new WaitForSeconds(dashTime);
        speed = normalSpeed;
        State = StateType.Idle;
        spriteTrailRenderer.enabled = false;
    }

    private bool isSucceesDashDrag()
    {
        //시간 체크
        float dragTime = Time.time - mouseDownTime;
        if (dragTime > dashableTime)
            return false;

        float dragDistance = Vector3.Distance(mouseDownPosition, Input.mousePosition);
        if (dragDistance < dashableDistance)
            return false;

        return true;
    }


    [BoxGroup("Jump")] public AnimationCurve jumpYac; //jumpYac 점프할 때 y 애니메이션
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
        NotBegin,
        Idle,
        Walk,
        JumpUp,
        JumpDown,
        Dash,
        Attack,
        TakeHit,
        Death
    }

    Animator animatior;
    JumpStateType jumpStateType;
    [BoxGroup("점프")] public float jumpYMultiply = 1;
    [BoxGroup("점프")] public float jumpTimeMultiply = 1;

    private IEnumerator JumpCo()
    {
        jumpStateType = JumpStateType.Jump;
        State = StateType.JumpUp;
        float jumpStateTime = Time.time;
        float jumpDuration = jumpYac[jumpYac.length - 1].time;
        jumpDuration *= jumpTimeMultiply;
        float jumpEndTime = jumpStartTime + jumpDuration;
        float sumEvaluateTime = 0;
        float previousY = float.MinValue;
        agent.enabled = false;

        while (Time.time < jumpEndTime)
        {
            float y = jumpYac.Evaluate(sumEvaluateTime / jumpTimeMultiply);
            y *= jumpYMultiply * Time.deltaTime;
            transform.Translate(0, y, 0);
            yield return null;

            if (previousY > transform.position.y)
            {
                State = StateType.JumpDown;
            }
            if (transform.position.y < 0)
            {
                break;
            }

            previousY = transform.position.y;
            sumEvaluateTime += Time.deltaTime;
        }
        agent.enabld = true;
        jumpState = JumpStateType.Ground;
        State = StateType.Idle;

    }

    private void Move() // 마우스 클릭으로 움직이는 클래스
    {
        if (Time.timeScale == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            mousePointer.position = hitPoint;
            float distance = Vector3.Distance(hitPoint, transform.position); //두개 사이의 거리를 측정하겠다. 

            float movableDistance = stopDistance;
            // State가 Walk 일땐 7(stopDistance)사용.
            // Idle에서 Walk로 갈땐 12(WalkDistance)사용
            if (State == StateType.Idle)
                movableDistance = walkDistance;

            var dir = hitPoint - transform.position;

            if (State == StateType.Dash)
                dir.Normalize();

            if (distance > movableDistance || State == StateType.Dash)
            {
                transform.Translate(dir * speed * Time.deltaTime, Space.World); ; // 움직이니까 이렇게 써주고  dir이 움직이는 방향, 
                if (ChangeableState())
                    State = StateType.Walk;
            }
            else
            {
                if (ChangeableState())
                    State = StateType.Idle;
            }

            //방향(dir)에 따라서
            //오른쪽이라면 Y : 0
            //왼쪽이라면 Y : 180

            bool isRightSide = dir.x > 0;
            if (isRightSide)
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            bool ChangeableState()
            {
                if (jumpState == JumpStateType.Jump)
                    return false;

                if (state == StateType.Dash)
                    return false;

                return true;
            }
        }
    }
}


