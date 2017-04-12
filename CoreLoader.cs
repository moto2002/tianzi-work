using System;
using System.Reflection;
using System.Security.Cryptography;

public class CoreLoader
{
    private static Assembly DecodeDllFile(byte[] CodeContext)
    {
        if ((CodeContext == null) || (CodeContext.Length < 4))
        {
            return null;
        }
        int length = CodeContext[0];
        length += CodeContext[1] << 8;
        length += CodeContext[2] << 0x10;
        length += CodeContext[3] << 0x18;
        byte[] destinationArray = new byte[length];
        Array.Copy(CodeContext, 9, destinationArray, 0, length);
        int num2 = CodeContext[4];
        num2 += CodeContext[5] << 8;
        num2 += CodeContext[6] << 0x10;
        num2 += CodeContext[7] << 0x18;
        byte bExType = CodeContext[8];
        byte[] buffer2 = new byte[num2];
        Array.Copy(CodeContext, 9 + length, buffer2, 0, num2);
        if (!RecoverCode(bExType, ref buffer2))
        {
            return null;
        }
        byte[] buffer3 = GetMd5Bytes(buffer2);
        if (((buffer3 == null) || (destinationArray == null)) || (buffer3.Length != destinationArray.Length))
        {
            return null;
        }
        for (int i = 0; i < buffer3.Length; i++)
        {
            if (buffer3[i] != destinationArray[i])
            {
                return null;
            }
        }
        return Assembly.Load(buffer2);
    }

    private static bool ExchangeContext0(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext1(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext10(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext11(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext12(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext13(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext14(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext15(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext16(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext17(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext18(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext19(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext2(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext20(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext21(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext22(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext23(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext24(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext25(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext26(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext27(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext28(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext29(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext3(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext4(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext5(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext6(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext7(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext8(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static bool ExchangeContext9(ref byte[] context)
    {
        if ((context == null) || (context.Length < 1))
        {
            return false;
        }
        int index = 0;
        int num2 = context.Length - 1;
        while (true)
        {
            if (index >= num2)
            {
                break;
            }
            byte num3 = context[num2];
            context[num2] = context[index];
            context[index] = num3;
            index++;
            num2--;
        }
        return true;
    }

    private static string GetMd5(byte[] bytes)
    {
        byte[] buffer = MD5.Create().ComputeHash(bytes);
        string str = string.Empty;
        for (int i = 0; i < buffer.Length; i++)
        {
            str = str + buffer[i].ToString("X");
        }
        return str;
    }

    private static byte[] GetMd5Bytes(byte[] bytes)
    {
        return MD5.Create().ComputeHash(bytes);
    }

    public static Assembly LoadDllFile(byte[] codeContext)
    {
        return DecodeDllFile(codeContext);
    }

    private static bool RecoverCode(byte bExType, ref byte[] context)
    {
        switch (bExType)
        {
            case 0:
                return ExchangeContext0(ref context);

            case 1:
                return ExchangeContext1(ref context);

            case 2:
                return ExchangeContext2(ref context);

            case 3:
                return ExchangeContext3(ref context);

            case 4:
                return ExchangeContext4(ref context);

            case 5:
                return ExchangeContext5(ref context);

            case 6:
                return ExchangeContext6(ref context);

            case 7:
                return ExchangeContext7(ref context);

            case 8:
                return ExchangeContext8(ref context);

            case 9:
                return ExchangeContext9(ref context);

            case 10:
                return ExchangeContext10(ref context);

            case 11:
                return ExchangeContext11(ref context);

            case 12:
                return ExchangeContext12(ref context);

            case 13:
                return ExchangeContext13(ref context);

            case 14:
                return ExchangeContext14(ref context);

            case 15:
                return ExchangeContext15(ref context);

            case 0x10:
                return ExchangeContext16(ref context);

            case 0x11:
                return ExchangeContext17(ref context);

            case 0x12:
                return ExchangeContext18(ref context);

            case 0x13:
                return ExchangeContext19(ref context);

            case 20:
                return ExchangeContext20(ref context);

            case 0x15:
                return ExchangeContext21(ref context);

            case 0x16:
                return ExchangeContext22(ref context);

            case 0x17:
                return ExchangeContext23(ref context);

            case 0x18:
                return ExchangeContext24(ref context);

            case 0x19:
                return ExchangeContext25(ref context);

            case 0x1a:
                return ExchangeContext26(ref context);

            case 0x1b:
                return ExchangeContext27(ref context);

            case 0x1c:
                return ExchangeContext28(ref context);

            case 0x1d:
                return ExchangeContext29(ref context);
        }
        return false;
    }
}

