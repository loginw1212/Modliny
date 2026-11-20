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
