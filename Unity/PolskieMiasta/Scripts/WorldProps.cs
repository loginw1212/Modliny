using UnityEngine;

namespace Miasta
{
    public class Magazyn : MonoBehaviour
    {
        public string cityName = "Miasto";
    }

    public class SalonNPC : MonoBehaviour
    {
        public string salonCity = "Miasto";
    }

    public static class LitShader
    {
        static Shader cached;

        public static Shader Get()
        {
            if (cached != null) return cached;
#if UNITY_2021_2_OR_NEWER
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null)
            {
                cached = Shader.Find("Universal Render Pipeline/Lit");
                if (cached == null) cached = Shader.Find("HDRP/Lit");
            }
#endif
            if (cached == null) cached = Shader.Find("Standard");
            if (cached == null) cached = Shader.Find("Sprites/Default");
            return cached;
        }
    }

    public static class Finder
    {
        public static T[] All<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }
    }

    public class CityHub : MonoBehaviour
    {
        public string cityName = "Miasto";
        public bool hasPaczkomat = true;
    }
}
