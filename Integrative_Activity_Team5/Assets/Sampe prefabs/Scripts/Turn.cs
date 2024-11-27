using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Turn : MonoBehaviour
{
    public GameObject car; // Coche asignado automáticamente desde MovimientoCoches
    [SerializeField]
    List<Vector3> path;
    List<Vector3> originals;
    Matrix4x4 mem, tra, rot, m, Sca;
    int index, rotCounter; // Índice para la ruta 
    bool corner;
    Vector3 pivot;

    void Start()
    {
        index = rotCounter = 0;

        if (car == null)
        {
            Debug.LogError("No se asignó un coche al script Turn.");
            return;
        }

        // Obtener los vértices originales del coche asignado
        originals = new List<Vector3>(car.GetComponent<MeshFilter>().mesh.vertices);
        mem = VecOps.TranslateM(path[index]);
        tra = VecOps.TranslateM(new Vector3(0, 0, -0.1f));
        m = Matrix4x4.identity;
    }

    void Update()
    {
        if (car == null) return; // Asegúrate de que el coche esté asignado

        Vector3 currPos = new Vector3(mem[0, 3], mem[1, 3], mem[2, 3]);
        Debug.Log($"Current Position: {currPos} | Index: {index}");

        if (!corner)
        {
            Vector3 nextPath = path[index + 1];
            if (VecOps.Magnitude(nextPath - currPos) <= 0.01f)
            {
                Debug.Log($"Reached path point: {nextPath}. Advancing to index {index + 1}");
                index++;
                Vector3 prevPath = path[index - 1];
                if (prevPath.x != nextPath.x || prevPath.y != nextPath.y)
                {
                    corner = true;
                    pivot = new Vector3((nextPath.x - prevPath.x) / 2.0f, 0, (nextPath.z - prevPath.z) / 2.0f);
                    Debug.Log($"Corner detected. Calculated pivot: {pivot}");
                    tra = Matrix4x4.identity;
                }
            }
        }
        else
        {
            tra = Matrix4x4.identity;
            if (rotCounter < 90)
            {
                Debug.Log($"Rotating around pivot: {pivot} | Rotation Step: {rotCounter}");
                Matrix4x4 mpiv = VecOps.TranslateM(pivot);
                Matrix4x4 Mpneg = VecOps.TranslateM(-pivot);
                Matrix4x4 rotM = VecOps.RotateYM(rotCounter);
                Matrix4x4 Tc = VecOps.TranslateM(currPos);
                rot = mpiv * rotM * Mpneg * Tc;
                rotCounter++;
            }
            else
            {
                Debug.Log($"Finished rotation at index {index}. Resetting rotation counter.");
                rotCounter = 0;
                corner = false;
                index++;
                rot = Matrix4x4.identity;
                tra = VecOps.TranslateM(new Vector3(0, 0, -0.1f));
            }
        }
        m = mem * tra * rot * Sca;
        car.GetComponent<MeshFilter>().mesh.vertices = VecOps.ApplyTransform(originals, m).ToArray();
        mem = mem * tra * rot;
    }
}
