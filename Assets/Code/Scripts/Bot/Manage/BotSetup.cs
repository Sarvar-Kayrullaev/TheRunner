using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotSetup : MonoBehaviour
    {
        public Bot bot;
        public BotAuthor author;
        public Health health;
        public BotBase memory;
        public BotAction action;
        public BotMovement movement;
        public BotStatus status;
        public BotUtility utility;
        public BotObjects objects;
        public BotSourceOfAction sourceOfAction;
        public BotFieldOfView fieldOfView;
        public BotAttribute attribute;
        public BotWeapon weapon;
        public BotSensor sensor;
        public IndicatorUtility indicatorUtility;
        public AgentUtility agent;
        public MovementUtility movementUtility;
        public BotAudio botAudio;
        public Transform enemyIndicator;
        public Transform enemyDamageNavigator;
        public AudioSource source;
        public OverallController overall;
        public MovePosition movePosition;
        public BotGlobal global;

        public bool initialized = false;

        public void Awake()
        {
            bot.setup = this;
            author.setup = this;
            health.setup = this;
            memory.setup = this;
            action.setup = this;
            movement.setup = this;
            status.setup = this;
            utility.setup = this;
            sourceOfAction.setup = this;
            fieldOfView.setup = this;
            attribute.setup = this;
            weapon.setup = this;
            sensor.setup = this;
            indicatorUtility.setup = this;
            agent.setup = this;
            movementUtility.setup = this;
            initialized = true;
        }

        public void DisableAllComponents()
        {
            botAudio.Play(PanicTalking.DYING, this);
            author.RemoveMarker();
            action.CancelInvoke("CallStaticEnemy");
            //utility.PlayRandomSound(objects.dyingSound, source);
            health.healthBarDestroyTime = 0.5f;
            health.DropWeapon();
            bot.enabled = false;
            action.enabled = false;
            fieldOfView.OnDisable();
            fieldOfView.enabled = false;
            Destroy(fieldOfView);
            movement.enabled = false;
            utility.enabled = false;
            objects.enabled = false;
            sourceOfAction.enabled = false;
            if (enemyIndicator) Destroy(enemyIndicator.gameObject);
            if (enemyDamageNavigator) Destroy(enemyDamageNavigator.gameObject);
            GetComponent<Outline>().enabled = false;
            sensor.gameObject.SetActive(false);
        }
    }

}
