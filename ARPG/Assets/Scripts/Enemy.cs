using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public float moveSpeed;
    public float attackRange;

    public bool hasLightAttacks;
    public bool hasHeavyAttacks;
    public bool hasGuard;
    private bool[] abilities;

    // Start is called before the first frame update
    void Start()
    {
        abilities = new[] {hasGuard, hasHeavyAttacks, hasLightAttacks};
    }

    // Update is called once per frame
    void Update()
    {
        bool inDistance = Vector3.Distance(transform.position, target.transform.position) < attackRange;

        if (inDistance)
        {
            StartCoroutine(SelectedAttack());
        }
        else
        {
            EnemyMovement();
        }
    }

    private void EnemyMovement()
    {
        //paste movement code for the enemy here so he can be interrupted :)
        transform.position =  Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);;
    }

    IEnumerator SelectedAttack()
    {
        var enabledAbilities = 0;
        var checkRolledNumbers = 0;
        var selectedAbility = new bool[3];
        foreach (var t in abilities)
        {
            if (t)
            {
                enabledAbilities++;
            }
        }

        var rolledNumber = Random.Range(0, enabledAbilities);
        for (var i = 0; i < abilities.Length; i++)
        {
            if (abilities[i])
            {
                if (rolledNumber == checkRolledNumbers)
                {
                    selectedAbility[i] = abilities[i];
                }
                else
                {
                    checkRolledNumbers++;
                }
            }
        }

        if (selectedAbility[0])
            return CO_EnemyGuard();
        if (selectedAbility[1])
            return CO_EnemyHeavyAttack();
        if (selectedAbility[2])
            return CO_EnemyLightAttack();
        return null;
    }

    private IEnumerator CO_EnemyLightAttack()
    {
        yield return null;
    }

    private IEnumerator CO_EnemyHeavyAttack()
    {
        yield return null;
    }

    private IEnumerator CO_EnemyGuard()
    {
        yield return null;
    }
}