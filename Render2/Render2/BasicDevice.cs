using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Render2
{
    public enum RenderState
    {
        WIRE,
        COLOR,
        TEXTURE
    }
    public enum TextureFilterMode
    {
        POINT
    }
    public enum LightMode
    {
        ON,
        OFF
    }



    public class Device
    {
        public int width, height;             //窗口尺寸
        public int texWidth, texHeight;       //纹理尺寸

        public RenderState renderState;
        public TextureFilterMode textureFilterMode;    //渲染模式
        public LightMode lightMode;

        public Light light;                     //光照信息
        public Camera camera;                   //相机信息
        public Transform ts;                    //转换器

        public Bitmap textureBuffer;            //储存纹理图像
        public Bitmap frameBuffer;              //储存以及显示图像
        public Graphics Gframe;                 //frameBuffer的画板
        public float[,] zBuffer;                //z缓存
        //双缓冲区
        public BufferedGraphicsContext dc;
        public BufferedGraphics backBuffer;

        public Vectex[] mesh;                   //立方体顶点信息
        public float alpha;                     //旋转角

        public void InitDevice(int _width,int _height)
        {
            this.width = _width;
            this.height = _height;
            texWidth = 256;
            texHeight = 256;


            renderState = RenderState.COLOR;
            textureFilterMode = TextureFilterMode.POINT;
            lightMode = LightMode.ON;

            light = new Light(new Vector3(50, 0, 0, 1), new Color(0.3f, 0.3f, 0.3f), new Color(1, 1, 1), new Color(1, 1, 1));
            camera = new Camera(new Vector3(3.5f, 0, 0, 1), new Vector3(0, 0, 0, 1),new Vector3(0, 0, 1, 1), 
                (float)Math.PI*0.5f,((float)_width) / ((float)_height), 1f, 500f);
            ts = new Transform(_width, _height);

            if(renderState==RenderState.TEXTURE)
            {
                InitTexture(texWidth, texHeight);
            }
            //缓冲及画板
            frameBuffer = new Bitmap(_width, _height);
            Gframe = Graphics.FromImage(frameBuffer);
            zBuffer = new float[_width, _height];

            //双缓存区
            dc = new BufferedGraphicsContext();
            backBuffer = dc.Allocate(Gframe, new Rectangle(new Point(0, 0),new Size(800,600)));


            //输入立方体的信息，包括法向量
            mesh = new Vectex[8]
            {
                new Vectex(1,-1,1,1,0,0,1,0.2f,0.2f,1),
                new Vectex(-1,-1,1,1,0,1,0.2f,1f,0.2f,1),
                new Vectex(-1,1,1,1,1,1,0.2f,0.2f,1,1),
                new Vectex(1,1,1,1,1,0,1,0.2f,1,1),
                new Vectex(1,-1,-1,1,0,0,1,1,0.2f,1),
                new Vectex(-1,-1,-1,1,0,1,0.2f,1,1,1),
                new Vectex(-1,1,-1,1,1,1,1,0.3f,0.3f,1),
                new Vectex(1,1,-1,1,1,0,0.2f,1,0.3f,1),
            };
            for(int i=0;i<8;i++)
            {
                //直接将顶点的位置方向作为其法向方向。
                mesh[i].nomal = Vector3.Colone(mesh[i].pos);
                mesh[i].nomal.Normalized();
            }
            alpha = 1f;
        }

        public void InitTexture(int _width,int _height)
        {
            if (renderState == RenderState.TEXTURE)
            {
                Image image = Image.FromFile("../../Texture/texture.png");
                textureBuffer = new Bitmap(image, _width, _height);
            }
            else
            {
                textureBuffer = new Bitmap(_width, _height);
                for(int i=0;i<_width-1;i++)
                {
                    for(int j=0;j<_height-1;j++)
                    {
                        textureBuffer.SetPixel(i, j, System.Drawing.Color.Blue);
                    }
                }
            }

        }

        //初始化framebuffer
        public void ClearBuffer()
        {
            Gframe.Clear(System.Drawing.Color.Black);
            Array.Clear(zBuffer, 0, zBuffer.Length);
        }
        //没用这个
        public void MoveCamare(float rot)
        {
            camera.eye.x = rot;
        }
    }

    public struct Camera
    {
        public Vector3 eye,at,up;
        public float fovy, aspect, zn, zf;

        public Camera(Vector3 _eye,Vector3 _at,Vector3 _up,float _fovy,float _aspect,float _zn,float _zf)
        {
            this.eye = _eye;
            this.at = _at;
            this.up = _up;
            this.fovy = _fovy;
            this.aspect = _aspect;
            this.zn = _zn;
            this.zf = _zf;
        }
    }

    public struct Light
    {
        //环境光位置
        public Vector3 lightPosition;
        //材质漫反射颜色
        public Color diffuse;
        //RGB强度项(光源颜色）
        public Color sourse;
        //环境颜色
        public Color ambient;

        public Light(Vector3 _lightPosition,Color _diffuse,Color _sourse,Color _ambient)
        {
            //光源位置
            this.lightPosition = _lightPosition;

            this.diffuse = _diffuse;
            //白光
            this.sourse = _sourse;
            this.ambient = _ambient;
        }
    }
}
