using System.Collections.Generic;
using UnityEngine;

namespace Miasta
{
    public static class WorldBuilder
    {
        public class CityDef
        {
            public string id;
            public string display;
            public Vector3 pos;
            public bool big;
        }

        static readonly CityDef[] defs = new CityDef[]
        {
            new CityDef { id = "Kasztanowice", display = "KASZTANOWICE", pos = new Vector3(-1600f, 0f, -1600f) },
            new CityDef { id = "Sloneczny Brzeg", display = "S\u0141ONECZNY BRZEG", pos = new Vector3(1600f, 0f, -1600f), big = true },
            new CityDef { id = "Solina", display = "SOLINA", pos = new Vector3(1600f, 0f, 1600f) },
            new CityDef { id = "Bialowoda", display = "BIA\u0141OWODA", pos = new Vector3(-1600f, 0f, 1600f) },
            new CityDef { id = "Krasnogrod", display = "KRASNOGR\u00D3D", pos = Vector3.zero }
        };

        static Material grass, asphalt, sidewalk, white, dark, yellow, sand, water, glass, orange, metal, brickA, brickB, brickC, brickD, skin;

        public static void Build(bool rebuild = false)
        {
            var old = GameObject.Find("Polskie_Miasta");
            if (old != null && !rebuild) return;
            if (old != null) Object.DestroyImmediate(old);
            var oldPh = GameObject.Find("Telefono_Canvas");
            if (oldPh != null) Object.DestroyImmediate(oldPh);

            MakeMaterials();

            var world = new GameObject("Polskie_Miasta");
            Ground(world.transform);
            Roads(world.transform);
            foreach (var d in defs) City(world.transform, d);
            GameManagerAndPlayer(world.transform);

            Debug.Log("GOTOWE: 5 miast zbudowane! Zapisz scene Ctrl+S, potem wcisnij PLAY. Telefon: klawisz H.");
        }

        static void MakeMaterials()
        {
            grass = Mat(new Color(0.3f, 0.5f, 0.2f), 0f, 0.15f);
            asphalt = Mat(new Color(0.2f, 0.21f, 0.23f), 0f, 0.15f);
            sidewalk = Mat(new Color(0.66f, 0.67f, 0.68f), 0f, 0.2f);
            white = Mat(new Color(0.94f, 0.94f, 0.94f), 0.1f, 0.6f);
            dark = Mat(new Color(0.1f, 0.11f, 0.13f), 0f, 0.3f);
            yellow = Mat(new Color(0.97f, 0.78f, 0.05f), 0f, 0.5f);
            sand = Mat(new Color(0.85f, 0.76f, 0.5f), 0f, 0.2f);
            water = Mat(new Color(0.2f, 0.5f, 0.8f, 0.8f), 0.2f, 0.8f);
            glass = GlassMat(new Color(0.55f, 0.75f, 0.85f), 0.55f);
            orange = Mat(new Color(0.95f, 0.6f, 0.1f), 0f, 0.5f);
            metal = Mat(new Color(0.6f, 0.63f, 0.66f), 0.7f, 0.6f);
            brickA = Mat(new Color(0.85f, 0.6f, 0.4f), 0f, 0.3f);
            brickB = Mat(new Color(0.7f, 0.8f, 0.85f), 0f, 0.3f);
            brickC = Mat(new Color(0.95f, 0.9f, 0.7f), 0f, 0.3f);
            brickD = Mat(new Color(0.6f, 0.7f, 0.5f), 0f, 0.3f);
            skin = Mat(new Color(0.9f, 0.75f, 0.6f), 0f, 0.3f);
        }

        static Material Mat(Color c, float metal, float smooth)
        {
            var m = new Material(LitShader.Get());
            m.color = c;
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
            if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smooth);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
            return m;
        }

        static Material GlassMat(Color c, float alpha)
        {
            var m = Mat(new Color(c.r, c.g, c.b, alpha), 0.6f, 0.8f);
            if (m.HasProperty("_Mode"))
            {
                m.SetFloat("_Mode", 3f);
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0);
                m.EnableKeyword("_ALPHABLEND_ON");
                m.renderQueue = 3000;
            }
            else if (m.HasProperty("_Surface"))
            {
                m.SetFloat("_Surface", 1f);
                if (m.HasProperty("_Blend")) m.SetFloat("_Blend", 0f);
                m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.EnableKeyword("_ALPHAPREMULTIPLY_OFF");
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0);
                if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.8f);
                m.renderQueue = 3000;
            }
            return m;
        }

        static GameObject Box(Transform parent, string name, Vector3 pos, Vector3 size, Material mat, Vector3 rot)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.localScale = size;
            go.transform.eulerAngles = rot;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        static GameObject Cyl(Transform parent, string name, Vector3 pos, Vector3 size, Material mat, Vector3 rot)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.localScale = size;
            go.transform.eulerAngles = rot;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        static GameObject Ball(Transform parent, string name, Vector3 pos, float size, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * size;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }

        static GameObject Text(Transform parent, string name, string text, Vector3 pos, float charSize, Color color, Vector3 rot, int size = 36)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.eulerAngles = rot;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = charSize;
            tm.fontSize = size;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color;
            Font f = null;
            try { f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
            if (f == null) { try { f = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { } }
            if (f != null) tm.font = f;
            var r = go.GetComponent<MeshRenderer>();
            r.sharedMaterial = new Material(Shader.Find("GUI/Text Shader"));
            r.sharedMaterial.color = color;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        static void Ground(Transform p)
        {
            Box(p, "Pola_i_Laki", new Vector3(0f, -0.15f, 0f), new Vector3(7000f, 0.3f, 7000f), grass, Vector3.zero);
            Box(p, "Morze_Sloneczny_Brzeg", new Vector3(1600f, 0.02f, -2450f), new Vector3(3600f, 0.04f, 700f), water, Vector3.zero);
            var l = new GameObject("Slonce");
            var dir = l.AddComponent<Light>();
            dir.type = LightType.Directional;
            dir.intensity = 1.15f;
            l.transform.rotation = Quaternion.Euler(48f, 320f, 0f);
        }

        static void Road(Transform p, string name, Vector3 a, Vector3 b, float width, Material mat)
        {
            Vector3 dir = b - a;
            dir.y = 0f;
            float len = dir.magnitude;
            if (len < 1f) return;
            Vector3 mid = (a + b) * 0.5f;
            mid.y = 0.08f;
            float ang = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            var slab = Box(p, name, mid, new Vector3(width, 0.16f, len), mat, new Vector3(0f, ang, 0f));
            for (int i = 1; i < len / 60f; i++)
            {
                Vector3 mp = a + dir * (0.06f + i / (len / 60f) * 0.88f);
                mp.y = 0.17f;
                Box(p, "Kreska_" + name + "_" + (i % 7), mp, new Vector3(0.25f, 0.02f, 5f), mat == sidewalk ? yellow : white, new Vector3(0f, ang, 0f));
            }
        }

        static void Roads(Transform p)
        {
            foreach (var d in defs)
            {
                if (d.pos == Vector3.zero) continue;
                Road(p, "Trasa_Centro_" + d.id, Vector3.zero, d.pos, 16f, asphalt);
            }
            Road(p, "Obwodnica_N", new Vector3(-1600f, 0f, -1600f), new Vector3(1600f, 0f, -1600f), 16f, asphalt);
            Road(p, "Obwodnica_S", new Vector3(-1600f, 0f, 1600f), new Vector3(1600f, 0f, 1600f), 16f, asphalt);
            Road(p, "Obwodnica_W", new Vector3(-1600f, 0f, -1600f), new Vector3(-1600f, 0f, 1600f), 16f, asphalt);
            Road(p, "Obwodnica_E", new Vector3(1600f, 0f, -1600f), new Vector3(1600f, 0f, 1600f), 16f, asphalt);
        }

        static GameObject Npc(Transform parent, string name, Vector3 pos, Color shirt, Color pants, Vector3 rot)
        {
            var np = new GameObject(name);
            np.transform.SetParent(parent);
            np.transform.position = pos;
            np.transform.eulerAngles = rot;
            Box(np.transform, "Nogi", new Vector3(0f, 0.9f, 0f), new Vector3(0.5f, 1.1f, 0.4f), Mat(pants, 0f, 0.3f), Vector3.zero);
            Box(np.transform, "Korpus", new Vector3(0f, 1.75f, 0f), new Vector3(0.7f, 0.8f, 0.4f), Mat(shirt, 0f, 0.3f), Vector3.zero);
            Ball(np.transform, "Glowa", new Vector3(0f, 2.45f, 0f), 0.42f, skin);
            var col = np.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, 1.3f, 0f);
            col.size = new Vector3(1.2f, 2.7f, 1.0f);
            return np;
        }

        static void City(Transform p, CityDef d)
        {
            var np = new GameObject("Miasto_" + d.id);
            var hub = np.AddComponent<Miasta.CityHub>();
            hub.cityName = d.id;
            hub.hasPaczkomat = true;
            np.transform.SetParent(p);
            np.transform.position = d.pos;
            var t = np.transform;
            float half = d.big ? 450f : 250f;
            var rng = new System.Random(d.id.GetHashCode());

            Box(t, "Pad_Gruntu", new Vector3(0f, 0.01f, 0f), new Vector3(half * 2f + 60f, 0.34f, half * 2f + 60f), grass, Vector3.zero);
            Box(t, "Droga_GX", new Vector3(0f, 0.16f, 0f), new Vector3(half * 2f, 0.34f, 12f), asphalt, Vector3.zero);
            Box(t, "Droga_GZ", new Vector3(0f, 0.16f, 0f), new Vector3(12f, 0.34f, half * 2f), asphalt, Vector3.zero);
            float rr = half * 0.62f;
            Box(t, "Obwodnica_N", new Vector3(0f, 0.16f, -rr), new Vector3(half * 1.26f, 0.34f, 9f), asphalt, Vector3.zero);
            Box(t, "Obwodnica_S", new Vector3(0f, 0.16f, rr), new Vector3(half * 1.26f, 0.34f, 9f), asphalt, Vector3.zero);
            Box(t, "Obwodnica_W", new Vector3(-rr, 0.16f, 0f), new Vector3(9f, 0.34f, half * 1.26f), asphalt, Vector3.zero);
            Box(t, "Obwodnica_E", new Vector3(rr, 0.16f, 0f), new Vector3(9f, 0.34f, half * 1.26f), asphalt, Vector3.zero);
            foreach (var sz in new float[] { -6.5f, 6.5f })
            {
                Box(t, "ChodnikGX" + sz, new Vector3(0f, 0.17f, sz), new Vector3(half * 2f, 0.34f, 2.2f), sidewalk, Vector3.zero);
                Box(t, "ChodnikGZ" + sz, new Vector3(sz, 0.17f, 0f), new Vector3(2.2f, 0.34f, half * 2f), sidewalk, Vector3.zero);
            }

            Sign(t, d.display, new Vector3(-half - 14f, 0f, 0f), d.big ? 7f : 5f);

            if (d.big)
            {
                Downtown(t, rng);
                Beach(t, half);
            }
            else
            {
                Rynek(t);
            }

            Houses(t, rng, half, d.big);

            PaczkomatMachine(t, new Vector3(half * 0.25f, 0f, half * 0.25f), 0f, d.id);
            MagazynBudynek(t, new Vector3(-half * 0.25f, 0f, half * 0.25f), d.id);
            SalonBudynek(t, new Vector3(half * 0.25f, 0f, -half * 0.25f), d.id);

            for (int i = -3; i <= 3; i++)
            {
                if (i == 0) continue;
                Lamp(t, new Vector3(i * half * 0.25f, 0f, 8.5f));
                Lamp(t, new Vector3(8.5f, 0f, i * half * 0.25f));
            }

            var traffic = new GameObject("Auto_Ruch");
            traffic.transform.SetParent(t);
            traffic.transform.position = new Vector3(-rr + 3f, 0f, -rr);
            var mesh = Miasta.CarFactory.BuildMesh(traffic.transform, "Auto", new Color(0.5f, 0.2f, 0.2f), 4.2f, 1.9f, 1.3f);
            var tc = traffic.AddComponent<Miasta.TrafficCar>();
            tc.route = new Vector3[] { new Vector3(-rr + 3f, 0f, -rr), new Vector3(rr - 3f, 0f, -rr), new Vector3(rr - 3f, 0f, rr), new Vector3(-rr + 3f, 0f, rr) };
            tc.speed = 8f;

            var traffic2 = new GameObject("Auto_Ruch2");
            traffic2.transform.SetParent(t);
            traffic2.transform.position = new Vector3(rr - 3f, 0f, rr);
            var mesh2 = Miasta.CarFactory.BuildMesh(traffic2.transform, "Auto", new Color(0.2f, 0.4f, 0.7f), 4.2f, 1.9f, 1.3f);
            var tc2 = traffic2.AddComponent<Miasta.TrafficCar>();
            tc2.route = new Vector3[] { new Vector3(rr - 3f, 0f, rr), new Vector3(rr - 3f, 0f, -rr), new Vector3(-rr + 3f, 0f, -rr), new Vector3(-rr + 3f, 0f, rr) };
            tc2.speed = 7f;
        }

        static void Sign(Transform t, string display, Vector3 localPos, float scale)
        {
            Cyl(t, "Slupek_Sign1", localPos + new Vector3(-5f, 3f, 0f), new Vector3(0.5f, 6f, 0.5f), dark, Vector3.zero);
            Cyl(t, "Slupek_Sign2", localPos + new Vector3(5f, 3f, 0f), new Vector3(0.5f, 6f, 0.5f), dark, Vector3.zero);
            Box(t, "Tablica_Sign", localPos + new Vector3(0f, 6.5f, 0f), new Vector3(18f, 3f, 0.6f), white, new Vector3(0f, 90f, 0f));
            Text(t, "Napis_Sign", "WITAJ W " + display, localPos + new Vector3(-0.45f, 6.5f, 0f), scale * 0.25f, new Color(0.1f, 0.15f, 0.4f), new Vector3(0f, 90f, 0f), 48);
        }

        static void Rynek(Transform t)
        {
            Box(t, "Rynek_Plac", new Vector3(0f, 0.2f, 0f), new Vector3(56f, 0.34f, 56f), sidewalk, Vector3.zero);
            Cyl(t, "Fontanna_Kupa", new Vector3(0f, 0.6f, 0f), new Vector3(7f, 1.1f, 7f), sidewalk, Vector3.zero);
            Cyl(t, "Fontanna_Woda", new Vector3(0f, 1.1f, 0f), new Vector3(6.6f, 0.2f, 6.6f), water, Vector3.zero);
            Cyl(t, "Fontanna_Srodek", new Vector3(0f, 1.7f, 0f), new Vector3(0.9f, 1.4f, 0.9f), orange, Vector3.zero);
            Ball(t, "Fontanna_Kula", new Vector3(0f, 2.7f, 0f), 1.1f, orange);
        }

        static void Downtown(Transform t, System.Random rng)
        {
            var towers = new[] {
                new[] { -180f, -160f, 34f },
                new[] { -90f, -210f, 56f },
                new[] { 60f, -200f, 44f },
                new[] { 190f, -150f, 70f },
                new[] { -220f, 120f, 40f },
                new[] { 200f, 160f, 62f }
            };
            int i = 0;
            foreach (var tw in towers)
            {
                float h = tw[2];
                var w = Mat(new Color(0.5f + (float)rng.NextDouble() * 0.3f, 0.6f + (float)rng.NextDouble() * 0.3f, 0.7f), 0.2f, 0.5f);
                Box(t, "Wiezowiec_" + i, new Vector3(tw[0], h * 0.5f, tw[1]), new Vector3(34f, h, 34f), w, Vector3.zero);
                Box(t, "Wiezowiec_Roof_" + i, new Vector3(tw[0], h + 1f, tw[1]), new Vector3(35f, 2f, 35f), dark, Vector3.zero);
                Box(t, "Wiezowiec_Szyby_" + i, new Vector3(tw[0], h * 0.25f, tw[1] + 17.2f), new Vector3(28f, h * 0.5f, 0.4f), glass, Vector3.zero);
                i++;
            }
        }

        static void Beach(Transform t, float half)
        {
            float bz = -half - 30f;
            Box(t, "Plaza", new Vector3(0f, 0.18f, bz), new Vector3(half * 2f + 80f, 0.34f, 130f), sand, Vector3.zero);
            int i = 0;
            foreach (var px in new float[] { -200f, -60f, 120f, 260f })
            {
                Cyl(t, "Palma_Tul_" + i, new Vector3(px, 4f, bz - 15f + (float)(i % 2 * 25)), new Vector3(0.6f, 8f, 0.6f), brickA, new Vector3(6f, 0f, 0f));
                Ball(t, "Palma_Lisce_" + i, new Vector3(px, 8.5f, bz - 13f + (float)(i % 2 * 25)), 4.5f, greenMat());
                i++;
            }
            Box(t, "Atrapa_Bar_Plaza", new Vector3(-280f, 2f, bz + 40f), new Vector3(14f, 4f, 8f), orange, Vector3.zero);
            Text(t, "Bar_Napis", "LANCHA NA PLAZY", new Vector3(-280f, 5f, bz + 44f), 1.1f, Color.white, Vector3.zero, 36);
        }

        static Material greenMat()
        {
            return Mat(new Color(0.2f, 0.6f, 0.25f), 0f, 0.25f);
        }

        static void Houses(Transform t, System.Random rng, float half, bool big)
        {
            var bricks = new[] { brickA, brickB, brickC, brickD };
            int n = big ? 22 : 12;
            int placed = 0;
            int guard = 0;
            while (placed < n && guard < 900)
            {
                guard++;
                float rMin = rr(half) + 14f;
                float rMax = half * 0.9f;
                float rad = rMin + (float)rng.NextDouble() * (rMax - rMin);
                float angd = (float)rng.NextDouble() * 360f;
                float x = Mathf.Cos(angd * Mathf.Deg2Rad) * rad;
                float z = Mathf.Sin(angd * Mathf.Deg2Rad) * rad;
                if (Mathf.Abs(x) < 40f && Mathf.Abs(z) < 40f) continue;
                float w = 12f + (float)rng.NextDouble() * 10f;
                float dep = 10f + (float)rng.NextDouble() * 8f;
                var wall = bricks[rng.Next(0, bricks.Length)];
                float hgt = 5f + (float)rng.NextDouble() * 2.5f;
                var roofCol = rng.NextDouble() > 0.8 ? new Color(0.3f, 0.3f, 0.6f) : new Color(0.68f, 0.2f, 0.2f);
                House(t, placed, x, z, w, dep, hgt, wall, roofCol);
                placed++;
            }
        }

        static float rr(float base_) { return base_; }

        static void House(Transform t, int idx, float x, float z, float w, float dep, float hgt, Material wall, Color roofCol)
        {
            var roofM = Mat(roofCol, 0f, 0.3f);
            var plinthM = Mat(new Color(0.55f, 0.54f, 0.52f), 0f, 0.2f);
            var windowM = Mat(new Color(0.25f, 0.36f, 0.46f), 0.2f, 0.6f);
            var chimneyM = Mat(new Color(0.5f, 0.3f, 0.25f), 0f, 0.3f);
            var doorM = Mat(new Color(0.4f, 0.26f, 0.12f), 0f, 0.3f);

            Box(t, "Dom_Esplanada_" + idx, new Vector3(x, 0.25f, z), new Vector3(w + 0.8f, 0.5f, dep + 0.8f), plinthM, Vector3.zero);
            Box(t, "Dom_Sciana_" + idx, new Vector3(x, hgt * 0.5f + 0.5f, z), new Vector3(w, hgt, dep), wall, Vector3.zero);

            float a = 34f * Mathf.Deg2Rad;
            float halfDep = dep * 0.5f + 0.8f;
            float slopeLen = halfDep / Mathf.Cos(a);
            float rise = halfDep * Mathf.Tan(a);
            float midY = hgt + 0.5f + rise * 0.5f;

            Box(t, "Dom_Dach_Lewo_" + idx, new Vector3(x, midY, z - halfDep * 0.5f), new Vector3(w + 1.6f, 0.26f, slopeLen), roofM, new Vector3(-34f, 0f, 0f));
            Box(t, "Dom_Dach_Prawo_" + idx, new Vector3(x, midY, z + halfDep * 0.5f), new Vector3(w + 1.6f, 0.26f, slopeLen), roofM, new Vector3(34f, 0f, 0f));
            Box(t, "Dom_Kalenica_" + idx, new Vector3(x, hgt + 0.5f + rise, z), new Vector3(w + 1.9f, 0.34f, 0.7f), roofM, Vector3.zero);
            Box(t, "Dom_Komin_" + idx, new Vector3(x + w * 0.25f, hgt + 1.5f + rise, z + dep * 0.25f), new Vector3(0.9f, 2.2f, 0.9f), chimneyM, Vector3.zero);

            float fz = dep * 0.5f + 0.42f;
            Box(t, "Dom_Drzwi_" + idx, new Vector3(x, hgt * 0.35f + 0.5f, z + fz), new Vector3(1.5f, hgt * 0.7f, 0.16f), doorM, Vector3.zero);
            Box(t, "Dom_Okno1_" + idx, new Vector3(x - w * 0.28f, hgt * 0.55f + 0.5f, z + fz), new Vector3(1.9f, 1.5f, 0.14f), windowM, Vector3.zero);
            Box(t, "Dom_Okno2_" + idx, new Vector3(x + w * 0.28f, hgt * 0.55f + 0.5f, z + fz), new Vector3(1.9f, 1.5f, 0.14f), windowM, Vector3.zero);
            Box(t, "Dom_Esplanada_Krok1" + idx, new Vector3(x, 0.12f, z + fz + 0.8f), new Vector3(2.4f, 0.24f, 1.4f), plinthM, Vector3.zero);
        }

        static void Lamp(Transform t, Vector3 localPos)
        {
            Cyl(t, "Latarnia", localPos, new Vector3(0.3f, 6f, 0.3f), dark, Vector3.zero);
            Ball(t, "Latarnia_Swiatlo", localPos + new Vector3(0f, 6.2f, 0f), 0.7f, yellow);
        }

        static void PaczkomatMachine(Transform t, Vector3 localPos, float rotY, string cityId)
        {
            var np = new GameObject("Paczkomat");
            np.transform.SetParent(t);
            np.transform.localPosition = localPos;
            np.transform.eulerAngles = new Vector3(0f, rotY, 0f);
            var m = np.AddComponent<Miasta.Paczkomat>();
            m.cityName = cityId;

            Box(np.transform, "Korpus", new Vector3(0f, 1.3f, 0f), new Vector3(2.6f, 2.6f, 0.8f), yellow, Vector3.zero);
            Box(np.transform, "Baza", new Vector3(0f, 0.15f, 0f), new Vector3(2.9f, 0.3f, 1.1f), dark, Vector3.zero);
            Box(np.transform, "Ekran", new Vector3(0.7f, 2.2f, 0.42f), new Vector3(1f, 0.7f, 0.06f), dark, Vector3.zero);
            Box(np.transform, "Pokretlo", new Vector3(-0.7f, 2.2f, 0.42f), new Vector3(0.5f, 0.7f, 0.06f), greenMat(), Vector3.zero);
            int i = 0;
            for (int row = 0; row < 2; row++)
                for (int col = 0; col < 4; col++)
                {
                    float x = -0.93f + col * 0.62f;
                    float y = 0.55f + row * 0.75f;
                    Box(np.transform, "Drzwi_Skrytki_" + i, new Vector3(x, y, 0.44f), new Vector3(0.55f, 0.66f, 0.07f), Mat(new Color(0.88f, 0.7f, 0.2f), 0.3f, 0.5f), Vector3.zero);
                    var pkg = Box(np.transform, "Paczka_Skrytki_" + i, new Vector3(x, y, 0.15f), new Vector3(0.4f, 0.42f, 0.34f), Mat(new Color(0.8f, 0.6f, 0.15f), 0f, 0.4f), Vector3.zero);
                    pkg.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    pkg.SetActive(false);
                    i++;
                }
            Text(np.transform, "Napis", "24/7", new Vector3(0f, 2.75f, 0.15f), 0.35f, darkMatColor(), Vector3.zero, 40);
            var col2 = np.AddComponent<BoxCollider>();
            col2.center = new Vector3(0f, 1.3f, 0.2f);
            col2.size = new Vector3(3.4f, 3f, 1.8f);
        }

        static Color darkMatColor() { return new Color(0.1f, 0.1f, 0.12f); }

        static void MagazynBudynek(Transform t, Vector3 localPos, string cityId)
        {
            var np = new GameObject("Magazyn");
            np.transform.SetParent(t);
            np.transform.localPosition = localPos;
            np.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            var mg = np.AddComponent<Miasta.Magazyn>();
            mg.cityName = cityId;

            Box(np.transform, "Budynek", new Vector3(0f, 6f, 0f), new Vector3(44f, 12f, 30f), Mat(new Color(0.5f, 0.52f, 0.55f), 0f, 0.3f), Vector3.zero);
            Box(np.transform, "Rozeta", new Vector3(0f, 13f, 0f), new Vector3(45f, 1.4f, 31f), dark, Vector3.zero);
            Box(np.transform, "Brama", new Vector3(0f, 4.5f, 15.1f), new Vector3(14f, 9f, 0.4f), Mat(new Color(0.75f, 0.78f, 0.8f), 0.6f, 0.6f), Vector3.zero);
            Box(np.transform, "Okno_Biuro", new Vector3(14f, 8f, 15.1f), new Vector3(8f, 3f, 0.3f), glass, Vector3.zero);
            for (int i = 0; i < 5; i++)
                Box(np.transform, "Skrzynia_" + i, new Vector3(-16f + i * 3f, 1f, 19f + (i % 2) * 2f), new Vector3(1.8f, 2f, 1.8f), Mat(new Color(0.75f, 0.6f, 0.2f), 0f, 0.3f), new Vector3(0f, i * 11f, 0f));
            Text(np.transform, "Napis", "MAGAZYN", new Vector3(0f, 13.5f, 15.4f), 1.5f, darkMatColor(), Vector3.zero, 48);
        }

        static void SalonBudynek(Transform t, Vector3 localPos, string cityId)
        {
            var np = new GameObject("Salon");
            np.transform.SetParent(t);
            np.transform.localPosition = localPos;
            np.transform.eulerAngles = new Vector3(0f, 180f, 0f);

            Box(np.transform, "Budynek", new Vector3(0f, 5f, 0f), new Vector3(40f, 10f, 26f), white, Vector3.zero);
            Box(np.transform, "Szyb_Predni", new Vector3(0f, 4.5f, 13.1f), new Vector3(37f, 8f, 0.4f), glass, Vector3.zero);
            Box(np.transform, "Pas_Pomarancz", new Vector3(0f, 10.2f, 0f), new Vector3(40.6f, 1.4f, 26.6f), orange, Vector3.zero);
            foreach (var x in new float[] { -16f, 0f, 16f })
                Box(np.transform, "Filar", new Vector3(x, 5f, 13.1f), new Vector3(1.4f, 10f, 1.2f), orange, Vector3.zero);
            Box(np.transform, "Pokrycie_Dachu", new Vector3(0f, 10.6f, 0f), new Vector3(41f, 1f, 27f), dark, Vector3.zero);
            Text(np.transform, "Napis", "SALON SAMOCHODOWY", new Vector3(0f, 11.7f, 13.5f), 1.1f, Color.white, Vector3.zero, 48);

            CarFactoryInEditor(np.transform, new Vector3(-8f, 0f, 4f), "Dostawczak");
            CarFactoryInEditor(np.transform, new Vector3(8f, 0f, 4f), "Krasnorower");

            var npc = Npc(np.transform, "Sprzedawca", new Vector3(0f, 0f, 20f), new Color(0.2f, 0.5f, 0.9f), new Color(0.2f, 0.2f, 0.25f), new Vector3(0f, 180f, 0f));
            var sn = npc.AddComponent<Miasta.SalonNPC>();
            sn.salonCity = cityId;

            Box(np.transform, "Plac_Podsalon", new Vector3(0f, 0.15f, -18f), new Vector3(34f, 0.34f, 24f), sidewalk, Vector3.zero);
        }

        static void CarFactoryInEditor(Transform parent, Vector3 localPos, string carId)
        {
            var info = FindCarInfo(carId);
            Color col = info != null ? info.color : Color.white;
            var car = Miasta.CarFactory.BuildMesh(parent, "Auto_Wyswietl", col, 4.6f, 2f, 1.4f);
            car.transform.localPosition = localPos;
            car.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            var cc = car.AddComponent<Miasta.CarController>();
            cc.carId = carId;
            cc.baseMaxSpeed = info != null ? info.maxSpeed : 26f;
            cc.displayCar = true;
        }

        static Miasta.Catalogs.CarInfo FindCarInfo(string id)
        {
            foreach (var c in Miasta.Catalogs.Cars) if (c.id == id) return c;
            return null;
        }

        static void GameManagerAndPlayer(Transform p)
        {
            var gm = new GameObject("Gra");
            gm.transform.SetParent(p);
            gm.AddComponent<Miasta.GameManager>();

            var gracz = new GameObject("Gracz");
            gracz.transform.SetParent(p);
            gracz.transform.position = new Vector3(18f, 1.1f, 34f);
            var ccObj = gracz.AddComponent<CharacterController>();
            ccObj.height = 2f;
            ccObj.radius = 0.4f;
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(gracz.transform, false);
            body.GetComponent<Collider>().enabled = false;
            body.GetComponent<MeshRenderer>().sharedMaterial = Mat(new Color(0.85f, 0.3f, 0.15f), 0f, 0.4f);
            gracz.AddComponent<Miasta.PlayerDriver>();

            var starter = Miasta.CarFactory.BuildMesh(null, "Auto_Start", new Color(0.85f, 0.34f, 0.16f), 4.6f, 2f, 1.4f);
            starter.transform.SetParent(p);
            starter.transform.position = new Vector3(28f, 0.15f, 40f);
            starter.transform.eulerAngles = new Vector3(0f, 50f, 0f);
            var sc = starter.AddComponent<Miasta.CarController>();
            sc.carId = "Kasztanka";
            sc.baseMaxSpeed = 26f;
        }
    }
}
