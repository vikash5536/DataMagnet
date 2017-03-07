using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MutiThreadServer
{
    //Class to handle each client request separatly
    public class handleClinet
    {
        TcpClient clientSocket;
        string clNo;
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            //string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    //networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    //dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    //dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    //Console.WriteLine(" >> " + "From client-" + clNo + dataFromClient);
                    if (requestCount == 4 || requestCount == 5)
                    {
                        rCount = Convert.ToString(requestCount);
                        serverResponse = "1.005";
                    }
                    else
                    {
                        rCount = Convert.ToString(requestCount);
                        serverResponse = clNo;
                    }
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }
    } 
}
