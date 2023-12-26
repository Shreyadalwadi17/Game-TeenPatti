using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.BaseFramework
{

    [InitializeOnLoad]
    public class CanvasHierarchyEditor
    {

        public static Texture2D IconON;
        public static Texture2D IconOFF;



        static CanvasHierarchyEditor()
        {

            IconON =
               AssetDatabase.LoadAssetAtPath("Assets/Project/Scripts/UIFramwork/UIBase/Editor/UtilityImage/On.png", typeof(Texture2D)) as
                   Texture2D;
            IconOFF = AssetDatabase.LoadAssetAtPath("Assets/Project/Scripts/UIFramwork/UIBase/Editor/UtilityImage/Off.png",
                typeof(Texture2D)) as Texture2D;

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {

            GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

            if (obj)
            {


                Canvas canvas;
                UIController controller;

                if (canvas = obj.GetComponent<Canvas>())
                {

                    controller = obj.GetComponent<UIController>();

                    if (controller == null)
                    {


                        Rect buttonRect = new Rect(selectionRect);

                        buttonRect.x = Screen.width - Screen.width / 10;

                        buttonRect.width = 40;
                        buttonRect.height = 17;

                        Texture2D texture;

                        if (canvas.enabled)
                        {
                            texture = IconON;
                        }
                        else
                        {
                            texture = IconOFF;
                        }

                        if (GUI.Button(buttonRect, texture, GUI.skin.label))
                        {

                            Undo.RecordObject(canvas, "canvasOfSelectedObject");
                            canvas.enabled = !canvas.enabled;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                        }
                    }
                }





            }
        }

    }

}