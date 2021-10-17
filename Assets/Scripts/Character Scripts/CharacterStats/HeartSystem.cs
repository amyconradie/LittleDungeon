using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSystem
{
    public const int MAX_FRAGMENT_AMOUNT = 2;
    public int MAX_HEART_AMOUNT;

    public int heartFragmentCount;

    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;
    public event EventHandler OnDead;

    public List<Heart> heartList;



    public int GetCurrentHealth()
    {
        heartFragmentCount = 0;
        foreach (var heart in heartList)
        {
            heartFragmentCount += heart.GetFragmentAmount();
        }

        return heartFragmentCount;
    }

    public HeartSystem(int heartAmount, int maxHearts)
    {
        heartList = new List<Heart>();

        MAX_HEART_AMOUNT = maxHearts;

        if (heartAmount > MAX_HEART_AMOUNT)
        {
            heartAmount = MAX_HEART_AMOUNT;
        }

        for (int i = 0; i < heartAmount; i++)
        {
            //Heart heart = new Heart(MAX_FRAGMENT_AMOUNT);
            //heartList.Insert(0, heart);
            AddHeart(heartList);
        }

        GetCurrentHealth();
    }

    public Heart AddHeart(List<Heart> heartList)
    {
        Heart heart = new Heart(MAX_FRAGMENT_AMOUNT);
        
        
        heartList.Add(heart);
        GetCurrentHealth();

        return heart;
    }

    public List<Heart> GetHeartList()
    {
        return heartList;
    }

    public void DamageHearts(int damageAmount)
    {
        //cycle through all the hearts starting at the end
        for (int i = 0; i < heartList.Count; i++)
        {
            Heart heart = heartList[i];

            if (damageAmount > heart.GetFragmentAmount())
            {
                //// heart cant absorb full damage amount
                //Debug.Log("heart " + i + " took " + heart.GetFragmentAmount() + "damage");

                // damage heart and keep going to next heart until full damage is dealt
                damageAmount -= heart.GetFragmentAmount();

                // remove all health in heart
                heart.Damage(heart.GetFragmentAmount());

                
            }
            else
            {
                //// heart can absorb full damage amount
                //Debug.Log("heart " + i + " took " + heart.GetFragmentAmount() + "damage");

                // heart has enough health to take full damage
                heart.Damage(damageAmount);

                //stop damaging hearts
                break;
            }
        }

        // event listener
        if (OnDamaged != null)
        {
            OnDamaged(this, EventArgs.Empty);
        }

        // event listener
        if (IsDead())
        {
            if (OnDead != null)
            {
                OnDead(this, EventArgs.Empty);
            }
        }

    }

    public void HealHearts(int healAmount)
    {
        for (int i = heartList.Count -1; i >= 0; i--)
        {
            Heart heart = heartList[i];

            int missingFragments = MAX_FRAGMENT_AMOUNT - heart.GetFragmentAmount();

            if (healAmount > missingFragments)
            {
                // heart cannot absorb full healing amount

                // subtract amount of health restored from healAmount
                healAmount -= missingFragments;

                // fully heal heart
                heart.Heal(missingFragments);
            }
            else
            {
                // heart can absorb leftover healing amount
                heart.Heal(healAmount);

                // stop HEALING hearts
                break;
            }
        }

        // event listener
        if (OnHealed != null)
        {
            OnHealed(this, EventArgs.Empty);
        }
    }

    public bool IsDead()
    {
        //return heartList[0].GetFragmentAmount() == 0;
        return GetCurrentHealth() == 0;

    }


    public class Heart
    {
        private int fragments;

        public Heart(int fragments)
        {
            this.fragments = fragments;
        }

        public int GetFragmentAmount()
        {
            return fragments;
        }

        public void SetFragments(int fragments)
        {
            this.fragments = fragments;
        }

        public void Damage(int damageAmount)
        {
            if (damageAmount >= fragments)
            {
                fragments = 0;
            }
            else
            {
                fragments -= damageAmount;
            }
        }

        public void Heal(int healAmount)
        {
            if (fragments + healAmount > MAX_FRAGMENT_AMOUNT)
            {
                fragments = MAX_FRAGMENT_AMOUNT;
            }
            else
            {
                fragments += healAmount;
            }

        }

    }

}
