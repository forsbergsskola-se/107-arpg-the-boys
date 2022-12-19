using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Header("Orbit Camera")] 
    public Transform target;
    public Vector3 orbitOffset = new(0.5f, 1.4f, 0f);
    public float sharpness = 2;
    public float bossToPlayerCamLerp = 0.5f;
    public float maxDistanceFromTarget = 10;

    private void Update()
    {
        AdvancedCamera();
    }

    private void AdvancedCamera()
    {
        OrbitAdjustPosition();
        OrbitAdjustRotation();
    }

    private void OrbitAdjustRotation()
    {
        transform.rotation = Quaternion.LookRotation(-orbitOffset);
    }

    private void OrbitAdjustPosition()
    {
        Vector3 desiredPos = target.position + orbitOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, sharpness * Time.deltaTime);
    }

    private void BossRoomCamera()
    {
        //Vector3 desiredPos = Vector3.Lerp()
    }
}