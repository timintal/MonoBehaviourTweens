using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using PGR;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EasingMethod
{
    None,
    Curve,
    EaseIn,
    EaseOut,
    EaseInOut
}

public enum LoopType
{
    None,
    Loop,
    PingPong
}

public class MBTweenBase : MonoBehaviour 
{
    #region Events

    public Action OnCompleteAction;

    #endregion


    #region Fields
    [HideInInspector] public float duration;
    [HideInInspector] public float durationScale = 1;

    [HideInInspector] public float delay;

    [HideInInspector] public EasingMethod easingMethod;
    [HideInInspector] public EasingType easingType;
    [HideInInspector] public AnimationCurve curve;
    [HideInInspector] public LoopType looping;

    [HideInInspector] public bool ignoreTimeScale;
   
    [HideInInspector] public UnityEvent OnEndStateSet;
    [HideInInspector] public UnityEvent OnBeginStateSet;

    float tweenFactor;
    float currentTime;
    float currentDelay;
    float timeStepMultiplier;

#if UNITY_EDITOR
    private double previousEditorTime;
    private float editorDeltaTime;
#endif

    #endregion

    #region Properties

    public virtual bool IsInBeginState
    {
        get
        {
            if (enabled || timeStepMultiplier > 0)
            {
                return false;
            }

            if (currentTime / (duration * durationScale) > float.Epsilon)
            {
                return false;
            }

            return true;
        }
    }

    public virtual bool IsInEndState
    {
        get
        {
            if (enabled || timeStepMultiplier < 0)
            {
                return false;
            }

            if (currentTime / (duration * durationScale) < 1f - float.Epsilon)
            {
                return false;
            }

            return true;
        }
    }

    #endregion


    #region Unity Lifecycle



    protected virtual void Awake()
    {
        if (enabled)
        {
            SetBeginStateImmediately();
            SetEndState();
        }
    }

#if UNITY_EDITOR
    void EditorUpdate()
    {
        editorDeltaTime = (float)(EditorApplication.timeSinceStartup - previousEditorTime);
        previousEditorTime = EditorApplication.timeSinceStartup;
        if (!EditorApplication.isPlaying)
        {
            Update();
        }
    }
#endif

    float GetDeltaTime()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            return editorDeltaTime;
        }
#endif
        return ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
    }

    void Update()
    {
        float currentDeltaTime = GetDeltaTime();

        //check Delay
        currentDelay += currentDeltaTime;

        if (currentDelay < delay)
        {
            return;
        }

        currentTime += currentDeltaTime * timeStepMultiplier;

        if (timeStepMultiplier > 0 && currentTime > duration * durationScale)
        {
            tweenFactor = GetTweenFactor(1);
            enabled = false;

            if (OnCompleteAction != null)
            {
                OnCompleteAction();
            }

            EndStateSet();
        }
        else if (timeStepMultiplier < 0 && currentTime < 0)
        {
            tweenFactor = GetTweenFactor(0);
            enabled = false;

            if (OnCompleteAction != null)
            {
                OnCompleteAction();
            }

            BeginStateSet();
        }
        else
        {
            tweenFactor = GetTweenFactor(currentTime / (duration * durationScale));
        }

        UpdateTweenWithFactor(tweenFactor);  
    }


    #endregion


    #region Public Methods

    public void SetBeginState()
    {
        SetBeginState(delay, duration);
    }

    public void SetBeginState(float newDelay, float newDuration)
    {
        delay = newDelay;
        this.duration = newDuration;

        tweenFactor = 1;
        enabled = true;
        timeStepMultiplier = -1;
        currentTime = this.duration * durationScale;
        currentDelay = 0;
    }

    public void SetEndState()
    {
        SetEndState(delay, duration);
    }

    public void SetEndState(float newDelay, float newDuration)
    {
        delay = newDelay;
        duration = newDuration;

        tweenFactor = 0;
        enabled = true;
        timeStepMultiplier = 1;
        currentTime = 0;
        currentDelay = 0;
    }

    public void SetBeginStateImmediately()
    {
        tweenFactor = 0;
        UpdateTweenWithFactor(tweenFactor);
        enabled = false;
    }

    public void SetEndStateImmediately()
    {
        tweenFactor = 1;
        UpdateTweenWithFactor(tweenFactor);
        enabled = false;
    }

   public void SubscribeToEditorUpdates()
    {
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
    }

    public void UnsubscribeFromEditorUpdates()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

    #endregion

    #region Private Methods

    protected virtual void UpdateTweenWithFactor(float factor)
    {
        
    }

    void Reset()
    {
        curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        durationScale = 1;
    }

    float GetTweenFactor(float relativeTime)
    {
        if (easingMethod == EasingMethod.None)
        {
            return relativeTime;
        }
        else if (easingMethod == EasingMethod.EaseIn)
        {
            return Easing.EaseIn(relativeTime, easingType);
        }
        else if (easingMethod == EasingMethod.EaseOut)
        {
            return Easing.EaseOut(relativeTime, easingType);
        }
        else if (easingMethod == EasingMethod.EaseInOut)
        {
            return Easing.EaseInOut(relativeTime, easingType);
        }
        else if (easingMethod == EasingMethod.Curve)
        {
            return curve.Evaluate(relativeTime);
        }

        return relativeTime;
    }

    void BeginStateSet()
    {
        if (looping == LoopType.Loop)
        {
            SetBeginState();
        }
        else if (looping == LoopType.PingPong)
        {
            SetEndState();
        }
        else
        {
            OnBeginStateSet.Invoke();
        }
    }

    void EndStateSet()
    {
        if (looping == LoopType.Loop)
        {
            SetEndState();
        }
        else if (looping == LoopType.PingPong)
        {
            SetBeginState();
        }
        else
        {
            OnEndStateSet.Invoke();
        }
    }

    #endregion
}
