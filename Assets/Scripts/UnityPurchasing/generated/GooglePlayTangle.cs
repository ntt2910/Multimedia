// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("k8KkUYBBxz8telqNkxIwdIKdW22NpnfKv6MtiBadf3q01RGN3MzBewPksWRdlT5cgUbCNqMCB6jbBcCDmRoUGyuZGhEZmRoaG5zx1ve+aqZfJZL433GUX0y9mmcFsD2ayxFUFCuZGjkrFh0SMZ1TnewWGhoaHhsYmhLFCQ9MLI0oqrCeDUGEgT2qtTK0W3B/u4kXB7c4OZ9vq6dlAQPX1L7WlXk8ij0E+VD0A8zYnCekKjy02tk+4vxWW0LFn26gsIvla1q+GyQOoPF2B2tpTY7/pTyREf4dr3QBNveFClEJlmYXsyarUxmyWxLAD/Ts4urBkXNWh0x9heq8f6ffAhptYRnF3fz6nQfe7mH1MX2afASOIEemT8GbiU/+8adXdhkYGhsa");
        private static int[] order = new int[] { 10,6,3,6,9,10,7,10,12,11,10,13,13,13,14 };
        private static int key = 27;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
