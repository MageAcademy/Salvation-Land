using System.Collections.Generic;

namespace com.PROS.SalvationLand
{
    public class Map
    {
        public Block[,] blocks;
        public int height;
        public List<Block> notWalkableBlockList = new List<Block>();
        public List<Block> walkableBlockList = new List<Block>();
        public int width;
    }
}