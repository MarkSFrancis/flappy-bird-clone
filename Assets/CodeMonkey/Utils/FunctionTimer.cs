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
     * Triggers a Action after a certain time 
     * */
    public class FunctionTimer
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

        private static List<FunctionTimer> _timerList; // Holds a reference to all active timers
        private static GameObject _initGameObject; // Global game object used for initializing class, is destroyed on scene change

        private static void InitIfNeeded()
        {
            if (_initGameObject is null)
            {
                _initGameObject = new GameObject("FunctionTimer_Global");
                _timerList = new List<FunctionTimer>();
            }
        }

        public static FunctionTimer Create(Action action, float timer)
        {
            return Create(action, timer, "", false, false);
        }
        public static FunctionTimer Create(Action action, float timer, string functionName)
        {
            return Create(action, timer, functionName, false, false);
        }
        public static FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime)
        {
            return Create(action, timer, functionName, useUnscaledDeltaTime, false);
        }
        public static FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime, bool stopAllWithSameName)
        {
            InitIfNeeded();

            if (stopAllWithSameName)
            {
                StopAllTimersWithName(functionName);
            }

            var obj = new GameObject("FunctionTimer Object " + functionName, typeof(MonoBehaviourHook));
            var funcTimer = new FunctionTimer(obj, action, timer, functionName, useUnscaledDeltaTime);
            obj.GetComponent<MonoBehaviourHook>().OnUpdate = funcTimer.Update;

            _timerList.Add(funcTimer);

            return funcTimer;
        }
        public static void RemoveTimer(FunctionTimer funcTimer)
        {
            _timerList.Remove(funcTimer);
        }
        public static void StopAllTimersWithName(string functionName)
        {
            for (var i = 0; i < _timerList.Count; i++)
            {
                if (_timerList[i]._functionName == functionName)
                {
                    _timerList[i].DestroySelf();
                    i--;
                }
            }
        }
        public static void StopFirstTimerWithName(string functionName)
        {
            for (var i = 0; i < _timerList.Count; i++)
            {
                if (_timerList[i]._functionName == functionName)
                {
                    _timerList[i].DestroySelf();
                    return;
                }
            }
        }

        private readonly GameObject _gameObject;
        private float _timer;
        private readonly string _functionName;
        private readonly bool _useUnscaledDeltaTime;
        private readonly Action _action;

        public FunctionTimer(GameObject gameObject, Action action, float timer, string functionName, bool useUnscaledDeltaTime)
        {
            _gameObject = gameObject;
            _action = action;
            _timer = timer;
            _functionName = functionName;
            _useUnscaledDeltaTime = useUnscaledDeltaTime;
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
                // Timer complete, trigger Action
                _action();
                DestroySelf();
            }
        }
        private void DestroySelf()
        {
            RemoveTimer(this);
            if (_gameObject != null)
            {
                UnityEngine.Object.Destroy(_gameObject);
            }
        }




        /*
         * Class to trigger Actions manually without creating a GameObject
         * */
        public class FunctionTimerObject
        {
            private float _timer;
            private readonly Action _callback;

            public FunctionTimerObject(Action callback, float timer)
            {
                _callback = callback;
                _timer = timer;
            }

            public void Update()
            {
                Update(Time.deltaTime);
            }
            public void Update(float deltaTime)
            {
                _timer -= deltaTime;
                if (_timer <= 0)
                {
                    _callback();
                }
            }
        }

        // Create a Object that must be manually updated through Update();
        public static FunctionTimerObject CreateObject(Action callback, float timer)
        {
            return new FunctionTimerObject(callback, timer);
        }
    }
}