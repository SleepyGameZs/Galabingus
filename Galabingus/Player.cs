﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System.Security.Cryptography;

// Matthew Rodriguez
// 2023, 3, 13
// Player
// PlayerStates - Player translations states
// Position - position of the player
// Sprite - player sprite
// Scale - scale of the player
// Animation - animation for the player
// Collider - collider for the player
// Update - updates player FSM
// Shoot - triggered when the player presses the shoot button
// Draw - draws the player to the screen

namespace Galabingus
{
    /// <summary>
    ///  Player translation states
    /// </summary>
    public enum PlayerStates
    {
        Idle, // The palyer is not moving
        Move, // The player is moving
        None  // The player has a undetermined state
    }

    /// <summary>
    ///  Player
    ///  Contains basic movment system for a top down player
    ///  Note: there is no gravity
    /// </summary>
    internal class Player : GameObject
    {
        private struct Ghost
        {
            public Vector2 Position;
            public float boostOpacity;
            public Color ghostColor;
        }

        private static Player playerInstance = null;
        private KeyboardState previousPreviousKeyboardStateX;
        private KeyboardState previousPreviousKeyboardStateY;
        private KeyboardState previousKeyboardState;
        private KeyboardState previousKeyboardStateX;
        private KeyboardState previousKeyboardStateY; // Previous KeyboardState (only updates every interval of input buffer time)
        private KeyboardState currentKeyboardState;  // Current KeyboardState (always current keyboard state)
        private Vector2 previousVelocity;            // Holds the previous direction and magnitude of velocity
        private Vector2 velocity;                    // Holds the current direction and magnitude of velocity
        private Vector2 speed;                       // Scalar quantity for velocity 
        private Vector2 acceleration;                // Accleration applied to velocity
        private Vector2 translationRatio;            // Ratio between height and width of the screen
        private PlayerStates playerState;            // Player translation state
        private Keys deAccFrom;                      // What key was last released
        private double totalTime;                    // total ellapsed time
        private float delayBufferTime;               // half the actual input buffer time
        private float inputBufferTime;               // input response time
        private ushort contentName;                  // the content index
        private float boostFrameRate;
        private float health;
        private bool previousCollision;
        private bool shot;
        private bool boost;
        private float boostSpeed;
        private List<Ghost> ghosts = new List<Ghost>();
        private Vector2 boostSpawnGhost;
        private float boostOpacity;
        private bool shiftBoost;
        private double totalBoostTime;

        public static Player PlayerInstance
        {
            get
            {
                return playerInstance;
            }
            set
            {
                playerInstance = value;
            }
        }

        public ushort ContentName
        {
            get
            {
                return PlayerInstance.contentName;
            }
        }

        public float Health
        {
            get
            {
                return PlayerInstance.health;
            }
            set
            {
                PlayerInstance.health = value;
            }
        }
        
        
        public Texture2D WhiteSprite
        {
            get
            {
                GameObject.Instance.Content = GameObject.Instance.Content.player_white_strip5;
                if (GetSprite(0) == null)
                {
                    LoadSprite(GameObject.Instance.Content.player_white_strip5, 0);
                }
                return GetSprite(0);
            }
        }


        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
        }

        public Vector2 TranslationRatio
        {
            get
            {
                return translationRatio * 2;
            }
        }

        /// <summary>
        ///  Position of the player
        /// </summary>
        public Vector2 Position
        {
            get
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                return GetPosition(0);
            }
            set
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                SetPosition(0, value);
            }
        }

        /// <summary>
        ///  Player sprite
        /// </summary>
        public Texture2D Sprite
        {
            get
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                return GetSprite(0);
            }
            set
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                SetSprite(0, value);
            }
        }

        /// <summary>
        ///  Single Player sprite bounds
        /// </summary>
        public Rectangle Transform
        {
            get
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                return GetTransform(0);
            }
            set
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                SetTransform(0, value);
            }
        }

        /// <summary>
        ///  Scale of the palyer sprite
        /// </summary>
        public float Scale
        {
            get
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                return GetScale(0);
            }
            set
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                SetScale(0, value);
            }
        }

        /// <summary>
        ///  Player animation
        /// </summary>
        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                return GetAnimation(0);
            }
            set
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                SetAnimation(0, value);
            }
        }

        /// <summary>
        ///  Player collider
        /// </summary>
        public Collider Collider
        {
            get
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                return GetCollider(0);
            }
            set
            {
                GameObject.Instance.Content = PlayerInstance.contentName;
                SetCollider(0, value);
            }
        }

        /// <summary>
        ///  Creates a player from a playerSprite
        /// </summary>
        /// <param name="playerSprite">player sprite image</param>
        /// <param name="speed">player translation speed</param>
        public Player(Vector2 speed, ushort contentName) :
            base(contentName, 0)
        {
            if (PlayerInstance == null)
            {
                PlayerInstance = this;
            }
            PlayerInstance.contentName = contentName;
            PlayerInstance.Position = Vector2.Zero;
            PlayerInstance.velocity = Vector2.Zero;
            PlayerInstance.acceleration = Vector2.Zero;
            PlayerInstance.speed = speed;
            PlayerInstance.playerState = PlayerStates.Idle;
            PlayerInstance.totalTime = 0;
            PlayerInstance.inputBufferTime = 0.00399939f;
            PlayerInstance.inputBufferTime = 0.003f;
            PlayerInstance.delayBufferTime = inputBufferTime / 2.0f;
            PlayerInstance.Scale = 2.35f;
            playerInstance.Animation.AnimationDuration = 0.05f;
            // Ratio is calclated via the shape of the player sprite
            // against the width and height of the screen
            // With the third factor a vector of 1 ie the directional vector ie normalized velocity
            PlayerInstance.translationRatio = new Vector2(
                ((((float)PlayerInstance.Transform.Width) / ((float)PlayerInstance.Transform.Height))) / 2.0f,
                ((((float)PlayerInstance.Transform.Height) / ((float)PlayerInstance.Transform.Width))) / 2.0f
            );
            PlayerInstance.previousCollision = false;
            PlayerInstance.shot = false;
            PlayerInstance.boost = false;
            PlayerInstance.boostSpeed = 1.625f;
            PlayerInstance.boostFrameRate = 0.0625f;
            PlayerInstance.boostOpacity = 1;
            PlayerInstance.boostSpawnGhost = Vector2.Zero;
            PlayerInstance.shiftBoost = false;
            PlayerInstance.ghosts = new List<Ghost>();
        }

        /// <summary>
        ///  Updates the player Finite State Machine
        /// </summary>
        public void Update(GameTime gameTime)
        {
            PlayerInstance.inputBufferTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 translationAjdustedRatio = translationRatio;
            float bufferTime = inputBufferTime;

            // Adjust velocity magnitude for dirational change
            if (
                (previousVelocity.Y > 0 && velocity.Y < 0) ||
                (previousVelocity.X > 0 && velocity.X < 0) ||
                (Math.Abs(previousVelocity.X - velocity.X) > 1f) ||
                (Math.Abs(previousVelocity.Y - velocity.Y) > 1f)
            )
            {
                // Micro adjust velocity for de-acceleration
                if ((Math.Abs(velocity.X) > 0.5 && Math.Abs(velocity.Y) > 0.5))
                {
                    velocity -= velocity * inputBufferTime * 2.0f;
                }
                else
                {
                    // If not vertical or horizontal de-acceleration
                    if (Math.Abs(velocity.X) > 0.5)
                    {
                        velocity.Y -= velocity.Y * inputBufferTime * 2.0f;
                    }
                    if (Math.Abs(velocity.Y) > 0.5)
                    {
                        velocity.X -= velocity.X * inputBufferTime * 2.0f;
                    }
                }
            }

            // Ease the transition of velocity when in diagnal
            if (previousVelocity != velocity)
            {
                if ((Math.Abs(previousVelocity.X) > 0 && Math.Abs(previousVelocity.Y) > 0) ||
                    (Math.Abs(velocity.X) > 0 && Math.Abs(velocity.Y) > 0)
                )
                {
                    velocity += previousVelocity / (3.0f);
                }
            }

            // Add the acceleration to the velocity
            // Clamp the velocity to zero when too small
            velocity += acceleration;
            if (Math.Abs(velocity.X) < 0.0000001)
            {
                velocity.X = 0;
            }
            if (Math.Abs(velocity.Y) < 0.0000001)
            {
                velocity.Y = 0;
            }

            // When not in a change the speed to be 45 to the large side
            if (!(Math.Abs(velocity.X) > 0 && Math.Abs(velocity.Y) > 0))
            {
                velocity.X = velocity.X / speed.Y * translationRatio.Y;
            }
            else if (Math.Abs(velocity.X) > 0 && Math.Abs(velocity.Y) > 0)
            {
                // When in a diagnal set the proper translation ratio
                // reset the buffer time
                bufferTime = delayBufferTime;
                if (translationRatio.X < translationRatio.Y)
                {
                    translationAjdustedRatio.X = translationRatio.Y;
                }
                else
                {
                    translationAjdustedRatio.Y = translationRatio.X;
                }
            }

            //Vector2 previousPosition = Position;

            PlayerInstance.Transform = PlayerInstance.Animation.Play(gameTime);
            List<Collision> intercepts = PlayerInstance.Collider.UpdateTransform(
                PlayerInstance.Sprite,                         // Player Sprite
                PlayerInstance.Position,                       // Player position
                PlayerInstance.Transform,                      // Player transform for sprite selection
                GameObject.Instance.GraphicsDevice,
                GameObject.Instance.SpriteBatch,
                PlayerInstance.Scale,                          // Player scale
                SpriteEffects.None,
                contentName,                                   // Content
                0
            );

            //Vector2 previousVelocity;
            bool collides = false;

            foreach (Collision collision in intercepts)
            {
                if (collision.other != null && this.Collider.Resolved)
                {
                    previousVelocity = velocity;
                    acceleration = Vector2.Zero;
                    velocity = Vector2.Zero;
                    collides = true;
                }
                else if (collision.other != null)
                {
                    previousVelocity = velocity;
                    acceleration = Vector2.Zero;
                    velocity = Vector2.Zero;
                    collides = true;
                }
            }

            // Determine if idle or moving
            if (playerState == PlayerStates.Idle || playerState == PlayerStates.None)
            {
                // Get the directional velocity to de-accelerate
                switch (deAccFrom)
                {
                    case Keys.D:
                        velocity.X = (velocity == Vector2.Zero ? velocity.X : (velocity.X) / 1.8f);
                        velocity.Y = 0;
                        break;
                    case Keys.A:
                        velocity.X = (velocity == Vector2.Zero ? velocity.X : (velocity.X) / 1.8f);
                        velocity.Y = 0;
                        break;
                    case Keys.W:
                        velocity.Y = (velocity == Vector2.Zero ? velocity.Y : (velocity.Y) / 1.8f);
                        velocity.X = 0;
                        break;
                    case Keys.S:
                        velocity.Y = (velocity == Vector2.Zero ? velocity.Y : (velocity.Y) / 1.8f);
                        velocity.X = 0;
                        break;
                }

                // Slow de-accelerate
                Position -= velocity;
            }
            else
            {
                if (!collides && !previousCollision)
                {
                    Position += (velocity == Vector2.Zero ? velocity : Vector2.Normalize(velocity) * (float)Animation.EllapsedTime * ((boost) ? boostSpeed : 1) * speed * translationAjdustedRatio);
                }
            }

            Vector2 normPreVelocity = previousVelocity;
            Vector2 normVelocity = velocity;

            if (Math.Abs(normPreVelocity.X) < 0.0000001)
            {
                normPreVelocity.X = 0;
            }
            if (Math.Abs(normPreVelocity.Y) < 0.0000001)
            {
                normPreVelocity.Y = 0;
            }

            normPreVelocity = (normPreVelocity == Vector2.Zero ? normPreVelocity : Vector2.Normalize(normPreVelocity));

            if (Math.Abs(normPreVelocity.X) > 0.99)
            {
                if (normPreVelocity.X > 0)
                {
                    normPreVelocity.X = 1;
                }
                else if (normPreVelocity.Y < 0)
                {
                    normPreVelocity.X = -1;
                }
            }
            if (Math.Abs(normPreVelocity.Y) > 0.99)
            {
                if (normPreVelocity.Y > 0)
                {
                    normPreVelocity.Y = 1;
                }
                else if (normPreVelocity.Y < 0)
                {
                    normPreVelocity.Y = -1;
                }
            }

            if (Math.Abs(normVelocity.X) < 0.0000001)
            {
                normVelocity.X = 0;
            }
            if (Math.Abs(normVelocity.Y) < 0.0000001)
            {
                normVelocity.Y = 0;
            }

            normVelocity = (normVelocity == Vector2.Zero ? normVelocity : Vector2.Normalize(normVelocity));

            if (Math.Abs(normVelocity.X) > 0.99)
            {
                if (normVelocity.X > 0)
                {
                    normVelocity.X = 1;
                }
                else if (normVelocity.X < 0)
                {
                    normVelocity.X = -1;
                }
            }
            if (Math.Abs(normVelocity.Y) > 0.99)
            {
                if (normVelocity.Y > 0)
                {
                    normVelocity.Y = 1;
                }
                else if (normVelocity.Y < 0)
                {
                    normVelocity.Y = -1;
                }
            }

            if (collides || !collides && normPreVelocity != Vector2.Zero && normPreVelocity != normVelocity)
            {
                previousCollision = collides;
            }

            previousVelocity = velocity;

            totalBoostTime += gameTime.ElapsedGameTime.TotalSeconds;
            totalTime += gameTime.ElapsedGameTime.TotalSeconds;

            currentKeyboardState = Keyboard.GetState();
            if (!previousCollision || currentKeyboardState != previousKeyboardState )
            {
                // Player Finite State Machine
                switch (playerState)
                {
                    // Input Transition state
                    case PlayerStates.None:
                        if (
                            previousKeyboardState.IsKeyDown(Keys.D) ||
                            previousKeyboardState.IsKeyDown(Keys.A) ||
                            previousKeyboardState.IsKeyDown(Keys.W) ||
                            previousKeyboardState.IsKeyDown(Keys.S)
                        )
                        {
                            playerState = PlayerStates.Move;
                        }
                        else
                        {
                            playerState = PlayerStates.Idle;
                        }
                        break;

                    // Player is idle so slow the velocity and get rid of acceleration
                    // Exit to the Player None state
                    case PlayerStates.Idle:
                        acceleration = Vector2.Zero;
                        playerState = PlayerStates.None;
                        break;

                    // Updates the player velocity and acceleration
                    // Uses standard WASD controls
                    // Exits to the player None state when not presing WASD
                    case PlayerStates.Move:
                        bool xPause = true;
                        bool yPause = true;
                        // When a directional key is lifted pick out
                        // and update the directional de acceleration
                        if (previousKeyboardState.IsKeyUp(Keys.D))
                        {
                            deAccFrom = Keys.D;
                        }
                        else if (previousKeyboardState.IsKeyUp(Keys.A))
                        {
                            deAccFrom = Keys.A;
                        }
                        else if (previousKeyboardState.IsKeyUp(Keys.W))
                        {
                            deAccFrom = Keys.W;
                        }
                        else if (previousKeyboardState.IsKeyUp(Keys.S))
                        {
                            deAccFrom = Keys.S;
                        }

                        // If we are not translating
                        if (!(
                            previousKeyboardState.IsKeyDown(Keys.D) ||
                            previousKeyboardState.IsKeyDown(Keys.A) ||
                            previousKeyboardState.IsKeyDown(Keys.W) ||
                            previousKeyboardState.IsKeyDown(Keys.S)
                        ))
                        {
                            // Exit PlayerInstance player state
                            // force next key update
                            // Instantly update the key
                            totalTime = inputBufferTime;
                            playerState = PlayerStates.None;
                            previousKeyboardState = Keyboard.GetState();
                        }

                        foreach (Keys key in previousKeyboardState.GetPressedKeys())
                        {
                            //Debug.WriteLine(key);
                        }
                        //Debug.WriteLine();

                        // Directional X +
                        if (
                            previousKeyboardStateX.IsKeyDown(Keys.D) && !previousPreviousKeyboardStateX.IsKeyDown(Keys.A) || previousKeyboardStateX.IsKeyDown(Keys.D) && previousPreviousKeyboardStateX.IsKeyDown(Keys.D)
                        )
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                            if (velocity.X >= 1)
                            {
                                velocity.X = 1;
                            }
                            if (velocity.X < -inputBufferTime && !currentKeyboardState.IsKeyDown(Keys.A))
                            {
                                velocity.X = inputBufferTime;
                                acceleration.X = acceleration.X / 10f;
                            }
                            velocity.X += 0.05f;
                            acceleration.X += 0.0005f;
                            if (!currentKeyboardState.IsKeyDown(Keys.W) && !currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                velocity.Y = velocity.Y / 10.0f;
                                acceleration.Y = acceleration.Y / 10f;
                                acceleration.X = acceleration.X / 10f;
                            }
                            xPause = false;
                        }

                        // Directional X -
                        if (
                            previousKeyboardStateX.IsKeyDown(Keys.A) && !previousPreviousKeyboardStateX.IsKeyDown(Keys.D) || previousKeyboardStateX.IsKeyDown(Keys.A) && previousPreviousKeyboardStateX.IsKeyDown(Keys.A)
                        )
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                            if (velocity.X <= -1)
                            {
                                velocity.X = -1;
                            }
                            if (velocity.X > inputBufferTime && !currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                velocity.X = -inputBufferTime;
                                acceleration.X = acceleration.X / 10f;

                            }
                            velocity.X += -0.05f;
                            acceleration.X += -0.0005f;
                            if (!currentKeyboardState.IsKeyDown(Keys.W) && !currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                velocity.Y = velocity.Y / 10.0f;
                                acceleration.Y = acceleration.Y / 10f;
                                acceleration.X = acceleration.X / 10f;
                            }
                            xPause = false;
                        }

                        if (previousKeyboardStateX.IsKeyDown(Keys.D) && previousPreviousKeyboardStateX.IsKeyDown(Keys.A) || previousPreviousKeyboardStateX.IsKeyDown(Keys.D) && previousKeyboardStateX.IsKeyDown(Keys.A))
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                            velocity.X = 0.0f;
                            if (!currentKeyboardState.IsKeyDown(Keys.W) && !currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                velocity.Y = velocity.Y / 10.0f;
                                acceleration.Y = acceleration.Y / 10f;
                                acceleration.X = acceleration.X / 10f;
                            }
                            xPause = false;
                        }

                        if (xPause)
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                        }

                        // Directional Y -
                        if (
                            previousKeyboardStateY.IsKeyDown(Keys.W) && !previousPreviousKeyboardStateY.IsKeyDown(Keys.S) || previousKeyboardStateY.IsKeyDown(Keys.W) && previousPreviousKeyboardStateY.IsKeyDown(Keys.W)
                        )
                        {
                            previousPreviousKeyboardStateY = previousKeyboardStateY;
                            if (velocity.Y <= -1)
                            {
                                velocity.Y = -1;
                            }
                            if (velocity.Y > inputBufferTime && !currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                velocity.Y = -inputBufferTime;
                                acceleration.Y = acceleration.Y / 10f;
                            }
                            velocity.Y += -0.05f;
                            acceleration.Y += -0.0005f;
                            if (!currentKeyboardState.IsKeyDown(Keys.A) && !currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                velocity.X = velocity.X / 10.0f;
                                acceleration.X = acceleration.X / 10f;
                                acceleration.Y = acceleration.Y / 10f;
                            }
                            yPause = false;
                        }

                        // Directional Y +
                        if (
                            previousKeyboardStateY.IsKeyDown(Keys.S) && !previousPreviousKeyboardStateY.IsKeyDown(Keys.W) || previousKeyboardStateY.IsKeyDown(Keys.S) && previousPreviousKeyboardStateY.IsKeyDown(Keys.S)
                        )
                        {
                            previousPreviousKeyboardStateY = previousKeyboardStateY;
                            if (velocity.Y >= 1)
                            {
                                velocity.Y = 1;
                            }
                            if (velocity.Y < inputBufferTime && !currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                velocity.Y = inputBufferTime;
                                acceleration.Y = acceleration.Y / 10f;
                            }
                            velocity.Y += 0.05f;
                            acceleration.Y += 0.0005f;
                            if (!currentKeyboardState.IsKeyDown(Keys.A) && !currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                velocity.X = velocity.X / 10.0f;
                                acceleration.X = acceleration.X / 10f;
                                acceleration.Y = acceleration.Y / 10f;
                            }
                            yPause = false;

                        }

                        if (previousKeyboardStateX.IsKeyDown(Keys.S) && previousPreviousKeyboardStateX.IsKeyDown(Keys.W) || previousPreviousKeyboardStateX.IsKeyDown(Keys.S) && previousKeyboardStateX.IsKeyDown(Keys.W))
                        {
                            previousPreviousKeyboardStateY = previousKeyboardStateY;
                            velocity.Y = 0;
                            if (!currentKeyboardState.IsKeyDown(Keys.A) && !currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                velocity.X = velocity.X / 10.0f;
                                acceleration.X = acceleration.X / 10f;
                                acceleration.Y = acceleration.Y / 10f;
                            }
                            yPause = false;
                        }

                        if (yPause)
                        {
                            previousPreviousKeyboardStateY = previousKeyboardStateY;
                        }

                        if (previousKeyboardState.IsKeyDown(Keys.LeftControl))
                        {
                            boost = true;
                        }

                        break;
                }
                //shot = false;
            }

            // When space is pressed trigger shoot
            if (previousKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                //GameObject.Instance.Content = GameObject.Instance.Content.tile_strip26;
                //Texture2D otherSprite = GetSprite(0);
                //Rectangle otherTransform = GetAnimation(0).GetFrame(2);
                //Animation otherAnimation = GetAnimation(0);
                //PlayerInstance.Animation = otherAnimation;
                //PlayerInstance.Sprite = otherSprite;
                //PlayerInstance.Transform = otherTransform;
                //PlayerInstance.Animation.AnimationDuration = 1000000000;
                Shoot();
            }

            // When the ellapsed time is the buffer time update the keyboard state
            if (totalTime >= bufferTime)
            {
                previousKeyboardState = currentKeyboardState;
                if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyUp(Keys.A) || currentKeyboardState.IsKeyUp(Keys.D))
                {
                    previousKeyboardStateX = currentKeyboardState;
                }
                if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyUp(Keys.W) || currentKeyboardState.IsKeyUp(Keys.S))
                {
                    previousKeyboardStateY = currentKeyboardState;
                }

                if (boostOpacity <= 0.0f)
                {
                    boostOpacity = 0.01f;
                }
                if (ghosts.Count >= 3 && totalBoostTime >= boostFrameRate)
                {
                    List<Ghost> newGhost = new List<Ghost>();
                    for (int i = 0; i < ghosts.Count; i++)
                    {
                        Ghost tempGhost = ghosts[i];
                        tempGhost.boostOpacity = tempGhost.boostOpacity - 0.5f;
                        tempGhost.ghostColor = tempGhost.ghostColor * 0.5f;
                        ghosts[i] = tempGhost;
                        if (ghosts[i].boostOpacity > -(2.0f))
                        {
                            newGhost.Add(ghosts[i]);
                        }
                    }
                    ghosts = newGhost;
                }
                
                if (!currentKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    boostOpacity = 1f;
                    boost = false;
                    List<Ghost> newGhost = new List<Ghost>();
                    ghosts = newGhost;
                }
                else
                {
                    if (ghosts.Count <= 2 && totalBoostTime >= boostFrameRate)
                    {
                        Ghost ghostBoost = new Ghost();
                        ghostBoost.ghostColor = new Color(Color.DarkSlateBlue, 1.0f);
                        ghostBoost.Position = Position + normVelocity * (float)Animation.EllapsedTime * new Vector2(speed.X, speed.Y).LengthSquared() * ((1 - boostSpeed) * -0.1f );
                        boostSpeed *= (float)Animation.EllapsedTime;
                        boostOpacity -= 0.0005f;
                        ghostBoost.boostOpacity = boostOpacity;
                        ghosts.Add(ghostBoost);
                    }
                }
                totalTime -= bufferTime;
            }

            if (totalBoostTime >= boostFrameRate)
            {
                totalBoostTime -= boostFrameRate;
            }

            //Debug.WriteLine();
        }

        /// <summary>
        ///  Triggered when the 
        ///  Player presses the shoot button
        /// </summary>
        public void Shoot()
        {
            float flt_playerShootX = (Transform.Width * PlayerInstance.Scale) / 2;
            float flt_playerShootY = (Transform.Height * PlayerInstance.Scale) / 2;
            Vector2 vc2_shootPos = new Vector2(Position.X               // Base player X position
                                               + flt_playerShootX       // Center horizontally
                                               ,                        // Account for possible next movement
                                               Position.Y               // Base player Y position
                                               + flt_playerShootY       // Center vertically
                                               + velocity.Y             // Account for possible next movement
                                               );

            BulletManager.Instance.CreateBullet(BulletType.Normal, vc2_shootPos, 0, 1);
        }

        /// <summary>
        ///  Draws the player
        /// </summary>
        public void Draw()
        {
            //PlayerInstance.Position = new Vector2(0, 0);
            //Debug.WriteLine(Position.X);
            //Debug.WriteLine(Position.Y);
            const float boostScale = 1.1f;

            if (boost) //&& totalBoostTime >= boostFrameRate)
            {
                
                foreach (Ghost ghost in ghosts)
                {
                    GameObject.Instance.SpriteBatch.Draw(
                        Sprite,                     // The sprite-sheet for the player
                        ghost.Position,    // The position for the player
                        Transform,                       // The scale and bounding box for the animation
                        ghost.ghostColor,                     // The color for the palyer
                        0.0f,                            // There cannot be any rotation of the player
                        Vector2.Zero,                    // Starting render position
                        PlayerInstance.Scale,                      // The scale of the sprite
                        SpriteEffects.None,              // Which direction the sprite faces
                        0.0f                             // Layer depth of the player is 0.0
                    );
                }
                GameObject.Instance.SpriteBatch.Draw(
                    WhiteSprite,                     // The sprite-sheet for the player
                    Position - new Vector2(Transform.Width,Transform.Height) * (boostScale * 0.1f + 0.0077637999f),    // The position for the player
                    Transform,                       // The scale and bounding box for the animation
                    new Color(Color.Blue,0.0f),                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    PlayerInstance.Scale * boostScale,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );
            }

            GameObject.Instance.SpriteBatch.Draw(
                Sprite,                          // The sprite-sheet for the player
                Position,                        // The position for the player
                Transform,                       // The scale and bounding box for the animation
                Color.White,                     // The color for the palyer
                0.0f,                            // There cannot be any rotation of the player
                Vector2.Zero,                    // Starting render position
                PlayerInstance.Scale,                      // The scale of the sprite
                SpriteEffects.None,              // Which direction the sprite faces
                0.0f                             // Layer depth of the player is 0.0
            );

        }
    }
}