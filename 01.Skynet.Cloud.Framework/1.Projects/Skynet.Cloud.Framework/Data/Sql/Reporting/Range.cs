/*----------------------------------------------------------------
// Copyright (C) 2010 �����������Ƽ����޹�˾
// ��Ȩ���С� 
//
// �ļ�����
// �ļ�����������
//
// 
// ������ʶ��
//
// �޸ı�ʶ��
// �޸�������
//
// �޸ı�ʶ��
// �޸�������
//----------------------------------------------------------------*/
using System;

namespace UWay.Skynet.Cloud.Data.Reporting
{

	/// <summary>
	/// Encapsulates a single interval
	/// </summary>
	public class Range
	{
		object lowBound, highBound;

		/// <summary>
		/// Creates a new DataRangeItem
		/// </summary>
		public Range()
		{
		}

		/// <summary>
		/// Creates a new DataRangeItem
		/// </summary>
		/// <param name="low">The low bound of the interval</param>
		/// <param name="high">The high bound of the interval</param>
		public Range(object low, object high)
		{
			this.lowBound = low;
			this.highBound = high;
		}

		/// <summary>
		/// Gets or sets the low bound of the interval
		/// </summary>
		public object LowBound
		{
			get { return lowBound; }
			set { lowBound = value; }
		}

		/// <summary>
		/// Gets or sets the high bound of the interval
		/// </summary>
		public object HighBound
		{
			get { return highBound; }
			set { highBound = value; }
		}
	}
}
