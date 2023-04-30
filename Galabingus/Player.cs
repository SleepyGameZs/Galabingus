using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

// Matthew Rodriguez
// 2023, 4, 30
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
        /// <summary>
        ///  The trail is made of these
        ///  Color defining and position relation to the orignal sprite
        /// </summary>
        private struct Ghost
        {
            public Vector2 Position;
            public float boostOpacity;
            public Color ghostColor;
        }

        // Feilds
        private static Player playerInstance = null;
        private KeyboardState previousPreviousKeyboardStateX;
        private KeyboardState previousPreviousKeyboardStateY;
        private KeyboardState previousKeyboardState;
        private KeyboardState previousKeyboardStateX;
        private KeyboardState previousKeyboardStateY;
        private KeyboardState currentKeyboardState; 
        private Vector2 previousVelocity;         
        private Vector2 velocity;                 
        private Vector2 speed;                   
        private Vector2 acceleration;              
        private Vector2 translationRatio;          
        private PlayerStates playerState;          
        private Keys deAccFrom;                   
        private double totalTime;                   
        private float delayBufferTime;             
        private float inputBufferTime;              
        private ushort contentName;                 
        private float boostFrameRate;
        private float health;
        private bool previousCollision;
        private bool shot;
        private bool boost;
        private bool iFrame;
        private float boostSpeed;
        private List<Ghost> ghosts = new List<Ghost>();
        private Vector2 boostSpawnGhost;
        private float boostOpacity;
        private bool shiftBoost;
        private double totalBoostTime;
        private Texture2D heartSprite;
        private Texture2D halfHeartSprite;
        private Texture2D fullHeartSprite;
        private bool cameraLock;
        private Vector2 translation;
        private Text textTest;
        private bool tSet;
        private float fadeDuration;
        private bool triggeredIFrame;
        private double fadeTimeTotal;
        private bool godMode;
        private bool holdShoot;
        private double totalShootTime;
        private float shootDuration;
        private bool bigShot;
        private float bigShotDuration;
        private double bigShotTotalTime;
        private bool realeaseHold;
        private GameObjectMaterial material;
        private GameObjectMaterialNode universalNode;
        private GameObjectMaterialNode hitEffectNode;
        private Color drawColor;

        /// <summary>
        ///  Whether or not the player is in a iFrame
        /// </summary>
        public bool inIFrame
        {
            get
            {
                return iFrame;
            }
        }

        /// <summary>
        ///  Actual translation applied to the player
        /// </summary>
        public Vector2 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                tSet = true;
                translation = value;
            }
        }

        /// <summary>
        ///  Don't die and Don't touch tiles 
        /// </summary>
        public bool GodMode
        {
            get
            {
                return godMode;
            }
        }

        /// <summary>
        ///  Speed of the player
        /// </summary>
        public Vector2 Speed
        {
            get
            {
                return speed;
            }
        }

        /// <summary>
        ///  If the camera is lcoked
        /// </summary>
        public bool CameraLock
        {
            get
            {
                return cameraLock;
            }
            set
            {
                cameraLock = value;
            }
        }

        /// <summary>
        ///  Player Instance
        /// </summary>
        public static Player PlayerInstance
        {
            get
            {
                return playerInstance;
            }
        }

        /// <summary>
        ///  Content dynamic identifer for the player
        /// </summary>
        public new ushort ContentName
        {
            get
            {
                return PlayerInstance.contentName;
            }
        }

        /// <summary>
        ///  Health of the player
        /// </summary>
        public float Health
        {
            get
            {
                return PlayerInstance.health;
            }
            set
            {
                // Only do damage when there is no iFrame and not in godMode
                if (!iFrame && !godMode)
                {
                    float healthBefore = PlayerInstance.health;
                    float healthAfter = value;
                    PlayerInstance.health = (healthAfter - healthBefore) > healthBefore ? 0.20f + healthBefore : healthBefore + (healthAfter - healthBefore) * 0.60f;

                    // Trigger iFrames when damage is done
                    if ((healthAfter - healthBefore) < 0)
                    {
                        iFrame = true;
                        if (!triggeredIFrame)
                        {
                            triggeredIFrame = true;
                        }
                    }
                    
                    // Heal to max
                    if (health > 4.5f && ((healthAfter - healthBefore) >= healthBefore))
                    {
                        health = 5;
                    }

                    // Play the hit sound effect
                    AudioManager.Instance.CallSound("Hit");
                }
            }
        }

        /// <summary>
        ///  The white sprite for the player
        /// </summary>
        public Texture2D WhiteSprite
        {
            get
            {
                GameObject.Instance.Content = GameObject.Instance.Content.player_white_strip4;
                if (GetSprite(0) == null)
                {
                    LoadSprite(GameObject.Instance.Content.player_white_strip4, 0);
                }
                return GetSprite(0);
            }
        }

        /// <summary>
        ///  Velocity of the player
        /// </summary>
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
        }

        /// <summary>
        ///  The ratio at which the player is translating on screen
        /// </summary>
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
        public new Vector2 Position
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
        public new Rectangle Transform
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
            base(contentName, 0, CollisionGroup.Player)
        {
            if (playerInstance == null)
            {
                playerInstance = this;
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
            PlayerInstance.Scale = 3.0f;
            PlayerInstance.Animation.AnimationDuration = 0.05f;

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
            PlayerInstance.boostSpeed = 2.125f;
            PlayerInstance.boostFrameRate = 0.002479166648f;
            PlayerInstance.boostOpacity = 1;
            PlayerInstance.boostSpawnGhost = Vector2.Zero;
            PlayerInstance.shiftBoost = false;
            PlayerInstance.ghosts = new List<Ghost>();
            this.thisGameObject = this;
            PlayerInstance.fullHeartSprite = GameObject.Instance.ContentManager.Load<Texture2D>("Player/player_heart_full_strip1");
            PlayerInstance.halfHeartSprite = GameObject.Instance.ContentManager.Load<Texture2D>("Player/player_heart_half_strip1");
            PlayerInstance.heartSprite = GameObject.Instance.ContentManager.Load<Texture2D>("Player/player_heart_empty_strip1");
            PlayerInstance.cameraLock = true;
            textTest = UIManager.Instance.AddText("Testing", Vector2.Zero, 12, Color.White, UIState.BaseGame);
            iFrame = false;
            tSet = false;
            godMode = false;
            holdShoot = false;
            fadeDuration = 0.7f;
            fadeTimeTotal = 0;
            totalShootTime = 0;
            shootDuration = 0.1f;
            health = 5;
            bigShot = false;
            bigShotDuration = 0.3f;
            bigShotTotalTime = 0;
            realeaseHold = false;
            triggeredIFrame = false;
            drawColor = Color.White;

            // Create a matiral node
            universalNode = new GameObjectMaterialNode(GameObject.Instance.UniversalShader,

                setup =>
                {
                    if (Player.PlayerInstance.inIFrame)
                    {
                        Player.PlayerInstance.material.SkipUniversalPass();
                    }
                },

                properties => {
                    if (Player.PlayerInstance.inIFrame)
                    {
                        drawColor = new Color(0.2f, 0.2f, 0.3f, (float)GameObject.ClockTime);
                    }
                    else
                    {
                        drawColor = Color.White;
                    }
                },

                reset =>
                {
                    // No reset
                }
            );
            material = new GameObjectMaterial();
            material.AddMaterialNode(universalNode);
        }

        /// <summary>
        ///  Updates the player Finite State Machine
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Clamp the health
            if (health < 0.5f)
            {
                health = 0;
            }
            if (health > 5)
            {
                health = 5;
            }

            // Setup for update
            triggeredIFrame = false;
            PlayerInstance.Collider.Resolved = true;
            PlayerInstance.inputBufferTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bigShotDuration = (float)gameTime.ElapsedGameTime.TotalSeconds * 15.0f;
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

            // First pass of Collison updates
            // Update the animation for the player
            PlayerInstance.Transform = PlayerInstance.Animation.Play(gameTime);
            List<Collision> intercepts = PlayerInstance.Collider.UpdateTransform(
                PlayerInstance.Sprite,                         // Player Sprite
                PlayerInstance.Position,                       // Player position
                PlayerInstance.Transform,                      // Player transform for sprite selection
                GameObject.Instance.GraphicsDevice,
                GameObject.Instance.SpriteBatch,
                PlayerInstance.Scale,                          // Player scale
                SpriteEffects.None,
                (ushort)CollisionGroup.Player,                 // Content
                0
            );
            bool collides = false;

            // Off-Screen Collision
            if (
                ((PlayerInstance.Position.Y <= 0 || PlayerInstance.Position.X <= 0) ||
                (PlayerInstance.Position.X + PlayerInstance.Transform.Width * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Width ||
                (PlayerInstance.Position.Y + PlayerInstance.Transform.Height * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Height)
                )
            {
                previousVelocity = velocity;
                if (PlayerInstance.Position.Y <= 0)
                {
                    velocity.Y = speed.Y;
                }
                else if ((PlayerInstance.Position.Y + PlayerInstance.Transform.Height * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Height)
                {
                    Player.playerInstance.Health = Player.PlayerInstance.Health - 1;
                    velocity.Y = -speed.Y;
                }
                if (PlayerInstance.Position.X <= 0)
                {
                    velocity.X = speed.X;
                }
                else if ((PlayerInstance.Position.X + PlayerInstance.Transform.Width * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Width)
                {
                    velocity.X = -speed.X;
                }
                collides = true;

                Position += velocity * 0.5f;
            }
            else if (previousCollision)
            {
                collides = ((PlayerInstance.Position.Y <= 0 || PlayerInstance.Position.X <= 0) ||
                (PlayerInstance.Position.X + PlayerInstance.Transform.Width * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Width ||
                (PlayerInstance.Position.Y + PlayerInstance.Transform.Height * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Height);
            }

            Vector2 normPreVelocity = previousVelocity;
            Vector2 normVelocity = velocity;

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

                // Velocity normilization
                // WIth clamps
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
            }
            else
            {
                // Velocity normilization
                // WIth clamps
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

                // Translate the player
                {
                    if (!tSet)
                    {
                        if (boost)
                        {
                            translation = (velocity == Vector2.Zero ? velocity : Vector2.Normalize(velocity) * (float)Animation.EllapsedTime * speed * translationAjdustedRatio);
                            translation.X = MathHelper.Lerp(translation.X, translation.X * boostSpeed, 0.5f);
                            translation.Y = MathHelper.Lerp(translation.Y, translation.Y * boostSpeed, 0.5f);
                        }
                        else
                        {
                            translation = (velocity == Vector2.Zero ? velocity : Vector2.Normalize(velocity) * (float)Animation.EllapsedTime * speed * translationAjdustedRatio);
                        }
                    }
                    else
                    {
                        translation = Vector2.Zero;
                    }
                    Position += translation;
                }
            }
            tSet = false;

            // Set the previous velocity
            previousCollision = collides;
            if (Math.Abs(velocity.Length()) < 0.001f)
            {
                translation = Vector2.Zero;
            }
            previousVelocity = velocity;
            
            // Update timers total time
            totalShootTime += gameTime.ElapsedGameTime.TotalSeconds;
            totalBoostTime += gameTime.ElapsedGameTime.TotalSeconds;
            totalTime += gameTime.ElapsedGameTime.TotalSeconds;
            fadeTimeTotal += gameTime.ElapsedGameTime.TotalSeconds;
            bigShotTotalTime += gameTime.ElapsedGameTime.TotalSeconds;

            // Update the player collider
            intercepts = PlayerInstance.Collider.UpdateTransform(
                PlayerInstance.Sprite,                         // Player Sprite
                PlayerInstance.Position,                       // Player position
                PlayerInstance.Transform,                      // Player transform for sprite selection
                GameObject.Instance.GraphicsDevice,
                GameObject.Instance.SpriteBatch,
                PlayerInstance.Scale,                          // Player scale
                SpriteEffects.None,
                (ushort)CollisionGroup.Player,                 // Content
                0
            );

            // Update the current keyboard state
            currentKeyboardState = Keyboard.GetState();

            // Handle the player translaiton states
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
                        if (previousKeyboardStateX.IsKeyDown(Keys.D) && previousPreviousKeyboardStateX.IsKeyDown(Keys.A) || previousPreviousKeyboardStateX.IsKeyDown(Keys.D) && previousKeyboardStateX.IsKeyDown(Keys.A))
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                            velocity.X = 0.0f;
                            if (!currentKeyboardState.IsKeyDown(Keys.W) && !currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                acceleration.Y = acceleration.Y / 10f;
                                acceleration.X = acceleration.X / 10f;
                            }
                            xPause = false;
                        }
                        else
                        {
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
                        }

                        if (xPause)
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                        }

                        if (previousKeyboardStateY.IsKeyDown(Keys.S) && previousPreviousKeyboardStateY.IsKeyDown(Keys.W) || previousPreviousKeyboardStateY.IsKeyDown(Keys.S) && previousKeyboardStateY.IsKeyDown(Keys.W))
                        {
                            previousPreviousKeyboardStateY = previousKeyboardStateY;
                            velocity.Y = 0.0f;
                            if (!currentKeyboardState.IsKeyDown(Keys.A) && !currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                acceleration.X = acceleration.X / 10f;
                                acceleration.Y = acceleration.Y / 10f;
                            }
                            yPause = false;
                        }
                        else
                        {
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
                        }
                        if (yPause)
                        {
                            previousPreviousKeyboardStateY = previousKeyboardStateY;
                        }
                        if (previousKeyboardState.IsKeyDown(Keys.LeftShift) || previousKeyboardState.IsKeyDown(Keys.RightShift))
                        {
                            boost = true;
                        }
                        break;
                }
            }
            bool bigShooting = false;

            // Ready the big shot
            if (bigShot && (bigShotTotalTime >= bigShotDuration))
            {
                realeaseHold = true;
                AudioManager.Instance.CallSoundOnce("Charge");
            }

            // Shoot a big shot
            if (realeaseHold && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                realeaseHold = false;
                bigShooting = true;
                BigShot();
                AudioManager.Instance.StopSound("Charge");
            }
            
            // Maage shooting normally and in godmode
            if (!bigShot || godMode)
            {
                realeaseHold = false;
                if (previousKeyboardState.IsKeyDown(Keys.G) && currentKeyboardState.IsKeyUp(Keys.G))
                {
                    godMode = !godMode;
                }

                // When space is pressed trigger shoot
                if ((currentKeyboardState.IsKeyUp(Keys.Space) && previousKeyboardState.IsKeyDown(Keys.Space)))
                {
                    Shoot();
                }

                if (previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    holdShoot = true;
                }
                if (holdShoot && godMode)
                {
                    Shoot();
                }
                if (currentKeyboardState.IsKeyUp(Keys.Space))
                {
                    holdShoot = false;
                }
            }
            else if (previousKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space) && !realeaseHold && !(bigShotTotalTime >= bigShotDuration) || previousKeyboardState.IsKeyUp(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (!bigShooting)
                {
                    Shoot();
                }
            }

            // Trigger the bigshot charge
            if (previousKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space) && (bigShotTotalTime >= bigShotDuration))
            {
                bigShot = true;
            }
            else if (currentKeyboardState.IsKeyUp(Keys.Space))
            {
                bigShot = false;
            }

            // When the ellapsed time is the buffer time update the keyboard state
            // Update the ghost
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
                
                // Create the ghost
                if (currentKeyboardState.IsKeyUp(Keys.LeftShift) && currentKeyboardState.IsKeyUp(Keys.RightShift))
                {
                    boost = false;
                    if (totalBoostTime >= boostFrameRate * 0.3333333f)
                    {
                        Ghost ghostBoost = new Ghost();
                        ghostBoost.ghostColor = new Color(new Color(1.5f * 0.1f, 1.5f * 0.1f, 1.5f * 0.1f), 1.0f);
                        ghostBoost.Position = Position + normVelocity * (float)Animation.EllapsedTime * new Vector2(speed.X, speed.Y).LengthSquared() * ((1 - boostSpeed) * -0.1f);
                        boostSpeed *= (float)Animation.EllapsedTime;
                        boostOpacity -= 0.05f;
                        ghostBoost.boostOpacity = boostOpacity;
                        ghosts.Add(ghostBoost);
                    }
                }
                else
                {
                    if (totalBoostTime >= boostFrameRate * 0.3333333f)
                    {
                        Ghost ghostBoost = new Ghost();
                        ghostBoost.ghostColor = new Color(new Color(0, 35, 85), 1.0f);
                        ghostBoost.Position = Position;
                        boostSpeed *= (float)Animation.EllapsedTime;
                        boostOpacity -= 0.0005f;
                        ghostBoost.boostOpacity = boostOpacity;
                        ghosts.Add(ghostBoost);
                    }
                }
                totalTime -= bufferTime;
            }

            // Update the boost timer
            if (totalBoostTime >= boostFrameRate)
            {
                totalBoostTime -= boostFrameRate;
            }

            // Hold the player health at 5 in godmode
            if (godMode)
            {
                Player.PlayerInstance.health = 5;
            }

            // Adjust the paralax for the camera
            if (cameraLock)
            {
                if (!Camera.Instance.Stopped)
                {
                    Camera.Instance.OffSet = new Vector2(Math.Clamp((normVelocity.X) + Math.Clamp(Camera.Instance.OffSet.X, -0.005f, 0.005f), -0.5f, 0.5f), Math.Clamp((normVelocity.Y) * 0.005f + Math.Clamp((Camera.Instance.OffSet.Y), -2.5f, 2.5f), -2.5f, 2.5f))
                        * (float)Animation.GetElapsedTime(gameTime, Vector2.Zero, new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.5f), Transform, Scale);
                }
                else
                {
                    Camera.Instance.OffSet = new Vector2(Math.Clamp((normVelocity.X), -1f, 1f), Math.Clamp((normVelocity.Y), -0, 0))
                        * (float)Animation.GetElapsedTime(gameTime, Vector2.Zero, new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.5f), Transform, Scale);
                }
            }

            // Update all timers
            if (fadeTimeTotal >= fadeDuration && iFrame)
            {
                fadeTimeTotal -= fadeDuration;
                iFrame = false;
            }
            else if (fadeTimeTotal >= fadeDuration)
            {
                fadeTimeTotal -= fadeDuration;
            }
            if (totalShootTime >= shootDuration)
            {
                totalShootTime -= shootDuration;
            }
            if (bigShotTotalTime >= bigShotDuration)
            {
                bigShotTotalTime -= bigShotDuration;
            }
        }

        /// <summary>
        ///  Triggered when the
        ///  Player holds the shoot button
        /// </summary>
        public void BigShot()
        {
            BulletManager.Instance.CreateBullet(BulletType.BigShot, Position, new Vector2(0, -1), this, false);
        }

        /// <summary>
        ///  Triggered when the 
        ///  Player presses the shoot button
        /// </summary>
        public void Shoot()
        {
            BulletManager.Instance.CreateBullet(BulletType.PlayerNormal, Position, new Vector2(0, -1), this, false);
        }

        /// <summary>
        ///  Draws the player
        /// </summary>
        public void Draw()
        {
            material.Draw(draw =>
            {
                // Draw the player text when the player is alive
                if (health > 0)
                {
                    textTest.UIText = "Player";
                    textTest.UITextPosition = Position + new Vector2(textTest.UIText.Length * 3 * 0.5f, PlayerInstance.Transform.Height * Scale + 1);
                }
                else
                {
                    textTest.UITextPosition = new Vector2(-100, -100);
                    textTest.UIText = "";
                }

                // When teh player is alive draw the player
                if (Player.PlayerInstance.Health > 0)
                {
                    // Draw the boost ghost trail
                    if (boost)
                    {
                        foreach (Ghost ghost in ghosts)
                        {
                            Color halfOColor = ghost.ghostColor;
                            if (halfOColor.B <= 5)
                            {
                                halfOColor = Color.Transparent;
                            }
                            GameObject.Instance.SpriteBatch.Draw(
                                WhiteSprite,
                                ghost.Position,
                                Transform,
                                halfOColor,
                                0.0f,
                                Vector2.Zero,
                                PlayerInstance.Scale,
                                SpriteEffects.None,
                                0.0f
                            );
                        }
                    }

                    // Load the outline sprite with the standard content manager
                    Texture2D outline = GameObject.Instance.ContentManager.Load<Texture2D>("Player/player_outline_strip4");

                    // Draw the outline when the palyer is big-shoting
                    if (realeaseHold)
                    {
                        GameObject.Instance.SpriteBatch.Draw(
                            outline,
                            Position - new Vector2(4*Scale,4*Scale),
                            new Rectangle(Transform.X / Sprite.Width * outline.Width + Transform.X / Sprite.Width, 0, (int)(outline.Width / 4), (int)(outline.Height)),
                            new Color(Color.LightBlue,0.05f),
                            0.0f,
                            Vector2.Zero,
                            PlayerInstance.Scale,
                            SpriteEffects.None,
                            0.0f
                        );
                    }

                    // Draw the player sprite
                    GameObject.Instance.SpriteBatch.Draw(
                        Sprite,
                        Position,
                        Transform,
                        drawColor,
                        0.0f,
                        Vector2.Zero,
                        PlayerInstance.Scale,
                        SpriteEffects.None,
                        0.0f
                    );
                }
                else if (godMode)
                {
                    // If the player is not in godmode then change the game state to PlayerDead
                    Collider.Resolved = false;
                    UIManager.Instance.GS = GameState.PlayerDead;
                }

                // Determine if the health bar is offscreen
                bool largeHealthCondition = (new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position).Y + halfHeartSprite.Height * 0.5f > GameObject.Instance.GraphicsDevice.Viewport.Height;

                // Draw the background heart based on teh health of the player
                GameObject.Instance.SpriteBatch.Draw(
                    heartSprite,
                    largeHealthCondition ? new Vector2(-1, -20) + Position : new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position,
                    new Rectangle(0, 0, halfHeartSprite.Width * 5, halfHeartSprite.Height),
                    new Color(Color.Gray, 0.9f),
                    0.0f,
                    Vector2.Zero,
                    0.6f,
                    SpriteEffects.None,
                    0.0f
                );

                // Draw the half hearts based on the health of the player
                GameObject.Instance.SpriteBatch.Draw(
                    halfHeartSprite,
                    largeHealthCondition ? new Vector2(-1, -20) + Position : new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position,
                    new Rectangle(0, 0, halfHeartSprite.Width * (int)Math.Clamp((int)Math.Round(playerInstance.Health, MidpointRounding.AwayFromZero), 0, 5), halfHeartSprite.Height),
                    new Color(Color.White, 0.9f),
                    0.0f,
                    Vector2.Zero,
                    0.6f,
                    SpriteEffects.None,
                    0.0f
                );

                // Daw the full hearts based on the health of the player
                GameObject.Instance.SpriteBatch.Draw(
                    fullHeartSprite,
                    largeHealthCondition ? new Vector2(-1, -20) + Position : new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position,
                    new Rectangle(0, 0, halfHeartSprite.Width * (int)Math.Clamp(Math.Floor(playerInstance.Health), 0, 5), halfHeartSprite.Height),
                    new Color(Color.White, 0.9f),
                    0.0f,
                    Vector2.Zero,
                    0.6f, 
                    SpriteEffects.None,
                    0.0f 
                );
            }
            );
        }

        /// <summary>
        ///  Draws A health bar
        /// </summary>
        /// <param name="health">Amount of health</param>
        /// <param name="max">Maximum amoutn of health</param>
        /// <param name="x">x position of the health bar</param>
        /// <param name="y">y position of the health bar</param>
        /// <param name="xSize">horizontal size of the health bar</param>
        /// <param name="ySize">vertical size of the health bar</param>
        /// <param name="backColor">Color behind the healt bar</param>
        /// <param name="frontColor">Color of the health bar</param>
        public void CreateHealthBar(float health, float max, int x, int y, float xSize, float ySize, Color backColor, Color frontColor)
        {
            float healthRatio = health / max;
            xSize = xSize / max;
            Texture2D pixelWhite = GameObject.Instance.ContentManager.Load<Texture2D>("white_pixel_strip1");

            // Use the Debug to draw the Health Bar
            GameObject.Instance.Debug += delegate (SpriteBatch spriteBatch)
            {
                GameObject.Instance.SpriteBatch.Draw(
                    pixelWhite,
                    new Vector2(x, y),
                    new Rectangle(0, 0, 1 * (int)max * (int)xSize, (int)(ySize * 1.3f)),
                    new Color(backColor, 0.9f),
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0.0f
                );
                GameObject.Instance.SpriteBatch.Draw(
                    pixelWhite,
                    new Vector2(x, y),
                    new Rectangle(0, 0, 1 * (int)Math.Clamp((int)Math.Round(health, MidpointRounding.AwayFromZero), 0, max) * (int)xSize, (int)ySize),
                    new Color(frontColor, 0.9f),
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0.0f
                );
            };
        }

        /// <summary>
        ///  Enables godmode
        /// </summary>
        public static void EnableGodMode()
        {
            Player.PlayerInstance.godMode = true;
        }

        /// <summary>
        ///  Disables god mdoe
        /// </summary>
        public static void DisableGodMode()
        {
            Player.PlayerInstance.godMode = false;
        }

        /// <summary>
        ///  Deletes the player text
        /// </summary>
        public static void RemovePlayerText()
        {
            Player.PlayerInstance.textTest.UITextPosition = new Vector2(-100, -100);
            Player.PlayerInstance.textTest.UIText = "";
        }

        /// <summary>
        ///  Resets the player instacne
        /// </summary>
        public new void Reset()
        {
            playerInstance = null;
        }
    }
}