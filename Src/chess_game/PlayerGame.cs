using G12_ChessApplication.Src.chess_game.util;
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

        private int? selectedSquareIndex = null;
        public PlayerGame(MainWindow main, string code, ChessColor chessColor, string userName) : base(main, chessColor)
        {
            _code = code;
            userPlayerUsername = userName;
            UserPlayer = new Player(userName, chessColor);
            if (chessColor == ChessColor.BLACK)
            {
                turnToMove = false;
            }
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

                SendObject(_stream, "Username " + userPlayerUsername);
                Trace.WriteLine("Connected to server");
            }
            catch (SocketException)
            {
                mainWindow.GoBackToMainMenu();
            }
            catch (Exception e)
            {

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
                        string[] commands = Username.Split(" ");
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
                            SendObject(networkStream, "DENIED");
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Server was stopped
                    Trace.WriteLine("Server stopped listening.");
                    break;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Error in ListenForClients: {ex.Message}");
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
                    SendObject(_stream, "Color " + (int)PlayerColor);
                    SendObject(_stream, "Turn " + turnToMove);
                    SendObject(_stream, "Username " + userPlayerUsername);
                    string board = FenParser.GetFenStringFromArray(gameState);
                    if (PlayerColor != ChessColor.WHITE)
                    {
                        board = Reverse(board);
                    }
                    SendObject(_stream, "Board " + board);
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
                        turnToMove = true;
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
            string[] command = s.Split(' ');
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
                        checkMate = true;
                        MessageBox.Show("YOU WIN");
                        break;
                    case "StaleMate":
                        staleMate = true;
                        MessageBox.Show("StaleMate dumbass!");
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
                    case "Turn":
                        turnToMove = command[1] == "False";
                        break;
                    case "Board":
                        string board = command[1];
                        gameState = FenParser.CreatePieceArray(board);
                        MainWindow.mainBoard.SetBoardSetup(board);
                        break;
                    case "Username":
                        userOpponentUsername = command[1];
                        mainWindow.SetOpponent(userOpponentUsername);
                        break;
                    case "DENIED":
                        mainWindow.GoBackToMainMenu();
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
            byte[] lengthPrefix = new byte[4];
            ReadExactBytes(stream, lengthPrefix, 4);
            int dataLength = BitConverter.ToInt32(lengthPrefix, 0);

            byte[] jsonBytes = new byte[dataLength];
            ReadExactBytes(stream, jsonBytes, dataLength);

            string jsonString = Encoding.UTF8.GetString(jsonBytes);

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

        // Method to send the Move object
        void SendMove(NetworkStream networkStream, Move move)
        {
            var serializer = new XmlSerializer(typeof(Move));
            serializer.Serialize(networkStream, move);
        }

        // Method to receive the Move object
        Move ReceiveMove(NetworkStream networkStream)
        {
            var serializer = new XmlSerializer(typeof(Move));
            Move move = (Move)serializer.Deserialize(networkStream);
            move.InvertMove();
            return move;
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
                    //string message = "";
                    //message += SelectedPieceIndex.ToString();
                    //message += " " + index.ToString();
                    //byte[] data = Encoding.ASCII.GetBytes(message);

                    //_stream.Write(data, 0, data.Length);


                    Move currentMove = prevLegalMoves.Find(item => item.toIndex == index);
                    await ApplyMove(currentMove);
                    currentMove = PlayerMoves.Last();
                    turnToMove = false;
                    SendObject(_stream, currentMove);
                    //SendMove(_stream, currentMove);
                }
                catch (Exception e)
                {
                    _stream = null;

                }
                selectedSquareIndex = null;
            }
        }

        public override void SendMsg(string msg)
        {
            SendObject(_stream, msg);
        }

        public override async Task HandleGameEnd(bool isCheckmate, ChessColor winnerColor)
        {
            if (Online)
            {
                var dbConnector = new SQLConnector();
                int result;
                if (isCheckmate)
                {
                    result = 2;
                }
                else
                {
                    result = 0;
                }

                await dbConnector.AddOrUpdateMatchResult(userPlayerUsername, userOpponentUsername, result);
            }
        }
    }
}
