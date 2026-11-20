using UnityEngine;
using UnityEngine.UI;

namespace Miasta
{
    public class SalonUI : MonoBehaviour
    {
        public static SalonUI I;
        public bool IsOpen { get { return root != null && root.activeSelf; } }

        GameObject root;
        RectTransform listT;
        Text titleT;
        static GameObject currentCar;
        SalonNPC npc;

        void Awake()
        {
            I = this;
            Build();
            root.SetActive(false);
        }

        void Build()
        {
            var canvas = UIFactory.RootCanvas();
            root = UIFactory.Panel(canvas.transform, "Salon_UI", new Color(0.14f, 0.2f, 0.3f, 0.98f), new Vector2(0f, 0f), new Vector2(520f, 580f));
            titleT = UIFactory.Label(root.transform, "Tytul", "SALON SAMOCHODOWY", 20, new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, -30f), new Vector2(480f, 40f), TextAnchor.MiddleCenter);
            RectTransform c;
            UIFactory.ScrollList(root.transform, "Auta", new Color(0.1f, 0.14f, 0.2f, 0.9f), new Vector2(0f, -270f), new Vector2(480f, 430f), out c);
            listT = c;
            UIFactory.Btn(root.transform, "B_Zamknij", "ZAMKNIJ (ESC)", new Color(0.4f, 0.4f, 0.46f), new Vector2(0f, -270f), new Vector2(480f, 40f), Close, 14);
        }

        public static void Open(SalonNPC from)
        {
            if (I == null) return;
            I.npc = from;
            I.Rebuild();
            I.root.SetActive(true);
        }

        public void Close() { if (root != null) root.SetActive(false); }

        void Update()
        {
            if (root != null && root.activeSelf && InputGate.GetKeyDown(KeyCode.Escape)) Close();
        }

        void Rebuild()
        {
            titleT.text = "SALON - " + (npc != null ? npc.salonCity : "?");
            foreach (Transform c in listT) Destroy(c.gameObject);
            var m = GameManager.I;
            foreach (var car in Catalogs.Cars)
            {
                string id = car.id;
                int price = car.price;
                bool owned = m.ownedCars.Contains(id) || price == 0;
                var row = UIFactory.Panel(listT, "R_" + id, new Color(0.16f, 0.22f, 0.3f), Vector2.zero, new Vector2(0f, 74f));
                UIFactory.Label(row.transform, "TT", id + "\n" + car.desc + " | " + (car.maxSpeed * 3.6f).ToString("0") + " km/h | " + price + " coins", 14, Color.white, new Vector2(-70f, 0f), new Vector2(230f, 70f), TextAnchor.MiddleLeft);
                if (owned)
                {
                    UIFactory.Btn(row.transform, "BT", "WYBIERZ", new Color(0.2f, 0.7f, 0.4f), new Vector2(165f, 0f), new Vector2(120f, 42f), () => Pick(id), 13);
                }
                else
                {
                    var b = UIFactory.Btn(row.transform, "BT", price <= m.coins ? "KUP" : "ZA MALO", price <= m.coins ? new Color(0.95f, 0.6f, 0.1f) : new Color(0.4f, 0.2f, 0.2f), new Vector2(165f, 0f), new Vector2(120f, 42f), () => Buy(id, price), 13);
                }
            }
        }

        void Buy(string id, int price)
        {
            var m = GameManager.I;
            if (m.coins < price) { m.Notify("Masz za malo COINS! (" + m.coins + "/" + price + ")"); return; }
            m.coins -= price;
            m.ownedCars.Add(id);
            m.Save();
            m.Notify("Kupiono " + id + " za " + price + " coins!");
            Rebuild();
        }

        void Pick(string id)
        {
            if (npc == null) return;
            Vector3 basePos = npc.transform.position;
            Vector3 spawnPos = basePos + npc.transform.forward * 6f + Vector3.up * 0.2f;
            if (currentCar != null) Destroy(currentCar);
            var car = CarFactory.Spawn(id, spawnPos, false);
            currentCar = car;
            CarController ctrl = car != null ? car.GetComponent<CarController>() : null;
            if (ctrl != null && PlayerDriver.I != null) ctrl.Enter(PlayerDriver.I);
            GameManager.I.Notify("Auto " + id + " gotowe w salonie! Wszedles do srodka. Uwazaj na pieszych!");
            Close();
        }
    }
}
