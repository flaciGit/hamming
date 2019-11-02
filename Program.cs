using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace hamming
{
    class Program
    {

        static int length;
        static List<int[]> positionList = new List<int[]>();

        static byte[] Encode(byte[] code)
        {

            string enc = "";

            int kitevo = 2, j = 0;
            enc = enc+"--";
            int offset = 3;

            while (j < code.Length)
            {
                
                if (j == Math.Pow(2, kitevo)-offset)
                {
                    enc += "-";
                    kitevo++;
                    offset++;
                }

                enc += code[j];

                j++;
            }

            Console.WriteLine("\nData bits and parity bits positions:\n" + enc);
            positionList = new List<int[]>();

            byte[] encoded = StringToByteArray(enc);
            length = encoded.Length;
            int pos = 1;
            while (pos < length)
            {
                encoded[pos - 1] = doXoringForPosition(encoded, length, pos);

                pos *= 2;

            }

            listUsedPositions(encoded);

            return encoded;
        }

        static void listUsedPositions(byte[] encoded) {

            int kitevo = 0;

            for(int k = 0; k < positionList.Count; k++)
            {
                int[] positions = positionList[k];
                Console.Write("\nPositions for the calculation: ");
                
                foreach (int i in positions)
                {
                    Console.Write(" "+i);
                }
                Console.Write("\n");
                for (int i = 0; i < encoded.Length; i++)
                {
                    if (IsPowerOfTwo(i+1) && i >= Math.Pow(2,k)) {
                        Console.Write(" _");
                    }
                    else
                        Console.Write(" " + encoded[i]);
                }
                Console.Write("\n");
                for (int i = 0; i < encoded.Length; i++)
                {
                    if (i == Math.Pow(2, kitevo)-1)
                    {
                        Console.Write(" ^");

                    }
                    else {

                        if (positions.Contains(i + 1))
                            Console.Write(" |");
                        else
                            Console.Write("  ");
                    }
                    
                }
                Console.Write("\n");
                kitevo++;
            }
            

        }

        static byte[] Decode(byte[] code)
        {
            string dec = "";

            int kitevo = 0, j = 0;

            while (j < code.Length)
            {

                if (j == Math.Pow(2, kitevo)-1)
                {
                    kitevo++;
                }else
                    dec += code[j];

                j++;
            }
            
            byte[] decoded = StringToByteArray(dec);
    
            return decoded;
        }

        static bool StringIsBinary(string input) {
            int output;
            for (int i = 0; i < input.Length; i++) {
                if (!int.TryParse(input[i].ToString(), out output) || output < 0 || output > 1)
                    return false;
            }
            return true;
        }

        static int GetErrorPosition(byte[] encoded)
        {
            string errorPosString = "";
            int syndrome = 0;
            int pow = 0;
            double pos = 1;
            int db = 0;
            Console.WriteLine("\nCALCULATED  ^   FOUND       =   ERROR\n");
            length = encoded.Length;
            while (pos < length) {
                syndrome += (Convert.ToInt32(doXoringForPosition(encoded, length, (int)pos) ^ encoded[(int)pos-1]) << db);

                byte x = doXoringForPosition(encoded, length, (int)pos);
                byte y = encoded[(int)pos - 1];
                Console.WriteLine("\n     "+x+"      ^     "+y +"         =     " +(x^y));
                errorPosString += (x^y)+" ";
                pow++;
                db++;
                pos = Math.Pow(2, pow);
                
            }
            
            Console.WriteLine("\nError position in binary: "+ ReverseString(errorPosString));

            return syndrome;
        }

        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        static void FlipByteAtPos(byte[] encoded, int pos)
        {
            encoded[pos] = (byte)((encoded[pos]+1)%2);
        }

        static byte[] StringToByteArray(string input) {
            byte[] output = new byte[input.Length];
            int i = 0;
            foreach (char s in input) {
                Byte.TryParse(s.ToString(),out output[i]);
                i++;
            }
            return output;
        }

        static string ByteArrayToString(byte[] input) {
            string output = "";
            foreach (byte b in input) {
                output += b;
            }
            return output;
        }
        
        public static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public static int[] getPositionsForXoring(int length, int currentHammingPosition)
        {
            
            var positions = new List<int>();
            for (int i = 1; i <= length; i++)
            {
                if ((i & currentHammingPosition) > 0 && !IsPowerOfTwo(i)) {
                    positions.Add(i);
                }
                
            }
            positionList.Add(positions.ToArray());
            return positions.ToArray();
        }

        public static byte doXoringForPosition(byte[] vector, int length, int currentHammingPosition)
        {
            return getPositionsForXoring(length, currentHammingPosition)
                .Select(x => vector[x - 1])
                .Aggregate((x, y) => (byte)(x ^ y));
        }

        public static bool ArraysAreEqual(byte[] a, byte[] b) {

            if (a.Length != b.Length) return false;
            for(int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        public static int getHammingWeight(byte[] input) {
            int db = 0;
            foreach (byte b in input)
                if (b == (byte)1)
                    db++;

            return db;
        }

        static void Main(string[] args)
        {
            byte[] encoded = null;
            int errorPosition = -1;
            byte[] code;


            Console.WriteLine("Error correction with Hamming code");
            Console.WriteLine("(One error correctable)\n");

            Console.WriteLine("Choose input mode:");
            Console.WriteLine(" To enter input data to encode: a");
            Console.WriteLine(" To enter received hamming code: b");

            string input = Console.ReadLine();

            while (input[0] != 'a' && input[0] != 'b')
            {
                Console.WriteLine("Choose input mode:");
                Console.WriteLine(" To enter input data to encode: a");
                Console.WriteLine(" To enter received hamming code: b");

                input = Console.ReadLine();
            }

            if (input[0] == 'a')
            {
                
                Console.WriteLine("\nGive code to encode:");

                string codeString = Console.ReadLine();

                while (!StringIsBinary(codeString))
                {
                    Console.WriteLine("\nGive code to encode:");
                    codeString = Console.ReadLine();
                }
                
                code = StringToByteArray(codeString);
                encoded = Encode(code);

                Console.WriteLine("\nEncoded format:");
                Console.WriteLine(ByteArrayToString(encoded));

                Console.WriteLine("\nGive an error bit position: (1-"+ encoded.GetLength(0)+ ")");
                input = Console.ReadLine();

                while (!int.TryParse(input, out errorPosition) || errorPosition < 1 || errorPosition > encoded.GetLength(0))
                {

                    Console.WriteLine("\nGive an error bit position: (1-" + encoded.GetLength(0) + ")");
                    input = Console.ReadLine();
                }
                
                Console.WriteLine("\nError given to the encoded data at the position: " + errorPosition);
                errorPosition--;
                FlipByteAtPos(encoded, errorPosition);
                Console.WriteLine(ByteArrayToString(encoded));

                length = encoded.Length;
                int foundErrorPosition = GetErrorPosition(encoded);
                Console.WriteLine("\nFound error position: " + foundErrorPosition);

                Console.WriteLine("\nAfter correction:");
                if (foundErrorPosition - 1 >= 0)
                    FlipByteAtPos(encoded, foundErrorPosition-1);
                Console.WriteLine(ByteArrayToString(encoded));

                Console.WriteLine("\nCorrected data decoded:");
                var decoded = Decode(encoded);
                Console.WriteLine(ByteArrayToString(decoded));

                if (ArraysAreEqual(code, decoded))
                    Console.WriteLine("\nThe input data and the decoded data are equal.");
                else
                    Console.WriteLine("\nThe input data and the decoded data are NOT equal.");
                
            }
            else {

                Console.WriteLine("\nGive the received hamming code:");

                string codeString = Console.ReadLine();

                while (!StringIsBinary(codeString))
                {
                    Console.WriteLine("\nGive the received hamming code:");
                    codeString = Console.ReadLine();
                }

                encoded = StringToByteArray(codeString);

                Console.WriteLine("\nWould you like to add an error? (y/n)");
                input = Console.ReadLine();

                while (input[0] != 'y' && input[0] != 'n' )
                {

                    Console.WriteLine("\nWould you like to add an error? (y/n)");
                    input = Console.ReadLine();
                }

                if (input[0] == 'y')
                {

                    Console.WriteLine("\nGive an error bit position: (1-" + encoded.GetLength(0) + ")");
                    input = Console.ReadLine();

                    while (!int.TryParse(input, out errorPosition) || errorPosition < 1 || errorPosition > encoded.GetLength(0))
                    {

                        Console.WriteLine("\nGive an error bit position: (1-" + encoded.GetLength(0) + ")");
                        input = Console.ReadLine();
                    }

                    Console.WriteLine("\nError given to the encoded data at the position: " + errorPosition);
                    errorPosition--;
                    FlipByteAtPos(encoded, errorPosition);
                    Console.WriteLine(ByteArrayToString(encoded));
                }
                
                int foundErrorPosition = GetErrorPosition(encoded);
                if(foundErrorPosition > 0)
                    Console.WriteLine("\nFound error position: " + foundErrorPosition);
                else
                {
                    Console.WriteLine("\nThere was no error");
                }

                Console.WriteLine("\nAfter correction:");
                if(foundErrorPosition - 1 >= 0)
                    FlipByteAtPos(encoded, foundErrorPosition-1);
                Console.WriteLine(ByteArrayToString(encoded));

                Console.WriteLine("\nCorrected data decoded:");
                var decoded = Decode(encoded);
                Console.WriteLine(ByteArrayToString(decoded));

            }
            
            Console.WriteLine("\n\nEnd");

            Console.ReadLine();
        }
    }
}