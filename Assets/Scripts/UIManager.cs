using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

    [SerializeField] private List<Texture2D> inputTextures;
    [SerializeField] private TMP_Text interactTextMesh;
    [SerializeField] private RawImage interactRawImg;
    [SerializeField] private TMP_Text RecipeName;
    [SerializeField] private GameObject IngredientInfo;
    [SerializeField] private GameObject panel;
    private InputDevice currentDevice;
    private Texture2D inputTexture;
    private List<GameObject> ingredientsTexts = new List<GameObject>();

    void Awake()
    {
        ToggleInteractText(false);
        UpdateImgFolder();
    }

    public void SetInteractText(string _txt){
        interactTextMesh.text = _txt;
    }

    public void GetDeviceType(InputAction.CallbackContext context){
        if (!context.performed) return;
        var contextDevice = context.control.device;
        if (currentDevice != contextDevice)
        {
            currentDevice = contextDevice;
            UpdateImgFolder();
            interactRawImg.texture = inputTexture;
        }
    }

    private void UpdateImgFolder(){
        if(currentDevice is Gamepad){
            if (currentDevice.name.Contains("DualShock")){
                inputTexture = inputTextures[0];
            }else{
                inputTexture = inputTextures[1];
            }
        }
        else{
            inputTexture = inputTextures[2];
        }
    }

    public void ToggleInteractText(bool _toggle){
        interactRawImg.enabled = _toggle;
        interactTextMesh.enabled = _toggle;
    }

    public void SetRecipe(Recipe recette){
        RecipeName.text = recette.name;
        foreach(IngredientInRecipe ingredient in recette.ingredients){
            GameObject newIngredientInfo = Instantiate(IngredientInfo, panel.transform);
            for(int i = 0; i < newIngredientInfo.transform.childCount; i++){
                TMP_Text tmp_text = newIngredientInfo.transform.GetChild(i).GetComponent<TMP_Text>();
                if(tmp_text.text == "Ingredient"){
                    tmp_text.text = ingredient.name;
                } else if(tmp_text.text == "Cut"){
                    string cross = "";
                    if(ingredient.cut){cross = "X";}
                    tmp_text.text = cross;
                } else{
                    string cross = "";
                    if(ingredient.cook){cross = "X";}
                    tmp_text.text = cross;
                }
            }
            ingredientsTexts.Add(newIngredientInfo);
        }
        panel.SetActive(true);
    }

    public void ClearIngredient(string ingredientName){
        foreach(GameObject ingredientsText in ingredientsTexts){
            for(int i = 0; i < ingredientsText.transform.childCount; i++){
                TMP_Text tmp_text = ingredientsText.transform.GetChild(i).GetComponent<TMP_Text>();
                if(tmp_text.text == ingredientName){
                    ingredientsTexts.Remove(ingredientsText);
                    Destroy(ingredientsText);
                    if(ingredientsTexts.Count == 0){
                        panel.SetActive(false);
                    }
                    return;
                }
            }
        }
    }
}
