using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IngredientManager : MonoBehaviour
{
    private string ingredientName;
    private bool canBeCut;
    private bool canBeCook;

    private bool isCook;
    private bool isCut;

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
}
