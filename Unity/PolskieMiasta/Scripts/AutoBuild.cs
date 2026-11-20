using UnityEngine;

namespace Miasta
{
    public static class AutoBuild
    {
        static bool done;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Start()
        {
            if (done) return;
            done = true;
            WorldBuilder.Build();
        }
    }
}
