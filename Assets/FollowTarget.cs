using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float originalY;
    private void Awake()
    {
        target = transform.parent; // 부모를 타겟으로 한다.
        originalY = transform.position.y; //Y의 좌표는 고정

    }
    private void LateUpdate() //타겟(플레이어)가 먼저 이동하고 그걸 추적하는거기 때문에 업데이트에서 실행되면 안되고(동시에) / 업데이트보다 늦은 레이트에서 실행
    {
        var newPos = target.position;
        newPos.y = originalY;

        transform.position = newPos; // 따라가셈
    }

}
