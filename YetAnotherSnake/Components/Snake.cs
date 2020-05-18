using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Entity Marker;
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

        private bool _isTimeDepended;

        private readonly Stopwatch _hungerTimer;
        private float _maxTimeWithoutFood;
        
        public readonly bool IsReally;
        public bool MoveLeft;
        public bool MoveRight;
        private Vector2 _startSnakePosition;

        /// <summary>
        /// Create new snake
        /// </summary>
        /// <param name="startSnakeSize">Length</param>
        /// <param name="startPosition">Start position</param>
        /// <param name="startDirection">Start direction</param>
        /// <param name="step">Move step</param>
        public Snake(bool real, int startSnakeSize, Vector2 startPosition, Vector2 startDirection, float step = 0.075f, bool isTimeDepended = false)
        {
            _startDirection = startDirection;
            _step = step;
            
            IsReally = real;

            if (IsReally)
                Console.WriteLine($"[CLIENT] Create snake is controllable for client ({MyGame.GameInstance.GameClient.Id})");
            
            _startSnakeSize = startSnakeSize;
            _startSnakePosition = startPosition;
            _snakeParts = new List<Entity>(1000);

            if (IsReally)
            {
                _leftArrow = new VirtualButton();
                _leftArrow.AddKeyboardKey(Keys.Left);
                _leftArrow.AddKeyboardKey(Keys.A);

                _rightArrow = new VirtualButton();
                _rightArrow.AddKeyboardKey(Keys.Right);
                _rightArrow.AddKeyboardKey(Keys.D);
            }

            _isTimeDepended = isTimeDepended;
            if (_isTimeDepended)
                _hungerTimer = new Stopwatch();
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
                    new Vector2(0, 0));
                e.AddComponent(new SpriteRenderer(_bodySprite));   
                //Skip first 10 bodies to prevent false collisions
                if (i > 10)
                    e.AddComponent<BoxCollider>();
                _snakeParts.Add(e);
            }

            //Separately setup snake head
            SnakeHead = _snakeParts[0];
            var render =  SnakeHead.GetComponent<SpriteRenderer>();
            render.RenderLayer = 0;
            //render.Color = new Color(18 , 64, 171);
            
            _snakeHeadCollider = SnakeHead.AddComponent<BoxCollider>();
            _snakeHeadCollider.Width -= 2;
            _snakeHeadCollider.Height -= 2;
            
            
            SnakeHead.AddComponent(new GridModifier()).AddComponent<CameraShake>();
            Marker = Scene.CreateEntity("marker",  SnakeHead.Position + _startDirection);
            Marker.Parent = SnakeHead.Transform;

            _cameraBounds = Scene.Camera.Entity.GetComponent<CameraBounds>();
            
            if (IsReally)
            {
                Scene.Camera.Entity.AddComponent(new FollowCamera(SnakeHead));
                _score = Scene.FindEntity("scoreText").GetComponent<ScoreDisplay>();
            }

            if (_isTimeDepended)
            {
                _hungerTimer.Start();
                _maxTimeWithoutFood = ((_startSnakeSize)/2f)*1000;
            }

            Eyes = Scene.AddSceneComponent(new SnakeEyes(SnakeHead, Marker, _bodySprite));

        }

        public SnakeEyes Eyes { get; private set; }

        public override void Update()
        {
            if (IsAlive)
            {
                if (MyGame.GameInstance.Pause)
                {
                    if (_isTimeDepended) _hungerTimer.Stop();
                    return;
                }
                if (_isTimeDepended && !_hungerTimer.IsRunning) _hungerTimer.Start();
                if (IsReally)
                {
                    if (_leftArrow.IsDown)
                        Marker.Position = Utils.RotateAboutOrigin(Marker.Position, Marker.Parent.Position, -0.1f);
                    if (_rightArrow.IsDown)
                        Marker.Position = Utils.RotateAboutOrigin(Marker.Position, Marker.Parent.Position, 0.1f);
                    
                    //Moving snake head
                    SnakeHead.Position = Utils.Move(SnakeHead.Position, Marker.Position, _step * 10);
                    SnakeHead.Position = Utils.Move(SnakeHead.Position, Marker.Position, _step);
                    
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
                    render.RenderLayer = i+3;
                    var newColor = new Color(51 + i, 30, 213 - i);
                    if (_isTimeDepended)
                    {
                        newColor*=((_startSnakeSize)/2f+1 -
                         ((_maxTimeWithoutFood - _hungerTimer.ElapsedMilliseconds) / 1000));
                        newColor.A = 255;
                    }

                    render.Color = newColor;


                }
                
                //Checking for collisions
                if (_snakeHeadCollider.CollidesWithAny(out CollisionResult result))
                {
                    var c = result.Collider.Entity;
                    
                    //"Gameover" if touch self or Stone
                    if (c.Name.Contains("Snake") || c.Name.Contains("Stone"))
                    {
                        SnakeHead.GetComponent<GridModifier>().Impulse( 500);
                        c.Destroy();
                        Die();
                    }
                    
                    //Increment snake length if touch food
                    if (c.HasComponent<SnakeFood>())
                    {
                        SnakeHead.GetComponent<GridModifier>().Impulse( 200);
                        IncSnake(5);
                        if (IsReally)
                        {
                            _score.IncScore();
                            MyGame.GameInstance.AudioManager.PickUpSound.Play();
                        }

                        MyGame.GameInstance.GameClient.SpawnFood();
                        if (_isTimeDepended)
                        {
                            _hungerTimer.Restart();
                            //_maxTimeWithoutFood = (_snakeParts.Count/2f)*1000*0.5f;
                        }

                        c.Destroy();
                    }
                }

                //If snake head is out of camera view - "Gameover"
                if (_cameraBounds.OutOfBounds(SnakeHead.Position) || (_isTimeDepended && _hungerTimer.ElapsedMilliseconds>_maxTimeWithoutFood)) 
                 Die();
                
                MyGame.GameInstance.GameClient.SendSnakePosition(SnakeHead.Position);
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
            //return;
            IsAlive = false;
            Eyes.Remove();
            //Create death-vectors
            _deathVectors = new List<Vector2>(_snakeParts.Count);
            for (int i = 0; i < _snakeParts.Count; i++)
            {
                Vector2 dir = Vector2.Zero;
                while (dir == Vector2.Zero)
                {
                    dir = new Vector2(Random.Range(-1f, 2f), Random.Range(-1f, 2f));
                    dir.Normalize();
                    dir*=Random.Range(20000, 35000);
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

            if (IsReally)
            {
                MyGame.GameInstance.AudioManager.DeathSound.Play();
                SnakeHead.AddComponent<CameraShake>().Shake(75, 1.5f);

                MyGame.GameInstance.AudioManager.StopMusic();
                _score.CheckHiScore();
                Core.StartCoroutine(ReturnToMenu());
            }
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

            Core.StartSceneTransition(new FadeTransition(() =>
            {
                MyGame.GameInstance.GameClient.Disconnect();
                return new MenuScene();
            }));

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


        public void SetMarkerPosition(Vector2 position, float delta)
        {
            if (IsReally) return;
            var speed = Math.Max(delta, Time.DeltaTime);
            SnakeHead.Position = Utils.Move(SnakeHead.Position, position, 1);
        }
    }
}