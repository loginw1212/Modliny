using UnityEngine;

namespace Miasta
{
    public class CarFactory
    {
        public static GameObject Spawn(string carId, Vector3 pos, bool display = false)
        {
            var info = GameManager.I != null ? GameManager.I.GetCar(carId) : null;
            Color col = info != null ? info.color : new Color(0.85f, 0.34f, 0.16f);
            var car = BuildMesh(null, "Auto_" + carId + "_" + Random.Range(100, 999), col, 4.6f, 2.0f, 1.4f);
            car.transform.position = pos;
            var cc = car.AddComponent<CarController>();
            cc.carId = carId;
            cc.baseMaxSpeed = info != null ? info.maxSpeed : 26f;
            cc.displayCar = display;
            if (GameManager.I != null && GameManager.I.goldPaint == "tak" && !display)
                cc.SetPaint(new Color(0.95f, 0.82f, 0.2f));
            return car;
        }

        public static GameObject BuildMesh(Transform parent, string name, Color col, float len, float wid, float roofZ)
        {
            var np = new GameObject(name);
            if (parent != null) np.transform.SetParent(parent, false);
            var bodyMat = new Material(LitShader.Get());
            bodyMat.color = col;
            var darkMat = new Material(LitShader.Get());
            darkMat.color = new Color(0.12f, 0.12f, 0.14f);
            var wheelMat = new Material(LitShader.Get());
            wheelMat.color = new Color(0.08f, 0.08f, 0.08f);
            var metalMat = new Material(LitShader.Get());
            metalMat.color = new Color(0.75f, 0.78f, 0.8f);
            if (metalMat.HasProperty("_Metallic")) metalMat.SetFloat("_Metallic", 0.8f);

            Body(np.transform, "Karoseria", new Vector3(0f, wid * 0.35f, 0f), new Vector3(wid, wid * 0.7f, len), bodyMat);
            Body(np.transform, "Kabina", new Vector3(0f, wid * 0.75f, roofZ), new Vector3(wid * 0.88f, wid * 0.55f, len * 0.45f), bodyMat);
            Body(np.transform, "Szyba_P", new Vector3(0f, wid * 0.75f, roofZ + len * 0.23f), new Vector3(wid * 0.8f, wid * 0.4f, 0.15f), darkMat);
            foreach (var sx in new float[] { -1f, 1f })
                foreach (var sz in new float[] { -1f, 1f })
                {
                    var w = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    w.name = "Kolo";
                    w.transform.SetParent(np.transform, false);
                    w.transform.localPosition = new Vector3(sx * wid * 0.5f, wid * 0.5f * 0.8f, sz * len * 0.32f);
                    w.transform.localScale = new Vector3(0.55f, 0.18f, 0.55f);
                    w.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                    w.GetComponent<MeshRenderer>().sharedMaterial = wheelMat;
                }
            foreach (var sx in new float[] { -1f, 1f })
            {
                var l = Body(np.transform, "Swiatlo", new Vector3(sx * wid * 0.32f, wid * 0.45f, len * 0.5f - 0.05f), new Vector3(0.3f, 0.16f, 0.1f), metalMat);
                l.name = sx > 0f ? "Swiatlo_P" : "Swiatlo_L";
            }
            var bc = np.AddComponent<BoxCollider>();
            bc.center = new Vector3(0f, wid * 0.25f + 0.3f, 0f);
            bc.size = new Vector3(wid + 0.4f, 2.2f, len + 0.4f);
            return np;
        }

        static GameObject Body(Transform parent, string name, Vector3 localPos, Vector3 size, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
            return go;
        }
    }
}
