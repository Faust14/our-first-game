using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
  [Header("Koga pratimo")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Glajdanje")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.15f;

    [Header("Granice mape (world pozicije)")]
    public Vector2 minBounds;   // minimalni X,Y
    public Vector2 maxBounds;   // maksimalni X,Y

    private void LateUpdate()
    {
        if (target == null) return;

        // željena pozicija
        Vector3 desiredPos = target.position + offset;

        // clamp unutar granica mape
        float clampedX = Mathf.Clamp(desiredPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(desiredPos.y, minBounds.y, maxBounds.y);

        Vector3 clampedPos = new Vector3(clampedX, clampedY, offset.z);

        // glatko približavanje
        transform.position = Vector3.Lerp(transform.position, clampedPos, smoothSpeed);
    }
}
