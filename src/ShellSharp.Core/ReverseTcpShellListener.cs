using ShellSharp.Core.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ShellSharp.Core
{
    public class ReverseTcpShellListener
    {
        private readonly int _port;
        private Boolean _stopRequested;

        public ReverseTcpShellListener(int port)
        {
            _port = port;
        }

        public void Start()
        {
            Task.Run(Listen);
        }

        public void Stop()
        {
            _stopRequested = true;
        }

        // Buffer for reading data
        private Byte[] _buffer = new Byte[8192 * 16];

        private AutoResetEvent _commandWaitHandle = new AutoResetEvent(false);
        private AutoResetEvent _answerWaitHandle = new AutoResetEvent(false);

        private string _commandToSend;

        private string _lastAnswer;
        private byte[] _prompt;

        public string GetLastAnswer()
        {
            _answerWaitHandle.WaitOne();
            return _lastAnswer;
        }

        public void SendCommand(string command)
        {
            _commandToSend = command.TrimEnd('\n', '\r') + "\n";
            _commandWaitHandle.Set();
        }

        private void Listen()
        {
            IPAddress localAddr = IPAddress.Parse("0.0.0.0");

            // TcpListener server = new TcpListener(port);
            var server = new TcpListener(localAddr, _port);

            // Start listening for client requests.
            server.Start();

            try
            {
                // Enter the listening loop.
                while (!_stopRequested)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing with 2 seconds timeout
                    NetworkStream stream = client.GetStream();
                    stream.ReadTimeout = 1000;

                    //receive the first stream of data.
                    ReceiveData(stream, false);

                    while (!_stopRequested)
                    {
                        //Enter main loop where we read and write to the shell
                        _commandWaitHandle.WaitOne();
                        _answerWaitHandle.Reset();
                        byte[] msg = Encoding.UTF8.GetBytes(_commandToSend);
                        stream.Write(msg, 0, msg.Length);

                        _lastAnswer = ReceiveData(stream, true);
                        _answerWaitHandle.Set();
                    }

                    // Shutdown and end connection
                    client.Close();
                    Console.WriteLine("Connection closed from the other party");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        private string ReceiveData(
            NetworkStream stream,
            Boolean isAnswerToCommand)
        {
            // Loop to receive all the data sent by the client.
            MemoryStream ms = ReadFromStream(stream, isAnswerToCommand);

            //we need to remove the first line if present
            Span<byte> array;
            if (isAnswerToCommand)
            {
                array = LinuxShellUtils.ParseShellAnswer(ms);
            }
            else
            {
                array = _prompt = ms.ToArray();
            }

            var answer = Encoding.UTF8.GetString(array);
            return answer;
        }

        /// <summary>
        /// Need to read until the other part returned an entire command 
        /// that is identified by having more than one line and the other part
        /// did not have anymore data.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private MemoryStream ReadFromStream(NetworkStream stream, bool waitForCommandAnswer)
        {
            int timeoutCount = 0;
            Boolean endCommandDetected = false;
            var ms = new MemoryStream();
            int bytesRead;

            do
            {
                try
                {
                    bytesRead = stream.Read(_buffer, 0, _buffer.Length);
                    ms.Write(_buffer, 0, bytesRead);

                    endCommandDetected =
                        ByteArrayUtils.ContainsPattern(_buffer, _prompt) ||
                        ByteArrayUtils.ContainsPattern(_buffer, LinuxShellUtils.EndCommandSequence, 0, bytesRead);
                }
                catch (IOException)
                {
                    Console.WriteLine("Timeout: " + ms.Length);
                    timeoutCount++;
                }
            } while (timeoutCount == 0 || (
                !endCommandDetected && timeoutCount < 10 && waitForCommandAnswer
                )
            );
            return ms;
        }
    }
}