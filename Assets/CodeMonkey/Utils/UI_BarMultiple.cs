/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Utils
{

    /*
     * UI Container with multiple bars, useful for displaying one bar with multiple inner bars like success chance and failure chance
     * */
    public class UI_BarMultiple
    {

        private GameObject _gameObject;
        private RectTransform _rectTransform;
        private readonly RectTransform[] _barArr;
        private Vector2 _size;

        public class Outline
        {
            public float size = 1f;
            public Color color = Color.black;
            public Outline(float size, Color color)
            {
                this.size = size;
                this.color = color;
            }
        }

        public UI_BarMultiple(Transform parent, Vector2 anchoredPosition, Vector2 size, Color[] barColorArr, Outline outline)
        {
            _size = size;
            SetupParent(parent, anchoredPosition, size);
            if (outline != null)
            {
                SetupOutline(outline, size);
            }

            var barList = new List<RectTransform>();
            var defaultSizeList = new List<float>();

            foreach (Color color in barColorArr)
            {
                barList.Add(SetupBar(color));
                defaultSizeList.Add(1f / barColorArr.Length);
            }

            _barArr = barList.ToArray();
            SetSizes(defaultSizeList.ToArray());
        }

        private void SetupParent(Transform parent, Vector2 anchoredPosition, Vector2 size)
        {
            _gameObject = new GameObject("UI_BarMultiple", typeof(RectTransform));
            _rectTransform = _gameObject.GetComponent<RectTransform>();
            _rectTransform.SetParent(parent, false);
            _rectTransform.sizeDelta = size;
            _rectTransform.anchorMin = new Vector2(0, .5f);
            _rectTransform.anchorMax = new Vector2(0, .5f);
            _rectTransform.pivot = new Vector2(0, .5f);
            _rectTransform.anchoredPosition = anchoredPosition;
        }

        private void SetupOutline(Outline outline, Vector2 size)
        {
            UtilsClass.DrawSprite(outline.color, _gameObject.transform, Vector2.zero, size + new Vector2(outline.size, outline.size), "Outline");
        }

        private RectTransform SetupBar(Color barColor)
        {
            RectTransform bar = UtilsClass.DrawSprite(barColor, _gameObject.transform, Vector2.zero, Vector2.zero, "Bar");
            bar.anchorMin = new Vector2(0, 0);
            bar.anchorMax = new Vector2(0, 1f);
            bar.pivot = new Vector2(0, .5f);
            return bar;
        }

        public void SetSizes(float[] sizeArr)
        {
            if (sizeArr.Length != _barArr.Length)
            {
                throw new System.Exception("Length doesn't match!");
            }
            Vector2 pos = Vector2.zero;
            for (var i = 0; i < sizeArr.Length; i++)
            {
                var scaledSize = sizeArr[i] * _size.x;
                _barArr[i].anchoredPosition = pos;
                _barArr[i].sizeDelta = new Vector2(scaledSize, 0f);
                pos.x += scaledSize;
            }
        }

        public Vector2 GetSize()
        {
            return _size;
        }

        public void DestroySelf()
        {
            Object.Destroy(_gameObject);
        }
    }
}
