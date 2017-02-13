using UnityEngine;
using System.Collections;

public class MBTweenPosition : MBTweenBase 
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] Transform target;

    protected override void Awake()
    {
        base.Awake();

        if (target == null)
        {
            target = transform;
        }
    }

    protected override void UpdateTweenWithFactor(float factor)
    {
        target.localPosition = startPosition + (endPosition - startPosition) * factor;

    }
}
