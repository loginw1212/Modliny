using UnityEngine;
using UnityEngine.UI;
using UnityEngine;

namespace Miasta
{
    public class PlayerDriver : MonoBehaviour
    {
        public static PlayerDriver I;
        public float walkSpeed = 5f;
        public float runSpeed = 9f;
        public float mouseSens = 2.2f;
        [HideInInspector] public bool hidden = false;
        [HideInInspector] public GameObject carryingPkg;

        CarController car;
        CharacterController cc;
        Camera cam;
        float pitch;
        float yaw;
        float vv;
        Text jobText;
        Text hintText;

        void Awake() { I = this; }

        void Start()
        {
            cc = GetComponent<CharacterController>();
            yaw = transform.eulerAngles.y;
            if (Camera.main == null)
            {
                var c = new GameObject("Kamera_Gracz");
                c.transform.SetParent(null);
                c.transform.position = transform.position + Vector3.up * 1.7f;
                cam = c.AddComponent<Camera>();
                cam.fieldOfView = 72f;
                cam.nearClipPlane = 0.08f;
            }
            else cam = Camera.main;
            BuildPkgVisual();
            BuildHud();
        }

        void RotateCam()
        {
            float mx = Input.GetAxis("Mouse X") * mouseSens;
            float my = Input.GetAxis("Mouse Y") * mouseSens;
            yaw += mx;
            pitch = Mathf.Clamp(pitch - my, -80f, 80f);
            transform.eulerAngles = new Vector3(0f, yaw, 0f);
        }

        void Move()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            float sp = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) * Mathf.Clamp(input.magnitude, 0f, 1f);
            Vector3 move = (transform.right * input.x + transform.forward * input.z).normalized * sp;
            if (cc.isGrounded) vv = -1f;
            else vv += Physics.gravity.y * 2f * Time.deltaTime;
            move.y = vv;
            cc.Move(move * Time.deltaTime);
        }

        void Update()
        {
            UpdateCarry();
            RefreshHud();
            if (hidden) return;
            RotateCam();
            Move();
            if (UIOpen()) return;
            if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        }

        bool UIOpen()
        {
            if (PaczkomatUI.I != null && PaczkomatUI.I.IsOpen) return true;
            if (MagazynUI.I != null && MagazynUI.I.IsOpen) return true;
            if (SalonUI.I != null && SalonUI.I.IsOpen) return true;
            return false;
        }

        void TryInteract()
        {
            if (car != null) { car.Exit(); return; }
            if (cam == null) return;
            RaycastHit hit;
            if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 6.5f)) return;
            var npc = hit.collider.GetComponentInParent<SalonNPC>();
            if (npc != null) { SalonUI.Open(npc); return; }
            var mg = hit.collider.GetComponentInParent<Magazyn>();
            if (mg != null) { MagazynUI.Open(mg); return; }
            var pk = hit.collider.GetComponentInParent<Paczkomat>();
            if (pk != null) { PaczkomatUI.Open(pk); return; }
            var cc2 = hit.collider.GetComponentInParent<CarController>();
            if (cc2 != null && !cc2.displayCar && !cc2.HasDriver) cc2.Enter(this);
        }

        public void SetCarEnter(CarController c)
        {
            car = c;
        }

        void BuildPkgVisual()
        {
            carryingPkg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carryingPkg.name = "Paczka_W_Reku";
            carryingPkg.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            var m = carryingPkg.GetComponent<MeshRenderer>();
            m.material = new Material(Shader.Find("Standard"));
            m.material.color = new Color(0.85f, 0.6f, 0.15f);
            carryingPkg.SetActive(false);
        }

        public void SetCarrying(bool on)
        {
            if (carryingPkg != null) carryingPkg.SetActive(on);
        }

        public bool IsCarrying { get { return carryingPkg != null && carryingPkg.activeSelf; } }

        void UpdateCarry()
        {
            if (carryingPkg == null || !carryingPkg.activeSelf) return;
            if (car != null)
            {
                carryingPkg.transform.position = car.transform.position + car.transform.up * 2.6f;
                carryingPkg.transform.rotation = car.transform.rotation;
            }
            else
            {
                Vector3 f = cam != null ? cam.transform.forward : transform.forward;
                f.y = 0f;
                if (f.sqrMagnitude < 0.01f) f = transform.forward;
                f.Normalize();
                carryingPkg.transform.position = transform.position + f * 0.9f + Vector3.up * 1.3f;
                carryingPkg.transform.rotation = Quaternion.LookRotation(f);
            }
        }

        void BuildHud()
        {
            var canvas = UIFactory.RootCanvas();
            var hgo = UIFactory.Panel(canvas.transform, "HUD_Zlecenie", new Color(0.1f, 0.12f, 0.15f, 0.85f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-280f, -50f), new Vector2(280f, 0f));
            jobText = UIFactory.Label(hgo.transform, "TT", "", 16, new Color(1f, 0.9f, 0.5f), new Vector2(0f, -30f), new Vector2(520f, 56f), TextAnchor.UpperLeft);
            var hgo2 = UIFactory.Panel(canvas.transform, "HUD_Podpowiedz", new Color(0.1f, 0.12f, 0.15f, 0.85f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-200f, 20f), new Vector2(200f, 50f));
            hintText = UIFactory.Label(hgo2.transform, "TT", "", 15, Color.white, new Vector2(0f, 0f), new Vector2(380f, 30f), TextAnchor.MiddleCenter);
        }

        void RefreshHud()
        {
            string jt = "";
            string ht = "";
            var m = GameManager.I;
            if (m != null && m.job != null)
            {
                if (m.job.loaded) jt = "ZLECENIE: wiezies " + m.job.item + " -> paczkomat " + m.job.toCity + " (+" + m.job.payout + " coins)";
                else jt = "ZLECENIE: wez " + m.job.item + " z MAGAZYNU " + m.job.fromCity;
            }
            else jt = "";
            if (car != null) ht = "E - wysiad | W/S gaz/hamulec | A/D skret | H - telefon";
            else
            {
                string kind = "";
                if (cam != null)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 6.5f))
                    {
                        if (hit.collider.GetComponentInParent<SalonNPC>() != null) kind = "sprzedawca salonu";
                        else if (hit.collider.GetComponentInParent<Magazyn>() != null) kind = "magazyn";
                        else if (hit.collider.GetComponentInParent<Paczkomat>() != null) kind = "paczkomat";
                        else if (hit.collider.GetComponentInParent<CarController>() != null && !hit.collider.GetComponentInParent<CarController>().displayCar) kind = "auto";
                    }
                }
                if (kind != "") ht = "E - " + kind + " | H - telefon";
            }
            if (jobText != null) jobText.text = jt;
            if (hintText != null) hintText.text = ht;
        }

        void LateUpdate()
        {
            if (cam == null) return;
            if (!hidden && car == null)
            {
                cam.transform.position = transform.position + Vector3.up * 1.7f;
                cam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }
            else if (car != null)
            {
                Vector3 target = car.transform.position - car.transform.forward * 8.5f + Vector3.up * 4.2f;
                cam.transform.position = Vector3.Lerp(cam.transform.position, target, Time.deltaTime * 6f);
                cam.transform.LookAt(car.transform.position + Vector3.up * 1.4f);
            }
        }

        public static CityHub NearestCityInfo()
        {
            CityHub best = null;
            if (I == null) return null;
            foreach (var c in Object.FindObjectsOfType<CityHub>())
            {
                if (best == null) best = c;
                else if (Vector3.Distance(c.transform.position, I.transform.position) < Vector3.Distance(best.transform.position, I.transform.position)) best = c;
            }
            return best;
        }
    }
}
