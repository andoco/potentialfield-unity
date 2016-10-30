namespace Andoco.Unity.Framework.Gui
{
    using UnityEngine;

    public class GuiHelper
    {
        private GUIStyle sectionSpace;

        public GuiHelper()
        {
            this.sectionSpace = new GUIStyle();
            this.sectionSpace.normal.background = MakeTex(600, 1, new Color(0.25f, 0.25f, 0.25f, 1.0f));
        }

        public void InspectorSeparator()
        {
            GUILayout.Space(10f);
            GUILayout.BeginVertical(this.sectionSpace);
            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width*height];
            
            for(int i = 0; i < pix.Length; i++)
                pix[i] = col;
            
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            result.hideFlags = HideFlags.DontSave;
            
            return result;
        }
    }
}
