using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BlockQueque
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        private readonly Random random = new Random();

        public Block Nextblock { get; private set; }

        public BlockQueque()
        {
            Nextblock = RamdomBlock();
        }

        private Block RamdomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = Nextblock;

            do
            {           
                Nextblock = RamdomBlock();
            }
            while (Nextblock.Id == block.Id);

            return block;
        }

    }
}
