using UnityEngine;

public class FollowTargetCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset; // 출발이라는 뜻 -카메라 위치?

    public BoxCollider movableArea;
    public float minX, maxX, minZ, maxZ;
    void Start()
    {
        offset = target.position - transform.position;
        minX = movableArea.center.x - movableArea.size.x/2; // 가장 낮은 좌표 값. 즉 왼쪽으로 제일 많이  간 지점이 X 값이 제일 작으니, X의 최소값이 된다. 
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
