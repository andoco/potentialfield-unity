namespace Andoco.Unity.Framework.Movement.Movers.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Unity.Framework.Gui;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Mover))]
    public class MoverEditor : Editor
    {
        private PopupState popupState;

        public override void OnInspectorGUI()
        {
            this.SetupPopup();

            var mover = (Mover)this.target;

            EditorGuiHelper.AutoPropertyFields(
                this.serializedObject,
                new KeyValuePair<string, Action>("moveDriverName", () => MoveDriverField(mover)));

            var moverSpeed = mover.Driver == null ? 0f : mover.Driver.Speed;
            EditorGUILayout.LabelField("Speed", moverSpeed.ToString());

            this.serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(this.target);
            }
        }

        private void MoveDriverField(Mover mover)
        {
            popupState.SetSelected(mover.moveDriverName);

            popupState.SetSelected(EditorGUILayout.Popup(new GUIContent("Driver"), popupState.SelectedIndex, popupState.Options));

            mover.moveDriverName = popupState.SelectedName;
        }

        private void SetupPopup()
        {
            var mover = (Mover)this.target;
            var drivers = mover.GetDrivers();
            var labels = drivers.Select(x => string.Format("{0}", x.Name));

            this.popupState = new PopupState(labels);
        }
    }
}
