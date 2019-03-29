using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStuff : MonoBehaviour {

    //playerstuff
    public Transform bullet;
    public Transform InstantPoint;

    
    public bool hasWon;
    public bool start;
    public bool go;
    public bool AbleToFire;
    

    public float fireRate;
    private float speed;
    private float fireTime;

 
    public int lives;
    public int score = 0;
    private int timeTillStart;

    private Vector3 origin;
    
    //Enemystuff
    public GameObject EnInst;
    
    //UI elements
    public Text LivesCount;
    public Text LoseText;
    public Text WinText;
    public Text Score;
    public Text finalScore;
    public Text Begin;

    public Button PlayAgain;
    public Button Continue;
    public Button Home;

    public SpriteRenderer Life1;
    public SpriteRenderer Life2;

    //Animation
    Animator anim;



    private void Start()
    {
        origin = transform.position;
        timeTillStart = 3;
        speed = 0.1f;
        lives = 3;
        AbleToFire = true;
        anim = GetComponent<Animator>();
    }

    public IEnumerator Wait(int timer)
    {
        Begin.gameObject.SetActive(true);
        lives = 3;
        LivesCount.text = lives.ToString();

        yield return new WaitForSeconds(1);
        Begin.text = timer.ToString();
        --timer;
        
        StartCoroutine(Wait(timer));
        if (timer == -1)
        {
            start = true;
            Begin.gameObject.SetActive(false);
            StopAllCoroutines();
        }
    }

    

    // Update is called once per frame
    void FixedUpdate ()
    {
        Score.text = score.ToString();

        if (go == false && start == false)//this gets called once
        {
            anim.SetBool("Restart", true);

            StartCoroutine(Wait(timeTillStart));
            
            
            hasWon = false;
            go = true;

            Begin.text = "Ready"; 
            transform.position = origin;
            finalScore.text = null;
            LoseText.gameObject.SetActive(false);
            PlayAgain.gameObject.SetActive(false);
            Home.gameObject.SetActive(false);
            Continue.gameObject.SetActive(false);
            WinText.gameObject.SetActive(false);
            Life1.enabled = true;
            Life2.enabled = true;

        }

        

        else if (hasWon)
        {
            WinText.gameObject.SetActive(true);
            Continue.gameObject.SetActive(true);
            finalScore.color = Color.green;
            finalScore.text = "Score: " + score;
            Home.gameObject.SetActive(true);
            start = false;
            
            EnInst.GetComponent<EnemyInstantiator>().AllStop = true;
        }

        else if (start == true)// need to fix player lives count reset it to 3
        {
            Shoot();

            float moveX = Input.GetAxis("Vertical"); // w and s
            transform.Translate(new Vector3(-moveX, 0, 0) * speed);
            // lives count
            switch (lives) //thought lives.tostring could have been used here it is more inefficient
            {
                case 3:
                    LivesCount.text = "3";  
                    break;
                case 2:
                    LivesCount.text = "2";
                    Life1.enabled = false;
                    break;
                case 1:
                    LivesCount.text = "1";
                    Life2.enabled = false;
                    break;
                case 0:
                    StartCoroutine("DamageAnimation");
                    LivesCount.text = "0";
                    break;

            }
        }
        
	}


    public void Shoot()
    {

        if(Input.GetKey(KeyCode.Space) )
        {
            if(AbleToFire)
            {
                ObjectPooling.Instance.SpawnFromPool("Bullet", InstantPoint.transform.position, Quaternion.Euler(0, 0, 270));
                AbleToFire = false;
            }  
        }   

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "EnemyBullet")
        {
            lives -= 1;
            collision.gameObject.SetActive(false);
            StartCoroutine("DamageAnimation");
        }
    }

    public IEnumerator DamageAnimation()
    {
        

        if (lives ==0 )
        {
            anim.SetBool("IsDead", true);
            EnInst.GetComponent<EnemyInstantiator>().AllStop = true;
            start = false;
            Life1.enabled = false;
            Life2.enabled = false;


            yield return new WaitForSeconds(1f);

            anim.SetBool("Restart", false);
            anim.SetBool("IsDead", false);

            yield return new WaitForSeconds(1f);

            LoseText.gameObject.SetActive(true);
            PlayAgain.gameObject.SetActive(true);
            finalScore.color = Color.red;
            finalScore.text = "Score: " + score;
            Home.gameObject.SetActive(true);
            
            
        }
        else if(lives > 0)
        {
            anim.SetBool("IsDamaged", true);

            yield return new WaitForSeconds(1.2f);

            anim.SetBool("IsDamaged", false);
        }
    }



}
