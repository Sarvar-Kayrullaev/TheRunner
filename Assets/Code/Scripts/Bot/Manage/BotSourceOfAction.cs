using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
namespace BotRoot
{
	public class BotSourceOfAction : MonoBehaviour
	{
		[HideInInspector] public BotSetup setup;
		BotAction action;
		BotUtility utility;
		BotObjects objects;
		BotMovement movement;

		void Awake()
		{
			action = setup.action;
			utility = setup.utility;
			objects = setup.objects;
			movement = setup.movement;
		}
		//--- GUARDING ATTRIBUTES ---//
		[Header("Guarding Attributes")]
		[SerializeField] float averageGuardingIdleTime = 10;
		float currentGuardingIdleTime;

		[Header("Checking Attributes")]
		[SerializeField] float checkingIdleTime = 10;
		float currentCheckingTime;

		[Header("Hiding Attributes")]
		[SerializeField] float hidingTime = 10;
		float currentHidingTime;
		[HideInInspector] public bool accesToHiding = true;

		[Header("Patrolling Attributes")]
		[SerializeField] float patrollingIdleTime = 10;
		float currentPatrollingTime;

		public void Guarding()
		{
			if (currentGuardingIdleTime <= 0)
			{
				utility.ChangeGuardingPoint();
				setup.movePosition.SetPosition(objects.currentGuardingPoint.position);
				currentGuardingIdleTime = Random.Range(averageGuardingIdleTime - (averageGuardingIdleTime / 2), averageGuardingIdleTime + (averageGuardingIdleTime / 2));
			}
			else
			{
				float guardingPointDistance = Vector3.Distance(objects.root.position, objects.currentGuardingPoint.position);
				if (guardingPointDistance > 1.5f)
				{
					//LookAndWalk//
					movement.Move(objects.movePoint);
				}
				else
				{
					currentGuardingIdleTime -= Time.deltaTime;
					setup.movementUtility.WalkToStop();
					//MoveToStop
				}
			}
		}

		public void Checking()
		{
			bool moveToPoint = setup.memory.MoveToWonderingPoint;
			bool moveToPointFoce = setup.memory.ForceMoveToWonderingPoint;
			bool moveToWonderingPointNotNecessarily = setup.memory.NotNecessarilyMoveToWonderingPoint;
			int numberOfSuspicions = setup.memory.NumberOfSuspicions;

			if (moveToPointFoce) MoveToWonderingPoint();
			else if (moveToWonderingPointNotNecessarily) LookToWonderingPoint();
			else if (moveToPoint && numberOfSuspicions < 5) MoveToWonderingPoint();
			else LookToWonderingPoint();
		}

		void MoveToWonderingPoint()
		{
			currentGuardingIdleTime = 0;

			setup.movePosition.SetPosition(objects.suspicionPoint.position);
			float suspicionPointDistance = Vector3.Distance(objects.root.position, objects.suspicionPoint.position);

			if (suspicionPointDistance > 1.5f)
			{
				//LookAndWalk//
				movement.Move(objects.movePoint);
				currentCheckingTime = checkingIdleTime;
			}
			else
			{
				if (suspicionPointDistance <= 1.5f)
				{
					currentCheckingTime -= Time.deltaTime;
					if (currentCheckingTime <= 0)
					{
						setup.utility.ChangeGuardingPoint();
						setup.movement.animator.SetFloat("Crouch", 0);
						setup.status.MentalState = BotEnum.MentalState.Quiet;
						//End Checking Purpose
					}

				}
				setup.movementUtility.WalkToStop();
				//MoveToStop
			}
		}

		bool lookWonderingTemp = true;
		void LookToWonderingPoint()
		{
			float angle = utility.Angle(objects.root, objects.suspicionPoint);
			if (lookWonderingTemp)
			{
				currentCheckingTime = checkingIdleTime;
				lookWonderingTemp = false;
			}
			else
			{
				currentCheckingTime -= Time.deltaTime;
				if (currentCheckingTime <= 0)
				{
					lookWonderingTemp = true;
					setup.utility.ChangeGuardingPoint();
					setup.movement.animator.SetFloat("Crouch", 0);
					setup.movementUtility.CrossFade("Short Idle", true);
					setup.status.MentalState = BotEnum.MentalState.Quiet;
				}
				movement.CheckingIdle();
			}
		}

		public void Attacking()
		{
			currentGuardingIdleTime = 0;
			if (objects.enemy)
			{
				setup.weapon.Shooting();
			}

			if (setup.status.assaultCommand == BotEnum.AssaultCommand.Avoid)
			{
				//setup.author.Message("ATK: Avoid");
				Avoid();
				//Attack();
			}
			else
			{
				//setup.author.Message("ATK: Front");
				Attack();
				//setup.status.messages[0].text = "ATTACK: Attacking";
			}
		}


		public void Avoid()
		{
			currentGuardingIdleTime = 0;
			if (objects.hidingPoint)
			{
				setup.movePosition.SetPosition(objects.hidingPoint.position);
				float pointDistance = Vector3.Distance(objects.root.position, objects.hidingPoint.position);

				if (pointDistance > 0.5f)
				{
					//LookAndWalk//

					setup.movement.animator.SetFloat("Crouch", 1);
					movement.Move(objects.movePoint);
					currentHidingTime = Random.Range(hidingTime / 2, hidingTime + (hidingTime / 2));
				}
				else
				{
					if (pointDistance <= 1.5f)
					{
						currentHidingTime -= Time.deltaTime;
						setup.author.Message("Hiding Time: " + currentHidingTime);
						if (currentHidingTime <= 0)
						{
							accesToHiding = false;
							if (setup.overall.IsAreaSafe())
							{
								//If Area Safe
								setup.movement.animator.SetFloat("Crouch", 0);
								objects.patrolling = null;
								setup.status.MentalState = BotEnum.MentalState.Quiet;
								setup.status.Purpose = BotEnum.Purpose.Command;
								setup.status.assaultCommand = BotEnum.AssaultCommand.Point;
							}
							else
							{
								//If Area Not Safe

								if (setup.overall.IsLastDiedPointChecked && setup.overall.IsLastEnemyPointChecked) //If Last Enemy Position Checked
								{
									PatrollingPoint patrollingPoint = setup.overall.FindClosestPoint(setup.transform, objects.suspicionPoint.position);
									if (patrollingPoint)
									{
										objects.patrolling = patrollingPoint;
										setup.status.Purpose = BotEnum.Purpose.Patrolling;
										setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
										objects.patrolling.SetIsBusy();
									}
									else
									{
										objects.patrolling = null;
										setup.status.Purpose = BotEnum.Purpose.Patrolling;
										setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
									}
								}
								else
								{
									if (setup.overall.CountOfPatrollingMainPoint() > 0)
									{
										objects.patrolling = null;
										setup.status.Purpose = BotEnum.Purpose.Patrolling;
										setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
									}
									else
									{
										objects.patrolling = null;
										setup.status.Purpose = BotEnum.Purpose.Patrolling;
										setup.status.patrollCommand = BotEnum.PatrollCommand.MainPoint;
									}
								}
							}

							if (objects.hidingPoint)
							{
								if (objects.hidingPoint.parent.parent.TryGetComponent(out HidingPoints hidingPoint)) hidingPoint.isBusy = false;
							}

							objects.hidingPoint = null;
							//End Hiding Purpose
						}
						else
						{
							// Idle Loop

							if (setup.memory.suspectionID != setup.memory.currentSuspectionID)
							{
								if (objects.enemy)
								{
									setup.status.Purpose = BotEnum.Purpose.Attacking;
									utility.CommandFront(0);
									movement.AttackingIdle();
								}
								else
								{

									//setup.status.Purpose = BotEnum.Purpose.Checking;
								}
								//utility.AccessToHiding(true,10);
							}
							else
							{
								if (objects.enemy)
								{
									setup.status.Purpose = BotEnum.Purpose.Attacking;
									utility.CommandFront(0);
									movement.AttackingIdle();
								}
							}
						}
					}
					setup.movementUtility.WalkToStopCrouch();
					//MoveToStop
				}
			}
			else
			{
				objects.hidingPoint = utility.GetHidingPoint();
				accesToHiding = false;
			}
		}
		public void Attack()
		{
			currentGuardingIdleTime = 0;

			if (objects.enemy)
			{
				setup.movement.animator.SetFloat("Crouch", 0);
				setup.movePosition.SetPosition(objects.enemy.position);
				movement.Move(objects.movePoint);
			}
			else
			{
				setup.movement.animator.SetFloat("Crouch", 0);
				setup.movePosition.SetPosition(objects.suspicionPoint.position);
				//setup.author.Message("SusPoint: " + objects.suspicionPoint.position, 12);
				float suspectionPointDistance = Vector3.Distance(objects.root.position, setup.movePosition.GetPosition());

				if (suspectionPointDistance > 1.5f)
				{
					movement.Move(objects.movePoint);
				}
				else
				{
					setup.status.Purpose = BotEnum.Purpose.Patrolling;
				}
			}
		}

		public void Patrolling()
		{
			if (setup.status.patrollCommand == BotEnum.PatrollCommand.MainPoint)
			{
				PatrollMainPoint();
				setup.author.Message("Patrolling: Main Point");
			}
			else if (setup.status.patrollCommand == BotEnum.PatrollCommand.PatrollPoint)
			{
				PatrollPoint();
				setup.author.Message("Patrolling: Patroll Point");
			}
			else
			{
				Avoid();
				setup.author.Message("Patrolling: Avoid");
			}
		}

		void PatrollMainPoint()
		{
			Debugging("------------ Main Point");
			currentGuardingIdleTime = 0;

			if (setup.overall.IsLastEnemyPointChecked)
			{
				if (setup.overall.IsLastDiedPointChecked)
				{
					if (setup.overall.IsAreaSafe())
					{
						setup.movement.animator.SetFloat("Crouch", 0);
						setup.status.MentalState = BotEnum.MentalState.Quiet;
						setup.status.Purpose = BotEnum.Purpose.Command;
					}
					else
					{
						setup.status.MentalState = BotEnum.MentalState.Panic;
						setup.status.Purpose = BotEnum.Purpose.Patrolling;
						setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
					}
				}
				else
				{
					setup.movePosition.SetPosition(setup.overall.lastDangerPoint);
					float lastDangerDistance = Vector3.Distance(objects.root.position, setup.movePosition.GetPosition());

					if (lastDangerDistance > 1.5f)
					{
						//LookAndWalk//
						movement.Move(objects.movePoint);
						currentPatrollingTime = Random.Range(patrollingIdleTime / 2, patrollingIdleTime);
					}
					else
					{
						if (lastDangerDistance <= 1.5f)
						{
							currentPatrollingTime -= Time.deltaTime;
							if (currentPatrollingTime <= 0)
							{
								if (setup.overall.IsAreaSafe())
								{
									setup.movement.animator.SetFloat("Crouch", 0);
									setup.status.MentalState = BotEnum.MentalState.Quiet;
									setup.status.Purpose = BotEnum.Purpose.Command;
								}
								else
								{
									setup.status.MentalState = BotEnum.MentalState.Panic;
									setup.status.Purpose = BotEnum.Purpose.Patrolling;
									setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
								}
								//End Checking Purpose
							}
						}
						setup.movementUtility.WalkToStopCrouch();
						setup.overall.IsLastDiedPointChecked = true;
						//MoveToStop
					}
				}
			}
			else
			{
				setup.movePosition.SetPosition(setup.overall.lastEnemyPoint);
				float lastEnemyDistance = Vector3.Distance(objects.root.position, setup.movePosition.GetPosition());

				if (lastEnemyDistance > 1.5f)
				{
					//LookAndWalk//
					movement.Move(objects.movePoint);
					currentPatrollingTime = Random.Range(patrollingIdleTime / 2, patrollingIdleTime);
				}
				else
				{
					if (lastEnemyDistance <= 1.5f)
					{
						currentPatrollingTime -= Time.deltaTime;
						if (currentPatrollingTime <= 0)
						{
							if (setup.overall.IsLastDiedPointChecked)
							{
								if (setup.overall.IsAreaSafe())
								{
									setup.movement.animator.SetFloat("Crouch", 0);
									setup.status.MentalState = BotEnum.MentalState.Quiet;
									setup.status.Purpose = BotEnum.Purpose.Command;
								}
								else
								{
									setup.status.MentalState = BotEnum.MentalState.Panic;
									setup.status.Purpose = BotEnum.Purpose.Patrolling;
									setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
								}
								//End Checking Purpose
							}
							else
							{
								// Died Point Not Checked
							}
						}
					}
					setup.movementUtility.WalkToStopCrouch();
					setup.overall.IsLastEnemyPointChecked = true;
					//MoveToStop
				}
			}
		}

		void PatrollPoint()
		{
			//
			currentGuardingIdleTime = 0;

			if (objects.patrolling)
			{
				setup.movePosition.SetPosition(objects.patrolling.GetRandomPoint().position);

				float patrollingPointDistance = Vector3.Distance(objects.root.position, setup.movePosition.GetPosition());

				if (patrollingPointDistance > 1.5f)
				{
					//LookAndWalk//
					movement.Move(objects.movePoint);
					currentPatrollingTime = Random.Range(patrollingIdleTime / 2, patrollingIdleTime);
				}
				else
				{
					if (patrollingPointDistance <= 1.5f)
					{
						currentPatrollingTime -= Time.deltaTime;
						if (currentPatrollingTime <= 0)
						{
							objects.patrolling = null;
							//End Checking Purpose
						}
					}
					setup.movementUtility.WalkToStopCrouch();
					if (objects.patrolling)
					{
						objects.patrolling.Reset();
						objects.patrolling.SetAreaIsSafe();
					}

					//MoveToStop
				}
			}
			else
			{
				PatrollingPoint patrollingPoint = setup.overall.FindClosestPoint(setup.transform, objects.suspicionPoint.position);
				if (patrollingPoint)
				{
					objects.patrolling = patrollingPoint;
					setup.status.Purpose = BotEnum.Purpose.Patrolling;
					setup.status.patrollCommand = BotEnum.PatrollCommand.PatrollPoint;
					objects.patrolling.SetIsBusy();
				}
				else
				{
					if (setup.overall.IsLastEnemyPointChecked && setup.overall.IsLastDiedPointChecked)
					{
						if (setup.overall.IsAreaSafe())
						{
							setup.movement.animator.SetFloat("Crouch", 0);
							objects.patrolling = null;
							setup.status.MentalState = BotEnum.MentalState.Quiet;
							setup.status.Purpose = BotEnum.Purpose.Command;
							setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
						}
						else
						{
							objects.patrolling = null;
							setup.status.Purpose = BotEnum.Purpose.Patrolling;
							setup.status.patrollCommand = BotEnum.PatrollCommand.Avoid;
						}
					}
					else
					{
						objects.patrolling = null;
						setup.status.Purpose = BotEnum.Purpose.Patrolling;
						setup.status.patrollCommand = BotEnum.PatrollCommand.MainPoint;
					}
				}
			}
		}

		Vector3 GetStopPoint(Vector3 point)
		{
			if(setup.agent.usedPathFinder)
			{
				return setup.agent.GetLastCorner();
			}
			else
			{
				return point;
			}
		}

		public void Debugging(string message)
		{
			if (setup.bot.debug)
			{
				print(message);
			}
		}
	}

}
