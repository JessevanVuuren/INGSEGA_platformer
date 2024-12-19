using System.Collections;
using UnityEngine;

public class PlayerGetDMG : MonoBehaviour
{
    public float red;
    public float white;
    public int loop;
    public SpriteRenderer spriteRenderer;
    public GameObject player;

    public void AnimateDMG(float health) {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink() {
        for (int i = 0; i < loop; i++)
        {  
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(red); 

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(white);
        }
    }

    public void ReturnHome() {
        Destroy(player, 3f);
    }

}
