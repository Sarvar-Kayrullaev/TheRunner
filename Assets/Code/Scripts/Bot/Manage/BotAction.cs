using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotAction : MonoBehaviour
    {
        [HideInInspector] public BotSetup setup;
        void Start()
        {
            
        }

        public void Main()
        {
            SenseBeforePurpose();

            if (setup.status.Purpose == BotEnum.Purpose.Alarm)
            {
                //status.messages[0].text = "ALARM";
                setup.author.Message("ALARM");
            }
            else if (setup.status.Purpose == BotEnum.Purpose.Attacking)
            {
                setup.sourceOfAction.Attacking();
                //setup.author.message.text = "ATTACKING";
                setup.author.Message("ATTACK");
            }
            else if (setup.status.Purpose == BotEnum.Purpose.Avoid)
            {
                setup.sourceOfAction.Avoid();
                //status.messages[0].text = "AVOID";
                setup.author.Message("AVOID");
            }
            else if (setup.status.Purpose == BotEnum.Purpose.Checking)
            {
                //Progress...
                setup.sourceOfAction.Checking();
                //status.messages[0].text = "CHECKING";
                setup.author.Message("CHECKING");
            }
            else if (setup.status.Purpose == BotEnum.Purpose.Patrolling)
            {
                setup.author.Message("PATROLLING");
                setup.sourceOfAction.Patrolling();
                //status.messages[0].text = "PATROLLING";
            }
            else
            {
                Command();
                //status.messages[0].text = "COMMAND";
                setup.author.Message("COMMAND");
            }
        }
        void Command()
        {
            if (setup.status.Command == BotEnum.Command.Guarding)
            {
                setup.sourceOfAction.Guarding();
            }
            else if (setup.status.Command == BotEnum.Command.Working)
            {

            }
            else if (setup.status.Command == BotEnum.Command.Delivery)
            {

            }
            else if (setup.status.Command == BotEnum.Command.Attacking)
            {

            }
            else
            {
                // Special or other
            }
        }
        void SenseBeforePurpose()
        {
            if (!setup.objects.enemy) setup.utility.IdentifyEnemyTime();
            setup.utility.GetPanic();
            if (setup.status.MentalState == BotEnum.MentalState.Panic)
            {
                if (setup.objects.enemy)
                {
                    setup.objects.lastEnemy = setup.objects.enemy;
                    setup.objects.ChangeSuspectionPoint(setup.objects.enemy.position);
                    if (setup.health.currentHealth < setup.health.health / 3)
                    {
                        if (setup.sourceOfAction.accesToHiding)
                        {
                            setup.status.Purpose = BotEnum.Purpose.Avoid;
                            //status.Purpose = BotEnum.Purpose.Attacking;
                        }
                        else
                        {
                            setup.status.Purpose = BotEnum.Purpose.Attacking;
                        }
                    }
                    else
                    {
                        setup.status.Purpose = BotEnum.Purpose.Attacking;
                    }

                    if (!setup.memory.EnemyPassed)
                    {
                        Invoke(nameof(CallStaticEnemy), setup.attribute.enemyPassTime);
                        setup.memory.EnemyPassed = true;
                    }
                }
                else
                {
                    if (setup.objects.staticEnemy)
                    {
                        if (setup.health.currentHealth < setup.health.health / 3)
                        {
                            if (setup.status.Purpose != BotEnum.Purpose.Patrolling)
                            {
                                if (setup.sourceOfAction.accesToHiding)
                                {
                                    setup.status.Purpose = BotEnum.Purpose.Avoid;
                                }
                                else
                                {
                                    setup.status.Purpose = BotEnum.Purpose.Attacking;
                                }
                            }
                        }
                        else
                        {
                            if (setup.status.Purpose != BotEnum.Purpose.Patrolling)
                            {
                                setup.status.Purpose = BotEnum.Purpose.Attacking;
                            }
                        }
                    }
                    else
                    {
                        setup.status.Purpose = BotEnum.Purpose.Patrolling;
                    }
                }
            }
            else if (setup.status.MentalState == BotEnum.MentalState.Suspicion)
            {
                setup.status.Purpose = BotEnum.Purpose.Checking;
            }
            else
            {
                setup.status.Purpose = BotEnum.Purpose.Command;
            }
        }

        //Invoke

        void CallStaticEnemy()
        {
            if (setup.health.died) return;
            //setup.overall.SetDangerAreas(setup.objects.lastEnemy.position, 30);
            setup.utility.CallStaticEnemy(setup.objects.lastEnemy);
            setup.botAudio.Play(PanicTalking.ENEMY_VISIBLE, setup);
            print("Call");
        }
    }

}
