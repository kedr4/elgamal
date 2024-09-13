using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfInformation.lab3.Service
{
    public static class CustomMath
    {
        public static bool IsPrime(uint number)
        {
            if (number < 2) 
	            return false;
            if (number % 2 == 0) 
	            return number == 2;
            uint root = (uint) Math.Sqrt(number);
            for (int i = 3; i <= root; i++)
            {
                if (number % i == 0) 
	                return false;
            }
            return true;
        }

		public static HashSet<uint> GetMuls(uint x)
		{
			HashSet<uint> set = new HashSet<uint>();
			for (uint i = 2; i < x; )
            {
				if (x % i == 0)
                {
					set.Add(i);
					x /= i;
                }
                else
                {
					i++;
                }
            }
			set.Add(x);
			return set;
        }

		public static ulong Eyler(ulong n)
		{
			ulong res = n;
			ulong en = Convert.ToUInt64(Math.Sqrt(n) + 1);
			for (ulong i = 2; i <= en; i++)
				if ((n % i) == 0)
				{
					while ((n % i) == 0)
						n /= i;
					res -= (res / i);
				}
			if (n > 1) 
				res -= (res / n);
			return res;
		}

		public static List<uint> GetRoots(uint p)
        {
			uint euler = (uint) Eyler(p);

			var muls = GetMuls(euler);
			List<uint> roots = new List<uint>((int)(euler / 2));

			for (uint g = 1; g < p; g++)
			{
				if (FastPower(g, euler, p) != 1) continue;
				bool breakMain = false;
				
				foreach (var mul in muls)
				{
					if (FastPower(g, euler / mul, p) == 1)
					{
						breakMain = true;
						break;
					}
				}

				if (!breakMain)
					roots.Add(g);
			}
			return roots;
        }

		public static uint FastPower(uint a, uint n, uint m)
		{
			// if (n == 0)
			// 	return 1 % m;
			// if (n % 2 == 1)
			// 	return (FastPower(a, n - 1, m) * a) % m;
			// else
			// 	return FastPower((a * a) % m, n / 2, m);
			uint x = 1;
			while (n != 0)
			{
				while (n % 2 == 0)
				{
					n /= 2;
					a = a * a % m;
				}
				n--;
				x = x * a % m;
			}
			return x;
		}

		public static (uint, uint, uint) gcd_ext(uint a, uint b)
		{
			uint d0 = a;
			uint d1 = b;
			uint x0 = 1;
			uint x1 = 0;
			uint y0 = 0;
			uint y1 = 1;
			while (d1 > 1)
			{
				uint q = d0 / d1;
				uint d2 = d0 % d1;
				uint x2 = x0 - q * x1;
				uint y2 = y0 - q * y1;
				d0 = d1;
				d1 = d2;
				x0 = x1;
				x1 = x2;
				y0 = y1;
				y1 = y2;
			}
			return (x1, y1, d1);
		}

        public static uint gcd(uint a, uint b)
        {
            while (a != 0 && b != 0)
                if (a > b)
                    a %= b;
                else
                    b %= a;
            return a | b;
        }
    }
}
