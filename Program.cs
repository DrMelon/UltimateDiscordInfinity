using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace CaffieneJam
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.theGame = new Game("ULTIMATE DISCORD: INFINITY", 400, 240, 60, false);
            Global.theGame.SetWindow(1280, 720, false, true);
            Global.theSession = Global.theGame.AddSession("MainPlayer");
            Global.theSession.Controller = new ControllerXbox360(0);
            Global.theController = Global.theSession.GetController<ControllerXbox360>();

            Global.theController.LeftStick.AddKeys(new Key[] { Otter.Key.Up, Otter.Key.Right, Otter.Key.Down, Otter.Key.Left });
            Global.theController.RightStick.AddKeys(new Key[] { Otter.Key.W, Otter.Key.D, Otter.Key.S, Otter.Key.A });
            Global.theController.Start.AddKey(Otter.Key.R);
            Global.theController.A.AddKey(Otter.Key.Q);
            Global.theController.B.AddKey(Otter.Key.B);


            Global.theGame.AddScene(new GameState());
            Global.theGame.Start();
        }
    }
}
