using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class FlaskConnector : MonoBehaviour
{
    private float timer = 0f;
    // Adjusted update time to match animation duration
    private float timeToUpdate = 16f; 
    private List<StepData> positionsOverTime;
    public GameObject carPrefab;
    private Dictionary<int, GameObject> cars = new Dictionary<int, GameObject>();

    private bool isAnimating = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeToUpdate)
        {
            Debug.Log($"Passed {timeToUpdate} seconds");
            Debug.Log("Calling GetCarPositions");
            StartCoroutine(GetCarPositions());
            timer = 0f;
        }
    }

    IEnumerator GetCarPositions()
    {
        string url = "http://127.0.0.1:8000/car_positions";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                //Debug.Log("Raw JSON response: ");
                //Debug.Log(jsonResponse);
                positionsOverTime = ParsePositions(jsonResponse);
                Debug.Log("Received car positions over time");
                InitializeOrUpdateCars();
            }
        }
    }

    List<StepData> ParsePositions(string json)
    {
        List<StepData> steps = JsonConvert.DeserializeObject<List<StepData>>(json);
        return steps;
    }

    void InitializeOrUpdateCars()
    {
        if (positionsOverTime == null || positionsOverTime.Count == 0)
        {
            Debug.Log("No positions to animate.");
            return;
        }

        if (!isAnimating)
        {
            Debug.Log("Starting animation.");
            StartCoroutine(AnimateCars());
        }
        else
        {
            Debug.Log("Animation is already running. Skipping new animation.");
        }
    }

    IEnumerator AnimateCars()
    {
        isAnimating = true;

        foreach (StepData step in positionsOverTime)
        {
            Debug.Log($"Animating step {step.step}");

            foreach (CarData carData in step.cars)
            {
                Debug.Log($"At step {step.step}, car {carData.id} at position ({carData.position[0]}, {carData.position[1]})");

                if (!cars.ContainsKey(carData.id))
                {
                    Debug.Log($"Creating car with id {carData.id}");
                    Vector3 initialPosition = new Vector3(carData.position[0], 0, carData.position[1]);
                    Debug.Log($"Initial position for car {carData.id}: {initialPosition}");
                    GameObject car = Instantiate(carPrefab, initialPosition, Quaternion.identity);
                    cars.Add(carData.id, car);
                }
                else
                {
                    GameObject car = cars[carData.id];
                    Vector3 targetPosition = new Vector3(carData.position[0], 0, carData.position[1]);
                    Debug.Log($"Moving car with id {carData.id} to {targetPosition}");
                    StartCoroutine(MoveCar(car, targetPosition));
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        isAnimating = false;
        positionsOverTime.Clear(); // Clear data after animation completes
    }

    IEnumerator MoveCar(GameObject car, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        Vector3 startingPosition = car.transform.position;

        while (elapsedTime < duration)
        {
            car.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        car.transform.position = targetPosition;
    }
}