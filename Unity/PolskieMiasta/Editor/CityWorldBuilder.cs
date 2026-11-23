using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace MiastaEditor
{
    public static class CityWorldBuilderMenu
    {
        [MenuItem("Tools/Polskie Miasta - Zbuduj mape od nowa")]
        public static void Build()
        {
            Miasta.WorldBuilder.Build(true);
            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            if (!string.IsNullOrEmpty(scene.path))
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log("POLSKIE MIASTA: mapa przebudowana I ZAPISANA na stale!");
            }
            else Debug.Log("POLSKIE MIASTA: mapa przebudowana. Zapisz scene Ctrl+S.");
        }
    }
}
