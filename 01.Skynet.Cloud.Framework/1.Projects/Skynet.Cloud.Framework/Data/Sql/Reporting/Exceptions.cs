/*----------------------------------------------------------------
// Copyright (C) 2010 �����������Ƽ����޹�˾
// ��Ȩ���С� 
//
// �ļ�����pivot table�쳣
// �ļ�����������pivot table�쳣
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
	/// pivot table�쳣
	/// </summary>
	public class PivotTableException : ApplicationException
	{
		/// <summary>
        /// ����һ���µ�pivot table�쳣
		/// </summary>
		/// <param name="text"></param>
		public PivotTableException(string text) : base(text) {}
	}

	/// <summary>
    /// pivot Transform�쳣
	/// </summary>
	public class PivotTransformException : ApplicationException 
	{
		/// <summary>
        /// ����pivot Transform�쳣
		/// </summary>
		/// <param name="text"></param>
		public PivotTransformException(string text) : base(text) {}
	}
}
