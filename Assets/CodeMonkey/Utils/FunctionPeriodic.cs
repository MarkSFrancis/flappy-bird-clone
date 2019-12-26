/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey
{

    /*
     * Executes a Function periodically
     * */
    public class FunctionPeriodic
    {

        /*
         * Class to hook Actions into MonoBehaviour
         * */
        private class MonoBehaviourHook : MonoBehaviour
        {

            public Action OnUpdate;

            private void Update()
            {
                OnUpdate?.Invoke();
            }

        }

        private static List<FunctionPeriodic> funcList; // Holds a reference to all active timers
        private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded()
        {
            if (initGameObject is null)
            {
                initGameObject = new GameObject("FunctionPeriodic_Global");
                funcList = new List<FunctionPeriodic>();
            }
        }

        // Persist through scene loads
        public static FunctionPeriodic Create_Global(Action action, Func<bool> testDestroy, float timer)
        {
            FunctionPeriodic functionPeriodic = Create(action, testDestroy, timer, "", false, false, false);
            UnityEngine.Object.DontDestroyOnLoad(functionPeriodic._gameObject);
            return functionPeriodic;
        }


        // Trigger [action] every [timer], execute [testDestroy] after triggering action, destroy if returns true
        public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer)
        {
            return Create(action, testDestroy, timer, "", false);
        }

        public static FunctionPeriodic Create(Action action, float timer)
        {
            return Create(action, null, timer, "", false, false, false);
        }

        public static FunctionPeriodic Create(Action action, float timer, string functionName)
        {
            return Create(action, null, timer, functionName, false, false, false);
        }

        public static FunctionPeriodic Create(Action callback, Func<bool> testDestroy, float timer, string functionName, bool stopAllWithSameName)
        {
            return Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);
        }

        public static FunctionPeriodic Create(Action action, Func<bool> testDestroy, float timer, string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
            {
                StopAllFunc(functionName);
            }

            var gameObject = new GameObject("FunctionPeriodic Object " + functionName, typeof(MonoBehaviourHook));
            var functionPeriodic = new FunctionPeriodic(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
            gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionPeriodic.Update;

            funcList.Add(functionPeriodic);

            if (triggerImmediately)
            {
                action();
            }

            return functionPeriodic;
        }

        public static void RemoveTimer(FunctionPeriodic funcTimer)
        {
            funcList.Remove(funcTimer);
        }
        public static void StopTimer(string _name)
        {
            for (var i = 0; i < funcList.Count; i++)
            {
                if (funcList[i]._functionName == _name)
                {
                    funcList[i].DestroySelf();
                    return;
                }
            }
        }
        public static void StopAllFunc(string _name)
        {
            for (var i = 0; i < funcList.Count; i++)
            {
                if (funcList[i]._functionName == _name)
                {
                    funcList[i].DestroySelf();
                    i--;
                }
            }
        }
        public static bool IsFuncActive(string name)
        {
            for (var i = 0; i < funcList.Count; i++)
            {
                if (funcList[i]._functionName == name)
                {
                    return true;
                }
            }
            return false;
        }

        private readonly GameObject _gameObject;
        private float _timer;
        private readonly float _baseTimer;
        private readonly bool _useUnscaledDeltaTime;
        private readonly string _functionName;
        public Action Action { get; }
        public Func<bool> TestDestroy { get; }


        private FunctionPeriodic(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime)
        {
            _gameObject = gameObject;
            Action = action;
            _timer = timer;
            TestDestroy = testDestroy;
            _functionName = functionName;
            _useUnscaledDeltaTime = useUnscaledDeltaTime;
            _baseTimer = timer;
        }

        public void SkipTimerTo(float timer)
        {
            _timer = timer;
        }

        private void Update()
        {
            if (_useUnscaledDeltaTime)
            {
                _timer -= Time.unscaledDeltaTime;
            }
            else
            {
                _timer -= Time.deltaTime;
            }
            if (_timer <= 0)
            {
                Action();
                if (TestDestroy != null && TestDestroy())
                {
                    //Destroy
                    DestroySelf();
                }
                else
                {
                    //Repeat
                    _timer += _baseTimer;
                }
            }
        }
        public void DestroySelf()
        {
            RemoveTimer(this);
            if (_gameObject != null)
            {
                UnityEngine.Object.Destroy(_gameObject);
            }
        }
    }
}