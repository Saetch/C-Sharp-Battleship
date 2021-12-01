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
        bool Active = false;
        private int Width { get; set; } = 0;
        private int Height { get; set; }=0;
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


        public bool IsActive()
        {
            return Active;
        }
    }



}
