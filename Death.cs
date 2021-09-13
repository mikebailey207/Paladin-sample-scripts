using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class Death : MonoBehaviour
{
    public static Death d;

    float turnAngle;
    Rigidbody2D rb;
    bool dying = false;
    AudioSource deathSound;
    bool canPlayDeath = true;
    public GameObject deathCanvas;
    
    int i = 0;
    [SerializeField]
    TextMeshProUGUI tipText;
    [SerializeField]
    string[] tips;
 
    private void Start()
    {
        d = this;
        deathSound = GameObject.Find("DeathSound").GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        turnAngle = transform.rotation.z;

        if (turnAngle <= 0)
        {           
            Die();
        }
      
        if (dying) transform.Rotate(0, 0, 500 * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Death"))
        {
            Die();
        }
    }
    public void Die() 
    {
        PlayerController.pc.started = false;
        if (canPlayDeath)
        {
            GenerateTip();
            deathSound.Play();
            canPlayDeath = false;
        }
        StartCoroutine(WaitToDie());
    }

    void GenerateTip()
    {
        i = Random.Range(0, tips.Length + 1);
        tipText.text = tips[i];
    }

    IEnumerator WaitToDie()
    {
        rb.gravityScale = 1;

        dying = true;
        
        rb.constraints = RigidbodyConstraints2D.None;
        yield return new WaitForSeconds(1.5f);
        deathCanvas.SetActive(true);
    
    }
}
