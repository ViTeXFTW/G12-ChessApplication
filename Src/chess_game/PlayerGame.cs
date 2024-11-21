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

        private int? selectedSquareIndex = null;
        public PlayerGame(MainWindow main, string code, ChessColor chessColor) : base(main, chessColor)
        {
            _code = code;
            if (chessColor == ChessColor.BLACK)
            {
                turnToMove = false;
            }
            Online = true;
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
                Trace.WriteLine("Connected to server");

                _clientThread = new Thread(ReceiveMessages);
                _clientThread.IsBackground = true;
                _clientThread.Start();
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
                try
                {
                    Object obj = ReceiveObject(_stream);

                    //Move oppMove = ReceiveMove(_stream);
                    HandleMessage(obj);
                }
                catch
                {
                    Trace.WriteLine("Server disconnected.\n");
                    break;
                }
            }
        }
        private void StartServer()
        {
            _server = new TcpListener(IPAddress.Any, 12345);
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
                    TryConnect();
                }
                catch (Exception)
                {

                    break;
                }
            }

            void TryConnect()
            {
                TcpClient client = _server.AcceptTcpClient();
                _stream = client.GetStream();
                SendObject(_stream, "Color " + (int) PlayerColor );
                SendObject(_stream, "Turn " + turnToMove );
                string board = FenParser.GetFenStringFromArray(gameState);
                if (PlayerColor != ChessColor.WHITE)
                {
                    board = Reverse(board);
                }
                SendObject(_stream, "Board " + board);
                SendGameHistrory(_stream);
                _clientThread = new Thread(ReceiveMessages);
                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
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
                        UserPlayer.SetColor(PlayerColor);
                        Setup();
                        break;
                    case "Turn":
                        turnToMove = command[1] == "False";
                        break;
                    case "Board":
                        string board = command[1];
                        gameState = FenParser.CreatePieceArray(board);
                        MainWindow.mainBoard.SetBoardSetup(board);
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
            stream.Read(lengthPrefix, 0, lengthPrefix.Length);
            int dataLength = BitConverter.ToInt32(lengthPrefix, 0);

            // Read the actual data
            byte[] jsonBytes = new byte[dataLength];
            stream.Read(jsonBytes, 0, dataLength);

            // Convert byte array to JSON string
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
            }
            if (_client != null)
            {
                _client.Close();
                _client.Dispose();
            }
            if (_stream != null)
            {
                _stream.Close();
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

        public override void Setup()
        {
            SetUpSocket();
            MainWindow.mainBoard.SetBoardSetup();
        }
    }
}
