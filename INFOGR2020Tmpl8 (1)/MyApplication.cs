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
		List<Circle> circles;
		List<Square> squares;
		Ray ray;

		// initialize
		public void Init()
		{
			float[] aax = { 0, 0.5f, 0, 0.5f };
			float[] aay = { 0, 0, 0.5f, 0.5f };

			light = new List<Circle>();
			circles = new List<Circle>();
			squares = new List<Square>();

			circles.Add(new Circle(0.3f, 0.1f, 0.1f, false, new floatColour(0, 0, 0)));
			circles.Add(new Circle(-0.3f, 0.5f, 0.1f, false, new floatColour(0, 0, 0)));
			squares.Add(new Square(-0.3f, -0.3f, 0.2f, new floatColour(0, 0, 0)));
			light.Add(new Circle(0f, 0.5f, 0.25f, true, new floatColour(1, 0, 1)));
			light.Add(new Circle(-0.2f, 0.2f, 0.25f, true, new floatColour(0, 1, 0)));
			light.Add(new Circle(0.2f, -0.4f, 0.25f, true, new floatColour(0, 0, 1)));


			ray = new Ray();
			for(int x = 0; x < screen.width; x++)
			{
				for(int y = 0; y < screen.height; y++)
				{
					floatColour[] pixelColour = { new floatColour(0, 0, 0), new floatColour(0, 0, 0), new floatColour(0, 0, 0), new floatColour(0, 0, 0) };
					foreach(Circle c in light)
					{
						for(int k = 0; k < 4; k++)
						{
							ray.O = pixelPosition(x + aax[k], y + aay[k]);
							ray.D = (new Vector2(ray.O.X - c.x, ray.O.Y - c.y)).Normalized();
							ray.t = (float)Math.Sqrt(Math.Pow(ray.O.X - c.x, 2) + Math.Pow(ray.O.Y - c.y, 2));

							bool occluded = false;
							foreach (Circle p in circles)
							{
								if ((Math.Pow(ray.O.X - p.x, 2) + Math.Pow(ray.O.Y - p.y, 2)) > (p.r * p.r))
								{
									if (ray.intersection(p))
									{
										occluded = true;
										float tmp = (float)Math.Sqrt((Math.Pow(c.x - p.x, 2) + Math.Pow(c.y - p.y, 2)));
										if (ray.t < tmp && (ray.t > 0 && tmp > 0)) occluded = false;
										tmp = (float)Math.Sqrt((Math.Pow(ray.O.X - p.x, 2) + Math.Pow(ray.O.Y - p.y, 2)));
										if (ray.t < tmp) occluded = false;
									}
								}
								else
								{
									occluded = true;
									pixelColour[k] = new floatColour(0, 0, 0);
								}
							}
							foreach (Square p in squares)
							{
								if (ray.intersection2(p))
								{
									occluded = true;
									float tmp = (float)Math.Sqrt((Math.Pow(ray.O.X - (p.x + p.s), 2) + Math.Pow(ray.O.Y - (p.y + p.s), 2)));
									if (ray.t < tmp) occluded = false;
								}
							}
							if (!occluded)
							{
								pixelColour[k] += c.colour * lightAttenuation(Vector2.Distance(ray.O, new Vector2(c.x, c.y)), c.r);
							}

						}
		
					}
					screen.Plot(x, y, ((pixelColour[0] + pixelColour[1] + pixelColour[2] + pixelColour[3]) / 4.0f).ToRGB32());
				}
			}

		}
		// tick: renders one frame
		public void Tick()
		{

		}
		Vector2 pixelPosition(float x, float y)
		{
			float TX =  ((float)x/screen.width) * 2.0f - 1.0f;
			float TY = ((float)y/screen.height) * -2.0f + 1.0f;
			return new Vector2(TX, TY);
		}

		floatColour lightAttenuation(float dl, float L) 
		{
			float i =  Clamp((L/2)/dl, 0f, 1f);
			return new floatColour(i, i, i);
		}
		public static float Clamp(float value, float min, float max)  
			{  
				return (value < min) ? min : (value > max) ? max : value;  
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
			red = Math.Min(red, 255);
			green = Math.Min(green, 255);
			blue = Math.Min(blue, 255);
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

		public static floatColour operator /(floatColour c1, float n)
		{
			return new floatColour(c1.r / n, c1.g / n, c1.b / n);
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
    }

	class Square
	{
		public float x { get; }
		public float y { get; }
		public float s { get; }
		public floatColour colour { get; }

		public Square(float x, float y, float s, floatColour colour)
		{
			this.x = x;
			this.y = y;
			this.s = s;
			this.colour = colour;
		}

	}

	class Ray
	{
		public Vector2 O{get; set;}
		public Vector2 D{get; set;}
		public float t{get; set;}

		public bool intersection(Circle p)
		{
			float a = Vector2.Dot(D, D);
			float b = Vector2.Dot(2 * D, O - new Vector2(p.x, p.y));
			float c = Vector2.Dot(O - new Vector2(p.x, p.y), (O - new Vector2(p.x, p.y))) - (p.r * p.r);
			if((b * b - 4 * a * c) < 0)return false;
			return true;
		}

		public bool intersection2(Square p)
		{
			float t1 = (p.x - O.X) / D.X;
			float t2 = ((p.x + p.s) - O.X) / D.X;
			float t3 = (p.y - O.Y) / D.Y;
			float t4 = ((p.y + p.s) - O.Y) / D.Y;

			float tmin = Math.Max(Math.Min(t1, t2), Math.Min(t3, t4));
			float tmax = Math.Min(Math.Max(t1, t2), Math.Max(t3, t4));

			// if tmax < 0, ray (line) is intersecting AABB, but the whole AABB is behind us
			if (tmax > 0 && tmax < tmax + tmin)
			{
				return false;
			}
			// if tmin > tmax, ray doesn't intersect AABB
			if (tmin > tmax)
			{
				return false;
			}

			return true;
		}
	}
}