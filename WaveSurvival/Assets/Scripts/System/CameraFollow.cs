using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    void LateUpdate()
    {
        if (_target != null)
        {
            transform.position = new Vector3(
                _target.position.x,
                _target.position.y,
                -10f
            );
        }
    }
}
