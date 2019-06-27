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
using UWay.Skynet.Cloud.Data;

namespace UWay.Skynet.Cloud.Data.Reporting
{
	/// <summary>
	/// ͸��ͼ����
	/// </summary>
	public class PivotColumn
	{
		SqlDataType dataType;
		string columnField;
		PivotColumnValueCollection values = new PivotColumnValueCollection();
		
		/// <summary>
		///����һ������
		/// </summary>
		public PivotColumn()
		{
		}

		/// <summary>
        ///����һ������
		/// </summary>
		/// <param name="columnField">�ֶ�����</param>
		/// <param name="dataType">�ֶ�����</param>
		public PivotColumn(string columnField, SqlDataType dataType)
		{
			this.columnField = columnField;
			this.dataType = dataType;
		}

		/// <summary>
		/// �ֶ�����
		/// </summary>
		public string ColumnField 
		{ 
			get { return columnField; } 
			set { columnField = value; } 
		}

		/// <summary>
		/// ֵ
		/// </summary>
		public PivotColumnValueCollection Values
		{ 
			get { return values; } 
		}

		/// <summary>
        /// �ֶ�����
		/// </summary>
		public SqlDataType DataType
		{
			get { return dataType; }
			set { dataType = value; }
		}
	}
}
