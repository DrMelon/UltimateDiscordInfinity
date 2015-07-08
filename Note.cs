using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class Note : Entity
    {
        public Note(float x, float y)
        {
            X = x;
            Y = y;
            Graphic = new Image(Assets.GFX_NOTE_SINGLE);
            Graphic.CenterOrigin();
        }

        public override void Update()
        {
            base.Update();

            // Fly towards player
            Vector2 VectorToPlayer = new Vector2(Global.theGhost.X - X, Global.theGhost.Y - Y - 10);

            if(VectorToPlayer.Length < 2)
            {
                Global.Score++;
                Sound bip = new Sound(Assets.SFX_GET);
                bip.Volume = 0.6f;
                
                bip.Pitch = Rand.Float(0.8f, 1.2f);
                bip.Play();
                RemoveSelf();
                //Global.theMusic.Pitch += 0.01f;
            }

            VectorToPlayer.Normalize();
            X += VectorToPlayer.X * 3;
            Y += VectorToPlayer.Y * 3;

            
        }
    }
}
