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

        public ChessColor color {  get; set; }

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
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Trace.WriteLine($"Server: {message}\n");
                    HandleMessage(message);
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
                _clientThread = new Thread(HandleClientComm);
                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
        }

        private void HandleClientComm()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    HandleMessage(message);
                }
                catch
                {
                    Trace.WriteLine("Client disconnected.");
                    break;
                }
            }
        }

        private void HandleMessage(string message)
        {
            string[] fields = message.Split(' ');
            int[] indexes = new int[2];
            for (int i = 0; i < fields.Length; i++)
            {
                indexes[i] = GetDigit(fields[i]);
            }

            Trace.WriteLine($"Client: {message}\n");
            Application.Current.Dispatcher.BeginInvoke(
              DispatcherPriority.Background,
                new Action(() => {
                    ApplyMove(indexes[0], indexes[1]);
                    mainWindow.UpdateUIAfterMove(indexes[0], indexes[1]);
                }));
            turnToMove = true;
        }

        private int GetDigit(string v)
        {
            int digit = Int32.Parse(v);
            int index = Math.Abs(digit - 63);
            return index;
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
                if (IsValidMove(SelectedPieceIndex, index))
                {
                    try
                    {


                        // Switch to the next player
                        ChessColor opponent = (userPlayer.Color == ChessColor.WHITE) ? ChessColor.BLACK : ChessColor.WHITE;
                        Trace.WriteLine($"Changed player to {opponent}");
                        string message = "";
                        message += SelectedPieceIndex.ToString();
                        message += " " + index.ToString();
                        byte[] data = Encoding.ASCII.GetBytes(message);

                        _stream.Write(data, 0, data.Length);

                        ApplyMove(SelectedPieceIndex, index);
                        mainWindow.UpdateUIAfterMove(SelectedPieceIndex, index);
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
            }
            else
            {
                if (CanSelectPieceAt(index))
                {
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
