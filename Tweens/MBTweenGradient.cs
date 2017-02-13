using UnityEngine;
using UnityEngine.UI;

public class MBTweenGradient : MBTweenBase
{
    #region Fields

    [SerializeField]
    private Color startColorTop;
    [SerializeField]
    private Color endColorTop;
    [SerializeField]
    private Color startColorBottom;
    [SerializeField]
    private Color endColorBottom;
    [SerializeField]
    private GameObject target;

    private UIGradient gradient = null;

    #endregion

    #region Properties

    private Color ColorTop
    {
        get
        {
            //UI Gradient element
            if (gradient != null)
            {
                return gradient.TopColor;
            }

            return endColorTop;
        }
        set
        {
            if (gradient != null)
            {
                gradient.TopColor = value;
            }
        }
    }

    private Color ColorBottom
    {
        get
        {
            //UI Gradient element
            if (gradient != null)
            {
                return gradient.BottomColor;
            }

            return endColorBottom;
        }
        set
        {
            if (gradient != null)
            {
                gradient.BottomColor = value;
            }
        }
    }

    #endregion

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();

        if (target != null)
        {
            gradient = target.GetComponent<UIGradient>();   
        }
        else
        {
            gradient = GetComponent<UIGradient>();
        }

    }

    #endregion

    #region Public Methods
    #endregion

    #region Private Methods

    protected override void UpdateTweenWithFactor(float factor)
    {
        ColorTop = Color.Lerp(startColorTop, endColorTop, factor);
        ColorBottom = Color.Lerp(startColorBottom, endColorBottom, factor);
    }

    #endregion


    #region Event Handlers
    #endregion
}