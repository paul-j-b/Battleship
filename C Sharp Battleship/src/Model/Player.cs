﻿/// <summary>
/// Player has its own _PlayerGrid, and can see an _EnemyGrid, it can also check if
/// all ships are deployed and if all ships are detroyed. A Player can also attach.
/// </summary>
using System;
using System.Collections;
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

    public class Player : IEnumerable<Ship>
    {
        protected static Random _Random = new Random();

        private Dictionary<ShipName, Ship> _Ships = new Dictionary<ShipName, Ship>();
        private SeaGrid _playerGrid;
        // so this had a squiggly line, I moved it to the constructor
        private ISeaGrid _enemyGrid;
        protected BattleShipsGame _game;

        private int _shots;
        private int _hits;
        private int _misses;

        /// <summary>
        /// Returns the game that the player is part of.
        /// </summary>
        /// <value>The game</value>
        /// <returns>The game that the player is playing</returns>
        public BattleShipsGame Game
        {
            get
            {
                return _game;
            }
            set
            {
                _game = value;
            }
        }

        /// <summary>
        /// Sets the grid of the enemy player
        /// </summary>
        /// <value>The enemy's sea grid</value>
        public ISeaGrid Enemy
        {
            set
            {
                _enemyGrid = value;
            }
        }

        /// <summary>
        /// Player constructor, a player keeps track of their shots, hits and misses.
        /// </summary>
        public Player(BattleShipsGame controller)
        {
            _playerGrid = new SeaGrid(_Ships);
            _game = controller;

            // for each ship add the ships name so the seagrid knows about them
            foreach (ShipName name in Enum.GetValues(typeof(ShipName)))
            {
                if (name != ShipName.None)
                    _Ships.Add(name, new Ship(name));
            }

            RandomizeDeployment();
        }

        /// <summary>
        /// The EnemyGrid is a ISeaGrid because you shouldn't be allowed to see the enemies ships
        /// </summary>
        public ISeaGrid EnemyGrid
        {
            get
            {
                return _enemyGrid;
            }
            set
            {
                _enemyGrid = value;
            }
        }

        /// <summary>
        /// The PlayerGrid is just a normal SeaGrid where the players ships can be deployed and seen
        /// </summary>
        public SeaGrid PlayerGrid
        {
            get
            {
                return _playerGrid;
            }
        }

        /// <summary>
        /// ReadyToDeploy returns true if all ships are deployed
        /// </summary>
        public bool ReadyToDeploy
        {
            get
            {
                return _playerGrid.AllDeployed;
            }
        }

        /// <summary>
        /// IsDestroyed returns true if all ships are destroyed
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                // Check if all ships are destroyed... -1 for the none ship
                return _playerGrid.ShipsKilled == (Enum.GetValues(typeof(ShipName)).Length - 1);
            }
        }

        /// <summary>
        /// Returns the Player's ship with the given name.
        /// </summary>
        /// <paramname="name">the name of the ship to return</param>
        /// <value>The ship</value>
        /// <returns>The ship with the indicated name</returns>
        /// <remarks>The none ship returns nothing/null</remarks>
        public Ship get_Ship(ShipName name)
        {
            if (name == ShipName.None)
                return null;

            return _Ships[name];
        }

        /// <summary>
        /// The number of shots the player has made
        /// </summary>
        /// <value>shots taken</value>
        /// <returns>teh number of shots taken</returns>
        public int Shots
        {
            get
            {
                return _shots;
            }
        }

        /// <summary>
        /// The number of hits the player has made
        /// </summary>
        /// <value>hits made</value>
        /// <returns>the number of hits made</returns>
        public int Hits
        {
            get
            {
                return _hits;
            }
        }

        /// <summary>
        /// Total number of shots that missed
        /// </summary>
        /// <value>miss count</value>
        /// <returns>the number of shots that have missed ships</returns>
        public int Missed
        {
            get
            {
                return _misses;
            }
        }

        /// <summary>
        /// Total score of the player.
        /// </summary>
        /// <value>score</value>
        /// <returns>the score of the player.</returns>
        public int Score
        {
            get
            {
                if (IsDestroyed)
                    return 0;
                else
                    return (Hits * 12) - Shots - (PlayerGrid.ShipsKilled * 20);
            }
        }

        /// <summary>
        /// Makes it possible to enumerate over the ships the player
        /// has.
        /// </summary>
        /// <returns>A Ship enumerator</returns>
        public IEnumerator<Ship> GetShipEnumerator()
        {
            Ship[] result = new Ship[_Ships.Values.Count + 1];
            _Ships.Values.CopyTo(result, 0);
            List<Ship> lst = new List<Ship>();
            lst.AddRange(result);

            return lst.GetEnumerator();
        }

        /// <summary>
        /// Makes it possible to enumerate over the ships the player
        /// has.
        /// </summary>
        /// <returns>A Ship enumerator</returns>
        public IEnumerator<Ship> GetEnumerator()
        {
            Ship[] result = new Ship[_Ships.Values.Count + 1];
            _Ships.Values.CopyTo(result, 0);
            List<Ship> lst = new List<Ship>();
            lst.AddRange(result);

            return lst.GetEnumerator();
        }

        /// <summary>
        /// Vitual Attack allows the player to shoot
        /// </summary>
        public virtual AttackResult Attack()
        {
            // human does nothing here...
            return null;
        }

        /// <summary>
        /// Shoot at a given row/column
        /// </summary>
        /// <param name="row">the row to attack</param>
        /// <param name="col">the column to attack</param>
        /// <returns>the result of the attack</returns>
        internal AttackResult Shoot(int row, int col)
        {
            _shots += 1;
            AttackResult result;
            result = EnemyGrid.HitTile(row, col);

            switch (result.Value)
            {
                case var @case when @case == ResultOfAttack.Destroyed:
                case var case1 when case1 == ResultOfAttack.Hit:
                    {
                        _hits += 1;
                        break;
                    }

                case var case2 when case2 == ResultOfAttack.Miss:
                    {
                        _misses += 1;
                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// Generate random deployment positions for the ships.
        /// </summary>
        public virtual void RandomizeDeployment()
        {
            bool placementSuccessful = default(bool);
            Direction heading = default(Direction);

            // for each ship to deploy in shipist
            foreach (ShipName shipToPlace in Enum.GetValues(typeof(ShipName)))
            {
                if (shipToPlace == ShipName.None)
                    continue;

                placementSuccessful = false;

                // generate random position until the ship can be placed
                do
                {
                    int dir = _Random.Next(2);
                    int x = _Random.Next(0, 11);
                    int y = _Random.Next(0, 11);
                    if (dir == 0)
                        heading = Direction.UpDown;
                    else
                        heading = Direction.LeftRight;

                    // try to place ship, if position unplaceable, generate new coordinates
                    try
                    {
                        PlayerGrid.MoveShip(x, y, shipToPlace, heading);
                        placementSuccessful = true;
                    }
                    catch
                    {
                        placementSuccessful = false;
                    }
                }
                while (!placementSuccessful);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}