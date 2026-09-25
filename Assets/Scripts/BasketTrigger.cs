using UnityEngine;

public class BasketTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрушка (по тегу или слою)
        if (other.CompareTag("Toy") || other.gameObject.layer == LayerMask.NameToLayer("Toy"))
        {
            // Сообщаем ClawController, что игрушка в корзине
            ClawController claw = FindObjectOfType<ClawController>();
            if (claw != null)
            {
                claw.OnToyLandedInBasket(other.gameObject);
            }
        }
    }
}