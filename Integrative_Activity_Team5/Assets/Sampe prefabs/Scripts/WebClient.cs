using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    public MovimientoCoches movimientoCoches; // Referencia al script de coches
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

                // Deserializar JSON en una lista de posiciones
                List<Vector3> nuevasPosiciones = ProcesarDatos(txt);
                movimientoCoches.ActualizarPosicionesLlegada(nuevasPosiciones);
            }
        }
    }

    // Método ficticio para procesar datos recibidos del servidor
    private List<Vector3> ProcesarDatos(string json)
    {
        // Aquí deserializarías el JSON a una lista de Vector3
        // Por ahora, devolvemos una lista vacía como ejemplo
        return new List<Vector3>();
    }
}
