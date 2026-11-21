using UnityEngine;
using UnityEngine.UI;

namespace Miasta
{
    public static class UIFactory
    {
        static Canvas canvas;
        static Font f;

        public static Font UiFont()
        {
            if (f == null) { try { f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { } }
            if (f == null) { try { f = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { } }
            return f;
        }

        public static Canvas RootCanvas()
        {
            EnsureEventSystem();
            if (canvas != null) return canvas;
            var go = new GameObject("Telefono_Canvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        static void EnsureEventSystem()
        {
            if (UnityEngine.EventSystems.EventSystem.current != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<UnityEngine.EventSystems.EventSystem>();
#if ENABLE_INPUT_SYSTEM
            go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
        }

        public static GameObject Panel(Transform parent, string name, Color c, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            var img = go.AddComponent<Image>();
            img.color = c;
            return go;
        }

        public static GameObject Panel(Transform parent, string name, Color c, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
            var img = go.AddComponent<Image>();
            img.color = c;
            return go;
        }

        public static Text Label(Transform parent, string name, string text, int fontSize, Color color, Vector2 pos, Vector2 size, TextAnchor anchor = TextAnchor.UpperLeft)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
            var t = go.AddComponent<Text>();
            t.text = text;
            t.color = color;
            t.alignment = anchor;
            t.font = UiFont();
            if (t.font != null) t.fontSize = fontSize;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        public static Button Btn(Transform parent, string name, string label, Color c, Vector2 pos, Vector2 size, System.Action onClick, int fontSize = 16)
        {
            var go = Panel(parent, name, c, pos, size);
            var b = go.AddComponent<Button>();
            var t = Label(go.transform, name + "_T", label, fontSize, Color.white, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter);
            var rt = t.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            if (onClick != null) b.onClick.AddListener(() => onClick());
            return b;
        }

        public static InputField Input(Transform parent, string name, string placeholder, Color c, Vector2 pos, Vector2 size, int fontSize = 18)
        {
            var go = Panel(parent, name, c, pos, size);
            var fld = go.AddComponent<InputField>();
            var t = Label(go.transform, name + "_T", "", fontSize, Color.white, Vector2.zero, Vector2.zero, TextAnchor.MiddleLeft);
            var rt = t.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = new Vector2(-16f, 0f);
            rt.anchoredPosition = new Vector2(8f, 0f);
            var ph = Label(go.transform, name + "_PH", placeholder, fontSize, new Color(1f, 1f, 1f, 0.4f), rt.anchoredPosition, rt.sizeDelta, TextAnchor.MiddleLeft);
            var rt2 = ph.GetComponent<RectTransform>();
            rt2.anchorMin = Vector2.zero;
            rt2.anchorMax = Vector2.one;
            rt2.sizeDelta = new Vector2(-16f, 0f);
            fld.textComponent = t;
            fld.placeholder = ph;
            return fld;
        }

        public static ScrollRect ScrollList(Transform parent, string name, Color bg, Vector2 pos, Vector2 size, out RectTransform content)
        {
            var scrollGo = Panel(parent, name, bg, pos, size);
            var sr = scrollGo.AddComponent<ScrollRect>();
            var maskGo = new GameObject("Mask");
            maskGo.transform.SetParent(scrollGo.transform, false);
            var mrt = maskGo.AddComponent<RectTransform>();
            mrt.anchorMin = Vector2.zero;
            mrt.anchorMax = Vector2.one;
            mrt.sizeDelta = Vector2.zero;
            maskGo.AddComponent<RectMask2D>();
            var cont = new GameObject("Lista");
            cont.transform.SetParent(maskGo.transform, false);
            var crt = cont.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0f, 1f);
            crt.anchorMax = Vector2.one;
            crt.pivot = new Vector2(0.5f, 1f);
            crt.sizeDelta = new Vector2(0f, 0f);
            var grid = cont.AddComponent<VerticalLayoutGroup>();
            grid.spacing = 6f;
            grid.padding = new RectOffset(6, 6, 6, 6);
            grid.childForceExpandHeight = false;
            grid.childForceExpandWidth = true;
            var le = cont.AddComponent<ContentSizeFitter>();
            le.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sr.content = crt;
            sr.viewport = mrt;
            content = crt;
            return sr;
        }
    }
}
