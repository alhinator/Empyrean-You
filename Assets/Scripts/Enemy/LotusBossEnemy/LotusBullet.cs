using UnityEngine;

public class LotusBullet : MonoBehaviour {
    public Petal associatedPetal;

    public static LotusBullet createAsChildOf(LotusBullet prefab, LotusBossEnemy boss, Petal petal) {
        LotusBullet next = GameObject.Instantiate(prefab, boss.transform);
        next.gameObject.SetActive(false);
        next.associatedPetal = petal;
        return next;
    }
}
