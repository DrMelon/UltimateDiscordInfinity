using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class Star : Entity
    {

        float XSpeed, YSpeed;
        float RemoveTimer = -1;

        public Star(float x, float y, float xspd, float yspd)
        {
            X = x;
            Y = y - 10;
            XSpeed = xspd;
            YSpeed = yspd;

            LifeSpan = 60.0f;

            Spritemap<string> myGfx = new Spritemap<string>(Assets.GFX_STAR, 20, 19);
            myGfx.Add("default", new int[] {0, 1, 2, 3}, new float[] {1f});
            myGfx.Play("default");
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

            //Graphic.Angle += 5.0f;
            X += XSpeed;
            Y += YSpeed;

            // Check collisions
            if (Collider.Overlap(X, Y, 2) && RemoveTimer == -1)
            {
                // Bam, hit an enemy
                RemoveTimer = 1.0f;
            }

            if(RemoveTimer != -1)
            {
                if (RemoveTimer <= 0)
                {
                    RemoveSelf();
                }
                RemoveTimer--;
            }

            
        }

        public override void Removed()
        {

            // Create paff
            Particle paff = new Particle(X, Y, Assets.GFX_LASERBREAK, 18, 17);

            paff.LifeSpan = 10.0f;
            paff.FrameCount = 4;
            paff.FinalAlpha = 0.6f;
            paff.Image.CenterOrigin();

            this.Scene.Add(paff);


            base.Removed();
        }
    }
}
