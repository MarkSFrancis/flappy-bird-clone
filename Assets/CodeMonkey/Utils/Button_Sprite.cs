/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

//#define SOUND_MANAGER // Has Sound_Manager in project
//#define CURSOR_MANAGER // Has Cursor_Manager in project

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeMonkey.Utils
{

    /*
     * Button Actions on a World BoxCollider
     * */
    public class Button_Sprite : MonoBehaviour
    {

        private static Func<Camera> _getWorldCamera;

        public static void SetGetWorldCamera(Func<Camera> GetWorldCamera)
        {
            Button_Sprite._getWorldCamera = GetWorldCamera;
        }

        public Action ClickFunc { get; set; }
        public Action MouseRightDownOnceFunc { get; set; }
        public Action MouseRightDownFunc { get; set; }
        public Action MouseRightUpFunc { get; set; }
        public Action MouseDownOnceFunc { get; set; }
        public Action MouseUpOnceFunc { get; set; }
        public Action MouseOverOnceFunc { get; set; }
        public Action MouseOutOnceFunc { get; set; }
        public Action MouseOverOnceTooltipFunc { get; set; }
        public Action MouseOutOnceTooltipFunc { get; set; }

        private bool _draggingMouseRight;
        private Vector3 _mouseRightDragStart;
        public Action<Vector3, Vector3> MouseRightDragFunc { get; set; }
        public Action<Vector3, Vector3> MouseRightDragUpdateFunc { get; set; }
        public bool TriggerMouseRightDragOnEnter { get; set; }

        public enum HoverBehaviour
        {
            Custom,
            Change_Color,
            Change_Image,
            Change_SetActive,
        }

        public HoverBehaviour hoverBehaviourType { get; set; } = HoverBehaviour.Custom;
        private Action _hoverBehaviourFunc_Enter, _hoverBehaviourFunc_Exit;

        public Color hoverBehaviour_Color_Enter = new Color(1, 1, 1, 1), hoverBehaviour_Color_Exit = new Color(1, 1, 1, 1);
        public SpriteRenderer hoverBehaviour_Image;
        public Sprite hoverBehaviour_Sprite_Exit, hoverBehaviour_Sprite_Enter;
        public bool hoverBehaviour_Move = false;
        public Vector2 hoverBehaviour_Move_Amount = Vector2.zero;
        private Vector3 posExit, posEnter;
        public bool triggerMouseOutFuncOnClick = false;
        public bool clickThroughUI = false;

        private readonly Action internalOnMouseDownFunc, internalOnMouseEnterFunc, internalOnMouseExitFunc;

#if SOUND_MANAGER
        public Sound_Manager.Sound mouseOverSound, mouseClickSound;
#endif
#if CURSOR_MANAGER
        public CursorManager.CursorType cursorMouseOver, cursorMouseOut;
#endif




        public void SetHoverBehaviourChangeColor(Color colorOver, Color colorOut)
        {
            hoverBehaviourType = HoverBehaviour.Change_Color;
            hoverBehaviour_Color_Enter = colorOver;
            hoverBehaviour_Color_Exit = colorOut;
            if (hoverBehaviour_Image == null)
            {
                hoverBehaviour_Image = transform.GetComponent<SpriteRenderer>();
            }

            hoverBehaviour_Image.color = hoverBehaviour_Color_Exit;
            SetupHoverBehaviour();
        }

        private void OnMouseDown()
        {
            if (!clickThroughUI && IsPointerOverUI())
            {
                return; // Over UI!
            }

            internalOnMouseDownFunc?.Invoke();
            ClickFunc?.Invoke();
            if (triggerMouseOutFuncOnClick)
            {
                OnMouseExit();
            }
        }
        public void Manual_OnMouseExit()
        {
            OnMouseExit();
        }

        private void OnMouseUp()
        {
            MouseUpOnceFunc?.Invoke();
        }

        private void OnMouseEnter()
        {
            if (!clickThroughUI && IsPointerOverUI())
            {
                return; // Over UI!
            }

            internalOnMouseEnterFunc?.Invoke();
            if (hoverBehaviour_Move)
            {
                transform.localPosition = posEnter;
            }

            _hoverBehaviourFunc_Enter?.Invoke();
            MouseOverOnceFunc?.Invoke();
            MouseOverOnceTooltipFunc?.Invoke();
        }

        private void OnMouseExit()
        {
            internalOnMouseExitFunc?.Invoke();
            if (hoverBehaviour_Move)
            {
                transform.localPosition = posExit;
            }

            _hoverBehaviourFunc_Exit?.Invoke();
            MouseOutOnceFunc?.Invoke();
            MouseOutOnceTooltipFunc?.Invoke();
        }

        private void OnMouseOver()
        {
            if (!clickThroughUI && IsPointerOverUI())
            {
                return; // Over UI!
            }

            if (Input.GetMouseButton(1))
            {
                MouseRightDownFunc?.Invoke();
                if (!_draggingMouseRight && TriggerMouseRightDragOnEnter)
                {
                    _draggingMouseRight = true;
                    _mouseRightDragStart = GetWorldPositionFromUI();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                _draggingMouseRight = true;
                _mouseRightDragStart = GetWorldPositionFromUI();
                MouseRightDownOnceFunc?.Invoke();
            }
        }

        private void Update()
        {
            if (_draggingMouseRight)
            {
                MouseRightDragUpdateFunc?.Invoke(_mouseRightDragStart, GetWorldPositionFromUI());
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (_draggingMouseRight)
                {
                    _draggingMouseRight = false;
                    MouseRightDragFunc?.Invoke(_mouseRightDragStart, GetWorldPositionFromUI());
                }

                MouseRightUpFunc?.Invoke();
            }
        }

        private void Awake()
        {
            if (_getWorldCamera == null)
            {
                SetGetWorldCamera(() => Camera.main); // Set default World Camera
            }

            posExit = transform.localPosition;
            posEnter = transform.localPosition + (Vector3)hoverBehaviour_Move_Amount;
            SetupHoverBehaviour();

#if SOUND_MANAGER
            // Sound Manager
            internalOnMouseDownFunc += () => { if (mouseClickSound != Sound_Manager.Sound.None) Sound_Manager.PlaySound(mouseClickSound); };
            internalOnMouseEnterFunc += () => { if (mouseOverSound != Sound_Manager.Sound.None) Sound_Manager.PlaySound(mouseOverSound); };
#endif

#if CURSOR_MANAGER
            // Cursor Manager
            internalOnMouseExitFunc += () => { if (cursorMouseOut != CursorManager.CursorType.None) CursorManager.SetCursor(cursorMouseOut); };
            internalOnMouseEnterFunc += () => { if (cursorMouseOver != CursorManager.CursorType.None) CursorManager.SetCursor(cursorMouseOver); };
#endif
        }
        private void SetupHoverBehaviour()
        {
            switch (hoverBehaviourType)
            {
                case HoverBehaviour.Change_Color:
                    _hoverBehaviourFunc_Enter = delegate () { hoverBehaviour_Image.color = hoverBehaviour_Color_Enter; };
                    _hoverBehaviourFunc_Exit = delegate () { hoverBehaviour_Image.color = hoverBehaviour_Color_Exit; };
                    break;
                case HoverBehaviour.Change_Image:
                    _hoverBehaviourFunc_Enter = delegate () { hoverBehaviour_Image.sprite = hoverBehaviour_Sprite_Enter; };
                    _hoverBehaviourFunc_Exit = delegate () { hoverBehaviour_Image.sprite = hoverBehaviour_Sprite_Exit; };
                    break;
                case HoverBehaviour.Change_SetActive:
                    _hoverBehaviourFunc_Enter = delegate () { hoverBehaviour_Image.gameObject.SetActive(true); };
                    _hoverBehaviourFunc_Exit = delegate () { hoverBehaviour_Image.gameObject.SetActive(false); };
                    break;
            }
        }







        private static Vector3 GetWorldPositionFromUI()
        {
            Vector3 worldPosition = _getWorldCamera().ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }
        private static bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            else
            {
                var pe = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                var hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);
                return hits.Count > 0;
            }
        }
    }

}