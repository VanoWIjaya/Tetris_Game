using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris
{
    public class GameGrid
    {
        // single reusable player for the "clear" effect
        public MediaPlayer clearRows = new MediaPlayer();

        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }
        public int this[int r, int c]
        {
             get => grid[r, c];
            set => grid[r, c] = value;
        }

        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }

        public bool IsInside(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        public bool IsEmpty(int r, int c)
        {
            return IsInside(r, c) && grid[r, c] == 0;
        }

        public bool IsRowFull(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] == 0)
                    return false;
            }
            return true;
        }

        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] != 0)
                    return false;
            }
            return true;
        }

        private void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r, c] = 0;
            }
        }

        private void MoveRowDown(int r, int numRows)
        {
            for (int c =0; c< Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;

            // prepare sound path once
            string musicPath = @"D:\VanoWijaya\VISUAL-STUDIO\Project-C-Tajam\Project-Game-C-Tajam\Tetris\Tetris\Music\clear.mp3";
            Uri musicUri = null;
            bool soundAvailable = false;

            try
            {
                if (File.Exists(musicPath))
                {
                    musicUri = new Uri(musicPath, UriKind.Absolute);
                    soundAvailable = true;

                    // set max volume (0.0 - 1.0)
                    clearRows.Volume = 1.0;
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

            for (int r = Rows-1; r >= 0; r-- )
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;

                    if (soundAvailable && musicUri != null)
                    {
                        try
                        {
                            // If the same source is already opened, just rewind and play.
                            // This avoids re-opening the file and ensures immediate replay.
                            if (clearRows.Source != null && clearRows.Source == musicUri)
                            {
                                clearRows.Position = TimeSpan.Zero;
                                clearRows.Play();
                            }
                            else
                            {
                                clearRows.Open(musicUri);
                                clearRows.Volume = 1.0; // ensure max
                                clearRows.Play();
                            }
                        }
                        catch (Exception)
                        {
                            // swallow playback exceptions (optional: log)
                        }
                    }
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }
    }
}
