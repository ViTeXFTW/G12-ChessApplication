﻿using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace G12_ChessApplication.Src.chess_game
{
    class PlayerGame : Game
    {
        private TcpClient _client { get; set; }
        private TcpListener _server { get; set; }
        private NetworkStream _stream { get; set; }
        private Thread _listenerThread { get; set; }
        private Thread _clientThread { get; set; }
        private string _code { get; set; }
        private bool host { get; set; } = true;
        public string userPlayerUsername { get; set; } = "";
        public string userOpponentUsername { get; set; } = "";
        public bool Draw { get; set; } = false;

        public PlayerGame(MainWindow main, string code, ChessColor chessColor, string userName) : base(main, chessColor)
        {
            _code = code;
            userPlayerUsername = userName;
            UserPlayer = new Player(userName, chessColor);
            Online = true;
            MainWindow.mainBoard.SetBoardSetup();
            SetUpSocket();
        }

        private void SetUpSocket()
        {
            if (_code == "Host")
            {
                StartServer();
            }
            else
            {
                ConnectToServer();
                host = false;
            }
        }
        private void ConnectToServer()
        {
            try
            {
                _client = new TcpClient(_code, 12345);
                _stream = _client.GetStream();

                _clientThread = new Thread(ReceiveMessages);
                _clientThread.IsBackground = true;
                _clientThread.Start();

                SendMsg("Username:" + userPlayerUsername);
                Trace.WriteLine("Connected to server");
            }
            catch (SocketException)
            {
                mainWindow.GoBackToMainMenu();
            }
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                if (_stream == null || _client == null)
                {
                    break;
                }
                try
                {
                    if (_stream.DataAvailable)
                    {
                        Object obj = ReceiveObject(_stream);

                        //Move oppMove = ReceiveMove(_stream);
                        HandleMessage(obj);
                    }
                }
                catch
                {
                    Trace.WriteLine("Failed to receive.\n");
                }
            }
        }
        private void StartServer()
        {
            _server = new TcpListener(IPAddress.Parse(mainWindow.LocalIP), 12345);
            _server.Start();
            _listenerThread = new Thread(ListenForClients);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }

        private void ListenForClients()
        {
            while (true)
            {
                if (_server == null)
                {
                    break;
                }

                try
                {
                    TcpClient client = _server.AcceptTcpClient();
                    NetworkStream networkStream = client.GetStream();
                    Object obj = ReceiveObject(networkStream);
                    if (obj is string Username)
                    {
                        string[] commands = Username.Split(":");
                        if ( (commands[1] == userOpponentUsername || _client == null) && commands[1] != userPlayerUsername)
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                              DispatcherPriority.Background,
                                new Action(() => {
                                    userOpponentUsername = commands[1];
                                    mainWindow.SetOpponent(userOpponentUsername);
                                }));
                            if (_client != null)
                            {
                                _client.Close();
                                _client = null;
                                Thread.Sleep(100);
                            }
                            HandleClientConnection(client);
                        }
                        else
                        {
                            SendMsg("DENIED");
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Server was stopped
                    Trace.WriteLine("Server stopped listening.");
                    break;
                }
                catch (SocketException s)
                {
                    Trace.WriteLine(s.Message);
                }
            }
        }

        void HandleClientConnection(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _clientThread = new Thread(ReceiveMessages);
            _clientThread.IsBackground = true;
            _clientThread.Start();

            Application.Current.Dispatcher.BeginInvoke(
              DispatcherPriority.Background,
                new Action(() => {
                    SendMsg("Color:" + (int)PlayerColor);
                    SendMsg("Username:" + userPlayerUsername);
                    string board = FenParser.GetFenStringFromArray(gameState, CurrentPlayerColor, PlayerColor != ChessColor.WHITE);
                    SendMsg("Board:" + board);
                    SendGameHistrory(_stream);
                }));
        }

        private void SendGameHistrory(NetworkStream stream)
        {
            foreach (GameRecord entry in mainWindow.GameHistory.Items)
            {
                SendObject(stream, entry);
            }
        }

        private void HandleMessage(Object obj)
        {
            Application.Current.Dispatcher.BeginInvoke(
              DispatcherPriority.Background,
                new Action(() => {
                    if (obj is Move move)
                    {
                        ApplyMove(move);
                        Draw = false;
                        CurrentPlayerColor = UserPlayer.Color;
                    }
                    else if (obj is GameRecord moveRecord)
                    {
                        mainWindow.GameHistory.Items.Add(moveRecord);
                    }
                    else if (obj is String s)
                    {
                        HandleStringMsg(s);
                    }
                }));
        }

        private void HandleStringMsg(string s)
        {
            string[] command = s.Split(':');
            if (command.Length != 0)
            {
                switch(command[0])
                {
                    case "Check":
                        mainWindow.HighlightSquare(ConvertIndex(Convert.ToInt32(command[1])), MainWindow.DefaultCheckColor);
                        break;
                    case "UnCheck":
                        mainWindow.ResetCheckColor(ConvertIndex(Convert.ToInt32(command[1])));
                        break;
                    case "CheckMate":
                        gameRunning = false;
                        MessageBox.Show("You win by checkmate");
                        break;
                    case "StaleMate":
                        gameRunning = false;
                        MessageBox.Show("Stalemate!");
                        break;
                    case "Color":
                        PlayerColor = (ChessColor)Math.Abs(Convert.ToInt32(command[1]) - 1);
                        if (UserPlayer == null)
                        {
                            UserPlayer = new Player(userPlayerUsername, PlayerColor);
                        }
                        else
                        {
                            UserPlayer.SetColor(PlayerColor);
                        }
                        break;
                    case "Board":
                        string board = command[1];
                        SetGameState(board);
                        break;
                    case "Username":
                        userOpponentUsername = command[1];
                        mainWindow.SetOpponent(userOpponentUsername);
                        break;
                    case "DENIED":
                        mainWindow.GoBackToMainMenu();
                        break;
                    case "Draw":
                        Draw = true;
                        MessageBox.Show("Opponent offered a draw");
                        break;
                    case "AcceptDraw":
                        gameRunning = false;
                        MessageBox.Show("The game ended in a draw");
                        Draw = false;
                        break;
                    case "Resign":
                        gameRunning = false;
                        MessageBox.Show("You win by resignation");
                        break;
                    default:
                        break;
                }
            }
        }

        private int ConvertIndex(int index)
        {
            return Math.Abs(index - 63);
        }


        public void SendObject(NetworkStream stream, Object obj)
        {
            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(obj);

            // Convert JSON string to byte array
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // Send the length of the data first, so the receiver knows how much to read
            byte[] lengthPrefix = BitConverter.GetBytes(jsonBytes.Length);
            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            // Send the serialized JSON object
            stream.Write(jsonBytes, 0, jsonBytes.Length);
            stream.Flush();
        }
        public Object ReceiveObject(NetworkStream stream)
        {
            // Read the length prefix to determine the size of the incoming data
            string jsonString = string.Empty;
            try
            {
                byte[] lengthPrefix = new byte[4];
                ReadExactBytes(stream, lengthPrefix, 4);
                int dataLength = BitConverter.ToInt32(lengthPrefix, 0);

                byte[] jsonBytes = new byte[dataLength];
                ReadExactBytes(stream, jsonBytes, dataLength);

                jsonString = Encoding.UTF8.GetString(jsonBytes);
            }
            catch (IOException i)
            {
                Trace.WriteLine(i.Message);
                return "";
            }

            // Deserialize JSON string to object
            try
            {
                if (jsonString.Contains("\"fromIndex\""))
                {
                    Move move = JsonSerializer.Deserialize<Move>(jsonString);
                    move.InvertMove();
                    return move;
                }
                else
                {
                    GameRecord moveRecord = JsonSerializer.Deserialize<GameRecord>(jsonString);
                    return moveRecord;
                }
            }
            catch (JsonException)
            {
                string msg = JsonSerializer.Deserialize<string>(jsonString);
                return msg;
            }

        }
        void ReadExactBytes(NetworkStream stream, byte[] buffer, int count)
        {
            int bytesRead = 0;
            while (bytesRead < count)
            {
                int read = stream.Read(buffer, bytesRead, count - bytesRead);
                if (read == 0)
                    throw new IOException("Connection closed before all data was received.");
                bytesRead += read;
            }
        }

        public void CleanUpSockets()
        {
            if (_server != null)
            {
                _server.Stop();
                _server.Dispose();
                _server = null;
            }
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

        }

        public override async void HandleClick(int index)
        {
            if (_stream == null)
            {
                if (!host)
                {
                    ConnectToServer();
                }
                return;
            }
            else if (prevLegalMoves.Any(item => item.toIndex == index))
            {
                try
                {
                    // Switch to the next player
                    ChessColor opponent = (UserPlayer.Color == ChessColor.WHITE) ? ChessColor.BLACK : ChessColor.WHITE;
                    Trace.WriteLine($"Changed player to {opponent}");


                    Move currentMove = prevLegalMoves.Find(item => item.toIndex == index);
                    await ApplyMove(currentMove);
                    Draw = false;
                    currentMove = PlayerMoves.Last();
                    CurrentPlayerColor = opponent;
                    SendObject(_stream, currentMove);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    _stream = null;

                }
                selectedSquareIndex = null;
            }
        }

        public override bool SendMsg(string msg)
        {
            try
            {
                SendObject(_stream, msg);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message + ", " + msg);
                return false;
            }

            return true;
        }


        // Result:
        //      0 = Draw
        //      1 = Win
        //      2 = Loss
        public override async Task HandleGameEnd(int result)
        {
            if (Online)
            {
                var dbConnector = new SQLConnector();

                await dbConnector.AddOrUpdateMatchResult(userPlayerUsername, userOpponentUsername, result);
            }
        }

        public void OfferDraw()
        {
            if (Draw)
            {
                gameRunning = false;
                Draw = false;
                SendMsg("AcceptDraw");
                MessageBox.Show("The game ended in a draw");
                _ = HandleGameEnd(0);
            }
            else
            {
                SendMsg("Draw");
                MessageBox.Show("You offered a draw");
            }

        }

        public void Resign()
        {
            gameRunning = false;
            SendMsg("Resign");
            MessageBox.Show("You lose by resignation");
            _ = HandleGameEnd(2);
        }
    }
}
