using System;
using System.Collections.Generic;
using System.Linq;
using static TheoryOfInformation.lab3.Service.CustomMath;

namespace TheoryOfInformation.lab3.Encryption
{
    public class ElGamal
    {
        public uint P { get; }
        public uint X { get; }
        public uint K { get; }
        public uint Y { get; private set; }
        public uint G { get; private set; }

        public byte Resize { get; }

        public IReadOnlyList<uint> Roots { get; }

        public ElGamal(uint p, uint x, uint k)
        {
            if (!IsPrime(p)) 
                throw new Exception($"p = {p} is not prime");
            else if (x >= p - 1 || x < 2) 
                throw new Exception($"x = {x} is not in range (1, {p-1})");
            else if (k >= p - 1 || k < 2 || gcd(k, p - 1) != 1) 
                throw new Exception($"k = {k} is not in range (1, {p-1}) or is not relatively prime with {p-1}");

            P = p;
            X = x;
            K = k;

            uint pCopy = p;
            while (pCopy > 0)
            {
                Resize++;
                pCopy >>= 8;
            }

            Roots = GetRoots(p);
            ChangeG(0);
        }

        public void ChangeG(int index)
        {
            index = index < Roots.Count ? index : Roots.Count;
            G = Roots[index];
            Y = FastPower(G, X, P);
        }
        public List<uint> Decrypt(List<uint> file)
        {
            uint[] result = new uint[file.Count >> 1];
            uint power = X * (uint)(Eyler(P) - 1);

            for (int i = 0; i < result.Length; i++)
            {
                ulong fileSeg = file[(i << 1) + 1] * FastPower(file[i << 1], power, P) % P;
                result[i] = (uint)fileSeg;
            }

            return result.ToList();
        }

        public List<uint> Encrypt(List<uint> file)
        {
            Random r = new Random();
            uint a = FastPower(G, K, P);
            uint mul = FastPower(Y, K, P);
            uint k = (uint) r.Next(2, (int)P - 2);
            
            uint[] result = new uint[file.Count<<1];
            for(int i = 0; i < file.Count; i++)
            {
                uint b = (mul * file[i]) % P;
                result[i << 1] = a;
                result[(i << 1) + 1] = b;
                
                while ((K + k) % (P - 1) >= P - 1 || (K + k) % (P - 1) < 2 || gcd((K + k) % (P - 1), P - 1) != 1)
                {
                    k = (uint) r.Next(2, (int)P - 2);
                }
                    
                a = FastPower(G, K + k, P);
                mul = FastPower(Y, K + k, P);
            }
            
            return result.ToList();
        }
    }
}
