using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiceProject4
{
    internal class Tractor : Drawable
    {
        #region A* Properties
        private List<Vector2> path = new List<Vector2>();
        public bool IsIdle { get; private set; } = true;
        public bool HasPath => path.Count > 0;

       

        #endregion
        private List<Rice> rices;




        private Map map;
        public float Speed { get; set; }
        private Random random = new Random();
        public Vector2 MovementDirection { get; private set; }
        private bool edgeInRange;
        private int radius = 5;
        private double timeSinceDirectionChange = 1; // Cooldown timer
        private const double directionChangeCooldown = 0.1; // Cooldown period in seconds
        private bool canChangeDirection = true;

        public Tractor(Vector2 position, Vector2 origin, Texture2D texture, float speed,Vector2 scale ,Map map,List<Rice> rices)
            : base(position, origin, texture,scale )
        {
            this.Speed = speed;
            this.map = map;
            this.MovementDirection = FindValidInitialDirection();
            this.rices = rices; // Set the reference to the passed list
        }

        public void UpdateRandomMovment(GameTime gameTime)
        {
            Vector2 potentialNewPosition = Position + MovementDirection * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool isEdgeNearby = IsEdgeNearby(Position, radius); // Check if there is an edge nearby

            if (isEdgeNearby && canChangeDirection)
            {
                ChangeDirectionRandomlyUntilValid();
                canChangeDirection = false; // Prevent further changes while in the radius
            }
            else if (!isEdgeNearby)
            {
                canChangeDirection = true; // Allow direction changes once out of the radius
            }

            if (map.IsValidRoad(potentialNewPosition))
            {
                Position = potentialNewPosition;
            }
        }


        private bool IsEdgeNearby(Vector2 position, int radius)
        {
            int centerX = (int)(position.X);
            int centerY = (int)(position.Y);

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius) // Circle check
                    {
                        Vector2 checkPos = new Vector2(centerX + x, centerY + y);
                        if (map.IsCross(checkPos))
                        {
                            edgeInRange = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ChangeDirectionRandomlyUntilValid()
        {
            Vector2 newDirection;
            do
            {
                newDirection = RandomDirection();
            } while (!IsDirectionValid(newDirection));
            MovementDirection = newDirection;
            UpdateDirectionBasedOnMovement();
        }

        private bool IsDirectionValid(Vector2 direction)
        {
            // Check if the direction from the current position leads to a road
            Vector2 checkPosition = Position;
            direction.Normalize();
            for (float i = 0; i <= 13; i++)
            {
                checkPosition += direction * i;
                if (!map.IsValidRoad(checkPosition))
                {
                    return false;
                }
            }
            return true;
        }

        private Vector2 FindValidInitialDirection()
        {
            Vector2 initialDirection;
            do
            {
                initialDirection = RandomDirection();
            } while (!IsDirectionValid(initialDirection));
            return initialDirection;
        }

        private Vector2 RandomDirection()
        {
            return random.Next(4) switch
            {
                0 => new Vector2(1, 0), // Right
                1 => new Vector2(-1, 0), // Left
                2 => new Vector2(0, 1), // Down
                _ => new Vector2(0, -1), // Up
            };
        }

        private void UpdateDirectionBasedOnMovement()
        {
            Rotation = 0; // Reset rotation for right and left movements
            Flip = SpriteEffects.None; // Reset flip

            if (MovementDirection.X > 0)
            {
                // Moving right: Normal orientation
                Flip = SpriteEffects.None;
            }
            else if (MovementDirection.X < 0)
            {
                // Moving left: Flip horizontally
                Flip = SpriteEffects.FlipHorizontally;
            }
            else if (MovementDirection.Y > 0)
            {
                // Moving down: Rotate 90 degrees
                Rotation = MathHelper.PiOver2;
            }
            else if (MovementDirection.Y < 0)
            {
                // Moving up: Rotate -90 degrees
                Rotation = -MathHelper.PiOver2;
            }
        }

        #region A*

        private void ReevaluateTarget(TargetManager targetManager)
        {
            Rice currentTarget = targetManager.GetTarget(this);
            Rice closestRice = FindNearestRice();

            if (closestRice != null && closestRice != currentTarget)
            {
                if (currentTarget != null)
                {
                    targetManager.ReleaseTarget(currentTarget);
                    currentTarget.IsTargeted = false;
                }
                if (targetManager.TryTargetRice(closestRice, this))
                {
                    currentTarget = closestRice;
                    currentTarget.IsTargeted = true;
                    FindPath(Position, closestRice.Position);
                }
            }
        }


        public void UpdateMovement(GameTime gameTime,TargetManager targetManager)
        {

            //if (rices.Count() == targetManager.TractorCount())
            //{
            //    ReevaluateTarget(targetManager);
            //}


            if (path.Count > 0)
            {
                IsIdle = false; // Tractor is not idle since it's moving

                Vector2 nextPoint = path[0]; // Get the next point from the path
                Vector2 direction = nextPoint - Position; // Calculate direction to next point

                // Normalize the direction vector if its length is greater than 1 for a consistent speed
                if (direction.LengthSquared() > 1)
                {
                    direction.Normalize();
                }

                // Update MovementDirection for sprite orientation before normalization
                MovementDirection = direction;

                // Calculate potential new position based on current direction and speed
                Vector2 potentialNewPosition = Position + direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Check if the tractor has reached the next point considering a small threshold for arrival
                if (Vector2.Distance(Position, nextPoint) <= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds)
                {
                    Position = nextPoint; // Snap the tractor to the next point
                    path.RemoveAt(0); // Remove the reached point from the path

                    // If the path is empty after moving to the next point, the tractor becomes idle
                    if (path.Count == 0)
                    {
                        IsIdle = true;
                    }
                }
                else
                {
                    Position = potentialNewPosition; // Move the tractor towards the next point
                }

                // Update the sprite orientation based on the direction of movement
                UpdateDirectionBasedOnMovement();
            }
            else
            {
                IsIdle = true; // Tractor is idle if there's no path
                               // Consider triggering new path finding here if needed
            }

        }




        public void FindPath(Vector2 start, Vector2 goal)
        {
            Node startNode = map.Nodes[(int)start.X, (int)start.Y];
            Node goalNode = map.Nodes[(int)goal.X, (int)goal.Y];

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].Total < currentNode.Total || openSet[i].Total == currentNode.Total && openSet[i].Heuristic < currentNode.Heuristic)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == goalNode)
                {
                    RetracePath(startNode, goalNode);
                    return;
                }

                foreach (Node neighbour in map.GetNeighbors(currentNode))
                {
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newCostToNeighbour = currentNode.Cost + GetDistance(currentNode, neighbour);
                    if (newCostToNeighbour < neighbour.Cost || !openSet.Contains(neighbour))
                    {
                        neighbour.Cost = newCostToNeighbour;
                        neighbour.Heuristic = GetDistance(neighbour, goalNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        private void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();

            this.path = path.Select(n => n.Position).ToList(); // Convert Node path to Vector2 path
        }

        private float GetDistance(Node a, Node b)
        {
            float dx = Math.Abs(a.Position.X - b.Position.X);
            float dy = Math.Abs(a.Position.Y - b.Position.Y);
            return dx + dy; // Manhattan Distance
        }

        public void UpdateTarget(GameTime gameTime, TargetManager targetManager)
        {
            if (IsIdle || !HasPath)
            {
                Rice nearestRice = FindNearestRice();
                if (nearestRice != null && targetManager.TryTargetRice(nearestRice, this))
                {
                    FindPath(Position, nearestRice.Position);
                }
            }

           
        }

        private Rice FindNearestRice()
        {
            Rice nearestRice = null;
            float nearestDistance = float.MaxValue;

            foreach (Rice rice in rices.Where(r => !r.IsCollected && !r.IsTargeted))
            {
                float distance = Vector2.Distance(Position, rice.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestRice = rice;
                }
            }

            return nearestRice;
        }

        // Ensure to mark rice as not targeted when collected or if the target changes


        #endregion
    }
}
