using UnityEngine;

public class MBTweenScale : MBTweenBase 
{
    #region Fields

    [SerializeField] Vector3 startScale;
    [SerializeField] Vector3 endScale;

    [SerializeField] Transform target;
    [SerializeField] RectTransform uiTarget;

    #endregion


    #region Properties

    Vector3 Scale
    {
        get
        {
            if (target != null)
            {
                return target.localScale;
            }
            else if (uiTarget != null)
            {
                return uiTarget.localScale;
            }

            return Vector3.zero;
        }

        set
        {
            if (target != null)
            {
                target.localScale = value;
            }
            else if (uiTarget != null)
            {
                uiTarget.localScale = value;
            }
        }
    }


    #endregion


    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();

        if (target == null && uiTarget == null)
        {
            uiTarget = transform as RectTransform;

            if (uiTarget == null)
            {
                target = transform;
            }
        }

        
    }

    #endregion


    #region Public Methods

    public static MBTweenScale ScaleTo(Transform target, Vector3 scale, float duration)
    {
        MBTweenScale tween = target.GetComponent<MBTweenScale>();
        if (tween == null)
        {
            tween = target.gameObject.AddComponent<MBTweenScale>();
        }

        tween.startScale = target.localScale;
        tween.endScale = scale;
        tween.SetEndState(0, duration);

        return tween;
    }

    #endregion


    #region Private Methods

    protected override void UpdateTweenWithFactor(float factor)
    {
	    Scale = startScale + (endScale - startScale) * factor;
    }

    #endregion


}
