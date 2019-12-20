using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("---- Audio ----")]
    [SerializeField]
    protected AudioClip gunShotA;

    // -- System ---
    protected float damage = 40f; // defualt 40
    protected float speed = 12f;
    private int liveTime = 3;
    private Vector3 direction;

    // ########################################### GETS & SETS
    public float BulletSpeed
    {
        set{ speed = value; }
    }
    public float Damage
    {
        set { damage = value; }
        get { return damage; }
    }

    // ########################################### INI
    public void IniBullet(Vector3 _direction, AudioSource sfxSource)
    {
        //speed = _speed;
        //StartCoroutine(Updater(direction));
        StartCoroutine(LifeTime());
        direction = _direction;
        PlayAudio(sfxSource); // play gunshot audio
    }

    // ########################################## SYSTEM

    void FixedUpdate()
    {
       // Debug.Log("Direction: " + direction + " * timeDelta" + Time.deltaTime +  " * speed: " + speed + " = " + (direction * Time.deltaTime * speed));
        transform.Translate(direction * Time.deltaTime * speed);
    }

    /// <summary>
    /// live for a time and then destroy this obj
    /// </summary>
    /// <returns></returns>
    private IEnumerator LifeTime()
    {
        float count = 0f;
        while (count < liveTime)
        {
            count += 0.02f;
            yield return new WaitForSeconds(0.03f);
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider col) // bullet collides
    {
        if(col.tag == "FloorSpawn") // hit wall or floor
        {
            // play sound on hit
            Destroy(this.gameObject);
        }

        if (col.tag == "NPC") // hit NPC
        {
            //Debug.Log("Hit NPC : " + damage);
            EnemyNPC npc = col.GetComponent<EnemyNPC>();
            npc.GetShot(damage);

            Destroy(this.gameObject);
        }

        if(col.tag == "Player")
        {
           // Debug.Log("Player hit");
            PlayerManager play = col.GetComponent<PlayerManager>();
            play.TakeDMG(damage);

            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Play gunshot audio 
    /// </summary>
    private void PlayAudio(AudioSource sfxSource)
    {
        sfxSource.clip = gunShotA;
        sfxSource.Play();
    }
}
