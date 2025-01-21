using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Original code provided by https://discussions.unity.com/t/object-rotation-clamping/798032/5
//This code has been modified.

public class AxisClampHelper : MonoBehaviour
{
    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    //public float Sensitvity = 2f;

    [SerializeField]
    private Axis _axis = Axis.X;

    [SerializeField]
    private float _rotationSpeed = 5f;

    [SerializeField]
    private float _minAngle = -90f,
                    _maxAngle = 90f;

    private Quaternion _targetRot;
    private Vector3 _rotationAxis = Vector3.zero;

    private void Start()
    {
        _targetRot = transform.rotation;
    }

    private void Update()
    {
        // var input = Input.GetAxis("Horizontal") * Sensitvity;

        // _rotationAxis[(int)_axis] = input;

        _targetRot *= Quaternion.Euler(_rotationAxis);

        _targetRot = ClampAngleOnAxis(_targetRot, (int)_axis, _minAngle, _maxAngle);

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, Time.deltaTime * _rotationSpeed);
    }

    private Quaternion ClampAngleOnAxis(Quaternion q, int axis, float minAngle, float maxAngle)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        var angle = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q[axis]);

        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        q[axis] = Mathf.Tan(0.5f * Mathf.Deg2Rad * angle);

        return q;
    }
}
