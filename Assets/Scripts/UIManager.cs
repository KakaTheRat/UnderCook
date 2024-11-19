using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private InputDevice currentDevice;
    private string imgFolder;
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
            interactRawImg.texture = GetInputImg();
        }
    }

    private void UpdateImgFolder(){
        if(currentDevice is Gamepad){
            if (currentDevice.name.Contains("DualShock")){
                imgFolder = "DualShock";
            }else{
                imgFolder = "Xbox";
            }
            imgFolder += "/West";
        }
        else{
            imgFolder = "Keyboard/E";
        }
    }

    public void ToggleInteractText(bool _toggle){
        interactRawImg.enabled = _toggle;
        interactTextMesh.enabled = _toggle;
    }

    private Texture2D GetInputImg(){
        string path = "Controls Icon/" + imgFolder;
        Texture2D img = Resources.Load<Texture2D>(path);
        return img;
    }


}
