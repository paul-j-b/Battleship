using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SwinGameSDK;


namespace Battleship
{
    /// <summary>
    /// The Resources Class stores all of the Games Media Resources, such as Images, Fonts
    /// Sounds, Music.
    /// </summary>
    public static class GameResources
    {
        private static Dictionary<string, Bitmap> _Images = new Dictionary<string, Bitmap>();
        private static Dictionary<string, Font> _Fonts = new Dictionary<string, Font>();
        private static Dictionary<string, SoundEffect> _Sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Music> _Music = new Dictionary<string, Music>();

        private static Bitmap _Background;
        private static Bitmap _Animation;
        private static Bitmap _LoaderFull;
        private static Bitmap _LoaderEmpty;
        private static Font _LoadingFont;
        //private static SoundEffect _StartSound;

        //This method is used to call smaller methods to load particular resource types, and display a message to the user for each load.
        public static void LoadResources()
        {
            int width, height;

            width = SwinGame.ScreenWidth();
            height = SwinGame.ScreenHeight();

            SwinGame.ChangeScreenSize(800, 600);

            ShowLoadingScreen();

            ShowMessage("Loading fonts...", 0);
            LoadFonts();
            SwinGame.Delay(100);

            ShowMessage("Loading images...", 1);
            LoadImages();
            SwinGame.Delay(100);

            ShowMessage("Loading sounds...", 2);
            LoadSounds();
            SwinGame.Delay(100);

            ShowMessage("Loading music...", 3);
            LoadMusic();
            SwinGame.Delay(100);

            SwinGame.Delay(100);
            ShowMessage("Game loaded...", 5);
            SwinGame.Delay(100);
            EndLoadingScreen(width, height);
        }

        //Load 4 new fonts in for use during the game, using the NewFont method. 
        private static void LoadFonts()
        {
            NewFont("ArialLarge", "arial.ttf", 80);
            NewFont("Courier", "cour.ttf", 14);
            NewFont("CourierSmall", "cour.ttf", 8);
            NewFont("Menu", "ffaccess.ttf", 8);
        }

        //Load images used in the game's background, deployment screen and ship images. 
        private static void LoadImages()
        {
            // Backgrounds
            NewImage("Menu", "main_page.jpg");
            NewImage("Discovery", "discover.jpg");
            NewImage("Deploy", "deploy.jpg");

            // Deployment
            NewImage("LeftRightButton", "deploy_dir_button_horiz.png");
            NewImage("UpDownButton", "deploy_dir_button_vert.png");
            NewImage("SelectedShip", "deploy_button_hl.png");
            NewImage("PlayButton", "deploy_play_button.png");
            NewImage("RandomButton", "deploy_randomize_button.png");

            // Ships
            for (int i = 1; i <= 5; i++)
            {
                NewImage("ShipLR" + Convert.ToString(i), "ship_deploy_horiz_" + Convert.ToString(i) + ".png");
                NewImage("ShipUD" + Convert.ToString(i), "ship_deploy_vert_" + Convert.ToString(i) + ".png");
            }

            // Explosions
            NewImage("Explosion", "explosion.png");
            NewImage("Splash", "splash.png");
        }

        //Load sound resources with the NewSound method
        private static void LoadSounds()
        {
            NewSound("Error", "error.wav");
            NewSound("Hit", "hit.wav");
            NewSound("Sink", "sink.wav");
            NewSound("Siren", "siren.wav");
            NewSound("Miss", "watershot.wav");
            NewSound("Winner", "winner.wav");
            NewSound("Lose", "lose.wav");
        }
        
        //Load the game music
        private static void LoadMusic()
        {
            NewMusic("Background", "horrordrone.mp3");
        }

        /// <summary>
        /// Gets a Font Loaded in the Resources
        /// </summary>
        /// <param name="font">Name of Font</param>
        /// <returns>The Font Loaded with this Name</returns>
        public static Font GameFont(string font)
        {
            return _Fonts[font];
        }

        /// <summary>
        /// Gets an Image loaded in the Resources
        /// </summary>
        /// <param name="image">Name of image</param>
        /// <returns>The image loaded with this name</returns>
        public static Bitmap GameImage(string image)
        {
            return _Images[image];
        }

        /// <summary>
        /// Gets an sound loaded in the Resources
        /// </summary>
        /// <param name="sound">Name of sound</param>
        /// <returns>The sound with this name</returns>
        public static SoundEffect GameSound(string sound)
        {
            return _Sounds[sound];
        }

        /// <summary>
        /// Gets the music loaded in the Resources
        /// </summary>
        /// <param name="music">Name of music</param>
        /// <returns>The music with this name</returns>
        public static Music GameMusic(string music)
        {
            return _Music[music];
        }

        //Creates a variable to store a background image file to then be displaying during loading
        //Calls the method to play the SwinGame introduction
        private static void ShowLoadingScreen()
        {
            _Background = SwinGame.LoadBitmap(SwinGame.PathToResource("SplashBack.png", ResourceKind.BitmapResource));
            SwinGame.DrawBitmap(_Background, 0, 0);
            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();

            _Animation = SwinGame.LoadBitmap(SwinGame.PathToResource("SwinGameAni.jpg", ResourceKind.BitmapResource));
            _LoadingFont = SwinGame.LoadFont(SwinGame.PathToResource("arial.ttf", ResourceKind.FontResource), 12);
            //_StartSound = Audio.LoadSoundEffect(SwinGame.PathToResource("SwinGameStart.ogg", ResourceKind.SoundResource));

            _LoaderFull = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_full.png", ResourceKind.BitmapResource));
            _LoaderEmpty = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_empty.png", ResourceKind.BitmapResource));

            PlaySwinGameIntro();
        }

        //Shows the background image variable with a 20ms pause for the 11 iterations specified by ANI_CELL_COUNT
        private static void PlaySwinGameIntro()
        {
            const int ANI_CELL_COUNT = 11;

            //Audio.PlaySoundEffect(_StartSound);
            SwinGame.Delay(200);

            int i;
            for (i = 0; i <= ANI_CELL_COUNT - 1; i++)
            {
                SwinGame.DrawBitmap(_Background, 0, 0);
                SwinGame.Delay(20);
                SwinGame.RefreshScreen();
                SwinGame.ProcessEvents();
            }

            SwinGame.Delay(1500);
        }

        //This method is called in the main LoadResources method, to display a message to the user about the resources being loaded at each time
        private static void ShowMessage(string message, int number)
        {
            const int TX = 310;
            const int TY = 493;
            const int TW = 200;
            const int TH = 25;
            const int STEPS = 5;
            const int BG_X = 279;
            const int BG_Y = 453;

            int fullW;
            Rectangle toDraw = default;

            fullW = (260 * number) / STEPS;
            SwinGame.DrawBitmap(_LoaderEmpty, BG_X, BG_Y);
            SwinGame.DrawCell(_LoaderFull, 0, BG_X, BG_Y);
            // SwinGame.DrawBitmapPart(_LoaderFull, 0, 0, fullW, 66, BG_X, BG_Y)

            toDraw.X = TX;
            toDraw.Y = TY;
            toDraw.Width = TW;
            toDraw.Height = TH;
            SwinGame.DrawText(message, Color.White, Color.Transparent, _LoadingFont, FontAlignment.AlignCenter, toDraw);
            // SwinGame.DrawTextLines(message, Color.White, Color.Transparent, _LoadingFont, FontAlignment.AlignCenter, TX, TY, TW, TH)

            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();
        }

        //Clears the bitmaps that were loaded to display background images
        private static void EndLoadingScreen(int width, int height)
        {
            SwinGame.ProcessEvents();
            SwinGame.Delay(500);
            SwinGame.ClearScreen();
            SwinGame.RefreshScreen();
            SwinGame.FreeFont(_LoadingFont);
            SwinGame.FreeBitmap(_Background);
            SwinGame.FreeBitmap(_Animation);
            SwinGame.FreeBitmap(_LoaderEmpty);
            SwinGame.FreeBitmap(_LoaderFull);
            //SwinGame.FreeSoundEffect(_StartSound);


            //Resets the screen size if the loading screen to width/height variables, instead of set 800x600 for loading screen.
            SwinGame.ChangeScreenSize(width, height);
        }

        //Used to load a new font into the game's memory, by identifying the font name and file location
        private static void NewFont(string fontName, string filename, int size)
        {
            _Fonts.Add(fontName, SwinGame.LoadFont(SwinGame.PathToResource(filename, ResourceKind.FontResource), size));
        }

        //Used to load a new image into the game's memory, by identifying the image name and file location
        private static void NewImage(string imageName, string filename)
        {
            _Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(filename, ResourceKind.BitmapResource)));
        }

        //Used to load a new transparent image into the game's memory, by identifying the image name and file location
        private static void NewTransparentColorImage(string imageName, string fileName, Color transColor)
        {
            _Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(fileName, ResourceKind.BitmapResource)));
        }

        //This method is not used
        private static void NewTransparentColourImage(string imageName, string fileName, Color transColor)
        {
            NewTransparentColorImage(imageName, fileName, transColor);
        }

        //Used to load a new sound into the game's memory, by identifying the sound name and file location
        private static void NewSound(string soundName, string filename)
        {
            _Sounds.Add(soundName, Audio.LoadSoundEffect(SwinGame.PathToResource(filename, ResourceKind.SoundResource)));
        }

        //Used to load a new music track into the game's memory, by identifying the name and file location
        private static void NewMusic(string musicName, string filename)
        {
            _Music.Add(musicName, Audio.LoadMusic(SwinGame.PathToResource(filename, ResourceKind.SoundResource)));
        }

        //This method can be called to clear the font variables in memory
        private static void FreeFonts()
        {
            Font obj = default;
            foreach (var o in _Fonts.Values)
                SwinGame.FreeFont(o);
        }

        //This method can be called to clear the image variables in memory
        private static void FreeImages()
        {
            Bitmap obj = default(Bitmap);
            foreach (var o in _Images.Values)
                SwinGame.FreeBitmap(o);
        }

        // ** BUG **
        // ** Exception unhandled. **
        private static void FreeSounds()
        {
            SoundEffect obj = default(SoundEffect);
            foreach (var o in _Sounds.Values)
                Audio.FreeSoundEffect(o);
        }

        //This method can be called to clear the music variables in memory
        private static void FreeMusic()
        {
            Music obj = default(Music);
            foreach (var o in _Music.Values)
                Audio.FreeMusic(o);
        }

        //This method is used to call each of the resource types relevant clear resources in memory
        public static void FreeResources()
        {
            FreeFonts();
            FreeImages();
            FreeMusic();
            FreeSounds();
            SwinGame.ProcessEvents();
        }
    }

}