using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MediaPlayer mediaPlayer = new MediaPlayer();

        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        private readonly Image[,]? imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new GameState();


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayBackgroundMusic();

            try
            {
                mediaPlayer = new MediaPlayer();

                // Method 1: Using absolute path (for testing)
                string musicPath = @"D:\VanoWijaya\VISUAL-STUDIO\Project-C-Tajam\Project-Game-C-Tajam\Tetris\Tetris\Music\bg.mp3";
                Uri musicUri = new Uri(musicPath, UriKind.Absolute);
                // Check if file exists



                if (File.Exists(musicPath))
                {
                    mediaPlayer.Open(musicUri);
                    mediaPlayer.Volume = 1.0;
                    mediaPlayer.MediaEnded += MediaPlayer_MediaEndedMusic;
                    mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
                    mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                    mediaPlayer.Play();
                }


                else
                {
                    MessageBox.Show($"Music file not found at: {musicPath}\n\nPlease ensure:\n1. The file exists in the Music folder\n2. Build Action is set to 'Content'\n3. Copy to Output Directory is set to 'Copy if newer'");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading music: {ex.Message}");
            }
        }

        private void PlayBackgroundMusic()
        {
            try
            {
                mediaPlayer = new MediaPlayer();

                // Build the path to your music file
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string musicPath = System.IO.Path.Combine(baseDirectory, "Music", "bg.mp3");

                // Check if file exists
                if (File.Exists(musicPath))
                {
                    // Create URI from the path
                    Uri musicUri = new Uri(musicPath, UriKind.Absolute);

                    // Open and play
                    mediaPlayer.Open(musicUri);
                    mediaPlayer.Volume = 1.0; // Adjust volume (0.0 to 1.0)
                    mediaPlayer.MediaEnded += MediaPlayer_MediaEndedMusic;
                    mediaPlayer.Play();

                    Console.WriteLine("Music is playing!");
                }
                else
                {
                    MessageBox.Show($"Music file not found!\n\nLooking for: {musicPath}\n\nMake sure:\n1. bg.mp3 is in the Music folder\n2. Right-click bg.mp3 > Properties\n3. Build Action ='Content'\n4. Copy to Output Directory = 'Copy if newer'");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading music: {ex.Message}");
            }
        }

        private void GameOverSound()
        {
            try
            {
                // Stop any background music instances (MediaElement and MediaPlayer field)
                try
                {
                    BackgroundMusic?.Stop();
                }
                catch { /* ignore if BackgroundMusic not present */ }

                try
                {
                    mediaPlayer?.Stop();
                    mediaPlayer?.Close();
                }
                catch { /* ignore */ }

                // Use a local player for the game over sound so we do not interfere with the background player reference
                MediaPlayer gameOverPlayer = new MediaPlayer();
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string musicPath = System.IO.Path.Combine(baseDirectory, "Music", "gameover.mp3");
                if (File.Exists(musicPath))
                {
                    Uri musicUri = new Uri(musicPath, UriKind.Absolute);
                    gameOverPlayer.Open(musicUri);
                    gameOverPlayer.Volume = 1.0;
                    gameOverPlayer.Play();

                    // When the effect ends, stop and close this local player
                    gameOverPlayer.MediaEnded += (s, e) =>
                    {
                        try
                        {
                            if (s is MediaPlayer mp)
                            {
                                mp.Stop();
                                mp.Close();
                            }
                        }
                        catch { }
                    };

                    Console.WriteLine("Game Over sound is playing!");
                }
                else
                {
                    MessageBox.Show($"Game Over sound file not found at: {musicPath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Game Over sound: {ex.Message}");
            }
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            Console.WriteLine("Media opened successfully!");
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show($"Media failed to load: {e.ErrorException.Message}");
        }

        private void MediaPlayer_MediaEndedMusic(object sender, EventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
        }

        private void MediaPlayer_MediaEndedSound(object sender, EventArgs e)
        {
            // Prefer stopping the sender if it's a MediaPlayer, otherwise stop the field
            if (sender is MediaPlayer mp)
            {
                try
                {
                    mp.Stop();
                    mp.Close();
                }
                catch { }
            }
            else
            {
                mediaPlayer?.Stop();
            }
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            if (imageControls == null)
                throw new InvalidOperationException("imageControls is not initialized.");

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            if (imageControls == null)
                throw new InvalidOperationException("imageControls is not initialized.");

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueque blockQueque)
        {
            Block next = blockQueque.Nextblock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block? heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            if (imageControls == null)
                throw new InvalidOperationException("imageControls is not initialized.");

            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                var imageControl = imageControls[p.Row + dropDistance, p.Column];
                if (imageControl != null)
                {
                    imageControl.Opacity = 0.25;
                    imageControl.Source = tileImages[block.Id];
                }
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueque);
            DrawHeldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            // Stop background and play game over effect
            GameOverSound();

            // Show GameOver UI
            GameOverMenu.Visibility = Visibility.Visible;

            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A:
                    gameState.MoveBlockLeft();
                    break;
                case Key.D:
                    gameState.MoveBlockRight();
                    break;
                case Key.S:
                    gameState.MoveBlockDown();
                    break;
                case Key.W:
                    gameState.DropBlock();
                    break;
                case Key.Right:
                    gameState.RotateCW();
                    break;
                case Key.Left:
                    gameState.RotateCCW();
                    break;
                case Key.Up:
                    gameState.HoldBlock();
                    break;
                
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            PlayBackgroundMusic();


            // Restart background music (if using MediaElement)
            try
            {
                BackgroundMusic.Position = TimeSpan.Zero;
                BackgroundMusic.Play();
            }
            catch
            {
                // If BackgroundMusic not present, start the mediaPlayer-based background
                PlayBackgroundMusic();
            }

            await GameLoop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundMusic.Play();
        }

        private void BackgroundMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroundMusic.Position = TimeSpan.Zero;
            BackgroundMusic.Play();
        }

    }
}