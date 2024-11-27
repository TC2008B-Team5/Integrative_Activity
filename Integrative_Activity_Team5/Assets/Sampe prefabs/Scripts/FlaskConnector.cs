using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class FlaskConnector : MonoBehaviour
{
    public GameObject carPrefab; // Assign a car prefab in the Unity editor
    private List<GameObject> cars = new List<GameObject>();

    void Start()
    {
        // Start fetching data from Flask
        StartCoroutine(FetchSimulationData());
    }

    IEnumerator FetchSimulationData()
    {
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:5000/get_simulation_data"))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    SimulationData data = JsonConvert.DeserializeObject<SimulationData>(json);
                    UpdateCars(data.cars);
                }
                else
                {
                    Debug.LogError("Failed to fetch data: " + request.error);
                }
            }
            yield return new WaitForSeconds(1); // Fetch data every second
        }
    }

    void UpdateCars(List<CarData> carsData)
    {
        for (int i = 0; i < carsData.Count; i++)
        {
            if (i >= cars.Count)
            {
                // Create new car GameObject if not enough exist
                GameObject car = Instantiate(carPrefab);
                car.name = "Car_" + carsData[i].id;
                cars.Add(car);
            }
            // Update position
            cars[i].transform.position = new Vector3(carsData[i].position[0], 0, carsData[i].position[1]);
        }
    }
}

[System.Serializable]
public class SimulationData
{
    public List<CarData> cars;
}

[System.Serializable]
public class CarData
{
    public int id;
    public List<int> position;
}