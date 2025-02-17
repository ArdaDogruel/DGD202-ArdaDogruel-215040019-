using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    WeaponBase weapon;

    public float attackArea = 0.8f;
    Vector3 direction;
    float speed;
    int damage = 5;
    int numOfHits = 1;

    List<IDamageable> enemiesHit;

    [SerializeField] float ttl;

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y);

        if (dir_x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = scale.x * -1;
            transform.localScale = scale;
        }
    }

    void Update()
    {
        Move();

        if (Time.frameCount % 3 == 0)
        {
            HitDetection();
        }

        TimerToLive();
    }

    private void TimerToLive()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0f)
        {
            Destroy(gameObject);
        }
    }

    private void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D c in hit)
        {
            if (numOfHits > 0)
            {
                IDamageable enemy = c.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    if (CheckRepeatHit(enemy) == false)
                    {
                        weapon.ApplyDamage(c.transform.position, damage, enemy);
                        enemiesHit.Add(enemy);
                        numOfHits -= 1;
                    }
                }
            }
            else
            {
                break;
            }
        }

        if (numOfHits <= 0)
        {
            Destroy(gameObject);
        }
    }

    private bool CheckRepeatHit(IDamageable enemy)
    {
        if (enemiesHit == null) { enemiesHit = new List<IDamageable>(); }

        return enemiesHit.Contains(enemy);
    }

    private void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void PostDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMassage(damage.ToString(), worldPosition);
    }

    public void SetStats(WeaponBase weaponBase)
    {
        weapon = weaponBase;
        damage = weaponBase.GetDamage();
        numOfHits = weaponBase.weaponStats.numberOfHits;
        speed = weaponBase.weaponStats.projectileSpeed;

    }
}
