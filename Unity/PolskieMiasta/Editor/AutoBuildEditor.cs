using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MiastaEditor
{
    [InitializeOnLoad]
    public static class AutoBuildEditor
    {
        static AutoBuildEditor()
        {
            EditorApplication.delayCall += BuildOnce;
        }

        static void BuildOnce()
        {
            if (Application.isPlaying) return;
            Miasta.WorldBuilder.Build();
            if (GameObject.Find("Polskie_Miasta") != null)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                Debug.Log("POLSKIE MIASTA: swiat gotowy w scenie! Zapisz Ctrl+S i wcisnij Play.");
            }
        }
    }
}
