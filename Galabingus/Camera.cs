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
    internal class Camera
    {
        private static Camera instance = null;

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

        // -------------------------------------------------
        // Fields
        // -------------------------------------------------

        private int x;
        private int y;
        private float initalCameraScroll;
        private Vector2 offSet;
        private bool cameraLock;
        private bool stop;
        private Vector2 position;

        // -------------------------------------------------
        // Properties
        // -------------------------------------------------

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public float InitalCameraScroll
        {
            get { return initalCameraScroll; }
            set { initalCameraScroll = value; }
        }

        public Vector2 OffSet
        {
            get { return offSet; }
            set { offSet = value; }
        }

        public bool Stopped
        {
            get 
            {
                return stop;
            }
        }

        public bool CameraLock
        {
            get
            {
                return cameraLock;
            }
        }

        // -------------------------------------------------
        // Contructors
        // -------------------------------------------------

        public Camera()
        {
            x = 0;
            y = 0;
            initalCameraScroll = 2f;
            offSet = new Vector2(0, -initalCameraScroll);
            stop = false;
            position = Vector2.Zero;

        }

        public Camera(int cameraScroll)
        {
            x = 0;
            y = 0;
            this.initalCameraScroll = cameraScroll;
            offSet = new Vector2(0, -initalCameraScroll);
            position = Vector2.Zero;
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void Start()
        {
            stop = false;
            cameraLock = false;
            offSet.Y = initalCameraScroll;
        }

        public void Stop()
        {
            stop = true;
            cameraLock = false;
            offSet.X = 0;
            offSet.Y = 0;
        }

        public void Reverse()
        {
            offSet.Y = -initalCameraScroll;
        }

        public void Update(GameTime gameTime)
        {
            Camera.Instance.position += offSet;

            Camera.Instance.offSet.Y = MathHelper.Lerp(Camera.Instance.offSet.Y, Camera.Instance.offSet.Y*1.2f, 0.1f);

            if (Camera.Instance.OffSet.Y > 2.5)
            {
                offSet.Y = 2.5f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                cameraLock = true;
            }
            if (cameraLock == true)
            {
                Player.PlayerInstance.CameraLock = false;
                Camera.Instance.offSet.Y = Player.PlayerInstance.Velocity == Vector2.Zero ? Vector2.Zero.Y :
                    Player.PlayerInstance.Translation.Y;
                Player.PlayerInstance.Position -= new Vector2(0, Camera.instance.offSet.Y);
                //Debug.WriteLine(Player.PlayerInstance.Velocity.Y);
                //Debug.WriteLine(Camera.Instance.OffSet.Y);
            }

            if (Camera.instance.Position.Y > 0 && Camera.instance.OffSet.Y > 0) 
            {
                Player.PlayerInstance.Health = 0;
            }
        }
    }
}
