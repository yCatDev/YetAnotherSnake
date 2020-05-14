using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Audio;
using Nez.Sprites;
using YetAnotherSnake.Scenes;
using Random = Nez.Random;

namespace YetAnotherSnake.Components
{
    public class Snake: SceneComponent, IUpdatable
    {

        /// <summary>
        /// Snake move step
        /// </summary>
        private float _step;
        
        /// <summary>
        /// Keys for input
        /// </summary>
        private VirtualButton _leftArrow, _rightArrow;
        
        /// <summary>
        /// Snake body list
        /// </summary>
        private List<Entity> _snakeParts;
        
        /// <summary>
        /// Initial snake length
        /// </summary>
        private int _startSnakeSize;
        
        /// <summary>
        /// MoveMarker that helps to move
        /// </summary>
        private Entity _marker;
        /// <summary>
        /// Direction in which snake is moving
        /// </summary>
        private Vector2 _startDirection;
        
        /// <summary>
        /// Snake body texture
        /// </summary>
        private Texture2D _bodySprite;
        
        /// <summary>
        /// Snake head collider
        /// </summary>
        private BoxCollider _snakeHeadCollider;
        
        /// <summary>
        /// CameraBounds component for snake positioning
        /// </summary>
        private CameraBounds _cameraBounds;
        
        /// <summary>
        /// SnakeDisplay for displaying score
        /// </summary>
        private ScoreDisplay _score;
        
        /// <summary>
        /// Is out snake alive?
        /// </summary>
        public bool IsAlive;

        /// <summary>
        /// Random direction vectors of snake body parts
        /// </summary>
        private List<Vector2> _deathVectors;
        
        /// <summary>
        /// Make snake entity
        /// </summary>
        public Entity SnakeHead;


        private bool _isReally;
        public bool MoveLeft;
        public bool MoveRight;

        /// <summary>
        /// Create new snake
        /// </summary>
        /// <param name="startSnakeSize">Length</param>
        /// <param name="startPosition">Start position</param>
        /// <param name="startDirection">Start direction</param>
        /// <param name="step">Move step</param>
        public Snake(bool real, int startSnakeSize, Vector2 startPosition, Vector2 startDirection, float step = 0.05f)
        {
            _startDirection = startDirection;
            _step = step;
            
            _isReally = real;
            
            _startSnakeSize = startSnakeSize;
            _snakeParts = new List<Entity>(1000);
            
            _leftArrow = new VirtualButton();
            _leftArrow.AddKeyboardKey(Keys.Left);
            _leftArrow.AddKeyboardKey(Keys.A);

            _rightArrow = new VirtualButton();
            _rightArrow.AddKeyboardKey(Keys.Right);
            _rightArrow.AddKeyboardKey(Keys.D);
            
            IsAlive = true;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            Initialize();
        }

        private void Initialize()
        {
            //Load snake body texture
            _bodySprite = Scene.Content.Load<Texture2D>(Content.SnakeBody);
            
            //Сreating snake bodies
            for (int i = 0; i < _startSnakeSize; i++)
            {
                var e =Scene.CreateEntity($"Snake{_snakeParts.Count}", 
                    new Vector2(0, 0+(_bodySprite.Height/4*i)));
                e.AddComponent(new SpriteRenderer(_bodySprite));   
                //Skip first 10 bodies to prevent false collisions
                if (i > 10)
                    e.AddComponent<BoxCollider>();
                _snakeParts.Add(e);
            }

            //Separately setup snake head
            SnakeHead = _snakeParts[0];
            _snakeHeadCollider = SnakeHead.AddComponent<BoxCollider>();
            _snakeHeadCollider.Width -= 2;
            _snakeHeadCollider.Height -= 2;
            
            
            SnakeHead.AddComponent(new GridModifier()).AddComponent<CameraShake>();
            _marker = Scene.CreateEntity("marker",  SnakeHead.Position + _startDirection);
            _marker.Parent = SnakeHead.Transform;

            _cameraBounds = Scene.Camera.Entity.GetComponent<CameraBounds>();
            if (_isReally)
            {
                Scene.Camera.Entity.AddComponent(new FollowCamera(SnakeHead));
                
                _score = Scene.FindEntity("scoreText").GetComponent<ScoreDisplay>();
            }
        }

        public override void Update()
        {
            if (IsAlive)
            {
                if (MyGame.GameInstance.Pause)
                    return;

                if (_isReally)
                {
                    if (_leftArrow.IsDown)
                        _marker.Position = Utils.RotateAboutOrigin(_marker.Position, _marker.Parent.Position, -0.1f);
                    if (_rightArrow.IsDown)
                        _marker.Position = Utils.RotateAboutOrigin(_marker.Position, _marker.Parent.Position, 0.1f);

                    //Moving snake head

                    SnakeHead.Position = Utils.Move(SnakeHead.Position, _marker.Position, _step * 10);
                    SnakeHead.Position = Utils.Move(SnakeHead.Position, _marker.Position, _step);
                }

                for (int i = _snakeParts.Count - 1; i > 0; i--)
                {
                    
                    //Move all boies
                    _snakeParts[i].Position =
                        Utils.Move(_snakeParts[i].Position, _snakeParts[i - 1].Position, _step * 10);
                    //_snakeParts[i].LocalRotationDegrees =
                    //    Utils.LookAt(_snakeParts[i].Transform, _snakeParts[i - 1].Transform);
                   
                    
                    //Set up gradient color for whole snake
                    if (!_snakeParts[i].HasComponent<SpriteRenderer>())
                        _snakeParts[i].AddComponent(new SpriteRenderer(_bodySprite));
                    var render = _snakeParts[i].GetComponent<SpriteRenderer>();
                    render.RenderLayer = i;
                    render.Color = new Color(51 + i, 30, 213 - i);
                }
                
                //Checking for collisions
                if (_snakeHeadCollider.CollidesWithAny(out CollisionResult result))
                {
                    var c = result.Collider.Entity;
                    
                    //"Gameover" if touch self
                    if (c.Name.Contains("Snake"))
                    {
                        SnakeHead.GetComponent<GridModifier>().Impulse( 500);
                        Die();
                    }

                    //Increment snake length if touch food
                    if (c.HasComponent<SnakeFood>())
                    {
                        SnakeHead.GetComponent<GridModifier>().Impulse( 200);
                        IncSnake(5);
                        //_score.IncScore();
                        MyGame.GameInstance.AudioManager.PickUpSound.Play();
                        MyGame.GameInstance.GameClient.SpawnFood();
                        c.Destroy();
                    }
                }

                //If snake head is out of camera view - "Gameover"
                if (_cameraBounds.OutOfBounds(SnakeHead.Position)) 
                 Die();
            }
            else
            {
                //If we died move snake bodies in death vectors direction
                for (var index = 1; index < _snakeParts.Count; index++)
                {
                    var part = _snakeParts[index];
                    part.Position = Utils.Move(part.Position, _deathVectors[index], _step/75);
                }
            }

            
        }
        
        
        /// <summary>
        /// Snake die... Gameover... GG, WP
        /// </summary>
        public void Die()
        {
            return;
            IsAlive = false;
            _score.CheckHiScore();
            MyGame.GameInstance.GameClient.Disconnect();
            //Create death-vectors
            _deathVectors = new List<Vector2>(_snakeParts.Count);
            for (int i = 0; i < _snakeParts.Count; i++)
            {
                Vector2 dir = Vector2.Zero;
                while (dir == Vector2.Zero)
                {
                    dir = new Vector2(Random.Range(-1, 2),Random.Range(-1, 2))*Random.Range(20000, 35000);
                    
                }

                
                
                //Adding "trail" particle to bodies
                var render = _snakeParts[i].GetComponent<SpriteRenderer>();
                if (i > 0)
                {
                    var trail = _snakeParts[i]
                        .AddComponent(new SpriteTrail(render));
                    trail.FadeDelay = 0;
                    trail.FadeDuration = 0.2f;
                    trail.MinDistanceBetweenInstances = 10f;
                    trail.InitialColor = render.Color * 0.5f;
                }

                SnakeHead.RemoveComponent<SpriteRenderer>();
                _deathVectors.Add(dir);
            }
            MyGame.GameInstance.AudioManager.DeathSound.Play();
            SnakeHead.AddComponent<CameraShake>().Shake(75, 1.5f);

            MyGame.GameInstance.AudioManager.StopMusic();
                      
            Core.StartCoroutine(ReturnToMenu());
        }
        
        
        /// <summary>
        /// Adding new bodies
        /// </summary>
        private void _addPart()
        {
            int last = _snakeParts.Count;
            var e = Scene.CreateEntity($"Snake{last}", 
                new Vector2(_snakeParts[last-1].Position.X, _snakeParts[last-1].Position.Y));
            
            
            e.AddComponent<BoxCollider>();
            _snakeParts.Add(e);
        }
        
        /// <summary>
        /// Load menu scene
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator ReturnToMenu()
        {
            yield return Coroutine.WaitForSeconds(2);
            
            Core.StartSceneTransition(new FadeTransition(() => new MenuScene()));
            Scene.RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
            Scene.RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);
            yield return null;
        }
        
        /// <summary>
        /// Increment snake length after eating food
        /// </summary>
        /// <param name="addSize"></param>
        private void IncSnake(int addSize)
        {
            for (int i = 0; i < addSize; i++)
            {
                _addPart();
            }

            if (_step<0.08f)
                _step += 0.0002f;
        }


      
    }
}