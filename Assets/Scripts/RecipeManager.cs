using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    UIManager uIManager;
    private Recipe recipe;
    List<GameObject> ingredientInPlate = new List<GameObject>();
    AddressableLoader loader;
    GameObject recipeGameObject;
    ParticleSystem pouf;
    JsonManager jsonManager;
    RabbitManager rabbitManager;

    void Awake(){
        loader = gameObject.AddComponent<AddressableLoader>();
        uIManager = canvas.GetComponent<UIManager>();
    }

    public void SelectRandomRecipe(){
        if(jsonManager == null){
            jsonManager = FindObjectOfType<JsonManager>();
        }
        List<Recipe> recipes = jsonManager.GetRecipes();
        recipe = recipes[UnityEngine.Random.Range(0,recipes.Count)];
        SetRecipeGameObjectAndPouf();
        uIManager.SetRecipe(recipe);
    }

    public bool CanAddThisIngrediant(IngredientManager _igredientManager){
        if(recipe == null){return false;}
        foreach(IngredientInRecipe ingredient in recipe.ingredients){
            if(ingredient.name == _igredientManager.GetIngredientName() && ingredient.cook == _igredientManager.GetCook() && ingredient.cut == _igredientManager.GetCut()){
                return true;
            }
        }
        return false;
    }

    public void AddIngrediantToPlate(GameObject _igredientToAdd){
        IngredientManager ingredientManager = _igredientToAdd.GetComponent<IngredientManager>();
        if(CanAddThisIngrediant(ingredientManager)){
           foreach(IngredientInRecipe ingredient in recipe.ingredients){
                if(ingredient.name == ingredientManager.GetIngredientName()){
                    ingredientInPlate.Add(ingredientManager.gameObject);
                    _igredientToAdd.transform.SetParent(gameObject.transform);
                    _igredientToAdd.transform.localPosition = new Vector3(0f,0f,0.00048f);
                    if(ingredientManager.GetIngredientName() == "Cucumber" || ingredientManager.GetIngredientName() == "Tentacle"){
                        _igredientToAdd.transform.localPosition = new Vector3(0f,0f,0.00208f);
                    }
                    if(ingredientManager.GetIngredientName() == "Nori"){
                        _igredientToAdd.transform.localPosition = new Vector3(0f,0f,0.00161f);
                    }
                    uIManager.ClearIngredient(ingredientManager.GetIngredientName());
                    CheckIfRecipeComplete();
                }
           }
        }
    }

    void CheckIfRecipeComplete(){
        if(ingredientInPlate.Count == recipe.ingredients.Count){
            CreateRecipe();
        }
    }

    void SetRecipeGameObjectAndPouf(){
        loader.GetGameObject(recipe.name, (addressableOject) => {
            if(addressableOject != null){
                recipeGameObject = addressableOject;
                Debug.Log("Loaded Item");
            }else{Debug.Log("Item Not Load " + recipe.name);}
        });
        loader.GetGameObject("Pouf", (addressableOject) => {
            if(addressableOject != null){
                GameObject poufGO = Instantiate(addressableOject,gameObject.transform);
                poufGO.transform.localScale = new Vector3(0.28405f,0.28405f,0.28405f);
                poufGO.transform.localPosition = new Vector3(0f,0f,0.00332f);
                pouf = poufGO.GetComponent<ParticleSystem>();
                Debug.Log("Loaded Item");
            }else{Debug.Log("Item Not Load " + recipe.name);}
        });
    }

    void CreateRecipe(){
        foreach(GameObject ingredient in ingredientInPlate){
                Destroy(ingredient);
        }
        pouf.gameObject.GetComponent<AudioSource>().Play();
        pouf.Play();
        recipeGameObject = Instantiate(recipeGameObject, gameObject.transform);
        recipeGameObject.transform.localScale = new Vector3(1f,1f,1f);
        if(recipeGameObject.name == "Food_Roll(Clone)"){
            recipeGameObject.transform.localRotation =  Quaternion.Euler(-90f,0f,0f);
            recipeGameObject.transform.localPosition = new Vector3(0f,-0.00208f,0.00048f);
        } else if(recipeGameObject.name == "Food_SalmonRoll(Clone)"){
            recipeGameObject.transform.localPosition = new Vector3(0f,0f,0.00246f);
            recipeGameObject.transform.localRotation =  Quaternion.Euler(0f,0f,0f);
        }else{
            recipeGameObject.transform.localPosition = new Vector3(0f,0f,0.00048f);
            recipeGameObject.transform.localRotation =  Quaternion.Euler(0f,0f,0f);
        }
        Clear();
    }

    void Clear(){
        recipe = null;
        if(rabbitManager == null){
            rabbitManager = FindObjectOfType<RabbitManager>();
        }
        ingredientInPlate.Clear();
        rabbitManager.Renvoyer();
        StartCoroutine(Coroutine_WaitThenLog(1.5f,EndClear));

    }

    void EndClear(){
        pouf.gameObject.GetComponent<AudioSource>().Play();
        pouf.Play();
        Destroy(recipeGameObject);
    }

    IEnumerator Coroutine_WaitThenLog(float _duration, Action _callback)
    {
        yield return new WaitForSeconds(_duration);
        _callback?.Invoke();
    }
}
