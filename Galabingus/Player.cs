using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

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

        public bool inIFrame
        {
            get
            {
                return iFrame;
            }
        }

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

        public bool GodMode
        {
            get
            {
                return godMode;
            }
        }

        public Vector2 Speed
        {
            get
            {
                return speed;
            }
        }

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

        public static Player PlayerInstance
        {
            get
            {
                return playerInstance;
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
                if (!iFrame && !godMode)
                {
                    float healthBefore = PlayerInstance.health;
                    float healthAfter = value;
                    //PlayerInstance.health = (healthAfter - healthBefore) > healthBefore ? 0.20f + healthBefore : healthBefore + (healthAfter - healthBefore) * 0.60f;

                    if ((healthAfter - healthBefore) < 0)
                    {
                        iFrame = true;
                        if (!triggeredIFrame)
                        {
                            triggeredIFrame = true;
                        }
                    }
                    /*
                    if (health > 4.5f && ((healthAfter - healthBefore) >= healthBefore))
                    {
                        health = 5;
                    }
                    AudioManager.Instance.CallSound("Hit");
                }
            }
        }


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
            //PlayerInstance.Scale = PostScaleRatio();
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

            //float previousFade = 0;

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
                        //previousFade = GameObject.Instance.UniversalShader.Parameters["fade"].GetValueSingle();
                        drawColor = new Color(0.2f, 0.2f, 0.3f, (float)GameObject.ClockTime);
                        //GameObject.Instance.UniversalShader.Parameters["fade"].SetValue(0.7f);
                    }
                    else
                    {
                        drawColor = Color.White;
                    }
                },

                reset =>
                {
                    //GameObject.Instance.UniversalShader.Parameters["fade"].SetValue(previousFade);
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
            if (health < 0.5f)
            {
                health = 0;
            }
            if (health > 5)
            {
                health = 5;
            }
            triggeredIFrame = false;

            PlayerInstance.Collider.Resolved = true;
            PlayerInstance.inputBufferTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bigShotDuration = (float)gameTime.ElapsedGameTime.TotalSeconds * 15.0f;
            //boostFrameRate = PlayerInstance.inputBufferTime;
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
                (ushort)CollisionGroup.Player,                 // Content
                0
            );

            //Vector2 previousVelocity;
            bool collides = false;

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
                (PlayerInstance.Position.Y + PlayerInstance.Transform.Height * Scale) >= GameObject.Instance.GraphicsDevice.Viewport.Height)
                ;
            }

            foreach (Collision collision in intercepts)
            {
                if (collision.other != null && this.Collider.Resolved && ((collision.other as Tile) is Tile))
                {
                    //previousVelocity = velocity;
                    //acceleration = Vector2.Zero;
                    //velocity = Vector2.Zero;
                    //collides = true;
                }
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

                //if (normPreVelocity != normVelocity && normVelocity != Vector2.Zero && normPreVelocity != Vector2.Zero && previousCollision || !collides)
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


            previousCollision = collides;

            if (Math.Abs(velocity.Length()) < 0.001f)
            {
                translation = Vector2.Zero;
            }

            //System.Diagnostics.Debug.WriteLine(previousCollision);

            previousVelocity = velocity;

            totalShootTime += gameTime.ElapsedGameTime.TotalSeconds;
            totalBoostTime += gameTime.ElapsedGameTime.TotalSeconds;
            totalTime += gameTime.ElapsedGameTime.TotalSeconds;
            fadeTimeTotal += gameTime.ElapsedGameTime.TotalSeconds;
            bigShotTotalTime += gameTime.ElapsedGameTime.TotalSeconds;

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

            currentKeyboardState = Keyboard.GetState();

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

                        //Debug.WriteLine();

                        if (previousKeyboardStateX.IsKeyDown(Keys.D) && previousPreviousKeyboardStateX.IsKeyDown(Keys.A) || previousPreviousKeyboardStateX.IsKeyDown(Keys.D) && previousKeyboardStateX.IsKeyDown(Keys.A))
                        {
                            previousPreviousKeyboardStateX = previousKeyboardStateX;
                            velocity.X = 0.0f;
                            if (!currentKeyboardState.IsKeyDown(Keys.W) && !currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                //velocity.Y = velocity.Y / 10.0f;
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
                            //if (previousKeyboardStateY.X)
                            velocity.Y = 0.0f;
                            if (!currentKeyboardState.IsKeyDown(Keys.A) && !currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                //velocity.X = velocity.X / 10.0f;
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
                //shot = false;
            }

            bool bigShooting = false;

            if (bigShot && (bigShotTotalTime >= bigShotDuration))
            {
                realeaseHold = true;
            }

            if (realeaseHold && currentKeyboardState.IsKeyUp(Keys.Space))
            {
                realeaseHold = false;
                bigShooting = true;
                BigShot();
            }

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

                /*
                if (currentKeyboardState.IsKeyUp(Keys.Space) && previousKeyboardState.IsKeyDown(Keys.Space) && !(totalShootTime <= shootDuration * 0.3f))
                {
                    totalShootTime = 0;
                }

                if (previousKeyboardState.IsKeyDown(Keys.Space))
                {
                    holdShoot = true;
                }
                if ( (currentKeyboardState.IsKeyUp(Keys.Space) && previousKeyboardState.IsKeyDown(Keys.Space)) || holdShoot && (godMode || !(totalShootTime >= shootDuration * 0.5f)))
                {
                    Shoot();
                }
                if (currentKeyboardState.IsKeyUp(Keys.Space))
                {
                    holdShoot = false;
                }
                */
            }
            else if (previousKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space) && !realeaseHold && !(bigShotTotalTime >= bigShotDuration) || previousKeyboardState.IsKeyUp(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (!bigShooting)
                {
                    Shoot();
                }
            }

            if (previousKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space) && (bigShotTotalTime >= bigShotDuration))
            {
                bigShot = true;
            }
            else if (currentKeyboardState.IsKeyUp(Keys.Space))
            {
                bigShot = false;
            }

            /*
            foreach (Keys key in currentKeyboardState.GetPressedKeys())
            {
                Debug.WriteLine(key);
            }
            */

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

                if (currentKeyboardState.IsKeyUp(Keys.LeftShift) && currentKeyboardState.IsKeyUp(Keys.RightShift))
                {
                    //boostOpacity = 1f;
                    boost = false;
                    //List<Ghost> newGhost = new List<Ghost>();
                    //ghosts = newGhost;
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
                        ghostBoost.Position = Position;// + normVelocity * 0.1f * (float)Animation.EllapsedTime * new Vector2(speed.X, speed.Y).LengthSquared() * ((1 - boostSpeed) * -0.1f );
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

            if (godMode)
            {
                Player.PlayerInstance.health = 5;
            }

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
                //holdShoot = false;
                totalShootTime -= shootDuration;
            }

            if (bigShotTotalTime >= bigShotDuration)
            {
                //holdShoot = false;
                bigShotTotalTime -= bigShotDuration;
            }

            //Debug.WriteLine();
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

                if (Player.PlayerInstance.Health > 0)
                {
                    if (boost) //&& totalBoostTime >= boostFrameRate)
                    {
                        foreach (Ghost ghost in ghosts)
                        {
                            Color halfOColor = ghost.ghostColor;//new Color(ghost.ghostColor * 0.825f, 0.825f);
                            if (halfOColor.B <= 5)
                            {
                                halfOColor = Color.Transparent;
                            }
                            GameObject.Instance.SpriteBatch.Draw(
                                WhiteSprite,                     // The sprite-sheet for the player
                                ghost.Position,    // The position for the player
                                Transform,                       // The scale and bounding box for the animation
                                halfOColor,                     // The color for the palyer
                                0.0f,                            // There cannot be any rotation of the player
                                Vector2.Zero,                    // Starting render position
                                PlayerInstance.Scale,                      // The scale of the sprite
                                SpriteEffects.None,              // Which direction the sprite faces
                                0.0f                             // Layer depth of the player is 0.0
                            );
                        }
                    }

                    Texture2D outline = GameObject.Instance.ContentManager.Load<Texture2D>("Player/player_outline_strip4");

                    if (realeaseHold)
                    {
                        GameObject.Instance.SpriteBatch.Draw(
                            outline,                          // The sprite-sheet for the player
                            Position - new Vector2(4*Scale,4*Scale),                        // The position for the player
                            new Rectangle(Transform.X / Sprite.Width * outline.Width + Transform.X / Sprite.Width, 0, (int)(outline.Width / 4), (int)(outline.Height)),                       // The scale and bounding box for the animation
                            new Color(Color.LightBlue,0.05f),                     // The color for the palyer
                            0.0f,                            // There cannot be any rotation of the player
                            Vector2.Zero,                    // Starting render position
                            PlayerInstance.Scale,                      // The scale of the sprite
                            SpriteEffects.None,              // Which direction the sprite faces
                            0.0f                             // Layer depth of the player is 0.0
                        );
                    }
                    GameObject.Instance.SpriteBatch.Draw(
                        Sprite,                          // The sprite-sheet for the player
                        Position,                        // The position for the player
                        Transform,                       // The scale and bounding box for the animation
                        drawColor,                     // The color for the palyer
                        0.0f,                            // There cannot be any rotation of the player
                        Vector2.Zero,                    // Starting render position
                        PlayerInstance.Scale,                      // The scale of the sprite
                        SpriteEffects.None,              // Which direction the sprite faces
                        0.0f                             // Layer depth of the player is 0.0
                    );
                }
                else if (godMode)
                {
                    Collider.Resolved = false;
                    UIManager.Instance.GS = GameState.PlayerDead;
                }

                bool largeHealthCondition = (new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position).Y + halfHeartSprite.Height * 0.5f > GameObject.Instance.GraphicsDevice.Viewport.Height;

                GameObject.Instance.SpriteBatch.Draw(
                    heartSprite,                          // The sprite-sheet for the player
                    largeHealthCondition ? new Vector2(-1, -20) + Position : new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position,                        // The position for the player
                    new Rectangle(0, 0, halfHeartSprite.Width * 5, halfHeartSprite.Height),                       // The scale and bounding box for the animation
                    new Color(Color.Gray, 0.9f),                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    0.6f,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );

                GameObject.Instance.SpriteBatch.Draw(
                    halfHeartSprite,                          // The sprite-sheet for the player
                    largeHealthCondition ? new Vector2(-1, -20) + Position : new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position,                        // The position for the player
                    new Rectangle(0, 0, halfHeartSprite.Width * (int)Math.Clamp((int)Math.Round(playerInstance.Health, MidpointRounding.AwayFromZero), 0, 5), halfHeartSprite.Height),                       // The scale and bounding box for the animation
                    new Color(Color.White, 0.9f),                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    0.6f,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );

                GameObject.Instance.SpriteBatch.Draw(
                    fullHeartSprite,                          // The sprite-sheet for the player
                    largeHealthCondition ? new Vector2(-1, -20) + Position : new Vector2(-1, 20 + PlayerInstance.Transform.Height * Scale) + Position,                        // The position for the player
                    new Rectangle(0, 0, halfHeartSprite.Width * (int)Math.Clamp(Math.Floor(playerInstance.Health), 0, 5), halfHeartSprite.Height),                       // The scale and bounding box for the animation
                    new Color(Color.White, 0.9f),                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    0.6f,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );
            }
            );
        }

        public void CreateHealthBar(float health, float max, int x, int y, float xSize, float ySize, Color backColor, Color frontColor)
        {
            float healthRatio = health / max;
            xSize = xSize / max;
            Texture2D pixelWhite = GameObject.Instance.ContentManager.Load<Texture2D>("white_pixel_strip1");

            GameObject.Instance.Debug += delegate (SpriteBatch spriteBatch)
            {
                GameObject.Instance.SpriteBatch.Draw(
                    pixelWhite,                          // The sprite-sheet for the player
                    new Vector2(x, y),                        // The position for the player
                    new Rectangle(0, 0, 1 * (int)max * (int)xSize, (int)(ySize * 1.3f)),                       // The scale and bounding box for the animation
                    new Color(backColor, 0.9f),                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    1.0f,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );
                GameObject.Instance.SpriteBatch.Draw(
                    pixelWhite,                          // The sprite-sheet for the player
                    new Vector2(x, y),                        // The position for the player
                    new Rectangle(0, 0, 1 * (int)Math.Clamp((int)Math.Round(health, MidpointRounding.AwayFromZero), 0, max) * (int)xSize, (int)ySize),                       // The scale and bounding box for the animation
                    new Color(frontColor, 0.9f),                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    1.0f,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );
            };
        }

        public static void EnableGodMode()
        {
            Player.PlayerInstance.godMode = true;
        }

        public static void DisableGodMode()
        {
            Player.PlayerInstance.godMode = false;
        }

        public void Reset()
        {
            playerInstance = null;
        }
    }
}