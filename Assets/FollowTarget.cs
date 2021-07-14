using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float originalY;
    private void Awake()
    {
        target = transform.parent; // �θ� Ÿ������ �Ѵ�.
        originalY = transform.position.y; //Y�� ��ǥ�� ����

    }
    private void LateUpdate() //Ÿ��(�÷��̾�)�� ���� �̵��ϰ� �װ� �����ϴ°ű� ������ ������Ʈ���� ����Ǹ� �ȵǰ�(���ÿ�) / ������Ʈ���� ���� ����Ʈ���� ����
    {
        var newPos = target.position;
        newPos.y = originalY;

        transform.position = newPos; // ���󰡼�
    }

}
