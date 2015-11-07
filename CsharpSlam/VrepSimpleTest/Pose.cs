using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrepSimpleTest
{
    struct Pose
    {
        /*public enum Type
        {
            Raw, Calculated
        }*/
        public Pose(int x, int y, double degree) {
            this.x = x;
            this.y = y;
            this.degree = degree;
        }
        public int x;
        public int y;
        public double degree;
    }
}
