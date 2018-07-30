using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Render2
{
    public partial class Form1 : Form
    {
        Device device;
        Drawing drawing;
        Graphics gc;

        float alpha;
        float rot;

        public Form1()
        {
            InitializeComponent();

            Width = 800;
            Height = 600;
            //初始化
            device = new Device();
            device.InitDevice(800, 600);
            drawing = new Drawing(device);

            alpha = device.alpha;
            rot = device.camera.eye.x;


            //时间事件
            System.Timers.Timer mainTimer = new System.Timers.Timer(1000 / 60f);//FPS=60

            mainTimer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            mainTimer.AutoReset = true;
            mainTimer.Enabled = true;
            mainTimer.Start();

            
        }

        private void Tick(object sender, EventArgs e)
        {
            lock(device.frameBuffer)
            {
                //数据刷新
                device.alpha = alpha;
                device.camera.eye.x = rot;
                //初始化bitmap并且在frameBuffer上重绘图像
                device.ClearBuffer();
                drawing.DrawBox(device.alpha);
                if (gc==null)
                {
                    gc = this.CreateGraphics();
                }
                //初始化窗体并将bitmap绘在窗体上 双缓冲区
                BufferedGraphicsContext dc = new BufferedGraphicsContext();
                BufferedGraphics backBufeer = dc.Allocate(gc, new Rectangle(new Point(0, 0), this.Size));
                gc.Clear(System.Drawing.Color.Black);
                gc.DrawImage(device.frameBuffer, 0, 0);
                backBufeer.Render(this.CreateGraphics());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                alpha += 0.1f;
            if (e.KeyCode == Keys.Right)
                alpha -= 0.1f;
            if (e.KeyCode == Keys.Up)
                rot -= 0.1f;
            if (e.KeyCode == Keys.Down)
                rot += 0.1f;

        }


    }
}
