using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class GameState : Scene
    {

      
        Floor theFloor;
        float TargetCamX, TargetCamY;
        bool Ready = false;
        Text ScoreText;
        float ScoreThreshNewBomb = 150;

        public GameState()
        {
            Global.theCamShaker = new CameraShaker();
            Add(Global.theCamShaker);

            Global.theMusic = new Music(Assets.MUS_DRIVE);



            Global.theGhost = new Ghost(100, 100);

            theFloor = new Floor();
            theFloor.gfx.Frame = 1;
            while(theFloor.gfx.Frame == 1)
            {
                theFloor.gfx.Frame = Rand.Int(0, 7);
            }
            Add(theFloor);

            Add(Global.theGhost);

            ScoreText = new Text(((int)Global.Score).ToString() + "     B: " + Global.Bombs.ToString(), Assets.FONT, 16);
            
            ScoreText.X = 165 + CameraX;
            ScoreText.Y = 0 + CameraY;
            ScoreText.OutlineQuality = TextOutlineQuality.Best;
            ScoreText.OutlineColor = Color.Black;
            ScoreText.OutlineThickness = 2;
            ScoreText.Smooth = false;
            ScoreText.Scroll = 0;
            
            Ready = false;
        }

        public void GetReady()
        {
            if(!Ready)
            {

                Global.theMusic.Play();
                SpawnDrones();
                Otter.Flash f = new Flash(Color.White);
                Add(f);
                Ready = true;
            }

        }

        public void SpawnDrones()
        {
            for(int i = 0; i < 32; i++)
            {
                Add(new Drone(Rand.Int(-300, 300), Rand.Int(-300, 300), 1));
            }
        }

        public override void UpdateLast()
        {
            base.UpdateLast();

            // Cam follow ghost

            TargetCamX = (Global.theGhost.X) + (Global.theController.RightStick.X * 48) + (Global.theController.LeftStick.X * 32);
            TargetCamY = (Global.theGhost.Y) + (Global.theController.RightStick.Y * 48) + (Global.theController.LeftStick.Y * 32); 

            float NewCamX = CameraCenterX;
            float NewCamY = CameraCenterY;

            if (TargetCamX - NewCamX < -16)
            {
                NewCamX -= 1;
                if(TargetCamX - NewCamX < -32)
                {
                    NewCamX -= 1 * (Global.Score / 100);
                }
            }
            if (TargetCamX - NewCamX > 16)
            {
                NewCamX += 1;
                if (TargetCamX - NewCamX > 32)
                {
                    NewCamX += 1 * (Global.Score / 100); 
                }
            }
            if (TargetCamY - NewCamY < -16)
            {
                NewCamY -= 1;
                if (TargetCamY - NewCamY < -32)
                {
                    NewCamY -= 1 * (Global.Score / 100);
                }
            }
            if (TargetCamY - NewCamY > 16)
            {
                NewCamY += 1;
                if (TargetCamY - NewCamY > 32)
                {
                    NewCamY += 1 * (Global.Score / 100);
                }
            }

            CenterCamera(NewCamX, NewCamY);

        }

        public override void Render()
        {
            base.Render();


           // Draw.Rectangle(TargetCamX - 5, TargetCamY - 5, 10, 10, Color.None, Color.Cyan, 1);
            ScoreText.Render();

        }

        public override void Update()
        {
            base.Update();
            ScoreText.String = ((int)Global.Score).ToString() + "     B: " + Global.Bombs.ToString();
            ScoreText.Update();
            Global.theMusic.Pitch = Global.Score / 100;
            if(Global.Score < 0)
            {
                //GAME OVER
                Global.theGhost.RemoveSelf();
                theFloor.gfx.Frame = 1;
            }

            if(Global.theController.Start.Pressed)
            {
                Global.theMusic.Stop();
                Global.theMusic.Dispose();
                Global.Score = 100;
                Global.Bombs = 3;
                Global.theGame.RemoveScene();
                Global.theGame.AddScene(new GameState());
            }
            if(Global.theController.A.Pressed)
            {
                GetReady();
            }

            if(Global.Score > ScoreThreshNewBomb)
            {
                Global.Bombs++;
                ScoreThreshNewBomb += 100;
            }
        }

    }
}
