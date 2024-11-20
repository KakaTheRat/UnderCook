using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    private Recipe recipe;
    List<GameObject> ingredientInPlate;

    // Start is called before the first frame update
    void Awake()
    {
        SelectRandomRecipe();
        Debug.Log(recipe.name);
    }

    void SelectRandomRecipe(){
        List<Recipe> recipes = FindObjectOfType<JsonManager>().GetRecipes();
        recipe = recipes[Random.Range(0,recipes.Count)];
    }

    bool CanAddThisIngrediant(IngredientManager _igredientManager){
        foreach(IngredientInRecipe ingredient in recipe.ingredients){
            if(ingredient.name == _igredientManager.GetIngredentName() && ingredient.cook == _igredientManager.GetCook() && ingredient.cut == _igredientManager.GetCut()){
                return true;
            }
        }
        return false;
    }

    // void AddIngrediantToPlate(IngredientManager _igredientManager){
    //     if(CanAddThisIngrediant(_igredientManager)){
    //        foreach(IngredientInRecipe ingredient in recipe.ingredients){
    //             if(ingredient.name == _igredientManager.GetIngredentName()){
    //                 ingredient.inPlate = true;
    //                 ingredientInPlate.Add(_igredientManager.gameObject);
    //             }
    //        }
    //     }
    // }

    // void CheckIfRecipeComplete(){
    //     bool complete = true;
    //     foreach(IngredientInRecipe ingredient in recipe.ingredients){
    //         if(!ingredient.inPlate){
    //             complete = false;
    //         }
    //     }
    //     if(complete){
    //         CreateRecipe();
    //     }
    // }

    void CreateRecipe(){
        // Il faut :
        //  - Destroy tout les GameObject dans la plate
        //  - Appeller un vfx qui fait "POUF"
        //  - Faire spawn le bon plat (Il va surement falloir que je déplasse tous les ingrédients et plats que j'utilise dans le dossier ressource mais il faut demander a Morgan si c'est ok de faire ça)
        //  - Renvoyer le lapin et en appeller un nouveau (Mais j'ai pas encore de systeme de client)
    }
}
