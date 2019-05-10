using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyGuy
{
    class Guy
    {        
        public float x, y, sx, sy, size;

        public Guy(float x, float y, float sx, float sy, float size)
        {
            this.x = x;
            this.y = y;
            this.sx = sx;
            this.sy = sy;
            this.size = size;
        }

        public void move(float time)
        {
            x += sx * time;
            y += sy * time;
        }
    }
}
