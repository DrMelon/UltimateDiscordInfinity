using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class Laser : Entity
    {

        float XSpeed;
        float YSpeed;

        float RemoveTimer = -1;

        public Laser(float x, float y, float xspd, float yspd)
        {
            X = x;
            Y = y;
            XSpeed = xspd;
            YSpeed = yspd;
            LifeSpan = 20.0f;

            Image laserImg = new Image(Assets.GFX_LASER);
            laserImg.CenterOrigin();
            laserImg.Smooth = true;

            AddGraphic(laserImg);
            AddCollider(new BoxCollider(8, 8, 3));
            Collider.CenterOrigin();
            Graphic.Angle = 360 - ((float)Math.Atan2(YSpeed, XSpeed) * (float)(180.0f / Math.PI));
        }

        public override void Update()
        {
            base.Update();
            X += XSpeed;
            Y += YSpeed;

            // Check collision
            if (Collider.Overlap(X, Y, 0) && RemoveTimer == -1)
            {
                // Bam, hit an enemy
                RemoveTimer = 1.0f;
            }

            if (RemoveTimer != -1)
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
