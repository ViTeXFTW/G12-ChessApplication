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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
                _client = new TcpClient("127.0.0.1", 12345);
                _stream = _client.GetStream();

                _clientThread = new Thread(ReceiveMessages);
                _clientThread.IsBackground = true;
                _clientThread.Start();
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
                    Move oppMove = ReceiveObject(_stream);
                    HandleMessage(oppMove);
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
                _clientThread = new Thread(ReceiveMessages);
                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
        }

        private void HandleMessage(Move move)
        {
            Trace.WriteLine($"Client: {move}\n");
            Application.Current.Dispatcher.BeginInvoke(
              DispatcherPriority.Background,
                new Action(() => {
                    ApplyMove(move);
                    mainWindow.UpdateUIAfterMove(move, false);
                }));
            turnToMove = true;
        }

        public void SendButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public override void SquareClicked(int index)
        {
            if (_stream == null)
            {
                if (!host)
                {
                    ConnectToServer();
                }
                return;
            }
            if (IsPieceSelected)
            {
                if (prevLegalMoves.Any(item => item.toIndex == index))
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
                        SendObject(_stream, currentMove);


                        ApplyMove(currentMove);
                        mainWindow.UpdateUIAfterMove(currentMove, false);
                        turnToMove = false;
                    }
                    catch (Exception e)
                    {
                        _stream = null; 
                    }
                    // Reset the color of the previously selected square
                    mainWindow.ResetSquareColor(SelectedPieceIndex);
                    selectedSquareIndex = null;

                }
                else
                {

                    // Invalid move, deselect the piece
                    DeselectPiece();

                    // Optionally update UI to reflect deselection

                    mainWindow.ResetSquareColor(SelectedPieceIndex);
                    selectedSquareIndex = null;
                }

                mainWindow.RemoveLegalMoves(prevLegalMoves);
            }
            else
            {
                if (CanSelectPieceAt(index))
                {
                    List<Move> legalMoves;

                    if (OpponentMoves.Count > 0)
                    {
                        legalMoves = gameState[index].FindLegalMoves(index, ref gameState, OpponentMoves.Last());
                    }
                    else
                    {
                        legalMoves = gameState[index].FindLegalMoves(index, ref gameState, null);
                    }
                    mainWindow.ShowLegalMoves(legalMoves);
                    prevLegalMoves = legalMoves;
                    // Deselect the previous selection if any
                    if (selectedSquareIndex != null)
                    {
                        mainWindow.ResetSquareColor(selectedSquareIndex.Value);
                    }

                    // Select the piece
                    SelectPieceAt(index);

                    // Highlight the selected square
                    mainWindow.HighlightSquare(index);

                    // Keep track of the selected square index
                    selectedSquareIndex = index;
                }
            }
        }


        public void SendObject(NetworkStream stream, Move obj)
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
        }
        public Move ReceiveObject(NetworkStream stream)
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
            Move move = JsonSerializer.Deserialize<Move>(jsonString);
            move.InvertMove();

            return move;
        }

        public void CleanUpSockets()
        {
            if (_server != null)
            {
                _server.Stop();
            }
            if (_client != null)
            {
                _client.Close();
            }
            if (_stream != null)
            {
                _stream.Close();
            }

        }
    }
}
