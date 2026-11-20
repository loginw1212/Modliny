using UnityEngine;
using UnityEngine.UI;

namespace Miasta
{
    public class MagazynUI : MonoBehaviour
    {
        public static MagazynUI I;
        public bool IsOpen { get { return root != null && root.activeSelf; } }

        GameObject root;
        GameObject takeBtn;
        Text titleT;
        Text bodyT;
        Magazyn mag;

        void Awake()
        {
            I = this;
            Build();
            root.SetActive(false);
        }

        void Build()
        {
            var canvas = UIFactory.RootCanvas();
            root = UIFactory.Panel(canvas.transform, "Magazyn_UI", new Color(0.2f, 0.26f, 0.34f, 0.98f), new Vector2(0f, 0f), new Vector2(500f, 320f));
            titleT = UIFactory.Label(root.transform, "Tytul", "MAGAZYN", 20, new Color(0.95f, 0.8f, 0.2f), new Vector2(0f, -30f), new Vector2(460f, 40f), TextAnchor.MiddleCenter);
            bodyT = UIFactory.Label(root.transform, "Tresc", "", 15, Color.white, new Vector2(0f, -105f), new Vector2(460f, 120f), TextAnchor.UpperLeft);
            takeBtn = UIFactory.Btn(root.transform, "B_Wez", "WEZ PACZKE", new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, -168f), new Vector2(420f, 48f), Take, 16).gameObject;
            UIFactory.Btn(root.transform, "B_Zamknij", "ZAMKNIJ (ESC)", new Color(0.4f, 0.4f, 0.46f), new Vector2(0f, -225f), new Vector2(420f, 40f), Close, 14);
        }

        public static void Open(Magazyn m)
        {
            if (I == null) return;
            I.mag = m;
            I.Refresh();
            I.root.SetActive(true);
        }

        public void Close() { if (root != null) root.SetActive(false); }

        void Update()
        {
            if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.Escape)) Close();
        }

        void Refresh()
        {
            if (mag == null) return;
            titleT.text = "MAGAZYN " + mag.cityName;
            bool canTake = false;
            var m = GameManager.I;
            string s = "";
            if (m.job == null)
            {
                s = "Nie masz zlecenia.\nW telefonie: PACZKI -> KURIER -> WEZ ZLECENIE.";
            }
            else if (m.job.loaded)
            {
                s = "Masz juz paczke przy sobie!\nCel: paczkomat " + m.job.toCity;
            }
            else if (m.job.fromCity == mag.cityName)
            {
                canTake = true;
                s = "Czeka na Ciebie paczka: " + m.job.item + "\nCel: paczkomat " + m.job.toCity + " (+" + m.job.payout + " coins)";
            }
            else
            {
                s = "Twoja paczka jest w magazynie " + m.job.fromCity + " - dojedz tam!";
            }
            bodyT.text = s;
            if (takeBtn != null) takeBtn.SetActive(canTake);
        }

        void Take()
        {
            var m = GameManager.I;
            if (m.job == null || m.job.loaded || mag == null) return;
            m.job.loaded = true;
            var p = m.GetPackage(m.job.pkgId);
            if (p != null) p.state = "Na_Pojazdzie";
            m.Notify("Paczka " + m.job.item + " w reku! Dojazd do paczkomatu " + m.job.toCity);
            PlayerDriver pd = PlayerDriver.I;
            if (pd != null) pd.SetCarrying(true);
            Close();
        }
    }
}
