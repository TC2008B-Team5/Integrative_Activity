using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public Light trafficLight; // Asigna aqu� la Point Light del sem�foro

    // Posici�n para cada estado del sem�foro
    private Vector3 initialPosition = new Vector3(-0.4984913f, 5.866117f, -0.1288072f);
    private Vector3 greenPosition = new Vector3(-0.4984913f, 5.3f, -0.1288072f); // Posici�n m�s baja para el verde
    private Vector3 redPosition = new Vector3(-0.4984913f, 6.5f, -0.1288072f);   // Posici�n m�s alta para el rojo

    private Color greenColor = Color.green;
    private Color yellowColor = Color.yellow;
    private Color redColor = Color.red;

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    private IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Luz Verde (60 segundos)
            trafficLight.color = greenColor;
            trafficLight.intensity = 10;
            trafficLight.transform.position = greenPosition; // Cambia a la posici�n para verde
            yield return new WaitForSeconds(6);

            // Luz Amarilla (12 segundos)
            trafficLight.color = yellowColor;
            trafficLight.intensity = 5;
            trafficLight.transform.position = initialPosition; // Cambia a la posici�n inicial para amarillo
            yield return new WaitForSeconds(2);

            // Luz Roja (60 segundos)
            trafficLight.color = redColor;
            trafficLight.intensity = 5;
            trafficLight.transform.position = redPosition; // Cambia a la posici�n superior para rojo
            yield return new WaitForSeconds(6);

            // Luz Amarilla (12 segundos)
            trafficLight.color = yellowColor;
            trafficLight.intensity = 5;
            trafficLight.transform.position = initialPosition; // Cambia a la posici�n inicial para amarillo
            yield return new WaitForSeconds(2);
        }
    }
}