using UnityEngine;
using System;

public class MBTweenBezierSplineMover : MBTweenBase 
{
    public enum FollowVector
    {
        FollowForward,
        FollowBackward,
        FollowLeft,
        FollowRight,
        FollowUp,
        FollowDown
    }


    #region Fields
    [SerializeField] BezierSpline spline;
    [SerializeField] Transform target;

    [SerializeField] int curveIndex = 0;

    [SerializeField] bool applyDirection;
    [SerializeField] FollowVector followVector;
    [SerializeField] float followDamping = 0.3f;

    EasingMethod initialEasingMethod = EasingMethod.None;

    #endregion 

    #region Properties

    public BezierSpline Spline
    {
        get
        {
            return spline;
        }
        set
        {
            spline = value;
        }
    }

    public int CurveIndex
    {
        get
        {
            return curveIndex;
        }
        set
        {
            curveIndex = value;
        }
    }

    public FollowVector TweenFollowVector
    {
        get
        {
            return followVector;
        }
        set
        {
            followVector = value;
        }
    }


    #endregion


    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();

        if (target == null)
        {
            target = transform;
        }
    }

    #endregion

    #region Public Methods

    public void MoveBetweenSplines(int firstIndex, int secondIndex, float speed, Action OnFinishedAction = null)
    {
        if (easingMethod != EasingMethod.None)
        {
            initialEasingMethod = easingMethod;
            easingMethod = EasingMethod.None;
        }

        if (firstIndex == secondIndex)
        {
            OnCompleteAction = null;

            easingMethod = initialEasingMethod;

            if (OnFinishedAction != null)
            {
                OnFinishedAction();
            }

            return;
        }

        int step = firstIndex > secondIndex ? -1 : 1;

        OnCompleteAction = delegate
        {
            MoveBetweenSplines(firstIndex + step, secondIndex, speed, OnFinishedAction);
        };

        if (step > 0)
        {
            curveIndex = firstIndex;

            Vector3[] points = spline.GetCurve(curveIndex);
            float curveLength = Bezier.GetBezierLength(points[0], points[1], points[2], points[3]);

            SetEndState(0, curveLength / speed);
        }
        else
        {
            curveIndex = firstIndex - 1;

            Vector3[] points = spline.GetCurve(curveIndex);
            float curveLength = Bezier.GetBezierLength(points[0], points[1], points[2], points[3]);

            SetBeginState(0, curveLength / speed);
        }
    }



    #endregion


    #region Private Methods

    protected override void UpdateTweenWithFactor(float factor)
    {
        if (spline == null)
        {
            enabled = false;
            return;
        }

        target.position = spline.GetPointForCurveAtIndex(factor, curveIndex);
        if (applyDirection)
        {
            float damping = factor < 0.95 ? followDamping : 1;

            switch (followVector)
            {
                case FollowVector.FollowForward:
                    target.forward = Vector3.Lerp(target.forward, spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;

                case FollowVector.FollowBackward:
                    target.forward = Vector3.Lerp(target.forward, -spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;

                case FollowVector.FollowDown:
                    target.up = Vector3.Lerp(target.up, -spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;

                case FollowVector.FollowLeft:
                    target.right = Vector3.Lerp(target.right, -spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;

                case FollowVector.FollowRight:
                    target.right = Vector3.Lerp(target.right, spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;

                case FollowVector.FollowUp:
                    target.up = Vector3.Lerp(target.up, spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;

                default:
                    target.forward = Vector3.Lerp(target.forward, spline.GetDirectionForCurveAtIndex(factor, curveIndex), damping);
                    break;
            }
        }
    }

    #endregion
}