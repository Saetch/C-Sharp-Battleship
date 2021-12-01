using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpie
{
    public class Model
    {
        private int[,] OwnField;
        private int[,] OpponentField;

        private bool InProgress;
        public bool Active { get; private set; } = false;
        public int Width { get; private set; } = 0;
        public int Height { get; private set; }=0;
        public Model(int widthV, int heightV) => (Width, Height) = (widthV, heightV);

        public bool CreateFields()
        {
            if (Width < 6 || Height < 6) return false;
            //int array by default initialized with 0
            OwnField = new int[Width, Height];
            OpponentField = new int[Width, Height];

            return true;
        }

        public void AddShip(int x, int y, int x2, int y2)
        {

        }

    }



}
