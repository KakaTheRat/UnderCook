using UnityEngine;
using System.Threading.Tasks;

public class InteractableObjects : MonoBehaviour
{

    public enum Type{
        Food,
        Bin,
        Pot,
        Cut,
        Plate
    }
    [SerializeField] private string itemName;
    [SerializeField] private Type itemType;
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
        }
    }

    private void GiveItem(){
        GameObject clone = Instantiate(itemToSpawn);
        clone.transform.localScale = transform.localScale; 
        SetInfos();
        IngredientManager ingredientManager = clone.AddComponent<IngredientManager>();
        ingredientManager.SetAttributes(itemName, canBeCut, canBeCook);
        playerController.HoldItem(clone);
    }

    private async void Cook(){
        preparingItem = playerController.GetHoldingItem();
        preparingItem.transform.SetParent(gameObject.transform);
        Vector3 previousScale = preparingItem.transform.localScale;
        preparingItem.transform.localPosition = new Vector3(0f,0f,0f);
        preparingItem.transform.localScale = new Vector3(0f,0f,0f);
        playerController.ReleaseItem();
        GetComponent<Animator>().SetTrigger("Cook");
        playerController.Static(true);
        outline.OutlineWidth = 0f;
        await Task.Delay(2000);
        GetComponent<Animator>().SetTrigger("Cook");
        playerController.Static(false);
        preparingItem.GetComponent<IngredientManager>().Cook(itemToSpawn);
        preparingItem.transform.localScale = previousScale;
        playerController.HoldItem(preparingItem);
        preparingItem = null;
        Debug.Log("FINITO");
    }

    private async void Cut(){
        GameObject knife = Instantiate(itemToSpawn, GameObject.FindGameObjectWithTag("HoldingPlaceHolder").transform);
        knife.transform.localPosition =  new Vector3(-0.00016f, 0.00039f, -0.00229f);
        knife.transform.localRotation =  Quaternion.Euler(-46.421f, 37.352f, -136.495f);
        knife.transform.localScale = new Vector3(1f, 0.8549f,1f);
        preparingItem = playerController.GetHoldingItem();
        playerController.ReleaseItem();
        preparingItem.transform.SetParent(GameObject.FindGameObjectWithTag("CutPlaceHolder").transform);
        preparingItem.transform.localPosition = new Vector3(0f,0f,0f);
        if(preparingItem.name.Contains("Cucumber")){
            preparingItem.transform.localPosition = new Vector3(0f,0.065f,0f); 
        }
        playerController.Static(true);
        playerController.ToggleCutAnim(true);
        await Task.Delay(2000);
        preparingItem.GetComponent<IngredientManager>().Cut();
        playerController.ToggleCutAnim(false);
        playerController.Static(false);
        Destroy(knife);
        playerController.HoldItem(preparingItem);
        preparingItem = null;
    }

    private void AddToPlate(){
        recipeManager.AddIngrediantToPlate(playerController.GetHoldingItem());
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
        if(itemType == Type.Bin){return;}
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

}
