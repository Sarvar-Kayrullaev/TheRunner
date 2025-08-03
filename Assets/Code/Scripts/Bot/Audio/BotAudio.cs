using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotAudio : MonoBehaviour
    {
        public float Volume = 1;

        public AudioClip[] aaahhh;
        public AudioClip[] shiiit;
        public AudioClip[] oghhh;
        public AudioClip[] ah_sheet;
        public AudioClip[] awww;
        public AudioClip[] i_kill_you_bitch;
        public AudioClip[] i_need_help_guys;
        public AudioClip[] shelter_me;
        public AudioClip[] attack;
        public AudioClip[] shoot;
        public AudioClip[] kill_him;
        public AudioClip[] the_enemy_on_this_side;
        public AudioClip[] oh_shit_dead;
        public AudioClip[] rat_kill_the_man;
        public AudioClip[] what_the;
        public AudioClip[] dead;
        public AudioClip[] oh_you_kidding;
        public AudioClip[] rat_an_around;
        public AudioClip[] the_enemy_is_around;
        public AudioClip[] look_for_the_enemy_guys;

        private float suspected_time;
        private float dead_alarm_time;
        private float enemy_visible_time;
        private float hit_body_time;
        private float dying_time;

        public void Play(PanicTalking panicTalking, BotSetup setup)
        {
            int variant = setup.bot.TalkingSoundVariant;
            if (setup.health.died)
            {
                
                if (panicTalking == PanicTalking.DYING) /* DYING*/
                {
                    float cooldown = Time.time - dying_time;
                    float cooldown2 = Time.time - hit_body_time;
                    if (cooldown < 0.5f || cooldown2 < 1f) return;
                    setup.source.PlayOneShot(oghhh[variant]);
                    dying_time = Time.time;
                }
                return;
            }

            if (panicTalking == PanicTalking.SUSPECTED) /* SUSPECTED*/
            {
                float cooldown = Time.time - suspected_time;
                if (cooldown < 3) return;
                if (setup.memory.allyIsKilled)
                {
                    setup.memory.allyIsKilled = false;
                    return;
                }
                if (setup.memory.NumberOfSuspicions > 1)
                {
                    float random = Random.value;
                    if (random > 0.75) setup.source.PlayOneShot(oh_you_kidding[variant]);
                    else setup.source.PlayOneShot(what_the[variant]);
                }
                else
                {
                    setup.source.PlayOneShot(what_the[variant]);
                }
                suspected_time = Time.time;
            }
            else if (panicTalking == PanicTalking.DEAD_ALARM) /* DEAD ALARM*/
            {
                float cooldown = Time.time - dead_alarm_time;
                if (cooldown < 3) return;
                if (setup.objects.staticEnemy)
                {
                    float random = Random.value;
                    if (random > 0.5) setup.source.PlayOneShot(rat_kill_the_man[variant]);
                    else setup.source.PlayOneShot(shiiit[variant]);
                }
                else
                {
                    float random = Random.value;
                    if (random > 0.25) setup.source.PlayOneShot(oh_shit_dead[variant]);
                    else setup.source.PlayOneShot(ah_sheet[variant]);
                }
                dead_alarm_time = Time.time;
            }
            else if (panicTalking == PanicTalking.ENEMY_VISIBLE) /* ENEMY VISIBLE*/
            {
                float cooldown = Time.time - enemy_visible_time;
                if (cooldown < 3) return;

                if (setup.memory.frightened)
                {
                    float random = Random.value;
                    if (random > 0.8) setup.source.PlayOneShot(shoot[variant]);
                    else if (random > 0.4) setup.source.PlayOneShot(kill_him[variant]);
                    else setup.source.PlayOneShot(attack[variant]);
                }
                else
                {
                    float healthPrecent = setup.health.currentHealth / setup.health.health * 100;
                    if (healthPrecent < 20)
                    {
                        setup.source.PlayOneShot(i_need_help_guys[variant]);
                    }
                    else
                    {
                        float random = Random.value;
                        if (random > 0.5) setup.source.PlayOneShot(the_enemy_on_this_side[variant]);
                        else setup.source.PlayOneShot(i_kill_you_bitch[variant]);
                    }
                }

                enemy_visible_time = Time.time;
            }
            else if (panicTalking == PanicTalking.HIT_BODY) /* HIT BODY*/
            {
                float cooldown = Time.time - hit_body_time;
                if (cooldown < 3) return;
                float random = Random.value;
                if (random > 0.7) setup.source.PlayOneShot(aaahhh[variant]);
                else if (random > 0.4) setup.source.PlayOneShot(ah_sheet[variant]);
                else setup.source.PlayOneShot(awww[variant]);
                hit_body_time = Time.time;
            }

        }
    }

    public enum PanicTalking
    {
        SUSPECTED,
        DEAD_ALARM,
        ENEMY_VISIBLE,
        HIT_BODY,
        DYING
    }
}


