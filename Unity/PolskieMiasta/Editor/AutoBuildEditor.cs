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
            var ready = GameObject.Find("Polskie_Miasta_v4");
            if (ready != null)
            {
                return; // mapa juz jest w scenie zapisana - nic nie rob
            }
            Miasta.WorldBuilder.Build();
            if (GameObject.Find("Polskie_Miasta_v4") == null) return;

            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("POLSKIE MIASTA: mapa gotowa i ustawiona w scenie!");

            // automatyczny zapis mapy do sceny (bez wciskania Ctrl+S)
            if (!string.IsNullOrEmpty(scene.path))
            {
                bool ok = EditorSceneManager.SaveScene(scene);
                Debug.Log(ok
                    ? "POLSKIE MIASTA: MAPA ZAPISANA NA STALE w scenie! Od teraz jest zawsze - wcisnij PLAY i graj."
                    : "POLSKIE MIASTA: nie udalo sie zapisac sceny - zapisz recznie Ctrl+S.");
            }
            else
            {
                Debug.Log("POLSKIE MIASTA: mapa w scenie. Zapicuj scene plikiem: File -> Save As... (nazwa np. Mapa) - wtedy zostanie na stale.");
            }
        }
    }
}
