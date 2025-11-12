using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Jeu2D
{
    public class Game1 : Game
    {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch = null!;
        public const int ScreenW = 1280, ScreenH = 720;
    Camera2D camera = null!;
    TileMap map = null!;
    Player player = null!;
        List<Enemy> enemies = new List<Enemy>();
        List<Projectile> projectiles = new List<Projectile>();
    ParticleSystem particleSystem = null!;
    SpriteFont? defaultFont = null;
        Random rnd = new Random();

        // Audio
    Song? backgroundMusic = null;
    SoundEffect? jumpSound = null, dashSound = null, shootSound = null, explosionSound = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = ScreenW;
            graphics.PreferredBackBufferHeight = ScreenH;
        }

        protected override void Initialize()
        {
            camera = new Camera2D(GraphicsDevice.Viewport);
            map = new TileMap(100, 50, 32, rnd);
            player = new Player(new Vector2(100, 100));
            particleSystem = new ParticleSystem();
            for (int i = 0; i < 8; i++) enemies.Add(new Enemy(new Vector2(300 + i * 140, 120)));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            try { defaultFont = Content.Load<SpriteFont>("DefaultFont"); } catch {}

            map.GenerateTextures(GraphicsDevice);
            player.GenerateTextures(GraphicsDevice);
            foreach (var e in enemies) e.GenerateTextures(GraphicsDevice);
            particleSystem.GenerateTextures(GraphicsDevice);

            try
            {
                backgroundMusic = Content.Load<Song>("Music/background");
                jumpSound = Content.Load<SoundEffect>("Sounds/jump");
                dashSound = Content.Load<SoundEffect>("Sounds/dash");
                shootSound = Content.Load<SoundEffect>("Sounds/shoot");
                explosionSound = Content.Load<SoundEffect>("Sounds/explosion");
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.45f;
                MediaPlayer.Play(backgroundMusic);
            } catch { }
        }

        KeyboardState prevK;
        protected override void Update(GameTime gameTime)
        {
            var k = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (k.IsKeyDown(Keys.Escape)) Exit();

            player.Update(dt, k, prevK, map, projectiles, rnd, jumpSound, dashSound, shootSound);
            foreach (var e in enemies) e.Update(dt, player, map, projectiles, rnd);

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var p = projectiles[i];
                p.Update(dt);
                if (p.FromPlayer)
                {
                    for (int j = enemies.Count - 1; j >= 0; j--)
                    {
                        if (enemies[j].Bounds.Intersects(p.Bounds))
                        {
                            enemies[j].TakeDamage(p.Damage);
                            particleSystem.EmitExplosion(p.Position, 10, rnd);
                            explosionSound?.Play(0.4f, 0, 0);
                            projectiles.RemoveAt(i);
                            if (enemies[j].Dead) { particleSystem.EmitExplosion(enemies[j].Position, 30, rnd); enemies.RemoveAt(j); }
                            goto nextProj;
                        }
                    }
                }
                else
                {
                    if (player.Bounds.Intersects(p.Bounds))
                    {
                        player.TakeDamage(p.Damage);
                        explosionSound?.Play(0.3f, 0, 0);
                        particleSystem.EmitExplosion(p.Position, 6, rnd);
                        projectiles.RemoveAt(i);
                        goto nextProj;
                    }
                }
                if (map.IsSolidAtWorld(p.Position)) { particleSystem.EmitExplosion(p.Position, 6, rnd); explosionSound?.Play(0.2f,0,0); projectiles.RemoveAt(i); }
                nextProj: ;
            }

            camera.Position = Vector2.Lerp(camera.Position, player.Position - new Vector2(ScreenW/2, ScreenH/2), 0.12f);
            particleSystem.Update(dt);
            prevK = k;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);
            foreach (var e in enemies) e.Draw(spriteBatch);
            foreach (var p in projectiles) p.Draw(spriteBatch);
            particleSystem.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin();
            DrawUI(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void DrawUI(SpriteBatch sb)
        {
            int barW = 300; int x = 20, y = 20;
            sb.Draw(PrimitiveBatch.WhitePx, new Rectangle(x - 2, y - 2, barW + 4, 24 + 4), Color.Black * 0.7f);
            sb.Draw(PrimitiveBatch.WhitePx, new Rectangle(x, y, barW, 24), Color.Gray * 0.9f);
            int healthW = (int)(barW * MathHelper.Clamp(player.Health / player.MaxHealth, 0f, 1f));
            sb.Draw(PrimitiveBatch.WhitePx, new Rectangle(x, y, healthW, 24), Color.Red);
            if (defaultFont != null) sb.DrawString(defaultFont, $"Vie: {player.Health}/{player.MaxHealth}", new Vector2(20,52), Color.White);
        }
    }

    // -- utility classes (PrimitiveBatch, Camera2D, TileMap, Player, Enemy, Projectile, ParticleSystem) --
    // To keep the project compact for testing, the rest of the code is simplified but functional.
    // You can replace these with the more advanced versions you already have.

    public static class PrimitiveBatch
    {
        public static Texture2D? WhitePx;
        public static void Ensure(GraphicsDevice gd)
        {
            if (WhitePx == null) { WhitePx = new Texture2D(gd, 1, 1); WhitePx.SetData(new []{ Color.White }); }
        }
    }

    public class Camera2D { public Vector2 Position; Viewport vp; public Camera2D(Viewport vp) { this.vp = vp; Position = Vector2.Zero; } public Matrix GetViewMatrix() => Matrix.CreateTranslation(new Vector3(-Position, 0f)); }

    public class TileMap
    {
        public int Width, Height, TileSize; int[,] tiles; Texture2D? tileTex; Random rnd;
        public TileMap(int w,int h,int ts, Random rnd){ Width=w; Height=h; TileSize=ts; this.rnd=rnd; tiles=new int[w,h]; GenerateProcedural(); }
        void GenerateProcedural(){ for(int x=0;x<Width;x++){ int surface=(int)(Height*0.6f)+ (int)(Math.Sin(x*0.4f)*2); for(int y=surface;y<Height;y++) tiles[x,y]=1; } }
        public void GenerateTextures(GraphicsDevice gd){ PrimitiveBatch.Ensure(gd); tileTex=new Texture2D(gd, TileSize, TileSize); Color[] data=new Color[TileSize*TileSize]; for(int i=0;i<data.Length;i++) data[i]=new Color(50,45,40); tileTex.SetData(data); }
        public bool IsSolidAtWorld(Vector2 worldPos){ int tx=(int)(worldPos.X/TileSize), ty=(int)(worldPos.Y/TileSize); if(tx<0||tx>=Width||ty<0||ty>=Height) return false; return tiles[tx,ty]==1; }
        public Rectangle GetTileRect(int tx,int ty)=> new Rectangle(tx*TileSize, ty*TileSize, TileSize, TileSize);
        public void Draw(SpriteBatch sb){ for(int x=0;x<Width;x++) for(int y=0;y<Height;y++) if(tiles[x,y]==1) sb.Draw(tileTex!, GetTileRect(x,y), Color.White); }
    }

    public class Player
    {
        public Vector2 Position, Velocity; public float MaxSpeed=220f, Health=100f, MaxHealth=100f;
        Texture2D? tex; int jumpsLeft=2; bool canDash=true; float dashCooldown=0f; float fireCooldown=0f;
        public Rectangle Bounds => new Rectangle((int)Position.X-12,(int)Position.Y-18,24,36);
        public Player(Vector2 pos){ Position=pos; Velocity=Vector2.Zero; }
        public void GenerateTextures(GraphicsDevice gd){ tex=new Texture2D(gd,24,36); Color[] data=new Color[24*36]; for(int i=0;i<data.Length;i++) data[i]=Color.CornflowerBlue; tex.SetData(data); }
        public void Update(float dt, KeyboardState k, KeyboardState prevK, TileMap map, List<Projectile> projectiles, Random rnd, SoundEffect? jumpSound, SoundEffect? dashSound, SoundEffect? shootSound)
        {
            Vector2 input=Vector2.Zero; if(k.IsKeyDown(Keys.A)) input.X-=1; if(k.IsKeyDown(Keys.D)) input.X+=1;
            float accel=1200f; float targetVX=input.X*MaxSpeed; Velocity.X=MathHelper.Lerp(Velocity.X, targetVX, MathHelper.Clamp(accel*dt/200f,0,1));
            Velocity.Y+=1400f*dt;
            if(k.IsKeyDown(Keys.Space) && !prevK.IsKeyDown(Keys.Space) && jumpsLeft>0){ Velocity.Y=-520f; jumpsLeft--; jumpSound?.Play(0.45f,0,0); }
            if(k.IsKeyDown(Keys.LeftShift) && !prevK.IsKeyDown(Keys.LeftShift) && canDash){ Vector2 dashDir=new Vector2(input.X!=0?input.X:1,0); dashDir.Normalize(); Velocity+=dashDir*640f; canDash=false; dashCooldown=0.8f; dashSound?.Play(0.6f,0,0); }
            dashCooldown-=dt; if(dashCooldown<=0) canDash=true;
            fireCooldown-=dt;
            if(k.IsKeyDown(Keys.K) && fireCooldown<=0){ Vector2 dir=new Vector2(1,0); if(k.IsKeyDown(Keys.Left)) dir.X=-1; if(k.IsKeyDown(Keys.Right)) dir.X=1; if(k.IsKeyDown(Keys.Up)) dir=new Vector2(0,-1); dir.Normalize(); projectiles.Add(new Projectile(Position + new Vector2(0,-6) + dir*20, dir*700f, true, 18)); fireCooldown=0.12f; shootSound?.Play(0.5f,0,0); }
            Vector2 newPos=Position+Velocity*dt;
            if(map.IsSolidAtWorld(newPos + new Vector2(0,18))){ Velocity.Y=0; jumpsLeft=2; int ty=(int)((newPos.Y+18)/map.TileSize); newPos.Y = ty*map.TileSize - 18 - 0.01f; }
            if(map.IsSolidAtWorld(newPos + new Vector2(12,0))) Velocity.X=0;
            if(map.IsSolidAtWorld(newPos + new Vector2(-12,0))) Velocity.X=0;
            Position=newPos; Health=MathHelper.Clamp(Health + 2f*dt, 0, MaxHealth);
        }
        public void Draw(SpriteBatch sb) => sb.Draw(tex!, Position - new Vector2(tex!.Width/2, tex.Height/2), Color.White);
        public void TakeDamage(float d) { Health -= d; if (Health <= 0) { Position = new Vector2(100,100); Health = MaxHealth; Velocity = Vector2.Zero; } }
    }

    public enum EnemyState { Patrol, Alert, Attack }
    public class Enemy
    {
        public Vector2 Position, Velocity; public float Health=40f; public bool Dead => Health<=0f;
        Texture2D? tex; public Rectangle Bounds => new Rectangle((int)Position.X-12,(int)Position.Y-12,24,24);
        EnemyState state = EnemyState.Patrol; float stateTimer = 0f; Vector2 patrolTarget;
        Random rndLocal = new Random();
        public Enemy(Vector2 pos) { Position = pos; patrolTarget = pos + new Vector2(80, 0); }
        public void GenerateTextures(GraphicsDevice gd){ tex=new Texture2D(gd,24,24); Color[] data=new Color[24*24]; for(int i=0;i<data.Length;i++) data[i]=Color.Orange; tex.SetData(data); }
        public void Update(float dt, Player player, TileMap map, List<Projectile> projectiles, Random rnd) {
            float dist = Vector2.Distance(player.Position, Position);
            switch(state){
                case EnemyState.Patrol:
                    if(Vector2.Distance(Position,patrolTarget)<8f) patrolTarget = Position + new Vector2((rndLocal.NextDouble()>0.5?1:-1)*rndLocal.Next(40,200), 0);
                    Velocity = Vector2.Lerp(Velocity, Vector2.Normalize(patrolTarget - Position) * 40f, 0.05f);
                    if(dist < 240f) { state = EnemyState.Alert; stateTimer = 0f; }
                    break;
                case EnemyState.Alert:
                    stateTimer += dt; Velocity = Vector2.Lerp(Velocity, Vector2.Normalize(player.Position - Position) * 80f, 0.08f);
                    if(dist < 160f) state = EnemyState.Attack;
                    if(stateTimer > 6f) state = EnemyState.Patrol;
                    break;
                case EnemyState.Attack:
                    Velocity = Vector2.Lerp(Velocity, Vector2.Normalize(player.Position - Position) * 30f, 0.06f);
                    if(dist > 220f) state = EnemyState.Alert;
                    if(rndLocal.NextDouble() < 0.02){ Vector2 dir = Vector2.Normalize(player.Position - Position); projectiles.Add(new Projectile(Position + dir*16, dir*420f, false, 8)); }
                    break;
            }
            Velocity.Y += 800f * dt; Position += Velocity * dt;
            if(map.IsSolidAtWorld(Position + new Vector2(0,12))){ Velocity.Y = 0; int ty = (int)((Position.Y + 12) / map.TileSize); Position.Y = ty * map.TileSize - 12 - 0.01f; }
        }
        public void Draw(SpriteBatch sb) => sb.Draw(tex!, Position - new Vector2(tex!.Width/2, tex.Height/2), Color.White);
        public void TakeDamage(float d) { Health -= d; }
    }

    public class Projectile { public Vector2 Position, Velocity; public bool FromPlayer; public float Damage; public Rectangle Bounds => new Rectangle((int)Position.X-6,(int)Position.Y-6,12,12); public Projectile(Vector2 p, Vector2 v, bool fp, float dmg){ Position=p; Velocity=v; FromPlayer=fp; Damage=dmg; } public void Update(float dt){ Position += Velocity * dt; } public void Draw(SpriteBatch sb){ PrimitiveBatch.Ensure(sb.GraphicsDevice); sb.Draw(PrimitiveBatch.WhitePx!, Bounds, FromPlayer?Color.Yellow:Color.Purple); } }

    public class Particle { public Vector2 Position, Velocity; public float Life, MaxLife; }
    public class ParticleSystem { List<Particle> parts = new List<Particle>(); Texture2D? dot; public void GenerateTextures(GraphicsDevice gd){ PrimitiveBatch.Ensure(gd); dot=new Texture2D(gd,4,4); Color[] data=new Color[4*4]; for(int i=0;i<data.Length;i++) data[i]=Color.White; dot.SetData(data); } public void EmitExplosion(Vector2 pos,int count, Random rnd){ for(int i=0;i<count;i++){ float a=(float)(rnd.NextDouble()*Math.PI*2); float s=(float)(rnd.NextDouble()*240); parts.Add(new Particle{ Position=pos, Velocity=new Vector2((float)Math.Cos(a),(float)Math.Sin(a))*s, Life=0, MaxLife=0.6f + (float)rnd.NextDouble()*0.6f }); } } public void Update(float dt){ for(int i=parts.Count-1;i>=0;i--){ var p=parts[i]; p.Life += dt; if(p.Life >= p.MaxLife){ parts.RemoveAt(i); continue; } p.Position += p.Velocity * dt; p.Velocity *= 0.98f; p.Velocity.Y += 400f * dt; } } public void Draw(SpriteBatch sb){ foreach(var p in parts){ float t = p.Life / p.MaxLife; float s = MathHelper.Lerp(1.2f, 0.2f, t); sb.Draw(dot!, p.Position - new Vector2(2,2), null, Color.White * (1 - t), 0f, Vector2.Zero, s, SpriteEffects.None, 0f); } } }
}
