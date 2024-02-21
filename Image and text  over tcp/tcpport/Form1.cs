using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections;

namespace tcpport
{
    public partial class Form1 : Form
    {
        Thread recievecamerathread;
        Thread recievetextthread;

        TcpClient tcpClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tcpClient = new TcpClient("10.100.48.173", 8888);  //192.168.43.251   65535     //10.255.85.246   5297     //100.100.43.74     //172.20.10.2 ahmet tel     //5.47.254.229 izzet tel
            NetworkStream networkStream = tcpClient.GetStream();  //hostname -I
            if (checkBox2.Checked)
            {
                recievecamerathread = new Thread(() => image_recieve(networkStream));
                recievecamerathread.Start();
            }
            
            else if (checkBox1.Checked)
            {
                recievetextthread = new Thread(() => text_recieve(networkStream));
                recievetextthread.Start();
            }
        }

        private void image_recieve(NetworkStream networkStream)
        {
            while (networkStream.CanRead)
            {
                byte[] readbuffer = new byte[tcpClient.ReceiveBufferSize*(5)];
                int nmbrbyteread = networkStream.Read(readbuffer, 0, readbuffer.Length);
                pictureBox1.Image = bytearraytoimage(readbuffer);
            }
        }

        private Image bytearraytoimage(byte[] readbuffer)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Position = 0;
            memoryStream.Write(readbuffer, 0, readbuffer.Length);
            //Image img = Image.FromStream(memoryStream,true,false); //true
            try
            {
                Bitmap bitmap = (Bitmap)Image.FromStream(memoryStream, true, false);
                return bitmap;
            }
            catch 
            {
                return null;
            }
        }

        private void text_recieve(NetworkStream networkStream)
        {
            while (networkStream.CanRead)
            {
                byte[] readbuffer = new byte[tcpClient.ReceiveBufferSize];
                int nmbrbyteread = networkStream.Read(readbuffer, 0, readbuffer.Length);
                textBox1.Text += System.Text.Encoding.UTF8.GetString(readbuffer);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tcpClient.Close();
            pictureBox1.Image = null;
        }
    }
}