using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
    public class Ingredient{
        public string name;
        public bool cut;
        public bool cook;
    }

    [System.Serializable]
    public class Recipe{
        public string name;
        public List<Ingredient> ingredients;
    }

    [System.Serializable]
    public class RecipeList
    {
        public List<Recipe> recipes;
    }

public class RecipeManager : MonoBehaviour
{
    private Recipe recipe;

    // Start is called before the first frame update
    void Awake()
    {
        SelectRandomRecipe();
        Debug.Log(recipe.name);
    }

    void SelectRandomRecipe(){
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/recipes");
        RecipeList recipeList = JsonUtility.FromJson<RecipeList>(jsonFile.text);
        recipe = recipeList.recipes[Random.Range(0,recipeList.recipes.Count)];
    }
}
