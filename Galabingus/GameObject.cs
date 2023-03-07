using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Matthew Rodriguez
// 2023, 3, 6
// GameObject
// Provides essentails for all game objects
//
// Setup:
// Create a dynamic for the initalization process
// Set the dynamic to the result of the Initlizer method Initialize()
// Accessing GameObjects:
// Set the instance like so: GameObject.Instance.Content = GameObject.Instance.Content.YOURCONTENTNAME
// Note: YOURCONTENTNAME must follow file name specifications ie _strip(i) where (i) is the number of sprites
//
// Inharatence:
// There is no default constructor so use one of the parameter constructors,
// you MUST re-implement every used property using the content setter like so:
// GameObject.Instance.Content = GameObject.Instance.Content.YOURCONTENTNAME
// then use the property as normal
//
// GameObject.Instance - All of the instances of GameObject
// GameObject.Instance.Index - The current GameObject
// GameObject.Instance.Content - Dynamic required for creating content on the fly
// GameObject.Instance.GraphicsDevice - a GraphicsDevice
// GameObject.Instance.SpriteBatch - a SpriteBatch
// GameObject.Instance[Animation.Empty] - animation array of the current Instance
// GameObject.Instance[Collider.Empty] - collider array of the current Instance
// GameObject.Instance[Vector2.Zero] - position array of the current Instance
// GameObject.Instance[Rectangle.Empty] - transform array of the current Instance
// GameObject.Instance[i, Rectangle.Empty] - transform at i of the current Instance
// GameObject.Instance[i, Vector2.Zero] - position at i of the current Instance
// GameObject.Instance[i, Animation.Empty] - animation at i of the current Instance
// GameObject.Instance[i, Collider.Empty] - collider at i of the current Instance
// GameObject.Sprite - sprite of the current Instance
// GameObject.Scale - scale of the sprite of the current Instance
// GameObject.Colliders - All instances collider arrays <!> Warning <!>

namespace Galabingus
{
    /// <summary>
    ///  GameObject contains
    ///  the common funtionalites 
    ///  all game objects have
    /// </summary>
    internal class GameObject : DynamicObject
    {
        private static GameObject allGameObjects = null; // GameObject singleton: contains all instances for all GameObjects
        private List<List<Animation>> animations;        // Animation content
        private List<List<Collider>> colliders;          // Collider content
        private List<List<Rectangle>> transforms;        // Transform content
        private List<List<Vector2>> positions;           // Position content
        private List<Texture2D> sprites;                 // Sprite content
        private List<float> scales;                      // Scale content
        private List<string> objectEnums;                // Actual content names
        private ushort index;                            // The current content index in all of the content arrays
        private ContentManager contentManager;           // Used to load in the sprite
        private GraphicsDevice graphicsDevice;           // Graphics Device
        private SpriteBatch spriteBatch;                 // Sprite Batch

        /// <summary>
        ///  GameObject master storage place
        /// </summary>
        public static GameObject Instance
        {
            // Singleton for GameObject
            get
            {
                if (allGameObjects == null)
                {
                    allGameObjects = new GameObject();
                }
                return allGameObjects;
            }
        }

        /// <summary>
        ///  Current index relation to all of the content arrays
        /// </summary>
        public ushort Index
        {
            // ONLY allow for receiving the index here
            get
            {
                return GameObject.Instance.index;
            }
        }

        /// <summary>
        ///  Gets the GameObject Instance for the index
        ///  Sets the index for the Instance
        /// </summary>
        public dynamic Content
        {
            get
            {
                return this;
            }
            set
            {
                GameObject.Instance.index = value;
            }
        }

        /// <summary>
        ///  Graphics Device
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            // Only allow to recive the GraphicsDevice
            get
            {
                return GameObject.Instance.graphicsDevice;
            }
        }

        /// <summary>
        ///  Sprite Batch
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            // Only alow to set the SpriteBatch
            get
            {
                return GameObject.Instance.spriteBatch;
            }
        }

        /// <summary>
        ///  Get a specific frame of the transform
        ///  Sets a specific frame of the transform
        /// </summary>
        /// <param name="frame">index to the transformt</param>
        /// <param name="rectangle">type determinate</param>
        /// <returns>Rectangle at the given frame</returns>
        public Rectangle this[ushort frame, Rectangle rectangle]
        {
            get
            {
                // When the index does not exist expand transforms
                if (GameObject.Instance.transforms.Count <= index)
                {
                    for (int i = GameObject.Instance.transforms.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.transforms.Add(new List<Rectangle>());
                    }
                }

                // When the frame does not exist expand the array inside transforms
                if (GameObject.Instance.transforms[index].Count <= frame)
                {
                    for (int i = GameObject.Instance.transforms[index].Count; i <= frame + 1; i++)
                    {
                        GameObject.Instance.transforms[index].Add(new Rectangle());
                    }
                }

                // return the frame
                return GameObject.Instance.transforms[index][frame];
            }
            set
            {
                // When the index does not exist expand transforms
                if (GameObject.Instance.transforms.Count <= index)
                {
                    for (int i = GameObject.Instance.transforms.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.transforms.Add(new List<Rectangle>());
                    }
                }

                // When the frame does not exist expand the array inside transforms
                if (GameObject.Instance.transforms[index].Count <= frame)
                {
                    for (int i = GameObject.Instance.transforms[index].Count; i <= frame + 1; i++)
                    {
                        GameObject.Instance.transforms[index].Add(new Rectangle());
                    }
                }

                // Set the frame
                GameObject.Instance.transforms[index][frame] = value;
            }
        }

        /// <summary>
        ///  Gets specific instance of a position
        ///  Sets the specific instance of a position
        /// </summary>
        /// <param name="instance">index of the position</param>
        /// <param name="position">type determinate</param>
        /// <returns>Position</returns>
        public Vector2 this[ushort instance, Vector2 position]
        {
            get
            {
                // When the index does not exist expand positions
                if (GameObject.Instance.positions.Count <= index)
                {
                    for (int i = GameObject.Instance.positions.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.positions.Add(new List<Vector2>());
                    }
                }

                // When the instance does not exist expand the array inside positions
                if (GameObject.Instance.positions[index].Count <= instance)
                {
                    for (int i = GameObject.Instance.positions[index].Count; i <= instance + 1; i++)
                    {
                        GameObject.Instance.positions[index].Add(new Vector2());
                    }
                }

                // Get the instance of the position
                return GameObject.Instance.positions[index][instance];
            }
            set
            {
                // When the index does not exist expand positions
                if (GameObject.Instance.positions.Count <= index)
                {
                    for (int i = GameObject.Instance.positions.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.positions.Add(new List<Vector2>());
                    }
                }

                // When the instance does not exist expand the array inside positions
                if (GameObject.Instance.positions[index].Count <= instance)
                {
                    for (int i = GameObject.Instance.positions[index].Count; i <= instance + 1; i++)
                    {
                        GameObject.Instance.positions[index].Add(new Vector2());
                    }
                }

                // Set the specific instance for the position
                GameObject.Instance.positions[index][instance] = value;
            }
        }

        /// <summary>
        ///  Gets the specific instance of a animation
        ///  Sets the specific instance of the animation
        /// </summary>
        /// <param name="instance">index of animation</param>
        /// <param name="animation">type discriptor</param>
        /// <returns></returns>
        public Animation this[ushort instance, Animation animation]
        {
            get
            {
                // When the index does not exist expand animations
                if (GameObject.Instance.animations.Count <= index)
                {
                    for (int i = GameObject.Instance.animations.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.animations.Add(new List<Animation>());
                    }
                }

                // When the instance does not exist expand the array in animations
                if (GameObject.Instance.animations[index].Count <= instance)
                {
                    for (int i = GameObject.Instance.animations[index].Count; i <= instance + 1; i++)
                    {
                        GameObject.Instance.animations[index].Add(new Animation(0, 0, 0));
                    }
                }

                // Get the instance of a animation
                return GameObject.Instance.animations[index][instance];
            }
            set
            {
                // When the index does not exist expand animations
                if (GameObject.Instance.animations.Count <= index)
                {
                    for (int i = GameObject.Instance.animations.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.animations.Add(new List<Animation>());
                    }
                }

                // When the instance does not exist expand the array in animations
                if (GameObject.Instance.animations[index].Count <= instance)
                {
                    for (int i = GameObject.Instance.animations[index].Count; i <= instance + 1; i++)
                    {
                        GameObject.Instance.animations[index].Add(new Animation(0, 0, 0));
                    }
                }

                // Set the specific instance for animation
                GameObject.Instance.animations[index][instance] = value;
            }
        }

        /// <summary>
        ///  Gets the specific instance of a collider
        ///  Sets the specific instance of a collider
        /// </summary>
        /// <param name="instance">index of collider</param>
        /// <param name="collider">type discriptor</param>
        /// <returns></returns>
        public Collider this[ushort instance, Collider collider]
        {
            get
            {
                // When the index does not exist expand colliders
                if (GameObject.Instance.colliders.Count <= index)
                {
                    for (int i = GameObject.Instance.colliders.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.colliders.Add(new List<Collider>());
                    }
                }

                // When the instance does not exist expand the array in colliders
                if (GameObject.Instance.colliders[index].Count <= instance)
                {
                    for (int i = GameObject.Instance.colliders[index].Count; i <= instance + 1; i++)
                    {
                        GameObject.Instance.colliders[index].Add(new Collider());
                    }
                }

                // Get the specific instance of the collider
                return GameObject.Instance.colliders[index][instance];
            }
            set
            {
                // When the index does not exist expand colliders
                if (GameObject.Instance.colliders.Count <= index)
                {
                    for (int i = GameObject.Instance.colliders.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.colliders.Add(new List<Collider>());
                    }
                }

                // When the instance does not exist expand the array in colliders
                if (GameObject.Instance.colliders[index].Count <= instance)
                {
                    for (int i = GameObject.Instance.colliders[index].Count; i <= instance + 1; i++)
                    {
                        GameObject.Instance.colliders[index].Add(new Collider());
                    }
                }

                // Set the specific instance for the collider
                GameObject.Instance.colliders[index][instance] = value;
            }
        }

        /// <summary>
        ///  Gets the array of animations
        ///  Sets the array of animations
        /// </summary>
        /// <param name="type">type discriptor</param>
        /// <returns>array of animation</returns>
        public List<Animation> this[Animation type]
        {
            get
            {
                // When the index does not exist expand animations
                if (GameObject.Instance.animations.Count <= index)
                {
                    for (int i = GameObject.Instance.animations.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.animations.Add(new List<Animation>());
                    }
                }

                // Get the animations array
                return GameObject.Instance.animations[index];
            }
            set
            {
                // When the index does not exist expand animations
                if (index >= GameObject.Instance.animations.Count)
                {
                    for (int i = GameObject.Instance.animations.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.animations.Add(value);
                    }
                }
                else
                {
                    // Set the value of animations
                    GameObject.Instance.animations[index] = value;
                }
            }
        }

        /// <summary>
        ///  Gets the collider array
        ///  Sets the collider array
        /// </summary>
        /// <param name="type">type discriptor</param>
        /// <returns>collider arrray</returns>
        public List<Collider> this[Collider type]
        {
            get
            {
                // When the index does not exist expand colliders
                if (GameObject.Instance.colliders.Count <= index)
                {
                    for (int i = GameObject.Instance.colliders.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.colliders.Add(new List<Collider>());
                    }
                }

                // Get the colliders array
                return GameObject.Instance.colliders[index];
            }
            set
            {
                // When the index does not exist expand colliders
                if (index >= GameObject.Instance.colliders.Count)
                {
                    for (int i = GameObject.Instance.colliders.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.colliders.Add(value);
                    }
                }
                else
                {
                    // Set the colliders array
                    GameObject.Instance.colliders[index] = value;
                }
            }
        }

        /// <summary>
        ///  Gets the positions array
        ///  Sets the positions array
        /// </summary>
        /// <param name="type">type discriptor</param>
        /// <returns>positions array</returns>
        public List<Vector2> this[Vector2 type]
        {
            get
            {
                // When the index does not exist expand positions
                if (GameObject.Instance.positions.Count <= index)
                {
                    for (int i = GameObject.Instance.positions.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.positions.Add(new List<Vector2>());
                    }
                }

                // Get the positions array
                return GameObject.Instance.positions[index];
            }
            set
            {
                // When the index does not exist expand positions
                if (index >= GameObject.Instance.positions.Count)
                {
                    for (int i = GameObject.Instance.positions.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.positions.Add(value);
                    }
                }
                else
                {
                    // Set the positions array
                    GameObject.Instance.positions[index] = value;
                }
            }
        }

        /// <summary>
        ///  Gets the transforms array
        ///  Sets the transforms array
        /// </summary>
        /// <param name="type">type discriptor</param>
        /// <returns>transforms array</returns>
        public List<Rectangle> this[Rectangle type]
        {
            get
            {
                // When the index does not exist expand transforms
                if (GameObject.Instance.transforms.Count <= index)
                {
                    for (int i = GameObject.Instance.transforms.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.transforms.Add(new List<Rectangle>());
                    }
                }

                // Get the transforms array
                return GameObject.Instance.transforms[index];
            }
            set
            {
                // When the index does not exist expand transforms
                if (index >= GameObject.Instance.transforms.Count)
                {
                    for (int i = GameObject.Instance.transforms.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.transforms.Add(value);
                    }
                }
                else
                {
                    // Sets the transforms array
                    GameObject.Instance.transforms[index] = value;
                }
            }
        }

        /// <summary>
        ///  Gets the Instances Colliders 
        ///  Sets the Instances Colliders
        ///  <!>Warning this effects all instances<!>
        /// </summary>
        public List<List<Collider>> Colliders
        {
            // Give direct access
            get
            {
                return colliders;
            }
            set
            {
                colliders = value;
            }
        }

        /// <summary>
        ///  Gets the sprite
        ///  Sets the sprite
        /// </summary>
        public Texture2D Sprite
        {
            get
            {
                // Gets the sprite
                return GameObject.Instance.sprites[index];
            }
            set
            {
                // When the index does not exist expand the sprite array
                if (index >= GameObject.Instance.sprites.Count)
                {
                    for (int i = GameObject.Instance.sprites.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.sprites.Add(value);
                    }
                }
                else
                {
                    // Set the sprite
                    GameObject.Instance.sprites[index] = value;
                }
            }
        }

        /// <summary>
        ///  Gets the scale
        ///  Sets the scale
        /// </summary>
        public float Scale
        {
            get
            {
                // Gets the scale
                return GameObject.Instance.scales[index];
            }
            set
            {
                // When the index does not exist expand the scale array
                if (index >= GameObject.Instance.scales.Count)
                {
                    for (int i = GameObject.Instance.scales.Count; i <= index + 1; i++)
                    {
                        GameObject.Instance.scales.Add(value);
                    }
                }
                else
                {
                    // Set the scale
                    GameObject.Instance.scales[index] = value;
                }
            }
        }

        /// <summary>
        ///  Creates a empty GameObject for the Instance
        /// </summary>
        private GameObject()
        {
            animations = new List<List<Animation>>();
            colliders = new List<List<Collider>>();
            transforms = new List<List<Rectangle>>();
            sprites = new List<Texture2D>();
            positions = new List<List<Vector2>>();
            scales = new List<float>();
            objectEnums = new List<string>();
        }

        /// <summary>
        ///  Generates a index for the instance Content 
        ///  property that cannot be found
        ///  The binder name will be stored at the index
        ///  in objectEnums that matches the index generated
        /// </summary>
        /// <param name="binder">Property binder</param>
        /// <param name="result">The generated index</param>
        /// <returns>GameObject Index</returns>
        public override bool TryGetMember(
            GetMemberBinder binder,
            out object result
        )
        {
            bool exist = false; // If the property exist
            ushort index = 0;   // The index of the property

            // Search to find the property
            foreach (string sprite in GameObject.Instance.objectEnums)
            {
                if (binder.Name == sprite)
                {
                    exist = true;
                }
                if (exist)
                {
                    this.index = index;
                    result = index;
                    return exist;
                }
                index++;
            }

            // When teh property does not exist create it
            if (!exist)
            {
                exist = true;
                GameObject.Instance.objectEnums.Add(binder.Name);
            }
            this.index = index;
            result = index;
            return exist;
        }

        /// <summary>
        ///  Creates a GameObject from the content name
        ///  the content name must be in the format file_strip(i) where 
        ///  (i) is the number of sprites in the sprite sheet
        /// </summary>
        /// <param name="contentName">content name</param>
        public GameObject(
            ushort contentName
        )
        {
            GameObject.Instance.Content = contentName;
            string path = GameObject.Instance.objectEnums[contentName];
            ushort strip = ushort.Parse(path.Split("strip")[1]);
            GameObject.Instance.index = contentName;
            GameObject.Instance.Sprite = GameObject.Instance.contentManager.Load<Texture2D>(path);
            GameObject.Instance[Animation.Empty].Add(new Animation(GameObject.Instance.Sprite.Width, GameObject.Instance.Sprite.Height, strip));
            Collider newCollider = new Collider();
            newCollider.Layer = contentName;
            GameObject.Instance[Collider.Empty].Add(newCollider);
            GameObject.Instance[Vector2.Zero].Add(Vector2.Zero);
            for (int frame = 1; frame <= strip; frame++)
            {
                GameObject.Instance[Rectangle.Empty].Add(
                    new Rectangle(
                        (GameObject.Instance.Sprite.Width / strip * frame), // Sprite starts at the frame starting position
                        0,                                                  // Sprite starts at Y = 0
                        GameObject.Instance.Sprite.Width / strip,           // Width of the sprite
                        GameObject.Instance.Sprite.Height                   // Height of the sprite
                   )
                );
            }
        }

        /// <summary>
        ///  Creates a GameObject from the content name
        ///  the content name must be in the format file_strip(i) where 
        ///  (i) is the number of sprites in the sprite sheet
        ///  Define this GameObject as a specific instance via its instanceNumber
        /// </summary>
        /// <param name="contentName">content name</param>
        /// <param name="instanceNumber">index of the instance</param>
        public GameObject(
            ushort contentName,
            ushort instanceNumber
        )
        {
            GameObject.Instance.Content = contentName;
            string path = GameObject.Instance.objectEnums[contentName];
            ushort strip = ushort.Parse(path.Split("strip")[1]);
            GameObject.Instance.index = contentName;
            GameObject.Instance.Sprite = GameObject.Instance.contentManager.Load<Texture2D>(path);
            GameObject.Instance[instanceNumber, Animation.Empty] = new Animation(GameObject.Instance.Sprite.Width, GameObject.Instance.Sprite.Height, strip);
            Collider newCollider = new Collider();
            newCollider.Layer = contentName;
            GameObject.Instance[instanceNumber, Collider.Empty] = newCollider;
            GameObject.Instance[instanceNumber, Vector2.Zero] = Vector2.Zero;
            for (int frame = 1; frame <= strip; frame++)
            {
                GameObject.Instance[Rectangle.Empty].Add(
                    new Rectangle(
                        (GameObject.Instance.Sprite.Width / strip * frame), // Sprite starts at the frame starting position
                        0,                                                  // Sprite starts at Y = 0
                        GameObject.Instance.Sprite.Width / strip,           // Width of the sprite
                        GameObject.Instance.Sprite.Height                   // Height of the sprite
                   )
                );
            }
        }

        /// <summary>
        ///  Initalizes the instance with 
        ///  a contentManager, graphicsDevice,
        ///  spriteBatch
        /// </summary>
        /// <param name="contentManager">Any: ContentManager</param>
        /// <param name="graphicsDevice">Any: GraphicsDevice</param>
        /// <param name="spriteBatch">Any: SpriteBatch</param>
        /// <returns></returns>
        public dynamic Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
            this.contentManager = contentManager;
            return new GameObject();
        }
    }
}