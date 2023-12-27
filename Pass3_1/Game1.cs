//Author: Shaun Feldman
//File Name: Pass3
//Project Name: Pass3_1
//Creation Date: December 17, 2021
//Modified Date: January 21, 2021
//Description: A 2 player fighting game, each player can choose a character, different characters have different speeds for different attacks, they then choose a map to fight on. Then they FIGHT! After which there is a winner and they can redo their selection and fight again. ITS PURE FIGHTING MANIA.
              //A lot of animations, timer, gamestates


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Animation2D;
using Helper;


namespace Pass3_1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Game States - Add/Remove/Modify as needed
        //These are the most common game states, but modify as needed
        //You will ALSO need to modify the two switch statements in Update and Draw
        const int MENU = 0;
        const int CHARACTER = 1;
        const int INSTRUCTIONS = 2;
        const int GAMEPLAY = 3;
        const int PAUSE = 4;
        const int ENDGAME = 5;
        const int MAP = 6;

        //A GamePadState variable to hold the current state of both controllers and previous state
        GamePadState[] gamePad = new GamePadState[2];
        GamePadState[] gamePadPrev = new GamePadState[2];

        //Bool variables to see if the players should still be selecting or not, as well as a bool to set a few variables when the game starts
        bool selection1 = true;
        bool selection2 = true;
        bool startingSetup = true;

        //Array of bools to store if the player should be moving, if theyre jumping, or if they're blocking damage from the enemy
        bool[] moves = new bool[] { false, false };
        bool[] jumpMoves = new bool[] { false, false };
        bool[] blockDamage = new bool[] { false, false };

        //Ints to store the position of the boxes, the line position, the distance the line should move, the screens height and width, and the direction of players as well as how many players there are
        int box1Pos = 1;
        int box2Pos = 2;
        int linePos = 1;
        int lineMoveOneSpace = 100;
        int screenWidth;
        int screenHeight;
        int[] dir = new int[] { 1, -1 };
        int[] player = new int[2];

        //Variabel to containe the speeds of the two players, the damage they can do, as well as their starting health
        double[] maxSpeed = new double[] { 5, 5 };
        double[] damage = new double[2];
        double[] health = new double[] { 200, 200 };

        //A timer variable to have a starting timer
        Timer countdownTimer;

        //Texture2D variables to store the menu background, the play button, the settings button, the exit button, the character background images, the character boxes, the player images, the background images, and the health bar images
        Texture2D menuImg;
        Texture2D playButton;
        Texture2D settingsButton;
        Texture2D exitButton;
        Texture2D characterBackground;
        Texture2D[] characterBoxes = new Texture2D[2];
        Texture2D[] playerImages = new Texture2D[3];
        Texture2D[] backgroundImg = new Texture2D[3];
        Texture2D[] healthBarImg = new Texture2D[14];

        //Vector2 locations to store the menu text location, the underline location, directional distances correlating to where the gamepad thumbstick is facing, a loaction for the character selection boxes and a location for the timer
        Vector2 menuFontLoc = new Vector2(100, 100);
        Vector2 underLineLoc = new Vector2(140, 280);
        Vector2 thumbLoc1 = new Vector2(1, 1);
        Vector2 thumbLoc2 = new Vector2(-1, -1);
        Vector2 movingLoc = new Vector2(0, 0);
        Vector2 characterSelectLoc = new Vector2(150, 50);
        Vector2 timerLoc = new Vector2((1400 / 2) - 30, 100);

        //A rectangle variable to store the menu image, the play setting and exit buttons, the character backgrounds, the character boxes, the 3 player image, the 3 map images, the screen background image for the gameplay, and the healthbar
        Rectangle menuRec;
        Rectangle playButtonRec;
        Rectangle settingsButtonRec;
        Rectangle exitButtonRec;
        Rectangle characterBackgroundRec;
        Rectangle characterBoxes1Rec;
        Rectangle characterBoxes2Rec;
        Rectangle[] playerImageRec = new Rectangle[3];
        Rectangle[] mapRec = new Rectangle[3];
        Rectangle screenRec;
        Rectangle[] healthBarRec = new Rectangle[2];

        //A spritefont variable to store the menu font, the underline font, the character select font, the health of the players, and the timer
        SpriteFont menuFont;
        SpriteFont underLine;
        SpriteFont characterSelect;
        SpriteFont healthPlayers;
        SpriteFont timerCount;

        //Text variables to store the text I want to produce, the title screen, the underline, the character names, the characters selected, the map selected, the map names, the health of the players
        string menuTxt = "Fight Time";
        string underLineTxt = "____";
        string selectTxt = "";
        string[] characters = new string[] { "Guile", "Thor", "Kia" };
        string[] characterSelected = new string[] { "       ", "       " };
        string mapSelected = "";
        string[] maps = new string[] { "Original", "Stadium", "Castle" };
        string healthPlayersTxt = "";

        //Vectore 2 locations to store the location of player one and player 2
        static Vector2 playerOneLoc = new Vector2(400, 600);
        static Vector2 playerTwoLoc = new Vector2(950, 600);


        //Animations array for guile
        Texture2D[] guileAnimationImg = new Texture2D[10];


        //Animations array for Thor
        Texture2D[] thorAnimationImg = new Texture2D[10];


        //Animations array for Kia
        Texture2D[] kiaAnimationImg = new Texture2D[10];


        //Overall Animation

        //Vector2 array to store the location of both players
        Vector2[] animationLoc = new Vector2[] { playerOneLoc, playerTwoLoc };

        //Animation variable array to store all of the arrays
        Animation[] animationGTK = new Animation[30];   //In order 0-9 guile, 10 - 19 thor, 20 - 29 kia,
                                                        //[0, 10, 20] - walking
                                                        //[1, 11, 21] - idle standing
                                                        //[2, 12, 22] - light punch
                                                        //[3, 13, 23] - heavy punch
                                                        //[4, 14, 24] - jump
                                                        //[5, 15, 25] - block
                                                        //[6, 16, 26] - kick
                                                        //[7, 17, 27] - jump + kick/punch
                                                        //[8, 18, 28] - lose emote
                                                        //[9, 19, 29] - win emote


        //Store and set the initial game state, typically MENU to start
        int gameState = MENU;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Sets the screen width and height to my preffered standards
            this._graphics.PreferredBackBufferWidth = 1400;
            this._graphics.PreferredBackBufferHeight = 790;

            this._graphics.ApplyChanges();

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            screenWidth = _graphics.GraphicsDevice.Viewport.Width;
            screenHeight = _graphics.GraphicsDevice.Viewport.Height;

            //Loads all the images into the game, background, buttons, players, boxes, etc..
            menuImg = Content.Load<Texture2D>("Images/Sprites/MenuImg");
            playButton = Content.Load<Texture2D>("Images/Sprites/buttonsGameStart");
            settingsButton = Content.Load<Texture2D>("Images/Sprites/buttonsGameSettings");
            exitButton = Content.Load<Texture2D>("Images/Sprites/buttonsGameExit");
            characterBackground = Content.Load<Texture2D>("Images/Sprites/background character selection");
            characterBoxes[0] = Content.Load<Texture2D>("Images/Sprites/characterBoxes");
            characterBoxes[1] = Content.Load<Texture2D>("Images/Sprites/characterBoxes");
            playerImages[0] = Content.Load<Texture2D>("Images/Sprites/FightingAni-1");
            playerImages[1] = Content.Load<Texture2D>("Images/Sprites/FightingAni2");
            playerImages[2] = Content.Load<Texture2D>("Images/Sprites/FightingAni-3");
            backgroundImg[0] = Content.Load<Texture2D>("Images/Sprites/Background1");
            backgroundImg[1] = Content.Load<Texture2D>("Images/Sprites/Background2");
            backgroundImg[2] = Content.Load<Texture2D>("Images/Sprites/Background3");

            //Loads the texts, menu text, underlines, character selection, the health, the timer
            menuFont = Content.Load<SpriteFont>("Fonts/MenuTxt");
            underLine = Content.Load<SpriteFont>("Fonts/MenuTxt");
            characterSelect = Content.Load<SpriteFont>("Fonts/CharacterSelection+Names");
            healthPlayers = Content.Load<SpriteFont>("Fonts/CharacterSelection+Names");
            timerCount = Content.Load<SpriteFont>("Fonts/timerCount.spritefont");

            //Loads the rectangles into the game, the menus rec, the buttons, the boxes, the images, the maps, the screen, the health bars, etc..
            menuRec = new Rectangle(0, -110, screenWidth, screenHeight + 110);
            playButtonRec = new Rectangle(117, 295, 200, 100);
            settingsButtonRec = new Rectangle(122, 400, 200, 100);
            exitButtonRec = new Rectangle(127, 487, 193, 77);
            characterBackgroundRec = new Rectangle(0, 0, screenWidth, screenHeight);
            characterBoxes1Rec = new Rectangle(150, 400, 150, 250);
            characterBoxes2Rec = new Rectangle(400, 400, 150, 250);
            playerImageRec[0] = new Rectangle(30, 375, 420, 275);
            playerImageRec[1] = new Rectangle(375, 400, 200, 250);
            playerImageRec[2] = new Rectangle(550, 400, 375, 250);
            mapRec[0] = new Rectangle(150, 400, 150, 250);
            mapRec[1] = new Rectangle(400, 400, 150, 250);
            mapRec[2] = new Rectangle(650, 400, 150, 250);
            screenRec = new Rectangle(0, 0, screenWidth, screenHeight);
            healthBarRec[0] = new Rectangle(125, 100, 600, 300);
            healthBarRec[1] = new Rectangle(750, 100, 600, 300);

            //Loops to load the different health bars images
            for (int i = 0; i <= 7; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull");
            }
            for (int i = 1; i <= 8; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull-1");
            }
            for (int i = 2; i <= 9; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull-2");
            }
            for (int i = 3; i <= 10; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull-3");
            }
            for (int i = 4; i <= 11; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull-4");
            }
            for (int i = 5; i <= 12; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull-5");
            }
            for (int i = 6; i <= 13; i += 7)
            {
                healthBarImg[i] = Content.Load<Texture2D>("Images/Sprites/HealthBarFull-6");
            }

            //guile Animation loading in the images
            guileAnimationImg[0] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-0");
            guileAnimationImg[1] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-1");
            guileAnimationImg[2] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-2");
            guileAnimationImg[3] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-3");
            guileAnimationImg[4] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-4");
            guileAnimationImg[5] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-5");
            guileAnimationImg[6] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-6");
            guileAnimationImg[7] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-7");
            guileAnimationImg[8] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-8");
            guileAnimationImg[9] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-9");

            //Sets the information in the animation for guile
            animationGTK[0] = new Animation(guileAnimationImg[0], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 4, animationLoc[0], 0.5f, false);
            animationGTK[1] = new Animation(guileAnimationImg[1], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 8, animationLoc[0], 0.37f, true);
            animationGTK[2] = new Animation(guileAnimationImg[2], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, (int)5.6, animationLoc[0], 0.5f, false);
            animationGTK[3] = new Animation(guileAnimationImg[3], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, animationLoc[0], 0.7f, false);
            animationGTK[4] = new Animation(guileAnimationImg[4], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 7, animationLoc[0], 0.38f, false);
            animationGTK[5] = new Animation(guileAnimationImg[5], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.38f, false);
            animationGTK[6] = new Animation(guileAnimationImg[6], 8, 1, 8, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, animationLoc[0], 1.3f, false);
            animationGTK[7] = new Animation(guileAnimationImg[7], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 5, animationLoc[0], 0.45f, false);
            animationGTK[8] = new Animation(guileAnimationImg[8], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.45f, false);
            animationGTK[9] = new Animation(guileAnimationImg[9], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.45f, false);

            //Thor Animation
            thorAnimationImg[0] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-10");
            thorAnimationImg[1] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-11");
            thorAnimationImg[2] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-12");
            thorAnimationImg[3] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-13");
            thorAnimationImg[4] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-14");
            thorAnimationImg[5] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-15");
            thorAnimationImg[6] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-16");
            thorAnimationImg[7] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-17");
            thorAnimationImg[8] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-18");
            thorAnimationImg[9] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-19");

            //Sets the information in the animation for thor
            animationGTK[10] = new Animation(thorAnimationImg[0], 6, 1, 6, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, animationLoc[0], 1.2f, false);
            animationGTK[11] = new Animation(thorAnimationImg[1], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 9, animationLoc[0], 0.65f, true);
            animationGTK[12] = new Animation(thorAnimationImg[2], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 5, animationLoc[0], 0.8f, false);
            animationGTK[13] = new Animation(thorAnimationImg[3], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 6, animationLoc[0], 1.2f, false);
            animationGTK[14] = new Animation(thorAnimationImg[4], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 7, animationLoc[0], 0.78f, false);
            animationGTK[15] = new Animation(thorAnimationImg[5], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.53f, false);
            animationGTK[16] = new Animation(thorAnimationImg[6], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 6, animationLoc[0], 0.75f, false);
            animationGTK[17] = new Animation(thorAnimationImg[7], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 13, animationLoc[0], 0.55f, false);
            animationGTK[18] = new Animation(thorAnimationImg[8], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.5f, false);
            animationGTK[19] = new Animation(thorAnimationImg[9], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.35f, false);


            //Kia Animation
            kiaAnimationImg[0] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-20");
            kiaAnimationImg[1] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-21");
            kiaAnimationImg[2] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-22");
            kiaAnimationImg[3] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-23");
            kiaAnimationImg[4] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-24");
            kiaAnimationImg[5] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-25");
            kiaAnimationImg[6] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-26");
            kiaAnimationImg[7] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-27");
            kiaAnimationImg[8] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-28");
            kiaAnimationImg[9] = Content.Load<Texture2D>("Images/AnimationImg/FightingAni-29");

            //Sets the information in the animation for kia
            animationGTK[20] = new Animation(kiaAnimationImg[0], 5, 1, 5, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 5, animationLoc[0], 0.625f, false);
            animationGTK[21] = new Animation(kiaAnimationImg[1], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 8, animationLoc[0], 0.48f, true);
            animationGTK[22] = new Animation(kiaAnimationImg[2], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, (int)4.5, animationLoc[0], 0.4f, false);
            animationGTK[23] = new Animation(kiaAnimationImg[3], 4, 1, 4, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, animationLoc[0], 0.58f, false);
            animationGTK[24] = new Animation(kiaAnimationImg[4], 6, 1, 6, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 8, animationLoc[0], 0.67f, false);
            animationGTK[25] = new Animation(kiaAnimationImg[5], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 10, animationLoc[0], 0.38f, false);
            animationGTK[26] = new Animation(kiaAnimationImg[6], 3, 1, 3, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 8, animationLoc[0], 0.45f, false);
            animationGTK[27] = new Animation(kiaAnimationImg[7], 9, 1, 9, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 5, animationLoc[0], 1.1f, false);
            animationGTK[28] = new Animation(kiaAnimationImg[8], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, animationLoc[0], 0.35f, false);
            animationGTK[29] = new Animation(kiaAnimationImg[9], 1, 1, 1, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 10, animationLoc[0], 0.4f, false);

            //Loads the timer into the game
            countdownTimer = new Timer(4000, false);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Gets the state of the game pad, and sets the previous state to what it is before it gets the state
            gamePadPrev[0] = gamePad[0];
            gamePad[0] = GamePad.GetState(PlayerIndex.One);

            gamePadPrev[1] = gamePad[1];
            gamePad[1] = GamePad.GetState(PlayerIndex.Two);

            switch (gameState)
            {
                case MENU:
                    //Get and implement menu interactions, e.g. when the user clicks a Play button set gameState = GAMEPLAY

                    //If the player 1's left thumbsticks y value is less than 0 or they press down and it is not at the lowest point right now, then move the line one space down, if not then check if they move the line up, 
                    if ((gamePad[0].ThumbSticks.Left.Y <= thumbLoc2.Y || gamePad[0].DPad.Down == ButtonState.Pressed) && linePos < 3 && gamePadPrev[0] != gamePad[0])
                    {
                        linePos++;
                        underLineLoc.Y += lineMoveOneSpace;
                    }
                    else if ((gamePad[0].ThumbSticks.Left.Y >= thumbLoc1.Y || gamePad[0].DPad.Up == ButtonState.Pressed) && linePos > 1 && gamePadPrev[0] != gamePad[0])
                    {
                        linePos--;
                        underLineLoc.Y -= lineMoveOneSpace;
                    }

                    //If the player presses A then check where the line is and do that action
                    if (gamePad[0].Buttons.A == ButtonState.Pressed)
                    {
                        //If the line is in position 3 end the game, if not, check if its in pos 2 - if so go to instructions, if not then go to character
                        if (linePos == 3)
                        {
                            //Ends the code
                            System.Environment.Exit(0);
                        }
                        else if (linePos == 2)
                        {
                            //Makes the gamestate go to instructions
                            gameState = INSTRUCTIONS;
                        }
                        else
                        {
                            //Makes the gamestate character
                            gameState = CHARACTER;
                        }
                    }
                    break;
                case CHARACTER:
                    //Get and apply changes to game character selection screen

                    //Changes the select text to the information when selecting the characters
                    selectTxt = "         SELECT YOUR CHARACTERS\n\n\nPlayer 1: " + characterSelected[0] + "\nPlayer 2: " + characterSelected[1] + "\n\n\n\n\n\n\n\n\n   " + characters[0] + "                 " + characters[1] + "                  " + characters[2];

                    //If both players haven't selected their player and player 1 presses b then go back to menu
                    if (selection1 == true && selection2 == true && gamePad[0].Buttons.B == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                    {
                        //Changes the gamestate to menu
                        gameState = MENU;
                    }

                    //If player one has not selected their character yet then if they move the thumbstick left then move the selection box left, if they move it right move it right
                    if (selection1 == true)
                    {
                        //If the player moves his thumbstick right or the d pad then move the box right
                        if ((gamePad[0].ThumbSticks.Left.X >= thumbLoc1.X || gamePad[0].DPad.Right == ButtonState.Pressed) && gamePadPrev[0] != gamePad[0] && box1Pos != 3)
                        {
                            //increases the box's position and the position of the first box
                            box1Pos++;
                            characterBoxes1Rec.X += 250;
                        }
                        else if ((gamePad[0].ThumbSticks.Left.X <= thumbLoc2.X || gamePad[0].DPad.Left == ButtonState.Pressed) && gamePadPrev[0] != gamePad[0] && box1Pos != 1)
                        {
                            //decreases the box's position and the position of the first box
                            box1Pos--;
                            characterBoxes1Rec.X -= 250;
                        }

                        //If player 1 presses a then make their selected character dependent on who they chose, if they press b make selection 1 true and make their character selected become null
                        if (gamePad[0].Buttons.A == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                        {
                            //Change selection to false
                            selection1 = false;

                            //If the two player boxes positions arent equal to one another or player 2 hasnt selected his character yet then make the chaaracter for player one selected the corresponding position. If not then make selecction for player 1 true again
                            if (box1Pos != box2Pos || selection2 == true)
                            {
                                //If the boxes position is in pos 1 then they chose guile, if not then if its in pos 2 then they chose thor, if not then they chose kia, else make selection 1 true again
                                if (box1Pos == 1)
                                {
                                    //Makes the character selected be guile
                                    characterSelected[0] = characters[0];
                                }
                                else if (box1Pos == 2)
                                {
                                    //Makes the character selected be thor
                                    characterSelected[0] = characters[1];
                                }
                                else if (box1Pos == 3)
                                {
                                    //Makes the character selected be kia
                                    characterSelected[0] = characters[2];
                                }
                            }
                            else
                            {
                                //Makes selection 1 true again
                                selection1 = true;
                            }
                        }

                    }
                    else if (gamePad[0].Buttons.B == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                    {
                        //Makes selection 1 true and nulls character selected
                        selection1 = true;
                        characterSelected[0] = "";
                    }

                    //If player two has not selected their character yet then if they move the thumbstick left then move the selection box left, if they move it right move it right
                    if (selection2 == true)
                    {
                        //If the player moves his thumbstick right or the d pad then move the box right
                        if ((gamePad[1].ThumbSticks.Left.X >= thumbLoc1.X || gamePad[1].DPad.Right == ButtonState.Pressed) && gamePadPrev[1] != gamePad[1] && box2Pos != 3)
                        {
                            //increases the box's position and the position of the first box
                            box2Pos++;
                            characterBoxes2Rec.X += 250;
                        }
                        else if ((gamePad[1].ThumbSticks.Left.X <= thumbLoc2.X || gamePad[1].DPad.Left == ButtonState.Pressed) && gamePadPrev[1] != gamePad[1] && box2Pos != 1)
                        {
                            //decreases the box's position and the position of the first box
                            box2Pos--;
                            characterBoxes2Rec.X -= 250;
                        }

                        //If player 2 presses a then make their selected character dependent on who they chose, if they press b make selection 2 true and make their character selected become null
                        if (gamePad[1].Buttons.A == ButtonState.Pressed && gamePadPrev[1] != gamePad[1])
                        {
                            //Makes selection 2 false
                            selection2 = false;

                            //If the boxes position is in pos 1 then they chose guile, if not then if its in pos 2 then they chose thor, if not then they chose kia, else make selection 2 true again
                            if (box1Pos != box2Pos || selection1 == true)
                            {
                                //If the boxes position is in pos 1 then they chose guile, if not then if its in pos 2 then they chose thor, if not then they chose kia, else make selection 2 true again
                                if (box2Pos == 1)
                                {
                                    //Makes them choose guile
                                    characterSelected[1] = characters[0];
                                }
                                else if (box2Pos == 2)
                                {
                                    //Makes them choose thor
                                    characterSelected[1] = characters[1];
                                }
                                else if (box2Pos == 3)
                                {
                                    //Makes them choose kia
                                    characterSelected[1] = characters[2];
                                }
                            }
                            else
                            {
                                //Makes selection 2 true again
                                selection2 = true;
                            }

                        }

                    }
                    else if (gamePad[1].Buttons.B == ButtonState.Pressed && gamePadPrev[1] != gamePad[1])
                    {
                        //Makes selection 2 true and nulls their character selected option
                        selection2 = true;
                        characterSelected[1] = "";
                    }

                    //If both players have selected their characters then show the option to press x to continue
                    if (selection1 == false && selection2 == false)
                    {
                        //Makes them know that if they both press x they can continue
                        selectTxt += "\n             Both hold X to continue";

                        //If the both press x then make the gamestate map
                        if (gamePad[0].Buttons.X == ButtonState.Pressed && gamePad[1].Buttons.X == ButtonState.Pressed)
                        {
                            //Makes the gamestate map and selection 1 becomes true again
                            gameState = MAP;
                            selection1 = true;
                        }
                    }
                    break;
                case INSTRUCTIONS:
                    //Get user input to return to MENU
                    if (gamePad[0].Buttons.B == ButtonState.Pressed)
                    {
                        //Sets the gamestate to menu
                        gameState = MENU;
                    }
                    break;
                case GAMEPLAY:
                    //Implement standared game logic (input, update game objects, apply physics, collision detection)

                    //If the starting setup is true call the starting setup subprogram
                    if (startingSetup == true)
                    {
                        //Calls the startupp subprogram
                        Startup();
                    }

                    //If the countdown timer gets to 3 seconds then begin the fighting, if not then do the idle standing animation
                    if (countdownTimer.GetTimePassed() >= 3000)
                    {
                        //This loop runs twice, the first time it checks input for the first player, the second time it checks input for the second player
                        for (int i = 0; i <= 1; i++)
                        {
                            //If the players thumbstick goes left and the move bool is false or jump move and the animations location isn't on the right edge of the screen move the player right. If not then if they are moving right make the dir -1 and do the same
                            if (gamePad[i].ThumbSticks.Left.X > movingLoc.X && (moves[i] == false || jumpMoves[i] == true) && animationGTK[player[i]].destRec.Right <= screenWidth)
                            {
                                //makes the direction positive and the max speed becomes 5
                                dir[i] = 1;
                                maxSpeed[i] = 5;

                                //Calls the walking subprogram and gives it the proper info
                                Walking(player[i], gamePad[i], jumpMoves[i], i);
                            }
                            else if (gamePad[i].ThumbSticks.Left.X < movingLoc.X && (moves[i] == false || jumpMoves[i] == true) && animationGTK[player[i]].destRec.Left >= 0)
                            {
                                //makes the direction negative and the max speed becomes 5
                                dir[i] = -1;
                                maxSpeed[i] = 5;

                                //Calls the walking subprogram and gives it the proper info
                                Walking(player[i], gamePad[i], jumpMoves[i], i);
                            }

                            //If the player is inbetween the screen then check to see which moves they want to do
                            if (animationGTK[player[i]].destRec.Left >= 0 && animationGTK[player[i]].destRec.Right <= screenWidth)
                            {
                                //If the player presses x and no current move is in progress then make moves equal true and call the light attack subprogram and make damage correspond, if not, it checks which button they press and does a move depending on which they pressed and calls that subprogram
                                if (gamePad[i].Buttons.X == ButtonState.Pressed && gamePadPrev[i] != gamePad[i] && moves[i] == false)
                                {
                                    //Sets moves to true and calls the light attack subprogram and makes damage 10
                                    moves[i] = true;
                                    LightAttack(player[i]);
                                    damage[i] = 10;
                                }
                                else if (gamePad[i].Buttons.B == ButtonState.Pressed && gamePadPrev[i] != gamePad[i] && moves[i] == false)
                                {
                                    //Sets moves to true and calls the heavy attack subprogram and makes damage 15
                                    moves[i] = true;
                                    HeavyAttack(player[i]);
                                    damage[i] = 15;
                                }
                                else if (gamePad[i].Buttons.A == ButtonState.Pressed && gamePadPrev[i] != gamePad[i] && moves[i] == false && health[i] > 40)
                                {
                                    //Sets moves to true and calls the jump subprogram and makes jump moves true
                                    moves[i] = true;
                                    jumpMoves[i] = true;
                                    Jump(player[i]);
                                }
                                else if (animationGTK[player[i] + 4].isAnimating == true && jumpMoves[i] == true)
                                {
                                    //If they press either trigger and the enemy health is above 40 do the jump kick attack, this only happens if theyve already jumped
                                    if ((gamePad[i].Triggers.Left > movingLoc.X || gamePad[i].Triggers.Right > movingLoc.X) && gamePadPrev[i] != gamePad[i] && health[(i + 1) % 2] > 40)
                                    {
                                        //Calls jump kick punch and makes damage 20
                                        JumpKickPunch(player[i]);
                                        damage[i] = 20;
                                    }
                                }
                                else if ((gamePad[i].Buttons.RightShoulder == ButtonState.Pressed || gamePad[i].Buttons.LeftShoulder == ButtonState.Pressed) && moves[i] == false)
                                {
                                    //Sets moves to true andcalls the block subprogram and makes block damage true
                                    moves[i] = true;
                                    Block(player[i]);
                                    blockDamage[i] = true;
                                }
                                else if (gamePad[i].Buttons.Y == ButtonState.Pressed && gamePadPrev[i] != gamePad[i] && moves[i] == false)
                                {
                                    //Makes moves true and calls the kick subprogram and makes damage 18
                                    moves[i] = true;
                                    Kick(player[i]);
                                    damage[i] = 18;
                                }
                            }

                            //If they are not pressing on either shoulder button then stop the block animation
                            if (gamePad[i].Buttons.RightShoulder != ButtonState.Pressed && gamePad[i].Buttons.LeftShoulder != ButtonState.Pressed)
                            {
                                //Stops the block animation and sets block damage to false
                                animationGTK[player[i] + 5].isAnimating = false;
                                blockDamage[i] = false;
                            }

                            //If they jumped and both jump and punch animations equal false, set the speed to 5 and make all the rec move back down
                            if (jumpMoves[i] == true && animationGTK[player[i] + 4].isAnimating == false && animationGTK[player[i] + 7].isAnimating == false)
                            {
                                //Sets the speed to 5
                                maxSpeed[i] = 5;

                                //Sets all the recs position to move down back to their original y
                                for (int j = player[i]; j <= player[i] + 9; j++)
                                {
                                    animationGTK[j].destRec.Y += 100;
                                }
                            }

                            //If no animations are currently animating then set moves to false and jump moves to false
                            if (animationGTK[player[i] + 2].isAnimating == false && animationGTK[player[i] + 3].isAnimating == false && animationGTK[player[i] + 4].isAnimating == false && animationGTK[player[i] + 5].isAnimating == false && animationGTK[player[i] + 6].isAnimating == false && animationGTK[player[i] + 7].isAnimating == false)
                            {
                                //Sets moves and jump moves to false
                                moves[i] = false;
                                jumpMoves[i] = false;
                            }

                            //If they are just standing there and no moves are currently active then make them do the idle animation
                            if (gamePad[i].ThumbSticks.Left.X == movingLoc.X && moves[i] == false)
                            {
                                //Makes the idle animation true and the walking animation false
                                animationGTK[player[i] + 1].isAnimating = true;
                                animationGTK[player[i]].isAnimating = false;
                            }
                        }
                    }
                    else
                    {
                        //Sets the idle animations to true
                        for (int i = 0; i <= 1; i++)
                        {
                            animationGTK[player[i] + 1].isAnimating = true;
                        }
                    }

                    //if player one is blocking set the other players damage to itself divided by 5
                    if (blockDamage[0] == true)
                    {
                        //Divides the damage by 5
                        damage[1] = damage[1] / 5;
                    }

                    //If the player two is blocking set the other players damage to itself divided by 5
                    if (blockDamage[1] == true)
                    {
                        //Divides the damage by 5
                        damage[0] = damage[0] / 5;
                    }

                    //If the two animations are intersecting then make them lose health by however much damage value is, sometimes could be 0 if theyre not attacking
                    if (animationGTK[player[0]].destRec.Intersects(animationGTK[player[1]].destRec))
                    {
                        //Takes away the health of both players by the enemys damage 
                        health[1] -= damage[0];
                        health[0] -= damage[1];
                    }

                    //If player 1 presses the start button then they will pause the game, they cant do this in the air after theyve jumped, its kinda like cheating \_0_/
                    if (gamePad[0].Buttons.Start == ButtonState.Pressed && animationGTK[player[0] + 4].isAnimating == false && animationGTK[player[1] + 4].isAnimating == false)
                    {
                        //Sets the gamestate to pause
                        gameState = PAUSE;
                    }

                    //Makes the damage of both players 0
                    damage[0] = 0;
                    damage[1] = 0;

                    //Shows the players chossen and will have their health inbetween
                    healthPlayersTxt = "                  " + characterSelected[0] + "                                                            " + characterSelected[1];

                    //If either players health reaches 0 then call the win lose animation depending on which player is dead and set all the animations position back to start and change the gamestate to end
                    if (health[0] <= 0 || health[1] <= 0)
                    {
                        //Repositions the players positions to location 0
                        for (int i = 0; i <= 29; i++)
                        {
                            //Sets x and y values to location 0
                            animationGTK[i].destRec.X = (int)animationLoc[0].X;
                            animationGTK[i].destRec.Y = (int)animationLoc[0].Y;
                        }

                        //If player one is dead make player two the first variable in the brackets as the "winner" and player one as the second as the "loser", if not then do the opposite
                        if (health[0] <= 0)
                        {
                            //Calls the win lose animation subprogram
                            WinLoseAni(player[1], player[0]);
                        }
                        else if (health[1] <= 0)
                        {
                            //Calls the win lose animation subprogram
                            WinLoseAni(player[0], player[1]);
                        }

                        //Sets the gamestate to endgame
                        gameState = ENDGAME;
                    }

                    //A loop to update all the animations in the game
                    for (int i = 0; i <= 29; i++)
                    {
                        animationGTK[i].Update(gameTime);
                    }

                    //Updates the countdown time
                    countdownTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    break;
                case PAUSE:
                    //Get user input to resume the game

                    //If player 1 presses b then go back to the gameplay gamestate
                    if (gamePad[0].Buttons.B == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                    {
                        //goes back to gameplay
                        gameState = GAMEPLAY;
                    }

                    //If player one presses start then go back to the menu and reset all variables
                    if (gamePad[0].Buttons.Start == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                    {
                        //Sets all the animations to stop animating
                        for (int i = 0; i <= 29; i++)
                        {
                            //Stops the animation from animating
                            animationGTK[i].isAnimating = false;
                        }

                        //Repositions the players positions to location 0
                        for (int i = 0; i <= 29; i++)
                        {
                            //Sets x and y values to location 0
                            animationGTK[i].destRec.X = (int)animationLoc[0].X;
                            animationGTK[i].destRec.Y = (int)animationLoc[0].Y;
                        }

                        //Sets the menu text to the title of the game
                        menuTxt = "Fight Time!";

                        //Resets the variables back to their original state: characters selected, map selected, health, box position, box locations, directions, etc.
                        characterSelected[0] = "";
                        characterSelected[1] = "";
                        mapSelected = "";
                        selection1 = true;
                        selection2 = true;
                        health[0] = 200;
                        health[1] = 200;
                        startingSetup = true;
                        box1Pos = 1;
                        box2Pos = 2;
                        characterBoxes1Rec.X = 150;
                        characterBoxes2Rec.X = 400;
                        dir[0] = 1;
                        dir[1] = -1;
                        gameState = MENU;
                    }
                    break;
                case ENDGAME:
                    //Wait for final input based on end of game options (end, restart, etc.)

                    //If player one presses the start button then end animation and reset all variables
                    if (gamePad[0].Buttons.Start == ButtonState.Pressed)
                    {
                        //Sets all the animations to stop animating
                        for (int i = 0; i <= 29; i++)
                        {
                            //makes animations stop animating
                            animationGTK[i].isAnimating = false;
                        }

                        //Resets the menu text to the title of the game
                        menuTxt = "Fight Time!";

                        //Resets the variables back to their original state: characters selected, map selected, health, box position, box locations, directions, etc.
                        characterSelected[0] = "";
                        characterSelected[1] = "";
                        mapSelected = "";
                        selection1 = true;
                        selection2 = true;
                        health[0] = 200;
                        health[1] = 200;
                        startingSetup = true;
                        box1Pos = 1;
                        box2Pos = 2;
                        characterBoxes1Rec.X = 150;
                        characterBoxes2Rec.X = 400;
                        dir[0] = 1;
                        dir[1] = -1;
                        gameState = MENU;
                    }
                    break;
                case MAP:

                    //Makes the select text the info about the maps available
                    selectTxt = "      SELECT YOUR MAP\n\nMAP: " + mapSelected + "\n\n\n\n\n\n\n\n\n\n\n " + maps[0] + "            " + maps[1] + "             " + maps[2];

                    //If player one hasn't chosen the map and they press be then go back to the character screen
                    if (selection1 == true && gamePad[0].Buttons.B == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                    {
                        //Goes back to the character gamestate and resets both players selection choice
                        gameState = CHARACTER;
                        characterSelected[0] = "";
                        characterSelected[1] = "";
                        selection1 = true;
                        selection2 = true;
                    }

                    //If player one has not selected the map then depending on where they move the thumbsstick move the selectore there and if they press a select that map as the map chosen, if not true then if they press b unselect the chosen map
                    if (selection1 == true)
                    {
                        //If the player moves the thumbstick or d pad right then move the sleection box right, if they go left move the selection box left
                        if ((gamePad[0].ThumbSticks.Left.X >= thumbLoc1.X || gamePad[0].DPad.Right == ButtonState.Pressed) && gamePadPrev[0] != gamePad[0] && box1Pos != 3)
                        {
                            //Moves the box one position right
                            box1Pos++;
                            characterBoxes1Rec.X += 250;
                        }
                        else if ((gamePad[0].ThumbSticks.Left.X <= thumbLoc2.X || gamePad[0].DPad.Left == ButtonState.Pressed) && gamePadPrev[0] != gamePad[0] && box1Pos != 1)
                        {
                            //Moves the box one position left
                            box1Pos--;
                            characterBoxes1Rec.X -= 250;
                        }

                        //If the player presses A then whichever map theyre on make that the selected map
                        if (gamePad[0].Buttons.A == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                        {
                            //Makes selection 1 false
                            selection1 = false;

                            //If the position of the box is 1 then they choose map 1, if 2 then map 2, if 3 then 3
                            if (box1Pos == 1)
                            {
                                //Makes the map selected map 1
                                mapSelected = maps[0];
                            }
                            else if (box1Pos == 2)
                            {
                                //Makes the map selected map 2
                                mapSelected = maps[1];
                            }
                            else if (box1Pos == 3)
                            {
                                //Makes the map selected map 3
                                mapSelected = maps[2];
                            }
                        }
                    }
                    else if (gamePad[0].Buttons.B == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                    {
                        //Makes selection 1 true and resets the map selected
                        selection1 = true;
                        mapSelected = "";
                    }

                    //If selection 1 is false then it says press x to ostart and if they press x make the gamestate gameplay
                    if (selection1 == false)
                    {
                        //Tells the user to press x to start
                        selectTxt += "\n             PRESS X TO START";

                        //If the user presses x then make the game state gameplay
                        if (gamePad[0].Buttons.X == ButtonState.Pressed && gamePadPrev[0] != gamePad[0])
                        {
                            //Changes gamestate to gameplay
                            gameState = GAMEPLAY;
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (gameState)
            {
                case MENU:
                    //Draw the possible menu options

                    //Draws the menu img, the font, the underline, the play exit and settings buttons
                    _spriteBatch.Draw(menuImg, menuRec, Color.White);
                    _spriteBatch.DrawString(menuFont, menuTxt, menuFontLoc, Color.Magenta);
                    _spriteBatch.DrawString(underLine, underLineTxt, underLineLoc, Color.White);
                    _spriteBatch.Draw(playButton, playButtonRec, Color.White);
                    _spriteBatch.Draw(settingsButton, settingsButtonRec, Color.Brown);
                    _spriteBatch.Draw(exitButton, exitButtonRec, Color.White);
                    break;
                case CHARACTER:
                    //Draw the character screen with prompts

                    //Draws the background, the character select txt, the 3 player images, and the 2 character boxes
                    _spriteBatch.Draw(characterBackground, characterBackgroundRec, Color.White);
                    _spriteBatch.DrawString(characterSelect, selectTxt, characterSelectLoc, Color.BlueViolet);
                    for (int n = 0; n <= 2; n++)
                    {
                        _spriteBatch.Draw(playerImages[n], playerImageRec[n], Color.White);
                    }
                    _spriteBatch.Draw(characterBoxes[0], characterBoxes1Rec, Color.Blue * 0.5f);
                    _spriteBatch.Draw(characterBoxes[1], characterBoxes2Rec, Color.LawnGreen * 0.5f);
                    break;
                case INSTRUCTIONS:
                    //Draw the game instructions including prompt to return to MENU

                    //Draws the instruction img, and the text to explain the instructions
                    _spriteBatch.Draw(menuImg, menuRec, Color.Pink);
                    _spriteBatch.DrawString(characterSelect, "                                                Controls\n\nA - Jump     B - Heavy Attack     X - Light Attack     Y - Kick\n                             L Stick - Move     R1/L1 - Block\n\n\n[Special Attack] Jump (A) + L2/R2 (only while enemy health is high)\n\nDont Forget - If health is too low you wont have enough stamina to jump\n\n\nPress B to go back to menu", characterSelectLoc, Color.Yellow);
                    break;
                case GAMEPLAY:
                    //Draw all game objects on each layers (background, middleground, foreground and user interface)

                    //If the map selected is map one draw map one, if map selected is map 2 draw map 2, if selected is 3 draw 3
                    if (mapSelected == maps[0])
                    {
                        //Draws map 1
                        _spriteBatch.Draw(backgroundImg[0], screenRec, Color.White);
                    }
                    else if (mapSelected == maps[1])
                    {
                        //Draws map 2
                        _spriteBatch.Draw(backgroundImg[1], screenRec, Color.White);
                    }
                    else
                    {
                        //Draws map 3
                        _spriteBatch.Draw(backgroundImg[2], screenRec, Color.White);
                    }

                    //Draws the health of the players text
                    _spriteBatch.DrawString(healthPlayers, healthPlayersTxt, characterSelectLoc, Color.Gold);

                    //Draws the health bar of both players,
                    for (int i = 0; i <= 1; i++)
                    {
                        //If health is 200 draw full health bar, if not then inbetween 170 and 200 health bar, if not so on keep checking
                        if (health[i] == 200)
                        {
                            //Draws the full health bar
                            _spriteBatch.Draw(healthBarImg[0], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 200 && health[i] >= 170)
                        {
                            //Draws a lesser health health bar
                            _spriteBatch.Draw(healthBarImg[1], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 170 && health[i] >= 140)
                        {
                            //Draws a lesser health health bar
                            _spriteBatch.Draw(healthBarImg[2], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 140 && health[i] >= 110)
                        {
                            //Draws a lesser health health bar
                            _spriteBatch.Draw(healthBarImg[3], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 110 && health[i] >= 80)
                        {
                            //Draws a lesser health health bar
                            _spriteBatch.Draw(healthBarImg[4], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 80 && health[i] >= 40)
                        {
                            //Draws a lesser health health bar
                            _spriteBatch.Draw(healthBarImg[5], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 40 && health[i] > 0)
                        {
                            //Draws a lesser health health bar
                            _spriteBatch.Draw(healthBarImg[6], healthBarRec[i], Color.White);
                        }
                    }

                    //Loop so it draws the characters for both player 1 and player 2
                    for (int j = 0; j <= 1; j++)
                    {
                        //If player 1 or 2 selected character 1 then load all his animations for each direction in 2 individual loops, if not then the same but for character 2, if not then the same but for character 3
                        if (characterSelected[j] == characters[0])
                        {
                            //If the direction is positive draw his animations facing right
                            if (dir[j] == 1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 0; i <= 9; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                                }

                            }

                            //If the direction is negative draw his animations facing left
                            if (dir[j] == -1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 0; i <= 9; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                                }
                            }
                        }
                        else if (characterSelected[j] == characters[1])
                        {
                            //If the direction is positive draw his animations facing right
                            if (dir[j] == 1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 10; i <= 19; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                                }
                            }

                            if (dir[j] == -1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 10; i <= 19; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                                }
                            }
                        }
                        else if (characterSelected[j] == characters[2])
                        {
                            //If the direction is positive draw his animations facing right
                            if (dir[j] == 1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 20; i <= 29; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                                }
                            }

                            if (dir[j] == -1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 20; i <= 29; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                                }
                            }
                        }
                    }

                    //Draws the time remaining in the timer dependent on how much it has counter
                    if (countdownTimer.GetTimePassed() < 1000)
                    {
                        //Moves the timers location to the middle of the screen
                        timerLoc.X = (1400 / 2) - 30;

                        //Outputs the number 3 on the screen
                        _spriteBatch.DrawString(timerCount, "3", timerLoc, Color.Gold);
                    }
                    else if (countdownTimer.GetTimePassed() > 1000 && countdownTimer.GetTimePassed() < 2000)
                    {
                        //Outputs 2 on the screen
                        _spriteBatch.DrawString(timerCount, "2", timerLoc, Color.Gold);
                    }
                    else if (countdownTimer.GetTimePassed() > 2000 && countdownTimer.GetTimePassed() < 3000)
                    {
                        //Outputs 1 on the screen
                        _spriteBatch.DrawString(timerCount, "1", timerLoc, Color.Gold);
                    }
                    else if (countdownTimer.GetTimePassed() > 3000 && countdownTimer.GetTimePassed() < 3800)
                    {
                        //Moves the timer a bit left
                        timerLoc.X = (1400 / 2) - 175;

                        //Outputs fight on the screen
                        _spriteBatch.DrawString(timerCount, "Fight!", timerLoc, Color.Gold);
                    }


                    break;
                case PAUSE:
                    //Draw the pause screen, this may include the full game drawing behind
                    
                    if (mapSelected == maps[0])
                    {
                        _spriteBatch.Draw(backgroundImg[0], screenRec, Color.White);
                    }
                    else if (mapSelected == maps[1])
                    {
                        _spriteBatch.Draw(backgroundImg[1], screenRec, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(backgroundImg[2], screenRec, Color.White);
                    }

                    _spriteBatch.DrawString(healthPlayers, healthPlayersTxt, characterSelectLoc, Color.Gold);
                    for (int i = 0; i <= 1; i++)
                    {
                        if (health[i] == 200)
                        {
                            _spriteBatch.Draw(healthBarImg[0], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 200 && health[i] >= 170)
                        {
                            _spriteBatch.Draw(healthBarImg[1], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 170 && health[i] >= 140)
                        {
                            _spriteBatch.Draw(healthBarImg[2], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 140 && health[i] >= 110)
                        {
                            _spriteBatch.Draw(healthBarImg[3], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 110 && health[i] >= 80)
                        {
                            _spriteBatch.Draw(healthBarImg[4], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 80 && health[i] >= 40)
                        {
                            _spriteBatch.Draw(healthBarImg[5], healthBarRec[i], Color.White);
                        }
                        else if (health[i] < 40 && health[i] > 0)
                        {
                            _spriteBatch.Draw(healthBarImg[6], healthBarRec[i], Color.White);
                        }
                    }

                    //Loop so it draws the characters for both player 1 and player 2
                    for (int j = 0; j <= 1; j++)
                    {
                        //If player 1 or 2 selected character 1 then load all his animations for each direction in 2 individual loops, if not then the same but for character 2, if not then the same but for character 3
                        if (characterSelected[j] == characters[0])
                        {
                            //If the direction is positive draw his animations facing right
                            if (dir[j] == 1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 0; i <= 9; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                                }

                            }

                            //If the direction is negative draw his animations facing left
                            if (dir[j] == -1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 0; i <= 9; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                                }
                            }
                        }
                        else if (characterSelected[j] == characters[1])
                        {
                            //If the direction is positive draw his animations facing right
                            if (dir[j] == 1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 10; i <= 19; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                                }
                            }

                            if (dir[j] == -1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 10; i <= 19; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                                }
                            }
                        }
                        else if (characterSelected[j] == characters[2])
                        {
                            //If the direction is positive draw his animations facing right
                            if (dir[j] == 1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 20; i <= 29; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                                }
                            }

                            if (dir[j] == -1)
                            {
                                //Loop for all 10 animations to draw
                                for (int i = 20; i <= 29; i++)
                                {
                                    //Draws all 10 animations
                                    animationGTK[i].Draw(_spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                                }
                            }
                        }
                    }

                    //Draws the instructions when you are paused
                    _spriteBatch.DrawString(characterSelect, "\n\n\nPAUSED\n\n\n(B) button to go back to game\n\n(Start) button to go back to main menu", characterSelectLoc, Color.PapayaWhip);

                    //Draws the time remaining in the timer dependent on how much it has counter
                    if (countdownTimer.GetTimePassed() < 1000)
                    {
                        //Moves the timers location to the middle of the screen
                        timerLoc.X = (1400 / 2) - 30;

                        //Outputs the number 3 on the screen
                        _spriteBatch.DrawString(timerCount, "3", timerLoc, Color.Gold);
                    }
                    else if (countdownTimer.GetTimePassed() > 1000 && countdownTimer.GetTimePassed() < 2000)
                    {
                        //Outputs 2 on the screen
                        _spriteBatch.DrawString(timerCount, "2", timerLoc, Color.Gold);
                    }
                    else if (countdownTimer.GetTimePassed() > 2000 && countdownTimer.GetTimePassed() < 3000)
                    {
                        //Outputs 1 on the screen
                        _spriteBatch.DrawString(timerCount, "1", timerLoc, Color.Gold);
                    }
                    else if (countdownTimer.GetTimePassed() > 3000 && countdownTimer.GetTimePassed() < 3800)
                    {
                        //Moves the timer a bit left
                        timerLoc.X = (1400 / 2) - 175;

                        //Outputs fight on the screen
                        _spriteBatch.DrawString(timerCount, "Fight!", timerLoc, Color.Gold);
                    }
                    break;
                case ENDGAME:
                    //Draw the final feedback and prompt for available options (exit,restart, etc.)

                    //If the map selected is map 1 draw the first map, if 2 then draw 2, if neither than draw map 3
                    if (mapSelected == maps[0])
                    {
                        //Draws map 1
                        _spriteBatch.Draw(backgroundImg[0], screenRec, Color.White);
                    }
                    else if (mapSelected == maps[1])
                    {
                        //Draws map 2
                        _spriteBatch.Draw(backgroundImg[1], screenRec, Color.White);
                    }
                    else
                    {
                        //Draws map 3
                        _spriteBatch.Draw(backgroundImg[2], screenRec, Color.White);
                    }

                    //Draws the menu font text
                    _spriteBatch.DrawString(menuFont, menuTxt, menuFontLoc, Color.Gold);

                    break;
                case MAP:

                    //Draws the background for the three maps loacted in the map screen
                    _spriteBatch.Draw(characterBackground, characterBackgroundRec, Color.White);
                    for (int i = 0; i <= 2; i++)
                    {
                        //Draws each map excpet an aesthetic mini version
                        _spriteBatch.Draw(backgroundImg[i], mapRec[i], Color.White);
                    }

                    //Draws the text for their selection and the selection box so they know which map they're on
                    _spriteBatch.DrawString(characterSelect, selectTxt, characterSelectLoc, Color.BlueViolet);
                    _spriteBatch.Draw(characterBoxes[0], characterBoxes1Rec, Color.Blue * 0.5f);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: None
        //Post: None
        //Desc: A startup subprogram which resets and activates the timer, sets the exact positions of both players, and makes starting setup false
        private void Startup()
        {
            //Resets the timer and activates it
            countdownTimer.ResetTimer(true);
            countdownTimer.Activate();


            //Makes specific precise movement for player one and player twos characters selected to better the look of it all
            if (characterSelected[1] == characters[0])
            {
                //Makes player [1] variable = 0
                player[1] = 0;

                //Sets the destination for the characters x and y axis to position 2 and adjusts it a little bit
                animationGTK[0].destRec.X = (int)playerTwoLoc.X;
                animationGTK[0].destRec.Y += 15;
                animationGTK[1].destRec.X = (int)playerTwoLoc.X;
                animationGTK[1].destRec.Y += 15;
                animationGTK[2].destRec.X = (int)playerTwoLoc.X - 5;
                animationGTK[2].destRec.Y += 10;
                animationGTK[3].destRec.X = (int)playerTwoLoc.X;
                animationGTK[3].destRec.Y += 10;
                animationGTK[4].destRec.X = (int)playerTwoLoc.X;
                animationGTK[5].destRec.X = (int)playerTwoLoc.X - 85;
                animationGTK[5].destRec.Y += 13;
                animationGTK[6].destRec.X = (int)playerTwoLoc.X;
                animationGTK[7].destRec.X = (int)playerTwoLoc.X;
                animationGTK[8].destRec.X = (int)playerTwoLoc.X;
                animationGTK[9].destRec.X = (int)playerTwoLoc.X;
            }
            else if (characterSelected[1] == characters[1])
            {
                //Makes player [1] variable = 10
                player[1] = 10;

                //Sets the destination for the characters x and y axis to position 2 and adjusts it a little bit
                animationGTK[10].destRec.X = (int)playerTwoLoc.X;
                animationGTK[11].destRec.X = (int)playerTwoLoc.X;
                animationGTK[12].destRec.X = (int)playerTwoLoc.X - 5;
                animationGTK[13].destRec.X = (int)playerTwoLoc.X;
                animationGTK[13].destRec.Y -= 15;
                animationGTK[14].destRec.X = (int)playerTwoLoc.X;
                animationGTK[14].destRec.Y -= 7;
                animationGTK[15].destRec.X = (int)playerTwoLoc.X - 70;
                animationGTK[15].destRec.Y += 15;
                animationGTK[16].destRec.X = (int)playerTwoLoc.X;
                animationGTK[17].destRec.X = (int)playerTwoLoc.X;
                animationGTK[18].destRec.X = (int)playerTwoLoc.X;
                animationGTK[19].destRec.X = (int)playerTwoLoc.X;
            }
            else
            {
                //Makes player [1] variable = 20
                player[1] = 20;

                //Sets the destination for the characters x and y axis to position 2 and adjusts it a little bit
                animationGTK[20].destRec.X = (int)playerTwoLoc.X;
                animationGTK[20].destRec.Y += 4;
                animationGTK[21].destRec.X = (int)playerTwoLoc.X;
                animationGTK[21].destRec.Y += 10;
                animationGTK[22].destRec.X = (int)playerTwoLoc.X - 5;
                animationGTK[22].destRec.Y += 10;
                animationGTK[23].destRec.X = (int)playerTwoLoc.X;
                animationGTK[23].destRec.Y += 11;
                animationGTK[24].destRec.X = (int)playerTwoLoc.X;
                animationGTK[24].destRec.Y += 6;
                animationGTK[25].destRec.X = (int)playerTwoLoc.X - 75;
                animationGTK[26].destRec.X = (int)playerTwoLoc.X;
                animationGTK[27].destRec.X = (int)playerTwoLoc.X;
                animationGTK[28].destRec.X = (int)playerTwoLoc.X;
                animationGTK[29].destRec.X = (int)playerTwoLoc.X;
            }

            if (characterSelected[0] == characters[0])
            {
                //Makes player [0] variable = 0
                //Adjusts the destination for the characters x and y axis a little bit
                player[0] = 0;
                animationGTK[0].destRec.Y += 15;
                animationGTK[1].destRec.Y += 15;
                animationGTK[2].destRec.Y += 10;
                animationGTK[3].destRec.Y += 10;
                animationGTK[5].destRec.X -= 85;
                animationGTK[5].destRec.Y += 13;
            }
            else if (characterSelected[0] == characters[1])
            {
                //Makes player [0] variable = 10
                //Adjusts the destination for the characters x and y axis a little bit
                player[0] = 10;
                animationGTK[13].destRec.Y -= 15;
                animationGTK[14].destRec.Y -= 7;
                animationGTK[15].destRec.X -= 70;
                animationGTK[15].destRec.Y += 15;
            }
            else
            {
                //Makes player [0] variable = 20
                //Adjusts the destination for the characters x and y axis a little bit
                player[0] = 20;
                animationGTK[20].destRec.Y += 4;
                animationGTK[21].destRec.Y += 10;
                animationGTK[22].destRec.Y += 10;
                animationGTK[23].destRec.Y += 11;
                animationGTK[24].destRec.Y += 6;
                animationGTK[25].destRec.X -= 75;
            }

            //Starting setup becomes false
            startingSetup = false;
        }

        //Pre: The player whos currently in turn
        //Post: None
        //Desc: Activates the light attack animation and makes movement and idle false
        private void LightAttack(int player)
        {
            //Activates the light attack animation and makes movement and idle false
            animationGTK[player].isAnimating = false;
            animationGTK[player + 1].isAnimating = false;
            animationGTK[player + 2].isAnimating = true;
        }

        //Pre: The player whos currently in turn
        //Post: None
        //Desc: Activates the heavy attack animation and makes movement and idle false
        private void HeavyAttack(int player)
        {
            //Activates the heavy attack animation and makes movement and idle false
            animationGTK[player].isAnimating = false;
            animationGTK[player + 1].isAnimating = false;
            animationGTK[player + 3].isAnimating = true;
        }

        //Pre: The player whos currently in turn
        //Post: None
        //Desc: Activates the jump animation and makes movement and idle false and moves the animation up by a little bit for a higher jump
        private void Jump(int player)
        {
            //Activates the jump animation and makes movement and idle false
            animationGTK[player].isAnimating = false;
            animationGTK[player + 1].isAnimating = false;
            animationGTK[player + 4].isAnimating = true;

            //Moves the animation up by a little bit for a higher jump
            for (int i = player; i <= player + 9; i++)
            {
                animationGTK[i].destRec.Y -= 100;
            }
        }

        //Pre: The player whos currently in turn
        //Post: None
        //Desc: Activates the block animation and makes movement and idle false
        private void Block(int player)
        {
            //Activates the block animation and makes movement and idle false
            animationGTK[player].isAnimating = false;
            animationGTK[player + 1].isAnimating = false;
            animationGTK[player + 5].isAnimating = true;
        }

        //Pre: The player whos currently in turn
        //Post: None
        //Desc: Activates the kick animation and makes movement and idle false
        private void Kick(int player)
        {
            //Activates the kick animation and makes movement and idle false
            animationGTK[player].isAnimating = false;
            animationGTK[player + 1].isAnimating = false;
            animationGTK[player + 6].isAnimating = true;
        }

        //Pre: The player whos currently in turn
        //Post: None
        //Desc: Activates the jumpKickPunch animation and makes the previous jump animation false
        private void JumpKickPunch(int player)
        {
            //Activates the jumpKickPunch animation and makes the previous jump animation false
            animationGTK[player + 4].isAnimating = false;
            animationGTK[player + 7].isAnimating = true;
        }

        //Pre: The player whos currently in turn, the x position of the thumbstick, the bool value of jump moves, and the turn of who it is 0 = player 1, 1 = player 2
        //Post: None
        //Desc: A subprogram to move the player whose tried to move, it will move their rectangle location by how much theyre moving the thumbstick
        private void Walking(int player, GamePadState gamePadSpeed, bool jumpMovesP, int n)
        {
            //If jump moves is false then make the walking animation true and idle animation false, else make both walking and idle not animate
            if (jumpMovesP == false)
            {
                //Walk animation starts animating while idle animation stops animating
                animationGTK[player].isAnimating = true;
                animationGTK[player + 1].isAnimating = false;
            }
            else
            {
                //Walk animation and idle animation stops animating
                animationGTK[player].isAnimating = false;
                animationGTK[player + 1].isAnimating = false;
            }

            //Loop to move all of the player selected animation rectangles by how much theyre moving the thumbstick
            for (int i = player; i <= player + 9; i++)
            {
                //Moves the position by the speed times the direction and x value of the thumbstick
                animationGTK[i].destRec.X += (int)(maxSpeed[n] * gamePadSpeed.ThumbSticks.Left.X);
            }
        }

        //Pre: The player whos won and the player whos lost
        //Post: None
        //Desc: Decides which player won and shows a winning text for them, and says for them to go back to menu, makes the winner do the winning animation and the loser do the losing animation
        private void WinLoseAni(int winner, int loser)
        {
            //If the winner is character 1 then say guile won and tell them how to go to menu, if character 2 then thor won, if 3 then kia won
            if (winner == 0)
            {
                //Changes menu text to the winner and how to go back to menu
                menuTxt = "              Guile Wins Congrats!\n\n          Player 1 Press Enter To Go\n                   Back To Menu";
            }
            else if (winner == 10)
            {
                //Changes menu text to the winner and how to go back to menu
                menuTxt =  "              Thor Wins Congrats!\n\n          Player 1 Press Enter To Go\n                   Back To Menu";
            }
            else
            {
                //Changes menu text to the winner and how to go back to menu
                menuTxt = "              Kia Wins Congrats!\n\n          Player 1 Press Enter To Go\n                   Back To Menu";
            }

            //Makes the winning characters winning animation true and the losing character losing animation true
            animationGTK[winner + 9].isAnimating = true;
            animationGTK[loser + 8].isAnimating = true;

            //Moves the losing animation to the right while the winning stays on the left
            animationGTK[loser + 8].destRec.X = 800;
        }
    }
}