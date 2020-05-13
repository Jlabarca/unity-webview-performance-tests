using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static UnityEngine.Mathf;

public class MoveAround : MonoBehaviour
{
    private float  _currentDist;
    private float  _angleH, _angleV;

    public Transform target;
    public float speed = 1;
    public float period = 2;

    public void Start()
    {
        InvokeRepeating(nameof(RandomAngles), 0, period);
    }

    private void RandomAngles()
    {
        _angleH += Random.Range(0, 360);
        _angleV += Random.Range(0, 360);
        _currentDist = Random.Range(6, 10);
    }

    [SuppressMessage("ReSharper", "ComplexConditionExpression")]
    public void LateUpdate()
    {
        Vector3 tmp;
        var position = target.position;
        tmp.x = Cos(f: _angleH * (PI / 180)) * Sin(_angleV * (PI / 180)) * _currentDist + position.x;
        tmp.z = Sin(_angleH * (PI / 180)) * Sin(_angleV * (PI / 180)) * _currentDist + position.z;
        tmp.y = Sin(_angleV * (PI / 180)) * _currentDist + position.y;
        transform.position = Vector3.Slerp(transform.position, tmp, speed * Time.deltaTime);
        transform.LookAt(target);
    }
}
