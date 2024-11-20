using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public MovimientoCoches movimientoCoches; // Referencia a MovimientoCoches
    public float timeToUpdate = 5f;          // Tiempo entre actualizaciones
    private float timer;

    void Start()
    {
        if (movimientoCoches == null)
        {
            movimientoCoches = GetComponent<MovimientoCoches>();
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

                // Aquí deberías deserializar `txt` en un formato que puedas usar (por ejemplo, JSON)
                // Suponiendo que el servidor devuelve una lista de posiciones:
                List<Vector3> nuevasPosiciones = ProcesarDatos(txt);
                movimientoCoches.ActualizarPosicionesLlegada(nuevasPosiciones);
            }
        }
    }

    // Método ficticio para procesar datos recibidos del servidor
    private List<Vector3> ProcesarDatos(string json)
    {
        // Aquí iría la deserialización del JSON a una lista de Vector3
        // Por ahora, devolvemos una lista vacía como ejemplo
        return new List<Vector3>();
    }
}

public class MovimientoCoches : MonoBehaviour
{
    // Lista de coordenadas iniciales (posiciones de los coches)
    public List<Vector3> posicionesIniciales = new List<Vector3>
    {
        new Vector3(-50f, 8.5f, 3f),  // coche numero 1
        new Vector3(-60f, 8.5f, 3f),  // coche numero 2
        new Vector3(-60f, 8.5f, -7f),  // coche numero 3
        new Vector3(-50f, 8.5f, -7f),  // coche numero 4
        new Vector3(160f, 8.5f, 3f),  // coche numero 5
        new Vector3(170f, 8.5f, 3f),  // coche numero 6
        new Vector3(170f, 8.5f, -7f),  // coche numero 7
        new Vector3(160f, 8.5f, -7f),  // coche numero 8
        new Vector3(160f, 8.5f, -217f),  // coche numero 9
        new Vector3(170f, 8.5f, -217f),  // coche numero 10
        new Vector3(170f, 8.5f, -227f),  // coche numero 11
        new Vector3(160f, 8.5f, -227f),  // coche numero 12
        new Vector3(-60f, 8.5f, -217f),  // coche numero 13
        new Vector3(-50f, 8.5f, -217f),  // coche numero 14
        new Vector3(-50f, 8.5f, -227f),  // coche numero 15
        new Vector3(-60f, 8.5f, -227f),  // coche numero 16
        new Vector3(100f, 8.5f, -137f)   // coche numero 17
    };

    // Lista de coordenadas de llegada (lugares de estacionamiento)
    public List<Vector3> posicionesLlegada = new List<Vector3>
    {
        new Vector3(-43f, 8.5f, -87.5f),  // estacionamiento numero 1
        new Vector3(-30f, 8.5f, -14f),  // estacionamiento numero 2
        new Vector3(-30f, 8.5f, -170.5f),  // estacionamiento numero 3
        new Vector3(-20f, 8.5f, -110f),  // estacionamiento numero 4
        new Vector3(-19.5f, 8.5f, -193.7f),  // estacionamiento numero 5
        new Vector3(-7f, 8.5f, -57f),  // estacionamiento numero 6
        new Vector3(17.5f, 8.5f, -76.5f),  // estacionamiento numero 7
        new Vector3(31f, 8.5f, -210f),  // estacionamiento numero 8
        new Vector3(41f, 8.5f, -38.5f),  // estacionamiento numero 9
        new Vector3(41f, 8.5f, -109.7f),  // estacionamiento numero 10
        new Vector3(41f, 8.5f, -156f),  // estacionamiento numero 11
        new Vector3(111f, 8.5f, -14f),  // estacionamiento numero 12
        new Vector3(113f, 8.5f, -166.5f),  // estacionamiento numero 13
        new Vector3(113f, 8.5f, -187f),  // estacionamiento numero 14
        new Vector3(141f, 8.5f, -50f),  // estacionamiento numero 15
        new Vector3(141f, 8.5f, -74f),  // estacionamiento numero 16
        new Vector3(137.5f, 8.5f, -186.5f)  // estacionamiento numero 17
    };

    void Start()
    {
        // Debug inicial
        for (int i = 0; i < posicionesIniciales.Count; i++)
        {
            Debug.Log($"Coche {i + 1} inicia en {posicionesIniciales[i]} y va al estacionamiento en {posicionesLlegada[i]}");
        }
    }

    public void ActualizarPosicionesLlegada(List<Vector3> nuevasPosiciones)
    {
        if (nuevasPosiciones.Count == posicionesLlegada.Count)
        {
            posicionesLlegada = nuevasPosiciones;
            Debug.Log("Se actualizaron las posiciones de llegada");
        }
        else
        {
            Debug.LogWarning("El número de nuevas posiciones no coincide con el número de coches");
        }
    }
}
