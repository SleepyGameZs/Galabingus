using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    /* The camera class scrolls the objects on screen when in motion.
     * Causing motion by scrolling the background and player.
     * Pushing F5 causes the camera to lock and allow for free roaming around the level. */

    internal class Camera
    {
        #region Fields

        // The instance of the camera
        private static Camera instance = null;

        // The position of the camera
        private Vector2 position;

        // The inital scrolling speed of the camera
        private float initalCameraScroll;
        
        // The scrolling speed of the camera
        private Vector2 offSet;

        // If the camera is locked
        private bool cameraLock;

        // If the camera is stoped 
        private bool stop;

        #endregion

        #region Properties

        /// <summary>
        /// Referance to the camera 
        /// </summary>
        public static Camera Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Camera();
                }
                return instance;
            }

        }

        /// <summary>
        /// The position of the camera
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Intial scrolling speed of the camera
        /// </summary>
        public float InitalCameraScroll
        {
            get { return initalCameraScroll; }
            set { initalCameraScroll = value; }
        }

        /// <summary>
        /// Scrolling speed of the camera
        /// </summary>
        public Vector2 OffSet
        {
            get { return offSet; }
            set { offSet = value; }
        }

        /// <summary>
        /// If the camera is stopped
        /// </summary>
        public bool Stopped
        {
            get 
            {
                return stop;
            }
        }

        /// <summary>
        /// If the camera is locked 
        /// </summary>
        public bool CameraLock
        {
            get
            {
                return cameraLock;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Deafult Constuctor which sets inital scroll speed to 2f
        /// </summary>
        public Camera()
        {
            // Set inital scroll speed
            initalCameraScroll = 2f;
            offSet = new Vector2(0, -initalCameraScroll);

            // Reset stop bool
            stop = false;

            // Set position to zero
            position = Vector2.Zero;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the movement of the camera
        /// </summary>
        public void Start()
        {
            // Allow for player to affect camera
            Player.PlayerInstance.CameraLock = true;

            // Reset stop and lock bools
            stop = false;
            cameraLock = false;

            // Set speed back to inital speed
            offSet.Y = initalCameraScroll;
        }

        /// <summary>
        /// Stop the movement of the camera 
        /// </summary>
        public void Stop()
        {
            // Allow for player to affect camera
            Player.PlayerInstance.CameraLock = true;

            // Update bools 
            stop = true;
            cameraLock = false;
        }

        /// <summary>
        /// Reverses the scrolling of the camera
        /// </summary>
        public void Reverse()
        {
            // Allow for player to affect camera
            Player.PlayerInstance.CameraLock = true;

            // Reverse the scrolling offset
            offSet.Y = -offSet.Y;
        }

        /// <summary>
        /// Checks for scrolling changes and updates the postion of the camera 
        /// </summary>
        /// <param name="gameTime"> Used to get the correct pace </param>
        public void Update(GameTime gameTime)
        {
            // Update camera position
            Camera.Instance.position += offSet;

            // Caping of the camera speed on the way back
            if (Camera.Instance.OffSet.Y > 2.5)
            {
                offSet.Y = 2.5f;
            }

            // Camera lock mode acitvation on F5
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                cameraLock = true;
            }

            // Camera lock mode
            if (cameraLock == true)
            {
                // Horizontal player movment no longer has an effect on the camera
                Player.PlayerInstance.CameraLock = false;

                // Move camera based on the player's movement 
                Camera.Instance.offSet.Y = Player.PlayerInstance.Velocity == Vector2.Zero ? Vector2.Zero.Y :
                    Player.PlayerInstance.Translation.Y;

                // Lock the player in place by scrolling them by their own velocity
                Player.PlayerInstance.Position -= new Vector2(0, Camera.instance.offSet.Y);
            }
        }

        /// <summary>
        /// Resets the singelton
        /// </summary>
        public void Reset()
        {
            instance = null;
        }

        #endregion
    }
}
