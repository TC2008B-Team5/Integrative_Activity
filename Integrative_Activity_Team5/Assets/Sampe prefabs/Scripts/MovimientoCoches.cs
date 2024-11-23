using System.Collections.Generic;
using UnityEngine;

public class MovimientoCoches : MonoBehaviour
{
    public List<GameObject> carPrefabs; // Lista de prefabs de los coches
    public List<Vector3> posicionesIniciales = new List<Vector3>
    {
        new Vector3(-5f, 0.85f, -164.5f),  // coche numero 1
        new Vector3(5f, 0.85f, -164.5f),  // coche numero 2
        new Vector3(5f, 0.85f, -154.5f), // coche numero 3
        new Vector3(-5f, 0.85f, -154.5f), // coche numero 4
        new Vector3(215f, 0.85f, -164.5f), // coche numero 5
        new Vector3(225f, 0.85f, -154.5f),  // coche numero 6
        new Vector3(215f, 0.85f, -154.5f),  // coche numero 7
        new Vector3(225f, 0.85f, -164.5f), // coche numero 8
        new Vector3(215f, 0.85f, 55.5f), // coche numero 9
        new Vector3(225f, 0.85f, 55.5f), // coche numero 10
        new Vector3(215f, 0.85f, 65.5f), // coche numero 11
        new Vector3(225f, 0.85f, 65.5f), // coche numero 12
        new Vector3(-5f, 0.85f, 65.5f), // coche numero 13
        new Vector3(-5f, 0.85f, 55.5f), // coche numero 14
        new Vector3(5f, 0.85f, 65.5f), // coche numero 15
        new Vector3(5f, 0.85f, 55.5f), // coche numero 16
        new Vector3(125f, 0.85f, -45.5f)  // coche numero 17
    };

    public List<Vector3> posicionesLlegada = new List<Vector3>
    {
        new Vector3(-43f, 8.5f, -87.5f), // estacionamiento numero 1
        new Vector3(-30f, 8.5f, -14f),  // estacionamiento numero 2
        new Vector3(-30f, 8.5f, -170.5f), // estacionamiento numero 3
        new Vector3(-20f, 8.5f, -110f), // estacionamiento numero 4
        new Vector3(-19.5f, 8.5f, -193.7f), // estacionamiento numero 5
        new Vector3(-7f, 8.5f, -57f), // estacionamiento numero 6
        new Vector3(17.5f, 8.5f, -76.5f), // estacionamiento numero 7
        new Vector3(31f, 8.5f, -210f), // estacionamiento numero 8
        new Vector3(41f, 8.5f, -38.5f), // estacionamiento numero 9
        new Vector3(41f, 8.5f, -109.7f), // estacionamiento numero 10
        new Vector3(41f, 8.5f, -156f), // estacionamiento numero 11
        new Vector3(111f, 8.5f, -14f), // estacionamiento numero 12
        new Vector3(113f, 8.5f, -166.5f), // estacionamiento numero 13
        new Vector3(113f, 8.5f, -187f), // estacionamiento numero 14
        new Vector3(141f, 8.5f, -50f), // estacionamiento numero 15
        new Vector3(141f, 8.5f, -74f), // estacionamiento numero 16
        new Vector3(137.5f, 8.5f, -186.5f) // estacionamiento numero 17
    };

    private List<GameObject> cars = new List<GameObject>(); // Instancias de los coches

    void Start()
    {
        CrearCoches();
    }

    // Método para crear los coches en las posiciones iniciales
    private void CrearCoches()
    {
        for (int i = 0; i < posicionesIniciales.Count; i++)
        {
            // Elegir un prefab aleatorio
            GameObject randomPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];

            // Instanciar el coche en la posición inicial
            GameObject car = Instantiate(randomPrefab, posicionesIniciales[i], Quaternion.identity);
            car.name = $"Car_{i + 1}";
            cars.Add(car);
        }
    }

    // Actualizar las posiciones de llegada
    public void ActualizarPosicionesLlegada(List<Vector3> nuevasPosiciones)
    {
        if (nuevasPosiciones.Count == cars.Count)
        {
            for (int i = 0; i < cars.Count; i++)
            {
                cars[i].transform.position = nuevasPosiciones[i];
            }
            Debug.Log("Se actualizaron las posiciones de los coches");
        }
        else
        {
            Debug.LogWarning("El número de nuevas posiciones no coincide con el número de coches");
        }
    }
}
