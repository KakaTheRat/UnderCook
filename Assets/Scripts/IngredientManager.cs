using UnityEngine;
using EzySlice;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

public class IngredientManager : MonoBehaviour
{
    private string ingredientName;
    private bool canBeCut ;
    private bool canBeCook;

    private bool isCook;
    private bool isCut;
    private Material mat;
    AddressableLoader loader;

    void Awake(){
        loader = gameObject.AddComponent<AddressableLoader>();
    }

    public void SetAttributes(string _name, bool _canBeCut, bool _canBeCook, Material _mat){
        ingredientName = _name;
        canBeCut = _canBeCut;
        canBeCook = _canBeCook;
        mat = _mat;
        Debug.Log($"lol2 {_mat.name}");
    }

    public string GetIngredientName(){
        return ingredientName;
    }

    public void Cook(GameObject smokePrefab){
        isCook = true;
        GameObject smoke = Instantiate(smokePrefab, gameObject.transform);
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
        SlicedHull slicedHull;
	    slicedHull = gameObject.Slice(gameObject.transform.position, new Vector3(0, 0, 1));
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
        Debug.Log($"lol3  {mat.name}");
        MeshRenderer renderer = slicedObject.GetComponent<MeshRenderer>();
        if (renderer != null && renderer.materials.Length > 1)
            {
                Debug.Log("Coucou");
                List<Material> mats = renderer.materials.ToList();
                mats[1] = mat;
                renderer.SetMaterials(mats);
            }
        foreach(Material material in renderer.materials)
        {
            Debug.Log(material.name);
        }
    }
}
