using UnityEngine;

namespace Miasta
{
    public class TrafficCar : MonoBehaviour
    {
        public Vector3[] route;
        public float speed = 9f;
        int idx;

        void Update()
        {
            if (route == null || route.Length < 2) return;
            Vector3 t = route[idx];
            Vector3 flat = t - transform.position;
            flat.y = 0f;
            if (flat.magnitude < 1.5f)
            {
                idx = (idx + 1) % route.Length;
                return;
            }
            Vector3 dir = flat.normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 2.5f);
            float d = Mathf.Min(speed * Time.deltaTime, flat.magnitude);
            transform.position += dir * d;
        }
    }
}
