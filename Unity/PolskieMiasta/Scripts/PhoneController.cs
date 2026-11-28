using UnityEngine;
using UnityEngine.UI;

namespace Miasta
{
    public class PhoneController : MonoBehaviour
    {
        public static PhoneController I;
        const string ADMIN_LOGIN = "admin";
        const string ADMIN_PASS = "admin123";
        string lastApp = "paczki";

        RectTransform contentT;
        RectTransform pulpitT;
        RectTransform appT;
        GameObject appGO;
        GameObject phonePanel;
        Text coinsText;
        Text feedText;
        float feedTimer;

        void Awake() { I = this; }

        void Start()
        {
            Build();
            GameManager.I.OnNotify += Notify;
        }

        void Update()
        {
            if (GameManager.I != null) coinsText.text = "COINS: " + GameManager.I.coins;
            if (feedTimer > 0f)
            {
                feedTimer -= Time.deltaTime;
                if (feedTimer <= 0f) feedText.text = "";
            }
            if (!InputGate.TypingInField() && InputGate.GetKeyDown(KeyCode.H)) TogglePhone();
        }

        public void Notify(string msg)
        {
            feedText.text = msg;
            feedTimer = 6f;
        }

        void TogglePhone()
        {
            bool t = !phonePanel.activeSelf;
            phonePanel.SetActive(t);
        }

        public void OpenPhone()
        {
            if (!phonePanel.activeSelf) phonePanel.SetActive(true);
        }

        void Build()
        {
            var canvas = UIFactory.RootCanvas();
            var phone = UIFactory.Panel(canvas.transform, "Telefon", new Color(0.08f, 0.09f, 0.12f, 0.94f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-356f, 10f), new Vector2(-10f, 648f));
            phone.GetComponent<Image>();
            var ph = phone.GetComponent<RectTransform>();
            phonePanel = phone;

            var screen = UIFactory.Panel(phone.transform, "Ekran", new Color(0.12f, 0.16f, 0.2f, 1f), Vector2.zero, Vector2.one, new Vector2(6f, 50f), new Vector2(-6f, -6f));
            var header = UIFactory.Panel(screen.transform, "Naglowek", new Color(0.95f, 0.6f, 0.1f, 1f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var hrt = header.GetComponent<RectTransform>();
            hrt.anchorMin = new Vector2(0f, 1f);
            hrt.anchorMax = Vector2.one;
            hrt.pivot = new Vector2(0.5f, 1f);
            hrt.anchoredPosition = Vector2.zero;
            hrt.sizeDelta = new Vector2(0f, 50f);

            coinsText = UIFactory.Label(header.transform, "Coins", "COINS: 0", 20, Color.white, new Vector2(-70f, -25f), new Vector2(180f, 30f), TextAnchor.MiddleLeft);

            feedText = UIFactory.Label(screen.transform, "Info", "", 15, new Color(0.7f, 0.95f, 1f), new Vector2(0f, -75f), new Vector2(270f, 40f), TextAnchor.UpperCenter);

            contentT = UIFactory.Panel(screen.transform, "Zawartosc", new Color(0.12f, 0.16f, 0.2f, 0.6f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero)
                .GetComponent<RectTransform>();
            var crt = contentT;
            crt.anchorMin = new Vector2(0f, 0f);
            crt.anchorMax = Vector2.one;
            crt.pivot = new Vector2(0.5f, 0.5f);
            crt.anchoredPosition = new Vector2(0f, -25f);
            crt.sizeDelta = new Vector2(0f, -100f);
            pulpitT = contentT;

            appGO = UIFactory.Panel(screen.transform, "Aplikacja_Root", new Color(0.12f, 0.16f, 0.2f, 0.99f), Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));
            appT = appGO.GetComponent<RectTransform>();
            appGO.SetActive(false);

            var tabs = UIFactory.Panel(phone.transform, "Przyciski", new Color(0.06f, 0.07f, 0.1f, 1f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var trt = tabs.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.pivot = new Vector2(0.5f, 0f);
            trt.anchoredPosition = Vector2.zero;
            trt.sizeDelta = new Vector2(0f, 44f);
            float w = 82f;
            UIFactory.Btn(tabs.transform, "T_Paczki", "PACZKI", new Color(0.95f, 0.6f, 0.1f), new Vector2(-1.5f * w, -22f), new Vector2(w - 4f, 36f), () => OpenApp("paczki"));
            UIFactory.Btn(tabs.transform, "T_Sklep", "SKLEP", new Color(0.2f, 0.7f, 0.4f), new Vector2(-0.5f * w, -22f), new Vector2(w - 4f, 36f), () => OpenApp("sklep"));
            UIFactory.Btn(tabs.transform, "T_Mapa", "MAPA", new Color(0.3f, 0.5f, 0.9f), new Vector2(0.5f * w, -22f), new Vector2(w - 4f, 36f), () => OpenApp("mapa"));
            UIFactory.Btn(tabs.transform, "T_Admin", "ADMIN", new Color(0.8f, 0.25f, 0.25f), new Vector2(1.5f * w, -22f), new Vector2(w - 4f, 36f), () => OpenApp("admin"));
            var hideBtn = UIFactory.Btn(phone.transform, "T_Ukryj", "X", new Color(0.5f, 0.2f, 0.2f), Vector2.zero, new Vector2(30f, 30f), () => TogglePhone(), 14);
            var hrt2 = hideBtn.GetComponent<RectTransform>();
            hrt2.anchorMin = new Vector2(1f, 1f);
            hrt2.anchorMax = new Vector2(1f, 1f);
            hrt2.pivot = new Vector2(1f, 1f);
            hrt2.anchoredPosition = new Vector2(-4f, -4f);

            BuildPhoneIcon(canvas.transform);

            OpenApp("menu");
        }

        void BuildPhoneIcon(Transform parent)
        {
            var icon = UIFactory.Btn(parent, "Ikonka_Telefonu", "", new Color(0.1f, 0.11f, 0.13f, 0.95f), Vector2.zero, new Vector2(48f, 80f), () => TogglePhone());
            var irt = icon.GetComponent<RectTransform>();
            irt.anchorMin = new Vector2(1f, 0f);
            irt.anchorMax = new Vector2(1f, 0f);
            irt.pivot = new Vector2(0.5f, 0.5f);
            irt.anchoredPosition = new Vector2(-408f, 55f);

            var scr = UIFactory.Panel(icon.transform, "Ikonka_Ekran", new Color(0.25f, 0.7f, 0.4f, 1f), Vector2.zero, Vector2.zero);
            var srt = scr.GetComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = Vector2.one;
            srt.offsetMin = new Vector2(7f, 16f);
            srt.offsetMax = new Vector2(-7f, -12f);
            UIFactory.Label(scr.transform, "Ikonka_Litera", "P", 16, Color.white, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter)
                .GetComponent<RectTransform>().sizeDelta = new Vector2(34f, 24f);

            var dot = UIFactory.Panel(icon.transform, "Ikonka_Dom", new Color(0.85f, 0.85f, 0.85f, 1f), Vector2.zero, Vector2.zero);
            var drt = dot.GetComponent<RectTransform>();
            drt.anchorMin = new Vector2(0.5f, 0f);
            drt.anchorMax = new Vector2(0.5f, 0f);
            drt.pivot = new Vector2(0.5f, 0f);
            drt.anchoredPosition = new Vector2(0f, 4f);
            drt.sizeDelta = new Vector2(10f, 3f);
        }

        public void Rebuild()
        {
            OpenApp(lastApp);
        }

        void OpenApp(string which)
        {
            lastApp = which;
            if (which == "menu")
            {
                contentT = pulpitT;
                appGO.SetActive(false);
                foreach (Transform c in contentT) Destroy(c.gameObject);
                AppMenu();
                return;
            }
            contentT = appT;
            appGO.SetActive(true);
            foreach (Transform c in contentT) Destroy(c.gameObject);
            try
            {
                if (which == "paczki") AppPaczkiRoot();
                else if (which == "sklep") AppSklep();
                else if (which == "mapa") AppMapa();
                else if (which == "admin") AppAdmin();
            }
            catch (System.Exception e)
            {
                Debug.LogError("BLAD APLIKACJI '" + which + "': " + e.Message + "\n" + e.StackTrace);
            }
            AddAppCloseBtn();
        }

        void AddAppCloseBtn()
        {
            var x = UIFactory.Btn(contentT, "X_App", "X", new Color(0.5f, 0.2f, 0.2f), Vector2.zero, new Vector2(34f, 34f), () => OpenApp("menu"), 15);
            var xrt = x.GetComponent<RectTransform>();
            xrt.anchorMin = new Vector2(1f, 1f);
            xrt.anchorMax = new Vector2(1f, 1f);
            xrt.pivot = new Vector2(1f, 1f);
            xrt.anchoredPosition = new Vector2(-6f, -6f);
        }

        // ===== PULPIT =====

        void AppMenu()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "APLIKACJE", 20, Color.white, new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            MakeTile(rt, 165f, "PACZKI", new Color(0.95f, 0.6f, 0.1f), "paczki");
            MakeTile(rt, 95f, "SKLEP", new Color(0.2f, 0.7f, 0.4f), "sklep");
            MakeTile(rt, 25f, "MAPA", new Color(0.3f, 0.5f, 0.9f), "mapa");
            MakeTile(rt, -45f, "ADMIN", new Color(0.8f, 0.25f, 0.25f), "admin");
        }

        void MakeTile(Transform rt, float y, string label, Color c, string app)
        {
            UIFactory.Btn(rt, "Tile_" + app, label, c, new Vector2(20f, y), new Vector2(270f, 58f), () => OpenApp(app), 18);
        }

        // ===== PACZKI =====

        void AppPaczkiRoot()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "APLIKACJA PACZKI", 20, Color.white, new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            UIFactory.Btn(rt, "B_Kurier", "JESTEM KURIEREM", new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, 155f), new Vector2(290f, 60f), () => AppPaczkiKurier(), 17);
            UIFactory.Btn(rt, "B_Klient", "JESTEM KLIENTEM", new Color(0.2f, 0.7f, 0.4f), new Vector2(0f, 80f), new Vector2(290f, 60f), () => AppPaczkiKlient(), 17);
        }

        void AppPaczkiKurier()
        {
            var rt = contentT;
            var m = GameManager.I;
            UIFactory.Label(rt, "Tytul", "KURIER", 20, Color.white, new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            var jobT = UIFactory.Label(rt, "Zlecenie", "", 14, new Color(0.95f, 0.8f, 0.4f), new Vector2(0f, 160f), new Vector2(320f, 110f), TextAnchor.UpperLeft);
            if (m.job == null)
            {
                jobT.text = "Nie masz zlecenia.";
                UIFactory.Btn(rt, "B_Wez", "WEZ ZLECENIE", new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, 75f), new Vector2(290f, 44f), () => { GameManager.GiveJobSafe(); AppPaczkiKurier(); }, 15);
                var npt = UIFactory.Label(rt, "NaglowekNPC", "ZAMOWIENIA NPC:", 14, new Color(0.5f, 0.9f, 0.7f), new Vector2(0f, 25f), new Vector2(320f, 26f), TextAnchor.MiddleCenter);
                float ny = -10f;
                if (m.npcOrders.Count == 0) npt.text = "ZAMOWIENIA NPC: (brak, poczekaj)";
                int shown = 0;
                foreach (var n in m.npcOrders)
                {
                    if (n == null || shown >= 3) continue;
                    shown++;
                    var cc = n;
                    UIFactory.Btn(rt, "B_NPC_" + shown, n.npcName + ": " + n.wantsItem, new Color(0.2f, 0.7f, 0.4f), new Vector2(0f, ny), new Vector2(290f, 34f), () => { GameManager.I.GiveNpcJob(cc); AppPaczkiKurier(); }, 12);
                    ny -= 40f;
                }
                UIFactory.Btn(rt, "B_Wstecz", "< MENU", new Color(0.3f, 0.3f, 0.34f), new Vector2(0f, -195f), new Vector2(290f, 38f), () => OpenApp("menu"), 14);
                return;
            }
            string state;
            if (m.job.npcName != null)
            {
                if (m.job.loaded) state = "Paczka w rece! Dowiez do NPC " + m.job.npcName + " (w " + m.job.toCity + "), patrz na niego i E.";
                else state = "1. Magazyn " + m.job.fromCity + " -> E -> wez paczke. 2. Do NPC " + m.job.npcName + " w " + m.job.toCity + ".";
            }
            else if (m.job.loaded) state = "Paczka w rece! Dojedz do PACZKOMATU " + m.job.toCity + " i wcisnij E.";
            else state = "1. Magazyn " + m.job.fromCity + " -> E -> wez paczke. 2. Paczkomat " + m.job.toCity + ".";
            jobT.text = m.job.item + "\nZAPLATA: " + m.job.payout + " coins\n" + state;
            if (!m.job.loaded)
                UIFactory.Btn(rt, "B_Porzuc", "PORZUC ZLECENIE", new Color(0.7f, 0.25f, 0.25f), new Vector2(0f, 35f), new Vector2(290f, 36f), () => { GameManager.AbandonJobSafe(); AppPaczkiKurier(); }, 13);
            var info = UIFactory.Label(rt, "Paczkomaty", "", 13, Color.white, new Vector2(0f, -35f), new Vector2(320f, 160f), TextAnchor.UpperLeft);
            string ps = "PACZKOMATY (droga):\n";
            foreach (var c in Finder.All<CityHub>())
            {
                float d2 = PlayerDriver.I != null ? Vector3.Distance(CityUtils.CityPos(c.cityName), PlayerDriver.I.transform.position) : 0f;
                string mark = m.job.npcName == null && c.cityName == m.job.toCity ? "  <<< CEL" : "";
                ps += c.cityName + ": " + Mathf.RoundToInt(d2) + " m" + mark + "\n";
            }
            info.text = ps;
            UIFactory.Btn(rt, "B_Wstecz", "< MENU", new Color(0.3f, 0.3f, 0.34f), new Vector2(0f, -195f), new Vector2(290f, 38f), () => OpenApp("menu"), 14);
        }

        void AppPaczkiKlient()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "KLIENT - zamow paczke", 18, Color.white, new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            RectTransform list;
            UIFactory.ScrollList(rt, "Lista", new Color(0.1f, 0.14f, 0.18f, 0.8f), new Vector2(0f, 145f), new Vector2(300f, 150f), out list);
            foreach (var it in Catalogs.ShopItems)
            {
                string item = it[0];
                int price = int.Parse(it[1]);
                var row = UIFactory.Panel(list, "R_" + item, new Color(0.16f, 0.22f, 0.28f), Vector2.zero, new Vector2(0f, 36f));
                UIFactory.Label(row.transform, "TT", item + " - " + price, 13, Color.white, new Vector2(-75f, 0f), new Vector2(150f, 36f), TextAnchor.MiddleLeft);
                UIFactory.Btn(row.transform, "BT", "ZAMOW", new Color(0.2f, 0.7f, 0.4f), new Vector2(95f, 0f), new Vector2(80f, 28f), () => { OrderFromPhone(item, price); }, 11);
            }
            var mine = UIFactory.Label(rt, "Moje", "", 12, new Color(0.5f, 0.9f, 0.7f), new Vector2(0f, -35f), new Vector2(320f, 290f), TextAnchor.UpperLeft);
            string ms = "MOJE PRZESYLKI:\n";
            foreach (var pp in GameManager.I.packages)
            {
                string st = pp.state == "W_Skrytce" ? "W PACZKOMACIE " + pp.toCity + "! KOD: " + pp.code
                    : pp.state == "Zamowiona" ? "kurier jedzie..."
                    : pp.state == "W_Magazynie" ? "w magazynie " + pp.toCity
                    : pp.state == "Na_Pojazdzie" ? "w drodze do " + pp.toCity
                    : "odebrana";
                ms += "*" + pp.item + ": " + st + "\n";
            }
            mine.text = ms;
            UIFactory.Btn(rt, "B_Wstecz", "< MENU", new Color(0.3f, 0.3f, 0.34f), new Vector2(0f, -195f), new Vector2(290f, 38f), () => OpenApp("menu"), 14);
        }

        void OrderFromPhone(string item, int price)
        {
            string myCity = "Kasztanowice";
            PlayerDriver pd = PlayerDriver.I;
            if (pd != null)
            {
                CityHub nc = PlayerDriver.NearestCityInfo();
                if (nc != null) myCity = nc.cityName;
            }
            GameManager.I.PlaceClientOrder(item, price, myCity);
            AppPaczkiKlient();
        }

        // ===== SKLEP =====

        void AppSklep()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "SKLEP (coins)", 20, Color.white, new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            float y = 160f;
            foreach (var it in Catalogs.PhoneShop)
            {
                int price = int.Parse(it[1]);
                string tag = it[2];
                var row = UIFactory.Panel(rt, "R_" + tag, new Color(0.16f, 0.22f, 0.28f), new Vector2(0f, y), new Vector2(310f, 62f));
                UIFactory.Label(row.transform, "TT", it[0] + "\n" + it[1] + " coins", 14, Color.white, new Vector2(-75f, 0f), new Vector2(170f, 55f), TextAnchor.MiddleLeft);
                UIFactory.Btn(row.transform, "BT", "KUP", new Color(0.95f, 0.6f, 0.1f), new Vector2(105f, 0f), new Vector2(75f, 38f), () => BuyUpgrade(tag, price), 14);
                y -= 72f;
            }
        }

        void BuyUpgrade(string tag, int price)
        {
            var m = GameManager.I;
            if (m.coins < price) { m.Notify("Masz za malo COINS!"); return; }
            m.coins -= price;
            m.Save();
            if (tag == "turbo") { m.turboBonus = 12f; m.Notify("Turbo kupione! +12 predkosci."); }
            else if (tag == "kurier") { m.courierBonus = 0.5f; m.Notify("Stawka kuriera +50%!"); }
            else if (tag == "lakier") { m.goldPaint = "tak"; m.Notify("Zloty lakier kupiony!"); }
            m.Notify("Kupiono za " + price + " coins. Zostalo: " + m.coins);
        }

        // ===== MAPA =====

        void AppMapa()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "MAPA - miasta", 18, Color.white, new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            RectTransform list;
            UIFactory.ScrollList(rt, "Lista", new Color(0.1f, 0.14f, 0.18f, 0.8f), new Vector2(0f, 60f), new Vector2(300f, 330f), out list);
            PlayerDriver pd = PlayerDriver.I;
            Vector3 pp = Vector3.zero;
            if (pd != null) pp = pd.transform.position;
            foreach (var c in Finder.All<CityHub>())
            {
                string city = c.cityName;
                float d3 = Vector3.Distance(CityUtils.CityPos(city), pp);
                var row = UIFactory.Panel(list, "R_" + city, new Color(0.16f, 0.22f, 0.28f), Vector2.zero, new Vector2(0f, 48f));
                UIFactory.Label(row.transform, "TT", city + "\ndroga ~" + Mathf.RoundToInt(d3) + " m", 13, Color.white, new Vector2(-10f, 0f), new Vector2(200f, 48f), TextAnchor.MiddleLeft);
                bool ziel = c.hasPaczkomat;
                UIFactory.Label(row.transform, "ST", ziel ? "OK" : "-", 14, ziel ? new Color(0.3f, 1f, 0.5f) : new Color(0.8f, 0.3f, 0.3f), new Vector2(115f, 0f), new Vector2(50f, 48f), TextAnchor.MiddleCenter);
            }
            UIFactory.Label(rt, "Podpowiedzi", "Jedz miedzy miastami droga!\nW miescie: paczkomat, salon, magazyn,\nBiedronka, sklep GSM.", 12, new Color(0.6f, 0.85f, 1f), new Vector2(0f, -160f), new Vector2(320f, 70f), TextAnchor.UpperCenter);
        }

        // ===== ADMIN =====

        void AppAdmin()
        {
            var rt = contentT;
            var m = GameManager.I;
            UIFactory.Label(rt, "Tytul", "ADMIN PANEL", 20, new Color(1f, 0.5f, 0.4f), new Vector2(0f, 235f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            if (!m.adminLogged)
            {
                UIFactory.Label(rt, "TI", "Zaloguj sie:", 15, Color.white, new Vector2(0f, 195f), new Vector2(330f, 30f), TextAnchor.MiddleCenter);
                var l = UIFactory.Input(rt, "I_Login", "Login", new Color(0.2f, 0.2f, 0.26f), new Vector2(0f, 145f), new Vector2(280f, 42f));
                var p2 = UIFactory.Input(rt, "I_Pass", "Haslo", new Color(0.2f, 0.2f, 0.26f), new Vector2(0f, 90f), new Vector2(280f, 42f));
                p2.inputType = InputField.InputType.Password;
                UIFactory.Btn(rt, "B_Log", "ZALOGUJ", new Color(0.8f, 0.25f, 0.25f), new Vector2(0f, 30f), new Vector2(280f, 46f), () => { TryLogin(l, p2); }, 15);
            }
            else
            {
                UIFactory.Label(rt, "TI", "Zalogowany: " + ADMIN_LOGIN, 14, new Color(0.5f, 1f, 0.6f), new Vector2(0f, 200f), new Vector2(330f, 28f), TextAnchor.MiddleCenter);
                UIFactory.Btn(rt, "B_Coins", "+1000 COINS", new Color(0.95f, 0.7f, 0.2f), new Vector2(0f, 155f), new Vector2(270f, 38f), () => GameManager.AdminGiveCoinsSafe(1000), 14);
                UIFactory.Btn(rt, "B_Cars", "ODBLOKUJ WSZYSTKIE AUTA", new Color(0.3f, 0.7f, 0.9f), new Vector2(0f, 110f), new Vector2(270f, 38f), () => GameManager.AdminUnlockAllSafe(), 12);
                UIFactory.Label(rt, "TI2", "TELEPORT:", 14, Color.white, new Vector2(0f, 70f), new Vector2(330f, 26f), TextAnchor.MiddleCenter);
                float y = 30f;
                foreach (var c in Finder.All<CityHub>())
                {
                    string city = c.cityName;
                    UIFactory.Btn(rt, "B_TP_" + city, "-> " + city, new Color(0.4f, 0.4f, 0.5f), new Vector2(0f, y), new Vector2(270f, 32f), () => GameManager.AdminTeleportSafe(city), 12);
                    y -= 38f;
                }
                UIFactory.Btn(rt, "B_Out", "WYLOGUJ", new Color(0.5f, 0.2f, 0.2f), new Vector2(0f, -195f), new Vector2(270f, 36f), () => { GameManager.AdminLogoutSafe(); AppAdmin(); }, 13);
            }
        }

        void TryLogin(InputField l, InputField p)
        {
            if (l.text == ADMIN_LOGIN && p.text == ADMIN_PASS)
            {
                GameManager.I.adminLogged = true;
                GameManager.I.Save();
                GameManager.I.Notify("ADMIN: zalogowano!");
                AppAdmin();
            }
            else
            {
                GameManager.I.Notify("ADMIN: zle haslo!");
            }
        }
    }
}
