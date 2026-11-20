using UnityEngine;

namespace Miasta
{
    public class CarController : MonoBehaviour
    {
        public string carId = "Kasztanka";
        public float baseMaxSpeed = 26f;
        public bool displayCar = false;

        PlayerDriver driver;
        float speed;

        void Update()
        {
            if (driver == null) return;
            if (InputGate.GetKeyDown(KeyCode.E)) { Exit(); return; }
            float accel = 0f;
            if (InputGate.GetKey(KeyCode.W)) accel = 1f;
            if (InputGate.GetKey(KeyCode.S)) accel = -0.6f;
            float steer = 0f;
            if (InputGate.GetKey(KeyCode.A)) steer = 1f;
            if (InputGate.GetKey(KeyCode.D)) steer = -1f;
            float max = baseMaxSpeed + (GameManager.I != null ? GameManager.I.turboBonus : 0f);
            speed = Mathf.MoveTowards(speed, accel * max, (accel != 0f ? 16f : 24f) * Time.deltaTime);
            if (Mathf.Abs(speed) > 0.1f)
                transform.Rotate(0f, steer * 65f * Time.deltaTime * Mathf.Clamp(Mathf.Abs(speed) / 14f, 0.25f, 1f) * Mathf.Sign(speed), 0f);
            transform.position += transform.forward * speed * Time.deltaTime;
            var pos = transform.position;
            float g = GroundY(pos);
            pos.y = Mathf.MoveTowards(pos.y, g, 8f * Time.deltaTime);
            transform.position = pos;
        }

        float GroundY(Vector3 p)
        {
            float best = 0f;
            foreach (var h in Physics.RaycastAll(p + Vector3.up * 3f, Vector3.down, 12f))
            {
                if (h.collider.transform.IsChildOf(transform)) continue;
                if (h.point.y > best) best = h.point.y;
            }
            return best;
        }

        public void Enter(PlayerDriver p)
        {
            driver = p;
            p.hidden = true;
            p.SetCarEnter(this);
            speed = 0f;
        }

        public void Exit()
        {
            if (driver == null) return;
            driver.hidden = false;
            driver.SetCarEnter(null);
            driver.transform.position = transform.position - transform.right * 2.5f + Vector3.up * 1.2f;
            driver = null;
        }

        public void SetPaint(Color c)
        {
            foreach (var r in GetComponentsInChildren<MeshRenderer>())
                if (r.name == "Karoseria" || r.name == "Kabina")
                    r.material.color = c;
        }

        public bool HasDriver { get { return driver != null; } }
    }
}
