using UnityEngine;
using UnityEngine.UI;

public class PlayerManaBarScript : MonoBehaviour
{
    public Image slider;
    public PlayerStats player;

    public void FixedUpdate()
    {
        SetHealth(player.CurrentMana);
    }

    private void SetHealth(float mana)
    {
        slider.fillAmount = mana / player.MaxMana;
    }
    
}