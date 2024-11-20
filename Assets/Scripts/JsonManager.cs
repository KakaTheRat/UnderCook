using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Rendering;

[System.Serializable]
    public class Ingredient{
        public string name;
        public bool canBeCut;
        public bool canBeCook;
    }

    [System.Serializable]
    public class IngredientInRecipe{
        public string name;
        public bool cut;
        public bool cook;
    }

    [System.Serializable]
    public class Recipe{
        public string name;
        public List<IngredientInRecipe> ingredients;
    }

    [System.Serializable]
    public class Food
    {
        public List<Recipe> recipes;
        public List<Ingredient> ingredients;
    }

public class JsonManager : MonoBehaviour
{
    Food foodInfo;
    // Start is called before the first frame update
    void Awake()
    {
        Addressables.LoadAssetAsync<TextAsset>("Json").Completed += OnJsonLoaded;
    }

    private void OnJsonLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Récupérer le contenu du fichier JSON
            string json = handle.Result.text;

            // Optionnel : Convertir le JSON en un objet C#
            foodInfo = JsonUtility.FromJson<Food>(json);
            Debug.Log($"JSON chargé");
            return;
        }
        Debug.Log($"Error JSON non chargé");
    }

    public Food GetFoodInfo(){
        return foodInfo;
    }

    public List<Ingredient> GetIngredients(){
        return foodInfo.ingredients;
    }

    public List<Recipe> GetRecipes(){
        return foodInfo.recipes;
    }

    public Ingredient GetIngredient(string _name){
        foreach(Ingredient ingredient in foodInfo.ingredients){
            if(ingredient.name == _name){return ingredient;}
        }
        return null;
    }
}
