using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace Template
{
	class MyApplication
	{

		// member variables
		public Surface screen;
		List<Circle> light;
		List<Circle> primitives;
		Ray ray;

		// initialize
		public void Init()
		{
			light = new List<Circle>();
			primitives = new List<Circle>();

			light.Add(new Circle(0 , 0, 0.2f, true, new floatColour(0, 0, 0)));
			primitives.Add(new Circle(0 , 0, 0.2f, true, new floatColour(0, 0, 0)));
			primitives.Add(new Circle(0 , 0.5f, 0.2f, false, new floatColour(1, 1, 1)));

			ray = new Ray();
			for(int x = 0; x < 640; x++)
			{
				for(int y = 0; y < 400; y++)
				{
					floatColour pixelColour = new floatColour(0, 0, 0);
					foreach(Circle c in light)
					{
						ray.O = pixelPosition(x, y);
						ray.D = (new Vector2(c.x - ray.O.X, c.y - ray.O.Y)).Normalized();
						ray.t = Vector2.Distance(ray.O, new Vector2(c.x, c.y));

						bool occluded = false;
						foreach(Circle p in primitives)
						{
							if(ray.intersection(p)) occluded = true;
						}
						if(!occluded) {
							pixelColour += c.colour * lightAttenuation(Vector2.Distance(ray.O, new Vector2(c.x, c.y)));
						}
						screen.Plot(x, y, pixelColour.ToRGB32());
					}
				}
			}
		}
		// tick: renders one frame
		public void Tick()
		{

		}

		Vector2 pixelPosition(int x, int y)
		{
			float TX =  (x/640) * 2 - 1;
			float TY = (y/400) * -2 + 1;
			return new Vector2(TX, TY);
		}

		floatColour lightAttenuation(float dl) 
		{
			float i = 1/(dl* dl);
			return new floatColour(i, i, i);
		}
		
	}

	public class floatColour
	{
		public float r{get;}
		public float g{get;}
		public float b{get;}

		public floatColour(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public int ToRGB32()
		{
			return MixColor((int)(this.r * 255), (int)(this.g * 255), (int)(this.b * 255));
		}

		public int MixColor( int red, int green, int blue )
		{
			return (red << 16) + (green << 8) + blue;
		}

		public static floatColour operator *(floatColour c1, floatColour c2)
		{
			return new floatColour(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b);
		}

		public static floatColour operator +(floatColour c1, floatColour c2)
		{
			return new floatColour(c1.r + c2.r, c1.g + c2.g, c1.b + c2.b);
		}
	}



	class Circle
	{
		public bool light;
		public float x{get;}
		public float y{get;}
		public float r{get;}
		public floatColour colour{get;}

		public Circle(float x, float y, float r, bool light, floatColour colour)
		{
			this.x = x;
			this.y = y;
			this.r = r;
			this.light = light;
			this.colour = colour;
		}

		public float CircleArea()
		{
			return (float)Math.PI * (r * r);
		}
	}

	class Ray
	{
		public Vector2 O{get; set;}
		public Vector2 D{get; set;}
		public float t{get; set;}

		public bool intersection(Circle p)
		{
			Vector2 c = new Vector2(p.x, p.y) - O;
			float t = Vector2.Dot(c, D);
			Vector2 q = c - t * D;
			float qdot = Vector2.Dot(q, q);
			if(qdot > (p.r * p.r)) return false;
			t -= (float)Math.Sqrt(p.r * p.r - qdot);
			if((t < this.t) && (t > 0)) this.t = t;
			return true;
		}
	}
}