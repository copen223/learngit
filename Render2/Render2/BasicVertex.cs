using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render2
{
    //顶点相关信息
    public class Vectex
    {
        public Vector3 pos;
        public Vector3 nomal;
        public TexturePosition uvs;
        public Color color;
        public Color lightcolor;
        public float rhw;

        public Vectex()
        {
            pos = new Vector3();
        }

        public Vectex(float x, float y, float z, float w, float u, float v, float r, float g, float b, float _rhw)
        {
            this.pos = new Vector3(x, y, z, w);
            this.uvs = new TexturePosition(u, v);
            this.color = new Color(r, g, b);
            this.rhw = _rhw;
        }

        public void SetNomal(float x,float y,float z,float w)
        {
            this.nomal.x = x;
            this.nomal.y = y;
            this.nomal.z = z;
            this.nomal.w = w;
        }

        public void InitRhw()
        {
            float _rhw = (float) 1/this.pos.w;
            this.rhw = _rhw;
            this.color *= _rhw;
            this.lightcolor *= rhw;
            this.uvs *= _rhw;
        }

        static public Vectex Colone(Vectex v1)
        {
            Vectex v2 = new Vectex();
            v2.pos = Vector3.Colone(v1.pos);
            v2.nomal = Vector3.Colone(v1.nomal);
            v2.uvs = v1.uvs;
            v2.color = v1.color;
            v2.lightcolor = v1.lightcolor;
            v2.rhw = v1.rhw;
            return v2;
        }

    }

    public class Vector3
    {
        public float x, y, z, w;

        public Vector3()
        {

        }

        public Vector3(float _x, float _y, float _z, float _w)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
            this.w = _w;
        }

        static public Vector3 Colone(Vector3 v1)
        {
            Vector3 v2 = new Vector3();
            v2.x = v1.x;
            v2.y = v1.y;
            v2.z = v1.z;
            v2.w = v1.w;
            return v2;
        }

        //单位化
        public void Normalized()
        {
            float length = (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            if (length == 0 || length == 1)
                return;
            float inv = (float)1 / length;
            this.x *= inv;
            this.y *= inv;
            this.z *= inv;
        }
        static public Vector3 Normalize(Vector3 v1)
        {
            Vector3 v2 = new Vector3();
            float length = (float)Math.Sqrt(v1.x * v1.x + v1.y * v1.y + v1.z * v1.z);
            if (length == 0)
                return v1;
            float inv = (float)1 / length;
            v2.x = v1.x * inv;
            v2.y = v1.y * inv;
            v2.z = v1.z * inv;
            return v2;
        }

        //在transform里归一化v1
        static public Vector3 Homogenize(Transform ts, Vector3 v1)
        {
            Vector3 v2 = new Vector3();
            float rhw = 1 / v1.w;
            v2.x = (v1.x * rhw + 1f) * ts.width * 0.5f;
            v2.y = (1f - v1.y * rhw) * ts.height * 0.5f;
            v2.z = v1.z * rhw;
            v2.w = 1f;
            return v2;
        }


        //计算
        static public Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            Vector3 v3 = new Vector3
            {
                x = v1.x + v2.x,
                y = v1.y + v2.y,
                z = v1.z + v2.z
            };
            return v3;
        }
        static public Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            Vector3 v3 = new Vector3
            {
                x = v1.x - v2.x,
                y = v1.y - v2.y,
                z = v1.z - v2.z
            };
            return v3;
        }
        static public float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        static public Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            Vector3 v3 = new Vector3
            {
                x = v1.y * v2.z - v1.z * v2.y,
                y = v1.z * v2.x - v1.x * v2.z,
                z = v1.x * v2.y - v1.y * v2.x
            };
            return v3;
        }

        //检测
        public bool CheckCVV()
        {
            if (this.x >= -this.w && this.x <= this.w && this.y >= -this.w && this.y <= this.w && this.z >= 0 && this.z <= w)
                return true;
            else
                return false;
        }
    }

    public struct TexturePosition
    {
        public float u, v;

        public TexturePosition(float _u, float _v)
        {
            this.u = _u;
            this.v = _v;
        }

        public static TexturePosition operator * (TexturePosition a,TexturePosition b)
        {
            TexturePosition c = new TexturePosition();
            c.u = a.u * b.u;
            c.v = a.v * b.v;
            return c;
        }
        public static TexturePosition operator *(float a, TexturePosition b)
        {
            TexturePosition c = new TexturePosition();
            c.u = a * b.u;
            c.v = a * b.v;
            return c;
        }
        public static TexturePosition operator *(TexturePosition a, float b)
        {
            TexturePosition c = new TexturePosition();
            c.u = a.u * b;
            c.v = a.v * b;
            return c;
        }
    }

    public struct Color
    {
        public float r, g, b;

        public Color(float _r, float _g, float _b)
        {
            this.r = _r;
            this.g = _g;
            this.b = _b;
        }
        public Color(System.Drawing.Color a)
        {
            this.r = NewMath.Round((float)a.R / 255f, 0, 1);
            this.g = NewMath.Round((float)a.G / 255f, 0, 1);
            this.b = NewMath.Round((float)a.B / 255f, 0, 1);
        }

        //颜色算术
        public static Color operator *(Color a, Color b)
        {
            Color c = new Color();
            c.r = a.r * b.r;
            c.g = a.g * b.g;
            c.b = a.b * b.b;
            return c;
        }
        public static Color operator *(float a, Color b)
        {
            Color c = new Color();
            c.r = a * b.r;
            c.g = a * b.g;
            c.b = a * b.b;
            return c;
        }
        public static Color operator *(Color a, float b)
        {
            Color c = new Color();
            c.r = a.r * b;
            c.g = a.g * b;
            c.b = a.b * b;
            return c;
        }
        public static Color operator +(Color a, Color b)
        {
            Color c = new Color();
            c.r = a.r + b.r;
            c.g = a.g + b.g;
            c.b = a.b + b.b;
            return c;
        }

        //颜色转换
        public static System.Drawing.Color SystemColor(Color a)
        {
            a.r = NewMath.Round(a.r, 0, 1);
            a.g = NewMath.Round(a.g, 0, 1);
            a.b = NewMath.Round(a.b, 0, 1);
            float _r = a.r * 255f;
            float _g = a.g * 255f;
            float _b = a.b * 255f;
            return System.Drawing.Color.FromArgb((int)_r, (int)_g, (int)_b);

        }
            

    }


}

