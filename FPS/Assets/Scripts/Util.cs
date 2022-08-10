using UnityEngine;

public class Util : MonoBehaviour
{
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        // On applique le layer à notre objet
        obj.layer = newLayer;

        //On le fait aussi pour tout ces enfants
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
