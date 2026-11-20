using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Miasta
{
    public class Catalogs
    {
        public class CarInfo
        {
            public string id;
            public int price;
            public float maxSpeed;
            public Color color;
            public string desc;
        }

        public static CarInfo[] Cars = new CarInfo[]
        {
            new CarInfo { id = "Kasztanka", price = 0, maxSpeed = 26f, color = new Color(0.85f, 0.34f, 0.16f), desc = "Zwykly woz za grosze" },
            new CarInfo { id = "Solanka", price = 500, maxSpeed = 33f, color = new Color(0.18f, 0.5f, 0.88f), desc = "Rodzinny kompresor" },
            new CarInfo { id = "Dostawczak", price = 800, maxSpeed = 30f, color = new Color(0.95f, 0.95f, 0.95f), desc = "Duzy, idealny na paczki" },
            new CarInfo { id = "SlonecznyKabrio", price = 1200, maxSpeed = 40f, color = new Color(0.98f, 0.78f, 0.2f), desc = "Sloneczna limuzyna" },
            new CarInfo { id = "Krasnorower", price = 2000, maxSpeed = 48f, color = new Color(0.2f, 0.2f, 0.2f), desc = "Czarny samochod-sportowiec" }
        };

        public static string[][] ShopItems = new string[][]
        {
            new string[] { "Laptop", "400" },
            new string[] { "Smartfon", "250" },
            new string[] { "Glosnik Bluetooth", "180" },
            new string[] { "Zabawny Kapelusz", "60" },
            new string[] { "Dron", "320" }
        };

        public static string[][] PhoneShop = new string[][]
        {
            new string[] { "Turbo silnik", "500", "turbo" },
            new string[] { "Stawka kuriera +50%", "800", "kurier" },
            new string[] { "Zloty lakier", "300", "lakier" }
        };
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager I;

        public int coins = 250;
        public bool adminLogged = false;
        public List<string> ownedCars = new List<string>();
        public float turboBonus = 0f;
        public float courierBonus = 0f;
        public string goldPaint = "nie";

        public delegate void NotifyHandler(string msg);
        public event NotifyHandler OnNotify;

        public class Package
        {
            public int id;
            public string item;
            public string toCity;
            public string code;
            public string state;
        }

        public class CourierJob
        {
            public int pkgId;
            public string item;
            public string fromCity;
            public string toCity;
            public int payout;
            public bool loaded;
        }

        public List<Package> packages = new List<Package>();
        public CourierJob job;
        int nextId = 1001;

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            coins = PlayerPrefs.GetInt("miasta_coins", 250);
            adminLogged = PlayerPrefs.GetInt("miasta_admin", 0) == 1;
            string oc = PlayerPrefs.GetString("miasta_cars", "Kasztanka");
            ownedCars = new List<string>(oc.Split(','));
            turboBonus = PlayerPrefs.GetFloat("miasta_turbo", 0f);
            courierBonus = PlayerPrefs.GetFloat("miasta_kurier", 0f);
            goldPaint = PlayerPrefs.GetString("miasta_lakier", "nie");
            if (!ownedCars.Contains("Kasztanka")) ownedCars.Add("Kasztanka");
        }

        void Start()
        {
            StartCoroutine(CleanupLoop());
        }

        IEnumerator CleanupLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(30f);
                packages.RemoveAll(p => p.state == "Odebrana");
            }
        }

        public void Save()
        {
            PlayerPrefs.SetInt("miasta_coins", coins);
            PlayerPrefs.SetInt("miasta_admin", adminLogged ? 1 : 0);
            PlayerPrefs.SetString("miasta_cars", string.Join(",", ownedCars.ToArray()));
            PlayerPrefs.SetFloat("miasta_turbo", turboBonus);
            PlayerPrefs.SetFloat("miasta_kurier", courierBonus);
            PlayerPrefs.SetString("miasta_lakier", goldPaint);
            PlayerPrefs.Save();
        }

        public void Notify(string msg)
        {
            if (OnNotify != null) OnNotify(msg);
        }

        public void AddCoins(int n)
        {
            coins += n;
            Save();
            Notify(n >= 0 ? "+ " + n + " COINS" : n + " COINS");
        }

        public Catalogs.CarInfo GetCar(string id)
        {
            foreach (var c in Catalogs.Cars) if (c.id == id) return c;
            return null;
        }

        public Catalogs.CarInfo[] AvailableCars()
        {
            var l = new List<Catalogs.CarInfo>();
            foreach (var c in Catalogs.Cars)
                if (ownedCars.Contains(c.id) || c.price == 0) l.Add(c);
            return l.ToArray();
        }

        // ================== KLIENT ==================

        public Package PlaceClientOrder(string item, int price, string myCity)
        {
            if (coins < price) { Notify("Masz za malo COINS! (" + coins + "/" + price + ")"); return null; }
            coins -= price;
            Save();
            string from = CityUtils.PickOtherCity(I, myCity);
            var p = new Package();
            p.id = nextId++;
            p.item = item;
            p.toCity = myCity;
            p.code = Random.Range(100000, 999999).ToString();
            p.state = "Zamowiona";
            packages.Add(p);
            Notify("Zamowienie przyjete: " + item + " -> paczkomat " + myCity + ". Kurier jedzie z " + from);
            StartCoroutine(AutoCourier(p, from));
            return p;
        }

        IEnumerator AutoCourier(Package p, string fromCity)
        {
            yield return new WaitForSeconds(Random.Range(20f, 35f));
            if (p.state != "Zamowiona") yield break;
            p.state = "W_Skrytce";
            var pk = FindPaczkomat(p.toCity);
            if (pk != null) pk.InsertPackage(p.item, p.code);
            Notify("PACZKA DOTARLA do paczkomatu " + p.toCity + "! Kod: " + p.code);
        }

        public Paczkomat FindPaczkomat(string city)
        {
            foreach (var pk in Object.FindObjectsOfType<Paczkomat>())
                if (pk.cityName == city) return pk;
            return null;
        }

        // ================== KURIER ==================

        public void GiveCourierJob()
        {
            if (job != null) { Notify("Masz juz zlecenie!"); return; }
            var hubs = Object.FindObjectsOfType<CityHub>();
            if (hubs == null || hubs.Length == 0) return;
            CityHub nearest = null;
            PlayerDriver pd = PlayerDriver.I;
            Vector3 pp = pd != null ? pd.transform.position : Vector3.zero;
            foreach (var h in hubs)
            {
                if (nearest == null) nearest = h;
                else if (Vector3.Distance(h.transform.position, pp) < Vector3.Distance(nearest.transform.position, pp)) nearest = h;
            }
            string from = nearest != null ? nearest.cityName : "Krasnogrod";
            string to = CityUtils.PickOtherCity(I, from);
            var p = new Package();
            p.id = nextId++;
            p.item = Catalogs.ShopItems[Random.Range(0, Catalogs.ShopItems.Length)][0];
            p.toCity = to;
            p.code = Random.Range(100000, 999999).ToString();
            p.state = "W_Magazynie";
            packages.Add(p);
            job = new CourierJob();
            job.pkgId = p.id;
            job.item = p.item;
            job.fromCity = from;
            job.toCity = to;
            job.payout = Mathf.RoundToInt((140f + Vector3.Distance(CityUtils.CityPos(from), CityUtils.CityPos(to)) / 10f) * (1f + courierBonus));
            Notify("Nowe zlecenie: " + p.item + " z MAGAZYNU " + from + " do PACZKOMATU " + to + ". Zaplata: " + job.payout + " coins");
        }

        public Package GetPackage(int id)
        {
            foreach (var p in packages) if (p.id == id) return p;
            return null;
        }

        public void AbandonJob()
        {
            if (job == null) return;
            var p = GetPackage(job.pkgId);
            if (p != null && p.state == "Na_Pojazdzie") p.state = "W_Magazynie";
            PlayerDriver.I.SetCarrying(false);
            job = null;
            Notify("Porzuciles zlecenie. Wez nowe w aplikacji PACZKI -> KURIER.");
        }

        public void DeliverCurrentJobTo(Paczkomat machine)
        {
            if (job == null || machine == null) return;
            if (!job.loaded) { Notify("Nie masz paczki przy sobie! Najpierw magazyn " + job.fromCity); return; }
            if (machine.cityName != job.toCity) { Notify("To zly paczkomat! Paczka jedzie do " + job.toCity); return; }
            var p = GetPackage(job.pkgId);
            machine.InsertPackage(job.item, p != null ? p.code : "000000");
            if (p != null) p.state = "W_Skrytce";
            AddCoins(job.payout);
            SetCarrying(false);
            Notify("Dostarczone! +" + job.payout + " coins. Kod klienta: " + (p != null ? p.code : ""));
            job = null;
        }

        public static void GiveJobSafe()
        {
            if (I != null) I.GiveCourierJob();
            if (PhoneController.I != null) PhoneController.I.Rebuild();
        }

        public static void AbandonJobSafe()
        {
            if (I != null) I.AbandonJob();
            if (PhoneController.I != null) PhoneController.I.Rebuild();
        }

        public static void AdminGiveCoinsSafe(int n)
        {
            if (I != null) I.AddCoins(n);
        }

        public static void AdminUnlockAllSafe()
        {
            if (I == null) return;
            foreach (var c in Catalogs.Cars)
                if (!I.ownedCars.Contains(c.id)) I.ownedCars.Add(c.id);
            I.Save();
            I.Notify("ADMIN: wszystkie auta odblokowane!");
        }

        public static void AdminTeleportSafe(string city)
        {
            if (PlayerDriver.I == null) return;
            PlayerDriver.I.transform.position = CityUtils.CityPos(city) + Vector3.up * 2f;
            I.Notify("ADMIN: teleport do " + city);
        }

        public static void AdminLogoutSafe()
        {
            if (I == null) return;
            I.adminLogged = false;
            I.Save();
            I.Notify("ADMIN: wylogowano.");
        }
    }

    public static class CityUtils
    {
        public static string PickOtherCity(object mgr, string notThis)
        {
            var cities = Object.FindObjectsOfType<CityHub>();
            var ok = new List<string>();
            foreach (var c in cities) if (c.cityName != notThis) ok.Add(c.cityName);
            if (ok.Count == 0) return notThis;
            return ok[Random.Range(0, ok.Count)];
        }

        public static Vector3 CityPos(string cityName)
        {
            foreach (var c in Object.FindObjectsOfType<CityHub>())
                if (c.cityName == cityName) return c.transform.position;
            return Vector3.zero;
        }
    }
}
