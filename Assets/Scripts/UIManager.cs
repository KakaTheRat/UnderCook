using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

    [SerializeField] private List<Texture2D> inputTextures;
    private InputDevice currentDevice;
    private Texture2D inputTexture;
    private TMP_Text interactTextMesh;
    private RawImage interactRawImg; 

    void Awake()
    {
        GameObject child = transform.GetChild(0).gameObject;
        for(int i = 0; i < child.transform.childCount;i++){
            GameObject underChild = child.transform.GetChild(i).gameObject;
            TMP_Text txt = underChild.GetComponent<TMP_Text>();
            RawImage img = underChild.GetComponent<RawImage>();
            if(txt != null){interactTextMesh = txt;}
            if(img != null){interactRawImg = img;}
        }
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


}
