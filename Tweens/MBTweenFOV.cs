using UnityEngine;

public class MBTweenFOV : MBTweenBase
{
    #region Fields

    [SerializeField] float startFOV;
    [SerializeField] float endFOV;

    [SerializeField] private Camera target;

    #endregion


    #region Properties


    #endregion


    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();

        if (target == null)
        {
            target = GetComponent<Camera>();
        }

        
    }

    #endregion


    #region Public Methods

    public static MBTweenFOV ChangeFOVTo(Camera target, float fov, float duration)
    {
        MBTweenFOV tween = target.GetComponent<MBTweenFOV>();
        if (tween == null)
        {
            tween = target.gameObject.AddComponent<MBTweenFOV>();
        }

        tween.startFOV = target.fieldOfView;
        tween.endFOV = fov;
        tween.SetEndState(0, duration);

        return tween;
    }

    #endregion


    #region Private Methods

    protected override void UpdateTweenWithFactor(float factor)
    {
        target.fieldOfView = Mathf.Lerp(startFOV, endFOV, factor);
    }

    #endregion


}
