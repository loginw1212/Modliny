using UnityEngine;
using UnityEngine.UI;

namespace Miasta
{
    public class PaczkomatUI : MonoBehaviour
    {
        public static PaczkomatUI I;
        public bool IsOpen { get { return root != null && root.activeSelf; } }

        GameObject root;
        Transform btnArea;
        Text bodyT;
        Text typedT;
        Paczkomat machine;
        string typed = "";

        void Awake()
        {
            I = this;
            Build();
            root.SetActive(false);
        }

        void Build()
        {
            var canvas = UIFactory.RootCanvas();
            root = UIFactory.Panel(canvas.transform, "Paczkomat_UI", new Color(0.95f, 0.6f, 0.1f, 0.98f), new Vector2(0f, 0f), new Vector2(470f, 640f));
            var back = UIFactory.Panel(root.transform, "Ekranik", new Color(0.1f, 0.12f, 0.15f), new Vector2(0f, 56f), new Vector2(440f, 570f));
            var trt = back.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0f, 1f);
            trt.anchorMax = Vector2.one;
            trt.pivot = new Vector2(0.5f, 1f);
            trt.anchoredPosition = new Vector2(0f, -8f);
            trt.sizeDelta = new Vector2(-24f, 0f);
            btnArea = back.transform;

            bodyT = UIFactory.Label(back.transform, "Tresc", "", 15, Color.white, new Vector2(0f, -55f), new Vector2(420f, 80f), TextAnchor.UpperLeft);
            typedT = UIFactory.Label(back.transform, "Kod", "KOD: ", 22, new Color(0.4f, 1f, 0.6f), new Vector2(0f, -128f), new Vector2(420f, 34f), TextAnchor.MiddleLeft);

            for (int i = 1; i <= 9; i++)
            {
                int k = i;
                int col = (i - 1) % 3;
                int row = 225 + (i - 1) / 3 * 62;
                int num = i;
                UIFactory.Btn(back.transform, "K" + i, num.ToString(), new Color(0.2f, 0.26f, 0.34f), new Vector2(-130f + col * 130f, -row), new Vector2(120f, 52f), () => { typed += num.ToString(); Refresh(); }, 20);
            }
            UIFactory.Btn(back.transform, "K_CLR", "WYCZYSC", new Color(0.7f, 0.25f, 0.25f), new Vector2(-130f, -411f), new Vector2(120f, 52f), () => { typed = ""; Refresh(); }, 12);
            UIFactory.Btn(back.transform, "K_0", "0", new Color(0.2f, 0.26f, 0.34f), new Vector2(0f, -411f), new Vector2(120f, 52f), () => { typed += "0"; Refresh(); }, 20);
            UIFactory.Btn(back.transform, "K_OK", "OK", new Color(0.2f, 0.7f, 0.4f), new Vector2(130f, -411f), new Vector2(120f, 52f), () => OnOk(), 16);
            UIFactory.Label(back.transform, "Esc_Hint", "ESC / ZAMKNIJ - wystarczy wcisnac ESC", 11, new Color(1f, 1f, 1f, 0.6f), new Vector2(0f, -452f), new Vector2(420f, 22f), TextAnchor.MiddleCenter);
        }

        void OnOk()
        {
            if (machine == null) return;
            if (typed.Length < 6) { bodyT.text = "Kod ma 6 cyfr!\n" + currentInfo(); Refresh(); return; }
            if (machine.TryByCode(typed)) GameManager.I.Notify("Skrytka otwarta! Wcisnij ZABIERZ.");
            else GameManager.I.Notify("Zly kod!");
            typed = "";
            Refresh();
        }

        public static void Open(Paczkomat p)
        {
            if (I == null) return;
            I.machine = p;
            I.typed = "";
            I.root.SetActive(true);
            I.Refresh();
        }

        public void Close() { if (root != null) root.SetActive(false); }

        void Update()
        {
            if (root != null && root.activeSelf && InputGate.GetKeyDown(KeyCode.Escape)) Close();
        }
        
        string currentInfo()
        {
            if (machine == null) return "";
            var m = GameManager.I;
            string s = machine.cityName + " | skrytki zajete: " + machine.OccupiedCount() + "/" + machine.slots.Count;
            if (m.job != null && m.job.loaded) s += "\nKURIER: masz paczke " + m.job.item + " do " + m.job.toCity;
            return s;
        }

        void Refresh()
        {
            if (machine == null) return;
            bodyT.text = currentInfo();
            typedT.text = typed == "" ? "KOD: _ _ _ _ _ _" : "KOD: " + typed;
            foreach (Transform c in btnArea)
                if (c.name == "D_Kurier" || c.name == "D_Zabierz")
                    Destroy(c.gameObject);
            var m = GameManager.I;
            if (m.job != null && m.job.loaded)
                UIFactory.Btn(btnArea, "D_Kurier", "SKANUJ I WSADZ PACZKE", new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, -170f), new Vector2(420f, 44f), DelDalej, 15);
            else
            {
                var openSlot = FindOpenSlot();
                if (openSlot != null)
                {
                    var s = openSlot;
                    UIFactory.Btn(btnArea, "D_Zabierz", "ZABIERZ PACZKE (" + s.item + ")", new Color(0.2f, 0.7f, 0.4f), new Vector2(0f, -112f), new Vector2(420f, 44f), () => Take(s), 15);
                }
            }
        }

        Paczkomat.Slot FindOpenSlot()
        {
            foreach (var s in machine.slots)
                if (s.open && s.pkg != null && s.pkg.activeSelf && s.item != "") return s;
            return null;
        }

        void DelDalej()
        {
            GameManager.I.DeliverCurrentJobTo(machine);
            Refresh();
        }

        void Take(Paczkomat.Slot s)
        {
            GameManager.I.AddCoins(20);
            GameManager.I.Notify("Obrales " + s.item + "! (+20 coins)");
            machine.Collect(s);
            Refresh();
        }
    }
}
