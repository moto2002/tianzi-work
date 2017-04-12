using System;
using System.Runtime.InteropServices;

public class UnityVerify
{
	public static IntPtr fxVerifyinstance = IntPtr.Zero;

	public static IntPtr fxVerifyType = IntPtr.Zero;

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr FxVerify_GetType();

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr FxVerify_GetInterface();

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern int FxVerify_GetVersion(IntPtr pInstance);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_SetClientCond(IntPtr pInstance, byte[] cond);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_SetClientCode(IntPtr pInstance, byte[] cond, int nLen);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_GetRetEncodeVerify(IntPtr pInstance, string strIp, int nDynamicKey, int nPort, byte[] strInfo, int nStrInfoLen, string strDecID, int nAddress, IntPtr output);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_GetLoginVerify(IntPtr pInstance, byte[] strInfo, int nStrInfoLen, int nPort, string strIp, string strDecID, int nAddress, int nDynamicKey, IntPtr output);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_GetChooseRoleVerify(IntPtr pInstance, byte[] strInfo, int nStrInfoLen, string strIp, int nPort, string strDecID, int nDynamicKey, byte[] szRoleName, int nRoleNameLen, int nAddress, IntPtr output);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_GetSelectVerify(IntPtr pInstance, int nDynamicKey, int nFunctionId, string strIp, int nPort, string strDecID, int nPersistidSerial, int nSerial, int nPersistidIdent, IntPtr output);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern void FxVerify_GetCustomVerify(IntPtr pInstance, string strIp, int nDynamicKey, int nPort, int nSerial, int nArgNum, string strDecID, byte[] pArgData, int nArgLen, IntPtr output);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool FxVerify_EncodeAccount(IntPtr pInstance, string strAccount, IntPtr OutBuffer, ref int nSize);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern bool FxVerify_EncodePassword(IntPtr pInstance, string strPassword, IntPtr OutBuffer, ref int nSize);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr FxVerify_Release(IntPtr pInstance);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr FxVerify_AntiDebuggerCheck(IntPtr pInstance);

	[DllImport("ClientVerify", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr FxVerify_AntiDebuggerStop(IntPtr pInstance);

	public static void UVFxVerify_GetType()
	{
		if (UnityVerify.fxVerifyType == IntPtr.Zero)
		{
			UnityVerify.fxVerifyType = UnityVerify.FxVerify_GetType();
		}
	}

	public static void UVFxVerify_GetInterface()
	{
		try
		{
			UnityVerify.fxVerifyinstance = UnityVerify.FxVerify_GetInterface();
		}
		catch (Exception ex)
		{
			UnityVerify.fxVerifyinstance = IntPtr.Zero;
			LogSystem.LogError(new object[]
			{
				ex.ToString()
			});
		}
	}

	public static int UVFxVerify_GetVersion()
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			return UnityVerify.FxVerify_GetVersion(UnityVerify.fxVerifyinstance);
		}
		return 768;
	}

	public static void UVFxVerify_fnSetClientCond(byte[] cond)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			UnityVerify.FxVerify_SetClientCond(UnityVerify.fxVerifyinstance, cond);
		}
	}

	public static void UVFxVerify_fnSetClientCode(byte[] cond, int nLen)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			UnityVerify.FxVerify_SetClientCode(UnityVerify.fxVerifyinstance, cond, nLen);
		}
	}

	public static void UVFxVerify_GetRetEncodeVerify(string strIp, int nDynamicKey, int nPort, byte[] strInfo, int nStrInfoLen, string strDecID, int nAddress, ref byte[] output)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(16);
			UnityVerify.FxVerify_GetRetEncodeVerify(UnityVerify.fxVerifyinstance, strIp, nDynamicKey, nPort, strInfo, nStrInfoLen, strDecID, nAddress, intPtr);
			Marshal.Copy(intPtr, output, 0, 16);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static void UVFxVerify_GetLoginVerify(byte[] strInfo, int nStrInfoLen, int nPort, string strIp, string strDecID, int nAddress, int nDynamicKey, ref byte[] output)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			try
			{
				IntPtr intPtr = Marshal.AllocHGlobal(16);
				UnityVerify.FxVerify_GetLoginVerify(UnityVerify.fxVerifyinstance, strInfo, nStrInfoLen, nPort, strIp, strDecID, nAddress, nDynamicKey, intPtr);
				Marshal.Copy(intPtr, output, 0, 16);
				Marshal.FreeHGlobal(intPtr);
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					ex.ToString()
				});
			}
		}
	}

	public static void UVFxVerify_GetChooseRoleVerify(byte[] strInfo, int nStrInfoLen, string strIp, int nPort, string strDecID, int nDynamicKey, byte[] szRoleName, int nRoleNameLen, int nAddress, ref byte[] output)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(16);
			UnityVerify.FxVerify_GetChooseRoleVerify(UnityVerify.fxVerifyinstance, strInfo, nStrInfoLen, strIp, nPort, strDecID, nDynamicKey, szRoleName, nRoleNameLen, nAddress, intPtr);
			Marshal.Copy(intPtr, output, 0, 16);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static void UVFxVerify_GetSelectVerify(int nDynamicKey, int nFunctionId, string strIp, int nPort, string strDecID, int nPersistidSerial, int nSerial, int nPersistidIdent, ref byte[] output)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(16);
			UnityVerify.FxVerify_GetSelectVerify(UnityVerify.fxVerifyinstance, nDynamicKey, nFunctionId, strIp, nPort, strDecID, nPersistidSerial, nSerial, nPersistidIdent, intPtr);
			Marshal.Copy(intPtr, output, 0, 16);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static void UVFxVerify_GetCustomVerify(string strIp, int nDynamicKey, int nPort, int nSerial, int nArgNum, string strDecID, byte[] pArgData, int nArgLen, ref byte[] output)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(16);
			UnityVerify.FxVerify_GetCustomVerify(UnityVerify.fxVerifyinstance, strIp, nDynamicKey, nPort, nSerial, nArgNum, strDecID, pArgData, nArgLen, intPtr);
			Marshal.Copy(intPtr, output, 0, 16);
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public static bool UVFxVerify_EncodeAccount(string strAccount, ref byte[] output, ref int nSize)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(strAccount.Length + 16 + 4);
			nSize = strAccount.Length + 16 + 4;
			bool result = UnityVerify.FxVerify_EncodeAccount(UnityVerify.fxVerifyinstance, strAccount, intPtr, ref nSize);
			output = new byte[nSize + 4];
			Marshal.Copy(intPtr, output, 0, nSize);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}
		return false;
	}

	public static bool UVFxVerify_EncodePassword(string strPassword, ref byte[] output, ref int nSize)
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(strPassword.Length + 16);
			nSize = strPassword.Length + 16;
			bool result = UnityVerify.FxVerify_EncodePassword(UnityVerify.fxVerifyinstance, strPassword, intPtr, ref nSize);
			output = new byte[nSize + 4];
			Marshal.Copy(intPtr, output, 0, nSize);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}
		return false;
	}

	public static void UVFxVerify_Release()
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			UnityVerify.FxVerify_Release(UnityVerify.fxVerifyinstance);
			UnityVerify.fxVerifyinstance = IntPtr.Zero;
			UnityVerify.fxVerifyType = IntPtr.Zero;
		}
	}

	public static void UVFxVerify_AntiDebuggerCheck()
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			UnityVerify.FxVerify_AntiDebuggerCheck(UnityVerify.fxVerifyinstance);
		}
	}

	public static void UVFxVerify_AntiDebuggerStop()
	{
		if (UnityVerify.fxVerifyinstance != IntPtr.Zero)
		{
			UnityVerify.FxVerify_AntiDebuggerStop(UnityVerify.fxVerifyinstance);
		}
	}
}
