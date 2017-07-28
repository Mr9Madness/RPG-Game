﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking {

    /// <summary>
    /// An indexed collection class to keep track of all the <see cref="Players"/>.
    /// </summary>
    public class Players {
        /// <summary>
        /// The list of <see cref="Players"/>.
        /// </summary>
        private static List<Player> _users = new List<Player>();
        /// <summary>
        /// The list of <see cref="TcpSocket"/>s connected to each <see cref="Player"/> by their userID.
        /// </summary>
        private static Dictionary<int, TcpSocket> _sockets = new Dictionary<int, TcpSocket>();

        /// <summary>
        /// This variable will keep track of the last used ID and increments it, so there will never be a duplicate ID.
        /// </summary>
        private static int _idAutoIncrement;

        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their ID.
        /// </summary>
        /// <param name="userID">The ID to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ int userID ] {
            get {
                foreach ( Player user in _users )
                    if ( user.ID == userID )
                        return user;
                return null;
            }
        }
        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ string username ] {
            get {
                foreach ( Player user in _users )
                    if ( user.Username == username )
                        return user;
                return null;
            }
        }
        /// <summary>
        /// Gets or sets a <see cref="Player"/> by finding their username.
        /// </summary>
        /// <param name="socket">The <see cref="TcpSocket"/> to search for</param>
        /// <returns>The <see cref="Player"/> if found, null if not</returns>
        public Player this[ TcpSocket socket ] {
            get {
                foreach ( Player user in _users )
                    if ( user.Socket == socket )
                        return user;
                return null;
            }
        }

        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( Player player ) { return _users.Contains( player ); }
        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="userID">The <see cref="Player"/>'s ID to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( int userID ) { return this[ userID ] != null; }
        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="username">The <see cref="Player"/>'s username to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( string username ) { return this[ username ] != null; }
        /// <summary>
        /// Checks if the given <see cref="Player"/> exists.
        /// </summary>
        /// <param name="socket">The <see cref="Player"/>'s <see cref="TcpSocket"/> to check</param>
        /// <returns>True if the <see cref="Player"/> was found, false if not</returns>
        public bool Exists( TcpSocket socket ) { return this[ socket ] != null; }

        /// <summary>
        /// Creates and adds a new <see cref="Player"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="username">The username of the <see cref="Player"/> to add</param>
        /// <param name="socket">The socket to connect the <see cref="Player"/> to</param>
        /// <returns>True if the player was successfully added, false if not</returns>
        public bool Add( string username, TcpSocket socket ) { return Add( new Player( username ), socket, false ); }

        /// <summary>
        /// Adds a <see cref="Player"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to add</param>
        /// <param name="socket">The socket to connect the <see cref="Player"/> to</param>
        /// <returns>True if the player was successfully added, false if not</returns>
        public bool Add( Player player, TcpSocket socket ) { return Add( player, socket, false ); }

        /// <summary>
        /// Adds a <see cref="Player"/> to the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to add</param>
        /// <param name="socket">The socket to connect the <see cref="Player"/> to</param>
        /// <param name="overwrite">Whether to overwrite if the player already exists or not (default: false)</param>
        /// <returns>True if the player was successfully added, false if not</returns>
        public bool Add( Player player, TcpSocket socket, bool overwrite ) {
            if ( player.ID < 0 )
                player.ID = _idAutoIncrement++;

            if ( this[ player.Username ] != null ) {
                if ( overwrite ) {
                    Remove( player.Username );
                    _users.Add( player );
                    _sockets.Add( player.ID, socket );
                } else return false;
            } else {
                _users.Add( player );
                _sockets.Add( player.ID, socket );
            }
            return true;
        }

        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( Player player ) {
            if ( !Exists( player ) )
                return true;

            _users.Remove( player );
            _sockets.Remove( player.ID );
            return !Exists( player ) && !_sockets.ContainsKey( player.ID );
        }
        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="username">The username of the <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( string username ) { return Remove( this[ username ] ); }
        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="userID">The ID of the <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( int userID ) { return Remove( this[ userID ] ); }
        /// <summary>
        /// Removes a <see cref="Player"/> from the <see cref="Players"/>-list.
        /// </summary>
        /// <param name="socket">The socket of the <see cref="Player"/> to remove from the list</param>
        /// <returns>True if the <see cref="Player"/> does not exist in the <see cref="Players"/>-list (anymore), false if the <see cref="Player"/> still exists</returns>
        public bool Remove( TcpSocket socket ) { return Remove( this[ socket ] ); }

        /// <summary>
        /// Clears the <see cref="Players"/>-list of all <see cref="Players"/> and <see cref="TcpSocket"/>s./>
        /// </summary>
        /// <returns>True if successfully cleared, false if not</returns>
        public bool Clear() {
            _users.Clear();
            _sockets.Clear();
            return _users.Count == 0 && _sockets.Count == 0;
        }

        /// <summary>
        /// Gets the <see cref="TcpSocket"/> connected to the <see cref="Player"/> by their userID.
        /// </summary>
        /// <param name="userID">The userID to search for</param>
        /// <returns></returns>
        public TcpSocket GetSocket( int userID ) { try { return _sockets[ userID ]; } catch ( KeyNotFoundException ) { return null; } }
        /// <summary>
        /// Sets the <see cref="TcpSocket"/> connected to the <see cref="Player"/> by their userID.
        /// </summary>
        /// <param name="userID">The userID of the <see cref="Player"/> to set the <see cref="TcpSocket"/> of</param>
        /// <param name="socket">The <see cref="TcpSocket"/> to set</param>
        public void SetSocket( int userID, TcpSocket socket ) { _sockets[ userID ] = socket; }

        /// <summary>
        /// Converts the <see cref="Players"/> class to a <see cref="Players"/>-list.
        /// </summary>
        /// <returns>The list of <see cref="Players"/></returns>
        public List<Player> ToList() { return _users; }

        /// <summary>
        /// Loads the <see cref="Players"/>-list from an external source (<see cref="TcpSocket"/>s are not available this way).
        /// </summary>
        /// <param name="usernameArray">The collection of <see cref="Players"/> to load</param>
        public void Load( IEnumerable<Player> usernameArray ) {
            Clear();
            foreach ( Player user in usernameArray )
                Add( user, null );
        }

        /// <summary>
        /// Loads the <see cref="Players"/>-list from an external source (<see cref="TcpSocket"/>s are not available this way).
        /// </summary>
        /// <param name="usernameArray">The collection of <see cref="Players"/> to load</param>
        public void Load( IEnumerable<string> usernameArray ) {
            Clear();
            foreach ( string username in usernameArray )
                Add( username, null );
        }
        public IEnumerator<Player> GetEnumerator() { return _users.GetEnumerator(); }
    }

    /// <summary>
    /// A data container class used to save the player's info like their username and socket information.
    /// </summary>
    [Serializable]
    public class Player {
        public int ID = -1;
        public string Username;
        public TcpSocket Socket {
            get => Data.Players.GetSocket( ID );
            set => Data.Players.SetSocket( ID, value );
        }

        public Player() { }

        public Player( string username ) {
            Username = username;
            Data.Players.Add( this, null );
        }

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="username"></param>
        /// <param name="socket"></param>
        public Player( string username, TcpSocket socket ) {
            Username = username;
            Socket = socket;
            Data.Players.Add( this, socket );
        }
    }

}