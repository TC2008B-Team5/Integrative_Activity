using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Turn : MonoBehaviour
{
    [SerializeField]
    GameObject carPrefab;
    GameObject car;
    [SerializeField]
    List<Vector3> path;
    List<Vector3> originals;
    Matrix4x4 mem,tra,rot,m,Sca;
    int index, rotCounter; //Index for the path 
    bool corner;
    Vector3 pivot;
    // Start is called before the first frame update
    void Start()
    {
        index = rotCounter=0;
        car = Instantiate(carPrefab,Vector3.zero,Quaternion.identity);
        originals = new List<Vector3>(car.GetComponent<MeshFilter>().mesh.vertices);
        mem = VecOps.TranslateM(path[index]);
        tra = VecOps.TranslateM(new Vector3(0,0,-0.1f));
        m = Matrix4x4.identity;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 currPos = new Vector3(mem[0,3],mem[1,3],mem[2,3]);
        if(!corner){ //GO forward
            Vector3 nextPath = path[index + 1];
            if(VecOps.Magnitude(nextPath-currPos) <= 0.01f){
                index++;
                //How can we know if this is a corner? it's a corner when prev.x != next.x or prev.y != next.y (I guess....)
                Vector3 prevPath = path[index -1];
                if(prevPath.x != nextPath.x || prevPath.y != nextPath.y){
                    corner = true;
                    //How can we get the pivot from the prevPath, curr{ath and nextPath?
                    pivot = new Vector3((nextPath.x - prevPath.x)/ 2.0f,0,(nextPath.z - prevPath.z)/2.0f);
                    tra = Matrix4x4.identity;
                }
            }
        }
        else{ 
            tra = Matrix4x4.identity; //Cancel the translation
            //Rotate around the pivot: Cancel,Rotate,BringBack = Tc * Mpneg * rotM * Mpiv. For UnityL Mpiv * rotM * Mpneg * Tc
            if(rotCounter < 90){
                Matrix4x4 mpiv = VecOps.TranslateM(pivot);
                Matrix4x4 Mpneg = VecOps.TranslateM(-pivot);
                Matrix4x4 rotM = VecOps.RotateYM(rotCounter);
                Matrix4x4 Tc = VecOps.TranslateM(currPos);
                rot = mpiv * rotM * Mpneg * Tc; 
                rotCounter++;
            }else{
                rotCounter = 0;
                corner = false;
                index++; //Were done rotating, and we switched to the next cell
                rot = Matrix4x4.identity; //Cancel de rotation
                tra = VecOps.TranslateM(new Vector3(0,0,-0.1f));
            }
        }
        m = mem * tra * rot * Sca;
        car.GetComponent<MeshFilter>().mesh.vertices = VecOps.ApplyTransform(originals,m).ToArray();
        mem = mem * tra * rot;

    }
}