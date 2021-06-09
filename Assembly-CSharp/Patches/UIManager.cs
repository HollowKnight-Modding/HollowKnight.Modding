﻿using Modding.Menu;
using UnityEngine;
using MonoMod;
using System.Collections;

// ReSharper disable All
#pragma warning disable 1591, 0108, 0169, 0649, 0414, CS0626

namespace Modding.Patches
{
    [MonoModPatch("global::UIManager")]
    public class UIManager : global::UIManager
    {
        [MonoModIgnore]
        private static UIManager _instance;

        [MonoModIgnore]
        private InputHandler ih;

        public MenuScreen currentDynamicMenu { get; set; }

        public static UIManager get_instance()
        {
            if (UIManager._instance == null)
            {
                UIManager._instance = UnityEngine.Object.FindObjectOfType<UIManager>();

                if (UIManager._instance == null)
                {
                    return null;
                }

                if (Application.isPlaying)
                {
                    UnityEngine.Object.DontDestroyOnLoad(UIManager._instance.gameObject);
                }
            }

            return UIManager._instance;
        }

        public extern IEnumerator orig_HideCurrentMenu();

        public IEnumerator HideCurrentMenu()
        {
            if (((MainMenuState)menuState) == MainMenuState.DYNAMIC_MENU)
            {
                return HideMenu(currentDynamicMenu);
            }
            else
            {
                return orig_HideCurrentMenu();
            }
        }

        [MonoModIgnore]
        public extern void SetMenuState(MainMenuState state);
        [MonoModIgnore]
        public extern Coroutine StartMenuAnimationCoroutine(IEnumerator coro);

        public void UIGoToDynamicMenu(MenuScreen menu, System.Action preLeaveAction = null)
        {
            StartMenuAnimationCoroutine(GoToDynamicMenu(menu, preLeaveAction));
        }

        public IEnumerator GoToDynamicMenu(MenuScreen menu, System.Action preLeaveAction = null)
        {
            ih.StopUIInput();
            if (preLeaveAction != null) preLeaveAction();
            yield return HideCurrentMenu();
            yield return ShowMenu(menu);
            currentDynamicMenu = menu;
            SetMenuState(MainMenuState.DYNAMIC_MENU);
            ih.StartUIInput();
            yield break;
        }

        public void UILeaveDynamicMenu(MenuScreen to, MainMenuState state) {
            StartMenuAnimationCoroutine(LeaveDynamicMenu(to, state));
        }

        public IEnumerator LeaveDynamicMenu(MenuScreen to, MainMenuState state) {
            ih.StopUIInput();
            yield return HideCurrentMenu();
            yield return ShowMenu(to);
            SetMenuState(state);
            ih.StartUIInput();
            yield break;
        }
    }

    [MonoModPatch("GlobalEnums.MainMenuState")]
    public enum MainMenuState
    {
        LOGO,
        MAIN_MENU,
        OPTIONS_MENU,
        GAMEPAD_MENU,
        KEYBOARD_MENU,
        SAVE_PROFILES,
        AUDIO_MENU,
        VIDEO_MENU,
        EXIT_PROMPT,
        OVERSCAN_MENU,
        GAME_OPTIONS_MENU,
        ACHIEVEMENTS_MENU,
        QUIT_GAME_PROMPT,
        RESOLUTION_PROMPT,
        BRIGHTNESS_MENU,
        PAUSE_MENU,
        PLAY_MODE_MENU,
        EXTRAS_MENU,
        REMAP_GAMEPAD_MENU,
        EXTRAS_CONTENT_MENU,
        ENGAGE_MENU,
        NO_SAVE_MENU,
        // Added for the dynamic menu API
        DYNAMIC_MENU
    }
}