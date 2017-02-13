using UnityEngine;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MBTweenAnimation : MonoBehaviour 
{
    [Serializable]
    public struct AnimationEntry
    {
        public MBTweenAnimation animation;
        public float delay;
    }

    [Serializable]
    public struct TweenEntry
    {
        public MBTweenBase tween;
        public float delay;
    }

	#region Fields

    [SerializeField] GameObject rootToDeactivate;
    [SerializeField] AnimationEntry[] animations;
    [SerializeField] TweenEntry[] tweens;
    [SerializeField] LoopType looping;
    [SerializeField] UnityEvent OnEndStateSet;
    [SerializeField] UnityEvent OnBeginStateSet;

    float totalDuration = -100;

    private float durationScale = 1;


    #endregion

	#region Properties

    public UnityEvent OnEndStateSetEvent
    {
        get { return OnEndStateSet; }
    }

    public UnityEvent OnBeginStateSetEvent
    {
        get { return OnBeginStateSet; }
    }

    public float TotalDuration
    {
        get
        {
            #if UNITY_EDITOR
            totalDuration = -1;
            #endif

            if (totalDuration < 0)
            {
                foreach (var t in tweens)
                {
                    if (t.tween.duration + t.delay > totalDuration)
                    {
                        totalDuration = t.tween.duration + t.delay;
                    }
                }

                foreach (var anim in animations)
                {
                    if (anim.animation.TotalDuration + anim.delay > totalDuration)
                    {
                        totalDuration = anim.animation.TotalDuration + anim.delay;
                    }
                }
            }

            return totalDuration;
        }
    }

    public float DurationScale
    {
        get { return durationScale; }
        set
        {
            durationScale = value;
            foreach (var t in tweens)
            {
                t.tween.durationScale = durationScale;
            }

            foreach (var anim in animations)
            {
                anim.animation.DurationScale = durationScale;
            }
        }
    }

    public bool IsAnimationRunning
    {
        get;
        set;
    }

    public bool IsInBeginState
    {
        get
        {
            
            foreach (var t in tweens)
            {
                if (!t.tween.IsInBeginState)
                {
                    return false;
                }
            }

            foreach (var anim in animations)
            {
                if (!anim.animation.IsInBeginState)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool IsInEndState
    {
        get
        {
            foreach (var t in tweens)
            {
                if (!t.tween.IsInEndState)
                {
                    return false;
                }
            }

            foreach (var anim in animations)
            {
                if (!anim.animation.IsInEndState)
                {
                    return false;
                }
            }

            return true;
        }
    } 

	#endregion

	#region Unity Lifecycle

#if UNITY_EDITOR
    void EditorUpdate()
    {
        if (!EditorApplication.isPlaying)
        {
            Update();
        }
    }
#endif

    void Update()
    {
        if (IsAnimationRunning)
        {
            bool allTweensFinished = true;

            for (int i = 0; i < tweens.Length; i++)
            {
                var t = tweens[i];
                if (t.tween.enabled)
                {
                    allTweensFinished = false;
                    break;
                }
            }

            for (int i = 0; i < animations.Length; i++)
            {
                var an = animations[i];
                if (an.animation.IsAnimationRunning)
                {
                    allTweensFinished = false;
                }
            }

            if (allTweensFinished)
            {
                IsAnimationRunning = false;

                if (IsInBeginState)
                {
                    OnBeginStateSet.Invoke();

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
                        if (rootToDeactivate != null)
                        {
                            rootToDeactivate.SetActive(false);
                        }
                    }
                }
                else if (IsInEndState)
                {
                    OnEndStateSet.Invoke();

                    if (looping == LoopType.Loop)
                    {
                        SetEndState();
                    }
                    else if (looping == LoopType.PingPong)
                    {
                        SetBeginState();
                    }
                }
            }
        }
    }

	#endregion

	#region Public Methods


    public void SubscribeToEditorUpdates()
    {
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
        foreach (var tween in tweens)
        {
            tween.tween.SubscribeToEditorUpdates();
        }

        foreach (var a in animations)
        {
            a.animation.SubscribeToEditorUpdates();
        }
#endif
    }

    public void UnsubscribeFromEditorUpdates()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
        foreach (var tween in tweens)
        {
            tween.tween.UnsubscribeFromEditorUpdates();
        }

        foreach (var a in animations)
        {
            a.animation.UnsubscribeFromEditorUpdates();
        }
#endif
    }

    public void SetEndState(float delay = 0)
    {
        if (rootToDeactivate != null)
        {
            rootToDeactivate.SetActive(true);
        }

        for (int i = 0; i < tweens.Length; i++)
        {
            var t = tweens[i];
            t.tween.SetBeginStateImmediately();
            t.tween.SetEndState(t.delay + delay, t.tween.duration);
        }

        for (int i = 0; i < animations.Length; i++)
        {
            var a = animations[i];
            a.animation.SetBeginStateImmediately();
            a.animation.SetEndState(a.delay + delay);
        }

        IsAnimationRunning = true;
    }

    public void SetBeginState(float delay = 0)
    {
        for (int i = 0; i < tweens.Length; i++)
        {
            var t = tweens[i];
            t.tween.SetEndStateImmediately();
            t.tween.SetBeginState(TotalDuration - (t.delay + t.tween.duration) + delay, t.tween.duration);
        }

        for (int i = 0; i < animations.Length; i++)
        {
            var a = animations[i];
            a.animation.SetEndStateImmediately();
            a.animation.SetBeginState(TotalDuration - (a.delay + a.animation.TotalDuration) + delay);
        }

        IsAnimationRunning = true;
    }

    public void SetBeginStateImmediately()
    {
        for (int i = 0; i < tweens.Length; i++)
        {
            var t = tweens[i];
            t.tween.SetBeginStateImmediately();
        }

        for (int i = 0; i < animations.Length; i++)
        {
            var a = animations[i];
            a.animation.SetBeginStateImmediately();
        }
    }

    public void SetEndStateImmediately()
    {
        for (int i = 0; i < tweens.Length; i++)
        {
            var t = tweens[i];
            t.tween.SetEndStateImmediately();
        }

        for (int i = 0; i < animations.Length; i++)
        {
            var a = animations[i];
            a.animation.SetEndStateImmediately();
        }
    }

	#endregion

	#region Private Methods
	#endregion

	#region Event Handlers
	#endregion

}