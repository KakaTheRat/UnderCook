using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader : MonoBehaviour
{
    public void GetGameObject(string addressablePath, System.Action<GameObject> onLoaded)
    {
        // Charger l'asset via Addressables (ici GameObject attendu)
        Addressables.LoadAssetAsync<GameObject>(addressablePath).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Appeler le callback avec le GameObject chargé
                onLoaded?.Invoke(handle.Result);
                Debug.Log("GameObject chargé");
            }
            else
            {
                Debug.LogError($"Erreur lors du chargement du GameObject à l'adresse: {addressablePath}");
                onLoaded?.Invoke(null); // Appeler le callback avec null si une erreur se produit
            }
        };
    }
}
