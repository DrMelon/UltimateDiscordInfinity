using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class Ghost : Entity
    {
        public Spritemap<string> mySprite;

        public bool Impostor = false;

        public bool isDistorting = false;
        public float aliveTime;
        public float hurtTime = 0.0f;

        public Speed mySpeed;

        public float MaxRefire = 5.0f;
        public float RefireTime = 0.0f;

        public ControllerXbox360 myController;

        public Ghost(float x, float y, bool impostor)
        {
            X = x;
            Y = y;
            Impostor = impostor;

            mySprite = new Spritemap<string>(Assets.GFX_GHOST_SHEET, 22, 20);
            mySprite.Add("idle", new int[] { 0, 1, 2, 3 }, new float[] { 8f });
            mySprite.Add("distort", new int[] { 8, 9, 10 }, new float[] { 1f });
            mySprite.Play("idle");
            mySprite.CenterOrigin();


            mySpeed = new Speed(2);
           

            Image ghostShadow = new Image(Assets.GFX_SHADOW);
            ghostShadow.CenterOrigin();
            ghostShadow.Alpha = 0.5f;
            AddGraphic(ghostShadow);
            AddGraphic(mySprite);

            AddCollider(new BoxCollider(22, 20, 0));
            Collider.CenterOrigin();
            Layer = 5;

            if(!impostor)
            {
                myController = Global.theController;
            }
            
        }

        public override void UpdateFirst()
        {
            base.UpdateFirst();
            aliveTime++;
            // Drift up and down from shadow.
            mySprite.OriginY = 22 - (float)Math.Sin(aliveTime * 0.3f);

            if(hurtTime > 0)
            {
                hurtTime--;

                mySprite.Play("distort");
            }
            else
            {
                
               // Global.theMusic.Pitch = 1.0f;
                mySprite.Play("idle");
            }

        }

        public void HandleInput()
        {
            Vector2 moveDelta = new Vector2();
            moveDelta.X = myController.LeftStick.X * 0.1f;
            moveDelta.Y = myController.LeftStick.Y * 0.1f;

            MoveInDirection(moveDelta);

            Vector2 shootAxis = new Vector2();
            shootAxis.X = myController.RightStick.X;
            shootAxis.Y = myController.RightStick.Y;

            ShootInDirection(shootAxis);

            if ((myController.B.Pressed && Global.Bombs > 0))
            {
                DropBombs();
            }


        }

        public void HandleImpostorInput()
        {
            Vector2 impostorMove = new Vector2(X, Y);
            impostorMove -= new Vector2(Global.theGhost.X + (float)Math.Sin(Y * X) * 50, Global.theGhost.Y + (float)Math.Sin(Y * X*X) * 50);
            impostorMove.Normalize();
            MoveInDirection(-impostorMove);

            ShootInDirection(impostorMove);

        }

        public void MoveInDirection(Vector2 moveDelta)
        {
            mySpeed.X += moveDelta.X;
            mySpeed.Y += moveDelta.Y;

            mySpeed.Max = (Global.Score / 50) * 1.0f;

            if (Math.Abs(moveDelta.X) <= 0.1)
            {
                mySpeed.X *= 0.95f;
               
            }
            if(Math.Abs(moveDelta.Y) <= 0.1)
            {
                mySpeed.Y *= 0.95f;
            }


        }

        public void ShootInDirection(Vector2 shootAxis)
        {
            // Shoot stars
            if ((shootAxis.Length > 0.9) && RefireTime <= 0.0 && hurtTime <= 0.0)
            {
                RefireTime = MaxRefire;


                float XSpeedOfStar = 4.0f * shootAxis.X;
                float YSpeedOfStar = 4.0f * shootAxis.Y;

                Star newStar = new Star(X, Y, XSpeedOfStar, YSpeedOfStar);
                this.Scene.Add(newStar);
                Sound bip = new Sound(Assets.SFX_SHOOT);
                bip.Volume = 0.9f;
                bip.Play();

                mySpeed.X -= shootAxis.X * 0.2f;
                mySpeed.Y -= shootAxis.Y * 0.2f;

            }
        }

        public void DropBombs()
        {
            Sound bip = new Sound(Assets.SFX_CHARGE);
            bip.Volume = 0.9f;
            bip.Play();

            // Create 8 missiles
            for (int i = 0; i < 8; i++)
            {
                Missile newMissile = new Missile(X, Y);
                newMissile.Graphic.Angle = Rand.Float(0, 360);
                this.Scene.Add(newMissile);
            }
            Global.Bombs--;
        }

        public override void Update()
        {
            base.Update();

            // Input & Movement
            if (!Impostor)
            {
                HandleInput();
            }
            else
            {
                HandleImpostorInput();
            }

            X += mySpeed.X;
            Y += mySpeed.Y;

            if (RefireTime > 0.0f)
            {
                RefireTime--;
            }

            // Hit By Laser
            if (Collider.Overlap(X, Y, 3) && hurtTime <= 0.0f)
            {
                // Got hurt!
                hurtTime = 45.0f;
                Global.theCamShaker.ShakeCamera(20.0f);
                
                // reduce score / screw with music
                Global.Score -= 10 + ((Global.Score / 100) * 10);
                //Global.theMusic.Pitch -= 0.01f;
                Sound bip = new Sound(Assets.SFX_HURT);
                bip.Volume = 0.9f;
                bip.Play();
            }

        }
    }
}
