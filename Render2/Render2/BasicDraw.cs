using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render2
{
    public class Drawing
    {
        public Device device;
        public Drawing(Device _device)
        {
            this.device = _device;
        }

        public void DrawBox(float theta)
        {
            //设置ts
            device.ts.world.SetRotate(-1, -0.5f, 1, theta);
            device.ts.view.SetLookat(device.camera.eye, device.camera.at, device.camera.up);
            device.ts.projection.SetPerspective(device.camera.fovy, device.camera.aspect, device.camera.zn, device.camera.zf);
            device.ts.SetTransform();

            //
            DrawPlane(0, 1, 2, 3);
            DrawPlane(7, 6, 5, 4);
            DrawPlane(0, 4, 5, 1);
            DrawPlane(1, 5, 6, 2);
            DrawPlane(2, 6, 7, 3);
            DrawPlane(3, 7, 4, 0);
            //device.backBuffer.Render(Graphics.FromImage(device.frameBuffer));
        }
        public void DrawPlane(int a,int b,int c,int d)
        {
            Vectex v1 = device.mesh[a], v2 = device.mesh[b], 
                v3 = device.mesh[c], v4 = device.mesh[d];
            v1.uvs.u = 0; v1.uvs.v = 0; v2.uvs.u = 0; v2.uvs.v = 1;
            v3.uvs.u = 1; v3.uvs.v = 1; v4.uvs.u = 1; v4.uvs.v = 0;
            DrawTriangle(v1, v2, v3);
            DrawTriangle(v3, v4, v1);
        }

        public void DrawTriangle(Vectex v1,Vectex v2, Vectex v3)
        {
            Vector3 p1, p2, p3, c1, c2, c3;

            //背面剔除 2 3
            p1 = v1.pos * (device.ts.world * device.ts.view);
            p2 = v2.pos * (device.ts.world * device.ts.view);
            p3 = v3.pos * (device.ts.world * device.ts.view);

            if(device.renderState!=RenderState.WIRE)
            {
                if (!CheckIsFront(p1, p2, p3))
                    return;
            }

            //设置顶点光照颜色 4
            if(device.lightMode==LightMode.ON)
            {
                SetLightingColor(v1);
                SetLightingColor(v2);
                SetLightingColor(v3);
            }


            //坐标转换 123
            p1 = v1.pos * device.ts.transform;
            p2 = v2.pos * device.ts.transform;
            p3 = v3.pos * device.ts.transform;
            //cvv裁剪 123
            if (!p1.CheckCVV() || !p2.CheckCVV() || !p3.CheckCVV())
            {
                return;
            }
            //归一化得到屏幕坐标 123
            c1 = Vector3.Homogenize(device.ts, p1);
            c2 = Vector3.Homogenize(device.ts, p2);
            c3 = Vector3.Homogenize(device.ts, p3);

            //坐标转换完毕 c为屏幕坐标,顶点其他信息来自v
            //开始渲染

            //初始化渲染顶点信息 123
            Vectex d1, d2, d3;
            //顶点的color等信息来自v
            d1 = Vectex.Colone(v1);
            d2 = Vectex.Colone(v2);
            d3 = Vectex.Colone(v3);
            //顶点位置信息来自最终屏幕坐标c
            d1.pos = Vector3.Colone(c1);
            d2.pos = Vector3.Colone(c2);
            d3.pos = Vector3.Colone(c3);
            //顶点w信息来自未归一化的p，w代表原z值
            d1.pos.w = p1.w;
            d2.pos.w = p2.w;
            d3.pos.w = p3.w;
            //初始化顶点的rhw数据并用此数据加工颜色，uv数据,为后续顶点颜色的深度插值作准备
            d1.InitRhw();
            d2.InitRhw();
            d3.InitRhw();

            //线框模式 1
            if (device.renderState==RenderState.WIRE)
            {
                DrawLine(d1, d2);
                DrawLine(d1, d3);
                DrawLine(d3, d2);
            }
            else
            {
                //color2
                Rasterization(d1, d2, d3);
            }

        }
        //线框画直线
        public void DrawLine(Vectex v1,Vectex v2)
        {
            
            
            //两点坐标
            int x1 = (int)(v1.pos.x + 0.5f), x2 = (int)(v2.pos.x + 0.5f);
            int y1 = (int)(v1.pos.y + 0.5f), y2 = (int)(v2.pos.y + 0.5f);
            //两点范围
            float  dx = v2.pos.x - v1.pos.x;
            float  dy = v2.pos.y - v1.pos.y;
            float adx = dx;float ady = dy;
            //画点坐标
            float x = x1,y = y1;
            int xIndex = x1,yIndex = y1;
            
            //递进
            int stepx, stepy;
            if (dx > 0)
                stepx = 1;
            else
            {
                stepx = -1;
                adx = v1.pos.x - v2.pos.x;
            }
            if (dy > 0)
                stepy = 1;
            else
            {
                stepy = -1;
                ady = v1.pos.y - v2.pos.y;
            }
            //按照斜率算出对应位置
            //开始画点
            if((adx > ady))
            {
                //沿x方向画点
                for (int i = 0; i < adx; i++)
                {
                    device.frameBuffer.SetPixel(xIndex, yIndex, System.Drawing.Color.White);
                    y += stepx * (dy / dx);
                    yIndex = (int)(y + 0.5f);
                    xIndex += stepx;
                }

            }
            else
            {
                //沿y方向
                for (int i = 0; i < ady; i++)
                {
                    device.frameBuffer.SetPixel(xIndex, yIndex, System.Drawing.Color.White);
                    x += stepy * (dx / dy);
                    xIndex = (int)(x + 0.5f);
                    yIndex += stepy;
                }
            }


        }
        //光栅化开始
        public void Rasterization(Vectex v1,Vectex v2,Vectex v3)
        {
            //保护?
            if (v1.pos.y == v2.pos.y && v1.pos.y == v3.pos.y)
                return;
            //交换v1,v2,v3使纵向1<2<3
            if (v1.pos.y > v2.pos.y)
            {
                Vectex v = Vectex.Colone(v2);
                v2 = Vectex.Colone(v1);
                v1 = Vectex.Colone(v);
            }
            if (v1.pos.y > v3.pos.y)
            {
                Vectex v = Vectex.Colone(v3);
                v3 = Vectex.Colone(v1);
                v1 = Vectex.Colone(v);
            }
            if (v2.pos.y > v3.pos.y)
            {
                Vectex v = Vectex.Colone(v3);
                v3 = Vectex.Colone(v2);
                v2 = Vectex.Colone(v);
            }

            //初步分析
            if(v1.pos.y == v2.pos.y)
            {
                DrawTriangleTop(v1, v2, v3);
            }
            else if(v2.pos.y==v3.pos.y)
            {
                DrawTriangleBottom(v2, v3, v1);
            }
            else//分割三角形
            {
                //重点在于找到13边上的中间点 作为新顶点
                Vectex newMiddle = new Vectex();
                float x1 = v1.pos.x;
                float y1 = v1.pos.y;
                float dx = v3.pos.x - v1.pos.x;
                float dy = v3.pos.y - v1.pos.y;
                //确定纵坐标
                float y = v2.pos.y;
                //进行插值
                float dy2 = y - v1.pos.y;
                float t = dy2 / dy;
                newMiddle = NewMath.Interp(v1, v3, t);
                //来吧
                DrawTriangleBottom(newMiddle, v2, v1);
                DrawTriangleTop(newMiddle, v2, v3);
            }
        }
        public void DrawTriangleTop(Vectex v1,Vectex v2,Vectex v3)
        {
            //先确定好形状
            if(v1.pos.x > v2.pos.x)
            {
                Vectex p;
                p = Vectex.Colone(v1);
                v1 = Vectex.Colone(v2);
                v2 = Vectex.Colone(p);
            }

            float y, dy,dy2,t;
            int yIndex,stepy;

            stepy = 1;
            dy = v3.pos.y - v1.pos.y;
            
            y = v1.pos.y;
            yIndex = (int)(y + 0.5f);

            for (; yIndex < v3.pos.y; yIndex += stepy)
            {
                //求该y值处的左右顶点
                dy2 = yIndex - v1.pos.y;
                t = dy2 / dy;
                Vectex left, right;
                left = NewMath.Interp(v1, v3, t);
                right = NewMath.Interp(v2, v3, t);
                DrawScanline(left, right, yIndex);
            }

        }
        public void DrawTriangleBottom(Vectex v1, Vectex v2, Vectex v3)
        {
             //先确定好形状
            if(v1.pos.x > v2.pos.x)
            {
                Vectex p;
                p = Vectex.Colone(v1);
                v1 = Vectex.Colone(v2);
                v2 = Vectex.Colone(p);
            }

            float y, dy,dy2,t;
            int yIndex,stepy;

            stepy = -1;
            dy = v1.pos.y - v3.pos.y;
            
            y = v1.pos.y;
            yIndex = (int)y;

            for (; yIndex > v3.pos.y; yIndex += stepy)
            {
                //求该y值处的左右顶点
                dy2 = yIndex - v3.pos.y;
                t = dy2 / dy;
                Vectex left, right;
                left = NewMath.Interp(v3, v1, t);
                right = NewMath.Interp(v3, v2, t);
                DrawScanline(left, right, yIndex);
            }
        }
        //扫描填色
        public void DrawScanline(Vectex v1,Vectex v2, int yIndex)
        {
            float x1 = v1.pos.x;
            float x2 = v2.pos.x;
            float dx = v2.pos.x - v1.pos.x;
            float x = x1;
            //扫描密度，可以减小
            float stepx = 0.5f;
            int xIndex = (int)(x + 0.5f);

            for(;x <= x2 && xIndex<=x2;x+=stepx)
            {
                //插值当前点
                Vectex p;
                float t = (x-x1) / dx;
                p = NewMath.Interp(v1, v2, t);
                //画点坐标
                xIndex = (int)(x + 0.5f);
                if (xIndex >= device.width || xIndex <= 0 || yIndex <= 0 || yIndex >= device.height)
                    return;

                if(p.rhw>=device.zBuffer[xIndex,yIndex])
                {
                    device.zBuffer[xIndex, yIndex] = p.rhw;
                    float w = 1 / p.rhw;
                    

                    if(device.lightMode==LightMode.ON)
                    {
                        Color lighlingColor = new Color(1, 1, 1);
                        lighlingColor = w * p.lightcolor;
                        if(device.renderState==RenderState.COLOR)
                        {
                            Color vertColor = new Color(1, 1, 1);
                            vertColor = w * p.color;
                            Color newColor =NewMath.Round( 0.7f*vertColor + 0.3f* lighlingColor,0,1);
                            device.frameBuffer.SetPixel(xIndex, yIndex, Color.SystemColor(newColor));
                            
                        }
                        if(device.renderState==RenderState.TEXTURE)
                        {
                            int uIndex;
                            int vIndex;
                            //目前只有点插值
                            int u = (int)(p.uvs.u * w * device.texWidth + 0.5f);
                            int v = (int)(p.uvs.v * w * device.texHeight + 0.5f);
                            uIndex = (int)NewMath.Round(u, 0, device.texWidth - 1);
                            vIndex = (int)NewMath.Round(v, 0, device.texHeight - 1);
                            //获取颜色
                            Color texColor = new Color(1, 1, 1);
                            texColor = new Color(device.textureBuffer.GetPixel(uIndex, vIndex));
                            Color newColor = NewMath.Round(0.7f*texColor + 0.3f * lighlingColor, 0, 1);
                            //画像素点
                            device.frameBuffer.SetPixel(xIndex, yIndex, Color.SystemColor(newColor));

                        }
                    }
                    else
                    {
                        if(device.renderState==RenderState.COLOR)
                        {
                            //用顶点颜色插值
                            Color vertColor = new Color(1, 1, 1);
                            vertColor = w * p.color;
                            device.frameBuffer.SetPixel(xIndex, yIndex, Color.SystemColor(vertColor));
                        }
                        if (device.renderState == RenderState.TEXTURE)
                        {
                            int uIndex;
                            int vIndex;
                            //目前只有点插值
                            int u = (int)(p.uvs.u * w * device.texWidth + 0.5f);
                            int v = (int)(p.uvs.v * w * device.texHeight + 0.5f);
                            uIndex = (int)NewMath.Round(u, 0, device.texWidth - 1);
                            vIndex = (int)NewMath.Round(v, 0, device.texHeight - 1);
                            //获取颜色
                            Color texColor = new Color(1, 1, 1);
                            texColor = new Color(device.textureBuffer.GetPixel(uIndex, vIndex));
                            //画像素点
                            device.frameBuffer.SetPixel(xIndex, yIndex, Color.SystemColor(texColor));
                        }
                    }
                }

            }
        }
        
        //背面剔除
        public bool CheckIsFront(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            //计算 n为逆时针为正面时的法向量的反向
            Vector3 p1, p2;
            p1 = v2 - v1;
            p2 = v3 - v2;
            Vector3 n = Vector3.Cross(p1, p2);
            //用p1点位置来判断是否正面
            //-n是面向外的向量，也就是摄像机在此方向时可以看到该片元
            //所以应该是n与v1点积大于零则显示该片元

            float m = Vector3.Dot(n, v1);

            if (m > 0)
            {
                return true;
            }
            else
                return false;


        }

        //光照相关
        public void SetLightingColor(Vectex v1)
        {
            //将顶点转换到世界空间中
            Vectex v = Vectex.Colone(v1);
            //求世界空间中的法向量
            Vector3 n = v.nomal * device.ts.world.Inverse().Transpose();
            //光线向量
            Vector3 l = device.light.lightPosition - v.pos;
            //公式 c=cr(ca+cl*n*l)
            float t = NewMath.Round(Vector3.Dot(n, l),0,1);
            Color c = device.light.diffuse * (device.light.ambient + device.light.sourse * t);
            c = NewMath.Round(c, 0, 1);

            v1.lightcolor = c;
        }
    }
}
