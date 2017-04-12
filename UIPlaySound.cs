using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom
	}

	public enum SoundMode
	{
		NULL,
		NormalClick,
		ViewClosed,
		Login,
		Award,
		RANDOM,
		SKILL,
		Button,
		GameButton,
		ViewEject,
		TipsEject,
		Dropdown,
		Putaway,
		FunctionEject,
		FunctionIncome,
		FunctionSwitch,
		ForGold,
		DressUp,
		Strengthen,
		JewelSet,
		JewelSynthesis,
		EquipRemove,
		Smelting,
		SkillUpgrade,
		SoulUpgrade,
		ScreenOPen,
		ScreenClose,
		createshow_yijian,
		AddMoney,
		system_button_all_8,
		system_button_all_9,
		createshow_yijian_1,
		createshow_bahuang,
		createshow_bahuang_1,
		createshow_taiqing,
		createshow_taiqing_1,
		Button_Arrange,
		Button_Bottle_of_HP,
		Button_Draw,
		Button_Mix,
		Button_Strengthen,
		Button_Horse,
		Button_Country,
		Button_Skill,
		Button_TenDraw,
		taskcomplete = 50,
		scene_Fail,
		Arena_01,
		system_remind_all_18 = 55,
		system_remind_all_19,
		system_remind_all_20,
		system_remind_all_21,
		system_remind_all_22,
		system_huoban_001 = 61,
		system_remind_shen,
		system_remind_001 = 89,
		system_remind_002,
		system_remind_003,
		system_remind_004,
		system_remind_005,
		system_remind_006,
		Playertalk_01_m = 101,
		Playertalk_01_f = 104,
		Playertalk_02_m = 102,
		Playertalk_02_f = 105,
		Playertalk_03_m = 103,
		Playertalk_03_f = 106,
		system_remind_all_30,
		system_remind_all_31,
		system_remind_all_32,
		system_remind_all_33,
		system_remind_all_34,
		system_remind_all_35,
		system_remind_all_36,
		system_remind_all_37,
		system_remind_all_38,
		system_remind_all_39,
		system_remind_all_40,
		system_remind_all_41,
		Npctalk0007 = 124,
		Npctalk0008,
		Npctalk0009,
		Npctalk0010,
		Npctalk0011,
		Npctalk0012,
		Npctalk0013,
		Npctalk0014,
		Npctalk0015,
		system_remind_item_18 = 137,
		system_remind_all_43 = 139,
		system_remind_all_44,
		system_remind_all_45,
		system_remind_all_50 = 146,
		system_remind_all_51,
		system_ride_015 = 152,
		system_ride_016,
		system_ride_017,
		system_pet_023,
		system_Role_ChangeBoss_Success,
		system_Role_ChangeBoss_Dead
	}

	public UIPlaySound.SoundMode soundMode;

	public AudioClip audioClip;

	public UIPlaySound.Trigger trigger;

	private bool mIsOver;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 2f)]
	public float pitch = 1f;

	private void OnHover(bool isOver)
	{
		if (this.trigger == UIPlaySound.Trigger.OnMouseOver)
		{
			if (this.mIsOver == isOver)
			{
				return;
			}
			this.mIsOver = isOver;
		}
		if (base.enabled && ((isOver && this.trigger == UIPlaySound.Trigger.OnMouseOver) || (!isOver && this.trigger == UIPlaySound.Trigger.OnMouseOut)))
		{
			if (this.soundMode != UIPlaySound.SoundMode.NULL)
			{
				NGUITools.PlaySound((int)this.soundMode);
			}
			else
			{
				NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		if (this.trigger == UIPlaySound.Trigger.OnPress)
		{
			if (this.mIsOver == isPressed)
			{
				return;
			}
			this.mIsOver = isPressed;
		}
		if (base.enabled && ((isPressed && this.trigger == UIPlaySound.Trigger.OnPress) || (!isPressed && this.trigger == UIPlaySound.Trigger.OnRelease)))
		{
			if (this.soundMode != UIPlaySound.SoundMode.NULL)
			{
				NGUITools.PlaySound((int)this.soundMode);
			}
			else
			{
				NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
			}
		}
	}

	private void OnClick()
	{
		if (base.enabled && this.trigger == UIPlaySound.Trigger.OnClick)
		{
			if (this.soundMode != UIPlaySound.SoundMode.NULL)
			{
				NGUITools.PlaySound((int)this.soundMode);
			}
			else
			{
				NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
			}
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	public void Play()
	{
		if (this.soundMode != UIPlaySound.SoundMode.NULL)
		{
			NGUITools.PlaySound((int)this.soundMode);
		}
		else
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}
}
