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

            appGO = UIFactory.Panel(screen.transform, "Aplikacja_Root", new Color(0.12f, 0.16f, 0.2f, 0.98f), Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -54f));
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
            if (which == "paczki") AppPaczkiRoot();
            else if (which == "sklep") AppSklep();
            else if (which == "mapa") AppMapa();
            else if (which == "admin") AppAdmin();
            AddAppCloseBtn();
        }

        void AddAppCloseBtn()
        {
            var x = UIFactory.Btn(contentT, "X_App", "X", new Color(0.5f, 0.2f, 0.2f), Vector2.zero, new Vector2(28f, 28f), () => OpenApp("menu"), 14);
            var xrt = x.GetComponent<RectTransform>();
            xrt.anchorMin = new Vector2(1f, 1f);
            xrt.anchorMax = new Vector2(1f, 1f);
            xrt.pivot = new Vector2(1f, 1f);
            xrt.anchoredPosition = new Vector2(-6f, -6f);
        }

        void AppMenu()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "TELEFON - APLIKACJE", 19, Color.white, new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            MakeTile(rt, 95f, "PACZKI", new Color(0.95f, 0.6f, 0.1f), "paczki");
            MakeTile(rt, 175f, "SKLEP", new Color(0.2f, 0.7f, 0.4f), "sklep");
            MakeTile(rt, 255f, "MAPA", new Color(0.3f, 0.5f, 0.9f), "mapa");
            MakeTile(rt, 335f, "ADMIN", new Color(0.8f, 0.25f, 0.25f), "admin");
        }

        void MakeTile(Transform rt, float y, string label, Color c, string app)
        {
            UIFactory.Btn(rt, "Tile_" + app, label, c, new Vector2(20f, -y), new Vector2(270f, 60f), () => OpenApp(app), 18);
        }

        // ============ PACZKI ============

        void AppPaczkiRoot()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "APLIKACJA PACZKI", 20, Color.white, new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            UIFactory.Btn(rt, "B_Kurier", "JESTEM KURIEREM", new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, -85f), new Vector2(300f, 54f), () => AppPaczkiKurier(), 18);
            UIFactory.Btn(rt, "B_Klient", "JESTEM KLIENTEM", new Color(0.2f, 0.7f, 0.4f), new Vector2(0f, -155f), new Vector2(300f, 54f), () => AppPaczkiKlient(), 18);
        }

        void AppPaczkiKurier()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "KURIER - zlecenia", 20, Color.white, new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            var m = GameManager.I;
            var jobT = UIFactory.Label(rt, "Zlecenie", "", 15, new Color(0.95f, 0.8f, 0.4f), new Vector2(0f, -85f), new Vector2(330f, 130f), TextAnchor.UpperLeft);
            if (m.job == null)
            {
                jobT.text = "Nie masz zlecenia.";
                UIFactory.Btn(rt, "B_Wez", "WEZ ZLECENIE", new Color(0.95f, 0.6f, 0.1f), new Vector2(0f, -155f), new Vector2(300f, 46f), () => { GameManager.GiveJobSafe(); AppPaczkiKurier(); }, 16);
                var npt = UIFactory.Label(rt, "NaglowekNPC", "ZAMOWIENIA NPC:", 15, new Color(0.5f, 0.9f, 0.7f), new Vector2(0f, -200f), new Vector2(330f, 30f), TextAnchor.MiddleCenter);
                float ny = 232f;
                if (m.npcOrders.Count == 0) npt.text = "ZAMOWIENIA NPC: (brak - czekaj, NPC sam zamawia)";
                int shown = 0;
                foreach (var n in m.npcOrders)
                {
                    if (n == null || shown >= 4) continue;
                    shown++;
                    var cc = n;
                    UIFactory.Btn(rt, "B_NPC_" + shown, n.npcName + ": " + n.wantsItem, new Color(0.2f, 0.7f, 0.4f), new Vector2(0f, -ny), new Vector2(300f, 36f), () => { GameManager.I.GiveNpcJob(cc); AppPaczkiKurier(); }, 13);
                    ny += 44f;
                }
                UIFactory.Btn(rt, "B_Wstecz", "< WSTECZ", new Color(0.3f, 0.3f, 0.34f), new Vector2(0f, -520f), new Vector2(300f, 40f), () => AppPaczkiRoot(), 14);
                return;
            }
            string state;
            if (m.job.npcName != null)
            {
                if (m.job.loaded) state = "Paczka w posiadaniu. Dowiez ja do NPC: " + m.job.npcName + " (w " + m.job.toCity + ") i wcisnij E przy nim.";
                else state = "1. Jedz do MAGAZYNU " + m.job.fromCity + ". 2. E przy bramie -> wez paczke. 3. Dowiez do NPC " + m.job.npcName + " w " + m.job.toCity + ".";
            }
            else if (m.job.loaded) state = "Paczka w posiadaniu. Dojedz do PACZKOMATU " + m.job.toCity + " i wcisnij E przy maszynie.";
            else if (m.job.toCity != null) state = "1. Jedz do MAGAZYNU w " + m.job.fromCity + ". 2. Wcisnij E przy bramie -> wez paczke. 3. Jedz do paczkomatu " + m.job.toCity + ".";
            else state = "Zadanie zakonczone.";
            jobT.text = "PACZKA: " + m.job.item +
                "\nOD: magazyn " + m.job.fromCity +
                "\nDO: paczkomat " + m.job.toCity +
                "\nZAPLATA: " + m.job.payout + " coins" +
                "\n\n" + state;
            if (!m.job.loaded)
                UIFactory.Btn(rt, "B_Porzuc", "PORZUC ZLECENIE", new Color(0.7f, 0.25f, 0.25f), new Vector2(0f, -215f), new Vector2(300f, 40f), () => { GameManager.AbandonJobSafe(); AppPaczkiKurier(); }, 14);
            {
                var info = UIFactory.Label(rt, "Paczkomaty", "", 15, Color.white, new Vector2(0f, -320f), new Vector2(330f, 340f), TextAnchor.UpperLeft);
                string s = "PACZKOMATY (droga):\n";
                foreach (var c in Finder.All<CityHub>())
                {
                    float d = PlayerDriver.I != null ? Vector3.Distance(CityUtils.CityPos(c.cityName), PlayerDriver.I.transform.position) : 0f;
                    string mark = c.cityName == m.job.toCity ? "  <<<<< CEL" : "";
                    s += c.cityName + ": " + Mathf.RoundToInt(d) + " m" + mark + "\n";
                }
                info.text = s;
            }
            UIFactory.Btn(rt, "B_Wstecz", "< WSTECZ", new Color(0.3f, 0.3f, 0.34f), new Vector2(0f, -520f), new Vector2(300f, 40f), () => AppPaczkiRoot(), 14);
        }

        void AppPaczkiKlient()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "KLIENT - zamow paczke", 20, Color.white, new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            RectTransform list;
            UIFactory.ScrollList(rt, "Lista", new Color(0.1f, 0.14f, 0.18f, 0.8f), new Vector2(0f, -175f), new Vector2(315f, 240f), out list);
            foreach (var it in Catalogs.ShopItems)
            {
                string item = it[0];
                int price = int.Parse(it[1]);
                var row = UIFactory.Panel(list, "R_" + item, new Color(0.16f, 0.22f, 0.28f), Vector2.zero, new Vector2(0f, 40f));
                UIFactory.Label(row.transform, "TT", item + " - " + price + " coins", 14, Color.white, new Vector2(-70f, 0f), new Vector2(160f, 40f), TextAnchor.MiddleLeft);
                var b = UIFactory.Btn(row.transform, "BT", "ZAMOW", new Color(0.2f, 0.7f, 0.4f), new Vector2(115f, 0f), new Vector2(90f, 32f), () => { OrderFromPhone(item, price); }, 12);
            }
            var mine = UIFactory.Label(rt, "Moje", "", 14, new Color(0.5f, 0.9f, 0.7f), new Vector2(0f, -380f), new Vector2(330f, 340f), TextAnchor.UpperLeft);
            string s = "MOJE PRZESYLKI:\n";
            foreach (var p in GameManager.I.packages)
            {
                string st = p.state == "W_Skrytce" ? "W PACZKOMACIE " + p.toCity + "! KOD: " + p.code
                    : p.state == "Zamowiona" ? "kurier jedzie..." 
                    : p.state == "W_Magazynie" ? "w magazynie " + p.toCity
                    : p.state == "W_Skrytce" ? "" 
                    : p.state == "Na_Pojazdzie" ? "w drodze do paczkomatu " + p.toCity
                    : "odebrana";
                s += "*" + p.item + ": " + st + "\n";
            }
            mine.text = s;
            UIFactory.Btn(rt, "B_Wstecz", "< WSTECZ", new Color(0.3f, 0.3f, 0.34f), new Vector2(0f, -545f), new Vector2(300f, 36f), () => AppPaczkiRoot(), 13);
        }

        void OrderFromPhone(string item, int price)
        {
            string myCity = "Krasnogrod";
            PlayerDriver pd = PlayerDriver.I;
            if (pd != null)
            {
                CityHub nc = PlayerDriver.NearestCityInfo();
                if (nc != null) myCity = nc.cityName;
            }
            GameManager.I.PlaceClientOrder(item, price, myCity);
            AppPaczkiKlient();
        }

        // ============ SKLEP ============

        void AppSklep()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "SKLEP (coins)", 20, Color.white, new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            int y = 90;
            foreach (var it in Catalogs.PhoneShop)
            {
                int price = int.Parse(it[1]);
                string tag = it[2];
                var row = UIFactory.Panel(rt, "R_" + tag, new Color(0.16f, 0.22f, 0.28f), new Vector2(0f, -y), new Vector2(330f, 70f));
                UIFactory.Label(row.transform, "TT", it[0] + "\n" + it[1] + " coins", 15, Color.white, new Vector2(-70f, 0f), new Vector2(170f, 60f), TextAnchor.MiddleLeft);
                UIFactory.Btn(row.transform, "BT", "KUP", new Color(0.95f, 0.6f, 0.1f), new Vector2(125f, 0f), new Vector2(80f, 40f), () => BuyUpgrade(tag, price), 15);
                y += 80;
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
            else if (tag == "lakier") { m.goldPaint = "tak"; m.Notify("Zloty lakier kupiony! Twoje nowe auta beda zlotowe."); }
            m.Notify("Kupiono za " + price + " coins. Zostalo: " + m.coins);
        }

        // ============ MAPA ============

        void AppMapa()
        {
            var rt = contentT;
            UIFactory.Label(rt, "Tytul", "MAPA - Wszystkie miasta", 18, Color.white, new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            RectTransform list;
            UIFactory.ScrollList(rt, "Lista", new Color(0.1f, 0.14f, 0.18f, 0.8f), new Vector2(0f, -270f), new Vector2(315f, 440f), out list);
            PlayerDriver pd = PlayerDriver.I;
            Vector3 pp = Vector3.zero;
            if (pd != null) pp = pd.transform.position;
            foreach (var c in Finder.All<CityHub>())
            {
                string city = c.cityName;
                float d = Vector3.Distance(CityUtils.CityPos(city), pp);
                var row = UIFactory.Panel(list, "R_" + city, new Color(0.16f, 0.22f, 0.28f), Vector2.zero, new Vector2(0f, 50f));
                UIFactory.Label(row.transform, "TT", city + "\ndroga ~" + Mathf.RoundToInt(d) + " m | paczkomat, salon, magazyn", 13, Color.white, new Vector2(-10f, 0f), new Vector2(230f, 50f), TextAnchor.MiddleLeft);
                bool ziel = c.hasPaczkomat;
                UIFactory.Label(row.transform, "ST", ziel ? "OK" : "-", 14, ziel ? new Color(0.3f, 1f, 0.5f) : new Color(0.8f, 0.3f, 0.3f), new Vector2(125f, 0f), new Vector2(60f, 50f), TextAnchor.MiddleCenter);
            }
            UIFactory.Label(rt, "Podpowiedzi", "Musisz dojechac do miast droga!\nW kazdym miescie: paczkomat, salon, magazyn.", 13, new Color(0.6f, 0.85f, 1f), new Vector2(0f, -530f), new Vector2(330f, 60f), TextAnchor.UpperCenter);
        }

        // ============ ADMIN ============

        void AppAdmin()
        {
            var rt = contentT;
            var m = GameManager.I;
            UIFactory.Label(rt, "Tytul", "ADMIN PANEL", 20, new Color(1f, 0.5f, 0.4f), new Vector2(0f, -30f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
            if (!m.adminLogged)
            {
                UIFactory.Label(rt, "TI", "Zaloguj sie:", 16, Color.white, new Vector2(0f, -80f), new Vector2(330f, 40f), TextAnchor.MiddleCenter);
                var l = UIFactory.Input(rt, "I_Login", "Login", new Color(0.2f, 0.2f, 0.26f), new Vector2(0f, -130f), new Vector2(280f, 44f));
                var p = UIFactory.Input(rt, "I_Pass", "Haslo", new Color(0.2f, 0.2f, 0.26f), new Vector2(0f, -185f), new Vector2(280f, 44f));
                p.inputType = InputField.InputType.Password;
                UIFactory.Btn(rt, "B_Log", "ZALOGUJ", new Color(0.8f, 0.25f, 0.25f), new Vector2(0f, -245f), new Vector2(280f, 46f), () => { TryLogin(l, p); }, 16);
            }
            else
            {
                UIFactory.Label(rt, "TI", "Zalogowany jako: " + ADMIN_LOGIN, 15, new Color(0.5f, 1f, 0.6f), new Vector2(0f, -75f), new Vector2(330f, 30f), TextAnchor.MiddleCenter);
                UIFactory.Btn(rt, "B_Coins", "+1000 COINS", new Color(0.95f, 0.7f, 0.2f), new Vector2(0f, -120f), new Vector2(270f, 40f), () => GameManager.AdminGiveCoinsSafe(1000), 15);
                UIFactory.Btn(rt, "B_Cars", "ODBILOKUJ WSZYSTKIE AUTA", new Color(0.3f, 0.7f, 0.9f), new Vector2(0f, -170f), new Vector2(270f, 40f), () => GameManager.AdminUnlockAllSafe(), 13);
                var ct = UIFactory.Label(rt, "TI2", "TELEPORT:", 15, Color.white, new Vector2(0f, -215f), new Vector2(330f, 30f), TextAnchor.MiddleCenter);
                int y = 255;
                foreach (var c in Finder.All<CityHub>())
                {
                    string city = c.cityName;
                    UIFactory.Btn(rt, "B_TP_" + city, "-> " + city, new Color(0.4f, 0.4f, 0.5f), new Vector2(0f, -y), new Vector2(270f, 34f), () => GameManager.AdminTeleportSafe(city), 13);
                    y += 40;
                }
                UIFactory.Btn(rt, "B_Out", "WYLOGUJ", new Color(0.5f, 0.2f, 0.2f), new Vector2(0f, -y), new Vector2(270f, 36f), () => { GameManager.AdminLogoutSafe(); AppAdmin(); }, 14);
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
