using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public Light trafficLight; // Asigna aquí la Point Light del semáforo

    // Posición para cada estado del semáforo
    private Vector3 initialPosition = new Vector3(-0.4984913f, 5.866117f, -0.1288072f);
    private Vector3 greenPosition = new Vector3(-0.4984913f, 5.3f, -0.1288072f); // Posición más baja para el verde
    private Vector3 redPosition = new Vector3(-0.4984913f, 6.5f, -0.1288072f);   // Posición más alta para el rojo

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
            trafficLight.transform.position = greenPosition; // Cambia a la posición para verde
            yield return new WaitForSeconds(6);

            // Luz Amarilla (12 segundos)
            trafficLight.color = yellowColor;
            trafficLight.intensity = 5;
            trafficLight.transform.position = initialPosition; // Cambia a la posición inicial para amarillo
            yield return new WaitForSeconds(2);

            // Luz Roja (60 segundos)
            trafficLight.color = redColor;
            trafficLight.intensity = 5;
            trafficLight.transform.position = redPosition; // Cambia a la posición superior para rojo
            yield return new WaitForSeconds(6);

            // Luz Amarilla (12 segundos)
            trafficLight.color = yellowColor;
            trafficLight.intensity = 5;
            trafficLight.transform.position = initialPosition; // Cambia a la posición inicial para amarillo
            yield return new WaitForSeconds(2);
        }
    }
}