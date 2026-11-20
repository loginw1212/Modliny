using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace MiastaEditor
{
    public static class CityWorldBuilderMenu
    {
        [MenuItem("Tools/Polskie Miasta - Zbuduj wielki swiat")]
        public static void Build()
        {
            Miasta.WorldBuilder.Build(true);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
