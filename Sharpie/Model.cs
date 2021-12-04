using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Sharpie
{
    public enum StatusEnum
    {
        Inactive = 0,
        SettingUp = 1,
        Playing = 2,
        Ended = 3
    }

    public enum Direction
    {
        NoDirection = 0,
        Top = 1,
        Right =2,
        Down = 3,
        Left = 4
    }
    public class Model
    {
        public int Hitpoints { get; private set; } = 0;
        private int[,] OwnField;
        private int[,] OpponentField;

        public int ObjectX { get; private set; } = 0;
        public int ObjectY { get; private set; } = 0;

        private bool ObjectVertical = true;

        private int ObjectLength = 1;

        private int[] ShipsToAdd;

        public bool Active { get; private set; } = false;

        public int Status { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Model(int widthV, int heightV) {
            (Width, Height) = (widthV, heightV);
            PlayersTurn = new Random().Next() %2;
        }

        public int PlayersTurn
        {
            get;
            private set;
        }

        public void Activate()
        {
            Active = true;

            ShipsToAdd = new int[4];
            int fields = Width * Height;
            //add a somewhat dynamic number of ships, size 2 -> 3 -> 4 -> 5
            ShipsToAdd[0] = fields / 21;
            ShipsToAdd[1] = fields / 30;
            ShipsToAdd[2] = fields / 42;
            ShipsToAdd[3] = fields / 70;
            Status = ((int)(StatusEnum.SettingUp));

            DealWithNewShip(GetShipToPlaceSize());

        }

        private void DealWithNewShip(int length)
        {
            ObjectX = 0;
            ObjectY = 0;
            ObjectLength = length;
            ObjectVertical = true;
            ColorateObject();
        }

        private void DecolorateObject()
        {
            if(Status != (int)StatusEnum.Playing)
            {
                //color object
                for (int i = 0; i < ObjectLength; i++)
                {
                    if (ObjectVertical)
                    {
                        if (OwnField[ObjectX, ObjectY + i] >= 2)
                        {
                            OwnField[ObjectX, ObjectY + i] -= 2;
                        }

                    }
                    else
                    {
                        if (OwnField[ObjectX + i, ObjectY] >= 2)
                        {
                            OwnField[ObjectX + i, ObjectY] -= 2;
                        }

                    }
                }
            }
            else
            {
                OpponentField[ObjectX, ObjectY] += 2;
            }

        }

        private void ColorateObject()
        {


            if (Status != (int)StatusEnum.Playing)
            {
                    //color object
                 for (int i = 0; i < ObjectLength; i++)
                 {
                    if (ObjectVertical)
                    {
                        if (OwnField[ObjectX, ObjectY + i] >= 0)
                        {
                            OwnField[ObjectX, ObjectY + i] += 2;
                        }

                    }
                    else
                    {
                        if (OwnField[ObjectX+i, ObjectY ] >= 0)
                        {
                            OwnField[ObjectX+i, ObjectY ] += 2;
                        }

                    }
                 }
            }
            else
            {
                if(OpponentField[ObjectX, ObjectY] != -2)
                {
                    OpponentField[ObjectX, ObjectY] -= 2;
                }
            }



            
        }

        internal void StartFight()
        {
            ObjectX = 0;
            ObjectY = 0;
            ObjectLength = 1;

            ColorateObject();
        }

        private int GetShipToPlaceSize()
        {
            for(int i = 0; i < ShipsToAdd.Length; i++)
            {
                if(ShipsToAdd[i] != 0)
                {
                    ShipsToAdd[i]--;
                    return 2 + i;
                }
            }

            return 0;
        }

        public void ForceTurn(int f)
        {
            PlayersTurn = f;
            Activate();
        }

        internal void Hit()
        {
            OpponentField[ObjectX, ObjectY] = -1;
        }

        internal void Miss()
        {
            OpponentField[ObjectX, ObjectY] = 6;
        }

        public void ForceWidthHeight(int x, int y)
        {
            Width = x;
            Height = y;
        }
        public bool CreateFields()
        {
            if (Width < 6 || Height < 6) return false;
            //int array by default initialized with 0
            OwnField = new int[Width, Height];
            OpponentField = new int[Width, Height];

            return true;
        }

        internal bool GotHit()
        {
            if(--Hitpoints == 0)
            {
                Status = (int)StatusEnum.Ended;
                return false;
            }
            return true;

        }

        internal void Won()
        {
            Status = (int)StatusEnum.Ended;
        }

        public int getVal(int x, int y, int player)
        {
            return player==1 ? OpponentField[x, y] : OwnField[x, y];
        }

        internal bool Space()
        {
            if(Status != (int)StatusEnum.SettingUp)
            {
                return false;
            }
            DecolorateObject();
            ObjectVertical = !ObjectVertical;
            if (ObjectVertical)
            {
                while(ObjectY+ObjectLength > Height)
                {
                    ObjectY--;
                }

            }
            else
            {
                while (ObjectX + ObjectLength > Width)
                {
                    ObjectX--;
                }
            }
            ColorateObject();
            return true;
        }

        internal void Right()
        {
            if (!AllowedRight())
            {
                return;
            }
            DecolorateObject();
            ObjectX++;
            ColorateObject();
        }

        private bool AllowedRight()
        {
            int x = ObjectX;
            if (!ObjectVertical)
            {
                x += ObjectLength-1;
            }
            if (x + 1 >= Width) return false;
            return true;
        }

        internal void Down()
        {
            if (!AllowedDown())
            {
                return;
            }
            DecolorateObject();

            ObjectY++;
            ColorateObject();
        }

        private bool AllowedDown()
        {
            int y = ObjectY;
            if (ObjectVertical)
            {
                y += ObjectLength - 1;
            }
            return y + 1 < Height;
        }

        internal void Left()
        {
            if (!AllowedLeft())
            {
                return;
            }
            DecolorateObject();

            ObjectX--;
            ColorateObject();

        }

        private bool AllowedLeft()
        {
            return ObjectX > 0;
        }

        internal void Up()
        {
            if (!AllowedUp())
            {
                return;
            }
            DecolorateObject();
            ObjectY--;
            ColorateObject();

        }

        private bool AllowedUp()
        {
            return ObjectY > 0;
        }

        internal bool Enter()
        {
            if(Status == (int)StatusEnum.SettingUp)
            {
                if (!CanPlaceObject())
                {
                    return true;
                }

                DecolorateObject();

                int x = ObjectX;
                int y = ObjectY;
                for (int i = 0; i < ObjectLength; i++)
                {
                    OwnField[ObjectVertical ? x : x + i, ObjectVertical ? y + i : y] = 1;
                }
                Hitpoints += ObjectLength;
                x = GetShipToPlaceSize();

                if (x > 0)
                {
                    DealWithNewShip(x);
                    return true;
                }
                else
                {
                    Status = (int)StatusEnum.Playing;
                    return false;
                }
                
            }
            else
            {
                DecolorateObject();

                if (OpponentField[ObjectX, ObjectY] == 0)
                {
                    PlayersTurn++;
                    if (PlayersTurn > 1)
                    {
                        PlayersTurn = 0;
                    }
                    return false;
                }
                ColorateObject();
                return true;
            }

        }

        private bool CanPlaceObject()
        {

            int neededValue = 2;
            if(Status == 1)
            {
                neededValue = 2;
            }
            if(Status == (int)StatusEnum.Playing)
            {
                neededValue = -2;
            }
            int x = ObjectX;
            int y = ObjectY;
            for (int i = 0; i < ObjectLength; i++)
            {
                if(OwnField[ObjectVertical ? x : x + i, ObjectVertical ? y + i : y] != neededValue)
                {
                    return false;
                }
            }
            return true;
        }

        internal bool CheckForHit(int v1, int v2)
        {
            PlayersTurn = 0;
            return OwnField[v1, v2]!=0;
        }
    }



}
