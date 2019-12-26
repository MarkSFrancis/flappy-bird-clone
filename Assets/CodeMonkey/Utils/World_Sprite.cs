/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;

namespace CodeMonkey.Utils
{

    /*
     * Sprite in the World
     * */
    public class World_Sprite
    {

        private const int _sortingOrderDefault = 5000;

        public GameObject GameObject { get; set; }
        public Transform Transform { get; set; }
        private readonly SpriteRenderer _spriteRenderer;

        public static World_Sprite CreateDebugButton(Vector3 position, System.Action ClickFunc)
        {
            var worldSprite = new World_Sprite(null, position, new Vector3(10, 10), Assets.i.s_White, Color.green, _sortingOrderDefault);
            worldSprite.AddButton(ClickFunc, null, null);
            return worldSprite;
        }

        public static World_Sprite CreateDebugButton(Transform parent, Vector3 localPosition, System.Action ClickFunc)
        {
            var worldSprite = new World_Sprite(parent, localPosition, new Vector3(10, 10), Assets.i.s_White, Color.green, _sortingOrderDefault);
            worldSprite.AddButton(ClickFunc, null, null);
            return worldSprite;
        }

        public static World_Sprite CreateDebugButton(Transform parent, Vector3 localPosition, string text, System.Action ClickFunc, int fontSize = 30, float paddingX = 5, float paddingY = 5)
        {
            var gameObject = new GameObject("DebugButton");
            gameObject.transform.parent = parent;
            gameObject.transform.localPosition = localPosition;
            TextMesh textMesh = UtilsClass.CreateWorldText(text, gameObject.transform, Vector3.zero, fontSize, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 20000);
            Bounds rendererBounds = textMesh.GetComponent<MeshRenderer>().bounds;

            Color color = UtilsClass.GetColorFromString("00BA00FF");
            if (color.r >= 1f)
            {
                color.r = .9f;
            }

            if (color.g >= 1f)
            {
                color.g = .9f;
            }

            if (color.b >= 1f)
            {
                color.b = .9f;
            }

            Color colorOver = color * 1.1f; // button over color lighter

            var worldSprite = new World_Sprite(gameObject.transform, Vector3.zero, rendererBounds.size + new Vector3(paddingX, paddingY), Assets.i.s_White, color, _sortingOrderDefault);
            worldSprite.AddButton(ClickFunc, () => worldSprite.SetColor(colorOver), () => worldSprite.SetColor(color));
            return worldSprite;
        }

        public static World_Sprite Create(Transform parent, Vector3 localPosition, Vector3 localScale, Sprite sprite, Color color, int sortingOrderOffset)
        {
            return new World_Sprite(parent, localPosition, localScale, sprite, color, sortingOrderOffset);
        }

        public static World_Sprite Create(Vector3 worldPosition, Vector3 localScale, Sprite sprite, Color color, int sortingOrderOffset)
        {
            return new World_Sprite(null, worldPosition, localScale, sprite, color, sortingOrderOffset);
        }

        public static World_Sprite Create(Vector3 worldPosition, Vector3 localScale, Sprite sprite, Color color)
        {
            return new World_Sprite(null, worldPosition, localScale, sprite, color, 0);
        }

        public static World_Sprite Create(Vector3 worldPosition, Vector3 localScale, Color color)
        {
            return new World_Sprite(null, worldPosition, localScale, Assets.i.s_White, color, 0);
        }
        public static World_Sprite Create(Vector3 worldPosition, Vector3 localScale)
        {
            return new World_Sprite(null, worldPosition, localScale, Assets.i.s_White, Color.white, 0);
        }
        public static World_Sprite Create(Vector3 worldPosition, Vector3 localScale, int sortingOrderOffset)
        {
            return new World_Sprite(null, worldPosition, localScale, Assets.i.s_White, Color.white, sortingOrderOffset);
        }

        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = _sortingOrderDefault)
        {
            return (int)(baseSortingOrder - position.y) + offset;
        }

        public World_Sprite(Transform parent, Vector3 localPosition, Vector3 localScale, Sprite sprite, Color color, int sortingOrderOffset)
        {
            var sortingOrder = GetSortingOrder(localPosition, sortingOrderOffset);
            GameObject = UtilsClass.CreateWorldSprite(parent, "Sprite", sprite, localPosition, localScale, sortingOrder, color);
            Transform = GameObject.transform;
            _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        }

        public void SetSortingOrderOffset(int sortingOrderOffset)
        {
            SetSortingOrder(GetSortingOrder(GameObject.transform.position, sortingOrderOffset));
        }

        public void SetSortingOrder(int sortingOrder)
        {
            GameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }

        public void SetLocalScale(Vector3 localScale)
        {
            Transform.localScale = localScale;
        }

        public void SetPosition(Vector3 localPosition)
        {
            Transform.localPosition = localPosition;
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void Show()
        {
            GameObject.SetActive(true);
        }

        public void Hide()
        {
            GameObject.SetActive(false);
        }

        public Button_Sprite AddButton(System.Action ClickFunc, System.Action MouseOverOnceFunc, System.Action MouseOutOnceFunc)
        {
            GameObject.AddComponent<BoxCollider2D>();
            Button_Sprite buttonSprite = GameObject.AddComponent<Button_Sprite>();
            if (ClickFunc != null)
            {
                buttonSprite.ClickFunc = ClickFunc;
            }

            if (MouseOverOnceFunc != null)
            {
                buttonSprite.MouseOverOnceFunc = MouseOverOnceFunc;
            }

            if (MouseOutOnceFunc != null)
            {
                buttonSprite.MouseOutOnceFunc = MouseOutOnceFunc;
            }

            return buttonSprite;
        }

        public void DestroySelf()
        {
            Object.Destroy(GameObject);
        }

    }
}