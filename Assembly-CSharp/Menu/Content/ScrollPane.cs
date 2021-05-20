using System;
using GlobalEnums;
using UnityEngine;
using UnityEngine.UI;
using Modding.Menu.Config;

// ReSharper disable CS1591
#pragma warning disable 1591

namespace Modding.Menu
{
    /// <summary>
    /// Helper class for creating scrollbars and their associated content panes
    /// </summary>
    public static class ScrollPaneContent
    {
        public static ContentArea AddScrollPaneContent(
            this ContentArea content,
            ScrollbarConfig config,
            RelLength contentHeight,
            ContentLayout layout,
            Action<ContentArea> action
        ) => content.AddScrollPaneContent(config, contentHeight, layout, action, out _, out _);

        public static ContentArea AddScrollPaneContent(
            this ContentArea content,
            ScrollbarConfig config,
            RelLength contentHeight,
            ContentLayout layout,
            Action<ContentArea> action,
            out GameObject scrollContent,
            out Scrollbar scroll
        )
        {
            // Scrollbar
            GameObject scrollbarObj;
            content.AddScrollbar(config, out scrollbarObj);
            var scrollbar = scrollbarObj.GetComponent<Scrollbar>();

            // ScrollMask
            var scrollMask = new GameObject("ScrollMask");
            GameObject.DontDestroyOnLoad(scrollMask);
            scrollMask.transform.SetParent(content.contentObject.transform, false);
            // RectTransform
            var scrollMaskRt = scrollMask.AddComponent<RectTransform>();
            scrollMaskRt.sizeDelta = new Vector2(0f, 0f);
            scrollMaskRt.pivot = new Vector2(0.5f, 0.5f);
            scrollMaskRt.anchorMin = new Vector2(0f, 0f);
            scrollMaskRt.anchorMax = new Vector2(1f, 1f);
            scrollMaskRt.anchoredPosition = new Vector2(0f, 0f);
            // CanvasRenderer
            scrollMask.AddComponent<CanvasRenderer>();
            // Mask
            var mask = scrollMask.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            // Image
            var maskImage = scrollMask.AddComponent<Image>();
            maskImage.raycastTarget = false;

            // Scrolling Pane
            var scrollPane = new GameObject("ScrollingPane");
            GameObject.DontDestroyOnLoad(scrollPane);
            scrollPane.transform.SetParent(scrollMask.transform, false);
            // RectTransform
            var scrollPaneRt = scrollPane.AddComponent<RectTransform>();
            RectTransformData.FromSizeAndPos(
                new RelVector2(new RelLength(0f, 1f), contentHeight),
                new AnchoredPosition(new Vector2(0.5f, 1f), new Vector2(0.5f, 1f))
            ).Apply(scrollPaneRt);
            // CanvasRenderer
            scrollPane.AddComponent<CanvasRenderer>();

            action(new ContentArea(scrollPane, layout).CopyEvents(content));

            scrollbar.onValueChanged = CreateScrollEvent(f =>
            {
                scrollPaneRt.anchoredPosition = new Vector2(
                    0f,
                    Mathf.Max(
                        0,
                        (scrollPaneRt.sizeDelta.y - scrollMaskRt.rect.height) * f
                    )
                );
            });

            scroll = scrollbar;
            scrollContent = scrollMask;
            return content;
        }

        public static ContentArea AddScrollbar(
            this ContentArea content,
            ScrollbarConfig config,
            out GameObject obj
        )
        {
            // Scrollbar
            var scrollbar = new GameObject("Scrollbar");
            GameObject.DontDestroyOnLoad(scrollbar);
            scrollbar.transform.SetParent(content.contentObject.transform, false);
            // RectTransform
            var scrollbarRt = scrollbar.AddComponent<RectTransform>();
            scrollbarRt.sizeDelta = new Vector2(38f, 906f);
            config.position.Reposition(scrollbarRt);
            // CanvasRenderer
            scrollbar.AddComponent<CanvasRenderer>();
            // Scrollbar
            var scrollbarComp = scrollbar.AddComponent<Scrollbar>();
            scrollbarComp.direction = Scrollbar.Direction.TopToBottom;
            scrollbarComp.numberOfSteps = 0;
            scrollbarComp.navigation = config.navigation;
            scrollbarComp.size = 0.1f;
            // MenuPreventDeselect
            var scrollbarMpd = scrollbar.AddComponent<MenuPreventDeselect>();
            scrollbarMpd.cancelAction = (CancelAction)Modding.Patches.CancelAction.CustomCancelAction;
            ((Modding.Patches.MenuPreventDeselect)scrollbarMpd).customCancelAction = config.cancelAction;

            // Sliding Area
            var slidingArea = new GameObject("Sliding Area");
            GameObject.DontDestroyOnLoad(slidingArea);
            slidingArea.transform.SetParent(scrollbar.transform, false);
            // RectTransform
            var slidingAreaRt = slidingArea.AddComponent<RectTransform>();
            slidingAreaRt.sizeDelta = new Vector2(-20f, -20f);
            slidingAreaRt.pivot = new Vector2(0.5f, 0.5f);
            slidingAreaRt.anchorMin = new Vector2(0f, 0f);
            slidingAreaRt.anchorMax = new Vector2(1f, 1f);
            slidingAreaRt.anchoredPosition = new Vector2(0f, 0f);

            // WARNING the following two game objects have been described by onlookers as
            // "disturbing", "akin to something from an HP Lovecraft story", "peepoUnhappy",
            // "the worst thing I have seen since the previous line of code" and "lmao thats bad"
            // I do not know what happened with these two rect transforms or the convoluted object structure
            // but I do not have enough braincells to try and fix it
            // Handle
            var handle = new GameObject("Handle");
            GameObject.DontDestroyOnLoad(handle);
            handle.transform.SetParent(scrollbar.transform, false);
            // RectTransform
            var handleRt = handle.AddComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(76f, 0f); // normal
            handleRt.pivot = new Vector2(0.5f, 0.5f); // wtf did you lie to me? you said this was gonna be bad
            handleRt.anchorMin = new Vector2(0.0f, 0.5f); // this seems normal
            handleRt.anchorMax = new Vector2(1.0f, 0.6f); // elderC
            handleRt.anchoredPosition = new Vector2(-1f, 0f); // omegaMaggotPrime
            // CanvasRenderer
            handle.AddComponent<CanvasRenderer>();
            // Post Component Config
            scrollbarComp.handleRect = handleRt;

            // TopFleur // no team cherry this is not in fact a top fleur seeing as your scrollbar is vertical
            var handleSprite = new GameObject("TopFleur"); // I'm only keeping this cause its funny as hell
            GameObject.DontDestroyOnLoad(handleSprite);
            handleSprite.transform.SetParent(handle.transform, false);
            // RectTransform
            var handleSpriteRt = handleSprite.AddComponent<RectTransform>();
            handleSpriteRt.sizeDelta = new Vector2(37.8f, 68.5f); // hmmm
            handleSpriteRt.pivot = new Vector2(0.5f, 0.6f); // why is it 0.6
            handleSpriteRt.anchorMin = new Vector2(0.5f, 0.6f); // please tell me
            handleSpriteRt.anchorMax = new Vector2(0.5f, 0.6f);
            handleSpriteRt.anchoredPosition = new Vector2(0.8f, 0f); // now its 0.8 wtf
            handleSpriteRt.localScale = new Vector3(2f, 2f, 1f);
            // CanvasRenderer
            handleSprite.AddComponent<CanvasRenderer>();
            // Image
            var handleSpriteImage = handleSprite.AddComponent<Image>();
            handleSpriteImage.sprite = MenuResources.ScrollbarHandleSprite;
            // ScrollBarHandle
            var handleSpriteSbh = handleSprite.AddComponent<ScrollBarHandle>();
            handleSpriteSbh.scrollBar = scrollbarComp;

            // Background
            var background = new GameObject("Background");
            GameObject.DontDestroyOnLoad(background);
            background.transform.SetParent(scrollbar.transform, false);
            // RectTransform
            var backgroundRt = background.AddComponent<RectTransform>();
            backgroundRt.sizeDelta = new Vector2(5f, 906f);
            backgroundRt.pivot = new Vector2(0.5f, 0.5f);
            backgroundRt.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRt.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRt.anchoredPosition = new Vector2(0f, 0f);
            // CanvasRenderer
            background.AddComponent<CanvasRenderer>();
            // Image
            var backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = MenuResources.ScrollbarBackgroundSprite;

            obj = scrollbar;
            return content;
        }

        private static Scrollbar.ScrollEvent CreateScrollEvent(Action<float> action)
        {
            var ret = new Scrollbar.ScrollEvent();
            ret.AddListener(action.Invoke);
            return ret;
        }
    }
    
    namespace Config
    {
        public struct ScrollbarConfig
        {
            public Navigation navigation;
            public AnchoredPosition position;
            public Action<MenuPreventDeselect> cancelAction;
        }
    }
}