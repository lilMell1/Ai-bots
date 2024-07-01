using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using SharpDX.Direct2D1.Effects; // Add this to use List

namespace RiceProject4
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Map map;
        private List<Tractor> tractors; // Use a list to hold multiple tractors
        private Map backgroundMap;
        private List<Rice> rices = new List<Rice>();
        private TargetManager targetManager = new TargetManager();
        private double elapsedTime = 0.0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 700;
            _graphics.PreferredBackBufferWidth = 1100;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Random random = new Random();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D mapTex = Content.Load<Texture2D>("regularImage");
            Texture2D backgroundMapTex = Content.Load<Texture2D>("bmpImage2");
            Texture2D tractorTexture = Content.Load<Texture2D>("tractor");


            map = new Map(mapTex);
            
            backgroundMap = new Map(backgroundMapTex);
            backgroundMap.InitializeNodes();
            // Initialize the tractors list
            tractors = new List<Tractor>();

           

            #region add tractors 
            //int numberOfTractors = 1;
            //for (int i = 0; i < numberOfTractors; i++)
            //{
            //    // Calculate positions so that tractors don't overlap
            //    Vector2 tractorPosition = new Vector2(150, 95); // Adjust the spacing as needed
            //    Tractor newTractor = new Tractor(
            //        tractorPosition,
            //        new Vector2(tractorTexture.Width / 2, tractorTexture.Height / 2),
            //        tractorTexture,
            //        20000,
            //        Vector2.One * 0.5f,
            //        backgroundMap,
            //        rices
            //        );

            //    tractors.Add(newTractor);
            //}
            #endregion

            Tractor newTractor1 = new Tractor(
                new Vector2(150, 95),
                new Vector2(tractorTexture.Width / 2, tractorTexture.Height / 2),
                tractorTexture,
                20000,
                Vector2.One * 0.5f,
                backgroundMap,
                rices
                );
            tractors.Add(newTractor1); 
           
            Tractor newTractor2 = new Tractor(
                new Vector2(986, 133),
                new Vector2(tractorTexture.Width / 2, tractorTexture.Height / 2),
                tractorTexture,
                20000,
                Vector2.One * 0.5f,
                backgroundMap,
                rices
                );
            tractors.Add(newTractor2);

            Tractor newTractor3 = new Tractor(
               new Vector2(487, 250),
               new Vector2(tractorTexture.Width / 2, tractorTexture.Height / 2),
               tractorTexture,
               20000,
               Vector2.One * 0.5f,
               backgroundMap,
               rices
               );
            tractors.Add(newTractor3);

            #region add rices
            Texture2D riceTexture = Content.Load<Texture2D>("rice");
            bool positionValid;
            for (int i = 0; i < 10; i++) // Generate 10 rice objects as an example
            {
                Vector2 ricePosition;
                do
                {
                    positionValid = true;
                    // Generate a random position within the bounds of the map
                    ricePosition = new Vector2(random.Next(0, map.width), random.Next(0, map.height));
                    //ricePosition *= new Vector2(map.Scale.X, map.Scale.Y); // Scale the position if necessary

                    // Check if the position is on a road and not too close to other rices
                    if (!backgroundMap.IsValidRoad(ricePosition))
                    {
                        positionValid = false;
                    }
                    else
                    {
                        foreach (var rice in rices)
                        {
                            if (Vector2.Distance(ricePosition, rice.Position) < 100) // Adjust radius as necessary
                            {
                                
                                positionValid = false;
                                break;
                            }
                        }
                    }
                } while (!positionValid); // Keep trying until a valid position is found

                rices.Add(new Rice(ricePosition, riceTexture,Vector2.One*0.7f));
            }
            #endregion
        }


        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var tractor in tractors)
            {
                tractor.UpdateTarget(gameTime,targetManager); // Pass targetManager
                tractor.UpdateMovement(gameTime,targetManager);
            }
            CheckRiceCollection();
            #region randomMovment
            // Update each tractor
            //foreach (var tractor in tractors)
            //{
            //    tractor.UpdateRandomMovment(gameTime);
            //}
            //for (int i = rices.Count - 1; i >= 0; i--) // Iterate backwards through the list
            //{
            //    foreach (var tractor in tractors)
            //    {
            //        if (Vector2.Distance(tractor.Position, rices[i].Position) < 60 ) // Assuming a small radius for collision
            //        {
            //            rices[i].Collect();
            //            rices.RemoveAt(i); // Remove the rice from the list
            //            break; // Exit the inner loop to avoid invalid operation due to removal
            //        }
            //    }
            //}
            
           
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Draw the map
            map.Draw(_spriteBatch);

            // Draw each tractor
            foreach (var tractor in tractors)
            {
                tractor.Draw(_spriteBatch);
            }
            foreach (var rice in rices)
            {
                rice.Draw(_spriteBatch);
            }

            SpriteFont font = Content.Load<SpriteFont>("font"); // Load your sprite font here
            int remainingRice = rices.Count(r => !r.IsCollected);
            string text = $"Rice left: {remainingRice}";
            _spriteBatch.DrawString(font, text, new Vector2(10, 10), Color.Black); // Position the text at the top-left corner
        

            // Draw the timer
            string timeText = $"Time: {elapsedTime:F2} seconds";
            _spriteBatch.DrawString(font, timeText, new Vector2(10, 30), Color.Black); // Position below the rice count

            _spriteBatch.End();
            base.Draw(gameTime);
        }

      

        // Method to check and handle rice collection
        private void CheckRiceCollection()
        {
            for (int i = rices.Count - 1; i >= 0; i--)
            {
                Rice rice = rices[i];
                foreach (var tractor in tractors)
                {
                    if (Vector2.Distance(tractor.Position, rice.Position) < 30 && !rice.IsCollected) // Assume 30 is the collection distance
                    {
                        rice.Collect();
                        rice.IsTargeted = false; // Mark as not targeted since it's collected
                        rices.RemoveAt(i);
                        break;
                    }
                }
            }
        }

    }
}
