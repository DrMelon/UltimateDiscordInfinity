using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;

namespace CaffieneJam
{
    class Missile : Entity
    {
        float MissileSpeed;
        float XSpeed, YSpeed;
        float RemoveTimer = -1;

        public Missile(float x, float y)
        {
            X = x;
            Y = y - 10;
            MissileSpeed = 0.5f;

            LifeSpan = 240.0f;

            Image myGfx = new Image(Assets.GFX_MISSILE);

            myGfx.Smooth = false;
            myGfx.CenterOrigin();
            //myGfx.Angle = Otter.Rand.Float(-180.0f, 180.0f);
            AddGraphic(myGfx);


            AddCollider(new BoxCollider(20, 19, 1));
            Collider.CenterOrigin();
        }

        public override void Update()
        {
            base.Update();

            Graphic.Angle += Rand.Float(-25.0f, 25.0f);
            XSpeed = (float)Math.Cos(Graphic.Angle * (float)(Math.PI / 180.0)) * MissileSpeed;
            YSpeed = -(float)Math.Sin(Graphic.Angle * (float)(Math.PI / 180.0)) * MissileSpeed;
            X += XSpeed;
            Y += YSpeed;
            if(MissileSpeed < 16)
            {
                MissileSpeed += 0.3f * (MissileSpeed / 16);
            }

            

            // Check collisions
            if (Collider.Overlap(X, Y, 2) && RemoveTimer == -1)
            {
                // Bam, hit an enemy
                RemoveTimer = 1.0f;
                Global.theCamShaker.ShakeCamera(20f);
            }

            if(RemoveTimer != -1)
            {
                if (RemoveTimer <= 0)
                {
                    RemoveSelf();
                }
                RemoveTimer--;
            }

            Particle paff = new Particle(X, Y, Assets.GFX_BUBBLE, 10, 10);

            paff.LifeSpan = 10.0f;
            paff.FrameCount = 5;
            paff.FinalAlpha = 0.8f;
            paff.Image.CenterOrigin();

            this.Scene.Add(paff);

            
        }

        public override void Removed()
        {

            // Create paff
            Particle paff = new Particle(X, Y, Assets.GFX_MISSILEBREAK, 32, 32);

            paff.LifeSpan = 10.0f;
            paff.FrameCount = 6;
            paff.FinalAlpha = 1.0f;
            paff.Image.CenterOrigin();

            this.Scene.Add(paff);
            Sound bip = new Sound(Assets.SFX_BOOM);
            bip.Volume = 0.9f;
            bip.Pitch = Rand.Float(0.8f, 1.2f);
            bip.Play();

            base.Removed();
        }
    
    }
}
