using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset; // ����̶�� �� -ī�޶� ��ġ?

    public BoxCollider movableArea;
    public float minX, maxX, minZ, maxZ;
    void Start()
    {
        offset = target.position - transform.position;
        minX = movableArea.center.x - movableArea.size.x/2; // ���� ���� ��ǥ ��. �� �������� ���� ����  �� ������ X ���� ���� ������, X�� �ּҰ��� �ȴ�. 
        maxX = movableArea.center.x + movableArea.size.x/2;
        minZ = movableArea.center.z - movableArea.size.z/2;
        maxZ = movableArea.center.z + movableArea.size.z/2;
    }

    void LateUpdate()
    {
        var newPos = target.position - offset;//target.position - target.position + transform.position = transform.position?
        newPos.x = Mathf.Min(newPos.x, maxX);
        newPos.x = Mathf.Max(newPos.x, minX);

        newPos.z = Mathf.Min(newPos.z, maxX);
        newPos.z = Mathf.Min(newPos.z, maxX);

        transform.position = newPos;
    }
}
