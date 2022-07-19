﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public bool btnstate = false;
        public Thread myThread;
        public Socket mySocket;
        public IPEndPoint localEP;
        public int localPort;
        public bool m_Listening;
        public Socket Client;
        public string text;

        string ip = "127.0.0.1";

        private delegate void Edit(string s);



        public int setPort
        {
            get { return setPort; }
            set { setPort = 60000; }
        }

        //socket连接通信程序
        public void OnConnectRequest(IAsyncResult ar)
        {
            // richTextBox1.Text = text + "  进入接受步骤";
            Edit e = new Edit(this.DisPlay);
            this.BeginInvoke(e, "连续监听状态");
            Socket server = (Socket)ar.AsyncState;
            Client = server.EndAccept(ar);
            //string strDateLine = "连接视觉算法服务器成功！！";
            Byte[] buffer = System.Text.Encoding.ASCII.GetBytes("");
            //Client.Send(buffer,buffer.Length,0);
            server.BeginAccept(new AsyncCallback(OnConnectRequest), server);
            while (true)
            {
                this.BeginInvoke(e, "进入接收指令状态");
                int recv = Client.Receive(buffer);
                string stringdata = Encoding.ASCII.GetString(buffer,0,recv);
                DateTimeOffset now = DateTimeOffset.Now;

                string ip = Client.RemoteEndPoint.ToString();
                this.BeginInvoke(e, stringdata);
                if (stringdata == "STOP")
                {
                    break;
                }
                if(stringdata == "S2")
                {
                    buffer = System.Text.Encoding.ASCII.GetBytes(algorithm());

                    Client.Send(buffer, buffer.Length, 0); 
                }
            }
            
        }

        //socket监听程序
        public void Listen()
        {

            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress myIP = IPAddress.Parse(ip);
            IPEndPoint ipEp = new IPEndPoint(myIP, 60000);

            //localEP = new IPEndPoint(IPAddress.Any, setPort);
            try { 
                Edit e = new Edit(this.DisPlay);
                mySocket.Bind(ipEp);
                this.BeginInvoke(e, "绑定成功");

                //richTextBox1.Text = text;
                mySocket.Listen(10);
                //   richTextBox1.Text = text+"  监听中";

                m_Listening = true;
                mySocket.BeginAccept(new AsyncCallback(OnConnectRequest), mySocket);
                //richTextBox1.Text = text + "  异步监听";
            }
            catch(Exception ex) { 
            
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            HObject ho_Image;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            //Image Acquisition 01: Code generated by Image Acquisition 01
            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, "D:/halcon-22.05.0.0-x64-win64/data/HALCON-22.05-Progress/examples/images/a01.png");


            HImage image = new HImage();
            image.ReadImage(@"D:/halcon-22.05.0.0-x64-win64/data/HALCON-22.05-Progress/examples/images/a01.png");
            
            int width, height;
            image.GetImageSize(out width, out height);

            #region 设置窗口大小
             double ratioWidth = (1.0) * width / hWindowControl1.Width;
            double ratioHeight = (1.0) * height / hWindowControl1.Height;
            HTuple row1, column1, row2, column2;
            if (ratioWidth >= ratioHeight)
            {
                row1 = -(1.0) * ((hWindowControl1.Height * ratioWidth) - height) / 2;
                column1 = 0;
                row2 = row1 + hWindowControl1.Height * ratioWidth;
                column2 = column1 + hWindowControl1.Width * ratioWidth;
            }
            else
            {
                row1 = 0;
                column1 = -(1.0) * ((hWindowControl1.Width * ratioHeight) - width) / 2;
                row2 = row1 + hWindowControl1.Height * ratioHeight;
                column2 = column1 + hWindowControl1.Width * ratioHeight;
            }

            hWindowControl1.HalconWindow.SetPart(row1, column1, row2, column2);
            #endregion



            hWindowControl1.HalconWindow.DispObj(image);




            ho_Image.Dispose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (btnstate == false)
            {
                ThreadStart myThreadDelegate = new ThreadStart(Listen);

                myThread = new Thread(myThreadDelegate);
                myThread.Start();
                btnstate = true;
                button2.Text = "停止程序";
            }
            else
            {
                m_Listening = false;
                try
                {
                    if(mySocket != null)
                    {
                        mySocket.Shutdown(SocketShutdown.Both);
                        mySocket.Close();
                    }
                    
                    myThread.Abort();
                }
                catch
                {

                }
                
                btnstate = false;
                button2.Text = "启动程序";
            }
        }

        //关闭程序时保证关闭线程
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(myThread != null)
            {
                myThread.Abort();
            }
        }

        //显示图像
        private void DisPlay(string s)
        {
            richTextBox1.AppendText(s);
        }

        //编写算法
        private string algorithm()
        {

            string res = "";

            return res;
        }
    }
}
