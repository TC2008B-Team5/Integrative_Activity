using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public Turn turnScript; // Referencia al script Turn para controlar el coche 7
    public float timeToUpdate = 5f; // Tiempo entre actualizaciones
    private float timer;

    void Start()
    {
        if (turnScript == null)
        {
            turnScript = GetComponent<Turn>();
        }
        StartCoroutine(RequestPositions());
    }

    void Update()
    {
        // Temporizador para solicitar nuevas posiciones
        timer += Time.deltaTime;
        if (timer >= timeToUpdate)
        {
            timer = 0;
            StartCoroutine(RequestPositions());
        }
    }

    IEnumerator RequestPositions()
    {
        string url = "http://127.0.0.1:8000/positions"; // URL del servidor
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string txt = www.downloadHandler.text;
                Debug.Log($"Recibido: {txt}");

                // Procesar y actualizar la ruta del coche 7
                List<Vector3> nuevasPosiciones = ProcesarDatos(txt);
                if (turnScript != null)
                {
                    //turnScript.ActualizarRuta(nuevasPosiciones); // Actualizar la ruta del coche 7
                }
            }
        }
    }

    // Método para procesar datos recibidos del servidor y convertirlos a una lista de Vector3
    private List<Vector3> ProcesarDatos(string json)
    {
        List<Vector3> posiciones = new List<Vector3>();

        // Deserializar el JSON en una lista de posiciones
        try
        {
            PositionList data = JsonUtility.FromJson<PositionList>(json);
            if (data != null && data.positions != null)
            {
                foreach (var item in data.positions)
                {
                    posiciones.Add(new Vector3(item.x, item.y, item.z));
                }
            }
            else
            {
                Debug.LogWarning("JSON vacío o estructura no válida.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al procesar el JSON: {e.Message}");
        }

        return posiciones;
    }

    // Clases auxiliares para deserialización de JSON
    [System.Serializable]
    public class PositionData
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class PositionList
    {
        public List<PositionData> positions;
    }
}
