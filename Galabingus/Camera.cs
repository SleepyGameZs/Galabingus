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

        // -------------------------------------------------
        // Properties
        // -------------------------------------------------

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
        }

        public Camera(int cameraScroll)
        {
            x = 0;
            y = 0;
            this.initalCameraScroll = cameraScroll;
            offSet = new Vector2(0, -initalCameraScroll);
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

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                cameraLock = true;
            }
            if (cameraLock == true)
            {
                Camera.Instance.offSet.Y = Player.PlayerInstance.Velocity == Vector2.Zero ? Vector2.Zero.Y*5 :
                    Vector2.Normalize(Player.PlayerInstance.Velocity).Y*5;
            }
        }
    }
}
