using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris
{
    public class GameState
    {
        private Block? currentBlock; 

        public MediaPlayer mediaPlayer = new MediaPlayer();

        public Block CurrentBlock
        {
            get => currentBlock!;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueque BlockQueque { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Block? HeldBlock { get; private set; }
        public bool CanHold { get; private set; }

        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueque = new BlockQueque();
            CurrentBlock = BlockQueque.GetAndUpdate();
            HeldBlock = null;
        }

        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }
            return true;
        }

        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }

            string musicPath = @"D:\VanoWijaya\VISUAL-STUDIO\Project-C-Tajam\Project-Game-C-Tajam\Tetris\Tetris\Music\select.mp3";
            Uri musicUri = null;
            bool soundAvailable = false;

            try
            {
                if (File.Exists(musicPath))
                {
                    musicUri = new Uri(musicPath, UriKind.Absolute);
                    soundAvailable = true;

                    // set max volume (0.0 - 1.0)
                    mediaPlayer.Volume = 1.0;
                }
                else
                {
                    Console.WriteLine($"Sound file not found at: {musicPath}");
                }
            }
            catch (Exception)
            {
                soundAvailable = false;
            }

            if (soundAvailable && musicUri != null)
            {
                try
                {
                    // If the same source is already opened, just rewind and play.
                    // This avoids re-opening the file and ensures immediate replay.
                    if (mediaPlayer.Source != null && mediaPlayer.Source == musicUri)
                    {
                        mediaPlayer.Position = TimeSpan.Zero;
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Open(musicUri);
                        mediaPlayer.Volume = 1.0; // ensure max
                        mediaPlayer.Play();
                    }
                }
                catch (Exception)
                {
                    // swallow playback exceptions (optional: log)
                }
            }


            if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueque.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }

        public void RotateCW()
        {

            string musicPath = @"D:\VanoWijaya\VISUAL-STUDIO\Project-C-Tajam\Project-Game-C-Tajam\Tetris\Tetris\Music\rotate.mp3";
            Uri musicUri = null;
            bool soundAvailable = false;

            try
            {
                if (File.Exists(musicPath))
                {
                    musicUri = new Uri(musicPath, UriKind.Absolute);
                    soundAvailable = true;

                    // set max volume (0.0 - 1.0)
                    mediaPlayer.Volume = 1.0;
                }
                else
                {
                    Console.WriteLine($"Sound file not found at: {musicPath}");
                }
            }
            catch (Exception)
            {
                soundAvailable = false;
            }

            if (soundAvailable && musicUri != null)
            {
                try
                {
                    // If the same source is already opened, just rewind and play.
                    // This avoids re-opening the file and ensures immediate replay.
                    if (mediaPlayer.Source != null && mediaPlayer.Source == musicUri)
                    {
                        mediaPlayer.Position = TimeSpan.Zero;
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Open(musicUri);
                        mediaPlayer.Volume = 1.0; // ensure max
                        mediaPlayer.Play();
                    }
                }
                catch (Exception)
                {
                    // swallow playback exceptions (optional: log)
                }
            }

            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }
        public void RotateCCW()
        {

            string musicPath = @"D:\VanoWijaya\VISUAL-STUDIO\Project-C-Tajam\Project-Game-C-Tajam\Tetris\Tetris\Music\rotate.mp3";
            Uri musicUri = null;
            bool soundAvailable = false;

            try
            {
                if (File.Exists(musicPath))
                {
                    musicUri = new Uri(musicPath, UriKind.Absolute);
                    soundAvailable = true;

                    // set max volume (0.0 - 1.0)
                    mediaPlayer.Volume = 1.0;
                }
                else
                {
                    Console.WriteLine($"Sound file not found at: {musicPath}");
                }
            }
            catch (Exception)
            {
                soundAvailable = false;
            }

            if (soundAvailable && musicUri != null)
            {
                try
                {
                    // If the same source is already opened, just rewind and play.
                    // This avoids re-opening the file and ensures immediate replay.
                    if (mediaPlayer.Source != null && mediaPlayer.Source == musicUri)
                    {
                        mediaPlayer.Position = TimeSpan.Zero;
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Open(musicUri);
                        mediaPlayer.Volume = 1.0; // ensure max
                        mediaPlayer.Play();
                    }
                }
                catch (Exception)
                {
                    // swallow playback exceptions (optional: log)
                }
            }

            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        { 
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += GameGrid.ClearFullRows();

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueque.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDintance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDintance(p));
            }

            return drop;
        }

        public void DropBlock()
        {

            string musicPath = @"D:\VanoWijaya\VISUAL-STUDIO\Project-C-Tajam\Project-Game-C-Tajam\Tetris\Tetris\Music\drop.mp3";
            Uri musicUri = null;
            bool soundAvailable = false;

            try
            {
                if (File.Exists(musicPath))
                {
                    musicUri = new Uri(musicPath, UriKind.Absolute);
                    soundAvailable = true;

                    // set max volume (0.0 - 1.0)
                    mediaPlayer.Volume = 1.0;
                }
                else
                {
                    Console.WriteLine($"Sound file not found at: {musicPath}");
                }
            }
            catch (Exception)
            {
                soundAvailable = false;
            }

            if (soundAvailable && musicUri != null)
            {
                try
                {
                    // If the same source is already opened, just rewind and play.
                    // This avoids re-opening the file and ensures immediate replay.
                    if (mediaPlayer.Source != null && mediaPlayer.Source == musicUri)
                    {
                        mediaPlayer.Position = TimeSpan.Zero;
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Open(musicUri);
                        mediaPlayer.Volume = 1.0; // ensure max
                        mediaPlayer.Play();
                    }
                }
                catch (Exception)
                {
                    // swallow playback exceptions (optional: log)
                }
            }

            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
