using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float _parallaxFactor;

    private Transform _cameraTransform;
    private Vector3 _lastCamPos;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCamPos = _cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = _cameraTransform.position - _lastCamPos;
        transform.position += delta * _parallaxFactor;
        _lastCamPos = _cameraTransform.position;
    }
}
