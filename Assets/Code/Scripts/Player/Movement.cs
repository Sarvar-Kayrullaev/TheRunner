using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZCameraShake;
using UnityEngine;

namespace PlayerRoot
{
    public class Movement
    {
        Player player;
        Gameplay.Debug debug;

        // PRIVATES //
        private bool isCrouching;
        private bool isGrounded;
        private bool canDownToGround = true;
        private bool isRunning;
        private bool jump;
        private bool isSlope;
        public bool isClimbing = false;
        private bool youCanClimbStart = false;
        bool callClimb = false;

        private bool isGroundedOnce;
        private bool isLeavGroundOnce;

        private Vector3 moveDirection;
        private Vector3 slopeNormal;

        private Transform ClimbTransform;
        private Transform currentClimbPosition;
        public int inCount = 0;

        //Float//
        float rotationX = 0;
        float climbLagTime = 0.3f;
        float climbLagTimer;
        float slopeForce;
        float slopeForceRayLength;
        float groundDistance;
        float crouchHeight;

        int climbTriggedCount;

        //Timers//
        float timeDisconnectedFromGround = 0;
        private float jumpOpportunityTime;
        private float doubleClickTime = .4f, lastClickTime;
        float accessTime;
        float footstepTimer;
        float jumpDownLimitTimer;
        float jumpAccessTime;
        float maxDistanceOfLeaveGround = 0;
        //Foot Step Sound
        private bool useFootStepSound = true;
        private float baseStepSpeed = 0.5f;
        private float walkStepMultipler = 0.9f;
        private float runStepMultipler = 0.6f;
        private float GetCurrentOffset => !isRunning ? baseStepSpeed * walkStepMultipler : isRunning ? baseStepSpeed * runStepMultipler : baseStepSpeed;


        private WeaponHolster holster;
        private CharacterController character;

        private float collapsingTime;
        private bool collapseDamaged = false;

        [System.Obsolete]
        public void Initialize(Player player)
        {
            this.player = player;
            holster = player.holster;
            character = player.character;
            debug = GameObject.FindObjectOfType<Gameplay.Debug>();
        }

        public void Update(Vector2 move, Vector2 mouse)
        {
            if (player.Died) return;
            bool canMove = player.canMove;
            isGrounded = canDownToGround && Utility.IsGrounded(player);
            player.OnGround = isGrounded;
            slopeNormal = Utility.GetSlopeNormal(player);
            jumpAccessTime -= Time.deltaTime;
            float groundDistance = GroundDistance();
            maxDistanceOfLeaveGround = maxDistanceOfLeaveGround < groundDistance ? groundDistance : maxDistanceOfLeaveGround;

            if (Input.GetKeyDown(KeyCode.C))
            {

            }
            // start crouch
            if (Input.GetKeyDown(KeyCode.C)) isCrouching = !isCrouching;
            crouchHeight = Mathf.Lerp(crouchHeight, isCrouching ? player.crouchHeight : 2, Time.deltaTime * player.crouchingSmooth);
            player.character.height = crouchHeight;
            player.collider.height = crouchHeight;

            if (jumpAccessTime <= 0) jumpAccess = true;

            if (isGrounded && moveDirection.y < 0)
            {
                moveDirection.y = 0;
            }

            if (holster.currentWeapon)
            {
                if (holster.currentWeapon.aim)
                {
                    player.lookSpeed = player.Sensitivity / 2;
                }
                else
                {
                    player.lookSpeed = player.Sensitivity;
                }
            }
            else
            {
                player.lookSpeed = player.Sensitivity;
            }

            Vector3 forward = player.transform.TransformDirection(Vector3.forward);
            Vector3 right = player.transform.TransformDirection(Vector3.right);
            if (holster.currentWeapon) isRunning = !holster.currentWeapon.aim && Input.GetKey(KeyCode.LeftShift);
            else isRunning = Input.GetKey(KeyCode.LeftShift);

            //CROSSHAIR//
            holster.crosshair.Running(Input.GetKey(KeyCode.LeftShift));
            holster.crosshair.Walking(move.x != 0 || move.y != 0);
            //CROSSHAIR//

            float curSpeedX;
            float curSpeedY;
            if (player.Mobile)
            {
                curSpeedX = player.moveSpeed * Speed(move).y;
                curSpeedY = player.moveSpeed * Speed(move).x;
            }
            else
            {
                curSpeedX = canMove ? (isRunning ? player.runningSpeed : player.walkingSpeed) * move.y : 0;
                curSpeedY = canMove ? (isRunning ? player.runningSpeed : player.walkingSpeed) * move.x : 0;
            }
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            StepSound(move);

            jumpDownLimitTimer -= Time.deltaTime;
            
            if (isGrounded)
            {
                jump = false;

                if (!isGroundedOnce)
                {
                    isGroundedOnce = true;
                    if (jumpDownLimitTimer <= 0)
                    {
                        if (maxDistanceOfLeaveGround > 0.5f)
                        {
                            player.audio.PlayOneShot(player.jumpDownSound[Random.Range(0, player.jumpDownSound.Length - 1)]);
                            if (holster.currentWeapon) player.weaponOnceSway.Once(holster.currentWeapon.aim);
                            jumpDownLimitTimer = 0.5f;
                            CollapseDamage();
                            collapsingTime = 0;
                        }
                    }
                    maxDistanceOfLeaveGround = 0;
                }

            }
            else
            {
                timeDisconnectedFromGround += Time.deltaTime;
                isGroundedOnce = false;
            }
            if(!isGrounded && movementDirectionY < 0)
            {
                collapsingTime += Time.deltaTime;
            }else
            {
                collapsingTime = 0;
            }

            jumpOpportunityTime = isGrounded ? 0.3f : jumpOpportunityTime - Time.deltaTime;

            if (Input.GetButtonDown("Jump") && canMove && !jump)
            {
                jump = true;
                if (isGrounded || jumpOpportunityTime > 0)
                {
                    if (holster.currentWeapon) player.weaponOnceSway.Once(holster.currentWeapon.aim);
                    moveDirection.y = player.jumpSpeed;
                    isGroundedOnce = false;
                    canDownToGround = false;

                    async Task Wait()
                    {
                        await Task.Delay(50);
                        {
                            canDownToGround = true;
                        }
                    }
                    {
                        _ = Wait();
                    }
                }
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // ------- SLOPE ------- //

            isSlope = Vector3.Angle(Vector3.up, slopeNormal) >= player.slipAngle;
            if (isSlope && !jump)
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeNormal);
                Vector3 normal = slopeNormal;
                Vector3 c = Vector3.Cross(Vector3.up, normal);
                Vector3 u = Vector3.Cross(c, normal);
                Vector3 slidingDirection = u * 4f;
                if (isGrounded)
                {
                    moveDirection.y = 0;
                    moveDirection.x += slidingDirection.x;
                    moveDirection.z += slidingDirection.z;
                    moveDirection.y += slidingDirection.y;
                }
            }
            // ------- SLOPE ------- //
            bool headbob;
            if (isGrounded) headbob = true;
            else
            {
                headbob = maxDistanceOfLeaveGround < 0.5f;
            }
            BobSpeed(isRunning, move, headbob);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                accessTime = 0.3f;
                if (ClimbTransform)
                {
                    Vector3 climbEulerAngles = Formula.RelativeToParent(ClimbTransform.localEulerAngles, ClimbTransform.parent.parent.eulerAngles);
                    if (CompareEuler(player.transform.eulerAngles, climbEulerAngles, 45))
                    {
                        if (timeSinceLastClick <= doubleClickTime)
                            if (youCanClimbStart && !isClimbing)
                            {
                                if (holster.currentWeapon)
                                    if (!holster.currentWeapon.isReloading)
                                    {
                                        climbLagTimer = climbLagTime;
                                        isClimbing = true;
                                        holster.Climb();
                                    }
                            }
                    }
                }
                else
                {
                    if (timeSinceLastClick <= doubleClickTime && !isGrounded)
                        callClimb = true;
                }
                lastClickTime = Time.time;
            }

            accessTime -= Time.deltaTime;
            if (callClimb)
            {
                if (isGrounded && accessTime <= 0)
                {
                    callClimb = false;
                }
                else
                {
                    if (ClimbTransform)
                    {
                        Vector3 climbEulerAngles = Formula.RelativeToParent(ClimbTransform.localEulerAngles, ClimbTransform.parent.parent.eulerAngles);
                        if (CompareEuler(player.transform.eulerAngles, climbEulerAngles, 45))
                        {
                            holster.Climb();
                            climbLagTimer = climbLagTime;
                            isClimbing = true;
                            callClimb = false;
                        }
                    }
                }
            }

            if (isClimbing)
            {
                ClimbProgress();
            }
            else
            {
                if (!isClimbing)
                {
                    moveDirection.y -= player.gravity * Time.deltaTime;
                }
                character.Move(moveDirection * Time.deltaTime);
            }

            if (canMove)
            {
                float Sensitivity = player.Mobile ? (player.lookSpeed * 0.1f) : player.lookSpeed;
                rotationX += -mouse.y * Sensitivity;
                rotationX = Mathf.Clamp(rotationX, -player.lookXLimit, player.lookXLimit);
                player.Head.localRotation = Quaternion.Euler(rotationX, 0, 0);
                player.transform.rotation *= Quaternion.Euler(0, mouse.x * Sensitivity, 0);
            }

            player.status.crouch = isCrouching;
        }

        void CollapseDamage()
        {
            Debug.Log("CollapseTime: "+ collapsingTime);
            if(collapsingTime > 0.8f) player.TakeDamage(20,player.transform);
            else if(collapsingTime > 1) player.TakeDamage(80,player.transform);
            else if(collapsingTime > 1.3f) player.TakeDamage(160,player.transform);
            else if(collapsingTime > 1.7f) player.TakeDamage(320,player.transform);
            else if(collapsingTime > 2) player.TakeDamage(100000,player.transform);

        }


        // ------------------------------------------------------------------------------------------------------
        public void Crouch()
        {
            isCrouching = !isCrouching;
        }

        public void Rotate(Vector2 mouse)
        {
            rotationX += -mouse.y * (player.lookSpeed * 0.1f);
            rotationX = Mathf.Clamp(rotationX, -player.lookXLimit, player.lookXLimit);
            player.Head.localRotation = Quaternion.Euler(rotationX, 0, 0);
            player.transform.rotation *= Quaternion.Euler(0, mouse.x * (player.lookSpeed * 0.1f), 0);
        }
        public Vector2 Speed(Vector2 vector)
        {
            bool aim = false;
            if (holster.currentWeapon) aim = holster.currentWeapon.aim;

            float y = vector.y < 0 ? vector.y / 2 : vector.y;
            float x = vector.x / 2;
            y = aim ? y / 2 : y;
            x = aim ? x / 2 : x;
            return new Vector3(x, y);
        }

        bool OnSlope()
        {
            if (isGrounded) return false;

            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, character.height / 2 * slopeForceRayLength))
                return true;
            return false;
        }

        float GroundDistance()
        {
            if (isGrounded) return 0;
            RaycastHit hit;
            Vector3 origin = player.transform.position;
            origin.y -= player.character.height / 2;
            if (Physics.Raycast(origin, Vector3.down, out hit, 5))
                return hit.distance;
            else return 5;
        }

        bool OnNearGround(float nearDistance)
        {
            if (isGrounded) return true;
            RaycastHit hit;
            Vector3 origin = player.transform.position;
            origin.y -= player.character.height / 2;
            if (Physics.Raycast(origin, Vector3.down, out hit, nearDistance))
                return true;
            else return false;
        }

        void ClimbProgress()
        {
            if (isClimbing)
            {
                climbLagTimer -= Time.deltaTime;
                if (currentClimbPosition != null)
                {
                    Vector3 pos = currentClimbPosition.position;
                    if (Vector3.Distance(player.transform.position, pos) <= 0.1f && climbLagTimer <= 0)
                    {
                        inCount++;
                        if (inCount >= 2)
                        {
                            isClimbing = false;
                            inCount = 0;
                            currentClimbPosition = null;
                        }
                        else
                        {
                            currentClimbPosition = currentClimbPosition.GetChild(0);

                        }
                    }
                    else
                    {
                        float Speed = Vector3.Distance(pos, player.transform.position) / 0.3f;
                        player.transform.position = Vector3.MoveTowards(player.transform.position, pos, Time.deltaTime * 6);
                    }
                }
                else
                {
                    player.Invoke("ShakeCamera", 0.1f);
                    currentClimbPosition = GetClosestChild(ClimbTransform);
                }
            }
        }
        bool jumpAccess = true;

        public void MobileJump()
        {
            if (player.canMove && !jump)
            {
                jump = true;
                if (isGrounded || jumpOpportunityTime > 0)
                {
                    if (holster.currentWeapon) player.weaponOnceSway.Once(holster.currentWeapon.aim);
                    moveDirection.y = player.jumpSpeed;
                    isGroundedOnce = false;
                    canDownToGround = false;

                    async Task Wait()
                    {
                        await Task.Delay(50);
                        {
                            canDownToGround = true;
                        }
                    }
                    {
                        _ = Wait();
                    }
                }
            }
        }


        public void Jump()
        {
            if (player.canMove && !jump && jumpAccess)
            {
                jump = true;
                jumpAccess = false;
                if (isGrounded || jumpOpportunityTime > 0)
                {
                    //exitSlope = true;
                    isGroundedOnce = false;
                    if (holster.currentWeapon) player.weaponOnceSway.Once(holster.currentWeapon.aim);
                    moveDirection.y = player.jumpSpeed;
                    jumpAccessTime = 0.3f;
                }
            }

            float timeSinceLastClick = Time.time - lastClickTime;
            accessTime = 0.3f;
            if (ClimbTransform)
            {
                Vector3 climbEulerAngles = Formula.RelativeToParent(ClimbTransform.localEulerAngles, ClimbTransform.parent.parent.eulerAngles);
                if (CompareEuler(player.transform.eulerAngles, climbEulerAngles, 45))
                {
                    if (timeSinceLastClick <= doubleClickTime)
                        if (youCanClimbStart && !isClimbing)
                        {
                            if (holster.currentWeapon)
                                if (!holster.currentWeapon.isReloading)
                                {
                                    climbLagTimer = climbLagTime;
                                    isClimbing = true;
                                    holster.Climb();
                                }
                        }
                }
            }
            else
            {
                if (timeSinceLastClick <= doubleClickTime && !isGrounded)
                    callClimb = true;
            }
            lastClickTime = Time.time;
        }
        void ShakeCamera()
        {
            CameraShaker.Instance.ShakeOnce(3, 3, 0.05f, 1);
        }

        public void TriggerEnter(Collider other)
        {

            if (other.tag == "Climb")
            {
                climbTriggedCount++;
                youCanClimbStart = true;
                ClimbTransform = other.transform;
            }
        }

        public void TriggerExit(Collider other)
        {

            if (other.tag == "Climb")
            {
                climbTriggedCount--;
                if (climbTriggedCount <= 0)
                {
                    youCanClimbStart = false;
                    ClimbTransform = null;
                }
            }
        }
        void StepSound(Vector2 vector)
        {
            if (!useFootStepSound || !isGrounded) return;
            if (moveDirection.x == 0 && moveDirection.z == 0)
            {
                player.liveAction.normalSpeed = 0;
                return;
            };
            footstepTimer -= Time.deltaTime;
            float normalizedSpeed;
            bool mute = false;
            if (player.Mobile)
            {
                normalizedSpeed = Formula.Positive(Speed(vector).x) + Formula.Positive(Speed(vector).y);
                normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
                isRunning = normalizedSpeed > 0.9f;
                mute = normalizedSpeed < 0.25f;

            }
            else
            {
                normalizedSpeed = Formula.Positive(Speed(vector).x) + Formula.Positive(Speed(vector).y);
                normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
            }
            player.liveAction.normalSpeed = normalizedSpeed;

            if (footstepTimer <= 0 && !mute)
            {
                if (Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 3))
                {
                    switch (hit.collider.tag)
                    {

                        case "Footsteps/Ground":

                            if (isRunning)
                            {
                                player.audio.PlayOneShot(player.runSounds[Random.Range(0, player.runSounds.Length - 1)]);
                            }
                            else
                            {
                                player.audio.PlayOneShot(player.walkSounds[Random.Range(0, player.walkSounds.Length - 1)]);
                            }
                            break;
                        case "Footsteps/Wood":
                            if (isRunning)
                            {
                                player.audio.PlayOneShot(player.woodRunSounds[Random.Range(0, player.woodRunSounds.Length - 1)]);
                            }
                            else
                            {
                                player.audio.PlayOneShot(player.woodWalkSounds[Random.Range(0, player.woodWalkSounds.Length - 1)]);
                            }
                            break;
                    }
                }
                footstepTimer = GetCurrentOffset;
            }

        }
        void BobSpeed(bool isRunning, Vector2 vector, bool isGrounded)
        {
            player.headBob.isGrounded = isGrounded;
            if (player.Mobile)
            {
                float ampMin = player.headBob._ampMin;
                float ampMax = player.headBob._ampMax;
                float freqMin = player.headBob._freqMin;
                float freqMax = player.headBob._freqMax;

                float normalizedSpeed = Formula.Positive(Speed(vector).x) + Formula.Positive(Speed(vector).y);
                normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

                if (holster.currentWeapon)
                {
                    if (holster.currentWeapon.aim)
                        player.headBob._amplitude = Mathf.Lerp(ampMin / 8, ampMax / 8, normalizedSpeed);
                    else
                        player.headBob._amplitude = Mathf.Lerp(ampMin, ampMax, normalizedSpeed);
                }
                else
                {
                    player.headBob._amplitude = Mathf.Lerp(ampMin, ampMax, normalizedSpeed);
                }

                if (normalizedSpeed > 0.9)
                {
                    player.headBob._frequency = Mathf.Lerp(freqMin, freqMax, 0.9f);
                }
                else if (normalizedSpeed > 0.45)
                {
                    player.headBob._frequency = Mathf.Lerp(freqMin, freqMax, 0.0f);
                }
                else
                {
                    player.headBob._frequency = 0;
                }
            }
            else
            {
                if (isRunning)
                {
                    player.headBob._amplitude = player.headBob._amplitudeSpring;
                    player.headBob._frequency = player.headBob._frequencySpring;
                }
                else
                {
                    float normalizedSpeed = Formula.Positive(Speed(vector).x) + Formula.Positive(Speed(vector).y);
                    if (normalizedSpeed > 0)
                    {
                        if (holster.currentWeapon)
                        {
                            if (holster.currentWeapon.aim)
                            {
                                player.headBob._amplitude = player.headBob._amplitudeAim;
                                player.headBob._frequency = player.headBob._frequencyAim;
                            }
                            else
                            {
                                player.headBob._amplitude = player.headBob._amplitudeRun;
                                player.headBob._frequency = player.headBob._frequencyRun;
                            }
                        }
                        else
                        {
                            player.headBob._amplitude = player.headBob._amplitudeRun;
                            player.headBob._frequency = player.headBob._frequencyRun;
                        }
                    }else
                    {
                        player.headBob._amplitude = 0;
                        player.headBob._frequency = 0;
                    }
                }
            }
        }

        Transform GetClosestChild(Transform parent)
        {
            Transform targetChild = null;
            float nearestChild = Mathf.Infinity;
            foreach (Transform child in parent)
            {
                float dist = Vector3.Distance(child.position, player.transform.position);
                if (dist < nearestChild)
                {
                    targetChild = child;
                    nearestChild = dist;
                }
            }
            return targetChild;
        }
        bool CompareEuler(Vector3 a, Vector3 b, float limit)
        {
            if (Formula.InRadian(limit, a.y, b.y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
