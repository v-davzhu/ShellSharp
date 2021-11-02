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

        private string? _commandToSend;

        private string? _lastAnswer;
        private string? _prompt;

        /// <summary>
        /// TODO: use https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-2-asyncautoresetevent/
        /// </summary>
        /// <returns></returns>
        public async Task<string?> GetLastAnswer()
        {
            _answerWaitHandle.WaitOne();
            return _lastAnswer;
        }

        public bool IsConnected => !String.IsNullOrEmpty(_prompt);

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
                    _prompt = ReceiveData(stream, false);

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

            var answer = Encoding.UTF8.GetString(ms.ToArray());

            if (isAnswerToCommand)
            {
                if (answer.StartsWith(_commandToSend))
                {
                    answer = answer.Substring(_commandToSend.Length).TrimStart('\n', '\r');
                }

                if (answer.EndsWith(_prompt))
                {
                    answer = answer
                        .Substring(0, answer.Length - _prompt.Length)
                        .TrimEnd('\n', '\r');
                }
            }
            return answer;
        }

        /// <summary>
        /// Need to read until the other part returned an entire command 
        /// that is identified by having more than one line and the other part
        /// did not have anymore data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="isAnswerToCommand"></param>
        /// <returns></returns>
        private MemoryStream ReadFromStream(NetworkStream stream, Boolean isAnswerToCommand)
        {
            int timeoutCount = 0;
            var ms = new MemoryStream();
            int bytesRead;
            int maxTimeout = isAnswerToCommand ? 10 : 1;
            do
            {
                try
                {
                    bytesRead = stream.Read(_buffer, 0, _buffer.Length);
                    ms.Write(_buffer, 0, bytesRead);

                    if (!stream.DataAvailable && !string.IsNullOrEmpty(_prompt))
                    {
                        //we have no more data               
                        var dataReceived = Encoding.UTF8.GetString(ms.ToArray());
                        if (dataReceived.EndsWith(_prompt))
                        {
                            //data read finished, prompt received no need to wait anymore
                            return ms;
                        }
                    }
                }
                catch (IOException)
                {
                    timeoutCount++;
                }
            } while (stream.DataAvailable || (timeoutCount < maxTimeout));

            return ms;
        }
    }
}