using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Goblin : MonoBehaviour
{
    public float watchRange = 25;
    public float attackRange = 10;
    public float speed = 40;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, watchRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange)

    }
    Image hpBar;
    Animator animator;
    Player target;
    Coroutine fsmHandle;
    IEnumerator Start()
    {
        target = Player.instance;
        animator = GetComponentInChildren<Animator>();
        //originalSpeed = speed;
        animator.Play("Idle");

        //fsm = IdleCo;

        //while (true)
        //{
        //    fsmChange = false;


        //}
    }
    IEnumerator IdleCo()
    {
        while (Vector3.Distance(target.transform.position, transform.position) > watchRange) // ÀÌ°Ô ¹»±î?
            yield return null;

        Fsm = ChaseTargetFSM;
    }

    IEnumerator ChseTargetFSM()
    {
        animator.Play("Run");
        while (fsmChange == false)
        {
            Vector3 toPlayerDirection = transform.position - target.transform.position;
            toPlayerDirection.y = 0;
            toPlayerDirection.Normalize();

            transform.Translate(toPlayerDirection * speed * Time.deltaTime);
            if (toPlayerDirection.x > 0)
            
                animator.transform.rotation = Quaternion.Euler(0, 180, 0);
                else
                    animator.transform.rotation = Quaternion.Euler(0, 180, 0);

            yield return null;
            if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
            {
                Fsm = AttackFsm;
            }

            IEnumerator AttackFsm()
            {
                animator.Play("Attack");

            }


        }

    }
}
