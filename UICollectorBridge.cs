using System;
using System.Text;
using UnityEngine;

public class UICollectorBridge : MonoBehaviour
{
	private static UICollectorBridge s_instance;

	public DebugDraw draw;

	private IntPtr clazzPtr = IntPtr.Zero;

	private IntPtr methodPtr = IntPtr.Zero;

	private StringBuilder _sb = new StringBuilder();

	private string msg = string.Empty;

	private UICollectorBridge()
	{
	}

	public static UICollectorBridge GetInstance()
	{
		if (UICollectorBridge.s_instance == null)
		{
			UICollectorBridge.s_instance = (UnityEngine.Object.FindObjectOfType(typeof(UICollectorBridge)) as UICollectorBridge);
			if (!UICollectorBridge.s_instance.Init())
			{
				UICollectorBridge.s_instance = null;
			}
		}
		return UICollectorBridge.s_instance;
	}

	private bool Init()
	{
		AndroidJNIHelper.debug = true;
		this.clazzPtr = AndroidJNI.FindClass("com.snailgames.uicollector.UICollectorBridge");
		if (this.clazzPtr == IntPtr.Zero)
		{
			return false;
		}
		this.methodPtr = AndroidJNI.GetStaticMethodID(this.clazzPtr, "SendUIList", "(Ljava/lang/String;)V");
		return !(this.methodPtr == IntPtr.Zero);
	}

	private void OnDestroy()
	{
		this.Deinit();
	}

	private void Deinit()
	{
		if (this.clazzPtr != IntPtr.Zero)
		{
			AndroidJNI.DeleteLocalRef(this.clazzPtr);
		}
	}

	public void SendUIList(string idsMsg)
	{
		if (idsMsg == null)
		{
			return;
		}
		this._sb.Remove(0, this._sb.Length);
		this._sb.Append(base.gameObject.name);
		this._sb.Append(",");
		this._sb.Append(idsMsg);
		IntPtr l = AndroidJNI.NewStringUTF(this._sb.ToString());
		jvalue jvalue = default(jvalue);
		jvalue.l = l;
		AndroidJNI.CallStaticVoidMethod(this.clazzPtr, this.methodPtr, new jvalue[]
		{
			jvalue
		});
	}

	private void JavaMessage(string message)
	{
	}

	private void DrawArea(string message)
	{
	}

	private void OnGUI()
	{
		GUILayout.Label("���棺 ��ǰ����UI�Զ�����״̬�����ܿ��ܻ��ܵ��ϴ�Ӱ�졣", new GUILayoutOption[0]);
		GUILayout.Label("�رոù��ܷ�����Player Setting --> Other Settings --> ��Scripting Define Symbols��\"__AUTO_TEST__\"ȥ��", new GUILayoutOption[0]);
		GUILayout.Label("msg : " + this.msg, new GUILayoutOption[0]);
	}
}
