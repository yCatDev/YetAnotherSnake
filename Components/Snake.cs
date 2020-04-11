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
        
        private float _step;
        private VirtualButton _leftArrow, _rightArrow, _space;
        private List<Entity> _snakeParts;
        private int _startSnakeSize;
        
        private Entity _marker;
        private Vector2 _startDirection;
        
        private Texture2D _bodySprite;
        private BoxCollider _snakeHeadCollider;
        private CameraBounds _cameraBounds;
        private ScoreDisplay _score;
        
        
        public bool IsAlive;
        private List<Vector2> _deathVectors;
        
        
        public Entity SnakeHead;
        
        public Snake(int startSnakeSize, Vector2 startPosition, Vector2 startDirection, float step = 0.05f)
        {

            _startDirection = startDirection;
            _step = step;
            
            _leftArrow = new VirtualButton();
            _leftArrow.AddKeyboardKey(Keys.Left);
            
            _rightArrow = new VirtualButton();
            _rightArrow.AddKeyboardKey(Keys.Right);
            _startSnakeSize = startSnakeSize;
            _snakeParts = new List<Entity>(1000);
            
            _space = new VirtualButton();
            _space.AddKeyboardKey(Keys.Space);
            
            IsAlive = true;


        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            Initialize();
        }

        private void Initialize()
        {
            
            
            _bodySprite = Scene.Content.Load<Texture2D>(Content.SnakeBody);
            


            for (int i = 0; i < _startSnakeSize; i++)
            {
                var e =Scene.CreateEntity($"Snake{_snakeParts.Count}", 
                    new Vector2(0, 0+(_bodySprite.Height/4*i)));
                e.AddComponent(new SpriteRenderer(_bodySprite));
                if (i > 10)
                    e.AddComponent<BoxCollider>();
                _snakeParts.Add(e);
            }

            SnakeHead = _snakeParts[0];
            _snakeHeadCollider = SnakeHead.AddComponent<BoxCollider>();
            _snakeHeadCollider.Width -= 2;
            _snakeHeadCollider.Height -= 2;
            
            SnakeHead.AddComponent(new GridModifier()).AddComponent<CameraShake>();
            _marker = Scene.CreateEntity("marker",  SnakeHead.Position + _startDirection);
            _marker.Parent = SnakeHead.Transform;
            
            Scene.Camera.Entity.AddComponent(new FollowCamera(SnakeHead));
            _cameraBounds = Scene.Camera.Entity.GetComponent<CameraBounds>();
            _score = Scene.FindEntity("scoreText").GetComponent<ScoreDisplay>();
        }

        public override void Update()
        {
            
            
            if (IsAlive)
            {
                if (MyGame.GameInstance.Pause)
                    return;
                SnakeHead.Position = Utils.Move(SnakeHead.Position, _marker.Position, _step * 10);
                if (_leftArrow.IsDown)
                    _marker.Position = Utils.RotateAboutOrigin(_marker.Position, _marker.Parent.Position, -0.1f);
                if (_rightArrow.IsDown)
                    _marker.Position = Utils.RotateAboutOrigin(_marker.Position, _marker.Parent.Position, 0.1f);
                SnakeHead.Position = Utils.Move(SnakeHead.Position, _marker.Position, _step);
                
                for (int i = _snakeParts.Count - 1; i > 0; i--)
                {

                    _snakeParts[i].Position =
                        Utils.Move(_snakeParts[i].Position, _snakeParts[i - 1].Position, _step * 10);
                    //_snakeParts[i].LocalRotationDegrees =
                    //    Utils.LookAt(_snakeParts[i].Transform, _snakeParts[i - 1].Transform);
                   

                    if (!_snakeParts[i].HasComponent<SpriteRenderer>())
                        _snakeParts[i].AddComponent(new SpriteRenderer(_bodySprite));
                    var render = _snakeParts[i].GetComponent<SpriteRenderer>();
                    render.RenderLayer = i;
                    render.Color = new Color(51 + i, 30, 213 - i);
                }
                
                if (_snakeHeadCollider.CollidesWithAny(out CollisionResult result))
                {
                    var c = result.Collider.Entity;

                    if (c.Name.Contains("Snake"))
                    {
                        SnakeHead.GetComponent<GridModifier>().Impulse( 500);
                        Die();
                    }

                    if (c.HasComponent<SnakeFood>())
                    {
                        SnakeHead.GetComponent<GridModifier>().Impulse( 200);
                        IncSnake(5);
                        _score.IncScore();
                        MyGame.GameInstance.AudioManager.PickUpSound.Play();
                        c.Destroy();
                    }
                }

                if (_cameraBounds.OutOfBounds(SnakeHead.Position)) 
                 Die();
            }
            else
            {
                for (var index = 1; index < _snakeParts.Count; index++)
                {
                    var part = _snakeParts[index];
                    part.Position = Utils.Move(part.Position, _deathVectors[index], _step/75);
                }
            }

            
        }
        
        

        public void Die()
        {
            
            IsAlive = false;
            _score.CheckHiScore();
            _deathVectors = new List<Vector2>(_snakeParts.Count);
            for (int i = 0; i < _snakeParts.Count; i++)
            {
                Vector2 dir = Vector2.Zero;
                while (dir == Vector2.Zero)
                {
                    dir = new Vector2(Random.Range(-1, 2),Random.Range(-1, 2))*Random.Range(20000, 35000);
                    
                }

                
                
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
        
        
        private void _addPart()
        {
            int last = _snakeParts.Count;
            var e = Scene.CreateEntity($"Snake{last}", 
                new Vector2(_snakeParts[last-1].Position.X, _snakeParts[last-1].Position.Y));
            
            
            e.AddComponent<BoxCollider>();
            _snakeParts.Add(e);
        }
        private IEnumerator ReturnToMenu()
        {
            yield return Coroutine.WaitForSeconds(2);
            
            Core.StartSceneTransition(new FadeTransition(() => new MenuScene()));
            Scene.RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
            Scene.RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);
            yield return null;
        }
        public void IncSnake(int addSize)
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