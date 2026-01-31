using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    // який режим це розблокуЇ (вибираЇмо в ≤нспектор≥)
    public AnimalType typeToUnlock;

    // ћожна додати ефект при знищенн≥, звук тощо
    public GameObject pickupEffect;

    public void OnDestroy()
    {
        Destroy(pickupEffect);
    }
}
