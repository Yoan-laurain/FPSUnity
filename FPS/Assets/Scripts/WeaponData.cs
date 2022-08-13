using UnityEngine;

[CreateAssetMenu(fileName ="WeaponData",menuName = "MyGame/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string name = "Ak47-U";

    public float damage = 10f;

    public float range = 100f;

    public float fireRate = 0f;

    public int magazineSize = 10;

    public float reloadTime = 1f;

    public GameObject graphics;

    public AudioClip shootSound;

    public AudioClip reloadSound;
}
