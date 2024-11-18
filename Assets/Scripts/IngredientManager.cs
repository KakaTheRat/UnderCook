using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using EzySlice;

public class IngredientManager : MonoBehaviour
{
    private string ingredientName;
    private bool canBeCut;
    private bool canBeCook;

    private bool isCook;
    private bool isCut;
    public Material crossSectionMaterial;


    public void SetAttributes(string _name, bool _canBeCut, bool _canBeCook){
        ingredientName = _name;
        canBeCut = _canBeCut;
        canBeCook = _canBeCook;
    }

    public void Cook(){
        isCook = true;
        GameObject smoke = Instantiate(Resources.Load<GameObject>("Prefabs/Smoke/Smoke01"), gameObject.transform);
        smoke.transform.localRotation = Quaternion.Euler(0f,0f,0f);
    }

    public void Cut(){
        isCut = true;
        Slice();
    }

    public bool GetCook(){
        return isCook;
    }

    public bool GetCut(){
        return isCut;
    }

    public bool GetCanBeCook(){
        return canBeCook;
    }

    public bool GetCanBeCut(){
        return canBeCut;
    }

    public void Slice() 
    {
	    SlicedHull slicedHull = gameObject.Slice(transform.position, new Vector3(0, 0, 1));
        if(slicedHull != null){

            GameObject slicedObject1 = slicedHull.CreateUpperHull(gameObject);
            GameObject slicedObject2 = slicedHull.CreateLowerHull(gameObject);

            ApplyInternalMaterial(slicedObject1);
            ApplyInternalMaterial(slicedObject2);

            if(slicedObject1 != null && slicedObject2 != null){
                Destroy(GetComponent<MeshRenderer>());
                Destroy(GetComponent<MeshFilter>());
                slicedObject1.transform.SetParent(transform);
                slicedObject1.transform.position = transform.position + new Vector3(0f,0f,0.1f);
                slicedObject1.transform.rotation = transform.rotation;
                slicedObject2.transform.SetParent(transform);
                slicedObject2.transform.position = transform.position - new Vector3(0f,0f,0.1f);
                slicedObject2.transform.rotation = transform.rotation;
            }
        }else{Debug.LogFormat("Error");}
    }

    private void ApplyInternalMaterial(GameObject slicedObject)
    {
        // Assurez-vous que l'objet possède un renderer
        Renderer renderer = slicedObject.GetComponent<Renderer>();

        if (renderer != null && renderer.materials.Length > 1)
        {
            Material blackMaterial = new Material(Shader.Find("Standard"));
            blackMaterial.color = Color.black;
            renderer.materials[1] = blackMaterial;
        }
    }
}
