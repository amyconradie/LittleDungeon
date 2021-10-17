using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationSelect : MonoBehaviour
{
    private Animator anim;
    public string trigger;
    public string anim_index;
    public int seconds_between_animations;
    public int number_of_animations;


    IEnumerator Start()
    {
        this.anim = GetComponent<Animator>();

        while (true)
        {
            yield return new WaitForSeconds(seconds_between_animations);

            anim.SetInteger(anim_index, Random.Range(0, number_of_animations + 1));
            anim.SetTrigger(trigger);
        }
    }
}
