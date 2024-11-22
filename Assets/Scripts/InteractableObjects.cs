using UnityEngine;
using System.Collections;
using System;

public class InteractableObjects : MonoBehaviour
{

    public enum Type{
        Food,
        Bin,
        Pot,
        Cut,
        Plate,
        Mirror
    }
    [SerializeField] private string itemName;
    [SerializeField] private Type itemType;
    [SerializeField] private Material mat;
    private string interactionText;
    private AddressableLoader loader;

    [Header("If Food")]
    private GameObject itemToSpawn;
    private bool canBeCut;
    private bool canBeCook;

    private GameObject player;
    private PlayerController playerController;
    private Outline outline;
    private GameObject preparingItem;
    private IngredientManager preparingItemManager;

    private RecipeManager recipeManager;


    void Awake(){
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        loader = gameObject.AddComponent<AddressableLoader>();
        SetLoadedItem();
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.blue;
        outline.OutlineWidth = 0f;
        SetInteractText();
    }

    public string GetName(){
        return itemName;
    }

    public string GetInteractrionText(){
        return interactionText;
    }

    public Type GetItemType(){
        return itemType;
    }

    public bool GetCanBeInteracted(){
        if(itemType == Type.Pot){
            if(preparingItem != null){return false;}
            GameObject holdingItem = playerController.GetHoldingItem();
            if(holdingItem == null){return false;}
            IngredientManager ingredientManager = holdingItem.GetComponent<IngredientManager>();
            if(ingredientManager != null && ingredientManager.GetCanBeCook() && !ingredientManager.GetCook()){
                return true;
            }else{return false;}
        }
        else if(itemType == Type.Bin){
            GameObject holdingItem = playerController.GetHoldingItem();
            if(holdingItem != null){
                return true;
            }
            else{return false;}
        } 
        else if(itemType == Type.Food){
            GameObject holdingItem = playerController.GetHoldingItem();
            if(holdingItem == null){
                return true;
            }else{return false;}
        }
        else if(itemType == Type.Cut){
            if(preparingItem != null){return false;}
            GameObject holdingItem = playerController.GetHoldingItem();
            if(holdingItem == null){return false;}
            IngredientManager ingredientManager = holdingItem.GetComponent<IngredientManager>();
            if(ingredientManager != null && ingredientManager.GetCanBeCut() && !ingredientManager.GetCut()){
                return true;
            }else{return false;}
        }else if(itemType == Type.Plate){
            GameObject holdingItem = playerController.GetHoldingItem();
            if(holdingItem == null){return false;}
            IngredientManager ingredientManager = holdingItem.GetComponent<IngredientManager>();
            return recipeManager.CanAddThisIngrediant(ingredientManager);
        }else if(itemType == Type.Mirror){
            return true;
        }
        return false;
    }

    public void ActivateOutline(bool _activate){
        if(_activate){
            outline.OutlineWidth = 5f;
            return;
        }
        outline.OutlineWidth = 0f;
    }

    public void Interact(){
        if(playerController == null){return;}
        switch(itemType){
            case Type.Food:
                GiveItem();
                break;
            case Type.Bin:
                ThrowToBin();
                break;
            case Type.Pot:
                Cook();
                break;
            case Type.Cut:
                Cut();
                break;
            case Type.Plate:
                AddToPlate();
                break;
            case Type.Mirror:
                Emote();
                break;
        }
    }

    private void GiveItem(){
        GameObject clone = Instantiate(itemToSpawn);
        clone.transform.localScale = transform.localScale; 
        SetInfos();
        IngredientManager ingredientManager = clone.AddComponent<IngredientManager>();
        ingredientManager.SetAttributes(itemName, canBeCut, canBeCook,mat);
        Debug.Log($"lol{mat.name}");   
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        playerController.HoldItem(clone);
    }

    private void Cook(){
        preparingItem = playerController.GetHoldingItem();
        preparingItem.transform.SetParent(gameObject.transform);
        preparingItem.SetActive(false);
        playerController.ReleaseItem();
        GetComponent<Animator>().SetTrigger("Cook");
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        playerController.Static(true);
        outline.OutlineWidth = 0f;
        StartCoroutine(Coroutine_WaitThenLog(2f, EndCook));
    }

    void EndCook(){
        preparingItem.SetActive(true);
        GetComponent<Animator>().SetTrigger("Cook");
        playerController.Static(false);
        preparingItem.GetComponent<IngredientManager>().Cook(itemToSpawn);
        preparingItem.SetActive(true);
        playerController.HoldItem(preparingItem);
        preparingItem = null;
        Debug.Log("FINITO");
    }

    private void Cut(){
        itemToSpawn = Instantiate(itemToSpawn, GameObject.FindGameObjectWithTag("HoldingPlaceHolder").transform);
        itemToSpawn.transform.localPosition =  new Vector3(-0.00016f, 0.00039f, -0.00229f);
        itemToSpawn.transform.localRotation =  Quaternion.Euler(-46.421f, 37.352f, -136.495f);
        itemToSpawn.transform.localScale = new Vector3(1f, 0.8549f,1f);
        preparingItem = playerController.GetHoldingItem();
        playerController.ReleaseItem();
        preparingItem.transform.SetParent(GameObject.FindGameObjectWithTag("CutPlaceHolder").transform);
        preparingItem.transform.localPosition = new Vector3(0f,0f,0f);
        if(preparingItem.name.Contains("Cucumber")){
            preparingItem.transform.localPosition = new Vector3(0f,0.065f,0f); 
        }
        playerController.Static(true);
        playerController.ToggleCutAnim(true);
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        StartCoroutine(Coroutine_WaitThenLog(2f, EndCut));
    }

    private void EndCut()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Stop();
        preparingItem.GetComponent<IngredientManager>().Cut();
        playerController.ToggleCutAnim(false);
        playerController.Static(false);
        Destroy(itemToSpawn);
        SetLoadedItem();
        playerController.HoldItem(preparingItem);
        preparingItem = null;
    }

    IEnumerator Coroutine_WaitThenLog(float _duration, Action _callback)
    {
        yield return new WaitForSeconds(_duration);
        _callback?.Invoke();
    }

    private void AddToPlate(){
        recipeManager.AddIngrediantToPlate(playerController.GetHoldingItem());
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        playerController.ReleaseItem();
    }

    private void ThrowToBin(){
        playerController.DestroyHoldingItem();
    }

    private void SetInfos(){
        Ingredient ingredientInfo = FindObjectOfType<JsonManager>().GetIngredient(itemName);
        if(ingredientInfo == null){return;}
        canBeCook = ingredientInfo.canBeCook;
        canBeCut = ingredientInfo.canBeCut;
    }

    void SetLoadedItem(){
        if(itemType == Type.Bin || itemType == Type.Mirror){return;}
        if(itemType == Type.Plate){recipeManager = GetComponent<RecipeManager>(); return;}
        loader.GetGameObject(itemName, (addressableOject) => {
            if(addressableOject != null){
                itemToSpawn = addressableOject;
                Debug.Log("Loaded Item");
            }else{Debug.Log("Item Not Load " + itemName);}
        });
    }

    public void SetInteractText(){
        GameObject playerHolding = playerController.GetHoldingItem();
        switch(itemType){
            case Type.Food:
                interactionText = $"take {itemName}" ;
                break;
            case Type.Mirror:
                interactionText = "Emote" ;
                break;
            case Type.Bin:
            if(playerHolding != null){
                interactionText = $"throw the {playerHolding.GetComponent<IngredientManager>().GetIngredientName()}";
            }
            break;
            case Type.Cut:
            if(playerHolding != null){
                interactionText = $"cut the {playerHolding.GetComponent<IngredientManager>().GetIngredientName()}";
            }
                break;
            case Type.Pot:
            if(playerHolding != null){
                interactionText = $"cook your {playerHolding.GetComponent<IngredientManager>().GetIngredientName()}";
            }
                break;
            case Type.Plate:
            if(playerHolding != null){
                interactionText = $"put the {playerHolding.GetComponent<IngredientManager>().GetIngredientName()} on the plate";
            }
                break;
        }
    }

    void Emote(){
        playerController.Emote();
    }

}
