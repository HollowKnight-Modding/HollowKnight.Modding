﻿using GlobalEnums;
using UnityEngine;

// ReSharper disable All
#pragma warning disable 1591

namespace Modding
{
    /// <inheritdoc />
    /// <summary>
    ///     Class to draw the version information for the mods on the main menu.
    /// </summary>
    public class ModVersionDraw : MonoBehaviour
    {
        private static GUIStyle style = GUIStyle.none;
        private Font font;

        /// <summary>
        ///     String to Draw
        /// </summary>
        public string drawString;

        /// <summary>
        ///     Run When GameObject is first active.
        /// </summary>
        private void Start()
        {
            font = new GUIStyle(GUI.skin.label).font;
        }

        /// <summary>
        ///     Run When Gui is shown.
        /// </summary>
        public void OnGUI()
        {
            if (UIManager.instance == null)
            {
                return;
            }

            if (drawString != null && UIManager.instance.uiState is UIState.MAIN_MENU_HOME or UIState.PAUSED)
            {
                style.font = font;
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.UpperLeft;
                style.padding = new RectOffset(5, 5, 5, 5);
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), drawString, style);
            }
        }
    }
}