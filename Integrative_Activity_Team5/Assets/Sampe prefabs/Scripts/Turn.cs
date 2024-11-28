using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Turn : MonoBehaviour
{
    public GameObject car; // Coche asignado autom�ticamente desde MovimientoCoches
    [SerializeField]
    List<Vector3> path;
    List<Vector3> originals;
    Matrix4x4 mem, tra, rot, m, Sca;
    int index, rotCounter; // �ndice para la ruta 
    bool corner;
    Vector3 pivot;

    void Start()
    {
        index = rotCounter = 0;

        if (car == null)
        {
            Debug.LogError("No se asign� un coche al script Turn.");
            return;
        }

        if (path == null || path.Count == 0)
        {
            Debug.LogError("Path no est� inicializado o est� vac�o.");
            return;
        }

        originals = new List<Vector3>(car.GetComponent<MeshFilter>().mesh.vertices);

        // Inicializa la posici�n inicial
        mem = VecOps.TranslateM(path[index]);
        tra = VecOps.TranslateM(new Vector3(0, 0, -0.1f));
        rot = Matrix4x4.identity;
        Sca = Matrix4x4.identity;

        Debug.Log($"Initial Matrix mem: {mem}");
        Debug.Log($"Initial Translation Matrix tra: {tra}");
        Debug.Log($"Initial Rotation Matrix rot: {rot}");
        Debug.Log($"Initial Scale Matrix Sca: {Sca}");
    }


    // A�ade esta variable al inicio de tu script Turn
    private bool hasReachedDestination = false;

    void Update()
    {
        // Det�n el m�todo si el coche ya lleg� a su destino final
        if (hasReachedDestination)
        {
            return; // Sale inmediatamente si el coche ya lleg�
        }

        if (car == null) return; // Aseg�rate de que el coche est� asignado

        // Verifica si ya se alcanz� el �ltimo punto del path
        if (index >= path.Count - 1)
        {
            Debug.Log("El coche ha llegado al destino final. Deteniendo movimiento.");
            hasReachedDestination = true; // Marca que el coche ya lleg�
            return;
        }

        // Depura el estado inicial de 'mem'
        Debug.Log($"Matrix mem (start of Update): {mem}");

        Vector3 currPos = new Vector3(mem[0, 3], mem[1, 3], mem[2, 3]);
        Debug.Log($"Current Position: {currPos} | Index: {index}");

        Vector3 nextPath = path[index + 1];
        float distanceToNextPath = VecOps.Magnitude(nextPath - currPos);
        Debug.Log($"Distance to next path point: {distanceToNextPath}");

        if (distanceToNextPath <= 0.1f) // Ajusta el umbral seg�n el tama�o del paso
        {
            Debug.Log($"Reached path point: {nextPath}. Advancing to index {index + 1}");
            index++;
            corner = false; // Reinicia el estado para permitir movimiento hacia el nuevo punto
            return; // Sal del ciclo actual y espera al siguiente frame
        }

        // Calcula la direcci�n hacia el siguiente punto
        Vector3 direction = VecOps.Normalize(nextPath - currPos);
        Debug.Log($"Direction towards next path point: {direction}");

        // Actualiza la traslaci�n usando la direcci�n y un paso fijo
        float step = 0.1f; // Ajusta el tama�o del paso seg�n la velocidad deseada
        tra = VecOps.TranslateM(direction * step);
        Debug.Log($"Updated Translation Matrix tra: {tra}");

        // Depura matrices de transformaci�n
        Debug.Log($"Rotation Matrix rot: {rot}");
        Debug.Log($"Scale Matrix Sca: {Sca}");

        // Acumula transformaciones en 'm'
        Matrix4x4 nextM = mem * tra * rot * Sca;
        if (nextM == Matrix4x4.zero)
        {
            Debug.LogError("Matrix m se convirti� en cero.");
            return;
        }
        m = nextM;

        // Depura el estado de 'm'
        Debug.Log($"Matrix m (final transformation): {m}");

        // Aplica la transformaci�n al coche
        List<Vector3> transformedVertices = VecOps.ApplyTransform(originals, m);
        if (transformedVertices.Count == 0)
        {
            Debug.LogError("La transformaci�n no gener� v�rtices.");
            return;
        }
        car.GetComponent<MeshFilter>().mesh.vertices = transformedVertices.ToArray();

        // Acumula en 'mem'
        Matrix4x4 nextMem = mem * tra * rot;
        if (nextMem == Matrix4x4.zero)
        {
            Debug.LogError("Matrix mem se convirti� en cero despu�s de acumulaci�n.");
            return;
        }
        mem = nextMem;

        // Depura el estado final de 'mem'
        Debug.Log($"Matrix mem (end of Update): {mem}");
    }

}
