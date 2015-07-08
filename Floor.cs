using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;

namespace CaffieneJam
{
    class Floor : Entity
    {
       public ImageSet gfx;
        public Floor()
        {
            gfx = new ImageSet(Assets.GFX_FLOOR, 32, 32);
            gfx.Repeat = true;
            Graphic = gfx;

            Layer = 20;
        }
    }
}
