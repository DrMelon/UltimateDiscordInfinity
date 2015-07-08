using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class Drone : Entity
    {

        Spritemap<string> mySprite;

        public float MaxChargeTime = 120.0f;
        public float ChargeTime = 0.0f;
        public float MaxFireTime = 60.0f;
        public float CurrentFireTime = 0.0f;

        public Vector2 TargetPos;

        bool FireDir;

        int Hits = 1;

        // Activities:
        // 0 = None/Spawn
        // 1 = Charging
        // 2 = Firing
        // 3 = Dead

        public int CurrentActivity = 0;

        public Drone(float x, float y, int hits)
        {
            X = x;
            Y = y;
            mySprite = new Spritemap<string>(Assets.GFX_DRONE, 32, 38);
            mySprite.CenterOrigin();
            mySprite.Add("open", new int[] { 0, 1, 2, 3, 4 }, new float[] { 6f, 6f, 6f, 6f, 24f });
            mySprite.Anim("open").NoRepeat();
            mySprite.Anim("open").OnComplete = this.PlayShineAnim;
            mySprite.Add("shine", new int[] { 5, 6, 7, 8, 9, 10, 11 }, new float[] { 3f });
            mySprite.Anim("shine").NoRepeat();
            mySprite.Anim("shine").OnComplete = this.ChargeUpLaser;
            mySprite.Add("charge", new int[] { 12, 13 }, new float[] { 8f });
            mySprite.Add("fire", new int[] { 14, 15 }, new float[] { 1f });
            mySprite.Add("dead", new int[] { 16, 17, 18, 19, 20 }, new float[] { 10f, 4f, 4f, 4f, 4f });
            mySprite.Anim("dead").NoRepeat();
            mySprite.Anim("dead").OnComplete = this.StopShaking;
            mySprite.Play("open");
            mySprite.FlippedX = Rand.Bool;
           


            Image ghostShadow = new Image(Assets.GFX_SHADOW);
            ghostShadow.CenterOrigin();
            ghostShadow.OriginY -= 25;
            ghostShadow.Alpha = 0.5f;
            AddGraphic(ghostShadow);
            AddGraphic(mySprite);

            MaxChargeTime = Rand.Float(60.0f, 240.0f);
            

            AddCollider(new BoxCollider(32, 38, 2));
            Collider.CenterOrigin();
        }

        public override void Update()
        {
            base.Update();
            if(CurrentActivity == 0)
            {
                //nada
            }

            if(CurrentActivity == 1)
            {
                // charging laser
                if(ChargeTime < MaxChargeTime)
                {
                    ChargeTime++;
                    //update anim
                    mySprite.Anim("charge").Speed((ChargeTime / (MaxChargeTime)) * 4);

                    // drift towards player
                    Vector2 VectorToPlayer = new Vector2(Global.theGhost.X - X, Global.theGhost.Y - Y);

                    if(VectorToPlayer.Length > 500)
                    {
                        // teleport to player
                        X = Rand.Float(Global.theGhost.X - 400, Global.theGhost.X + 400);
                        Y = Rand.Float(Global.theGhost.Y - 400, Global.theGhost.Y + 400);
                    }

                    VectorToPlayer.Normalize();
                    X += VectorToPlayer.X * 0.5f;
                    Y += VectorToPlayer.Y * 0.5f;
                }
                else
                {
                    FireLaser();
                }
            }
            if(CurrentActivity == 2)
            {
                // firing laser
                if(CurrentFireTime < MaxFireTime)
                {
                    CurrentFireTime++;
                    // create laser particles, aim at player, then tween angle displacement slowly
                    float ProgressThroughFire = CurrentFireTime / MaxFireTime;
                    Vector2 VectorToPlayer = new Vector2(TargetPos.X - X, TargetPos.Y - Y);
                    float AngleToPlayer = (float)Math.Atan2(VectorToPlayer.Y, VectorToPlayer.X);
                    AngleToPlayer *= (float)(180.0 / Math.PI);
                    if(FireDir)
                    {
                        AngleToPlayer += (-15) + (ProgressThroughFire * 30);
                    }
                    else
                    {
                        AngleToPlayer -= (-15) + (ProgressThroughFire * 30);
                    }
                    

                    AngleToPlayer *= (float)(Math.PI / 180.0);
                    Laser newLaser = new Laser(X, Y, (float)Math.Cos(AngleToPlayer) * 7, (float)Math.Sin(AngleToPlayer) * 7);
                    this.Scene.Add(newLaser);

                }
                else
                {
                    ChargeUpLaser();
                }

            }

            if(CurrentActivity == 3)
            {
                RemoveColliders(Collider);
                mySprite.Play("dead");
                return;
            }

            if(Collider.Overlap(X, Y, 1))
            {
                // Hit by star, die
                // spawn music note & particles
                Hits--;
                if(Hits <= 0)
                {
                    this.Scene.Add(new Note(X, Y));
                    int maxSpawn = 1 + ((int)Global.Score / 200);
                    for (int i = 0; i < maxSpawn; i++)
                    {
                        this.Scene.Add(new Drone(Rand.Float(Global.theGhost.X - 400, Global.theGhost.X + 400), Rand.Float(Global.theGhost.Y - 400, Global.theGhost.Y + 400), maxSpawn * 2));
                    }
                    CurrentActivity = 3;
                    mySprite.OriginY -= 14;
                    Layer = 10;
                    mySprite.ShakeX = 3;
                    Sound bip = new Sound(Assets.SFX_HURT);
                    bip.Volume = 0.9f;
                    bip.Pitch = Rand.Float(0.8f, 1.2f);
                    bip.Play();
                }

            }
        }

        public void PlayShineAnim()
        {
            mySprite.Play("shine");
        }

        public void ChargeUpLaser()
        {
            mySprite.Play("charge");
            CurrentActivity = 1;

            ChargeTime = 0;

        }

        public void FireLaser()
        {
            mySprite.Play("fire");
            CurrentActivity = 2;
            CurrentFireTime = 0;
            FireDir = Rand.Bool;

            // capture player location/angle relative to us
            TargetPos = new Vector2(Global.theGhost.X, Global.theGhost.Y);
            Sound bip = new Sound(Assets.SFX_DRONEFIRE);
            bip.Volume = 0.3f;
            bip.Play();
        }

        public void StopShaking()
        {
            mySprite.ShakeX = 0;
        }
    }
}
