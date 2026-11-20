using System.Collections.Generic;
using UnityEngine;

namespace Miasta
{
    public class Paczkomat : MonoBehaviour
    {
        public string cityName = "Miasto";

        public class Slot
        {
            public Transform door;
            public Vector3 closedPos;
            public GameObject pkg;
            public string code = "";
            public string item = "";
            public bool open;
            public float closeAt = 0f;
        }

        public List<Slot> slots = new List<Slot>();

        void Start()
        {
            int i = 0;
            while (true)
            {
                var d = transform.Find("Drzwi_Skrytki_" + i);
                if (d == null) break;
                var s = new Slot();
                s.door = d;
                s.closedPos = d.localPosition;
                s.pkg = transform.Find("Paczka_Skrytki_" + i) != null ? transform.Find("Paczka_Skrytki_" + i).gameObject : null;
                if (s.pkg != null) s.pkg.SetActive(false);
                slots.Add(s);
                i++;
            }
        }

        void Update()
        {
            float now = Time.time;
            foreach (var s in slots)
            {
                bool wantOpen = s.open;
                if (!s.open && s.closeAt > 0f && now < s.closeAt) wantOpen = true;
                Vector3 target = s.closedPos + (wantOpen ? Vector3.forward * 0.55f : Vector3.zero);
                s.door.localPosition = Vector3.Lerp(s.door.localPosition, target, Time.deltaTime * 6f);
            }
        }

        Slot FreeSlot()
        {
            foreach (var s in slots) if (s.code == "") return s;
            return slots.Count > 0 ? slots[0] : null;
        }

        public void InsertPackage(string item, string code)
        {
            var s = FreeSlot();
            if (s == null) return;
            s.code = code;
            s.item = item;
            s.pkg.SetActive(true);
            s.open = true;
            s.closeAt = Time.time + 6f;
        }

        public Slot FindByCode(string code)
        {
            foreach (var s in slots) if (s.code != "" && s.code == code) return s;
            return null;
        }

        public bool TryByCode(string code)
        {
            var s = FindByCode(code);
            if (s == null) return false;
            s.open = true;
            s.closeAt = 999999f;
            return true;
        }

        public void Collect(Slot s)
        {
            s.code = "";
            s.item = "";
            s.pkg.SetActive(false);
            s.open = false;
            s.closeAt = 0f;
        }

        public int OccupiedCount()
        {
            int n = 0;
            foreach (var s in slots) if (s.code != "") n++;
            return n;
        }
    }
}
